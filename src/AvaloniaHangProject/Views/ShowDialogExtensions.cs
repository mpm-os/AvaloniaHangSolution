using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;

namespace AvaloniaHangProject.Views {

    public static class ShowDialogExtensions {

        private static readonly IShowDialogHelper Helper = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
            (IShowDialogHelper)new MacOSShowDialogHelper() :
            new WindowsShowDialogHelper();

        private interface IShowDialogHelper {
            Task<T> ShowDialogSync<T>(Window window, Window owner);
        }

        private class MacOSShowDialogHelper : IShowDialogHelper {

            [DllImport("/usr/lib/libobjc.dylib")]
            private static extern IntPtr objc_getClass(string name);

            [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
            private static extern IntPtr GetHandle(string name);

            [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
            private static extern long Int64_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

            [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
            private static extern void Void_objc_msgSend(IntPtr receiver, IntPtr selector);

            [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
            private static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

            private static readonly IntPtr NsAppStaticClass = objc_getClass("NSApplication");
            private static readonly IntPtr SharedApplicationSelector = GetHandle("sharedApplication");
            private static readonly IntPtr RunModalForSelector = GetHandle("runModalForWindow:");
            private static readonly IntPtr StopModalSelector = GetHandle("stopModal");

            private static readonly Lazy<IntPtr> SharedApplication = new Lazy<IntPtr>(() => IntPtr_objc_msgSend(NsAppStaticClass, SharedApplicationSelector));

            public Task<T> ShowDialogSync<T>(Window window, Window owner) {
                var handle = (IMacOSTopLevelPlatformHandle)window.PlatformImpl.Handle;

                void OnDialogClosed(object sender, EventArgs e) {
                    Void_objc_msgSend(SharedApplication.Value, StopModalSelector);

                    window.Closed -= OnDialogClosed;
                }

                window.Closed += OnDialogClosed;
                var task = window.ShowDialog<T>(owner);
                // RICT-3612 - use background priority to avoid the following crash: "[NSApplication runModalForWindow:] may not be invoked inside of transaction begin/commit pair, or inside of transaction commit (usually this means it was invoked inside of a view's -drawRect: method.)"
                Dispatcher.UIThread.InvokeAsync(() => Int64_objc_msgSend_IntPtr(SharedApplication.Value, RunModalForSelector, handle.NSWindow), DispatcherPriority.Background);
                return task;
            }
        }

        private class WindowsShowDialogHelper : IShowDialogHelper {

            public Task<T> ShowDialogSync<T>(Window window, Window owner) {
                return window.ShowDialog<T>(owner);
            }
        }

        public static void ShowDialogSync(this Window window, Window owner) {
            window.ShowDialogSync<object>(owner);
        }

        public static Task<T> ShowDialogSync<T>(this Window window, Window owner) {
            return Helper.ShowDialogSync<T>(window, owner);
        }
    }
}
