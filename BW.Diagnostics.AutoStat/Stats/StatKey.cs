namespace BW.Diagnostics.StatCollection.Stats
{
    struct StatKey
    {
        public readonly string MemberName;
        public readonly string StatName;

        public StatKey(string memberName, string statName)
        {
            MemberName = memberName;
            StatName = statName;
        }
        // Equals and GetHashCode ommitted
    }
}
