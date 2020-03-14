# SocketIOSharp
`SocketIOSharp` is a **Socket.IO protocol revision `4` client** library based on `Engine.IO` and `WebSocket` protocol. It depends on [WebSocketSharp](https://github.com/sta/websocket-sharp) to use `WebSocket` protocol. `Engine.IO` protocol is partly implemented by itself.

# Installation


# Usage
See [README_USAGE.md](https://github.com/uhm0311/SocketIOSharp/blob/master/README_USAGE.md) for details.

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
