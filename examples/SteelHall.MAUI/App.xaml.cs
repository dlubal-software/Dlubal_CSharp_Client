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
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();            
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            const int newWidth = 950;
            const int newHeight = 525;

            window.MinimumWidth = window.MaximumWidth = newWidth;
            window.MaximumHeight = window.MinimumHeight = newHeight;

            return window;
        }
    }
}