# SocketIOSharp
`SocketIOSharp` is a **Socket.IO protocol revision `4` client** library based on `Engine.IO` and `WebSocket` protocol. It depends on [WebSocketSharp](https://github.com/sta/websocket-sharp) to use `WebSocket` protocol. `Engine.IO` protocol is partly implemented by itself.

# Installation


# Usage
## Namespace ##
```csharp
using SocketIOSharp.Client;
```
## Constructor ##
```csharp
SocketIOClient client = new SocketIOClient(SocketIOClient.Scheme.ws, host, port);
```
### SocketIOClient.Scheme ###
```csharp
public enum Scheme
{
  ws,
  wss
}
```
It is used to determine whether `SocketIOClient` instance will connect to server using `WebSocket` protocol or `WebSocketSecure` protocol. 

### Options in constructor ###
```csharp
public SocketIOClient(SocketIOClient.Scheme Scheme, string Host, int Port, bool JsonOnly = false, bool AutoReconnect = false, bool UseAckTimeout = false)
```

1. ```bool JsonOnly``` is used to determine whether `SocketIOClient` instance will or will not accept `WebSocket` binary packet.

2. ```bool AutoReconnect``` is used to determine whether `SocketIOClient` instnace will or will not reconnect to server when `WebSocket.OnClose` is called.

3. ```bool UseAckTimeout``` is used to determine whether `SocketIOClient` instnace will or will not automatically remove timed out ack actions.

- All options above can be changed after construct like ```client.JsonOnly = false```. Default value of these are `false`. 

- If `JsonOnly` is `true`, `SocketIOClient` instance will **NOT** handle `WebSocket` binary packet.

- If `AutoReconnect` is `true`, `SocketIOClient` instance will reconnect after `WebSocket.OnClose` is called.

- If `UseAckTimeout` is `true`, `SocketIOClient` instance will automatically remove timed out ack actions.

## Connect ##
```csharp
client.Connect();
```

## Disconnect ##
```csharp
client.Close();
```
or
```csharp
client.Dispose();
```

Since `SocketIOClient` implements `IDisposable` interface, it will be automatically disconnect when `SocketIOClient.Dispose` is called.

## Handlers ##
For convenient usage, it is implemented to can be used as `Javascript` style.

### Event handlers ###
```csharp
client.On(SocketIOClient.Event.CONNECTION, (JToken[] Data) => // Type of argument is JToken[].
{
  Console.WriteLine("Connected!");
});

client.On(SocketIOClient.Event.DISCONNECT, (Data) => // Argument can be used without type.
{
  Console.WriteLine("Disconnected!");
});

client.On(SocketIOClient.Event.ERROR, (Data) =>
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

client.On("message", (Data) => 
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

### ACK handlers ###
```csharp
client.On("ACK1", (JToken[] Data, (JToken[] Data) => { Console.WriteLine("ACK : " + Data) } =>
{
  // Type of first argument is JToken[].
  // Type of second arguemtn is Action<JToken[]>.
  
  Console.Write("On event ack1 : " + Data);
});

client.On("ACK2", (Data, AckAction) => // Second argument can be method.
{
  Console.Write("On event ack2 : " + Data);
});

client.On("ACK3", CustomAckHandler); // Handler can be method.
client.On(42, LifeTheUniverseAndTheEverything); // Type of event is JToken. So, it can be a number.

client.Off(42, LifeTheUniverseAndTheEverything); // Remove 42 ack handler.
```

#### SocketIOClient.Event ####
```csharp
public static class Event
{
  public static readonly string CONNECTION = "connection";
  public static readonly string DISCONNECT = "disconnect";
  public static readonly string ERROR = "error";
}
```

These are the common basic `Socket.IO` events.

## Emit ##
```csharp
client.Emit("Event without data and ack");

client.Emit("Event only with data", "Hello world");

client.Emit("Event only with ack, action as lambda", (Data) => Console.WriteLine("ACK : " + Data));
client.Emit("Event only with ack, action as method", Console.WriteLine);

client.Emit("Event with data and ack, action as lambda", 9001, (Data) => Console.WriteLine("ACK : " + Data));
client.Emit("Event with data and ack, action as method", 42, Console.WriteLine);
// Type of data is JToken. So, it can be a number.
```

# Implemented features
- Fully implemented `Socket.IO` protocol includes `hartbeat`. See [Socket.IO protocol specifications](https://github.com/socketio/socket.io-protocol) for details.
- Partly implemented `Engine.IO` protocol. For now, `WebSocket` is the **ONLY** supported transport. And the packet type `upgrade` and `noop` is **NOT** supproted. See [Engine.IO protocol specifications](https://github.com/socketio/engine.io-protocol) for details.

# Planned features
- Fully implemented `Engine.IO` client (It will implemented as independent project)
- Transport without `WebSocket`

# Limitations
- **Partly implemented Engine.IO** : It implements part of `Engine.IO` only for processing `Socket.IO` packet. Therefore, it can **NOT** be used as `Engine.IO` client. 
- **Only with WebSocket** : Since it uses `Websocket` as transport, it can be used **ONLY** when `WebSocket` is avalible at `Socket.IO` server.
- **Code style** : `SocketIOSharp` is written in 2016 by a college student. It can be looked as old or odd style.

# Maintenance
Welcome to report issue or create pull request. I will check it happily.

# Dependencies
- [WebSocketSharp v1.0.3-rc11](https://github.com/sta/websocket-sharp)
- [Newtonsoft.Json v12.0.3](https://github.com/JamesNK/Newtonsoft.Json)

# License
`SocketIOSharp` is under [The MIT License](https://github.com/uhm0311/SocketIOSharp/blob/master/LICENSE).
