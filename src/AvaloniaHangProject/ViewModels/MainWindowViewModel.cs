using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaHangProject.Views;
using ReactiveUI;

namespace AvaloniaHangProject.ViewModels {
    public class MainWindowViewModel : ReactiveObject {
        private List<DialogWindow> dialogWindows = new List<DialogWindow>();
        private int captionsCounter = 0;

        public MainWindowViewModel(Window parent) {
            OpenGuidedTourExampleCommand = ReactiveCommand.Create(
                () => {
                    var activeTab = ((MainWindow)parent).GetActiveTab();
                    var dialogWindow = new DialogWindow(parent, activeTab) { MinHeight = 100,  MinWidth = 100,};
                    var innerContent = new Border() { BorderBrush = new SolidColorBrush(Colors.Red), BorderThickness = new Thickness(3)};


                    // Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith((t) => {
                    //     Dispatcher.UIThread.InvokeAsync(() => {
                    //         // Do something with the window
                    //     });
                    // });

                    dialogWindow.Content = innerContent;
                    dialogWindow.ShowWindow();
                    dialogWindow.SetDialogStartupLocation();

                    dialogWindows.Add(dialogWindow);
                }
            );


            AddTabCommand = ReactiveCommand.Create(
                () => {
                    ((MainWindow)parent).AddTab(++captionsCounter);
                });
        }

        public ICommand OpenGuidedTourExampleCommand { get; }

        public ICommand AddTabCommand { get; }
    }
}