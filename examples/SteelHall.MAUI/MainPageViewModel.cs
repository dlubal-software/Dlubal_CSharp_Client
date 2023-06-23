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
                    this.HallGenerator.GenerateHall(this.FrameHeight, this.FrameSpan, this.FrameDistance, this.FrameNumber, this.RoofAngle, new VerticalBracing(), new HorizontalBracing());
                }
                );
            //this.CloseModel = new DelegateCommand(
            //    (o) =>
            //    {
            //        this.HallGenerator.CloseModel();
            //    }
            //    );
        }

        public HallGenerator HallGenerator { get; set; } = new();
       

        private double frameHeight;
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
                    roofAngle = value;
                    this.RaisePropertyChanged();
                }
            }        
        }

       

        public DelegateCommand StartCalculationCommand { get; set; }
        public Delegate CloseModel { get; set; }

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
