using System.Collections.Generic;

using SIS.Common;

namespace SIS.MvcFramework.Validation
{
    public class ModelStateDictionary
    {
        private readonly IDictionary<string, IList<string>> errorMessages;

        public ModelStateDictionary()
            => this.errorMessages = new Dictionary<string, IList<string>>();

        public bool IsValid => this.errorMessages.Count == 0;

        public IReadOnlyDictionary<string, IList<string>> ErrorMessages
            => (IReadOnlyDictionary<string, IList<string>>) this.errorMessages;

        public void AddErrorMessage(string propertyName, string errorMessage)
        {
            propertyName.ThrowIfNullOrEmpty(nameof(propertyName));
            errorMessage.ThrowIfNullOrEmpty(nameof(errorMessage));

            if (!this.errorMessages.ContainsKey(propertyName))
            {
                this.errorMessages[propertyName] = new List<string>();
            }

            this.errorMessages[propertyName].Add(errorMessage);
        }
    }
}