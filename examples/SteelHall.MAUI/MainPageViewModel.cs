using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelHall.MAUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {

            this.StartCalculationCommand = new DelegateCommand(
                (o) =>
                {
                    this.CheckVerticalBracing();
                    this.CheckHorizontalBracing();
                    this.HallGenerator.GenerateHall(this.FrameHeight, this.FrameSpan, this.FrameDistance, this.FrameNumber, this.RoofAngle, VerticalBracing, HorizontalBracing);
                    
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
                }
                );

            this.FreeRfemCommand = new DelegateCommand(
                (o) =>
                {
                    System.Environment.Exit( 0 );
                }
                );
        }

        public HallGenerator HallGenerator { get; set; } = new();
        public VerticalBracing VerticalBracing { get; set; } = new VerticalBracing();
        public HorizontalBracing HorizontalBracing { get; set; } = new HorizontalBracing();


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
        private double frameSpan;
        public double FrameSpan 
        {
            get => frameSpan;
            set
            {
                if(frameSpan != value)
                {
                    frameSpan = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private double frameDistance;
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

        private int frameNumber;
        public int FrameNumber 
        { 
            get => frameNumber;
            set
            {
                if (frameNumber!= value)
                {
                    frameNumber = value;
                    this.RaisePropertyChanged();
                }
            } 
        }

        private double roofAngle;
        public double RoofAngle 
        { 
            get => roofAngle;
            set
            {
                if (roofAngle != value)
                {
                    roofAngle = value * (Math.PI / 180);
                    this.RaisePropertyChanged();
                }
            }        
        }

        public bool RadioButton1Checked { get; set; } = false;
        public bool RadioButton2Checked { get; set; } = false;
        public bool RadioButton3Checked { get; set; } = false;
        public bool RadioButton4Checked { get; set; } = false;
        public bool RadioButton5Checked { get; set; } = false;
        public bool RadioButton6Checked { get; set; } = false;

        public VerticalBracing CheckVerticalBracing()
        {
            //VerticalBracing VerticalBracing = new VerticalBracing();

            if (RadioButton1Checked)
            {
                VerticalBracing.BracingType = 1;
                VerticalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
                VerticalBracing.LoopCount = VerticalBracing.BracingNumber / 2;
                VerticalBracing.Increment = 2;                
            }
            else if (RadioButton2Checked)
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
            else if (RadioButton3Checked)
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
            //HorizontalBracing HorizontalBracing = new HorizontalBracing();

            if (RadioButton4Checked)
            {
                HorizontalBracing.BracingType = 4;
                HorizontalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
                HorizontalBracing.LoopCount = HorizontalBracing.BracingNumber / 2;
                HorizontalBracing.Increment = 2;
                HorizontalBracing.IncrementMiddleNode = 1;
            }
            else if (RadioButton5Checked)
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
            else if (RadioButton6Checked)
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

        //CallermemberName = if parameter not given -> takes automatically parameter of property where method was called (FrameHeight)
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        

    }
}
