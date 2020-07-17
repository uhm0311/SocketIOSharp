# SocketIOSharp
`SocketIOSharp` is a **Socket.IO protocol revision `4`** library based on `Engine.IO` and `WebSocket` protocol. It depends on [EngineIOSharp](https://github.com/uhm0311/EngineIOSharp) to use `Engine.IO` protocol.

# Installation
- [Nuget gallery](https://www.nuget.org/packages/SocketIOSharp)

- Command `Install-Package SocketIOSharp` in nuget package manager console.

# Usage
## Client
### Namespace ###
```csharp
using EngineIOSharp.Common.Enum;
using SocketIOSharp.Client;
using SocketIOSharp.Common;
```

### Constructor ###
```csharp
SocketIOClient client = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, "localhost", 9001));
```

#### SocketIOClientOption ####
  - **Essential Parameters**
  
    - `Scheme` : Scheme to connect to. It can be `EngineIOScheme.http` or `EngineIOScheme.https`. Internally, it supports `ws` and `wss`.
    
    - `Host` : Host to connect to.
    
    - `Port` : Port to connect to.
    
  - **Optional Parameters**
  
    - `PolicyPort` : Port the policy server listens on. Defaults to `843`.
    
    - `Path` : Path to connect to. Defaults to `"/socket.io"`.
    
    - `Reconnection` : Whether to reconnect to server after Socket.IO client is closed or not.
    
    - `ReconnectionAttempts` : Number of reconnection attempts before giving up.
    
    - `ReconnectionDelay` : How ms to initially wait before attempting a new reconnection.
        
    - `ReconnectionDelayMax` : Maximum amount of time to wait between reconnections. Each attempt increases the `ReconnectionDelay` by 2x along with a randomization.
        
    - `RandomizationFactor` : 0 <= RandomizationFactor <= 1.
    
    - `Query` : Parameters that will be passed for each request to the server. Defaults to `null`.
    
    - `Upgrade` : Whether the client should try to upgrade the transport. Defaults to `true`.
    
    - `RemeberUpgrade` : Whether the client should bypass normal upgrade process when previous websocket connection is succeeded. Defaults to `false`.
    
    - `ForceBase64` : Forces base 64 encoding for transport. Defaults to `false`.
    
    - `WithCredentials` : Whether to include credentials such as cookies, authorization headers, TLS client certificates, etc. with polling requests. Defaults to `false`.
    
    - `TimestampRequests` : Whether to add the timestamp with each transport request. Polling requests are always stamped. Defaults to `null`.
    
    - `TimestampParam` : Timestamp parameter. Defaults to `"t"`.
    
    - `Polling` : Whether to include polling transport. Defaults to `true`.
    
    - `PollingTimeout` : Timeout for polling requests in milliseconds. Defaults to `0`, which waits indefinitely.
    
    - `WebSocket` : Whether to include websocket transport. Defaults to `true`.
    
    - `WebSocketSubprotocols` : List of websocket subprotocols. Defaults to `null`.
    
    - `ExtraHeaders` : Headers that will be passed for each request to the server. Defaults to `null`.
    
    - `ClientCertificates` : The collection of security certificates that are associated with each request. Defaults to `null`.
    
    - `ClientCertificateSelectionCallback` : Callback used to select the certificate to supply to the server. Defaults to `null`.
    
    - `ServerCertificateValidationCallback` : Callback method to validate the server certificate. Defaults to `null` and server certificate will be always validated.

### Connect ###
```csharp
client.Connect();
```

### Disconnect ###
```csharp
client.Close();
```
or
```csharp
client.Dispose();
```

Since `SocketIOClient` implements `IDisposable` interface, it will be automatically disconnect when `SocketIOClient.Dispose` is called.

### Handlers ###
For convenient usage, it is implemented to can be used as `Javascript` style.

#### Event handlers ####
```csharp
client.On(SocketIOClient.Event.CONNECTION, () =>
{
  Console.WriteLine("Connected!");
});

client.On(SocketIOClient.Event.DISCONNECT, () =>
{
  Console.WriteLine("Disconnected!");
});

client.On(SocketIOClient.Event.ERROR, (JToken[] Data) => // Type of argument is JToken[].
{
  if (Data != null && Data.Length > 0 && Data[0] != null)
  {
    Console.WriteLine("Error : " + Data[0]);
  }
  else
  {
    Console.WrtieLine("Unkown Error");
  }
});

client.On("message", (Data) => // Argument can be used without type.
{
  if (Data != null && Data.Length > 0 && Data[0] != null)
  {
    Console.WriteLine("Message : " + Data[0]);
  }
});

client.On("CustomEvent", CustomEventHandler); // Handler can be method.
client.On(9001, ItsOverNineThousands); // Type of event is JToken. So, it can be a number.

client.Off(9001, ItsOverNineThousands); // Remove 9001 event handler.
```

#### ACK handlers ####
```csharp
client.On("ACK1", (SocketIOAckEvent EventArgument) =>
{
  // Type of SocketIOAckEvent.Data is JToken[].
  // Type of SocketIOAckEvent.Callback is Action<JToken[]>.
  
  Console.Write("On event ack1 : " + EventArgument.Data);
});

client.On("ACK2", (EventArgument) => // Argument can be used without type.
{
  Console.Write("On event ack2 : " + EventArgument.Data);
});

client.On("ACK3", CustomAckHandler); // Handler can be method.
client.On(42, LifeTheUniverseAndTheEverything); // Type of event is JToken. So, it can be a number.

client.Off(42, LifeTheUniverseAndTheEverything); // Remove 42 ack handler.
```

##### SocketIOClient.Event #####
```csharp
public static class Event
{
  public static readonly string CONNECTION = "connection";
  public static readonly string DISCONNECT = "disconnect";
  public static readonly string ERROR = "error";
}
```

These are the common basic `Socket.IO` events.

### Emit ###
```csharp
client.Emit("Event without data and ack");

client.Emit("Event only with data", "Hello world");

client.Emit("Event only with ack, action as lambda", (Data) => Console.WriteLine("ACK : " + Data));
client.Emit("Event only with ack, action as method", Console.WriteLine);

client.Emit("Event with data and ack, action as lambda", 9001, (Data) => Console.WriteLine("ACK : " + Data));
client.Emit("Event with data and ack, action as method", 42, Console.WriteLine);
// Type of data is JToken. So, it can be a number.
```

## Server
### Namespace

```csharp
using SocketIOSharp.Server;
```

### Constructor
```csharp
SocketIOServer server = new SocketIOServer(new SocketIOServerOption(9001));
```

#### SocketIOServerOption

  - **Essential Parameters**
  
    - `Port` : Port to listen.
  
  - **Optional Parameters**
    
    - `Secure` : Whether to secure connections. Defatuls to `false`.
    
    - `PingTimeout` : How many ms without a pong packet to consider the connection closed. Defatuls to `5000`.
    
    - `PingInterval` : How many ms before sending a new ping packet. Defatuls to `25000`.
    
    - `UpgradeTimeout` : How many ms before an uncompleted transport upgrade is cancelled. Defatuls to `10000`.
    
    - `Polling` : Whether to accept polling transport. Defatuls to `true`.
    
    - `WebSocket` : Whether to accept websocket transport. Defatuls to `true`.
    
    - `AllowUpgrade` : Whether to allow transport upgrade. Defatuls to `true`.
    
    - `SetCookie` : Whether to use cookie. Defatuls to `true`.
    
    - `SIDCookieName` : Name of sid cookie. Defatuls to `"io"`.
    
    - `Cookies` : Configuration of the cookie that contains the client sid to send as part of handshake response headers. This cookie might be used for sticky-session. Defatuls to `null`.
    
    - `AllowHttpRequest` : A function that receives a given handshake or upgrade http request as its first parameter, and can decide whether to continue or not. Defatuls to `null`.
    
    - `AllowWebSocket` : A function that receives a given handshake or upgrade websocket connection as its first parameter, and can decide whether to continue or not. Defatuls to `null`.
    
    - `InitialData` : An optional packet which will be concatenated to the handshake packet emitted by Engine.IO. Defatuls to `null`.
    
    - `ServerCertificate` : The certificate used to authenticate the server. Defatuls to `null`.
    
    - `ClientCertificateValidationCallback` : Callback used to validate the certificate supplied by the client. Defatuls to `null` and  client certificate will be always validated.
    
### Start

```csharp
server.Start();
```

### Stop

```csharp
server.Stop();
```

or

```csharp
server.Dispose();
```

Since `SocketIOServer` implements `IDisposable` interface, it will be automatically stoped when `SocketIOServer.Dispose` is called.

### Connection
For convenient usage, it is implemented to can be used as `Javascript` style.

```csharp
server.OnConnection((SocketIOSocket socket) =>
{
  Console.WriteLine("Client connected!");

  socket.On("input", (Data) =>
  {
    foreach (JToken Token in Data)
    {
      Console.Write(Token + " ");
    }

    Console.WriteLine();
    socket.Emit("echo", Data);
  });

  socket.On(SocketIOEvent.DISCONNECT, () =>
  {
    Console.WriteLine("Client disconnected!");
  });
});
```

#### SocketIOSocket

- `SocketIOSocket` is a type of parameter in `SocketIOServer.OnConnection` event callback. It can be used similarly as `SocketIOClient`.

##### Disconnect

```csharp
socket.Close();
```

or

```csharp
socket.Dispose();
```

Since `SocketIOSocket` implements `IDisposable` interface, it will be automatically disconnected when `SocketIOSocket.Dispose` is called.

##### Handlers #####
For convenient usage, it is implemented to can be used as `Javascript` style.

###### Event handlers ######
```csharp
client.On(SocketIOClient.Event.DISCONNECT, () =>
{
  Console.WriteLine("Disconnected!");
});

client.On(SocketIOClient.Event.ERROR, (JToken[] Data) => // Type of argument is JToken[].
{
  if (Data != null && Data.Length > 0 && Data[0] != null)
  {
    Console.WriteLine("Error : " + Data[0]);
  }
  else
  {
    Console.WrtieLine("Unkown Error");
  }
});

client.On("message", (Data) => // Argument can be used without type.
{
  if (Data != null && Data.Length > 0 && Data[0] != null)
  {
    Console.WriteLine("Message : " + Data[0]);
  }
});

client.On("CustomEvent", CustomEventHandler); // Handler can be method.
client.On(9001, ItsOverNineThousands); // Type of event is JToken. So, it can be a number.

client.Off(9001, ItsOverNineThousands); // Remove 9001 event handler.
```

- There is no `SocketIOSocket.On(SocketIOEvent.CONNECTION)` event since it is already opened when `SocketIOServer.OnConnection` event callback is called.

###### ACK handlers ######
```csharp
client.On("ACK1", (SocketIOAckEvent EventArgument) =>
{
  // Type of SocketIOAckEvent.Data is JToken[].
  // Type of SocketIOAckEvent.Callback is Action<JToken[]>.
  
  Console.Write("On event ack1 : " + EventArgument.Data);
});

client.On("ACK2", (EventArgument) => // Argument can be used without type.
{
  Console.Write("On event ack2 : " + EventArgument.Data);
});

client.On("ACK3", CustomAckHandler); // Handler can be method.
client.On(42, LifeTheUniverseAndTheEverything); // Type of event is JToken. So, it can be a number.

client.Off(42, LifeTheUniverseAndTheEverything); // Remove 42 ack handler.
```

##### Emit #####
```csharp
client.Emit("Event without data and ack");

client.Emit("Event only with data", "Hello world");

client.Emit("Event only with ack, action as lambda", (Data) => Console.WriteLine("ACK : " + Data));
client.Emit("Event only with ack, action as method", Console.WriteLine);

client.Emit("Event with data and ack, action as lambda", 9001, (Data) => Console.WriteLine("ACK : " + Data));
client.Emit("Event with data and ack, action as method", 42, Console.WriteLine);
// Type of data is JToken. So, it can be a number.
```

### Emit ###

```csharp
server.Emit("Event without data and ack");

server.Emit("Event only with data", "Hello world");

server.Emit("Event only with ack, action as lambda", (Data) => Console.WriteLine("ACK : " + Data));
server.Emit("Event only with ack, action as method", Console.WriteLine);

server.Emit("Event with data and ack, action as lambda", 9001, (Data) => Console.WriteLine("ACK : " + Data));
server.Emit("Event with data and ack, action as method", 42, Console.WriteLine);
// Type of data is JToken. So, it can be a number.
```

# Maintenance
Welcome to report issue or create pull request. I will check it happily.

# Dependencies
- [EngineIOSharp v1.0.2](https://github.com/uhm0311/EngineIOSharp)

# License
`SocketIOSharp` is under [The MIT License](https://github.com/uhm0311/SocketIOSharp/blob/master/LICENSE).
