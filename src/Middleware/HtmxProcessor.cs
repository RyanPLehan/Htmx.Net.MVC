using AspNetCoreGeneratedDocument;
using ContosoUniversity.Middleware.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ContosoUniversity.Middleware
{
    /// <summary>
    /// Middleware to check for HTMX Request
    /// </summary>
    /// <remarks>
    /// The following links were help getting this to work
    /// https://scottsauber.com/2018/07/07/walkthrough-creating-an-html-email-template-with-razor-and-razor-class-libraries-and-rendering-it-from-a-net-standard-class-library/
    /// https://github.com/scottsauber/RazorHtmlEmails
    /// </remarks>
    public class HtmxProcessor
    {
        private readonly RequestDelegate _next;
        private readonly IViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public HtmxProcessor(RequestDelegate next, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider)
        {
            // IRazorViewEngine viewEngine
            // ICompositeViewEngine viewEngine
            _next = next; 
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            bool isHtmxRequest = !String.IsNullOrWhiteSpace(httpContext.Request.Headers["HX-Request"]);

            if (isHtmxRequest)
            {
                await _next(httpContext);
                return;
            }

            // Create landing page manually and pass in the route
            var viewResult = _viewEngine.GetView(null, @"/Middleware/Views/HtmxLayout.cshtml", true);
            if (viewResult.Success)
            {
                using (StringWriter sw = new StringWriter())
                {
                    var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
                    var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                    viewData.Model = new HtmxLayoutModel() { Route = httpContext.Request.Path };
                    var tempData = new TempDataDictionary(httpContext, _tempDataProvider);
                    var viewContext = new ViewContext(actionContext, viewResult.View, 
                                                      viewData, tempData, 
                                                      sw, new HtmlHelperOptions());

                    await viewResult.View.RenderAsync(viewContext);
                    await httpContext.Response.WriteAsync(sw.ToString());
                }
            }
        }
    }
}
