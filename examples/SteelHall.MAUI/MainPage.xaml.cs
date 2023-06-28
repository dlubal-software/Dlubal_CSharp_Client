namespace SteelHall.MAUI
{
    public partial class MainPage : ContentPage
    {
        public const double BasicTextFontSize = 14;
        public const string BasicTextColor = "White";

        public MainPage()
        {
            InitializeComponent();
            
        }              
    }

    public class GlobalFontSizeExtension : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return MainPage.BasicTextFontSize;
        }
    }
    public class  GlobalTextColor : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider provider)
        {
            return MainPage.BasicTextColor;
        }
    }
}