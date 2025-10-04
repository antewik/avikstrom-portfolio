using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AvikstromPortfolio.Services.FlightBoard
{
    /// <summary>
    /// Renders the FlightBoard partial view to an HTML string.
    /// Used for generating dynamic updates (e.g. SignalR broadcasts or AJAX responses).
    /// </summary>
    public class FlightBoardPartialRenderer : IFlightBoardPartialRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FlightBoardPartialRenderer> _logger;

        public FlightBoardPartialRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            ILogger<FlightBoardPartialRenderer> logger)
        {
            _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            _tempDataProvider = tempDataProvider ?? throw new ArgumentNullException(nameof(tempDataProvider));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Renders the "_FlightBoardRows" partial view to a string using the provided model.
        /// </summary>
        public async Task<string> RenderToStringAsync(object model)
        {
            _logger.LogInformation("Rendering FlightBoard partial view with model type {ModelType}", model?.GetType().Name);

            var actionContext = new ActionContext(
                new DefaultHttpContext { RequestServices = _serviceProvider },
                new RouteData(),
                new ActionDescriptor());

            var viewResult = _viewEngine.GetView(null, "~/Views/FlightBoard/_FlightBoardRows.cshtml", false);

            if (!viewResult.Success)
                throw new InvalidOperationException("View '_FlightBoardRows' not found.");

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            using var sw = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions());

            try
            {
                await viewResult.View.RenderAsync(viewContext);
                var result = sw.ToString();

                _logger.LogInformation("Successfully rendered FlightBoard partial view. Output length: {Length} characters", result.Length);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering FlightBoard partial view.");
                return string.Empty;
            }
        }
    }
}
