using CodeRunner.Managements;
using System;

namespace CodeRunner.Test.Mocks
{
    public class TestWorkItem : IWorkItem
    {
        public TestWorkItem(string name) => Name = name;

        public Guid Id => Guid.NewGuid();

        public string Name { get; }
    }
}
