using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using AvaloniaHangProject.ViewModels;

namespace AvaloniaHangProject.Views {
    public class MainWindow : Window {
        private readonly TabControl tabs;
        private Control currentTab = null;
        public MainWindow() {
            InitializeComponent();

            tabs = this.FindControl<TabControl>("tabs");


#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnSelectedTabChanged(object sender, SelectionChangedEventArgs e) {
            this.currentTab = ((IControl)e.AddedItems[0]).GetLogicalDescendants().OfType<Button>().FirstOrDefault();
        }

        public Control GetActiveTab() {
            return this.currentTab;
        }

        public void AddTab(int index) {
            var header = new TabHeaderInfo();
            header.Caption = "Caption " + index;

            var view = new Button() {
                Name = "view",
                Content = "Open Window Example",
                Command = ((MainWindowViewModel)this.DataContext).OpenWindowCommand
            };

            var tab = new TabItem() {
                Content = view,
                DataContext = header,
                Header = header,
                IsVisible = true
            };

            ((IList)tabs.Items).Add(tab);
        }
    }
}