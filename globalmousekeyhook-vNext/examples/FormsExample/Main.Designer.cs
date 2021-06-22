namespace Demo
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBoxSuppressMouse = new System.Windows.Forms.CheckBox();
            this.panelSeparator = new System.Windows.Forms.Panel();
            this.radioGlobal = new System.Windows.Forms.RadioButton();
            this.radioApplication = new System.Windows.Forms.RadioButton();
            this.labelWheel = new System.Windows.Forms.Label();
            this.labelMousePosition = new System.Windows.Forms.Label();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.checkBoxSupressMouseWheel = new System.Windows.Forms.CheckBox();
            this.radioNone = new System.Windows.Forms.RadioButton();
            this.labelHWheel = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxSuppressMouse
            // 
            this.checkBoxSuppressMouse.AutoSize = true;
            this.checkBoxSuppressMouse.Location = new System.Drawing.Point(246, 57);
            this.checkBoxSuppressMouse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxSuppressMouse.Name = "checkBoxSuppressMouse";
            this.checkBoxSuppressMouse.Size = new System.Drawing.Size(159, 17);
            this.checkBoxSuppressMouse.TabIndex = 13;
            this.checkBoxSuppressMouse.Text = "Suppress Right Mouse Click";
            this.checkBoxSuppressMouse.UseVisualStyleBackColor = true;
            this.checkBoxSuppressMouse.CheckedChanged += new System.EventHandler(this.checkBoxSuppressMouse_CheckedChanged);
            // 
            // panelSeparator
            // 
            this.panelSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSeparator.BackColor = System.Drawing.Color.White;
            this.panelSeparator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSeparator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.panelSeparator.Location = new System.Drawing.Point(7, 43);
            this.panelSeparator.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelSeparator.Name = "panelSeparator";
            this.panelSeparator.Size = new System.Drawing.Size(681, 1);
            this.panelSeparator.TabIndex = 11;
            // 
            // radioGlobal
            // 
            this.radioGlobal.AutoSize = true;
            this.radioGlobal.BackColor = System.Drawing.Color.White;
            this.radioGlobal.Checked = true;
            this.radioGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.radioGlobal.Location = new System.Drawing.Point(148, 15);
            this.radioGlobal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioGlobal.Name = "radioGlobal";
            this.radioGlobal.Size = new System.Drawing.Size(87, 17);
            this.radioGlobal.TabIndex = 10;
            this.radioGlobal.TabStop = true;
            this.radioGlobal.Text = "Global hooks";
            this.radioGlobal.UseVisualStyleBackColor = false;
            this.radioGlobal.CheckedChanged += new System.EventHandler(this.radioGlobal_CheckedChanged);
            // 
            // radioApplication
            // 
            this.radioApplication.AutoSize = true;
            this.radioApplication.BackColor = System.Drawing.Color.White;
            this.radioApplication.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.radioApplication.Location = new System.Drawing.Point(14, 15);
            this.radioApplication.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioApplication.Name = "radioApplication";
            this.radioApplication.Size = new System.Drawing.Size(109, 17);
            this.radioApplication.TabIndex = 9;
            this.radioApplication.Text = "Application hooks";
            this.radioApplication.UseVisualStyleBackColor = false;
            this.radioApplication.CheckedChanged += new System.EventHandler(this.radioApplication_CheckedChanged);
            // 
            // labelWheel
            // 
            this.labelWheel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWheel.AutoSize = true;
            this.labelWheel.BackColor = System.Drawing.Color.White;
            this.labelWheel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelWheel.Location = new System.Drawing.Point(10, 88);
            this.labelWheel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelWheel.Name = "labelWheel";
            this.labelWheel.Size = new System.Drawing.Size(89, 13);
            this.labelWheel.TabIndex = 6;
            this.labelWheel.Text = "Wheel={0:####}";
            // 
            // labelMousePosition
            // 
            this.labelMousePosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMousePosition.AutoSize = true;
            this.labelMousePosition.BackColor = System.Drawing.Color.White;
            this.labelMousePosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelMousePosition.Location = new System.Drawing.Point(10, 58);
            this.labelMousePosition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMousePosition.Name = "labelMousePosition";
            this.labelMousePosition.Size = new System.Drawing.Size(125, 13);
            this.labelMousePosition.TabIndex = 2;
            this.labelMousePosition.Text = "x={0:####}; y={1:####}";
            // 
            // textBoxLog
            // 
            this.textBoxLog.BackColor = System.Drawing.Color.White;
            this.textBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxLog.Location = new System.Drawing.Point(0, 122);
            this.textBoxLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(702, 269);
            this.textBoxLog.TabIndex = 8;
            this.textBoxLog.WordWrap = false;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.labelHWheel);
            this.groupBox2.Controls.Add(this.clearLogButton);
            this.groupBox2.Controls.Add(this.checkBoxSupressMouseWheel);
            this.groupBox2.Controls.Add(this.radioNone);
            this.groupBox2.Controls.Add(this.checkBoxSuppressMouse);
            this.groupBox2.Controls.Add(this.panelSeparator);
            this.groupBox2.Controls.Add(this.radioGlobal);
            this.groupBox2.Controls.Add(this.radioApplication);
            this.groupBox2.Controls.Add(this.labelWheel);
            this.groupBox2.Controls.Add(this.labelMousePosition);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(702, 122);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Location = new System.Drawing.Point(601, 83);
            this.clearLogButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(88, 27);
            this.clearLogButton.TabIndex = 16;
            this.clearLogButton.Text = "Clear Log";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLog_Click);
            // 
            // checkBoxSupressMouseWheel
            // 
            this.checkBoxSupressMouseWheel.AutoSize = true;
            this.checkBoxSupressMouseWheel.Location = new System.Drawing.Point(246, 83);
            this.checkBoxSupressMouseWheel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxSupressMouseWheel.Name = "checkBoxSupressMouseWheel";
            this.checkBoxSupressMouseWheel.Size = new System.Drawing.Size(139, 17);
            this.checkBoxSupressMouseWheel.TabIndex = 15;
            this.checkBoxSupressMouseWheel.Text = "Suppress Mouse Wheel";
            this.checkBoxSupressMouseWheel.UseVisualStyleBackColor = true;
            this.checkBoxSupressMouseWheel.CheckedChanged += new System.EventHandler(this.checkBoxSupressMouseWheel_CheckedChanged);
            // 
            // radioNone
            // 
            this.radioNone.AutoSize = true;
            this.radioNone.BackColor = System.Drawing.Color.White;
            this.radioNone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.radioNone.Location = new System.Drawing.Point(257, 15);
            this.radioNone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioNone.Name = "radioNone";
            this.radioNone.Size = new System.Drawing.Size(51, 17);
            this.radioNone.TabIndex = 14;
            this.radioNone.Text = "None";
            this.radioNone.UseVisualStyleBackColor = false;
            this.radioNone.CheckedChanged += new System.EventHandler(this.radioNone_CheckedChanged);
            // 
            // labelHWheel
            // 
            this.labelHWheel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHWheel.AutoSize = true;
            this.labelHWheel.BackColor = System.Drawing.Color.White;
            this.labelHWheel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelHWheel.Location = new System.Drawing.Point(128, 88);
            this.labelHWheel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelHWheel.Name = "labelHWheel";
            this.labelHWheel.Size = new System.Drawing.Size(97, 13);
            this.labelHWheel.TabIndex = 17;
            this.labelHWheel.Text = "HWheel={0:####}";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 391);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Main";
            this.Text = "Mouse and Keyboard Hooks Demo";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxSuppressMouse;
        private System.Windows.Forms.Panel panelSeparator;
        private System.Windows.Forms.RadioButton radioGlobal;
        private System.Windows.Forms.RadioButton radioApplication;
        private System.Windows.Forms.Label labelWheel;
        private System.Windows.Forms.Label labelMousePosition;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioNone;
        private System.Windows.Forms.CheckBox checkBoxSupressMouseWheel;
        private System.Windows.Forms.Button clearLogButton;
        private System.Windows.Forms.Label labelHWheel;
    }
}

