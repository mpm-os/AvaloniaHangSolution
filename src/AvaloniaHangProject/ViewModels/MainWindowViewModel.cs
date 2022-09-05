using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHangProject.Views;
using ReactiveUI;

namespace AvaloniaHangProject.ViewModels {
    public class MainWindowViewModel : ReactiveObject {
        private List<DialogWindow> dialogWindows = new List<DialogWindow>();
        private int captionsCounter = 0;

        public MainWindowViewModel(Window parent) {
            OpenWindowCommand = ReactiveCommand.Create(
                () => {
                    var activeTab = ((MainWindow)parent).GetActiveTab();
                    var dialogWindow = new DialogWindow(parent, activeTab) {
                        MinHeight = 500,
                        MinWidth = 500,
                        Position = new PixelPoint(500, 500)
                    };

                    var innerContent = new Border() { BorderBrush = new SolidColorBrush(Colors.Red), BorderThickness = new Thickness(3)};

                    dialogWindow.Content = new Button() { Name = "Test Button", Content = "Open Modal", Command = OnClickButtonCommand(parent)};
                    //dialogWindow.DisplayModal(false);
                    dialogWindow.ShowDialogSync(parent);

                    dialogWindows.Add(dialogWindow);
                }
            );


            AddTabCommand = ReactiveCommand.Create(
                () => {
                    ((MainWindow)parent).AddTab(++captionsCounter);
                });
        }

        public ICommand OnClickButtonCommand(Window parent) {
            return ReactiveCommand.Create(() => {
                var dialogWindow = new DialogWindow(parent, null) {
                    MinHeight = 500,
                    MinWidth = 500,
                    Position = new PixelPoint(500, 500)
                };

                dialogWindow.Content = new Button() { Name = "Button", Content = "Proceed Anyway", Command = OnClickButtonCommand2(dialogWindow)};
                dialogWindow.Show();
            });
        }

        public ICommand OnClickButtonCommand2(Window parent) {
            return ReactiveCommand.Create(() => {

            });
        }

        public ICommand OpenWindowCommand { get; }

        public ICommand AddTabCommand { get; }
    }
}