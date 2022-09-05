using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace AvaloniaHangProject.Views {
    public class DialogWindow : MainWindow {
        private Control topLevelView;
        private IDisposable[] disposableShowAndHideEventHandlers;

        public DialogWindow() {
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        public DialogWindow(Window aggregatorWindow, Control topLevelView) : this() {
            ParentWindow = aggregatorWindow;
            this.topLevelView = topLevelView;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        public bool IsModal { get; private set; }

        public Window ParentWindow { get; }

        public static IDisposable AddShownEventHandler(Control view, Action action) {
            void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e) => action();
            view.AttachedToVisualTree += OnAttachedToVisualTree;
            return Disposable.Create(() => view.AttachedToVisualTree -= OnAttachedToVisualTree);
        }

        public static IDisposable AddHiddenEventHandler(Control view, Action action) {
            void OnDettachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e) => action();
            view.DetachedFromVisualTree += OnDettachedToVisualTree;
            return Disposable.Create(() => view.DetachedFromVisualTree -= OnDettachedToVisualTree);
        }

        private void SubscribeTopLevelViewShowHideEvents() {
            if (disposableShowAndHideEventHandlers == null && topLevelView != null) {
                disposableShowAndHideEventHandlers = new[] {
                    AddShownEventHandler(topLevelView, OnTopLevelViewShown),
                    AddHiddenEventHandler(topLevelView, OnTopLevelViewHidden),
                };
            }
        }


        private void UnsubscribeTopLevelViewShowHideEvents() {
            if (disposableShowAndHideEventHandlers != null) {
                foreach (IDisposable x in disposableShowAndHideEventHandlers) {
                    x.Dispose();
                }
                disposableShowAndHideEventHandlers = null;
            }
        }

        private void SafeShow(Window ownerWindow = null) {
            SubscribeTopLevelViewShowHideEvents();

        // if (!dialogConfiguration.IsSticky && topLevelView?.IsVisible() == false) {
            //     isShowPending = true;
            //     return;
            // }
            //
            // if (!showCalled) {
            //     if (IsModal) {
            //         if (topLevelView != null) {
            //             var viewNonModalOwnedWindows = GetTopLevelViewOwnedWindows(nonModalWindowsPerView);
            //             var viewModalOwnedWindows = GetTopLevelViewOwnedWindows(modalWindowsPerView);
            //             Window directOwnerWindow;
            //             (ownerWindow, directOwnerWindow) = GetOwnerWindows(viewModalOwnedWindows, viewNonModalOwnedWindows);
            //
            //             HandleModalWindowOpen(viewModalOwnedWindows, viewNonModalOwnedWindows, visibleOwnerWindow: ownerWindow, directOwnerWindow);
            //
            //             Closed += delegate {
            //                 HandleModalWindowClose(viewModalOwnedWindows, viewNonModalOwnedWindows, visibleOwnerWindow: ownerWindow, directOwnerWindow);
            //             };
            //         }
            //     } else {
            //         if (topLevelView != null) {
            //             var viewNonModalOwnedWindows = GetTopLevelViewOwnedWindows(nonModalWindowsPerView);
            //             viewNonModalOwnedWindows.Add(this);
            //
            //             Closed += delegate { viewNonModalOwnedWindows.Remove(this); };
            //         }
            //     }
            //
            //     SetupNativeTopMenuIfNeeded();
            //
            //     ownerWindow ??= GetDefaultOwnerWindow();
            // }
            //
            // ResetPositionIfUnreachableWindow(ownerWindow);
            //
            // showCalled = true;
            // isShowPending = false;
            // SetShowActivated();
            //
            // void OnOwnerWindowStateChanged() {
            //     if (ownerWindow?.WindowState != WindowStateAvalonia.Minimized && !IsVisible) {
            //         ((BaseWindow)ownerWindow).WindowStateChanged -= OnOwnerWindowStateChanged;
            //         SafeShowCore(ownerWindow);
            //     }
            // }
            //
            // if (ownerWindow?.WindowState == WindowStateAvalonia.Minimized && ownerWindow is BaseWindow ownerBaseWindow) {
            //     ownerBaseWindow.WindowStateChanged += OnOwnerWindowStateChanged;
            //     Closed += delegate {
            //         ownerBaseWindow.WindowStateChanged -= OnOwnerWindowStateChanged;
            //     };
            // } else {
            //     SafeShowCore(ownerWindow);
            // }

            SafeShowCore(ownerWindow);

        }
        private static bool IsWindowValid(Window window) => window?.IsVisible == true && !(window is DialogWindow dialog);


       private void SafeShowCore(Window ownerWindow = null) {
           if (IsWindowValid(ownerWindow)) {
               ShowInTaskbar = false;
               Show(ownerWindow);
           } else {
               Show();
           }
       }


       public void DisplayModal(bool applicationWideModal) {
                    ShowWindow(true);
        }

       private void ShowWindow(bool showAsModal) {
         //   SetupWindowDimensions();
         //   SetupWindowOptions();
         //   SetupWindowPosition();
            IsModal = showAsModal;

           SafeShow();
        }

        private void OnTopLevelViewShown() {
            Show(ParentWindow);
        }

        private void OnTopLevelViewHidden() {
            Hide();
        }

        public void ShowDialog() {
            SubscribeTopLevelViewShowHideEvents();
            ShowDialog(ParentWindow);
        }

        public void ShowNonModal() {
            SubscribeTopLevelViewShowHideEvents();
            Show();
        }

        protected override void OnClosed(EventArgs e) {
            UnsubscribeTopLevelViewShowHideEvents();
            base.OnClosed(e);
        }
    }
}