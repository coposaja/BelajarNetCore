---
title: Use MessagePack Hub Protocol in SignalR for ASP.NET Core
author: bradygaster
description: Add MessagePack Hub Protocol to ASP.NET Core SignalR.
monikerRange: '>= aspnetcore-2.1'
ms.author: bradyg
ms.custom: mvc
ms.date: 02/27/2019
uid: signalr/messagepackhubprotocol
---

# Use MessagePack Hub Protocol in SignalR for ASP.NET Core

By [Brennan Conroy](https://github.com/BrennanConroy)

This article assumes the reader is familiar with the topics covered in [Get Started](xref:tutorials/signalr).

## What is MessagePack?

[MessagePack](https://msgpack.org/index.html) is a binary serialization format that is fast and compact. It's useful when performance and bandwidth are a concern because it creates smaller messages compared to [JSON](https://www.json.org/). Because it's a binary format, messages are unreadable when looking at network traces and logs unless the bytes are passed through a MessagePack parser. SignalR has built-in support for the MessagePack format, and provides APIs for the client and server to use.

## Configure MessagePack on the server

To enable the MessagePack Hub Protocol on the server, install the `Microsoft.AspNetCore.SignalR.Protocols.MessagePack` package in your app. In the Startup.cs file add `AddMessagePackProtocol` to the `AddSignalR` call to enable MessagePack support on the server.

> [!NOTE]
> JSON is enabled by default. Adding MessagePack enables support for both JSON and MessagePack clients.

```csharp
services.AddSignalR()
    .AddMessagePackProtocol();
```

To customize how MessagePack will format your data, `AddMessagePackProtocol` takes a delegate for configuring options. In that delegate, the `FormatterResolvers` property can be used to configure MessagePack serialization options. For more information on how the resolvers work, visit the MessagePack library at [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp). Attributes can be used on the objects you want to serialize to define how they should be handled.

```csharp
services.AddSignalR()
    .AddMessagePackProtocol(options =>
    {
        options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
        {
            MessagePack.Resolvers.StandardResolver.Instance
        };
    });
```

## Configure MessagePack on the client

> [!NOTE]
> JSON is enabled by default for the supported clients. Clients can only support a single protocol. Adding MessagePack support will replace any previously configured protocols.

### .NET client

To enable MessagePack in the .NET Client, install the `Microsoft.AspNetCore.SignalR.Protocols.MessagePack` package and call `AddMessagePackProtocol` on `HubConnectionBuilder`.

```csharp
var hubConnection = new HubConnectionBuilder()
                        .WithUrl("/chatHub")
                        .AddMessagePackProtocol()
                        .Build();
```

> [!NOTE]
> This `AddMessagePackProtocol` call takes a delegate for configuring options just like the server.

### JavaScript client

MessagePack support for the JavaScript client is provided by the `@aspnet/signalr-protocol-msgpack` npm package.

```console
npm install @aspnet/signalr-protocol-msgpack
```

After installing the npm package, the module can be used directly via a JavaScript module loader or imported into the browser by referencing the *node_modules\\@aspnet\signalr-protocol-msgpack\dist\browser\signalr-protocol-msgpack.js* file. In a browser, the `msgpack5` library must also be referenced. Use a `<script>` tag to create a reference. The library can be found at *node_modules\msgpack5\dist\msgpack5.js*.

> [!NOTE]
> When using the `<script>` element, the order is important. If *signalr-protocol-msgpack.js* is referenced before *msgpack5.js*, an error occurs when trying to connect with MessagePack. *signalr.js* is also required before *signalr-protocol-msgpack.js*.

```html
<script src="~/lib/signalr/signalr.js"></script>
<script src="~/lib/msgpack5/msgpack5.js"></script>
<script src="~/lib/signalr/signalr-protocol-msgpack.js"></script>
```

Adding `.withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())` to the `HubConnectionBuilder` will configure the client to use the MessagePack protocol when connecting to a server.

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
    .build();
```

> [!NOTE]
> At this time, there are no configuration options for the MessagePack protocol on the JavaScript client.

## MessagePack quirks

There are a few issues to be aware of when using the MessagePack Hub Protocol.

### MessagePack is case-sensitive

The MessagePack protocol is case-sensitive. For example, consider the following C# class:

```csharp
public class ChatMessage
{
    public string Sender { get; }
    public string Message { get; }
}
```

When sending from the JavaScript client, you must use `PascalCased` property names, since the casing must match the C# class exactly. For example:

```javascript
connection.invoke("SomeMethod", { Sender: "Sally", Message: "Hello!" });
```

Using `camelCased` names won't properly bind to the C# class. You can work around this by using the `Key` attribute to specify a different name for the MessagePack property. For more information, see [the MessagePack-CSharp documentation](https://github.com/neuecc/MessagePack-CSharp#object-serialization).

### DateTime.Kind is not preserved when serializing/deserializing

The MessagePack protocol doesn't provide a way to encode the `Kind` value of a `DateTime`. As a result, when deserializing a date, the MessagePack Hub Protocol assumes the incoming date is in UTC format. If you're working with `DateTime` values in local time, we recommend converting to UTC before sending them. Convert them from UTC to local time when you receive them.

For more information on this limitation, see GitHub issue [aspnet/SignalR#2632](https://github.com/aspnet/SignalR/issues/2632).

### DateTime.MinValue is not supported by MessagePack in JavaScript

The [msgpack5](https://github.com/mcollina/msgpack5) library used by the SignalR JavaScript client doesn't support the `timestamp96` type in MessagePack. This type is used to encode very large date values (either very early in the past or very far in the future). The value of `DateTime.MinValue` is `January 1, 0001` which must be encoded in a `timestamp96` value. Because of this, sending `DateTime.MinValue` to a JavaScript client isn't supported. When `DateTime.MinValue` is received by the JavaScript client, the following error is thrown:

```
Uncaught Error: unable to find ext type 255 at decoder.js:427
```

Usually, `DateTime.MinValue` is used to encode a "missing" or `null` value. If you need to encode that value in MessagePack, use a nullable `DateTime` value (`DateTime?`) or encode a separate `bool` value indicating if the date is present.

For more information on this limitation, see GitHub issue [aspnet/SignalR#2228](https://github.com/aspnet/SignalR/issues/2228).

### MessagePack support in "ahead-of-time" compilation environment

The [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp) library used by the .NET client and server uses code generation to optimize serialization. As a result, it isn't supported by default on environments that use "ahead-of-time" compilation (such as Xamarin iOS or Unity). It's possible to use MessagePack in these environments by "pre-generating" the serializer/deserializer code. For more information, see [the MessagePack-CSharp documentation](https://github.com/neuecc/MessagePack-CSharp#pre-code-generationunityxamarin-supports). Once you have pre-generated the serializers, you can register them using the configuration delegate passed to `AddMessagePackProtocol`:

```csharp
services.AddSignalR()
    .AddMessagePackProtocol(options =>
    {
        options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
        {
            MessagePack.Resolvers.GeneratedResolver.Instance,
            MessagePack.Resolvers.StandardResolver.Instance
        };
    });
```

### Type checks are more strict in MessagePack

The JSON Hub Protocol will perform type conversions during deserialization. For example, if the incoming object has a property value that is a number (`{ foo: 42 }`) but the property on the .NET class is of type `string`, the value will be converted. However, MessagePack doesn't perform this conversion and will throw an exception that can be seen in server-side logs (and in the console):

```
InvalidDataException: Error binding arguments. Make sure that the types of the provided values match the types of the hub method being invoked.
```

For more information on this limitation, see GitHub issue [aspnet/SignalR#2937](https://github.com/aspnet/SignalR/issues/2937).

## Related resources

* [Get Started](xref:tutorials/signalr)
* [.NET client](xref:signalr/dotnet-client)
* [JavaScript client](xref:signalr/javascript-client)
