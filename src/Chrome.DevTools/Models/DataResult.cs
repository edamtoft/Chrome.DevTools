using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Chrome.DevTools.Models
{
  /// <summary>
  /// Result with a stream of data, such as an image from a screenshot
  /// </summary>
  public sealed class DataResult
  {
    public byte[] Data { get; set; }

    public async Task WriteToFileAync(string path) => await File.WriteAllBytesAsync(path, Data);
  }
}
