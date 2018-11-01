using System;
using System.Collections.Generic;
using System.Text;

namespace Chrome.DevTools.Models
{
  public sealed class PdfOptions
  {
    public bool? Landscape { get; set; }
    public bool? DisplayHeaderFooter { get; set; }
    public bool? PrintBackground { get; set; }
    public double? Scale { get; set; }
    public double? PaperWidth { get; set; }
    public double? PaperHeight { get; set; }
    public double? MarginTop { get; set; }
    public double? MarginBottom { get; set; }
    public double? MarginLeft { get; set; }
    public double? MarginRight { get; set; }
    public string PageRanges { get; set; }
    public bool? IgnoreInvalidPageRanges { get; set; }
    public string HeaderTemplate { get; set; }
    public string FooterTemplate { get; set; }
    public bool? PreferCSSPageSize { get; set; }
  }
}
