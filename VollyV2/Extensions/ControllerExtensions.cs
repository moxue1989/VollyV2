using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync(
            this Controller controller,
            Controllers.EmailTemplateController emailTemplateController,
            string viewName, 
            //TModel model, 
            bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = emailTemplateController.ControllerContext.ActionDescriptor.ActionName;
            }

            //controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                string ViewLocation = "~/Views/EmailTemplate/Newsletter.cshtml";
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.GetView(ViewLocation, ViewLocation, false);

                if (viewResult.Success == false)
                {
                    return $"A view with the name {viewName} could not be found";
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
