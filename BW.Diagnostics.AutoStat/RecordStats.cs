using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BW.Diagnostics.StatCollection.Stats;

namespace BW.Diagnostics.StatCollection
{
    /// <summary>
    /// Represents a list of stat collection results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class RecordStats<T> : IEnumerable<T> where T : IStat
    {
        Dictionary<StatKey, T> _stats = new Dictionary<StatKey, T>();
        HashSet<string> _memberNames = new HashSet<string>();
        HashSet<string> _statNames = new HashSet<string>();
        int _maxMemberNameLength;
        int _maxStatNameLength;

        internal RecordStats() { }

        internal RecordStats(IEnumerable<T> stats)
        {
            Add(stats);
        }

        internal void Add(IEnumerable<T> stats)
        {
            foreach (var stat in stats)
                Add(stat);
        }

        internal void Add(T stat)
        {
            _stats.Add(stat.GetKey(), stat);
            _memberNames.Add(stat.MemberName);
            _statNames.Add(stat.Name);
            _maxMemberNameLength = Math.Max(_maxMemberNameLength, stat.MemberName.Length);
            _maxStatNameLength = Math.Max(_maxStatNameLength, stat.Name.Length);
        }

        /// <summary></summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _stats.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Allows the user to choose under what circumstances to highlight a significant stat when output.
        /// </summary>
        /// <param name="predicate">Function that indicates when a stat(s) are to be noted as significant.</param>
        /// <returns></returns>
        public RecordStats<T> HighlightWhen(Func<T, bool> predicate)
        {
            foreach(var stat in this)
                if (predicate(stat)) stat.StringValue += "**";

            return this;
        }

        /// <summary>
        /// Returns the statistics in a simple list format.
        /// </summary>
        /// <returns></returns>
        public string ToTextFormat()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var memberName in _memberNames)
            {
                var stats = _stats.Values.Where(stat => stat.MemberName == memberName).ToList();

                if (stats.Count == 0) continue;

                stringBuilder.AppendLine();

                foreach (var stat in stats)
                    stringBuilder.AppendLine($"{(stat.MemberName + ":").PadRight(_maxMemberNameLength + 1)} {(stat.Name + ":").PadRight(_maxStatNameLength + 1)} {stat.StringValue}");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Outputs the statistics to a .csv file.
        /// </summary>
        /// <param name="outputPath"></param>
        public void ToCsvFile(string outputPath)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (_statNames.Count == 0) return;

            stringBuilder.AppendLine("\"Stats\"," + string.Join(",", _memberNames.Select(name => $"\"{name}\"")));
            
            foreach (var statName in _statNames)
            {
                stringBuilder.Append($"\"{statName}\",");

                stringBuilder.Append(string.Join(",", _memberNames.Select(memberName =>
                {
                    if (_stats.TryGetValue(new StatKey(memberName, statName), out T stat))
                        return $"\"{stat.StringValue}\"";
                    else
                        return "\"\"";
                })));

                stringBuilder.AppendLine();
            }

            File.WriteAllText(outputPath, stringBuilder.ToString());
        }

        /// <summary>
        /// Opens a .csv file in a special Powershell window.
        /// </summary>
        /// <param name="csvOutputPath"></param>
        public void OpenCsvInPowershell(string csvOutputPath)
        {
            ToCsvFile(csvOutputPath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
                throw new InvalidOperationException("Powershell not supported on this platform.");

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $@"/C start powershell -windowstyle hidden -NoExit -Command ""Import-Csv {csvOutputPath} | Out-GridView"""
            };
            process.StartInfo = startInfo;
            process.Start();
        }

        /// <summary>
        /// Returns the statistics in a table-like format.
        /// </summary>
        /// <param name="maxWidth">The maximum character width of the table before performing wrapping.</param>
        /// <returns></returns>
        public string ToTextTableFormat(int maxWidth = int.MaxValue)
        {
            maxWidth -= 2; // Add some overall padding.

            int hPadding = 2;
            int vPadding = 2;

            int maxPaddedStatNameLength = _maxStatNameLength + 1 + hPadding;
            var statNamesPadded = _statNames.Select(name => (name + '|').PadLeft(maxPaddedStatNameLength - hPadding).PadRight(maxPaddedStatNameLength)).ToList();

            List<(int width, int height, List<string> lines)> statsOutputs = new List<(int width, int height, List<string> lines)>();

            var statsGroupsByMemberName = _stats.Values.GroupBy(stat => stat.MemberName).Select(group => group);

            foreach (var statsGroupByMemberName in statsGroupsByMemberName)
            {
                int maxStringValueLength = statsGroupByMemberName.Where(stat => stat.StringValue != null).Select(stat => stat.StringValue.Length).Max();
                maxStringValueLength = Math.Max(maxStringValueLength, statsGroupByMemberName.Key.Length);

                List<string> lines = new List<string>();
                lines.Add(statsGroupByMemberName.Key.PadRight(maxStringValueLength + hPadding));
                lines.Add(new string('-', maxStringValueLength).PadRight(maxStringValueLength + hPadding));
                foreach (var statName in _statNames)
                {
                    var stat = statsGroupByMemberName.Where(innerStat => statName == innerStat.Name).FirstOrDefault();

                    if (stat?.Name == null) lines.Add(new string(' ', maxStringValueLength + hPadding));
                    else lines.Add($"{stat.StringValue?.PadRight(maxStringValueLength + hPadding)}");
                }
                for (int i = 0; i < vPadding; i++) lines.Add("");

                statsOutputs.Add((maxStringValueLength + hPadding, lines.Count, lines));
            }

            statNamesPadded.Insert(0, new string(' ', maxPaddedStatNameLength));
            statNamesPadded.Insert(0, new string(' ', maxPaddedStatNameLength));

            StringBuilder stringBuilder = new StringBuilder();

            int widthLeft = maxWidth - maxPaddedStatNameLength;
            List<string> currentLines = new List<string>(statNamesPadded);

            foreach (var statsOutput in statsOutputs)
            {
                if (statsOutput.width > widthLeft && statsOutput.width < maxWidth)
                {
                    foreach (var line in currentLines) stringBuilder.AppendLine(line);
                    currentLines = new List<string>(statNamesPadded);
                    widthLeft = maxWidth - maxPaddedStatNameLength;
                }

                for (int i = 0; i < statsOutput.lines.Count; i++)
                {
                    if (currentLines.Count <= i) currentLines.Add(statsOutput.lines[i]);
                    else currentLines[i] += statsOutput.lines[i];
                }

                widthLeft -= statsOutput.width;
            }
            foreach (var line in currentLines) stringBuilder.AppendLine(line);

            return stringBuilder.ToString();
        }
    }
}
