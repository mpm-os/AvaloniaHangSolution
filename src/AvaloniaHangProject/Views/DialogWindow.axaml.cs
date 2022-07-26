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


        #region Avalonia Properties

        private static readonly StyledProperty<int> OwnerBoundsMarginProperty =
            AvaloniaProperty.Register<DialogWindow, int>(nameof(OwnerBoundsMargin), defaultValue: 24, inherits: true);

        public int OwnerBoundsMargin {
            get => GetValue(OwnerBoundsMarginProperty);
            set => SetValue(OwnerBoundsMarginProperty, value);
        }

        #endregion

        internal PixelRect DialogRect => new PixelRect(Position, PixelSize.FromSize(Bounds.Size, PlatformImpl?.DesktopScaling ?? 1));

        internal PixelRect OwnerRect => new PixelRect(ParentWindow.Position, PixelSize.FromSize(ParentWindow.Bounds.Size, ParentWindow.PlatformImpl?.DesktopScaling ?? 1));

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

        public void SetDialogStartupLocation() {
            if (ParentWindow == null ) {
                return;
            }

            if (PlatformImpl == null || !IsInitialized) {
                throw new Exception("The guided tour window must be shown before setting position.");
            }

            this.InnerSetDialogStartupLocation();
        }

        private void InnerSetDialogStartupLocation() {
            //BottomLeft:
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Position = new PixelPoint(
                x: this.OwnerRect.BottomLeft.X + this.OwnerBoundsMargin,
                y: this.OwnerRect.BottomLeft.Y - this.OwnerBoundsMargin - this.DialogRect.Height
            );
        }
    }
}