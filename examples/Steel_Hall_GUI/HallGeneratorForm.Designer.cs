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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // buttonCalculate
            // 
            buttonCalculate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonCalculate.Location = new Point(298, 239);
            buttonCalculate.Name = "buttonCalculate";
            buttonCalculate.Size = new Size(173, 31);
            buttonCalculate.TabIndex = 0;
            buttonCalculate.Text = "Start Calculation";
            buttonCalculate.UseVisualStyleBackColor = true;
            buttonCalculate.Click += button1_Click;
            // 
            // buttonClose
            // 
            buttonClose.Location = new Point(606, 195);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(173, 29);
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
            labelCalculation.Location = new Point(282, 287);
            labelCalculation.Name = "labelCalculation";
            labelCalculation.Size = new Size(0, 17);
            labelCalculation.TabIndex = 2;
            // 
            // textBoxFrameHeight
            // 
            textBoxFrameHeight.Location = new Point(417, 31);
            textBoxFrameHeight.Name = "textBoxFrameHeight";
            textBoxFrameHeight.Size = new Size(54, 23);
            textBoxFrameHeight.TabIndex = 3;
            // 
            // textBoxFramSpan
            // 
            textBoxFramSpan.Location = new Point(417, 72);
            textBoxFramSpan.Name = "textBoxFramSpan";
            textBoxFramSpan.Size = new Size(54, 23);
            textBoxFramSpan.TabIndex = 4;
            // 
            // textBoxFrameDistance
            // 
            textBoxFrameDistance.Location = new Point(417, 114);
            textBoxFrameDistance.Name = "textBoxFrameDistance";
            textBoxFrameDistance.Size = new Size(54, 23);
            textBoxFrameDistance.TabIndex = 5;
            // 
            // textBoxFrameNumber
            // 
            textBoxFrameNumber.Location = new Point(417, 156);
            textBoxFrameNumber.Name = "textBoxFrameNumber";
            textBoxFrameNumber.Size = new Size(54, 23);
            textBoxFrameNumber.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(288, 37);
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
            label2.Location = new Point(297, 78);
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
            label3.Location = new Point(218, 122);
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
            label4.Location = new Point(279, 162);
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
            label5.Location = new Point(586, 9);
            label5.Name = "label5";
            label5.Size = new Size(152, 25);
            label5.TabIndex = 15;
            label5.Text = "Vertical Bracing";
            // 
            // buttonCsv
            // 
            buttonCsv.Location = new Point(606, 152);
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
            radioButtonBracing1.Location = new Point(586, 48);
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
            radioButtonBracing2.Location = new Point(586, 73);
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
            radioButtonBracing3.Location = new Point(586, 98);
            radioButtonBracing3.Name = "radioButtonBracing3";
            radioButtonBracing3.Size = new Size(240, 21);
            radioButtonBracing3.TabIndex = 21;
            radioButtonBracing3.TabStop = true;
            radioButtonBracing3.Text = "Include bracing only in the end fields";
            radioButtonBracing3.UseVisualStyleBackColor = true;
            // 
            // buttonDisconnect
            // 
            buttonDisconnect.Location = new Point(606, 239);
            buttonDisconnect.Name = "buttonDisconnect";
            buttonDisconnect.Size = new Size(173, 29);
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
            Lable_RoofAngle.Location = new Point(272, 200);
            Lable_RoofAngle.Name = "Lable_RoofAngle";
            Lable_RoofAngle.Size = new Size(128, 17);
            Lable_RoofAngle.TabIndex = 23;
            Lable_RoofAngle.Text = "Roof Pitch Angle [°]: ";
            Lable_RoofAngle.Click += Label_RoofAngle;
            // 
            // textBoxRoofAngle
            // 
            textBoxRoofAngle.Location = new Point(417, 199);
            textBoxRoofAngle.Name = "textBoxRoofAngle";
            textBoxRoofAngle.Size = new Size(54, 23);
            textBoxRoofAngle.TabIndex = 24;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.logo_white;
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(-4, 169);
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
            // HallGeneratorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(19, 12, 84);
            ClientSize = new Size(839, 340);
            Controls.Add(labelHeading);
            Controls.Add(pictureBox1);
            Controls.Add(textBoxRoofAngle);
            Controls.Add(Lable_RoofAngle);
            Controls.Add(buttonDisconnect);
            Controls.Add(radioButtonBracing3);
            Controls.Add(radioButtonBracing2);
            Controls.Add(radioButtonBracing1);
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
    }
}