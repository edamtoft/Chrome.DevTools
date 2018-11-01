using System;
using System.Collections.Generic;
using System.Text;

namespace Chrome.DevTools
{
  public sealed class ChromeException : Exception
  {
    public int Code { get; set; }

    public ChromeException(int code, string message) : base(message)
    {
      Code = code;
    }
  }
}
