using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace OrigoApiGateway.Controllers
{
    public class HideTestControllersConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            controller.ApiExplorer.IsVisible = false;
        }
    }
}
