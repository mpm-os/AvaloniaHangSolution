using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace AvaloniaHangProject.Views {
    public class DialogWindow : Window {
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

        private void OnTopLevelViewShown() {
            Show(ParentWindow);
        }

        private void OnTopLevelViewHidden() {
            Hide();
        }

        public void ShowWindow() {
            SubscribeTopLevelViewShowHideEvents();
            Show(ParentWindow);
        }

        protected override void OnClosed(EventArgs e) {
            UnsubscribeTopLevelViewShowHideEvents();
            base.OnClosed(e);
        }
    }
}