using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace AvaloniaHangProject.Views {
    public class BaseGuidedTourDialogWindow : Window {

        public BaseGuidedTourDialogWindow() {
            InitializeComponent();
            SetupWindowDimensions();
        }

        public BaseGuidedTourDialogWindow(Window owner) : this() {
            ParentWindow = owner;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupWindowDimensions() {
            SizeToContent = SizeToContent.WidthAndHeight;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        #region Avalonia Properties

        private static readonly StyledProperty<int> OwnerBoundsMarginProperty =
            AvaloniaProperty.Register<BaseGuidedTourDialogWindow, int>(nameof(OwnerBoundsMargin), defaultValue: 24, inherits: true);

        public int OwnerBoundsMargin {
            get => GetValue(OwnerBoundsMarginProperty);
            set => SetValue(OwnerBoundsMarginProperty, value);
        }

        #endregion

        internal PixelRect DialogRect => new PixelRect(Position, PixelSize.FromSize(Bounds.Size, PlatformImpl?.DesktopScaling ?? 1));

        internal PixelRect OwnerRect => new PixelRect(ParentWindow.Position, PixelSize.FromSize(ParentWindow.Bounds.Size, ParentWindow.PlatformImpl?.DesktopScaling ?? 1));

        public Window ParentWindow { get; }

        public void ShowWindow() {
            Show(ParentWindow);
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
            Debug.WriteLine(format: "DialogWindowExtensions#InnerSetDialogStartupLocation (window.OwnerRect.BottomLeft.X + window.OwnerBoundsMargin) = ({0} + {1}); (window.OwnerRect.BottomLeft.Y - window.OwnerBoundsMargin - window.DialogRect.Height) = ({2} - {3} - {4})", this.OwnerRect.BottomLeft.X, this.OwnerBoundsMargin, this.OwnerRect.BottomLeft.Y, this.OwnerBoundsMargin, this.DialogRect.Height);
            this.SetDialogCoordinates(new PixelPoint(
                x: this.OwnerRect.BottomLeft.X + this.OwnerBoundsMargin,
                y: this.OwnerRect.BottomLeft.Y - this.OwnerBoundsMargin - this.DialogRect.Height
            ));
        }

        private void SetDialogCoordinates(PixelPoint coordinates) {
            //if (window.WindowStartupLocation != WindowStartupLocation.Manual) {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            // }

            // if (window.Position != coordinates) {
            Debug.WriteLine(format:"DialogWindowExtensions#SetDialogCoordinates setting from ({0},{1}) to ({2},{3})", Position.X, Position.Y, coordinates.X, coordinates.Y);
            this.Position = coordinates;
            // } else {
            //     Debug.WriteLine(format:"DialogWindowExtensions#SetDialogCoordinates skipped from ({0},{1}) to ({2},{3})", window.Position.X, window.Position.Y, coordinates.X, coordinates.Y);
            // }
        }
    }
}