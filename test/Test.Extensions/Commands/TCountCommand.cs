using CodeRunner.Extensions.FileStatistics.Commands;
using CodeRunner.Pipelines;
using CodeRunner.Test.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Test.Extensions
{
    [TestClass]
    public class TCountCommand
    {
        [TestMethod]
        public async Task Basic()
        {
            PipelineResult<Wrapper<int>> result = await PipelineGenerator.CreateBuilder().UseSampleCommandInvoker(new CountCommand().Build(),
                new string[] { "count" });

            ResultAssert.OkWithZero(result);
        }
    }
}
