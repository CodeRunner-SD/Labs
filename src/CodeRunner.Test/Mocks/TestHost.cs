using CodeRunner.Extensions;
using CodeRunner.Loggings;

namespace CodeRunner.Test.Mocks
{
    public class TestHost : IHost
    {
        public TestHost(LoggerScope scope) => Scope = scope;

        private LoggerScope Scope { get; }

        public virtual void Restart() => Scope.Invoked("Restart request");

        public virtual void SendMessage(string message) => Scope.Invoked("Recieved message: " + message);

        public virtual void Shutdown() => Scope.Invoked("Shutdown request");
    }
}
