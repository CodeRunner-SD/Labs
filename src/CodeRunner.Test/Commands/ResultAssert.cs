using CodeRunner.Pipelines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeRunner.Test.Commands
{
    public static class ResultAssert
    {
        public static void OkWithZero(this PipelineResult<Wrapper<int>> result) => result.OkWithCode(0);

        public static void OkWithCode(this PipelineResult<Wrapper<int>> result, int code)
        {
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual<int>(code, result.Result!);
        }

        public static void ErrorWithInnerException<TException>(this PipelineResult<Wrapper<int>> result) where TException : Exception
        {
            Assert.IsTrue(result.IsError);
            Assert.IsInstanceOfType(result.Exception!.InnerException, typeof(TException));
        }
    }
}
