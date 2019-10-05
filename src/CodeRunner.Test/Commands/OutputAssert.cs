using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.CommandLine;

namespace CodeRunner.Test.Commands
{
    public static class OutputAssert
    {
        public static void OutputContains(this IConsole result, string data) => StringAssert.Contains(result.Out.ToString(), data);

        public static void ErrorContains(this IConsole result, string data) => StringAssert.Contains(result.Error.ToString(), data);
    }
}
