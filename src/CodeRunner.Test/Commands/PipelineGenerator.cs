using CodeRunner.Commands;
using CodeRunner.Diagnostics;
using CodeRunner.Extensions;
using CodeRunner.Extensions.Helpers;
using CodeRunner.Loggings;
using CodeRunner.Managements;
using CodeRunner.Pipelines;
using CodeRunner.Test.Mocks;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Builder = CodeRunner.Pipelines.PipelineBuilder<string[], CodeRunner.Pipelines.Wrapper<int>>;

namespace CodeRunner.Test.Commands
{
    public static class PipelineGenerator
    {
        public static readonly PipelineOperation<string[], Wrapper<int>> InitializeWorkspace = async context =>
        {
            IWorkspace workspace = context.Services.GetWorkspace();
            await workspace.Initialize();
            return 0;
        };

        public static Builder ConfigureConsole(this Builder builder, IConsole console) => builder.Configure(nameof(ConfigureConsole),
            scope => scope.Add<IConsole>(console));

        public static Builder ConfigureTerminal(this Builder builder, Extensions.Terminals.ITerminal terminal) => builder.Configure(nameof(ConfigureTerminal),
            scope => scope.Add<Extensions.Terminals.ITerminal>(terminal));

        public static Builder ConfigureWorkspace(this Builder builder, IWorkspace workspace) => builder.Configure(nameof(ConfigureWorkspace),
            scope => scope.Add<IWorkspace>(workspace));

        internal static Builder ConfigureLogger(this Builder builder, ILogger logger) => builder.Configure(nameof(ConfigureLogger),
            scope => scope.Add<ILogger>(logger));

        public static Builder ConfigureHost(this Builder builder, IHost host) => builder.Configure(nameof(ConfigureHost),
            scope => scope.Add<IHost>(host));

        public static Builder CreateBuilder() => new Builder();

        public static async Task<PipelineResult<Wrapper<int>>> UseSampleCommandInvoker(this Builder builder, CodeRunner.Commands.Command command, string[] origin, string input = "", IConsole? console = null, ILogger? logger = null, IHost? host = null, IWorkspace? workspace = null, Func<PipelineContext<string[], Wrapper<int>>, Task>? before = null, Func<PipelineContext<string[], Wrapper<int>>, Task>? after = null)
        {
            Assert.ArgumentNotNull(input, nameof(input));

            ILogger rlogger = logger ?? new Logger();
            IConsole rconsole = console ?? new System.CommandLine.Rendering.TestTerminal();
            IHost rhost = host ?? new TestHost(rlogger.CreateScope("host", LogLevel.Debug));
            IWorkspace rws = workspace ?? new TestWorkspace(rlogger.CreateScope("ws", LogLevel.Debug));

            using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
            using StreamReader sr = new StreamReader(ms);

            _ = builder.ConfigureLogger(rlogger)
                       .ConfigureHost(rhost)
                       .ConfigureWorkspace(rws)
                       .ConfigureConsole(rconsole)
                       .ConfigureTerminal(new Mocks.TestTerminal(rconsole.GetTerminal(), sr));

            if (before != null)
            {
                _ = builder.Use("before", async context =>
                {
                    await before(context);
                    return context.IgnoreResult();
                });
            }
            _ = builder.Use("main", async context =>
            {
                Parser firstParser = new CommandLineBuilder(command.Transform()).BuildDefault();
                ParseResult result = firstParser.Parse(context.Origin);
                Parser parser = new CommandLineBuilder(command.Transform()).UsePipelineContext(context)
                                                                           .UseParserContext(new ParserContext(result.UnparsedTokens, result.UnmatchedTokens))
                                                                           .BuildDefault();
                return await parser.InvokeAsync(context.Origin, context.Services.GetService<IConsole>());
            });
            if (after != null)
            {
                _ = builder.Use("after", async context =>
                {
                    await after(context);
                    return context.IgnoreResult();
                });
            }

            return await (await builder.Build(origin, rlogger)).Consume();
        }
    }
}
