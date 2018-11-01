using Chrome.DevTools;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScreenshotTool
{
  class Program
  {
    static async Task Main(string[] args)
    {

      using (var proc = Process.Start("chrome", "--headless --remote-debugging-port=9222 --disable-gpu"))
      using (var conn = await Connection.Create("localhost", 9222))
      {
        Console.WriteLine($"Chrome process running - pid: {proc.Id}");

        await conn.Page().Navigate("https://www.google.com");

        var screenshot = await conn.Page().CaptureScreenshot(format: "png");

        await screenshot.WriteToFileAync("screenshot.png");

        proc.Kill();
      }
    }
  }
}
