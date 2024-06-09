using System;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace DiffPlex.Avalonia.Controls
{
    /// <summary>
    /// The inline diff control for text.
    /// Interaction logic for InlineDiffViewer.xaml
    /// </summary>
    public partial class InlineDiffViewer : UserControl
    {
        #region Properties
        /// <summary>
        /// The property of diff model.
        /// </summary>
        public static readonly StyledProperty<DiffPaneModel> DiffModelProperty = RegisterDependencyProperty<DiffPaneModel>(nameof(DiffModel));

        /// <summary>
        /// The property of line number background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> LineNumberForegroundProperty = RegisterDependencyProperty<Brush>(nameof(LineNumberForeground), new SolidColorBrush(Color.FromArgb(255, 64, 128, 160)));

        /// <summary>
        /// The property of line number.
        /// </summary>
        public static readonly StyledProperty<double> LineNumberWidthProperty = RegisterDependencyProperty<double>(nameof(LineNumberWidth), 60D);

        /// <summary>
        /// The property of change type symbol foreground brush.
        /// </summary>
        public static readonly StyledProperty<Brush> ChangeTypeForegroundProperty = RegisterDependencyProperty<Brush>(nameof(ChangeTypeForeground), new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)));

        /// <summary>
        /// The property of text inserted background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> InsertedForegroundProperty = RegisterDependencyProperty<Brush>(nameof(InsertedForeground), new SolidColorBrush(Color.FromArgb(255, 64, 128, 160)));

        /// <summary>
        /// The property of text inserted background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> InsertedBackgroundProperty = RegisterDependencyProperty<Brush>(nameof(InsertedBackground), new SolidColorBrush(Color.FromArgb(64, 96, 216, 32)));

        /// <summary>
        /// The property of text inserted background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> DeletedForegroundProperty = RegisterDependencyProperty<Brush>(nameof(DeletedForeground), new SolidColorBrush(Color.FromArgb(255, 64, 128, 160)));

        /// <summary>
        /// The property of text inserted background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> DeletedBackgroundProperty = RegisterDependencyProperty<Brush>(nameof(DeletedBackground), new SolidColorBrush(Color.FromArgb(64, 216, 32, 32)));

        /// <summary>
        /// The property of text inserted background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> UnchangedForegroundProperty = RegisterDependencyProperty<Brush>(nameof(UnchangedForeground), new SolidColorBrush(Color.FromArgb(255, 64, 128, 160)));

        /// <summary>
        /// The property of text inserted background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> UnchangedBackgroundProperty = RegisterDependencyProperty<Brush>(nameof(UnchangedBackground), new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)));

        /// <summary>
        /// The property of grid splitter background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> SplitterForegroundProperty = RegisterDependencyProperty<Brush>(nameof(SplitterForeground));

        /// <summary>
        /// The property of grid splitter background brush.
        /// </summary>
        public static readonly StyledProperty<Brush> SplitterBackgroundProperty = RegisterDependencyProperty<Brush>(nameof(SplitterBackground), new SolidColorBrush(Color.FromArgb(64, 128, 128, 128)));

        /// <summary>
        /// The property of grid splitter border brush.
        /// </summary>
        public static readonly StyledProperty<Brush> SplitterBorderBrushProperty = RegisterDependencyProperty<Brush>(nameof(SplitterBorderBrush));

        /// <summary>
        /// The property of grid splitter border thickness.
        /// </summary>
        public static readonly StyledProperty<Thickness> SplitterBorderThicknessProperty = RegisterDependencyProperty<Thickness>(nameof(SplitterBorderThickness));

        /// <summary>
        /// The property of grid splitter width.
        /// </summary>
        public static readonly StyledProperty<double> SplitterWidthProperty = RegisterDependencyProperty<double>(nameof(SplitterWidth), 5);
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the InlineDiffViewer class.
        /// </summary>
        public InlineDiffViewer()
        {
            InitializeComponent();

            ContentPanel.Bind(ForegroundProperty, new Binding(nameof(Foreground)) { Source = this, Mode = BindingMode.OneWay });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private InternalLinesViewer ContentPanel => this.FindControl<InternalLinesViewer>("ContentPanel");

        #region Properties
        /// <summary>
        /// Gets or sets the side by side diff model.
        /// </summary>
        [Category("Appearance")]
        public DiffPaneModel DiffModel
        {
            get => GetValue(DiffModelProperty);
            set
            {
                SetValue(DiffModelProperty, value);
                this.UpdateContent(value);
            }
        }

        /// <summary>
        /// Gets or sets the foreground brush of the line number.
        /// </summary>
        [Bindable(true)]
        public Brush LineNumberForeground
        {
            get => GetValue(LineNumberForegroundProperty);
            set => SetValue(LineNumberForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the line number width.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public int LineNumberWidth
        {
            get => (int)GetValue(LineNumberWidthProperty);
            set 
            {
                SetValue(LineNumberWidthProperty, value);
                ContentPanel.LineNumberWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the foreground brush of the change type symbol.
        /// </summary>
        [Bindable(true)]
        public Brush ChangeTypeForeground
        {
            get => GetValue(ChangeTypeForegroundProperty);
            set => SetValue(ChangeTypeForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush of the line added.
        /// </summary>
        [Bindable(true)]
        public Brush InsertedForeground
        {
            get => GetValue(InsertedForegroundProperty);
            set => SetValue(InsertedForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush of the line added.
        /// </summary>
        [Bindable(true)]
        public Brush InsertedBackground
        {
            get => GetValue(InsertedBackgroundProperty);
            set => SetValue(InsertedBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush of the line deleted.
        /// </summary>
        [Bindable(true)]
        public Brush DeletedForeground
        {
            get => GetValue(DeletedForegroundProperty);
            set => SetValue(DeletedForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush of the line deleted.
        /// </summary>
        [Bindable(true)]
        public Brush DeletedBackground
        {
            get => GetValue(DeletedBackgroundProperty);
            set => SetValue(DeletedBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush of the line unchanged.
        /// </summary>
        [Bindable(true)]
        public Brush UnchangedForeground
        {
            get => GetValue(UnchangedForegroundProperty);
            set => SetValue(UnchangedForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush of the line unchanged.
        /// </summary>
        [Bindable(true)]
        public Brush UnchangedBackground
        {
            get => GetValue(UnchangedBackgroundProperty);
            set => SetValue(UnchangedBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush of the grid splitter.
        /// </summary>
        [Bindable(true)]
        public Brush SplitterForeground
        {
            get => GetValue(SplitterForegroundProperty);
            set => SetValue(SplitterForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush of the grid splitter.
        /// </summary>
        [Bindable(true)]
        public Brush SplitterBackground
        {
            get => GetValue(SplitterBackgroundProperty);
            set => SetValue(SplitterBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the border brush of the grid splitter.
        /// </summary>
        [Bindable(true)]
        public Brush SplitterBorderBrush
        {
            get => GetValue(SplitterBackgroundProperty);
            set => SetValue(SplitterBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the border thickness of the grid splitter.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public Thickness SplitterBorderThickness
        {
            get => GetValue(SplitterBorderThicknessProperty);
            set => SetValue(SplitterBorderThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the border thickness of the grid splitter.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public double SplitterWidth
        {
            get => GetValue(SplitterWidthProperty);
            set => SetValue(SplitterWidthProperty, value);
        }
        #endregion
        
        /// <summary>
        /// Sets a new diff model.
        /// </summary>
        /// <param name="oldText">The old text string to compare.</param>
        /// <param name="newText">The new text string.</param>
        /// <param name="ignoreWhiteSpace">true if ignore the white space; otherwise, false.</param>
        /// <param name="ignoreCase">true if case-insensitive; otherwise, false.</param>
        public void SetDiffModel(string oldText, string newText, bool ignoreWhiteSpace = true, bool ignoreCase = false)
        {
            DiffModel = InlineDiffBuilder.Diff(oldText, newText, ignoreWhiteSpace, ignoreCase);
        }

        /// <summary>
        /// Sets a new diff model.
        /// </summary>
        /// <param name="differ">The differ instance.</param>
        /// <param name="oldText">The old text string to compare.</param>
        /// <param name="newText">The new text string.</param>
        /// <param name="ignoreWhiteSpace">true if ignore the white space; otherwise, false.</param>
        /// <param name="ignoreCase">true if case-insensitive; otherwise, false.</param>
        public void SetDiffModel(IDiffer differ, string oldText, string newText, bool ignoreWhiteSpace = true, bool ignoreCase = false)
        {
            DiffModel = InlineDiffBuilder.Diff(differ, oldText, newText, ignoreWhiteSpace, ignoreCase);
        }

        /// <summary>
        /// Sets a new diff model.
        /// </summary>
        /// <param name="builder">The differ builder instance.</param>
        /// <param name="oldText">The old text string to compare.</param>
        /// <param name="newText">The new text string.</param>
        /// <param name="ignoreWhiteSpace">true if ignore the white space; otherwise, false.</param>
        /// <param name="ignoreCase">true if case-insensitive; otherwise, false.</param>
        /// <param name="chunker">The chunker.</param>
        public void SetDiffModel(IInlineDiffBuilder builder, string oldText, string newText, bool ignoreWhiteSpace = true, bool ignoreCase = false, IChunker chunker = null)
        {
            if (builder == null)
            {
                DiffModel = InlineDiffBuilder.Diff(oldText, newText, ignoreWhiteSpace, ignoreCase);
                return;
            }

            DiffModel = builder.BuildDiffModel(oldText, newText, ignoreWhiteSpace, ignoreCase, chunker ?? LineChunker.Instance);
        }

        /// <summary>
        /// Sets a new diff model.
        /// </summary>
        /// <param name="oldFile">The old text file to compare.</param>
        /// <param name="newFile">The new text file.</param>
        /// <param name="ignoreWhiteSpace">true if ignore the white space; otherwise, false.</param>
        /// <param name="ignoreCase">true if case-insensitive; otherwise, false.</param>
        /// <param name="chunker">The chunker.</param>
        /// <exception cref="ArgumentNullException">oldFile or newFile was null.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="IOException">Read file failed because of I/O exception.</exception>
        /// <exception cref="UnauthorizedAccessException">Cannot access the file.</exception>
        public void SetDiffModel(FileInfo oldFile, FileInfo newFile, bool ignoreWhiteSpace = true, bool ignoreCase = false, IChunker chunker = null)
        {
            if (oldFile == null) throw new ArgumentNullException(nameof(oldFile), "oldFile should not be null.");
            if (newFile == null) throw new ArgumentNullException(nameof(newFile), "newFile should not be null.");
            var oldText = File.ReadAllText(oldFile.FullName);
            var newText = File.ReadAllText(newFile.FullName);
            var builder = new InlineDiffBuilder();
            DiffModel = builder.BuildDiffModel(oldText, newText, ignoreWhiteSpace, ignoreCase, chunker ?? LineChunker.Instance);
        }

        /// <summary>
        /// Sets a new diff model.
        /// </summary>
        /// <param name="oldFile">The old text file to compare.</param>
        /// <param name="newFile">The new text file.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <param name="ignoreWhiteSpace">true if ignore the white space; otherwise, false.</param>
        /// <param name="ignoreCase">true if case-insensitive; otherwise, false.</param>
        /// <param name="chunker">The chunker.</param>
        /// <exception cref="ArgumentNullException">oldFile or newFile was null.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="IOException">Read file failed because of I/O exception.</exception>
        /// <exception cref="UnauthorizedAccessException">Cannot access the file.</exception>
        public void SetDiffModel(FileInfo oldFile, FileInfo newFile, Encoding encoding, bool ignoreWhiteSpace = true, bool ignoreCase = false, IChunker chunker = null)
        {
            if (oldFile == null) throw new ArgumentNullException(nameof(oldFile), "oldFile should not be null.");
            if (newFile == null) throw new ArgumentNullException(nameof(newFile), "newFile should not be null.");
            var oldText = File.ReadAllText(oldFile.FullName, encoding);
            var newText = File.ReadAllText(newFile.FullName, encoding);
            var builder = new InlineDiffBuilder();
            DiffModel = builder.BuildDiffModel(oldText, newText, ignoreWhiteSpace, ignoreCase, chunker ?? LineChunker.Instance);
        }

        /// <summary>
        /// Refreshes.
        /// </summary>
        public void Refresh()
        {
            UpdateContent(DiffModel);
        }

        /// <summary>
        /// Goes to a specific line.
        /// </summary>
        /// <param name="lineIndex">The index of line.</param>
        /// <returns>true if it has turned to the specific line; otherwise, false.</returns>
        //public bool GoTo(int lineIndex)
        //{
        //    return Helper.GoTo(ContentPanel, lineIndex);
        //}

        /// <summary>
        /// Updates the content.
        /// </summary>
        /// <param name="m">The diff model.</param>
        private void UpdateContent(DiffPaneModel m)
        {
            Helper.RenderInlineDiffs(ContentPanel, m, this);
        }

        private static StyledProperty<T> RegisterDependencyProperty<T>(string name)
        {
            return AvaloniaProperty.Register<InlineDiffViewer,T>(name);
        }

        private static StyledProperty<T> RegisterDependencyProperty<T>(string name, T defaultValue)
        {
            return AvaloniaProperty.Register<InlineDiffViewer, T>(name, defaultValue);
        }
    }
}
