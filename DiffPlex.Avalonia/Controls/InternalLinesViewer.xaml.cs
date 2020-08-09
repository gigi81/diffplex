using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Data;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Layout;

namespace DiffPlex.Avalonia.Controls
{
    /// <summary>
    /// Interaction logic for InternalLinesViewer.xaml
    /// </summary>
    internal partial class InternalLinesViewer : UserControl
    {
        private readonly Dictionary<string, Binding> bindings = new Dictionary<string, Binding>();

        public InternalLinesViewer()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private ScrollViewer NumberScrollViewer => this.FindControl<ScrollViewer>("NumberScrollViewer");

        private ScrollViewer OperationScrollViewer => this.FindControl<ScrollViewer>("OperationScrollViewer");

        private ScrollViewer ValueScrollViewer => this.FindControl<ScrollViewer>("ValueScrollViewer");

        private Grid Grid => this.FindControl<Grid>("Grid");

        private ColumnDefinition NumberColumn => this.Grid.ColumnDefinitions[0];

        private ColumnDefinition OperationColumn => this.Grid.ColumnDefinitions[1];

        private ColumnDefinition ValueColumn => this.Grid.ColumnDefinitions[2];

        private StackPanel NumberPanel => this.FindControl<StackPanel>("StackPanel");

        private StackPanel OperationPanel => this.FindControl<StackPanel>("OperationPanel");

        private StackPanel ValuePanel => this.FindControl<StackPanel>("ValuePanel");

        //public event ScrollChangedEventHandler ScrollChanged
        //{
        //    add => ValueScrollViewer.ScrollChanged += value;
        //    remove => ValueScrollViewer.ScrollChanged -= value;
        //}

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => ValueScrollViewer.VerticalScrollBarVisibility;
            set => ValueScrollViewer.VerticalScrollBarVisibility = value;
        }

        public Guid TrackingId { get; set; }

        //public double VerticalOffset => ValueScrollViewer.VerticalOffset;

        public int LineNumberWidth
        {
            get
            {
                return (int)(NumberColumn.ActualWidth + OperationColumn.ActualWidth);
            }

            set
            {
                var aThird = value / 3;
                OperationColumn.Width = new GridLength(aThird);
                NumberColumn.Width = new GridLength(value - aThird);
            }
        }

        public void Clear()
        {
            NumberPanel.Children.Clear();
            OperationPanel.Children.Clear();
            ValuePanel.Children.Clear();
        }

        public StackPanel Add(int? number, string operation, string value, string changeType, Control source)
        {
            var index = new TextBlock
            {
                Text = number.HasValue ? number.ToString() : string.Empty,
                TextAlignment = TextAlignment.Right
            };
            index.Bind(TextBlock.ForegroundProperty, GetBindings("LineNumberForeground", source, Foreground));
            index.Bind(TextBlock.BackgroundProperty, GetBindings(changeType + "Background", source));
            ApplyTextBlockProperties(index, source);
            NumberPanel.Children.Add(index);

            var op = new TextBlock
            {
                Text = operation,
                TextAlignment = TextAlignment.Center
            };
            op.Bind(TextBlock.ForegroundProperty, GetBindings("ChangeTypeForeground", source, Foreground));
            op.Bind(TextBlock.BackgroundProperty, GetBindings(changeType + "Background", source));
            ApplyTextBlockProperties(op, source);
            OperationPanel.Children.Add(op);

            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Bind(BackgroundProperty, GetBindings(changeType + "Background", source));
            var text = new TextBlock
            {
                Text = value
            }; ;
            if (!string.IsNullOrEmpty(value))
            {
                text.Bind(TextBlock.ForegroundProperty, GetBindings(changeType + "Foreground", source, Foreground));
                text.Bind(TextBlock.BackgroundProperty, GetBindings(changeType + "Background", source));
                ApplyTextBlockProperties(text, source);
            }

            panel.Children.Add(text);
            ValuePanel.Children.Add(panel);
            return panel;
        }

        public StackPanel Add(int? number, string operation, List<KeyValuePair<string, string>> value, string changeType, Control source)
        {
            var index = new TextBlock
            {
                Text = number.HasValue ? number.ToString() : string.Empty,
                TextAlignment = TextAlignment.Right
            };
            index.Bind(TextBlock.ForegroundProperty, GetBindings("LineNumberForeground", source, Foreground));
            index.Bind(TextBlock.BackgroundProperty, GetBindings(changeType + "Background", source));
            ApplyTextBlockProperties(index, source);
            NumberPanel.Children.Add(index);

            var op = new TextBlock
            {
                Text = operation,
                TextAlignment = TextAlignment.Center
            };
            op.Bind(TextBlock.ForegroundProperty, GetBindings("ChangeTypeForeground", source, Foreground));
            op.Bind(TextBlock.BackgroundProperty, GetBindings(changeType + "Background", source));
            ApplyTextBlockProperties(op, source);
            OperationPanel.Children.Add(op);

            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Bind(BackgroundProperty, GetBindings(changeType + "Background", source));
            if (value == null) value = new List<KeyValuePair<string, string>>();
            foreach (var ele in value)
            {
                if (string.IsNullOrEmpty(ele.Key)) continue;
                var text = new TextBlock
                {
                    Text = ele.Key
                };
                if (!string.IsNullOrEmpty(ele.Value))
                {
                    if (!string.IsNullOrEmpty(ele.Key))
                        text.Bind(TextBlock.ForegroundProperty, GetBindings(ele.Value + "Foreground", source, Foreground));
                    text.Bind(TextBlock.BackgroundProperty, GetBindings(ele.Value + "Background", source));
                }
                
                ApplyTextBlockProperties(text, source);
                panel.Children.Add(text);
            }

            if (panel.Children.Count == 0) panel.Children.Add(new TextBlock());
            ValuePanel.Children.Add(panel);
            return panel;
        }

        private Binding GetBindings(string key, Control source)
        {
            if (bindings.TryGetValue(key, out var r) && r.Source == source) return r;
            return bindings[key] = new Binding(key) { Source = source, Mode = BindingMode.OneWay };
        }

        private Binding GetBindings(string key, Control source, object defaultValue)
        {
            if (bindings.TryGetValue(key, out var r) && r.Source == source) return r;
            return bindings[key] = new Binding(key) { Source = source, Mode = BindingMode.OneWay, FallbackValue = defaultValue };
        }

        //public void ScrollToVerticalOffset(double offset)
        //{
        //    ValueScrollViewer.ScrollToVerticalOffset(offset);
        //}

        internal void AdjustScrollView()
        {
            var isV = ValueScrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
            var hasV = ValuePanel.Margin.Bottom > 10;
            if (isV)
            {
                if (!hasV) ValuePanel.Margin = NumberPanel.Margin = OperationPanel.Margin = new Thickness(0, 0, 0, 20);
            }
            else
            {
                if (hasV) ValuePanel.Margin = NumberPanel.Margin = OperationPanel.Margin = new Thickness(0);
            }
        }

        private void ApplyTextBlockProperties(TextBlock text, Control source)
        {

            text.Bind(TextBlock.FontSizeProperty, GetBindings("FontSize", source));
            text.Bind(TextBlock.FontFamilyProperty, GetBindings("FontFamily", source, Helper.FontFamily ));
            text.Bind(TextBlock.FontWeightProperty, GetBindings("FontWeight", source));
            //text.Bind(TextBlock.FontStretchProperty, GetBindings("FontStretch", source));
            text.Bind(TextBlock.FontStyleProperty, GetBindings("FontStyle", source));
        }

        private void NumberScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var offset = NumberScrollViewer.Offset.Y;
            ScrollVertical(ValueScrollViewer, offset);
        }

        private void OperationScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var offset = OperationScrollViewer.Offset.Y;
            ScrollVertical(ValueScrollViewer, offset);
        }

        private void ValueScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var offset = ValueScrollViewer.Offset.Y;
            ScrollVertical(NumberScrollViewer, offset);
            ScrollVertical(OperationScrollViewer, offset);
        }

        private void ScrollVertical(ScrollViewer scrollViewer, double offset)
        {
            if (Math.Abs(scrollViewer.Offset.Y - offset) > 1)
                scrollViewer.Offset = new Vector(scrollViewer.Offset.X, offset);
        }

        //private void ValueScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    AdjustScrollView();
        //}
    }
}
