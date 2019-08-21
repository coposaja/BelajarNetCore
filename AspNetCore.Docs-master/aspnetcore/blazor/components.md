---
title: Create and use ASP.NET Core Razor components
author: guardrex
description: Learn how to create and use Razor components, including how to bind to data, handle events, and manage component life cycles.
monikerRange: '>= aspnetcore-3.0'
ms.author: riande
ms.custom: mvc
ms.date: 08/02/2019
uid: blazor/components
---
# Create and use ASP.NET Core Razor components

By [Luke Latham](https://github.com/guardrex) and [Daniel Roth](https://github.com/danroth27)

[View or download sample code](https://github.com/aspnet/AspNetCore.Docs/tree/master/aspnetcore/blazor/common/samples/) ([how to download](xref:index#how-to-download-a-sample))

Blazor apps are built using *components*. A component is a self-contained chunk of user interface (UI), such as a page, dialog, or form. A component includes HTML markup and the processing logic required to inject data or respond to UI events. Components are flexible and lightweight. They can be nested, reused, and shared among projects.

## Component classes

Components are implemented in [Razor](xref:mvc/views/razor) component files (*.razor*) using a combination of C# and HTML markup. A component in Blazor is formally referred to as a *Razor component*.

Components can be authored using the *.cshtml* file extension. Use the `_RazorComponentInclude` MSBuild property in the project file to identify the component *.cshtml* files. For example, an app that specifies that all *.cshtml* files under the *Pages* folder should be treated as Razor components files:

```xml
<PropertyGroup>
  <_RazorComponentInclude>Pages\**\*.cshtml</_RazorComponentInclude>
</PropertyGroup>
```

The UI for a component is defined using HTML. Dynamic rendering logic (for example, loops, conditionals, expressions) is added using an embedded C# syntax called [Razor](xref:mvc/views/razor). When an app is compiled, the HTML markup and C# rendering logic are converted into a component class. The name of the generated class matches the name of the file.

Members of the component class are defined in an `@code` block. In the `@code` block, component state (properties, fields) is specified with methods for event handling or for defining other component logic. More than one `@code` block is permissible.

> [!NOTE]
> In prior previews of ASP.NET Core 3.0, `@functions` blocks were used for the same purpose as `@code` blocks in Razor components. `@functions` blocks continue to function in Razor components, but we recommend using the `@code` block in ASP.NET Core 3.0 Preview 6 or later.

Component members can be used as part of the component's rendering logic using C# expressions that start with `@`. For example, a C# field is rendered by prefixing `@` to the field name. The following example evaluates and renders:

* `_headingFontStyle` to the CSS property value for `font-style`.
* `_headingText` to the content of the `<h1>` element.

```cshtml
<h1 style="font-style:@_headingFontStyle">@_headingText</h1>

@code {
    private string _headingFontStyle = "italic";
    private string _headingText = "Put on your new Blazor!";
}
```

After the component is initially rendered, the component regenerates its render tree in response to events. Blazor then compares the new render tree against the previous one and applies any modifications to the browser's Document Object Model (DOM).

Components are ordinary C# classes and can be placed anywhere within a project. Components that produce webpages usually reside in the *Pages* folder. Non-page components are frequently placed in the *Shared* folder or a custom folder added to the project. To use a custom folder, add the custom folder's namespace to either the parent component or to the app's *_Imports.razor* file. For example, the following namespace makes components in a *Components* folder available when the app's root namespace is `WebApplication`:

```cshtml
@using WebApplication.Components
```

## Integrate components into Razor Pages and MVC apps

Use components with existing Razor Pages and MVC apps. There's no need to rewrite existing pages or views to use Razor components. When the page or view is rendered, components are prerendered at the same time.

To render a component from a page or view, use the `RenderComponentAsync<TComponent>` HTML helper method:

```cshtml
<div id="Counter">
    @(await Html.RenderComponentAsync<Counter>(new { IncrementAmount = 10 }))
</div>
```

While pages and views can use components, the converse isn't true. Components can't use view- and page-specific scenarios, such as partial views and sections. To use logic from partial view in a component, factor out the partial view logic into a component.

For more information on how components are rendered and component state is managed in Blazor server-side apps, see the <xref:blazor/hosting-models> article.

## Using components

Components can include other components by declaring them using HTML element syntax. The markup for using a component looks like an HTML tag where the name of the tag is the component type.

The following markup in *Index.razor* renders a `HeadingComponent` instance:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/Index.razor?name=snippet_HeadingComponent)]

*Components/HeadingComponent.razor*:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/HeadingComponent.razor)]

## Component parameters

Components can have *component parameters*, which are defined using properties (usually *non-public*) on the component class with the `[Parameter]` attribute. Use attributes to specify arguments for a component in markup.

*Components/ChildComponent.razor*:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/ChildComponent.razor?highlight=11-12)]

In the following example, the `ParentComponent` sets the value of the `Title` property of the `ChildComponent`.

*Pages/ParentComponent.razor*:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/ParentComponent.razor?name=snippet_ParentComponent&highlight=5-6)]

## Child content

Components can set the content of another component. The assigning component provides the content between the tags that specify the receiving component.

In the following example, the `ChildComponent` has a `ChildContent` property that represents a `RenderFragment`, which represents a segment of UI to render. The value of `ChildContent` is positioned in the component's markup where the content should be rendered. The value of `ChildContent` is received from the parent component and rendered inside the Bootstrap panel's `panel-body`.

*Components/ChildComponent.razor*:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/ChildComponent.razor?highlight=3,14-15)]

> [!NOTE]
> The property receiving the `RenderFragment` content must be named `ChildContent` by convention.

The following `ParentComponent` can provide content for rendering the `ChildComponent` by placing the content inside the `<ChildComponent>` tags.

*Pages/ParentComponent.razor*:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/ParentComponent.razor?name=snippet_ParentComponent&highlight=7-8)]

## Attribute splatting and arbitrary parameters

Components can capture and render additional attributes in addition to the component's declared parameters. Additional attributes can be captured in a dictionary and then *splatted* onto an element when the component is rendered using the [@attributes](xref:mvc/views/razor#attributes) Razor directive. This scenario is useful when defining a component that produces a markup element that supports a variety of customizations. For example, it can be tedious to define attributes separately for an `<input>` that supports many parameters.

In the following example, the first `<input>` element (`id="useIndividualParams"`) uses individual component parameters, while the second `<input>` element (`id="useAttributesDict"`) uses attribute splatting:

```cshtml
<input id="useIndividualParams"
       maxlength="@Maxlength"
       placeholder="@Placeholder"
       required="@Required"
       size="@Size" />

<input id="useAttributesDict"
       @attributes="InputAttributes" />

@code {
    [Parameter]
    private string Maxlength { get; set; } = "10";

    [Parameter]
    private string Placeholder { get; set; } = "Input placeholder text";

    [Parameter]
    private string Required { get; set; } = "required";

    [Parameter]
    private string Size { get; set; } = "50";

    [Parameter]
    private Dictionary<string, object> InputAttributes { get; set; } =
        new Dictionary<string, object>()
        {
            { "maxlength", "10" },
            { "placeholder", "Input placeholder text" },
            { "required", "true" },
            { "size", "50" }
        };
}
```

The type of the parameter must implement `IEnumerable<KeyValuePair<string, object>>` with string keys. Using `IReadOnlyDictionary<string, object>` is also an option in this scenario.

The rendered `<input>` elements using both approaches is identical:

```html
<input id="useIndividualParams"
       maxlength="10"
       placeholder="Input placeholder text"
       required="required"
       size="50">

<input id="useAttributesDict"
       maxlength="10"
       placeholder="Input placeholder text"
       required="true"
       size="50">
```

To accept arbitrary attributes, define a component parameter using the `[Parameter]` attribute with the `CaptureUnmatchedValues` property set to `true`:

```cshtml
@code {
    [Parameter(CaptureUnmatchedValues = true)]
    private Dictionary<string, object> InputAttributes { get; set; }
}
```

The `CaptureUnmatchedValues` property on `[Parameter]` allows the parameter to match all attributes that don't match any other parameter. A component can only define a single parameter with `CaptureUnmatchedValues`. The property type used with `CaptureUnmatchedValues` must be assignable from `Dictionary<string, object>` with string keys. `IEnumerable<KeyValuePair<string, object>>` or `IReadOnlyDictionary<string, object>` are also options in this scenario.

## Data binding

Data binding to both components and DOM elements is accomplished with the [@bind](xref:mvc/views/razor#bind) attribute. The following example binds the `_italicsCheck` field to the check box's checked state:

```cshtml
<input type="checkbox" class="form-check-input" id="italicsCheck" 
    @bind="_italicsCheck" />
```

When the check box is selected and cleared, the property's value is updated to `true` and `false`, respectively.

The check box is updated in the UI only when the component is rendered, not in response to changing the property's value. Since components render themselves after event handler code executes, property updates are usually reflected in the UI immediately.

Using `@bind` with a `CurrentValue` property (`<input @bind="CurrentValue" />`) is essentially equivalent to the following:

```cshtml
<input value="@CurrentValue"
    @onchange="@((UIChangeEventArgs __e) => CurrentValue = __e.Value)" />
```

When the component is rendered, the `value` of the input element comes from the `CurrentValue` property. When the user types in the text box, the `onchange` event is fired and the `CurrentValue` property is set to the changed value. In reality, the code generation is a little more complex because `@bind` handles a few cases where type conversions are performed. In principle, `@bind` associates the current value of an expression with a `value` attribute and handles changes using the registered handler.

In addition to handling `onchange` events with `@bind` syntax, a property or field can be bound using other events by specifying an [@bind-value](xref:mvc/views/razor#bind) attribute with an `event` parameter ([@bind-value:event](xref:mvc/views/razor#bind)). The following example binds the `CurrentValue` property for the `oninput` event:

```cshtml
<input @bind-value="CurrentValue" @bind-value:event="oninput" />
```

Unlike `onchange`, which fires when the element loses focus, `oninput` fires when the value of the text box changes.

**Format strings**

Data binding works with <xref:System.DateTime> format strings using [@bind:format](xref:mvc/views/razor#bind). Other format expressions, such as currency or number formats, aren't available at this time.

```cshtml
<input @bind="StartDate" @bind:format="yyyy-MM-dd" />

@code {
    [Parameter]
    private DateTime StartDate { get; set; } = new DateTime(2020, 1, 1);
}
```

The `@bind:format` attribute specifies the date format to apply to the `value` of the `<input>` element. The format is also used to parse the value when an `onchange` event occurs.

**Component parameters**

Binding recognizes component parameters, where `@bind-{property}` can bind a property value across components.

The following child component (`ChildComponent`) has a `Year` component parameter and `YearChanged` callback:

```cshtml
<h2>Child Component</h2>

<p>Year: @Year</p>

@code {
    [Parameter]
    private int Year { get; set; }

    [Parameter]
    private EventCallback<int> YearChanged { get; set; }
}
```

`EventCallback<T>` is explained in the [EventCallback](#eventcallback) section.

The following parent component uses `ChildComponent` and binds the `ParentYear` parameter from the parent to the `Year` parameter on the child component:

```cshtml
@page "/ParentComponent"

<h1>Parent Component</h1>

<p>ParentYear: @ParentYear</p>

<ChildComponent @bind-Year="ParentYear" />

<button class="btn btn-primary" @onclick="ChangeTheYear">
    Change Year to 1986
</button>

@code {
    [Parameter]
    private int ParentYear { get; set; } = 1978;

    private void ChangeTheYear()
    {
        ParentYear = 1986;
    }
}
```

Loading the `ParentComponent` produces the following markup:

```html
<h1>Parent Component</h1>

<p>ParentYear: 1978</p>

<h2>Child Component</h2>

<p>Year: 1978</p>
```

If the value of the `ParentYear` property is changed by selecting the button in the `ParentComponent`, the `Year` property of the `ChildComponent` is updated. The new value of `Year` is rendered in the UI when the `ParentComponent` is rerendered:

```html
<h1>Parent Component</h1>

<p>ParentYear: 1986</p>

<h2>Child Component</h2>

<p>Year: 1986</p>
```

The `Year` parameter is bindable because it has a companion `YearChanged` event that matches the type of the `Year` parameter.

By convention, `<ChildComponent @bind-Year="ParentYear" />` is essentially equivalent to writing:

```cshtml
<ChildComponent @bind-Year="ParentYear" @bind-Year:event="YearChanged" />
```

In general, a property can be bound to a corresponding event handler using `@bind-property:event` attribute. For example, the property `MyProp` can be bound to `MyEventHandler` using the following two attributes:

```cshtml
<MyComponent @bind-MyProp="MyValue" @bind-MyProp:event="MyEventHandler" />
```

## Event handling

Razor components provide event handling features. For an HTML element attribute named `on{event}` (for example, `onclick` and `onsubmit`) with a delegate-typed value, Razor components treats the attribute's value as an event handler. The attribute's name is always formatted [@on{event}](xref:mvc/views/razor#onevent).

The following code calls the `UpdateHeading` method when the button is selected in the UI:

```cshtml
<button class="btn btn-primary" @onclick="UpdateHeading">
    Update heading
</button>

@code {
    private void UpdateHeading(UIMouseEventArgs e)
    {
        ...
    }
}
```

The following code calls the `CheckChanged` method when the check box is changed in the UI:

```cshtml
<input type="checkbox" class="form-check-input" @onchange="CheckChanged" />

@code {
    private void CheckChanged()
    {
        ...
    }
}
```

Event handlers can also be asynchronous and return a <xref:System.Threading.Tasks.Task>. There's no need to manually call `StateHasChanged()`. Exceptions are logged when they occur.

In the following example, `UpdateHeading` is called asynchronously when the button is selected:

```cshtml
<button class="btn btn-primary" @onclick="UpdateHeading">
    Update heading
</button>

@code {
    private async Task UpdateHeading(UIMouseEventArgs e)
    {
        ...
    }
}
```

### Event argument types

For some events, event argument types are permitted. If access to one of these event types isn't necessary, it isn't required in the method call.

Supported [UIEventArgs](https://github.com/aspnet/AspNetCore/blob/release/3.0-preview8/src/Components/Components/src/UIEventArgs.cs) are shown in the following table.

| Event | Class |
| ----- | ----- |
| Clipboard | `UIClipboardEventArgs` |
| Drag  | `UIDragEventArgs` &ndash; `DataTransfer` is used to hold the dragged data during a drag and drop operation and may hold one or more `UIDataTransferItem`. `UIDataTransferItem` represents one drag data item. |
| Error | `UIErrorEventArgs` |
| Focus | `UIFocusEventArgs` &ndash; Doesn't include support for `relatedTarget`. |
| `<input>` change | `UIChangeEventArgs` |
| Keyboard | `UIKeyboardEventArgs` |
| Mouse | `UIMouseEventArgs` |
| Mouse pointer | `UIPointerEventArgs` |
| Mouse wheel | `UIWheelEventArgs` |
| Progress | `UIProgressEventArgs` |
| Touch | `UITouchEventArgs` &ndash; `UITouchPoint` represents a single contact point on a touch-sensitive device. |

For information on the properties and event handling behavior of the events in the preceding table, see [EventArgs classes in the reference source](https://github.com/aspnet/AspNetCore/tree/release/3.0-preview8/src/Components/Web/src).

### Lambda expressions

Lambda expressions can also be used:

```cshtml
<button @onclick="@(e => Console.WriteLine("Hello, world!"))">Say hello</button>
```

It's often convenient to close over additional values, such as when iterating over a set of elements. The following example creates three buttons, each of which calls `UpdateHeading` passing an event argument (`UIMouseEventArgs`) and its button number (`buttonNumber`) when selected in the UI:

```cshtml
<h2>@message</h2>

@for (var i = 1; i < 4; i++)
{
    var buttonNumber = i;

    <button class="btn btn-primary"
            @onclick="@(e => UpdateHeading(e, buttonNumber))">
        Button #@i
    </button>
}

@code {
    private string message = "Select a button to learn its position.";

    private void UpdateHeading(UIMouseEventArgs e, int buttonNumber)
    {
        message = $"You selected Button #{buttonNumber} at " +
            $"mouse position: {e.ClientX} X {e.ClientY}.";
    }
}
```

> [!NOTE]
> Do **not** use the loop variable (`i`) in a `for` loop directly in a lambda expression. Otherwise the same variable is used by all lambda expressions causing `i`'s value to be the same in all lambdas. Always capture its value in a local variable (`buttonNumber` in the preceding example) and then use it.

### EventCallback

A common scenario with nested components is the desire to run a parent component's method when a child component event occurs&mdash;for example, when an `onclick` event occurs in the child. To expose events across components, use an `EventCallback`. A parent component can assign a callback method to a child component's `EventCallback`.

The `ChildComponent` in the sample app demonstrates how a button's `onclick` handler is set up to receive an `EventCallback` delegate from the sample's `ParentComponent`. The `EventCallback` is typed with `UIMouseEventArgs`, which is appropriate for an `onclick` event from a peripheral device:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/ChildComponent.razor?highlight=5-7,17-18)]

The `ParentComponent` sets the child's `EventCallback<T>` to its `ShowMessage` method:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/ParentComponent.razor?name=snippet_ParentComponent&highlight=6,16-19)]

When the button is selected in the `ChildComponent`:

* The `ParentComponent`'s `ShowMessage` method is called. `messageText` is updated and displayed in the `ParentComponent`.
* A call to `StateHasChanged` isn't required in the callback's method (`ShowMessage`). `StateHasChanged` is called automatically to rerender the `ParentComponent`, just as child events trigger component rerendering in event handlers that execute within the child.

`EventCallback` and `EventCallback<T>` permit asynchronous delegates. `EventCallback<T>` is strongly typed and requires a specific argument type. `EventCallback` is weakly typed and allows any argument type.

```cshtml
<p><b>@messageText</b></p>

@{ var message = "Default Text"; }

<ChildComponent 
    OnClick="@(async () => { await Task.Yield(); messageText = "Blaze It!"; })" />

@code {
    private string messageText;
}
```

Invoke an `EventCallback` or `EventCallback<T>` with `InvokeAsync` and await the <xref:System.Threading.Tasks.Task>:

```csharp
await callback.InvokeAsync(arg);
```

Use `EventCallback` and `EventCallback<T>` for event handling and binding component parameters.

Prefer the strongly typed `EventCallback<T>` over `EventCallback`. `EventCallback<T>` provides better error feedback to users of the component. Similar to other UI event handlers, specifying the event parameter is optional. Use `EventCallback` when there's no value passed to the callback.

## Capture references to components

Component references provide a way to reference a component instance so that you can issue commands to that instance, such as `Show` or `Reset`. To capture a component reference, add a [@ref](xref:mvc/views/razor#ref) attribute to the child component and then define a field with the same name and the same type as the child component.

```cshtml
<MyLoginDialog @ref="loginDialog" ... />

@code {
    private MyLoginDialog loginDialog;

    private void OnSomething()
    {
        loginDialog.Show();
    }
}
```

When the component is rendered, the `loginDialog` field is populated with the `MyLoginDialog` child component instance. You can then invoke .NET methods on the component instance.

> [!IMPORTANT]
> The `loginDialog` variable is only populated after the component is rendered and its output includes the `MyLoginDialog` element. Until that point, there's nothing to reference. To manipulate components references after the component has finished rendering, use the `OnAfterRenderAsync` or `OnAfterRender` methods.

While capturing component references use a similar syntax to [capturing element references](xref:blazor/javascript-interop#capture-references-to-elements), it isn't a [JavaScript interop](xref:blazor/javascript-interop) feature. Component references aren't passed to JavaScript code&mdash;they're only used in .NET code.

> [!NOTE]
> Do **not** use component references to mutate the state of child components. Instead, use normal declarative parameters to pass data to child components. Use of normal declarative parameters result in child components that rerender at the correct times automatically.

## Use \@key to control the preservation of elements and components

When rendering a list of elements or components and the elements or components subsequently change, Blazor's diffing algorithm must decide which of the previous elements or components can be retained and how model objects should map to them. Normally, this process is automatic and can be ignored, but there are cases where you may want to control the process.

Consider the following example:

```csharp
@foreach (var person in People)
{
    <DetailsEditor Details="@person.Details" />
}

@code {
    [Parameter]
    private IEnumerable<Person> People { get; set; }
}
```

The contents of the `People` collection may change with inserted, deleted, or re-ordered entries. When the component rerenders, the `<DetailsEditor>` component may change to receive different `Details` parameter values. This may cause more complex rerendering than expected. In some cases, rerendering can lead to visible behavior differences, such as lost element focus.

The mapping process can be controlled with the `@key` directive attribute. `@key` causes the diffing algorithm to guarantee preservation of elements or components based on the key's value:

```csharp
@foreach (var person in People)
{
    <DetailsEditor @key="@person" Details="@person.Details" />
}

@code {
    [Parameter]
    private IEnumerable<Person> People { get; set; }
}
```

When the `People` collection changes, the diffing algorithm retains the association between `<DetailsEditor>` instances and `person` instances:

* If a `Person` is deleted from the `People` list, only the corresponding `<DetailsEditor>` instance is removed from the UI. Other instances are left unchanged.
* If a `Person` is inserted at some position in the list, one new `<DetailsEditor>` instance is inserted at that corresponding position. Other instances are left unchanged.
* If `Person` entries are re-ordered, the corresponding `<DetailsEditor>` instances are preserved and re-ordered in the UI.

In some scenarios, use of `@key` minimizes the complexity of rerendering and avoids potential issues with stateful parts of the DOM changing, such as focus position.

> [!IMPORTANT]
> Keys are local to each container element or component. Keys aren't compared globally across the document.

### When to use \@key

Typically, it makes sense to use `@key` whenever a list is rendered (for example, in a `@foreach` block) and a suitable value exists to define the `@key`.

You can also use `@key` to prevent Blazor from preserving an element or component subtree when an object changes:

```cshtml
<div @key="@currentPerson">
    ... content that depends on @currentPerson ...
</div>
```

If `@currentPerson` changes, the `@key` attribute directive forces Blazor to discard the entire `<div>` and its descendants and rebuild the subtree within the UI with new elements and components. This can be useful if you need to guarantee that no UI state is preserved when `@currentPerson` changes.

### When not to use \@key

There's a performance cost when diffing with `@key`. The performance cost isn't large, but only specify `@key` if controlling the element or component preservation rules benefit the app.

Even if `@key` isn't used, Blazor preserves child element and component instances as much as possible. The only advantage to using `@key` is control over *how* model instances are mapped to the preserved component instances, instead of the diffing algorithm selecting the mapping.

### What values to use for \@key

Generally, it makes sense to supply one of the following kinds of value for `@key`:

* Model object instances (for example, a `Person` instance as in the earlier example). This ensures preservation based on object reference equality.
* Unique identifiers (for example, primary key values of type `int`, `string`, or `Guid`).

Avoid supplying a value that can clash unexpectedly. If `@key="@someObject.GetHashCode()"` is supplied, unexpected clashes may occur because the hash codes of unrelated objects can be the same. If clashing `@key` values are requested within the same parent, the `@key` values won't be honored.

## Lifecycle methods

`OnInitAsync` and `OnInit` execute code to initialize the component. To perform an asynchronous operation, use `OnInitAsync` and the `await` keyword on the operation:

```csharp
protected override async Task OnInitAsync()
{
    await ...
}
```

For a synchronous operation, use `OnInit`:

```csharp
protected override void OnInit()
{
    ...
}
```

`OnParametersSetAsync` and `OnParametersSet` are called when a component has received parameters from its parent and the values are assigned to properties. These methods are executed after component initialization and each time the component is rendered:

```csharp
protected override async Task OnParametersSetAsync()
{
    await ...
}
```

```csharp
protected override void OnParametersSet()
{
    ...
}
```

`OnAfterRenderAsync` and `OnAfterRender` are called after a component has finished rendering. Element and component references are populated at this point. Use this stage to perform additional initialization steps using the rendered content, such as activating third-party JavaScript libraries that operate on the rendered DOM elements.

```csharp
protected override async Task OnAfterRenderAsync()
{
    await ...
}
```

```csharp
protected override void OnAfterRender()
{
    ...
}
```

### Handle incomplete async actions at render

Asynchronous actions performed in lifecycle events may not have completed before the component is rendered. Objects might be `null` or incompletely populated with data while the lifecycle method is executing. Provide rendering logic to confirm that objects are initialized. Render placeholder UI elements (for example, a loading message) while objects are `null`.

In the `FetchData` component of the Blazor templates, `OnInitAsync` is overridden to asychronously receive forecast data (`forecasts`). When `forecasts` is `null`, a loading message is displayed to the user. After the `Task` returned by `OnInitAsync` completes, the component is rerendered with the updated state.

*Pages/FetchData.razor*:

[!code-cshtml[](components/samples_snapshot/3.x/FetchData.razor?highlight=9)]

### Execute code before parameters are set

`SetParameters` can be overridden to execute code before parameters are set:

```csharp
public override void SetParameters(ParameterCollection parameters)
{
    ...

    base.SetParameters(parameters);
}
```

If `base.SetParameters` isn't invoked, the custom code can interpret the incoming parameters value in any way required. For example, the incoming parameters aren't required to be assigned to the properties on the class.

### Suppress refreshing of the UI

`ShouldRender` can be overridden to suppress refreshing of the UI. If the implementation returns `true`, the UI is refreshed. Even if `ShouldRender` is overridden, the component is always initially rendered.

```csharp
protected override bool ShouldRender()
{
    var renderUI = true;

    return renderUI;
}
```

## Component disposal with IDisposable

If a component implements <xref:System.IDisposable>, the [Dispose method](/dotnet/standard/garbage-collection/implementing-dispose) is called when the component is removed from the UI. The following component uses `@implements IDisposable` and the `Dispose` method:

```csharp
@using System
@implements IDisposable

...

@code {
    public void Dispose()
    {
        ...
    }
}
```

## Routing

Routing in Blazor is achieved by providing a route template to each accessible component in the app.

When a Razor file with an `@page` directive is compiled, the generated class is given a <xref:Microsoft.AspNetCore.Mvc.RouteAttribute> specifying the route template. At runtime, the router looks for component classes with a `RouteAttribute` and renders whichever component has a route template that matches the requested URL.

Multiple route templates can be applied to a component. The following component responds to requests for `/BlazorRoute` and `/DifferentBlazorRoute`:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/BlazorRoute.razor?name=snippet_BlazorRoute)]

## Route parameters

Components can receive route parameters from the route template provided in the `@page` directive. The router uses route parameters to populate the corresponding component parameters.

*Route Parameter component*:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/RouteParameter.razor?name=snippet_RouteParameter)]

Optional parameters aren't supported, so two `@page` directives are applied in the example above. The first permits navigation to the component without a parameter. The second `@page` directive takes the `{text}` route parameter and assigns the value to the `Text` property.

## Base class inheritance for a "code-behind" experience

Component files mix HTML markup and C# processing code in the same file. The `@inherits` directive can be used to provide Blazor apps with a "code-behind" experience that separates component markup from processing code.

The [sample app](https://github.com/aspnet/AspNetCore.Docs/tree/master/aspnetcore/blazor/common/samples/) shows how a component can inherit a base class, `BlazorRocksBase`, to provide the component's properties and methods.

*Pages/BlazorRocks.razor*:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/BlazorRocks.razor?name=snippet_BlazorRocks)]

*BlazorRocksBase.cs*:

[!code-csharp[](common/samples/3.x/BlazorSample/Pages/BlazorRocksBase.cs)]

The base class should derive from `ComponentBase`.

## Import components

The namespace of a component authored with Razor is based on:

* The project's `RootNamespace`.
* The path from the project root to the component. For example, `ComponentsSample/Pages/Index.razor` is in the namespace `ComponentsSample.Pages`. Components follow C# name binding rules. In the case of *Index.razor*, all components in the same folder, *Pages*, and the parent folder, *ComponentsSample*, are in scope.

Components defined in a different namespace can be brought into scope using Razor's [\@using](xref:mvc/views/razor#using) directive.

If another component, `NavMenu.razor`, exists in the folder `ComponentsSample/Shared/`, the component can be used in `Index.razor` with the following `@using` statement:

```cshtml
@using ComponentsSample.Shared

This is the Index page.

<NavMenu></NavMenu>
```

Components can also be referenced using their fully qualified names, which removes the need for the [\@using](xref:mvc/views/razor#using) directive:

```cshtml
This is the Index page.

<ComponentsSample.Shared.NavMenu></ComponentsSample.Shared.NavMenu>
```

> [!NOTE]
> The `global::` qualification isn't supported.
>
> Importing components with aliased `using` statements (for example, `@using Foo = Bar`) isn't supported.
>
> Partially qualified names aren't supported. For example, adding `@using ComponentsSample` and referencing `NavMenu.razor` with `<Shared.NavMenu></Shared.NavMenu>` isn't supported.

## Conditional HTML element attributes

HTML element attributes are conditionally rendered based on the .NET value. If the value is `false` or `null`, the attribute isn't rendered. If the value is `true`, the attribute is rendered minimized.

In the following example, `IsCompleted` determines if `checked` is rendered in the element's markup:

```cshtml
<input type="checkbox" checked="@IsCompleted" />

@code {
    [Parameter]
    private bool IsCompleted { get; set; }
}
```

If `IsCompleted` is `true`, the check box is rendered as:

```html
<input type="checkbox" checked />
```

If `IsCompleted` is `false`, the check box is rendered as:

```html
<input type="checkbox" />
```

For more information, see <xref:mvc/views/razor>.

## Raw HTML

Strings are normally rendered using DOM text nodes, which means that any markup they may contain is ignored and treated as literal text. To render raw HTML, wrap the HTML content in a `MarkupString` value. The value is parsed as HTML or SVG and inserted into the DOM.

> [!WARNING]
> Rendering raw HTML constructed from any untrusted source is a **security risk** and should be avoided!

The following example shows using the `MarkupString` type to add a block of static HTML content to the rendered output of a component:

```html
@((MarkupString)myMarkup)

@code {
    private string myMarkup = 
        "<p class='markup'>This is a <em>markup string</em>.</p>";
}
```

## Templated components

Templated components are components that accept one or more UI templates as parameters, which can then be used as part of the component's rendering logic. Templated components allow you to author higher-level components that are more reusable than regular components. A couple of examples include:

* A table component that allows a user to specify templates for the table's header, rows, and footer.
* A list component that allows a user to specify a template for rendering items in a list.

### Template parameters

A templated component is defined by specifying one or more component parameters of type `RenderFragment` or `RenderFragment<T>`. A render fragment represents a segment of UI to render. `RenderFragment<T>` takes a type parameter that can be specified when the render fragment is invoked.

`TableTemplate` component:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/TableTemplate.razor)]

When using a templated component, the template parameters can be specified using child elements that match the names of the parameters (`TableHeader` and `RowTemplate` in the following example):

```cshtml
<TableTemplate Items="@pets">
    <TableHeader>
        <th>ID</th>
        <th>Name</th>
    </TableHeader>
    <RowTemplate>
        <td>@context.PetId</td>
        <td>@context.Name</td>
    </RowTemplate>
</TableTemplate>
```

### Template context parameters

Component arguments of type `RenderFragment<T>` passed as elements have an implicit parameter named `context` (for example from the preceding code sample, `@context.PetId`), but you can change the parameter name using the `Context` attribute on the child element. In the following example, the `RowTemplate` element's `Context` attribute specifies the `pet` parameter:

```cshtml
<TableTemplate Items="@pets">
    <TableHeader>
        <th>ID</th>
        <th>Name</th>
    </TableHeader>
    <RowTemplate Context="pet">
        <td>@pet.PetId</td>
        <td>@pet.Name</td>
    </RowTemplate>
</TableTemplate>
```

Alternatively, you can specify the `Context` attribute on the component element. The specified `Context` attribute applies to all specified template parameters. This can be useful when you want to specify the content parameter name for implicit child content (without any wrapping child element). In the following example, the `Context` attribute appears on the `TableTemplate` element and applies to all template parameters:

```cshtml
<TableTemplate Items="@pets" Context="pet">
    <TableHeader>
        <th>ID</th>
        <th>Name</th>
    </TableHeader>
    <RowTemplate>
        <td>@pet.PetId</td>
        <td>@pet.Name</td>
    </RowTemplate>
</TableTemplate>
```

### Generic-typed components

Templated components are often generically typed. For example, a generic `ListViewTemplate` component can be used to render `IEnumerable<T>` values. To define a generic component, use the `@typeparam` directive to specify type parameters:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/ListViewTemplate.razor)]

When using generic-typed components, the type parameter is inferred if possible:

```cshtml
<ListViewTemplate Items="@pets">
    <ItemTemplate Context="pet">
        <li>@pet.Name</li>
    </ItemTemplate>
</ListViewTemplate>
```

Otherwise, the type parameter must be explicitly specified using an attribute that matches the name of the type parameter. In the following example, `TItem="Pet"` specifies the type:

```cshtml
<ListViewTemplate Items="@pets" TItem="Pet">
    <ItemTemplate Context="pet">
        <li>@pet.Name</li>
    </ItemTemplate>
</ListViewTemplate>
```

## Cascading values and parameters

In some scenarios, it's inconvenient to flow data from an ancestor component to a descendent component using [component parameters](#component-parameters), especially when there are several component layers. Cascading values and parameters solve this problem by providing a convenient way for an ancestor component to provide a value to all of its descendent components. Cascading values and parameters also provide an approach for components to coordinate.

### Theme example

In the following example from the sample app, the `ThemeInfo` class specifies the theme information to flow down the component hierarchy so that all of the buttons within a given part of the app share the same style.

*UIThemeClasses/ThemeInfo.cs*:

```csharp
public class ThemeInfo
{
    public string ButtonClass { get; set; }
}
```

An ancestor component can provide a cascading value using the Cascading Value component. The `CascadingValue` component wraps a subtree of the component hierarchy and supplies a single value to all components within that subtree.

For example, the sample app specifies theme information (`ThemeInfo`) in one of the app's layouts as a cascading parameter for all components that make up the layout body of the `@Body` property. `ButtonClass` is assigned a value of `btn-success` in the layout component. Any descendent component can consume this property through the `ThemeInfo` cascading object.

`CascadingValuesParametersLayout` component:

```cshtml
@inherits LayoutComponentBase
@using BlazorSample.UIThemeClasses

<div class="container-fluid">
    <div class="row">
        <div class="col-sm-3">
            <NavMenu />
        </div>
        <div class="col-sm-9">
            <CascadingValue Value="@theme">
                <div class="content px-4">
                    @Body
                </div>
            </CascadingValue>
        </div>
    </div>
</div>

@code {
    private ThemeInfo theme = new ThemeInfo { ButtonClass = "btn-success" };
}
```

To make use of cascading values, components declare cascading parameters using the `[CascadingParameter]` attribute or based on a string name value:

```cshtml
<CascadingValue Value=@PermInfo Name="UserPermissions">...</CascadingValue>

[CascadingParameter(Name = "UserPermissions")]
private PermInfo Permissions { get; set; }
```

Binding with a string name value is relevant if you have multiple cascading values of the same type and need to differentiate them within the same subtree.

Cascading values are bound to cascading parameters by type.

In the sample app, the `CascadingValuesParametersTheme` component binds the `ThemeInfo` cascading value to a cascading parameter. The parameter is used to set the CSS class for one of the buttons displayed by the component.

`CascadingValuesParametersTheme` component:

```cshtml
@page "/cascadingvaluesparameterstheme"
@layout CascadingValuesParametersLayout
@using BlazorSample.UIThemeClasses

<h1>Cascading Values & Parameters</h1>

<p>Current count: @currentCount</p>

<p>
    <button class="btn" @onclick="IncrementCount">
        Increment Counter (Unthemed)
    </button>
</p>

<p>
    <button class="btn @ThemeInfo.ButtonClass" @onclick="IncrementCount">
        Increment Counter (Themed)
    </button>
</p>

@code {
    private int currentCount = 0;

    [CascadingParameter]
    protected ThemeInfo ThemeInfo { get; set; }

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

### TabSet example

Cascading parameters also enable components to collaborate across the component hierarchy. For example, consider the following *TabSet* example in the sample app.

The sample app has an `ITab` interface that tabs implement:

[!code-cs[](common/samples/3.x/BlazorSample/UIInterfaces/ITab.cs)]

The `CascadingValuesParametersTabSet` component uses the `TabSet` component, which contains several `Tab` components:

[!code-cshtml[](common/samples/3.x/BlazorSample/Pages/CascadingValuesParametersTabSet.razor?name=snippet_TabSet)]

The child `Tab` components aren't explicitly passed as parameters to the `TabSet`. Instead, the child `Tab` components are part of the child content of the `TabSet`. However, the `TabSet` still needs to know about each `Tab` component so that it can render the headers and the active tab. To enable this coordination without requiring additional code, the `TabSet` component *can provide itself as a cascading value* that is then picked up by the descendent `Tab` components.

`TabSet` component:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/TabSet.razor)]

The descendent `Tab` components capture the containing `TabSet` as a cascading parameter, so the `Tab` components add themselves to the `TabSet` and coordinate on which tab is active.

`Tab` component:

[!code-cshtml[](common/samples/3.x/BlazorSample/Components/Tab.razor)]

## Razor templates

Render fragments can be defined using Razor template syntax. Razor templates are a way to define a UI snippet and assume the following format:

```cshtml
@<{HTML tag}>...</{HTML tag}>
```

The following example illustrates how to specify `RenderFragment` and `RenderFragment<T>` values and render templates directly in a component. Render fragments can also be passed as arguments to [templated components](#templated-components).

```cshtml
@timeTemplate

@petTemplate(new Pet { Name = "Rex" })

@code {
    private RenderFragment timeTemplate = @<p>The time is @DateTime.Now.</p>;
    private RenderFragment<Pet> petTemplate = 
        (pet) => @<p>Your pet's name is @pet.Name.</p>;

    private class Pet
    {
        public string Name { get; set; }
    }
}
```

Rendered output of the preceding code:

```html
<p>The time is 10/04/2018 01:26:52.</p>

<p>Your pet's name is Rex.</p>
```

## Manual RenderTreeBuilder logic

`Microsoft.AspNetCore.Components.RenderTree` provides methods for manipulating components and elements, including building components manually in C# code.

> [!NOTE]
> Use of `RenderTreeBuilder` to create components is an advanced scenario. A malformed component (for example, an unclosed markup tag) can result in undefined behavior.

Consider the following `PetDetails` component, which can be manually built into another component:

```cshtml
<h2>Pet Details Component</h2>

<p>@PetDetailsQuote</p>

@code
{
    [Parameter]
    private string PetDetailsQuote { get; set; }
}
```

In the following example, the loop in the `CreateComponent` method generates three `PetDetails` components. When calling `RenderTreeBuilder` methods to create the components (`OpenComponent` and `AddAttribute`), sequence numbers are source code line numbers. The Blazor difference algorithm relies on the sequence numbers corresponding to distinct lines of code, not distinct call invocations. When creating a component with `RenderTreeBuilder` methods, hardcode the arguments for sequence numbers. **Using a calculation or counter to generate the sequence number can lead to poor performance.** For more information, see the [Sequence numbers relate to code line numbers and not execution order](#sequence-numbers-relate-to-code-line-numbers-and-not-execution-order) section.

`BuiltContent` component:

```cshtml
@page "/BuiltContent"

<h1>Build a component</h1>

@CustomRender

<button type="button" @onclick="RenderComponent">
    Create three Pet Details components
</button>

@code {
    private RenderFragment CustomRender { get; set; }
    
    private RenderFragment CreateComponent() => builder =>
    {
        for (var i = 0; i < 3; i++) 
        {
            builder.OpenComponent(0, typeof(PetDetails));
            builder.AddAttribute(1, "PetDetailsQuote", "Someone's best friend!");
            builder.CloseComponent();
        }
    };    
    
    private void RenderComponent()
    {
        CustomRender = CreateComponent();
    }
}
```

### Sequence numbers relate to code line numbers and not execution order

Blazor `.razor` files are always compiled. This is potentially a great advantage for `.razor` because the compile step can be used to inject information that improve app performance at runtime.

A key example of these improvements involve *sequence numbers*. Sequence numbers indicate to the runtime which outputs came from which distinct and ordered lines of code. The runtime uses this information to generate efficient tree diffs in linear time, which is far faster than is normally possible for a general tree diff algorithm.

Consider the following simple `.razor` file:

```cshtml
@if (someFlag)
{
    <text>First</text>
}

Second
```

The preceding code compiles to something like the following:

```csharp
if (someFlag)
{
    builder.AddContent(0, "First");
}

builder.AddContent(1, "Second");
```

When the code executes for the first time, if `someFlag` is `true`, the builder receives:

| Sequence | Type      | Data   |
| :------: | --------- | :----: |
| 0        | Text node | First  |
| 1        | Text node | Second |

Imagine that `someFlag` becomes `false`, and the markup is rendered again. This time, the builder receives:

| Sequence | Type       | Data   |
| :------: | ---------- | :----: |
| 1        | Text node  | Second |

When the runtime performs a diff, it sees that the item at sequence `0` was removed, so it generates the following trivial *edit script*:

* Remove the first text node.

#### What goes wrong if you generate sequence numbers programmatically

Imagine instead that you wrote the following render tree builder logic:

```csharp
var seq = 0;

if (someFlag)
{
    builder.AddContent(seq++, "First");
}

builder.AddContent(seq++, "Second");
```

Now, the first output is:

| Sequence | Type      | Data   |
| :------: | --------- | :----: |
| 0        | Text node | First  |
| 1        | Text node | Second |

This outcome is identical to the prior case, so no negative issues exist. `someFlag` is `false` on the second rendering, and the output is:

| Sequence | Type      | Data   |
| :------: | --------- | ------ |
| 0        | Text node | Second |

This time, the diff algorithm sees that *two* changes have occurred, and the algorithm generates the following edit script:

* Change the value of the first text node to `Second`.
* Remove the second text node.

Generating the sequence numbers has lost all the useful information about where the `if/else` branches and loops were present in the original code. This results in a diff **twice as long** as before.

This is a trivial example. In more realistic cases with complex and deeply nested structures, and especially with loops, the performance cost is more severe. Instead of immediately identifying which loop blocks or branches have been inserted or removed, the diff algorithm has to recurse deeply into the render trees and usually build far longer edit scripts because it is misinformed about how the old and new structures relate to each other.

#### Guidance and conclusions

* App performance suffers if sequence numbers are generated dynamically.
* The framework can't create its own sequence numbers automatically at runtime because the necessary information doesn't exist unless it's captured at compile time.
* Don't write long blocks of manually-implemented `RenderTreeBuilder` logic. Prefer `.razor` files and allow the compiler to deal with the sequence numbers.
* If sequence numbers are hardcoded, the diff algorithm only requires that sequence numbers increase in value. The initial value and gaps are irrelevant. One legitimate option is to use the code line number as the sequence number, or start from zero and increase by ones or hundreds (or any preferred interval). 
* Blazor uses sequence numbers, while other tree-diffing UI frameworks don't use them. Diffing is far faster when sequence numbers are used, and Blazor has the advantage of a compile step that deals with sequence numbers automatically for developers authoring `.razor` files.
