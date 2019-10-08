using CodeRunner.Diagnostics;
using CodeRunner.Extensions.Terminals;
using CodeRunner.Extensions.Terminals.Rendering;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.IO;

namespace CodeRunner.Test.Mocks
{
    public class TestTerminal : CodeRunner.Extensions.Terminals.ITerminal
    {
        private class StandardReaderWrapper : IStandardReader
        {
            public StandardReaderWrapper(TextReader inner) => Inner = inner;

            private TextReader Inner { get; }

            public string? ReadLine() => Inner.ReadLine();
        }

        private class StandardWriterWrapper : IStandardWriter
        {
            public StandardWriterWrapper(System.CommandLine.Rendering.ITerminal terminal, System.CommandLine.IStandardStreamWriter inner, CodeRunner.Extensions.Terminals.ITerminal parent)
            {
                Terminal = terminal;
                Inner = inner;
                Parent = parent;
            }

            private System.CommandLine.Rendering.ITerminal Terminal { get; }

            private System.CommandLine.IStandardStreamWriter Inner { get; }

            private CodeRunner.Extensions.Terminals.ITerminal Parent { get; }

            private void WriteColor(ForegroundColorSpan color, string content)
            {
                Terminal.Render(color);
                Write(content);
                Terminal.Render(ForegroundColorSpan.Reset());
            }

            public void Write(string content) => Inner.Write(content);

            public void WriteBlink(string content)
            {
                Terminal.Render(StyleSpan.BlinkOn());
                Write(content);
                Terminal.Render(StyleSpan.BlinkOff());
            }

            public void WriteBold(string content)
            {
                Terminal.Render(StyleSpan.BoldOn());
                Write(content);
                Terminal.Render(StyleSpan.BoldOff());
            }

            public void WriteDebug(string content) => WriteColor(ForegroundColorSpan.Green(), content);

            public void WriteEmphasize(string content)
            {
                Terminal.Render(StyleSpan.BoldOn());
                Terminal.Render(StyleSpan.UnderlinedOn());
                Write(content);
                Terminal.Render(StyleSpan.BoldOff());
                Terminal.Render(StyleSpan.UnderlinedOff());
            }

            public void WriteError(string content) => WriteColor(ForegroundColorSpan.Red(), content);

            public void WriteFatal(string content) => WriteColor(ForegroundColorSpan.Red(), content);

            public void WriteInformation(string content) => WriteColor(ForegroundColorSpan.Cyan(), content);

            public void WriteStandout(string content)
            {
                Terminal.Render(StyleSpan.StandoutOn());
                Write(content);
                Terminal.Render(StyleSpan.StandoutOff());
            }

            public void WriteTable<TSource>(IEnumerable<TSource> sources, params IOutputTableColumnView<TSource>[] columns)
            {
                Assert.ArgumentNotNull(sources, nameof(sources));

                int[] length = new int[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    length[i] = Math.Max(length[i], columns[i].MeasureHeader());
                    foreach (TSource v in sources)
                    {
                        length[i] = Math.Max(length[i], columns[i].Measure(v));
                    }
                }
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i].RenderHeader(Parent, length[i]);
                    Write(" ");
                }
                this.WriteLine();
                foreach (TSource v in sources)
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        columns[i].Render(Parent, v, length[i]);
                        Write(" ");
                    }
                    this.WriteLine();
                }
            }

            public void WriteUnderline(string content)
            {
                Terminal.Render(StyleSpan.UnderlinedOn());
                Write(content);
                Terminal.Render(StyleSpan.UnderlinedOff());
            }

            public void WriteWarning(string content) => WriteColor(ForegroundColorSpan.Yellow(), content);
        }

        private System.CommandLine.Rendering.ITerminal Inner { get; }

        public TextReader InnerInput { get; }

        public TestTerminal(System.CommandLine.Rendering.ITerminal inner, TextReader input)
        {
            Inner = inner;
            InnerInput = input;
            Input = new StandardReaderWrapper(InnerInput);
            Output = new StandardWriterWrapper(Inner, Inner.Out, this);
            Error = new StandardWriterWrapper(Inner, Inner.Error, this);
        }

        public IStandardReader Input { get; }

        public IStandardWriter Output { get; }

        public IStandardWriter Error { get; }

        public ConsoleColor BackgroundColor
        {
            get => Inner.BackgroundColor;
            set => Inner.BackgroundColor = value;
        }

        public ConsoleColor ForegroundColor
        {
            get => Inner.ForegroundColor;
            set => Inner.ForegroundColor = value;
        }

        public int CursorLeft
        {
            get => Inner.CursorLeft;
            set => Inner.CursorLeft = value;
        }

        public int CursorTop
        {
            get => Inner.CursorTop;
            set => Inner.CursorTop = value;
        }

        public void Clear() => Inner.Clear();

        public void HideCursor() => Inner.HideCursor();

        public void ResetColor() => Inner.ResetColor();

        public void SetCursorPosition(int left, int top) => Inner.SetCursorPosition(left, top);

        public void ShowCursor() => Inner.ShowCursor();
    }
}
