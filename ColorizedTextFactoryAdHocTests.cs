using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace SnippetColorizer
{
    [Export]
    class ColorizedTextFactoryAdHocTests
    {
        readonly ColorizedTextFactory colorizedTextFactory;

        [ImportingConstructor]
        ColorizedTextFactoryAdHocTests(ColorizedTextFactory colorizedTextFactory)
        {
            this.colorizedTextFactory = colorizedTextFactory;
        }

        [STAThread]
        void ShowTextView_ExistingFile()
        {
            var fileName = GetFileName();
            var source = File.ReadAllText(fileName);
            var view = colorizedTextFactory.CreateTextView(source, fileName, "CSharp");
            var window = new Window { Content = view };
            window.ShowDialog();
        }

        [STAThread]
        void ShowTextView_ReplacementFile()
        {
            var fileName = GetFileName();
            var view = colorizedTextFactory.CreateTextView(ExampleSource, fileName, "CSharp");
            var window = new Window { Content = view };
            window.ShowDialog();
        }

        [STAThread]
        void ShowTextView_NewFile()
        {
            var fileName = @"c:\new.cs";
            var view = colorizedTextFactory.CreateTextView(ExampleSource, fileName, "CSharp");
            var window = new Window { Content = view };
            window.ShowDialog();
        }

        [STAThread]
        void ShowTextViewHost_ExistingFile()
        {
            var fileName = GetFileName();
            var source = File.ReadAllText(fileName);
            var host = colorizedTextFactory.CreateTextViewHost(source, fileName, "CSharp");
            var window = new Window { Content = host.HostControl };
            window.ShowDialog();
        }

        static string GetFileName() => new StackFrame(true).GetFileName();

        static string ExampleSource =>
@"using System.Windows;

class Foo
{
    void Bar()
    {
        new Window { Content = ""Hello, World"" }.ShowDialog();
    }
}";
    }
}
