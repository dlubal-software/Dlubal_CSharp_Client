namespace SteelHall.MAUI
{
    public partial class MainPage : ContentPage
    {
        //int count = 0;
        public const double BasicTextFontSize = 14;
        public const string BasicTextColor = "White";


        HallGenerator hallGenerator = new HallGenerator();
        VerticalBracing verticalBracing = new VerticalBracing();
        HorizontalBracing horizontalBracing = new HorizontalBracing();


        //double frameHeight = 5;
        //double frameSpan = 10;
        //double frameDistance = 5;
        //int frameNumber = 5;
        //double roofAngle = 10;
        public MainPage()
        {
            InitializeComponent();
            
        }

        //private void OnStartCalculationClicked(object sender, EventArgs e)
        //{
        //    //count += 4;

        //    //if (count == 1)
        //    //    CounterBtn.Text = $"Clicked {count} time";
        //    //else
        //    //    CounterBtn.Text = $"Clicked {count} times";

        //    //SemanticScreenReader.Announce(CounterBtn.Text);
        //    //horizontalBracing.BracingType = 1;
        //    //horizontalBracing.BracingNumber = 2;
        //    //horizontalBracing.LoopCount = 3;
        //    //horizontalBracing.Increment = 2;
        //    //verticalBracing.BracingType = 1;
        //    //verticalBracing.BracingNumber = 2;
        //    //verticalBracing.LoopCount = 3;
        //    //verticalBracing.Increment = 2;
        //    hallGenerator.GenerateHall(frameHeight, frameSpan, frameDistance, frameNumber, roofAngle, verticalBracing, horizontalBracing);
        }

        //private void VerticalBracing1Checked(object sender, EventArgs e)
        //{
        //    verticalBracing.BracingType = 1;
        //    verticalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
        //    verticalBracing.LoopCount = verticalBracing.BracingNumber / 2;
        //    verticalBracing.Increment = 2;
        //}
        //private void VerticalBracing2Checked(object sender, EventArgs e)
        //{
        //    verticalBracing.BracingType = 2;
        //    verticalBracing.BracingNumber = (frameNumber * 2) - 2;
        //    if (frameNumber % 2 == 0)
        //    {
        //        verticalBracing.LoopCount = (verticalBracing.BracingNumber / 2) + 1;
        //    }
        //    else
        //    {
        //        verticalBracing.LoopCount = verticalBracing.BracingNumber / 2;
        //    }
        //    verticalBracing.Increment = 4;
        //}
        //private void VerticalBracing3Checked(object sender, EventArgs e)
        //{
        //    verticalBracing.BracingType = 3;
        //    verticalBracing.BracingType = 3;
        //    verticalBracing.BracingNumber = 8;
        //    verticalBracing.LoopCount = 4;
        //    verticalBracing.Increment = (frameNumber * 2) - 4;
        //}
        //private void HorizontalBracing4Checked(object sender, EventArgs e)
        //{
        //    horizontalBracing.BracingType = 4;
        //    horizontalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
        //    horizontalBracing.LoopCount = horizontalBracing.BracingNumber / 2;
        //    horizontalBracing.Increment = 2;
        //    horizontalBracing.IncrementMiddleNode = 1;
        //}
        //private void HorizontalBracing5Checked(object sender, EventArgs e)
        //{
        //    horizontalBracing.BracingType = 5;

        //    if (frameNumber % 2 == 0)
        //    {
        //        horizontalBracing.BracingNumber = frameNumber * 2;
        //    }
        //    else
        //    {
        //        horizontalBracing.BracingNumber = frameNumber * 2 - 2;
        //    }

        //    horizontalBracing.LoopCount = horizontalBracing.BracingNumber / 2;
        //    horizontalBracing.Increment = 4;
        //    horizontalBracing.IncrementMiddleNode = 2;
        //}
        //private void HorizontalBracing6Checked(object sender, EventArgs e)
        //{
        //    horizontalBracing.BracingType = 6;
        //    horizontalBracing.BracingNumber = 8;
        //    horizontalBracing.LoopCount = 4;
        //    horizontalBracing.Increment = (frameNumber * 2) - 4;
        //    horizontalBracing.IncrementMiddleNode = frameNumber - 2;
        //}

        //private string ChangedInputHeight(object sender, TextChangedEventArgs e)
        //{
        //    return e.NewTextValue;
        //}

        //double frameHeight = HallGenerator.GetDoubleInput(ChangedInputHeight());
    //}
        
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