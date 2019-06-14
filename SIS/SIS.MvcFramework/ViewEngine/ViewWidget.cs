using System.IO;

namespace SIS.MvcFramework.ViewEngine
{
    public abstract class ViewWidget : IViewWidget
    {
        private const string WidgetFolder = "../../../Views/Shared/Validation";
        private const string WidgetExtension = "vwhtml";

        public string Render()
            => File.ReadAllText($"{WidgetFolder}/{this.GetType().Name}.{WidgetExtension}");
    }
}