using CodeRunner.Diagnostics;
using CodeRunner.Extensions;
using CodeRunner.Extensions.Helpers;
using CodeRunner.Loggings;
using CodeRunner.Managements;
using CodeRunner.Pipelines;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Builder = CodeRunner.Pipelines.PipelineBuilder<string[], CodeRunner.Pipelines.Wrapper<int>>;

namespace CodeRunner.Test.Commands
{
    public static class PipelineGenerator
    {
        public static Command CombineCommands(params Command[] commands)
        {
            RootCommand cmd = new RootCommand();
            foreach (Command v in commands) cmd.AddCommand(v);
            return cmd;
        }

        public static readonly PipelineOperation<string[], Wrapper<int>> InitializeWorkspace = async context =>
        {
            IWorkspace workspace = context.Services.GetWorkspace();
            await workspace.Initialize();
            return 0;
        };

        public static Parser CreateDefaultParser(Command command, PipelineContext context) => CreateParserBuilder(command, context)
            .UseDefaults()
            .Build();

        public static CommandLineBuilder CreateParserBuilder(Command command, PipelineContext context) => new CommandLineBuilder(command)
            .UseMiddleware(inv => inv.BindingContext.AddService(typeof(PipelineContext), () => context));

        public static Builder ConfigureConsole(this Builder builder, IConsole console) => builder.Configure(nameof(ConfigureConsole),
            scope => scope.Add<IConsole>(console));

        public static Builder ConfigureInput(this Builder builder, TextReader input) => builder.Configure(nameof(ConfigureInput),
            scope => scope.Add<TextReader>(input));

        public static Builder ConfigureWorkspace(this Builder builder, IWorkspace workspace) => builder.Configure(nameof(ConfigureWorkspace),
            scope => scope.Add<IWorkspace>(workspace));

        internal static Builder ConfigureLogger(this Builder builder, ILogger logger) => builder.Configure(nameof(ConfigureLogger),
            scope => scope.Add<ILogger>(logger));

        public static Builder ConfigureHost(this Builder builder, IHost host) => builder.Configure(nameof(ConfigureHost),
            scope => scope.Add<IHost>(host));

        public static async Task<PipelineResult<Wrapper<int>>> UseSampleCommandInvoker(this Builder builder, Command command, string[] origin, string input = "", ILogger? logger = null, Func<PipelineContext<string[], Wrapper<int>>, Task>? before = null, Func<PipelineContext<string[], Wrapper<int>>, Task>? after = null)
        {
            Assert.ArgumentNotNull(input, nameof(input));

            ILogger rlogger = logger ?? new Logger();
            _ = builder.ConfigureLogger(rlogger);

            using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
            using StreamReader sr = new StreamReader(ms);
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
                Parser parser = CreateDefaultParser(command, context);
                return await parser.InvokeAsync(context.Origin, context.Services.GetConsole());
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
