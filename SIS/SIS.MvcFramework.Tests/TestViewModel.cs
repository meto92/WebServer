using System.Collections.Generic;

namespace SIS.MvcFramework.Tests
{
    public class TestViewModel
    {
        public TestViewModel()
        {
            this.StringValue = "str";
            
            this.ListValues = new List<string>
            {
                "123",
                "val1",
                string.Empty
            };
        }

        public string StringValue { get; set; }

        public IEnumerable<string> ListValues { get; set; }
    }
}