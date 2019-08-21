---
title: ASP.NET Core SignalR production hosting and scaling
author: bradygaster
description: Learn how to avoid performance and scaling problems in apps that use ASP.NET Core SignalR.
monikerRange: '>= aspnetcore-2.1'
ms.author: bradyg
ms.custom: mvc
ms.date: 11/28/2018
uid: signalr/scale
---

# ASP.NET Core SignalR hosting and scaling

By [Andrew Stanton-Nurse](https://twitter.com/anurse), [Brady Gaster](https://twitter.com/bradygaster), and [Tom Dykstra](https://github.com/tdykstra),

This article explains hosting and scaling considerations for high-traffic apps that use ASP.NET Core SignalR.

## TCP connection resources

The number of concurrent TCP connections that a web server can support is limited. Standard HTTP clients use *ephemeral* connections. These connections can be closed when the client goes idle and reopened later. On the other hand, a SignalR connection is *persistent*. SignalR connections stay open even when the client goes idle. In a high-traffic app that serves many clients, these persistent connections can cause servers to hit their maximum number of connections.

Persistent connections also consume some additional memory, to track each connection.

The heavy use of connection-related resources by SignalR can affect other web apps that are hosted on the same server. When SignalR opens and holds the last available TCP connections, other web apps on the same server also have no more connections available to them.

If a server runs out of connections, you'll see random socket errors and connection reset errors. For example:

```
An attempt was made to access a socket in a way forbidden by its access permissions...
```

To keep SignalR resource usage from causing errors in other web apps, run SignalR on different servers than your other web apps.

To keep SignalR resource usage from causing errors in a SignalR app, scale out to limit the number of connections a server has to handle.

## Scale out

An app that uses SignalR needs to keep track of all its connections, which creates problems for a server farm. Add a server, and it gets new connections that the other servers don't know about. For example, SignalR on each server in the following diagram is unaware of the connections on the other servers. When SignalR on one of the servers wants to send a message to all clients, the message only goes to the clients connected to that server.

![Scaling SignalR without a backplane](scale/_static/scale-no-backplane.png)

The options for solving this problem are the [Azure SignalR Service](#azure-signalr-service) and [Redis backplane](#redis-backplane).

## Azure SignalR Service

The Azure SignalR Service is a proxy rather than a backplane. Each time a client initiates a connection to the server, the client is redirected to connect to the service. That process is illustrated in the following diagram:

![Establishing a connection to the Azure SignalR Service](scale/_static/azure-signalr-service-one-connection.png)

The result is that the service manages all of the client connections, while each server needs only a small constant number of connections to the service, as shown in the following diagram:

![Clients connected to the service, servers connected to the service](scale/_static/azure-signalr-service-multiple-connections.png)

This approach to scale-out has several advantages over the Redis backplane alternative:

* Sticky sessions, also known as [client affinity](/iis/extensions/configuring-application-request-routing-arr/http-load-balancing-using-application-request-routing#step-3---configure-client-affinity), is not required, because clients are immediately redirected to the Azure SignalR Service when they connect.
* A SignalR app can scale out based on the number of messages sent, while the Azure SignalR Service automatically scales to handle any number of connections. For example, there could be thousands of clients, but if only a few messages per second are sent, the SignalR app won't need to scale out to multiple servers just to handle the connections themselves.
* A SignalR app won't use significantly more connection resources than a web app without SignalR.

For these reasons, we recommend the Azure SignalR Service for all ASP.NET Core SignalR apps hosted on Azure, including App Service, VMs, and containers.

For more information see the [Azure SignalR Service documentation](/azure/azure-signalr/signalr-overview).

## Redis backplane

[Redis](https://redis.io/) is an in-memory key-value store that supports a messaging system with a publish/subscribe model. The SignalR Redis backplane uses the pub/sub feature to forward messages to other servers. When a client makes a connection, the connection information is passed to the backplane. When a server wants to send a message to all clients, it sends to the backplane. The backplane knows all connected clients and which servers they're on. It sends the message to all clients via their respective servers. This process is illustrated in the following diagram:

![Redis backplane, message sent from one server to all clients](scale/_static/redis-backplane.png)

The Redis backplane is the recommended scale-out approach for apps hosted on your own infrastructure. Azure SignalR Service isn't a practical option for production use with on-premises apps due to connection latency between your data center and an Azure data center.

The Azure SignalR Service advantages noted earlier are disadvantages for the Redis backplane:

* Sticky sessions, also known as [client affinity](/iis/extensions/configuring-application-request-routing-arr/http-load-balancing-using-application-request-routing#step-3---configure-client-affinity), is required. Once a connection is initiated on a server, the connection has to stay on that server.
* A SignalR app must scale out based on number of clients even if few messages are being sent.
* A SignalR app uses significantly more connection resources than a web app without SignalR.

## Next steps

For more information, see the following resources:

* [Azure SignalR Service documentation](/azure/azure-signalr/signalr-overview)
* [Set up a Redis backplane](xref:signalr/redis-backplane)
