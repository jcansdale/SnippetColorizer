### What is this?

This is an experiment to find a clean way to turn a snippet of text into a WPF component.

See Twitter thread starting here:
https://twitter.com/jcansdale/status/1059793737600110592

Unfortunately getting colorization to work isn't simply a case of creating a `TextBuffer` with a `ContentType` of `CSharp` and calling `ITextEditorFactoryService.CreateTextView(textBuffer)`. We need to create a `Microsoft.CodeAnalysis.Workspace` and associate it with the `TextBuffer`.

This is the ceremony `MiscellaneousFilesWorkspace` performs on loose files that aren't associated with a project or `Workspace`.
https://github.com/dotnet/roslyn/blob/2468ad7ccb5fa5982579279454341123b20846ed/src/VisualStudio/Core/Def/Implementation/ProjectSystem/MiscellaneousFilesWorkspace.cs#L338-L341

### How to test

This project can be tested using [TestDriven.Net](https://marketplace.visualstudio.com/items?itemName=JamieCansdale.TestDrivenNet)'s `Test With > In-Proc (VS SDK)` feature. This allows code to be executed inside the current Visual Studio process, MEF services to be prototypes and calls made to services. You can download from the marketplace [here](https://marketplace.visualstudio.com/items?itemName=JamieCansdale.TestDrivenNet).

![image](https://user-images.githubusercontent.com/11719160/48125901-b3c7c700-e277-11e8-99a5-fadab6e08dcb.png)

The `ColorizedTextFactoryAdHocTests` class is marked with `[Export]` so it knows to register MEF services in the same assembly and the `ShowTextViewHost` method is marked with `[STAThread]` so that it knows to execute on the UI thread.

Any MEF service or Visual Studio service can be passed as a parameter to a "test" method (for Visual Studio services, the service type will be inferred form the interface type).

Here is what happens when I `Test With > In-Proc (VS SDK)` on the `ColorizedTextFactoryAdHocTests.ShowTextViewHost` method.

![image](https://user-images.githubusercontent.com/11719160/48126562-5d5b8800-e279-11e8-9818-6d72818e0e39.png)

### Questions

The `ColorizedTextFactory.CreateProjectInfoForDocument` uses reflection to call `CreateProjectInfoForDocument` on `MiscellaneousFilesWorkspace`. This is bad.

I didn't know enough about Roslyn to create a minimal `ProjectInfo` that is associated with a language, but ideally not a specific file on disk. Any help here would be appreciated.

Thanks everyone who helped on twitter and happy spelunking!
