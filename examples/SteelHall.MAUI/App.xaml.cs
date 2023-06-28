#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using System;
#endif

namespace SteelHall.MAUI
{
    public partial class App : Application
    {
        //const int WindowWidth = 950;
        //const int WindowHeight = 525;
        //const int width = (int)MainPage.WidthRequest;
        //const int height = (int)MainPage.HeightRequest;

        public App()
        {
            InitializeComponent();


            MainPage = new AppShell();
            //            var width = (int)MainPage.WidthRequest;
            //            var height = (int)MainPage.HeightRequest;

            //            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            //            {
            //#if WINDOWS
            //                                    var mauiWindow = handler.VirtualView;
            //                                    var nativeWindow = handler.PlatformView;
            //                                    nativeWindow.Activate();
            //                                    IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            //                                    WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            //                                    AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            //                                    appWindow.Resize(new SizeInt32(width, height));
            //#endif
            //            });
            //        }

            //protected override Window CreateWindow(IActivationState activationState)
            //{
            //    var window = base.CreateWindow(activationState);
            //    const int newWidth = 950;
            //    const int newHeight = 650;
            //    var screen = DeviceDisplay.Current.MainDisplayInfo;

            //    window.Width = Auto;
            //    window.Height = newHeight;
            //    window.MinimumWidth = window.MaximumWidth = newWidth;
            //    window.MaximumHeight = window.MinimumHeight = newHeight;

            //    return window;
        }
    }
}