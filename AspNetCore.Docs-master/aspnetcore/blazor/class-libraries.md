---
title: ASP.NET Core Razor components class libraries
author: guardrex
description: Discover how components can be included in Blazor apps from an external component library.
monikerRange: '>= aspnetcore-3.0'
ms.author: riande
ms.custom: mvc
ms.date: 07/02/2019
uid: blazor/class-libraries
---
# ASP.NET Core Razor components class libraries

By [Simon Timms](https://github.com/stimms)

Components can be shared in a [Razor class library (RCL)](xref:razor-pages/ui-class) across projects. A *Razor components class library* can be included from:

* Another project in the solution.
* A NuGet package.
* A referenced .NET library.

Just as components are regular .NET types, components provided by an RCL are normal .NET assemblies.

## Create an RCL

Follow the guidance in the <xref:blazor/get-started> article to configure your environment for Blazor.

# [Visual Studio](#tab/visual-studio)

1. Create a new project.
1. Select **ASP.NET Core Web Application**. Select **Next**.
1. Provide a project name in the **Project name** field or accept the default project name. The examples in this topic use the project name `MyComponentLib1`. Select **Create**.
1. In the **Create a new ASP.NET Core Web Application** dialog, confirm that **.NET Core** and **ASP.NET Core 3.0** are selected.
1. Select the **Razor Class Library** template. Select **Create**.
1. Add the RCL to a solution:
   1. Right-click the solution. Select **Add** > **Existing Project**.
   1. Navigate to the RCL's project file.
   1. Select the RCL's project file (*.csproj*).
1. Add a reference the RCL from the app:
   1. Right-click the app project. Select **Add** > **Reference**.
   1. Select the RCL project. Select **OK**.

# [.NET Core CLI](#tab/netcore-cli)

1. Use the **Razor Class Library** template (`razorclasslib`) with the [dotnet new](/dotnet/core/tools/dotnet-new) command in a command shell. In the following example, an RCL is created named `MyComponentLib1`. The folder that holds `MyComponentLib1` is created automatically when the command is executed:

   ```console
   dotnet new razorclasslib -o MyComponentLib1
   ```

1. To add the library to an existing project, use the [dotnet add reference](/dotnet/core/tools/dotnet-add-reference) command in a command shell. In the following example, the RCL is added to the app. Execute the following command from the app's project folder with the path to the library:

   ```console
   dotnet add reference {PATH TO LIBRARY}
   ```

---

## RCLs not supported for client-side apps

In the current ASP.NET Core 3.0 Preview, Razor class libraries aren't compatible with Blazor client-side apps. For Blazor client-side apps, use a Blazor component library created by the `blazorlib` template in a command shell:

```console
dotnet new blazorlib -o MyComponentLib1
```

Component libraries using the `blazorlib` template can include static files, such as images, JavaScript, and stylesheets. At build time, static files are embedded into the built assembly file (*.dll*), which allows consumption of the components without having to worry about how to include their resources. Any files included in the `content` directory are marked as an embedded resource.

## Consume a library component

In order to consume components defined in a library in another project, use either of the following approaches:

* Use the full type name with the namespace.
* Use Razor's [\@using](xref:mvc/views/razor#using) directive. Individual components can be added by name.

In the following examples, `MyComponentLib1` is a component library containing a `SalesReport` component.

The `SalesReport` component can be referenced using its full type name with namespace:

```cshtml
<h1>Hello, world!</h1>

Welcome to your new app.

<MyComponentLib1.SalesReport />
```

The component can also be referenced if the library is brought into scope with an `@using` directive:

```cshtml
@using MyComponentLib1

<h1>Hello, world!</h1>

Welcome to your new app.

<SalesReport />
```

Include the `@using MyComponentLib1` directive in the top-level *_Import.razor* file to make the library's components available to an entire project. Add the directive to an *_Import.razor* file at any level to apply the namespace to a single page or set of pages within a folder.

## Build, pack, and ship to NuGet

Because component libraries are standard .NET libraries, packaging and shipping them to NuGet is no different from packaging and shipping any library to NuGet. Packaging is performed using the [dotnet pack](/dotnet/core/tools/dotnet-pack) command in a command shell:

```console
dotnet pack
```

Upload the package to NuGet using the [dotnet nuget publish](/dotnet/core/tools/dotnet-nuget-push) command in a command shell:

```console
dotnet nuget publish
```

When using the `blazorlib` template, static resources are included in the NuGet package. Library consumers automatically receive scripts and stylesheets, so consumers aren't required to manually install the resources.

## Create a Razor components class library with static assets

An RCL can include static assets. The static assets are available to any app that consumes the library. For more information, see <xref:razor-pages/ui-class#create-an-rcl-with-static-assets>.

## Additional resources

* <xref:razor-pages/ui-class>
