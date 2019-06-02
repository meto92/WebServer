using System;

using SIS.HTTP.Enums;

namespace SIS.MvcFramework.Attributes.Methods
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class HttpMethodAttribute : Attribute
    {
        public string ActionName { get; set; }

        public string Path { get; set; }

        public virtual HttpRequestMethod HttpMethod
        {
            get
            {
                string name = this.GetType().Name.ToUpper();
                string httpRequestMethodStr = name.Substring(4, name.Length - 4 - nameof(Attribute).Length);

                HttpRequestMethod httpRequestMethod = HttpRequestMethod.Get;

                Enum.TryParse(httpRequestMethodStr, true, out httpRequestMethod);

                return httpRequestMethod;
            }
        }
    }
}