using System.Threading.Tasks;

namespace Test.Host
{
    internal class Program
    {
        private static Task<int> Main(string[] args)
        {
            CodeRunner.Test.ExtensionDI.ExtensionAssemblies.Add(typeof(CodeRunner.Extensions.FileStatistics.FileStatisticsExtension).Assembly);
            return CodeRunner.Program.Main(args);
        }
    }
}
