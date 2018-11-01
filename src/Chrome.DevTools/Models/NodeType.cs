using System;
using System.Collections.Generic;
using System.Text;

namespace Chrome.DevTools.Models
{
  /// <summary>
  /// See https://developer.mozilla.org/en-US/docs/Web/API/Node/nodeType
  /// </summary>
  public enum NodeType
  {
    Element = 1,
    Text = 3,
    CDataSection = 4,
    ProcessingInstruction = 7,
    Comment = 8,
    Document = 9,
    DocumentType = 10,
    DocumentFragment = 11,
  }
}
