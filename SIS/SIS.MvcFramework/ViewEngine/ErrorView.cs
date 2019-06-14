using SIS.MvcFramework.Identity;
using SIS.MvcFramework.Validation;

namespace SIS.MvcFramework.ViewEngine
{
    public class ErrorView : IView
    {
        private string errors;

        public ErrorView(string errors)
            => this.errors = errors;

        public string GetHtml(object model, ModelStateDictionary modelState, Principal user)
            => this.errors;
    }
}