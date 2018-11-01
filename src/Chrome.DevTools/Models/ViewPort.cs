using System;
using System.Collections.Generic;
using System.Text;

namespace Chrome.DevTools.Models
{
  public sealed class ViewPort
  {
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Zoom { get; set; }
  }
}
