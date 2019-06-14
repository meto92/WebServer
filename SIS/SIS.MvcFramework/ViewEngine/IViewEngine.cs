using SIS.MvcFramework.Identity;
using SIS.MvcFramework.Validation;

namespace SIS.MvcFramework.ViewEngine
{
    public interface IViewEngine
    {
        string GetHtml<T>(string viewContent, T model, ModelStateDictionary modelState, Principal user = null);
    }
}