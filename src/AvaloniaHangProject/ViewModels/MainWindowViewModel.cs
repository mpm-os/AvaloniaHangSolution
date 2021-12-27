using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHangProject.Views;
using ReactiveUI;

namespace AvaloniaHangProject.ViewModels {
    public class MainWindowViewModel : ReactiveObject {
        private BaseGuidedTourDialogWindow guidedTourDialogWindow;

        public MainWindowViewModel(Window parent) {
            OpenGuidedTourExampleCommand = ReactiveCommand.Create(
                () => {
                    guidedTourDialogWindow?.Close();

                    // To fix "Cannot re-show a closed window" we create a new window
                    guidedTourDialogWindow = new BaseGuidedTourDialogWindow(parent) { MinHeight = 100,  MinWidth = 100,};
                    
                    var innerContent = new Border() { BorderBrush = new SolidColorBrush(Colors.Red), BorderThickness = new Thickness(3)};

                    innerContent.LayoutUpdated += (sender, args) => {
                        Debug.WriteLine("MainWindowViewModel#LayoutUpdated");
                        guidedTourDialogWindow.SetDialogStartupLocation();
                    };

                    guidedTourDialogWindow.Content = innerContent;
                    guidedTourDialogWindow.ShowWindow();
                    guidedTourDialogWindow.SetDialogStartupLocation();
                }
            );

            CloseGuidedTourExampleCommand = ReactiveCommand.Create(
                () => {
                    guidedTourDialogWindow.Close();
                }
            );
        }

        public ICommand OpenGuidedTourExampleCommand { get; }
        public ICommand CloseGuidedTourExampleCommand { get; }
    }
}