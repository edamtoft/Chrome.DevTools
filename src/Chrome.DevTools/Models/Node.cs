using System;
using System.Collections.Generic;
using System.Text;

namespace Chrome.DevTools.Models
{
  public sealed class Node
  {
    public string NodeId { get; set; }
    public string ParentId { get; set; }
    public string BackendNodeId { get; set; }
    public NodeType NodeType { get; set; }
    public string NodeName { get; set; }
    public Node[] Children { get; set; }
    public string NodeValue { get; set; }
    public string[] Attributes { get; set; }
    public Node ContentDocument { get; set; }
  }
}
