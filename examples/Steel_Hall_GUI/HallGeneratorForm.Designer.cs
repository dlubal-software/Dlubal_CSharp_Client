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
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelCalculation = new System.Windows.Forms.Label();
            this.textBoxFrameHeight = new System.Windows.Forms.TextBox();
            this.textBoxFramSpan = new System.Windows.Forms.TextBox();
            this.textBoxFrameDistance = new System.Windows.Forms.TextBox();
            this.textBoxFrameNumber = new System.Windows.Forms.TextBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelWarningMessage = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonCsv = new System.Windows.Forms.Button();
            this.labelExport = new System.Windows.Forms.Label();
            this.labelResults = new System.Windows.Forms.Label();
            this.radioButtonBracing1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBracing2 = new System.Windows.Forms.RadioButton();
            this.radioButtonBracing3 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Location = new System.Drawing.Point(21, 232);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.Size = new System.Drawing.Size(144, 27);
            this.buttonCalculate.TabIndex = 0;
            this.buttonCalculate.Text = "Start Calculation";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(350, 232);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(144, 27);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close Model";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelCalculation
            // 
            this.labelCalculation.AutoSize = true;
            this.labelCalculation.Location = new System.Drawing.Point(661, 422);
            this.labelCalculation.Name = "labelCalculation";
            this.labelCalculation.Size = new System.Drawing.Size(0, 15);
            this.labelCalculation.TabIndex = 2;
            // 
            // textBoxFrameHeight
            // 
            this.textBoxFrameHeight.Location = new System.Drawing.Point(171, 26);
            this.textBoxFrameHeight.Name = "textBoxFrameHeight";
            this.textBoxFrameHeight.Size = new System.Drawing.Size(100, 23);
            this.textBoxFrameHeight.TabIndex = 3;
            // 
            // textBoxFramSpan
            // 
            this.textBoxFramSpan.Location = new System.Drawing.Point(171, 69);
            this.textBoxFramSpan.Name = "textBoxFramSpan";
            this.textBoxFramSpan.Size = new System.Drawing.Size(100, 23);
            this.textBoxFramSpan.TabIndex = 4;
            // 
            // textBoxFrameDistance
            // 
            this.textBoxFrameDistance.Location = new System.Drawing.Point(171, 114);
            this.textBoxFrameDistance.Name = "textBoxFrameDistance";
            this.textBoxFrameDistance.Size = new System.Drawing.Size(100, 23);
            this.textBoxFrameDistance.TabIndex = 5;
            // 
            // textBoxFrameNumber
            // 
            this.textBoxFrameNumber.Location = new System.Drawing.Point(171, 161);
            this.textBoxFrameNumber.Name = "textBoxFrameNumber";
            this.textBoxFrameNumber.Size = new System.Drawing.Size(100, 23);
            this.textBoxFrameNumber.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Frame Height [m]:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Frame Span [m]:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-1, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "Distance between Frames [m]:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(56, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Number of Frames:";
            // 
            // labelWarningMessage
            // 
            this.labelWarningMessage.AutoSize = true;
            this.labelWarningMessage.Location = new System.Drawing.Point(339, 207);
            this.labelWarningMessage.Name = "labelWarningMessage";
            this.labelWarningMessage.Size = new System.Drawing.Size(0, 15);
            this.labelWarningMessage.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(499, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 15);
            this.label5.TabIndex = 15;
            this.label5.Text = "Vertical Bracing:";
            // 
            // buttonCsv
            // 
            this.buttonCsv.Location = new System.Drawing.Point(171, 231);
            this.buttonCsv.Name = "buttonCsv";
            this.buttonCsv.Size = new System.Drawing.Size(173, 29);
            this.buttonCsv.TabIndex = 16;
            this.buttonCsv.Text = "Export Results to CSV-File";
            this.buttonCsv.UseVisualStyleBackColor = true;
            this.buttonCsv.Click += new System.EventHandler(this.buttonCsv_Click);
            // 
            // labelExport
            // 
            this.labelExport.AutoSize = true;
            this.labelExport.Location = new System.Drawing.Point(21, 285);
            this.labelExport.Name = "labelExport";
            this.labelExport.Size = new System.Drawing.Size(0, 15);
            this.labelExport.TabIndex = 17;
            // 
            // labelResults
            // 
            this.labelResults.AutoSize = true;
            this.labelResults.Location = new System.Drawing.Point(12, 270);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(0, 15);
            this.labelResults.TabIndex = 18;
            // 
            // radioButtonBracing1
            // 
            this.radioButtonBracing1.AutoSize = true;
            this.radioButtonBracing1.Location = new System.Drawing.Point(499, 41);
            this.radioButtonBracing1.Name = "radioButtonBracing1";
            this.radioButtonBracing1.Size = new System.Drawing.Size(177, 19);
            this.radioButtonBracing1.TabIndex = 19;
            this.radioButtonBracing1.TabStop = true;
            this.radioButtonBracing1.Text = "Include bracing in every field";
            this.radioButtonBracing1.UseVisualStyleBackColor = true;
            // 
            // radioButtonBracing2
            // 
            this.radioButtonBracing2.AutoSize = true;
            this.radioButtonBracing2.Location = new System.Drawing.Point(499, 70);
            this.radioButtonBracing2.Name = "radioButtonBracing2";
            this.radioButtonBracing2.Size = new System.Drawing.Size(218, 19);
            this.radioButtonBracing2.TabIndex = 20;
            this.radioButtonBracing2.TabStop = true;
            this.radioButtonBracing2.Text = "Include bracing in every second field";
            this.radioButtonBracing2.UseVisualStyleBackColor = true;
            // 
            // radioButtonBracing3
            // 
            this.radioButtonBracing3.AutoSize = true;
            this.radioButtonBracing3.Location = new System.Drawing.Point(499, 97);
            this.radioButtonBracing3.Name = "radioButtonBracing3";
            this.radioButtonBracing3.Size = new System.Drawing.Size(220, 19);
            this.radioButtonBracing3.TabIndex = 21;
            this.radioButtonBracing3.TabStop = true;
            this.radioButtonBracing3.Text = "Include bracing only in the end fields";
            this.radioButtonBracing3.UseVisualStyleBackColor = true;
            // 
            // HallGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(848, 513);
            this.Controls.Add(this.radioButtonBracing3);
            this.Controls.Add(this.radioButtonBracing2);
            this.Controls.Add(this.radioButtonBracing1);
            this.Controls.Add(this.labelResults);
            this.Controls.Add(this.labelExport);
            this.Controls.Add(this.buttonCsv);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelWarningMessage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFrameNumber);
            this.Controls.Add(this.textBoxFrameDistance);
            this.Controls.Add(this.textBoxFramSpan);
            this.Controls.Add(this.textBoxFrameHeight);
            this.Controls.Add(this.labelCalculation);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonCalculate);
            this.Name = "HallGeneratorForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}