using System;
using System.IO;
using System.Linq;
using SIS.MvcFramework.Results;
using SIS.MvcFramework.ViewEngine;
using Xunit;

namespace SIS.MvcFramework.Tests
{
    public class TestSisViewEngine
    {
        private const string TestFilesDirectory = "../../../ViewTests";

        [Theory]
        [InlineData("TestWithoutCSharpCode")]
        [InlineData("UseForForeahAndIf")]
        [InlineData("UseModelData")]
        public void TestGetHtml(string testFileName)
        {
            string viewContent = File.ReadAllText($"{TestFilesDirectory}/{testFileName}.html");
            string expectedResult = File.ReadAllText($"{TestFilesDirectory}/{testFileName}.Result.html");

            IViewEngine viewEngine = new SisViewEngine();

            string actualResult = viewEngine.GetHtml<TestViewModel>(viewContent, new TestViewModel());

            Assert.Equal(expectedResult, actualResult);
        }
    }
}