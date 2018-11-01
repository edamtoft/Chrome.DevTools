using Chrome.DevTools.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chrome.DevTools
{
  public static class DomainExtensions
  {
    public static Page Page(this IConnection connection) => new Page(connection);
  }
}
