using System.Collections.Generic;

using SIS.HTTP.Common;

namespace SIS.HTTP.Sessions
{
    public class HttpSession : IHttpSession
    {
        private readonly IDictionary<string, object> parameters;

        public HttpSession(string id)
        {
            this.Id = id;

            this.parameters = new Dictionary<string, object>();
        }

        public string Id { get; }

        public void AddParameter(string name, object parameter)
        {
            CoreValidator.ThrowIfNullOrEmpty(name, nameof(name));
            CoreValidator.ThrowIfNull(parameter, nameof(parameter));

            this.parameters[name] = parameter;
        }

        public void ClearParameters() => this.parameters.Clear();

        public bool ContainsParameter(string name)
            => this.parameters.ContainsKey(name);

        public object GetParameter(string name)
        {
            CoreValidator.ThrowIfNullOrEmpty(name, nameof(name));

            return this.parameters.TryGetValue(name, out object obj)
                ? obj
                : null;
        }

        public T GetParameter<T>(string name) => (T)this.GetParameter(name);
    }
}