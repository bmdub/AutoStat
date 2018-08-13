namespace BW.Diagnostics.StatCollection.Stats
{
    struct StatCollectorKey
    {
        public readonly string MemberName;
        public readonly string TypeName;

        public StatCollectorKey(string memberName, string typeName)
        {
            MemberName = memberName;
            TypeName = typeName;
        }
        // Equals and GetHashCode ommitted
    }
}
