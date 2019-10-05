using CodeRunner.Extensions;
using CodeRunner.Loggings;

namespace CodeRunner.Test.Mocks
{
    public class TestHost : IHost
    {
        public TestHost(LoggerScope scope) => Scope = scope;

        private LoggerScope Scope { get; }

        public virtual void Restart() => Scope.Information("Restart request");

        public virtual void SendMessage(string message) => Scope.Information("Recieved message: " + message);

        public virtual void Shutdown() => Scope.Information("Shutdown request");
    }
}
