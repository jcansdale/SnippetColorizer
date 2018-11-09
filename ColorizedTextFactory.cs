using System;
using System.Linq;
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

        [ImportingConstructor]
        public ColorizedTextFactory(
            VisualStudioWorkspace visualStudioWorkspace,
            ITextBufferFactoryService textBufferFactory,
            ITextEditorFactoryService editorFactory,
            IContentTypeRegistryService contentTypeRegistry)
            : base(visualStudioWorkspace.Services.HostServices, WorkspaceKind.MiscellaneousFiles)
        {
            this.textBufferFactory = textBufferFactory;
            this.editorFactory = editorFactory;
            this.contentTypeRegistry = contentTypeRegistry;

            SetCurrentSolution(visualStudioWorkspace.CurrentSolution);
        }

        public IWpfTextViewHost CreateTextViewHost(string source, string filePath, string contentType)
        {
            var textView = CreateTextView(source, filePath, contentType);
            return editorFactory.CreateTextViewHost(textView, false);
        }

        public IWpfTextView CreateTextView(string source, string filePath, string contentType)
        {
            var textBuffer = CreateTextBuffer(source, filePath, contentType);
            return editorFactory.CreateTextView(textBuffer);
        }

        public ITextBuffer CreateTextBuffer(string source, string filePath, string contentType)
        {
            var ct = contentTypeRegistry.GetContentType(contentType);
            var textBuffer = textBufferFactory.CreateTextBuffer(source, ct);

            var documentId = CurrentSolution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
            documentId = documentId ?? CreateDocumentId(filePath);
            OnDocumentOpened(documentId, textBuffer.AsTextContainer());

            return textBuffer;
        }

        DocumentId CreateDocumentId(string filePath)
        {
            var project = CurrentSolution.Projects.First();
            var projectId = project.Id;
            var documentId = DocumentId.CreateNewId(projectId, debugName: filePath);
            var documentInfo = DocumentInfo.Create(documentId, filePath);
            var solution = CurrentSolution.AddDocument(documentInfo);
            SetCurrentSolution(solution);
            return documentId;
        }
    }
}
