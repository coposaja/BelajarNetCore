---
title: Build your first Blazor app
author: guardrex
description: Build a Blazor app step-by-step.
monikerRange: '>= aspnetcore-3.0'
ms.author: riande
ms.custom: mvc
ms.date: 07/01/2019
uid: tutorials/first-blazor-app
---
# Build your first Blazor app

By [Daniel Roth](https://github.com/danroth27) and [Luke Latham](https://github.com/guardrex)

This tutorial shows you how to build and modify a Blazor app.

Follow the guidance in the <xref:blazor/get-started> article to create a Blazor project for this tutorial. Name the project *ToDoList*.

## Build components

1. Browse to each of the app's three pages in the *Pages* folder: Home, Counter, and Fetch data. These pages are implemented by the Razor component files *Index.razor*, *Counter.razor*, and *FetchData.razor*.

1. On the Counter page, select the **Click me** button to increment the counter without a page refresh. Incrementing a counter in a webpage normally requires writing JavaScript, but Blazor provides a better approach using C#.

1. Examine the implementation of the `Counter` component in the *Counter.razor* file.

   *Pages/Counter.razor*:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/Counter1.razor)]

   The UI of the `Counter` component is defined using HTML. Dynamic rendering logic (for example, loops, conditionals, expressions) is added using an embedded C# syntax called [Razor](xref:mvc/views/razor). The HTML markup and C# rendering logic are converted into a component class at build time. The name of the generated .NET class matches the file name.

   Members of the component class are defined in an `@code` block. In the `@code` block, component state (properties, fields) and methods are specified for event handling or for defining other component logic. These members are then used as part of the component's rendering logic and for handling events.

   When the **Click me** button is selected:

   * The `Counter` component's registered `onclick` handler is called (the `IncrementCount` method).
   * The `Counter` component regenerates its render tree.
   * The new render tree is compared to the previous one.
   * Only modifications to the Document Object Model (DOM) are applied. The displayed count is updated.

1. Modify the C# logic of the `Counter` component to make the count increment by two instead of one.

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/Counter2.razor?highlight=14)]

1. Rebuild and run the app to see the changes. Select the **Click me** button. The counter increments by two.

## Use components

Include a component in another component using an HTML syntax.

1. Add the `Counter` component to the app's `Index` component by adding a `<Counter />` element to the `Index` component (*Index.razor*).

   If you're using Blazor client-side for this experience, a `SurveyPrompt` component is used by the `Index` component. Replace the `<SurveyPrompt>` element with a `<Counter />` element. If you're using a Blazor server-side app for this experience, add the `<Counter />` element to the `Index` component:

   *Pages/Index.razor*:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/Index1.razor?highlight=7)]

1. Rebuild and run the app. The `Index` component has its own counter.

## Component parameters

Components can also have parameters. Component parameters are defined using non-public properties on the component class decorated with `[Parameter]`. Use attributes to specify arguments for a component in markup.

1. Update the component's `@code` C# code:

   * Add a `IncrementAmount` property decorated with the `[Parameter]` attribute.
   * Change the `IncrementCount` method to use the `IncrementAmount` when increasing the value of `currentCount`.

   *Pages/Counter.razor*:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/Counter.razor?highlight=13,17)]

<!-- Add back when supported.
   > [!NOTE]
   > From Visual Studio, you can quickly add a component parameter by using the `para` snippet. Type `para` and press the `Tab` key twice.
-->

1. Specify an `IncrementAmount` parameter in the `Index` component's `<Counter>` element using an attribute. Set the value to increment the counter by ten.

   *Pages/Index.razor*:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/Index2.razor?highlight=7)]

1. Reload the `Index` component. The counter increments by ten each time the **Click me** button is selected. The counter in the `Counter` component continues to increment by one.

## Route to components

The `@page` directive at the top of the *Counter.razor* file specifies that the `Counter` component is a routing endpoint. The `Counter` component handles requests sent to `/counter`. Without the `@page` directive, a component doesn't handle routed requests, but the component can still be used by other components.

## Dependency injection

Services registered in the app's service container are available to components via [dependency injection (DI)](xref:fundamentals/dependency-injection). Inject services into a component using the `@inject` directive.

Examine the directives of the `FetchData` component.

If working with a Blazor server-side app, the `WeatherForecastService` service is registered as a [singleton](xref:fundamentals/dependency-injection#service-lifetimes), so one instance of the service is available throughout the app. The `@inject` directive is used to inject the instance of the `WeatherForecastService` service into the component.

*Pages/FetchData.razor*:

[!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/FetchData1.razor?highlight=3)]

The `FetchData` component uses the injected service, as `ForecastService`, to retrieve an array of `WeatherForecast` objects:

[!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/FetchData2.razor?highlight=6)]

If working with a Blazor client-side app, `HttpClient` is injected to obtain weather forecast data from the *weather.json* file in the *wwwroot/sample-data* folder:

*Pages/FetchData.razor*:

[!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/FetchData1_client.razor?highlight=7-8)]

A [\@foreach](/dotnet/csharp/language-reference/keywords/foreach-in) loop is used to render each forecast instance as a row in the table of weather data:

[!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/FetchData3.razor?highlight=11-19)]


## Build a todo list

Add a new component to the app that implements a simple todo list.

1. Add an empty file named *Todo.razor* to the app in the *Pages* folder:

1. Provide the initial markup for the component:

   ```cshtml
   @page "/todo"

   <h1>Todo</h1>
   ```

1. Add the `Todo` component to the navigation bar.

   The `NavMenu` component (*Shared/NavMenu.razor*) is used in the app's layout. Layouts are components that allow you to avoid duplication of content in the app.

   Add a `<NavLink>` element for the `Todo` component by adding the following list item markup below the existing list items in the *Shared/NavMenu.razor* file:

   ```cshtml
   <li class="nav-item px-3">
       <NavLink class="nav-link" href="todo">
           <span class="oi oi-list-rich" aria-hidden="true"></span> Todo
       </NavLink>
   </li>
   ```

1. Rebuild and run the app. Visit the new Todo page to confirm that the link to the `Todo` component works.

1. Add a *TodoItem.cs* file to the root of the project to hold a class that represents a todo item. Use the following C# code for the `TodoItem` class:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/TodoItem.cs)]

1. Return to the `Todo` component (*Pages/Todo.razor*):

   * Add a field for the todo items in an `@code` block. The `Todo` component uses this field to maintain the state of the todo list.
   * Add unordered list markup and a `foreach` loop to render each todo item as a list item (`<li>`).

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/ToDo4.razor?highlight=5-10,12-14)]

1. The app requires UI elements for adding todo items to the list. Add a text input (`<input>`) and a button (`<button>`) below the unordered list (`<ul>...</ul>`):

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/ToDo5.razor?highlight=12-13)]

1. Rebuild and run the app. When the **Add todo** button is selected, nothing happens because an event handler isn't wired up to the button.

1. Add an `AddTodo` method to the `Todo` component and register it for button selections using the `@onclick` attribute. The `AddTodo` C# method is called when the button is selected:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/ToDo6.razor?highlight=2,7-10)]

1. To get the title of the new todo item, add a `newTodo` string field at the top of the `@code` block and bind it to the value of the text input using the `bind` attribute in the `<input>` element:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/ToDo7.razor?highlight=2)]

   ```cshtml
   <input placeholder="Something todo" @bind="@newTodo" />
   ```

1. Update the `AddTodo` method to add the `TodoItem` with the specified title to the list. Clear the value of the text input by setting `newTodo` to an empty string:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/ToDo8.razor?highlight=19-26)]

1. Rebuild and run the app. Add some todo items to the todo list to test the new code.

1. The title text for each todo item can be made editable, and a check box can help the user keep track of completed items. Add a check box input for each todo item and bind its value to the `IsDone` property. Change `@todo.Title` to an `<input>` element bound to `@todo.Title`:

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/ToDo9.razor?highlight=5-6)]

1. To verify that these values are bound, update the `<h1>` header to show a count of the number of todo items that aren't complete (`IsDone` is `false`).

   ```cshtml
   <h1>Todo (@todos.Count(todo => !todo.IsDone))</h1>
   ```

1. The completed `Todo` component (*Pages/Todo.razor*):

   [!code-cshtml[](build-your-first-blazor-app/samples_snapshot/3.x/Todo.razor)]

1. Rebuild and run the app. Add todo items to test the new code.

> [!div class="nextstepaction"]
> <xref:blazor/components>
