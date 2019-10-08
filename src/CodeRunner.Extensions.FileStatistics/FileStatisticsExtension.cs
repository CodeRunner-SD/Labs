using CodeRunner.Extensions;
using System;

[assembly: EntryExtension(typeof(CodeRunner.Extensions.FileStatistics.FileStatisticsExtension))]

namespace CodeRunner.Extensions.FileStatistics
{
    public class FileStatisticsExtension : IExtension
    {
        public string Name => "FileStatistics";

        public string Publisher => "CodeRunner";

        public string Description => "Add file and code statistics commands.";

        public Version Version => new Version(0, 0, 1);
    }
}
