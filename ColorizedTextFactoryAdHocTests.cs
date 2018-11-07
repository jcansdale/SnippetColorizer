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
        void ShowTextViewHost()
        {
            var source = GetSource();
            var host = colorizedTextFactory.CreateTextViewHost(source, "CSharp");
            var window = new Window { Content = host.HostControl };
            window.ShowDialog();
        }

        [STAThread]
        void ShowTextView()
        {
            var source = GetSource();
            var view = colorizedTextFactory.CreateTextView(source, "CSharp");
            var window = new Window { Content = view };
            window.ShowDialog();
        }

        static string GetSource() =>
            File.ReadAllText(new StackFrame(true).GetFileName());
    }
}
