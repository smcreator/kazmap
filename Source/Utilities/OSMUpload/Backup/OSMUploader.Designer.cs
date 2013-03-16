namespace OSMUpload
{
    partial class OSMUploader
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTargetFile = new System.Windows.Forms.TextBox();
            this.btnBrowseTarget = new System.Windows.Forms.Button();
            this.cbCreateRouting = new System.Windows.Forms.CheckBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.pbxMap = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNbNodes = new System.Windows.Forms.TextBox();
            this.txtNbWays = new System.Windows.Forms.TextBox();
            this.txtNbRels = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tbCurrentProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnBuildMatrix = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxMap)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "OSM File:";
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFile.Location = new System.Drawing.Point(106, 19);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(493, 20);
            this.txtSourceFile.TabIndex = 1;
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSource.Location = new System.Drawing.Point(605, 17);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.Text = "Browse";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Target Database:";
            // 
            // txtTargetFile
            // 
            this.txtTargetFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetFile.Location = new System.Drawing.Point(106, 45);
            this.txtTargetFile.Name = "txtTargetFile";
            this.txtTargetFile.Size = new System.Drawing.Size(493, 20);
            this.txtTargetFile.TabIndex = 4;
            // 
            // btnBrowseTarget
            // 
            this.btnBrowseTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseTarget.Location = new System.Drawing.Point(605, 43);
            this.btnBrowseTarget.Name = "btnBrowseTarget";
            this.btnBrowseTarget.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseTarget.TabIndex = 5;
            this.btnBrowseTarget.Text = "Browse";
            this.btnBrowseTarget.UseVisualStyleBackColor = true;
            this.btnBrowseTarget.Click += new System.EventHandler(this.btnBrowseTarget_Click);
            // 
            // cbCreateRouting
            // 
            this.cbCreateRouting.AutoSize = true;
            this.cbCreateRouting.Checked = true;
            this.cbCreateRouting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCreateRouting.Location = new System.Drawing.Point(15, 75);
            this.cbCreateRouting.Name = "cbCreateRouting";
            this.cbCreateRouting.Size = new System.Drawing.Size(124, 17);
            this.cbCreateRouting.TabIndex = 6;
            this.cbCreateRouting.Text = "Build Routing Tables";
            this.cbCreateRouting.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(573, 71);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(107, 23);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run Upload";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // pbxMap
            // 
            this.pbxMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxMap.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbxMap.Location = new System.Drawing.Point(15, 113);
            this.pbxMap.Name = "pbxMap";
            this.pbxMap.Size = new System.Drawing.Size(417, 230);
            this.pbxMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxMap.TabIndex = 9;
            this.pbxMap.TabStop = false;
            this.pbxMap.SizeChanged += new System.EventHandler(this.pbxMap_SizeChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(451, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Uploaded Nodes:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(455, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Uploaded Ways:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(438, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Uploaded Relations:";
            // 
            // txtNbNodes
            // 
            this.txtNbNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNbNodes.Location = new System.Drawing.Point(547, 118);
            this.txtNbNodes.Name = "txtNbNodes";
            this.txtNbNodes.ReadOnly = true;
            this.txtNbNodes.Size = new System.Drawing.Size(133, 20);
            this.txtNbNodes.TabIndex = 13;
            // 
            // txtNbWays
            // 
            this.txtNbWays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNbWays.Location = new System.Drawing.Point(547, 148);
            this.txtNbWays.Name = "txtNbWays";
            this.txtNbWays.ReadOnly = true;
            this.txtNbWays.Size = new System.Drawing.Size(133, 20);
            this.txtNbWays.TabIndex = 14;
            // 
            // txtNbRels
            // 
            this.txtNbRels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNbRels.Location = new System.Drawing.Point(547, 176);
            this.txtNbRels.Name = "txtNbRels";
            this.txtNbRels.ReadOnly = true;
            this.txtNbRels.Size = new System.Drawing.Size(133, 20);
            this.txtNbRels.TabIndex = 15;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbCurrentProgress,
            this.tbStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 359);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(692, 22);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tbCurrentProgress
            // 
            this.tbCurrentProgress.Name = "tbCurrentProgress";
            this.tbCurrentProgress.Size = new System.Drawing.Size(200, 16);
            this.tbCurrentProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // tbStatus
            // 
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.Size = new System.Drawing.Size(65, 17);
            this.tbStatus.Text = "... v1.00 ...";
            // 
            // btnBuildMatrix
            // 
            this.btnBuildMatrix.Location = new System.Drawing.Point(458, 71);
            this.btnBuildMatrix.Name = "btnBuildMatrix";
            this.btnBuildMatrix.Size = new System.Drawing.Size(109, 23);
            this.btnBuildMatrix.TabIndex = 17;
            this.btnBuildMatrix.Text = "Rebuild Matrix";
            this.btnBuildMatrix.UseVisualStyleBackColor = true;
            this.btnBuildMatrix.Click += new System.EventHandler(this.btnBuildMatrix_Click);
            // 
            // OSMUploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 381);
            this.Controls.Add(this.btnBuildMatrix);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtNbRels);
            this.Controls.Add(this.txtNbWays);
            this.Controls.Add(this.txtNbNodes);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pbxMap);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.cbCreateRouting);
            this.Controls.Add(this.btnBrowseTarget);
            this.Controls.Add(this.txtTargetFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowseSource);
            this.Controls.Add(this.txtSourceFile);
            this.Controls.Add(this.label1);
            this.Name = "OSMUploader";
            this.Text = "OSM Uploader";
            ((System.ComponentModel.ISupportInitialize)(this.pbxMap)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTargetFile;
        private System.Windows.Forms.Button btnBrowseTarget;
        private System.Windows.Forms.CheckBox cbCreateRouting;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.PictureBox pbxMap;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtNbNodes;
        private System.Windows.Forms.TextBox txtNbWays;
        private System.Windows.Forms.TextBox txtNbRels;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar tbCurrentProgress;
        private System.Windows.Forms.ToolStripStatusLabel tbStatus;
        private System.Windows.Forms.Button btnBuildMatrix;
    }
}

