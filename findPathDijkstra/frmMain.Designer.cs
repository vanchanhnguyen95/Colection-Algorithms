
namespace findPathDijkstra
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
            this.grbSelectEngine = new System.Windows.Forms.GroupBox();
            this.txtNumberYen = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rdoDijkstra = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuMain = new System.Windows.Forms.ToolStripMenuItem();
            this.mniOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mniReset = new System.Windows.Forms.ToolStripMenuItem();
            this.mniExit = new System.Windows.Forms.ToolStripMenuItem();
            this.grbInput = new System.Windows.Forms.GroupBox();
            this.lbTime = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtDest = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbOutput = new System.Windows.Forms.GroupBox();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.grbSelectEngine.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.grbInput.SuspendLayout();
            this.grbOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbSelectEngine
            // 
            this.grbSelectEngine.Controls.Add(this.txtNumberYen);
            this.grbSelectEngine.Controls.Add(this.label1);
            this.grbSelectEngine.Controls.Add(this.rdoDijkstra);
            this.grbSelectEngine.Location = new System.Drawing.Point(12, 27);
            this.grbSelectEngine.Name = "grbSelectEngine";
            this.grbSelectEngine.Size = new System.Drawing.Size(788, 134);
            this.grbSelectEngine.TabIndex = 3;
            this.grbSelectEngine.TabStop = false;
            this.grbSelectEngine.Text = "Chọn cách tính";
            // 
            // txtNumberYen
            // 
            this.txtNumberYen.Enabled = false;
            this.txtNumberYen.Location = new System.Drawing.Point(165, 77);
            this.txtNumberYen.Name = "txtNumberYen";
            this.txtNumberYen.Size = new System.Drawing.Size(100, 23);
            this.txtNumberYen.TabIndex = 5;
            this.txtNumberYen.Text = "10";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "Số đường đi ngắn nhất muốn tìm:";
            // 
            // rdoDijkstra
            // 
            this.rdoDijkstra.AutoSize = true;
            this.rdoDijkstra.Location = new System.Drawing.Point(6, 22);
            this.rdoDijkstra.Name = "rdoDijkstra";
            this.rdoDijkstra.Size = new System.Drawing.Size(203, 19);
            this.rdoDijkstra.TabIndex = 0;
            this.rdoDijkstra.TabStop = true;
            this.rdoDijkstra.Text = "Tìm đường đi ngắn nhất - Dijkstra";
            this.rdoDijkstra.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMain});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuMain
            // 
            this.mnuMain.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniOpenFile,
            this.mniReset,
            this.mniExit});
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(50, 20);
            this.mnuMain.Text = "Menu";
            // 
            // mniOpenFile
            // 
            this.mniOpenFile.Name = "mniOpenFile";
            this.mniOpenFile.Size = new System.Drawing.Size(121, 22);
            this.mniOpenFile.Text = "OpenFile";
            this.mniOpenFile.Click += new System.EventHandler(this.mniOpenFile_Click);
            // 
            // mniReset
            // 
            this.mniReset.Name = "mniReset";
            this.mniReset.Size = new System.Drawing.Size(121, 22);
            this.mniReset.Text = "Reset";
            // 
            // mniExit
            // 
            this.mniExit.Name = "mniExit";
            this.mniExit.Size = new System.Drawing.Size(121, 22);
            this.mniExit.Text = "Exit";
            this.mniExit.Click += new System.EventHandler(this.mniExit_Click);
            // 
            // grbInput
            // 
            this.grbInput.Controls.Add(this.lbTime);
            this.grbInput.Controls.Add(this.btnExecute);
            this.grbInput.Controls.Add(this.txtDest);
            this.grbInput.Controls.Add(this.label3);
            this.grbInput.Controls.Add(this.txtSource);
            this.grbInput.Controls.Add(this.label2);
            this.grbInput.Location = new System.Drawing.Point(12, 181);
            this.grbInput.Name = "grbInput";
            this.grbInput.Size = new System.Drawing.Size(788, 100);
            this.grbInput.TabIndex = 4;
            this.grbInput.TabStop = false;
            this.grbInput.Text = "Nhập vào 2 điểm cần tìm đường đi";
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.lbTime.Location = new System.Drawing.Point(7, 59);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(198, 15);
            this.lbTime.TabIndex = 5;
            this.lbTime.Text = "Thời gian thực hiện chương trình: ";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(430, 20);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 4;
            this.btnExecute.Text = "Tính";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtDest
            // 
            this.txtDest.Location = new System.Drawing.Point(282, 20);
            this.txtDest.Name = "txtDest";
            this.txtDest.Size = new System.Drawing.Size(100, 23);
            this.txtDest.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(212, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Điểm đến: ";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(59, 20);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(100, 23);
            this.txtSource.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Điểm đi: ";
            // 
            // grbOutput
            // 
            this.grbOutput.Controls.Add(this.rtbResult);
            this.grbOutput.Location = new System.Drawing.Point(12, 313);
            this.grbOutput.Name = "grbOutput";
            this.grbOutput.Size = new System.Drawing.Size(788, 197);
            this.grbOutput.TabIndex = 5;
            this.grbOutput.TabStop = false;
            this.grbOutput.Text = "Đường đi ngắn nhất";
            // 
            // rtbResult
            // 
            this.rtbResult.Location = new System.Drawing.Point(6, 22);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(776, 169);
            this.rtbResult.TabIndex = 0;
            this.rtbResult.Text = "";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 538);
            this.Controls.Add(this.grbOutput);
            this.Controls.Add(this.grbInput);
            this.Controls.Add(this.grbSelectEngine);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmMain";
            this.Text = "Đường đi ngắn nhất";
            this.grbSelectEngine.ResumeLayout(false);
            this.grbSelectEngine.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grbInput.ResumeLayout(false);
            this.grbInput.PerformLayout();
            this.grbOutput.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbSelectEngine;
        private System.Windows.Forms.TextBox txtNumberYen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdoDijkstra;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mniOpenFile;
        private System.Windows.Forms.ToolStripMenuItem mniReset;
        private System.Windows.Forms.ToolStripMenuItem mniExit;
        private System.Windows.Forms.GroupBox grbInput;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtDest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grbOutput;
        private System.Windows.Forms.RichTextBox rtbResult;
    }
}

