using Chrome.DevTools.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chrome.DevTools.Domains
{
  /// <summary>
  /// Actions and events related to the inspected page belong to the page domain. 
  /// See https://chromedevtools.github.io/devtools-protocol/1-2/Page
  /// </summary>
  public sealed class Page
  {
    private readonly IConnection _connection;

    public Page(IConnection connection) => _connection = connection;

    public Task Enable() =>
      _connection.Send<VoidResult>("Page.enable");

    public Task Disable() =>
      _connection.Send<VoidResult>("Page.disable");

    public Task Reload(bool? ignoreCache = null, string scriptToEvaluateOnLoad = null) =>
      _connection.Send<VoidResult>("Page.reload", new { ignoreCache, scriptToEvaluateOnLoad });

    public Task<NavigationResult> Navigate(string url) => 
      _connection.Send<NavigationResult>("Page.navigate", new { url });

    public Task HandleJavaScriptDialog(bool accept, string promptText = null) =>
      _connection.Send<VoidResult>("Page.handleJaveScriptDialog", new { accept, promptText });

    public Task<DataResult> CaptureScreenshot(string format = null, int? quality = null, ViewPort viewPort = null, bool? fromSurface = null) =>
      _connection.Send<DataResult>("Page.captureScreenshot", new { format, quality, viewPort, fromSurface });

    public Task BringToFront() => 
      _connection.Send<VoidResult>("Page.bringToFront");

    public Task<DataResult> PrintToPdf(PdfOptions options) =>
      _connection.Send<DataResult>("Page.printToPDF", options ?? throw new ArgumentNullException(nameof(options)));
  }
}
