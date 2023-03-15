#if RFEM
using Dlubal.WS.Rfem6.Application;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using Dlubal.WS.Rfem6.Model;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB

#endif
using System;
using System.Globalization;
using System.IO;

namespace Steel_Hall_GUI
{
    public partial class HallGeneratorForm : Form
    {
        Bracing bracing = new Bracing();
        
        public HallGeneratorForm()
        {
            InitializeComponent();
        }
        public HallGenerator hallgenerator = new HallGenerator();
        private void button1_Click(object sender, EventArgs e)
        {
            labelCalculation.Text = "Calculation started!";
            //bool doubleCheck = HallGenerator.CheckDouble(frameHeight);
            //GenerateHall(fram);

            int frameHeight = int.Parse(textBoxFrameHeight.Text);
            int frameSpan = int.Parse(textBoxFramSpan.Text);
            int frameDistance = int.Parse(textBoxFrameDistance.Text);
            int frameNumber = int.Parse(textBoxFrameNumber.Text);

            if (radioButtonBracing1.Checked == true)
            {
                bracing.BracingType = 1;
                bracing.BracingNumber = 2 * (frameNumber * 2 - 2);
                bracing.LoopCount = bracing.BracingNumber / 2;
                bracing.Increment = 2;
            }
            else if (radioButtonBracing2.Checked == true)
            {
                bracing.BracingType = 2;
                bracing.BracingNumber = (frameNumber * 2) - 2;
                if (frameNumber % 2 == 0)
                {
                    bracing.LoopCount = (bracing.BracingNumber / 2) + 1;
                }
                else
                {
                    bracing.LoopCount = bracing.BracingNumber / 2;
                }
                bracing.Increment = 4;
            }
            else if (radioButtonBracing3.Checked == true) 
            {
                bracing.BracingType = 3;
                bracing.BracingNumber = 8;
                bracing.LoopCount = 4;
                bracing.Increment = (frameNumber * 2) - 4;
            }

            int[] variableListInt = { frameHeight, frameSpan, frameDistance, frameNumber };

            hallgenerator.GenerateHall(frameHeight, frameSpan, frameDistance, frameNumber, bracing);
            labelResults.Text = hallgenerator.CreateResultMessage();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            hallgenerator.CloseModel();
        }

        private void buttonCsv_Click(object sender, EventArgs e)
        {
            hallgenerator.ExportCsv();
            labelExport.Text = "Results have been exported as CSV-files to the current directory.";
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}