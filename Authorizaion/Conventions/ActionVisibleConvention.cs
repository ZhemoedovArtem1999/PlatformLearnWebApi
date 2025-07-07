using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Authentication.Conventions
{
    /// <summary>
    /// Отмечает все <see cref="ActionModel"/> с контроллером, наследуемым от <see cref="WebApiControllerBase"/> и атрибутами <see cref="HttpGetAttribute"/>, <see cref="HttpPostAttribute"/>, <see cref="HttpDeleteAttribute"/>, <see cref="HttpPutAttribute"/>, <see cref="HttpPatchAttribute"/> как видимые в апи.
    /// </summary>
    internal class ActionVisibleConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            action.ApiExplorer.IsVisible = action.Controller.ControllerType.IsSubclassOf(typeof(ControllerBase)) 
                && action.Attributes.Any(a => a is HttpGetAttribute || a is HttpPostAttribute || a is HttpDeleteAttribute || a is HttpPutAttribute || a is HttpPatchAttribute);
        }
    }
}
