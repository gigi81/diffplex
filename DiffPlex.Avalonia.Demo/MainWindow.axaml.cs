using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DiffPlex.Avalonia.Controls;
using System.IO;

namespace DiffPlex.Avalonia.Demo
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.FindControl<InlineDiffViewer>("DiffView").SetDiffModel(new FileInfo(@"C:\Temp\compare1.txt"), new FileInfo(@"C:\Temp\compare2.txt"));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
