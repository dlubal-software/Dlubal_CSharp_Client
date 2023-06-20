namespace Steel_Hall_GUI
{
    partial class HallGeneratorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HallGeneratorForm));
            radioButtonBracing4 = new RadioButton();
            radioButtonBracing5 = new RadioButton();
            radioButtonBracing6 = new RadioButton();
            buttonCalculate = new Button();
            buttonClose = new Button();
            labelCalculation = new Label();
            textBoxFrameHeight = new TextBox();
            textBoxFramSpan = new TextBox();
            textBoxFrameDistance = new TextBox();
            textBoxFrameNumber = new TextBox();
            fontDialog1 = new FontDialog();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            labelWarningMessage = new Label();
            label5 = new Label();
            buttonCsv = new Button();
            labelExport = new Label();
            labelResults = new Label();
            radioButtonBracing1 = new RadioButton();
            radioButtonBracing2 = new RadioButton();
            radioButtonBracing3 = new RadioButton();
            buttonDisconnect = new Button();
            Lable_RoofAngle = new Label();
            textBoxRoofAngle = new TextBox();
            pictureBox1 = new PictureBox();
            labelHeading = new Label();
            labelGeometry = new Label();
            labelHorizontalBracing = new Label();
            panelBracingVertical = new Panel();
            panelBracingHorizontal = new Panel();
            labelStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelBracingVertical.SuspendLayout();
            panelBracingHorizontal.SuspendLayout();
            SuspendLayout();
            // 
            // radioButtonBracing4
            // 
            radioButtonBracing4.AutoSize = true;
            radioButtonBracing4.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            radioButtonBracing4.ForeColor = SystemColors.Control;
            radioButtonBracing4.Location = new Point(7, 3);
            radioButtonBracing4.Name = "radioButtonBracing4";
            radioButtonBracing4.Size = new Size(193, 21);
            radioButtonBracing4.TabIndex = 29;
            radioButtonBracing4.TabStop = true;
            radioButtonBracing4.Text = "Include bracing in every field";
            radioButtonBracing4.UseVisualStyleBackColor = true;
            // 
            // radioButtonBracing5
            // 
            radioButtonBracing5.AutoSize = true;
            radioButtonBracing5.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            radioButtonBracing5.ForeColor = SystemColors.Control;
            radioButtonBracing5.Location = new Point(7, 30);
            radioButtonBracing5.Name = "radioButtonBracing5";
            radioButtonBracing5.Size = new Size(239, 21);
            radioButtonBracing5.TabIndex = 30;
            radioButtonBracing5.TabStop = true;
            radioButtonBracing5.Text = "Include bracing in every second field";
            radioButtonBracing5.UseVisualStyleBackColor = true;
            // 
            // radioButtonBracing6
            // 
            radioButtonBracing6.AutoSize = true;
            radioButtonBracing6.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            radioButtonBracing6.ForeColor = SystemColors.Control;
            radioButtonBracing6.Location = new Point(7, 57);
            radioButtonBracing6.Name = "radioButtonBracing6";
            radioButtonBracing6.Size = new Size(240, 21);
            radioButtonBracing6.TabIndex = 31;
            radioButtonBracing6.TabStop = true;
            radioButtonBracing6.Text = "Include bracing only in the end fields";
            radioButtonBracing6.UseVisualStyleBackColor = true;
            radioButtonBracing6.CheckedChanged += radioButtonBracing6_CheckedChanged;
            // 
            // buttonCalculate
            // 
            buttonCalculate.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            buttonCalculate.ForeColor = Color.FromArgb(19, 12, 84);
            buttonCalculate.Location = new Point(298, 328);
            buttonCalculate.Name = "buttonCalculate";
            buttonCalculate.Size = new Size(173, 31);
            buttonCalculate.TabIndex = 0;
            buttonCalculate.Text = "Start Calculation";
            buttonCalculate.UseVisualStyleBackColor = true;
            buttonCalculate.Click += button1_Click;
            // 
            // buttonClose
            // 
            buttonClose.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            buttonClose.ForeColor = Color.FromArgb(19, 12, 84);
            buttonClose.Location = new Point(604, 330);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(195, 29);
            buttonClose.TabIndex = 1;
            buttonClose.Text = "Close Model";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // labelCalculation
            // 
            labelCalculation.AutoSize = true;
            labelCalculation.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            labelCalculation.ForeColor = SystemColors.Control;
            labelCalculation.Location = new Point(298, 461);
            labelCalculation.Name = "labelCalculation";
            labelCalculation.Size = new Size(235, 17);
            labelCalculation.TabIndex = 2;
            labelCalculation.Text = "Calculation has not been started yet!";
            // 
            // textBoxFrameHeight
            // 
            textBoxFrameHeight.Location = new Point(416, 87);
            textBoxFrameHeight.Name = "textBoxFrameHeight";
            textBoxFrameHeight.Size = new Size(54, 23);
            textBoxFrameHeight.TabIndex = 3;
            // 
            // textBoxFramSpan
            // 
            textBoxFramSpan.Location = new Point(416, 122);
            textBoxFramSpan.Name = "textBoxFramSpan";
            textBoxFramSpan.Size = new Size(54, 23);
            textBoxFramSpan.TabIndex = 4;
            // 
            // textBoxFrameDistance
            // 
            textBoxFrameDistance.Location = new Point(416, 162);
            textBoxFrameDistance.Name = "textBoxFrameDistance";
            textBoxFrameDistance.Size = new Size(54, 23);
            textBoxFrameDistance.TabIndex = 5;
            // 
            // textBoxFrameNumber
            // 
            textBoxFrameNumber.Location = new Point(417, 199);
            textBoxFrameNumber.Name = "textBoxFrameNumber";
            textBoxFrameNumber.Size = new Size(54, 23);
            textBoxFrameNumber.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(288, 93);
            label1.Name = "label1";
            label1.Size = new Size(112, 17);
            label1.TabIndex = 10;
            label1.Text = "Frame Height [m]:";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(297, 128);
            label2.Name = "label2";
            label2.Size = new Size(103, 17);
            label2.TabIndex = 11;
            label2.Text = "Frame Span [m]:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(218, 168);
            label3.Name = "label3";
            label3.Size = new Size(182, 17);
            label3.TabIndex = 12;
            label3.Text = "Distance between Frames [m]:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = SystemColors.Control;
            label4.Location = new Point(279, 205);
            label4.Name = "label4";
            label4.Size = new Size(121, 17);
            label4.TabIndex = 13;
            label4.Text = "Number of Frames:";
            // 
            // labelWarningMessage
            // 
            labelWarningMessage.AutoSize = true;
            labelWarningMessage.Location = new Point(339, 207);
            labelWarningMessage.Name = "labelWarningMessage";
            labelWarningMessage.Size = new Size(0, 15);
            labelWarningMessage.TabIndex = 14;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
            label5.ForeColor = SystemColors.Control;
            label5.Location = new Point(605, 44);
            label5.Name = "label5";
            label5.Size = new Size(152, 25);
            label5.TabIndex = 15;
            label5.Text = "Vertical Bracing";
            // 
            // buttonCsv
            // 
            buttonCsv.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            buttonCsv.ForeColor = Color.FromArgb(19, 12, 84);
            buttonCsv.Location = new Point(298, 377);
            buttonCsv.Name = "buttonCsv";
            buttonCsv.Size = new Size(173, 29);
            buttonCsv.TabIndex = 16;
            buttonCsv.Text = "Export Results to CSV-File";
            buttonCsv.UseVisualStyleBackColor = true;
            buttonCsv.Click += buttonCsv_Click;
            // 
            // labelExport
            // 
            labelExport.AutoSize = true;
            labelExport.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            labelExport.ForeColor = SystemColors.Control;
            labelExport.Location = new Point(630, 122);
            labelExport.Name = "labelExport";
            labelExport.Size = new Size(0, 30);
            labelExport.TabIndex = 17;
            // 
            // labelResults
            // 
            labelResults.AutoSize = true;
            labelResults.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            labelResults.ForeColor = SystemColors.Control;
            labelResults.Location = new Point(586, 287);
            labelResults.Name = "labelResults";
            labelResults.Size = new Size(0, 30);
            labelResults.TabIndex = 18;
            // 
            // radioButtonBracing1
            // 
            radioButtonBracing1.AutoSize = true;
            radioButtonBracing1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            radioButtonBracing1.ForeColor = SystemColors.Control;
            radioButtonBracing1.Location = new Point(7, 9);
            radioButtonBracing1.Name = "radioButtonBracing1";
            radioButtonBracing1.Size = new Size(193, 21);
            radioButtonBracing1.TabIndex = 19;
            radioButtonBracing1.TabStop = true;
            radioButtonBracing1.Text = "Include bracing in every field";
            radioButtonBracing1.UseVisualStyleBackColor = true;
            radioButtonBracing1.CheckedChanged += radioButtonBracing1_CheckedChanged;
            // 
            // radioButtonBracing2
            // 
            radioButtonBracing2.AutoSize = true;
            radioButtonBracing2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            radioButtonBracing2.ForeColor = SystemColors.Control;
            radioButtonBracing2.Location = new Point(7, 36);
            radioButtonBracing2.Name = "radioButtonBracing2";
            radioButtonBracing2.Size = new Size(239, 21);
            radioButtonBracing2.TabIndex = 20;
            radioButtonBracing2.TabStop = true;
            radioButtonBracing2.Text = "Include bracing in every second field";
            radioButtonBracing2.UseVisualStyleBackColor = true;
            // 
            // radioButtonBracing3
            // 
            radioButtonBracing3.AutoSize = true;
            radioButtonBracing3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            radioButtonBracing3.ForeColor = SystemColors.Control;
            radioButtonBracing3.Location = new Point(7, 63);
            radioButtonBracing3.Name = "radioButtonBracing3";
            radioButtonBracing3.Size = new Size(240, 21);
            radioButtonBracing3.TabIndex = 21;
            radioButtonBracing3.TabStop = true;
            radioButtonBracing3.Text = "Include bracing only in the end fields";
            radioButtonBracing3.UseVisualStyleBackColor = true;
            // 
            // buttonDisconnect
            // 
            buttonDisconnect.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            buttonDisconnect.ForeColor = Color.FromArgb(19, 12, 84);
            buttonDisconnect.Location = new Point(604, 377);
            buttonDisconnect.Name = "buttonDisconnect";
            buttonDisconnect.Size = new Size(195, 29);
            buttonDisconnect.TabIndex = 22;
            buttonDisconnect.Text = "Close this app and free RFEM";
            buttonDisconnect.UseVisualStyleBackColor = true;
            buttonDisconnect.Click += buttonDisconnect_Click;
            // 
            // Lable_RoofAngle
            // 
            Lable_RoofAngle.AutoSize = true;
            Lable_RoofAngle.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            Lable_RoofAngle.ForeColor = SystemColors.Control;
            Lable_RoofAngle.Location = new Point(272, 242);
            Lable_RoofAngle.Name = "Lable_RoofAngle";
            Lable_RoofAngle.Size = new Size(128, 17);
            Lable_RoofAngle.TabIndex = 23;
            Lable_RoofAngle.Text = "Roof Pitch Angle [°]: ";
            Lable_RoofAngle.Click += Label_RoofAngle;
            // 
            // textBoxRoofAngle
            // 
            textBoxRoofAngle.Location = new Point(416, 236);
            textBoxRoofAngle.Name = "textBoxRoofAngle";
            textBoxRoofAngle.Size = new Size(54, 23);
            textBoxRoofAngle.TabIndex = 24;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.logo_white;
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(-13, 368);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(177, 177);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 25;
            pictureBox1.TabStop = false;
            // 
            // labelHeading
            // 
            labelHeading.AutoSize = true;
            labelHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            labelHeading.ForeColor = SystemColors.Control;
            labelHeading.Location = new Point(12, 13);
            labelHeading.Name = "labelHeading";
            labelHeading.Size = new Size(242, 32);
            labelHeading.TabIndex = 26;
            labelHeading.Text = "Steel Hall Generator";
            // 
            // labelGeometry
            // 
            labelGeometry.AutoSize = true;
            labelGeometry.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
            labelGeometry.ForeColor = SystemColors.Control;
            labelGeometry.Location = new Point(339, 44);
            labelGeometry.Name = "labelGeometry";
            labelGeometry.Size = new Size(101, 25);
            labelGeometry.TabIndex = 27;
            labelGeometry.Text = "Geometry";
            // 
            // labelHorizontalBracing
            // 
            labelHorizontalBracing.AutoSize = true;
            labelHorizontalBracing.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
            labelHorizontalBracing.ForeColor = SystemColors.Control;
            labelHorizontalBracing.Location = new Point(605, 181);
            labelHorizontalBracing.Name = "labelHorizontalBracing";
            labelHorizontalBracing.Size = new Size(180, 25);
            labelHorizontalBracing.TabIndex = 28;
            labelHorizontalBracing.Text = "Horizontal Bracing";
            // 
            // panelBracingVertical
            // 
            panelBracingVertical.Controls.Add(radioButtonBracing1);
            panelBracingVertical.Controls.Add(radioButtonBracing2);
            panelBracingVertical.Controls.Add(radioButtonBracing3);
            panelBracingVertical.Location = new Point(584, 78);
            panelBracingVertical.Name = "panelBracingVertical";
            panelBracingVertical.Size = new Size(254, 100);
            panelBracingVertical.TabIndex = 34;
            // 
            // panelBracingHorizontal
            // 
            panelBracingHorizontal.Controls.Add(radioButtonBracing4);
            panelBracingHorizontal.Controls.Add(radioButtonBracing5);
            panelBracingHorizontal.Controls.Add(radioButtonBracing6);
            panelBracingHorizontal.Location = new Point(584, 224);
            panelBracingHorizontal.Name = "panelBracingHorizontal";
            panelBracingHorizontal.Size = new Size(247, 100);
            panelBracingHorizontal.TabIndex = 35;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            labelStatus.ForeColor = SystemColors.Control;
            labelStatus.Location = new Point(297, 425);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(61, 21);
            labelStatus.TabIndex = 36;
            labelStatus.Text = "Status:";
            // 
            // HallGeneratorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(19, 12, 84);
            ClientSize = new Size(839, 523);
            Controls.Add(labelStatus);
            Controls.Add(panelBracingHorizontal);
            Controls.Add(panelBracingVertical);
            Controls.Add(labelHorizontalBracing);
            Controls.Add(labelGeometry);
            Controls.Add(labelHeading);
            Controls.Add(pictureBox1);
            Controls.Add(textBoxRoofAngle);
            Controls.Add(Lable_RoofAngle);
            Controls.Add(buttonDisconnect);
            Controls.Add(labelResults);
            Controls.Add(labelExport);
            Controls.Add(buttonCsv);
            Controls.Add(label5);
            Controls.Add(labelWarningMessage);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxFrameNumber);
            Controls.Add(textBoxFrameDistance);
            Controls.Add(textBoxFramSpan);
            Controls.Add(textBoxFrameHeight);
            Controls.Add(labelCalculation);
            Controls.Add(buttonClose);
            Controls.Add(buttonCalculate);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "HallGeneratorForm";
            Text = "Steel Hall Generator";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelBracingVertical.ResumeLayout(false);
            panelBracingVertical.PerformLayout();
            panelBracingHorizontal.ResumeLayout(false);
            panelBracingHorizontal.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonCalculate;
        private Button button1;
        private Label labelCalculation;
        private TextBox textBoxFrameHeight;
        private TextBox textBoxFramSpan;
        private TextBox textBoxFrameDistance;
        private TextBox textBoxFrameNumber;
        private FontDialog fontDialog1;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label labelExport;
        private Label label3;
        private Label labelWarningMessage;
        private Label label5;
        private Button buttonClose;
        private Button buttonCsv;
        private Label labelResults;
        private RadioButton radioButtonBracing1;
        private RadioButton radioButtonBracing2;
        private RadioButton radioButtonBracing3;
        private Button buttonDisconnect;
        private Label Lable_RoofAngle;
        private TextBox textBoxRoofAngle;
        private PictureBox pictureBox1;
        private Label labelHeading;
        private Label labelGeometry;
        private Label labelHorizontalBracing;
        private RadioButton radioButtonBracing4;
        private RadioButton radioButtonBracing5;
        private RadioButton radioButtonBracing6;
        private Panel panelBracingVertical;
        private Panel panelBracingHorizontal;
        private Label labelStatus;
    }
}