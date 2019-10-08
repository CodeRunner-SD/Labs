using CodeRunner.Pipelines;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;

namespace CodeRunner.Test.Commands
{
    public static class CommandAdapter
    {
        public static Command CombineCommands(params Command[] commands)
        {
            RootCommand cmd = new RootCommand();
            foreach (Command v in commands) cmd.AddCommand(v);
            return cmd;
        }

        public static Parser BuildDefault(this CommandLineBuilder builder) => builder
            .UseDefaults()
            .Build();

        public static CommandLineBuilder UsePipelineContext(this CommandLineBuilder builder, PipelineContext context) => builder.UseMiddleware(inv => inv.BindingContext.AddService(typeof(PipelineContext), () => context));

        public static CommandLineBuilder UseParserContext(this CommandLineBuilder builder, CodeRunner.Commands.ParserContext context) => builder.UseMiddleware(inv => inv.BindingContext.AddService(typeof(CodeRunner.Commands.ParserContext), () => context));

        public static Command Transform(this CodeRunner.Commands.Command command)
        {
            Command res = new Command(command.Name, command.Description)
            {
                TreatUnmatchedTokensAsErrors = false
            };
            foreach (string v in command.Aliases)
                res.AddAlias(v);
            foreach (CodeRunner.Commands.Command v in command.Commands)
                res.AddCommand(v.Transform());
            foreach (CodeRunner.Commands.Option v in command.Options)
                res.AddOption(v.Transform());
            foreach (CodeRunner.Commands.Argument v in command.Arguments)
                res.AddArgument(v.Transform());
            res.Handler = CommandHandler.Create(command.Handler);
            return res;
        }

        public static Option Transform(this CodeRunner.Commands.Option option)
        {
            Option res = new Option(option.Name, option.Description);
            foreach (string v in option.Aliases)
                res.AddAlias(v);
            if (option.Argument != null)
                res.Argument = option.Argument.Transform();
            return res;
        }

        public static Argument Transform(this CodeRunner.Commands.Argument argument)
        {
            Argument res = new Argument(argument.Name)
            {
                Description = argument.Description,
                ArgumentType = argument.ArgumentType,
                Arity = new ArgumentArity(argument.Arity.MinimumNumberOfValues, argument.Arity.MaximumNumberOfValues)
            };
            res.SetDefaultValue(argument.DefaultValue);
            return res;
        }
    }
}
