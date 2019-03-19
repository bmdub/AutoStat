using BW.Diagnostics.StatCollection.Stats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// todo: 
// support fixed-size array as trecord
// support external key?

namespace BW.Diagnostics.StatCollection
{
    /// <summary>
    /// Records statistics for a stream or collection of objects.
    /// </summary>
    /// <typeparam name="TRECORD">The type of the class to monitor.</typeparam>
    public partial class AutoStat<TRECORD>
    {
        /// <summary>The current count of records sampled.</summary>
        public long Count { get; private set; }

        Action<TRECORD> _collectAction;
        List<IStatCollector> _statCollectors = new List<IStatCollector>();
        Configuration _configuration;
        string _keyName;

        /// <summary></summary>
        /// <param name="configuration">Optional configuration.</param>
        /// <param name="keyName">Optional name of the property to key on. Required for sample comparison.</param>
        public AutoStat(Configuration configuration = null, string keyName = null)
        {
            _configuration = configuration;
            _keyName = keyName;

            Init();
        }

        void Init()
        {
            Count = 0;
            _statCollectors.Clear();

            if (_configuration == null)
                _configuration = new Configuration(SelectionMode.All);

            var recordParameter = Expression.Parameter(typeof(TRECORD));
            var variables = new List<ParameterExpression>();
            List<Expression> expressions = new List<Expression>();

            // Get the key column / hash function
            ParameterExpression keyHashVariable = Expression.Variable(typeof(ulong));
            variables.Add(keyHashVariable);
            foreach (PropertyInfo propertyInfo in typeof(TRECORD).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (string.Compare(propertyInfo.Name, _keyName, true) != 0) continue;

                if (!propertyInfo.CanRead) { continue; }
                MethodInfo getMethod = propertyInfo.GetGetMethod(false);
                if (getMethod == null) { continue; }

                MethodCallExpression getterCall = Expression.Call(recordParameter, getMethod);
                var keyVariable = Expression.Variable(propertyInfo.PropertyType);
                variables.Add(keyVariable);
                expressions.Add(Expression.Assign(keyVariable, getterCall));

                var genericType = typeof(Hasher<>).MakeGenericType(propertyInfo.PropertyType);

                var constructor = genericType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

                var hasher = Activator.CreateInstance(genericType);

                var hashMethod = hasher.GetType().GetMethod(nameof(Hasher<bool>.GetHash), new[] { propertyInfo.PropertyType });

                var hashCall = Expression.Call(Expression.Constant(hasher), hashMethod, keyVariable);
                expressions.Add(hashCall);

                expressions.Add(Expression.Assign(keyHashVariable, hashCall));

                break;
            }

            // Search for properties in the given type to collect stats on.
            foreach (PropertyInfo propertyInfo in typeof(TRECORD).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!propertyInfo.CanRead) { continue; }
                MethodInfo getMethod = propertyInfo.GetGetMethod(false);
                if (getMethod == null) { continue; }

                MethodCallExpression getterCall = Expression.Call(recordParameter, getMethod);
                var valueVariable = Expression.Variable(propertyInfo.PropertyType);
                variables.Add(valueVariable);
                expressions.Add(Expression.Assign(valueVariable, getterCall));

                // Create stat collectors for each property, when applicable.
                List<IStatCollector> memberStatCollectors = new List<IStatCollector>();
                foreach (var statType in _configuration.StatCollectorTypes)
                {
                    if (_configuration.SelectionMode == SelectionMode.Attribute)
                        if (!Attribute.IsDefined(propertyInfo, typeof(AutoStatAttribute)))
                            continue;

                    if (!statType.IsGenericTypeDefinition)
                        throw new ArgumentException($"Type {statType} is not a generic type definition. ex. 'SomeStat<>'");

                    if (!typeof(IStatCollector).IsAssignableFrom(statType))
                        throw new ArgumentException($"Type {statType} does not implement interface '{nameof(IStatCollector)}<T>'."); // (IStat)                 

                    var genericArgument = statType.GetGenericArguments().FirstOrDefault();
                    if (genericArgument == null)
                        throw new ArgumentException($"Type {statType} does not implement interface '{nameof(IStatCollector)}<T>'."); // (IStat)   

                    Type genericStatType;
                    try
                    {
                        genericStatType = statType.MakeGenericType(propertyInfo.PropertyType);
                    }
                    catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                    {
                        // Could not create the stat with this type as the generic parameter; it likely does not meet the constraint.
                        // Ignore collecting a stat for a type that cannot be collected.
                        continue;
                    }

                    try
                    {
                        var constructor = genericStatType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();
                        if (constructor == null)
                            Debugger.Break();

                        var parameters = constructor.GetParameters().ToList();
                        if (parameters.Count != 3 ||
                            parameters[0].ParameterType != typeof(string) ||
                            !typeof(Configuration).IsAssignableFrom(parameters[1].ParameterType) ||
                            parameters[2].ParameterType != Type.GetType("System.Boolean&"))
                        {
                            throw new NotImplementedException($"No suitable constructor has been implemented for {genericStatType.Name}. Constructors must accept a first parameter (memberName) of type 'string', a second parameter of type '{nameof(Configuration)}', and a third parameter of type 'boolean'.");
                        }

                        object[] args = new object[] { propertyInfo.Name, _configuration, false };

                        var statCollector = Activator.CreateInstance(genericStatType, args) as IStatCollector;
                        //var statCollector = CreateInstanceViaExpressions(genericStatType) as IStat;

                        if ((bool)args[2] == false)
                            memberStatCollectors.Add(statCollector);
                        // else canceled
                    }
                    catch (NotImplementedException)
                    {
                        // noop
                    }
                }

                // Use expressions to dynamically generate the code that will be called to collect stats for each member for each stat collector.
                for (int i = 0; i < memberStatCollectors.Count; i++)
                {
                    var addValueMethod = memberStatCollectors[i].GetType().GetMethod(nameof(IStatCollector<bool>.AddValue), new[] { typeof(ulong).UnderlyingSystemType, propertyInfo.PropertyType });
                    if (addValueMethod == null)
                        throw new ArgumentException($"Type {memberStatCollectors[i].GetType()} does not implement interface '{nameof(IStatCollector)}<T>'."); // (IStat<T>)  
                    var addValueCall = Expression.Call(Expression.Constant(memberStatCollectors[i]), addValueMethod, keyHashVariable, valueVariable);
                    expressions.Add(addValueCall);
                }

                _statCollectors.AddRange(memberStatCollectors);
            }

            _collectAction = Expression.Lambda<Action<TRECORD>>(Expression.Block(variables, expressions), recordParameter).Compile();
        }

        /// <summary>Clears the gathered statistics.</summary>
        public void Reset()
        {
            // TODO
            // We can be more efficient than re-initializing.
            Init();
        }

        /// <summary>Add the given record to the statistics.</summary>
        /// <param name="record"></param>
        public void Collect(TRECORD record)
        {
            _collectAction(record);
            Count++;
        }

        /// <summary>Add the given records to the statistics.</summary>
        /// <param name="records"></param>
        public void Collect(IEnumerable<TRECORD> records)
        {
            foreach (var record in records)
                Collect(record);
        }

        /// <summary>Calculates and returns statistics for the records collected up to this point.</summary>
        /// <returns></returns>
        public RecordStats<IStat> GetStats()
        {
            RecordStats<IStat> recordStats = new RecordStats<IStat>();

            foreach (var group in _statCollectors.GroupBy(collector => collector.MemberName))
            {
                recordStats.Add(group.SelectMany(collector => collector.GetStats()));
            }

            return recordStats;
        }

        /// <summary>Calculates and returns statistics for two sets of records, and returns the comparison of the two sets of statistics.</summary>
        /// <returns></returns>
        public RecordStats<IComparedStat> GetStatsComparedTo(AutoStat<TRECORD> otherAutoStat)
        {
            RecordStats<IComparedStat> recordStats = new RecordStats<IComparedStat>();

            var collectorPairs = _statCollectors.Join(otherAutoStat._statCollectors, collector => collector.GetKey(), collector => collector.GetKey(), (collector1, collector2) => (collector1, collector2));

            foreach (var collectorPair in collectorPairs)
            {
                recordStats.Add(collectorPair.collector1.GetStatsComparedTo(collectorPair.collector2));
            }

            return recordStats;
        }

        /*static object CreateInstanceViaExpressions(Type type)
        {
            var createdType = type;
            var ctor = Expression.New(createdType);
            var memberInit = Expression.MemberInit(ctor);

            var lambda = Expression.Lambda<Func<object>>(memberInit).Compile();
            return lambda();
        }*/
    }
}
