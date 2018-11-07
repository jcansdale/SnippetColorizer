using System.Linq;
using System.Reflection;
using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.LanguageServices;

namespace SnippetColorizer
{
    [Export]
    public class ColorizedTextFactory : Workspace
    {
        readonly ITextBufferFactoryService textBufferFactory;
        readonly ITextEditorFactoryService editorFactory;
        readonly IContentTypeRegistryService contentTypeRegistry;
        readonly object miscellaneousFilesWorkspace;

        [ImportingConstructor]
        public ColorizedTextFactory(
            VisualStudioWorkspace visualStudioWorkspace,
            ITextBufferFactoryService textBufferFactory,
            ITextEditorFactoryService editorFactory,
            IContentTypeRegistryService contentTypeRegistry,
            [Import("Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.MiscellaneousFilesWorkspace")]
        object miscellaneousFilesWorkspace)
            : base(visualStudioWorkspace.Services.HostServices, WorkspaceKind.MiscellaneousFiles)
        {
            this.textBufferFactory = textBufferFactory;
            this.editorFactory = editorFactory;
            this.contentTypeRegistry = contentTypeRegistry;
            this.miscellaneousFilesWorkspace = miscellaneousFilesWorkspace;
        }

        public IWpfTextViewHost CreateTextViewHost(string source, string contentType)
        {
            var textView = CreateTextView(source, contentType);
            return editorFactory.CreateTextViewHost(textView, false);
        }

        public IWpfTextView CreateTextView(string source, string contentType)
        {
            var textBuffer = CreateTextBuffer(source, contentType);
            return editorFactory.CreateTextView(textBuffer);
        }

        public ITextBuffer CreateTextBuffer(string source, string contentType)
        {
            var ct = contentTypeRegistry.GetContentType(contentType);
            var textBuffer = textBufferFactory.CreateTextBuffer(source, ct);
            var projectInfo = CreateProjectInfoForDocument(@"x:\moniker.cs");
            OnProjectAdded(projectInfo);
            var sourceTextContainer = textBuffer.AsTextContainer();
            OnDocumentOpened(projectInfo.Documents.Single().Id, sourceTextContainer);
            return textBuffer;
        }

        ProjectInfo CreateProjectInfoForDocument(string filePath)
        {
            return (ProjectInfo)miscellaneousFilesWorkspace.GetType()
                .GetMethod("CreateProjectInfoForDocument", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(miscellaneousFilesWorkspace, new object[] { filePath });
        }
    }
}
