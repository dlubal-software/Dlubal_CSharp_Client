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
        VerticalBracing verticalBracing = new VerticalBracing();
        HorizontalBracing horizontalBracing = new HorizontalBracing();

        public HallGeneratorForm()
        {
            InitializeComponent();
        }
        public HallGenerator hallgenerator = new HallGenerator();
        private async void button1_Click(object sender, EventArgs e)
        {
            await StartCalculation();
        }

        public async Task StartCalculation()
        {
            labelCalculation.Text = "Calculation started!";
            //bool doubleCheck = HallGenerator.CheckDouble(frameHeight);
            double frameHeight = 0;
            //double frameHeight = double.Parse(textBoxFrameHeight.Text);
            try
            {
                frameHeight = HallGenerator.GetDoubleInput(textBoxFrameHeight.Text);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Please enter a number for frame height!");
            }
            //double frameHeight = HallGenerator.GetDoubleInput(textBoxFrameHeight.Text);
            //double frameSpan = double.Parse(textBoxFramSpan.Text);
            double frameSpan = HallGenerator.GetDoubleInput(textBoxFramSpan.Text);
            //double frameDistance = double.Parse(textBoxFrameDistance.Text);
            double frameDistance = HallGenerator.GetDoubleInput(textBoxFrameDistance.Text);
            //int frameNumber = int.Parse(textBoxFrameNumber.Text);
            int frameNumber = HallGenerator.GetIntegerInput(textBoxFrameNumber.Text);
            double roofAngle = (double.Parse(textBoxRoofAngle.Text)) * (Math.PI / 180);

            //vertical bracing

            if (radioButtonBracing1.Checked == true)
            {
                verticalBracing.BracingType = 1;
                verticalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
                verticalBracing.LoopCount = verticalBracing.BracingNumber / 2;
                verticalBracing.Increment = 2;
            }
            else if (radioButtonBracing2.Checked == true)
            {
                verticalBracing.BracingType = 2;
                verticalBracing.BracingNumber = (frameNumber * 2) - 2;
                if (frameNumber % 2 == 0)
                {
                    verticalBracing.LoopCount = (verticalBracing.BracingNumber / 2) + 1;
                }
                else
                {
                    verticalBracing.LoopCount = verticalBracing.BracingNumber / 2;
                }
                verticalBracing.Increment = 4;
            }
            else if (radioButtonBracing3.Checked == true)
            {
                verticalBracing.BracingType = 3;
                verticalBracing.BracingNumber = 8;
                verticalBracing.LoopCount = 4;
                verticalBracing.Increment = (frameNumber * 2) - 4;
            }

            //horizontal bracing
            if (radioButtonBracing4.Checked == true)
            {
                horizontalBracing.BracingType = 4;
                horizontalBracing.BracingNumber = 2 * (frameNumber * 2 - 2);
                horizontalBracing.LoopCount = horizontalBracing.BracingNumber / 2;
                horizontalBracing.Increment = 2;
                horizontalBracing.IncrementMiddleNode = 1;
            }
            else if (radioButtonBracing5.Checked == true)
            {
                horizontalBracing.BracingType = 5;
                if (frameNumber % 2 == 0)
                {
                    horizontalBracing.BracingNumber = frameNumber * 2;
                }
                else
                {
                    horizontalBracing.BracingNumber = frameNumber * 2 - 2;
                }
                //horizontalBracing.BracingNumber = frameNumber * 2 - 2;
                //if (frameNumber % 2 == 0)
                //{
                //    horizontalBracing.LoopCount = (horizontalBracing.BracingNumber / 2) + 1;
                //}
                //else
                //{
                //    horizontalBracing.LoopCount = horizontalBracing.BracingNumber / 2;
                //}
                horizontalBracing.LoopCount = horizontalBracing.BracingNumber / 2;
                horizontalBracing.Increment = 4;
                horizontalBracing.IncrementMiddleNode = 2;

            }
            else if (radioButtonBracing6.Checked == true)
            {
                horizontalBracing.BracingType = 6;
                horizontalBracing.BracingNumber = 8;
                horizontalBracing.LoopCount = 4;
                horizontalBracing.Increment = (frameNumber * 2) - 4;
                horizontalBracing.IncrementMiddleNode = frameNumber - 2;
            }

            await Task.Run(() => { hallgenerator.GenerateHall(frameHeight, frameSpan, frameDistance, frameNumber, roofAngle, verticalBracing, horizontalBracing); });
            //hallgenerator.GenerateHall(frameHeight, frameSpan, frameDistance, frameNumber, roofAngle, bracing);
            labelCalculation.Text = hallgenerator.CreateResultMessage();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            hallgenerator.CloseModel();
        }

        private void buttonCsv_Click(object sender, EventArgs e)
        {
            string currentDirectory = hallgenerator.ExportCsv();
            labelCalculation.Text = $"Results have been exported as CSV-files to \n {currentDirectory}.";
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Label_RoofAngle(object sender, EventArgs e)
        {

        }

        private void radioButtonBracing1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButtonBracing6_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}