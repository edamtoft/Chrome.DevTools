using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chrome.DevTools
{
  public sealed class Connection : IConnection
  {
    private readonly Uri _endpoint;
    private readonly ConcurrentDictionary<int, TaskCompletionSource<JToken>> _Callbacks;
    private readonly ClientWebSocket _socket;
    private readonly CancellationTokenSource _cancellation;
    private readonly JsonSerializer _serializer;
    private int _currentId;
    private Task _readLoop;

    private const int ChunkSize = 1024;

    private Connection(Uri endpoint)
    {
      _endpoint = endpoint;
      _Callbacks = new ConcurrentDictionary<int, TaskCompletionSource<JToken>>();
      _socket = new ClientWebSocket();
      _cancellation = new CancellationTokenSource();
      _serializer = JsonSerializer.Create(new JsonSerializerSettings
      {
        ContractResolver = new DefaultContractResolver
        {
          NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore
      });
    }

    private async Task Connect()
    {
      await _socket.ConnectAsync(_endpoint, _cancellation.Token);
      _readLoop = ReadLoop();
    }

    public static async Task<Connection> Create(string hostname, int port)
    {
      await Task.Delay(1000);
      var uri = new UriBuilder("http", hostname, port, "/json").Uri;
      using (var client = new HttpClient())
      {
        var jsonString = await client.GetStringAsync(uri);
        var json = JArray.Parse(jsonString);
        var wsAddress = json[0]["webSocketDebuggerUrl"].Value<string>();
        var connection = new Connection(new Uri(wsAddress));
        await connection.Connect();
        return connection;
      }
    }

    private async Task ReadLoop()
    {
      var buffer = new byte[ChunkSize];
      while (!_cancellation.IsCancellationRequested)
      {
        try
        {
          await ReadMessage(buffer);
        }
        catch (Exception err)
        {
          Console.WriteLine(err);
        }
      }
    }

    public async Task<TResult> Send<TResult>(string method, object @params)
    {
      var id = Interlocked.Increment(ref _currentId);

      var completion = new TaskCompletionSource<JToken>();

      if (!_Callbacks.TryAdd(id, completion))
      {
        throw new Exception($"Unable to add message id {id} to callbacks.");
      }

      using (var ms = new MemoryStream(ChunkSize))      
      using (var writer = new StreamWriter(ms))
      using (var jsonWriter = new JsonTextWriter(writer))
      {
        _serializer.Serialize(jsonWriter, new { id, method, @params });
        jsonWriter.Flush();
        writer.Flush();
        await SendMessage(ms.ToArray());
      }
      

      var jsonResult = await completion.Task;

      return jsonResult.ToObject<TResult>(_serializer);
    }

    private async ValueTask SendMessage(Memory<byte> buffer)
    {
      await _socket.SendAsync(buffer, WebSocketMessageType.Text, true, _cancellation.Token);
    }

    private async ValueTask ReadMessage(Memory<byte> buffer)
    {
      using (var message = new MemoryStream(capacity: buffer.Length))
      {
        ValueWebSocketReceiveResult result;
        do
        {
          result = await _socket.ReceiveAsync(buffer, _cancellation.Token);
          message.Write(buffer.Span.Slice(0, result.Count));
        }
        while (!result.EndOfMessage);

        message.Position = 0;

        ProcessMessage(message);
      }
    }

    private void ProcessMessage(Stream message)
    {
      using (var reader = new StreamReader(message))
      using (var jsonReader = new JsonTextReader(reader))
      {
        var json = JObject.Load(jsonReader);

        var id = json["id"].Value<int>();

        if (!_Callbacks.TryRemove(id, out var callback))
        {
          throw new Exception($"Unable to retrieve callback for message id {id}");
        }
        if (json.TryGetValue("error", out var error))
        {
          var errorCode = error.Value<int>("code");
          var errorMessage = error.Value<string>("message");

          callback.TrySetException(new ChromeException(errorCode, errorMessage));
        }
        else if (json.TryGetValue("result", out var result))
        {
          callback.TrySetResult(result);
        }
        else
        {
          callback.TrySetException(new Exception("Unexpected message - Not error or result"));
        }
      }
    }

    public void Dispose() => _cancellation.Cancel();
  }
}
