using CodeRunner.Diagnostics;
using CodeRunner.Loggings;
using System.Runtime.CompilerServices;

namespace CodeRunner.Test
{
    public static class LoggerExtensions
    {
        public static void Invoked(this LoggerScope scope, string content = "", [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0) => scope.Log($"Invoked: {content}", LogLevel.Debug, memberName, sourceFilePath, sourceLineNumber);

        public static void AssertInvoked(this Logger logger, string name)
        {
            foreach (LogItem v in logger.View())
            {
                if (v.Content.Contains("Invoke"))
                {
                    if (v.Content.Contains(name))
                        return;
                }
            }
            Assert.Fail($"{name} not invoked");
        }
    }
}
