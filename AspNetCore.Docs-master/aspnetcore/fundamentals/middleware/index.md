---
title: ASP.NET Core Middleware
author: rick-anderson
description: Learn about ASP.NET Core middleware and the request pipeline.
monikerRange: '>= aspnetcore-2.1'
ms.author: riande
ms.custom: mvc
ms.date: 07/09/2019
uid: fundamentals/middleware/index
---
# ASP.NET Core Middleware

By [Rick Anderson](https://twitter.com/RickAndMSFT) and [Steve Smith](https://ardalis.com/)

Middleware is software that's assembled into an app pipeline to handle requests and responses. Each component:

* Chooses whether to pass the request to the next component in the pipeline.
* Can perform work before and after the next component in the pipeline.

Request delegates are used to build the request pipeline. The request delegates handle each HTTP request.

Request delegates are configured using <xref:Microsoft.AspNetCore.Builder.RunExtensions.Run*>, <xref:Microsoft.AspNetCore.Builder.MapExtensions.Map*>, and <xref:Microsoft.AspNetCore.Builder.UseExtensions.Use*> extension methods. An individual request delegate can be specified in-line as an anonymous method (called in-line middleware), or it can be defined in a reusable class. These reusable classes and in-line anonymous methods are *middleware*, also called *middleware components*. Each middleware component in the request pipeline is responsible for invoking the next component in the pipeline or short-circuiting the pipeline. When a middleware short-circuits, it's called a *terminal middleware* because it prevents further middleware from processing the request.

<xref:migration/http-modules> explains the difference between request pipelines in ASP.NET Core and ASP.NET 4.x and provides additional middleware samples.

## Create a middleware pipeline with IApplicationBuilder

The ASP.NET Core request pipeline consists of a sequence of request delegates, called one after the other. The following diagram demonstrates the concept. The thread of execution follows the black arrows.

![Request processing pattern showing a request arriving, processing through three middlewares, and the response leaving the app. Each middleware runs its logic and hands off the request to the next middleware at the next() statement. After the third middleware processes the request, the request passes back through the prior two middlewares in reverse order for additional processing after their next() statements before leaving the app as a response to the client.](index/_static/request-delegate-pipeline.png)

Each delegate can perform operations before and after the next delegate. Exception-handling delegates should be called early in the pipeline, so they can catch exceptions that occur in later stages of the pipeline.

The simplest possible ASP.NET Core app sets up a single request delegate that handles all requests. This case doesn't include an actual request pipeline. Instead, a single anonymous function is called in response to every HTTP request.

[!code-csharp[](index/snapshot/Middleware/Startup.cs?name=snippet1)]

The first <xref:Microsoft.AspNetCore.Builder.RunExtensions.Run*> delegate terminates the pipeline.

Chain multiple request delegates together with <xref:Microsoft.AspNetCore.Builder.UseExtensions.Use*>. The `next` parameter represents the next delegate in the pipeline. You can short-circuit the pipeline by *not* calling the *next* parameter. You can typically perform actions both before and after the next delegate, as the following example demonstrates:

[!code-csharp[](index/snapshot/Chain/Startup.cs?name=snippet1)]

When a delegate doesn't pass a request to the next delegate, it's called *short-circuiting the request pipeline*. Short-circuiting is often desirable because it avoids unnecessary work. For example, [Static File Middleware](xref:fundamentals/static-files) can act as a *terminal middleware* by processing a request for a static file and short-circuiting the rest of the pipeline. Middleware added to the pipeline before the middleware that terminates further processing still processes code after their `next.Invoke` statements. However, see the following warning about attempting to write to a response that has already been sent.

> [!WARNING]
> Don't call `next.Invoke` after the response has been sent to the client. Changes to <xref:Microsoft.AspNetCore.Http.HttpResponse> after the response has started throw an exception. For example, changes such as setting headers and a status code throw an exception. Writing to the response body after calling `next`:
>
> * May cause a protocol violation. For example, writing more than the stated `Content-Length`.
> * May corrupt the body format. For example, writing an HTML footer to a CSS file.
>
> <xref:Microsoft.AspNetCore.Http.HttpResponse.HasStarted*> is a useful hint to indicate if headers have been sent or the body has been written to.

## Order

The order that middleware components are added in the `Startup.Configure` method defines the order in which the middleware components are invoked on requests and the reverse order for the response. The order is critical for security, performance, and functionality.

The following `Startup.Configure` method adds middleware components for common app scenarios:

1. Exception/error handling
   * When the app runs in the Development environment:
     * Developer Exception Page Middleware (<xref:Microsoft.AspNetCore.Builder.DeveloperExceptionPageExtensions.UseDeveloperExceptionPage*>) reports app runtime errors.
     * Database Error Page Middleware (<xref:Microsoft.AspNetCore.Builder.DatabaseErrorPageExtensions.UseDatabaseErrorPage*>) reports database runtime errors.
   * When the app runs in the Production environment:
     * Exception Handler Middleware (<xref:Microsoft.AspNetCore.Builder.ExceptionHandlerExtensions.UseExceptionHandler*>) catches exceptions thrown in the following middlewares.
     * HTTP Strict Transport Security Protocol (HSTS) Middleware (<xref:Microsoft.AspNetCore.Builder.HstsBuilderExtensions.UseHsts*>) adds the `Strict-Transport-Security` header.
1. HTTPS Redirection Middleware (<xref:Microsoft.AspNetCore.Builder.HttpsPolicyBuilderExtensions.UseHttpsRedirection*>) redirects HTTP requests to HTTPS.
1. Static File Middleware (<xref:Microsoft.AspNetCore.Builder.StaticFileExtensions.UseStaticFiles*>) returns static files and short-circuits further request processing.
1. Cookie Policy Middleware (<xref:Microsoft.AspNetCore.Builder.CookiePolicyAppBuilderExtensions.UseCookiePolicy*>) conforms the app to the EU General Data Protection Regulation (GDPR) regulations.
1. Authentication Middleware (<xref:Microsoft.AspNetCore.Builder.AuthAppBuilderExtensions.UseAuthentication*>) attempts to authenticate the user before they're allowed access to secure resources.
1. Session Middleware (<xref:Microsoft.AspNetCore.Builder.SessionMiddlewareExtensions.UseSession*>) establishes and maintains session state. If the app uses session state, call Session Middleware after Cookie Policy Middleware and before MVC Middleware.
1. MVC (<xref:Microsoft.AspNetCore.Builder.MvcApplicationBuilderExtensions.UseMvc*>) to add MVC to the request pipeline.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCookiePolicy();
    app.UseAuthentication();
    app.UseSession();
    app.UseMvc();
}
```

In the preceding example code, each middleware extension method is exposed on <xref:Microsoft.AspNetCore.Builder.IApplicationBuilder> through the <xref:Microsoft.AspNetCore.Builder?displayProperty=fullName> namespace.

<xref:Microsoft.AspNetCore.Builder.ExceptionHandlerExtensions.UseExceptionHandler*> is the first middleware component added to the pipeline. Therefore, the Exception Handler Middleware catches any exceptions that occur in later calls.

Static File Middleware is called early in the pipeline so that it can handle requests and short-circuit without going through the remaining components. The Static File Middleware provides **no** authorization checks. Any files served by it, including those under *wwwroot*, are publicly available. For an approach to secure static files, see <xref:fundamentals/static-files>.

If the request isn't handled by the Static File Middleware, it's passed on to the Authentication Middleware (<xref:Microsoft.AspNetCore.Builder.AuthAppBuilderExtensions.UseAuthentication*>), which performs authentication. Authentication doesn't short-circuit unauthenticated requests. Although Authentication Middleware authenticates requests, authorization (and rejection) occurs only after MVC selects a specific Razor Page or MVC controller and action.

The following example demonstrates a middleware order where requests for static files are handled by Static File Middleware before Response Compression Middleware. Static files aren't compressed with this middleware order. The MVC responses from <xref:Microsoft.AspNetCore.Builder.MvcApplicationBuilderExtensions.UseMvcWithDefaultRoute*> can be compressed.

```csharp
public void Configure(IApplicationBuilder app)
{
    // Static files not compressed by Static File Middleware.
    app.UseStaticFiles();
    app.UseResponseCompression();
    app.UseMvcWithDefaultRoute();
}
```

## Use, Run, and Map

Configure the HTTP pipeline using <xref:Microsoft.AspNetCore.Builder.UseExtensions.Use*>, <xref:Microsoft.AspNetCore.Builder.RunExtensions.Run*>, and <xref:Microsoft.AspNetCore.Builder.MapExtensions.Map*>. The `Use` method can short-circuit the pipeline (that is, if it doesn't call a `next` request delegate). `Run` is a convention, and some middleware components may expose `Run[Middleware]` methods that run at the end of the pipeline.

<xref:Microsoft.AspNetCore.Builder.MapExtensions.Map*> extensions are used as a convention for branching the pipeline. `Map` branches the request pipeline based on matches of the given request path. If the request path starts with the given path, the branch is executed.

[!code-csharp[](index/snapshot/Chain/StartupMap.cs?name=snippet1)]

The following table shows the requests and responses from `http://localhost:1234` using the previous code.

| Request             | Response                     |
| ------------------- | ---------------------------- |
| localhost:1234      | Hello from non-Map delegate. |
| localhost:1234/map1 | Map Test 1                   |
| localhost:1234/map2 | Map Test 2                   |
| localhost:1234/map3 | Hello from non-Map delegate. |

When `Map` is used, the matched path segments are removed from `HttpRequest.Path` and appended to `HttpRequest.PathBase` for each request.

<xref:Microsoft.AspNetCore.Builder.MapWhenExtensions.MapWhen*> branches the request pipeline based on the result of the given predicate. Any predicate of type `Func<HttpContext, bool>` can be used to map requests to a new branch of the pipeline. In the following example, a predicate is used to detect the presence of a query string variable `branch`:

[!code-csharp[](index/snapshot/Chain/StartupMapWhen.cs?name=snippet1)]

The following table shows the requests and responses from `http://localhost:1234` using the previous code.

| Request                       | Response                     |
| ----------------------------- | ---------------------------- |
| localhost:1234                | Hello from non-Map delegate. |
| localhost:1234/?branch=master | Branch used = master         |

`Map` supports nesting, for example:

```csharp
app.Map("/level1", level1App => {
    level1App.Map("/level2a", level2AApp => {
        // "/level1/level2a" processing
    });
    level1App.Map("/level2b", level2BApp => {
        // "/level1/level2b" processing
    });
});
   ```

`Map` can also match multiple segments at once:

[!code-csharp[](index/snapshot/Chain/StartupMultiSeg.cs?name=snippet1&highlight=13)]

## Built-in middleware

ASP.NET Core ships with the following middleware components. The *Order* column provides notes on middleware placement in the request processing pipeline and under what conditions the middleware may terminate request processing. When a middleware short-circuits the request processing pipeline and prevents further downstream middleware from processing a request, it's called a *terminal middleware*. For more information on short-circuiting, see the [Create a middleware pipeline with IApplicationBuilder](#create-a-middleware-pipeline-with-iapplicationbuilder) section.

| Middleware | Description | Order |
| ---------- | ----------- | ----- |
| [Authentication](xref:security/authentication/identity) | Provides authentication support. | Before `HttpContext.User` is needed. Terminal for OAuth callbacks. |
| [Cookie Policy](xref:security/gdpr) | Tracks consent from users for storing personal information and enforces minimum standards for cookie fields, such as `secure` and `SameSite`. | Before middleware that issues cookies. Examples: Authentication, Session, MVC (TempData). |
| [CORS](xref:security/cors) | Configures Cross-Origin Resource Sharing. | Before components that use CORS. |
| [Exception Handling](xref:fundamentals/error-handling) | Handles exceptions. | Before components that generate errors. |
| [Forwarded Headers](xref:host-and-deploy/proxy-load-balancer) | Forwards proxied headers onto the current request. | Before components that consume the updated fields. Examples: scheme, host, client IP, method. |
| [Health Check](xref:host-and-deploy/health-checks) | Checks the health of an ASP.NET Core app and its dependencies, such as checking database availability. | Terminal if a request matches a health check endpoint. |
| [HTTP Method Override](xref:Microsoft.AspNetCore.Builder.HttpMethodOverrideExtensions) | Allows an incoming POST request to override the method. | Before components that consume the updated method. |
| [HTTPS Redirection](xref:security/enforcing-ssl#require-https) | Redirect all HTTP requests to HTTPS. | Before components that consume the URL. |
| [HTTP Strict Transport Security (HSTS)](xref:security/enforcing-ssl#http-strict-transport-security-protocol-hsts) | Security enhancement middleware that adds a special response header. | Before responses are sent and after components that modify requests. Examples: Forwarded Headers, URL Rewriting. |
| [MVC](xref:mvc/overview) | Processes requests with MVC/Razor Pages. | Terminal if a request matches a route. |
| [OWIN](xref:fundamentals/owin) | Interop with OWIN-based apps, servers, and middleware. | Terminal if the OWIN Middleware fully processes the request. |
| [Response Caching](xref:performance/caching/middleware) | Provides support for caching responses. | Before components that require caching. |
| [Response Compression](xref:performance/response-compression) | Provides support for compressing responses. | Before components that require compression. |
| [Request Localization](xref:fundamentals/localization) | Provides localization support. | Before localization sensitive components. |
| [Routing](xref:fundamentals/routing) | Defines and constrains request routes. | Terminal for matching routes. |
| [Session](xref:fundamentals/app-state) | Provides support for managing user sessions. | Before components that require Session. |
| [Static Files](xref:fundamentals/static-files) | Provides support for serving static files and directory browsing. | Terminal if a request matches a file. |
| [URL Rewriting](xref:fundamentals/url-rewriting) | Provides support for rewriting URLs and redirecting requests. | Before components that consume the URL. |
| [WebSockets](xref:fundamentals/websockets) | Enables the WebSockets protocol. | Before components that are required to accept WebSocket requests. |

## Additional resources

* <xref:fundamentals/middleware/write>
* <xref:migration/http-modules>
* <xref:fundamentals/startup>
* <xref:fundamentals/request-features>
* <xref:fundamentals/middleware/extensibility>
* <xref:fundamentals/middleware/extensibility-third-party-container>
