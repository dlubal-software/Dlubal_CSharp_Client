using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace SteelHall.MAUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {

            this.StartCalculationCommand = new DelegateCommand(
                async (o) =>
                {
                    await StartCalculation();                    
                }
                );

            this.CloseModelCommand = new DelegateCommand(
                (o) =>
                {
                    this.HallGenerator.CloseModel();
                }
                );

            this.ExportCsvCommand = new DelegateCommand(
                (o) =>
                {
                    this.HallGenerator.ExportCsv();
                    StatusText = "Results have been exported as csv-file!";
                }
                );

            this.FreeRfemCommand = new DelegateCommand(
                (o) =>
                {
                    System.Environment.Exit(0);
                }
                );
        }

        public async Task StartCalculation()
        {
            StatusText = "Calculation started!";
            this.CheckVerticalBracing();
            this.CheckHorizontalBracing();

            await Task.Run(() =>
            {
                this.HallGenerator.GenerateHall(this.FrameHeight, this.FrameSpan, this.FrameDistance, this.FrameNumber, this.RoofAngle, VerticalBracing, HorizontalBracing);
            });

            StatusText = HallGenerator.CreateResultMessage();
        }

        public HallGenerator HallGenerator { get; set; } = new();
        public VerticalBracing VerticalBracing { get; set; } = new VerticalBracing();
        public HorizontalBracing HorizontalBracing { get; set; } = new HorizontalBracing();

        private string statusText = "Please use only number input! Otherwise default values will be applied!";
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                this.RaisePropertyChanged();
            }
        }

        public void ChangeStatusText(string message)
        {
            StatusText = message;
        }

        private double frameHeight = 5.0;
        public double FrameHeight
        {
            get => frameHeight;
            set
            {
                if (frameHeight != value)
                {
                    frameHeight = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private double frameSpan = 10.0;
        public double FrameSpan
        {
            get => frameSpan;
            set
            {
                if (frameSpan != value)
                {
                    frameSpan = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private double frameDistance = 5.0;
        public double FrameDistance
        {
            get => frameDistance;
            set
            {
                if (frameDistance != value)
                {
                    frameDistance = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private int frameNumber = 5;
        public int FrameNumber
        {
            get => frameNumber;
            set
            {
                if (frameNumber != value)
                {
                    frameNumber = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private double roofAngle = 12.0;
        public double RoofAngle
        {
            get => roofAngle;
            set
            {
                if (roofAngle != value)
                {
                    roofAngle = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool RadioButtonVerticalEveryFieldChecked { get; set; } = false;
        public bool RadioButtonVerticalSecondFieldChecked { get; set; } = false;
        public bool RadioButtonVerticalEndFieldChecked { get; set; } = false;
        public bool RadioButtonHorizontalEveryFieldChecked { get; set; } = false;
        public bool RadioButtonHorizontalSecondFieldChecked { get; set; } = false;
        public bool RadioButtonHorizontalEndFieldChecked { get; set; } = false;

        public VerticalBracing CheckVerticalBracing()
        {
            if (RadioButtonVerticalEveryFieldChecked)
            {
                VerticalBracing.BracingType = 1;
                VerticalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
                VerticalBracing.LoopCount = VerticalBracing.BracingNumber / 2;
                VerticalBracing.Increment = 2;
            }
            else if (RadioButtonVerticalSecondFieldChecked)
            {
                VerticalBracing.BracingType = 2;
                VerticalBracing.BracingNumber = (frameNumber * 2) - 2;
                if (frameNumber % 2 == 0)
                {
                    VerticalBracing.LoopCount = (VerticalBracing.BracingNumber / 2) + 1;
                }
                else
                {
                    VerticalBracing.LoopCount = VerticalBracing.BracingNumber / 2;
                }
                VerticalBracing.Increment = 4;
            }
            else if (RadioButtonVerticalEndFieldChecked)
            {
                VerticalBracing.BracingType = 3;
                VerticalBracing.BracingNumber = 8;
                VerticalBracing.LoopCount = 4;
                VerticalBracing.Increment = (frameNumber * 2) - 4;
            }
            return VerticalBracing;
        }

        public HorizontalBracing CheckHorizontalBracing()
        {
            if (RadioButtonHorizontalEveryFieldChecked)
            {
                HorizontalBracing.BracingType = 4;
                HorizontalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
                HorizontalBracing.LoopCount = HorizontalBracing.BracingNumber / 2;
                HorizontalBracing.Increment = 2;
                HorizontalBracing.IncrementMiddleNode = 1;
            }
            else if (RadioButtonHorizontalSecondFieldChecked)
            {
                HorizontalBracing.BracingType = 5;
                if (frameNumber % 2 == 0)
                {
                    HorizontalBracing.BracingNumber = frameNumber * 2;
                }
                else
                {
                    HorizontalBracing.BracingNumber = frameNumber * 2 - 2;
                }

                HorizontalBracing.LoopCount = HorizontalBracing.BracingNumber / 2;
                HorizontalBracing.Increment = 4;
                HorizontalBracing.IncrementMiddleNode = 2;
            }
            else if (RadioButtonHorizontalEndFieldChecked)
            {
                HorizontalBracing.BracingType = 6;
                HorizontalBracing.BracingNumber = 8;
                HorizontalBracing.LoopCount = 4;
                HorizontalBracing.Increment = (frameNumber * 2) - 4;
                HorizontalBracing.IncrementMiddleNode = frameNumber - 2;
            }
            return HorizontalBracing;
        }      

        public DelegateCommand StartCalculationCommand { get; set; }
        public DelegateCommand CloseModelCommand { get; set; }
        public DelegateCommand ExportCsvCommand { get; set; }
        public DelegateCommand FreeRfemCommand { get; set; }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 5;
            }
            else if (double.TryParse(value.ToString(), out double doubleValue) == false)
            {
                return 5;
            }
            else
            {                
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
            {
                return 5;
            }
            string stringValue = (string)value;


            try
            {
                return double.Parse((stringValue).Replace(".", ","), CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return 5;
            }
            
        }
    }

    public class StringToIntConverter : IValueConverter
    {
        //converts data type back to UI value(string)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 5;
            }
            else if (int.TryParse(value.ToString(), out int intValue) == false)
            {
                if (double.TryParse(value.ToString(), out double doubleValue) == true)
                {
                    return Math.Round(doubleValue);
                }
                else
                {
                    return 5;
                }
            }
            return value.ToString();
        }
        //Convert Input in UI to data type
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value.ToString();

            if (value == null)
            {
                return 5;
            }
            else if (int.TryParse(value.ToString(), out int intValue) == false)
            {
                if (double.TryParse(stringValue.Replace(".", ","), CultureInfo.CurrentCulture, out double doubleValue) == true && stringValue.Length > 2)
                {
                    return Math.Round(doubleValue);
                }
                else
                {
                    if (stringValue.Length < 3)
                    {
                        return stringValue;
                    }
                    return 5;
                }
            }
            return int.Parse(value.ToString(), CultureInfo.CurrentCulture);
        }
    }
}
