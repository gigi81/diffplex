using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DiffPlex.Avalonia.Controls;
using System.IO;

namespace DiffPlex.Avalonia.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.FindControl<InlineDiffViewer>("DiffView").SetDiffModel(new FileInfo(@"D:\Temp\compare1.md"), new FileInfo(@"D:\Temp\compare2.md"));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
