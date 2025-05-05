namespace AI_Test_Verification_Tool
{
    partial class frmMain
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
            pnlMain = new Panel();
            btnStart = new Button();
            btnLoad = new Button();
            txtPath = new TextBox();
            lblQuestions = new Label();
            pnlBot = new Panel();
            rtbOutput = new RichTextBox();
            pnlMain.SuspendLayout();
            pnlBot.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(btnStart);
            pnlMain.Controls.Add(btnLoad);
            pnlMain.Controls.Add(txtPath);
            pnlMain.Controls.Add(lblQuestions);
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(1280, 720);
            pnlMain.TabIndex = 0;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(468, 334);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(330, 23);
            btnStart.TabIndex = 3;
            btnStart.Text = "Start asking questions";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(723, 304);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "Select File";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // txtPath
            // 
            txtPath.Location = new Point(468, 305);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(249, 23);
            txtPath.TabIndex = 1;
            // 
            // lblQuestions
            // 
            lblQuestions.AutoSize = true;
            lblQuestions.Location = new Point(587, 287);
            lblQuestions.Name = "lblQuestions";
            lblQuestions.Size = new Size(100, 15);
            lblQuestions.TabIndex = 0;
            lblQuestions.Text = "Questions.txt File:";
            // 
            // pnlBot
            // 
            pnlBot.Controls.Add(rtbOutput);
            pnlBot.Location = new Point(0, 0);
            pnlBot.Name = "pnlBot";
            pnlBot.Size = new Size(1280, 720);
            pnlBot.TabIndex = 1;
            // 
            // rtbOutput
            // 
            rtbOutput.Dock = DockStyle.Fill;
            rtbOutput.Location = new Point(0, 0);
            rtbOutput.Name = "rtbOutput";
            rtbOutput.Size = new Size(1280, 720);
            rtbOutput.TabIndex = 0;
            rtbOutput.Text = "";
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1281, 722);
            Controls.Add(pnlMain);
            Controls.Add(pnlBot);
            Name = "frmMain";
            Text = "AI Test";
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            pnlBot.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMain;
        private Panel pnlBot;
        private Button btnStart;
        private Button btnLoad;
        private TextBox txtPath;
        private Label lblQuestions;
        private RichTextBox rtbOutput;
    }
}
