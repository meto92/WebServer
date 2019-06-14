using SIS.MvcFramework.Identity;
using SIS.MvcFramework.Validation;

namespace SIS.MvcFramework.ViewEngine
{
    public interface IView
    {
        string GetHtml(object model, ModelStateDictionary modelState, Principal user);
    }
}