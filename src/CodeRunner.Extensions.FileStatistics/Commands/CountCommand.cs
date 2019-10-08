using CodeRunner.Commands;
using CodeRunner.Extensions.Commands;
using CodeRunner.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace CodeRunner.Extensions.FileStatistics.Commands
{
    [Export]
    public class CountCommand : BaseCommand<CountCommand.CArgument>
    {
        public override string Name => "count";

        public override Command Configure()
        {
            Command res = new Command("count", "Count file content.");
            return res;
        }

        public override Task<int> Handle(CArgument argument, ParserContext parser, PipelineContext pipeline, CancellationToken cancellationToken) => Task.FromResult(0);

        public class CArgument
        {
        }
    }
}
