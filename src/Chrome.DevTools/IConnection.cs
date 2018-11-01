using System;
using System.Threading.Tasks;

namespace Chrome.DevTools
{
  public interface IConnection : IDisposable
  {
    Task<TResult> Send<TResult>(string method, object @params = null);
  }
}