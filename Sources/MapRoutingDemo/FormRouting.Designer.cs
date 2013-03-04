namespace MapRoutingDemo
{
    partial class FormRouting
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDatabaseFile = new System.Windows.Forms.TextBox();
            this.btnOpenDatabase = new System.Windows.Forms.Button();
            this.gbxFrom = new System.Windows.Forms.GroupBox();
            this.txtFromCoord = new System.Windows.Forms.TextBox();
            this.btnFromView = new System.Windows.Forms.Button();
            this.btnFromFind = new System.Windows.Forms.Button();
            this.txtFromId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbxTo = new System.Windows.Forms.GroupBox();
            this.txtToCoord = new System.Windows.Forms.TextBox();
            this.btnToView = new System.Windows.Forms.Button();
            this.btnToFind = new System.Windows.Forms.Button();
            this.txtToId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabResults = new System.Windows.Forms.TabControl();
            this.tabProgress = new System.Windows.Forms.TabPage();
            this.txtStats = new System.Windows.Forms.TextBox();
            this.pbProgess = new System.Windows.Forms.PictureBox();
            this.tabItinerary = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgItinary = new System.Windows.Forms.DataGridView();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.btnRun = new System.Windows.Forms.Button();
            this.tmProgress = new System.Windows.Forms.Timer(this.components);
            this.cbClassicDijkstra = new System.Windows.Forms.CheckBox();
            this.cbDoubleSearch = new System.Windows.Forms.CheckBox();
            this.cbNoProgressView = new System.Windows.Forms.CheckBox();
            this.gbxFrom.SuspendLayout();
            this.gbxTo.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgess)).BeginInit();
            this.tabItinerary.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgItinary)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database File:";
            // 
            // txtDatabaseFile
            // 
            this.txtDatabaseFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabaseFile.Location = new System.Drawing.Point(93, 6);
            this.txtDatabaseFile.Name = "txtDatabaseFile";
            this.txtDatabaseFile.ReadOnly = true;
            this.txtDatabaseFile.Size = new System.Drawing.Size(538, 20);
            this.txtDatabaseFile.TabIndex = 1;
            // 
            // btnOpenDatabase
            // 
            this.btnOpenDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenDatabase.Location = new System.Drawing.Point(637, 4);
            this.btnOpenDatabase.Name = "btnOpenDatabase";
            this.btnOpenDatabase.Size = new System.Drawing.Size(75, 23);
            this.btnOpenDatabase.TabIndex = 2;
            this.btnOpenDatabase.Text = "Select file...";
            this.btnOpenDatabase.UseVisualStyleBackColor = true;
            this.btnOpenDatabase.Click += new System.EventHandler(this.btnOpenDatabase_Click);
            // 
            // gbxFrom
            // 
            this.gbxFrom.Controls.Add(this.txtFromCoord);
            this.gbxFrom.Controls.Add(this.btnFromView);
            this.gbxFrom.Controls.Add(this.btnFromFind);
            this.gbxFrom.Controls.Add(this.txtFromId);
            this.gbxFrom.Controls.Add(this.label2);
            this.gbxFrom.Location = new System.Drawing.Point(12, 32);
            this.gbxFrom.Name = "gbxFrom";
            this.gbxFrom.Size = new System.Drawing.Size(200, 95);
            this.gbxFrom.TabIndex = 3;
            this.gbxFrom.TabStop = false;
            this.gbxFrom.Text = "From";
            // 
            // txtFromCoord
            // 
            this.txtFromCoord.Location = new System.Drawing.Point(9, 68);
            this.txtFromCoord.Name = "txtFromCoord";
            this.txtFromCoord.ReadOnly = true;
            this.txtFromCoord.Size = new System.Drawing.Size(185, 20);
            this.txtFromCoord.TabIndex = 4;
            // 
            // btnFromView
            // 
            this.btnFromView.Enabled = false;
            this.btnFromView.Location = new System.Drawing.Point(90, 39);
            this.btnFromView.Name = "btnFromView";
            this.btnFromView.Size = new System.Drawing.Size(104, 23);
            this.btnFromView.TabIndex = 3;
            this.btnFromView.Text = "Quick View...";
            this.btnFromView.UseVisualStyleBackColor = true;
            this.btnFromView.Click += new System.EventHandler(this.btnFromView_Click);
            // 
            // btnFromFind
            // 
            this.btnFromFind.Enabled = false;
            this.btnFromFind.Location = new System.Drawing.Point(9, 39);
            this.btnFromFind.Name = "btnFromFind";
            this.btnFromFind.Size = new System.Drawing.Size(75, 23);
            this.btnFromFind.TabIndex = 2;
            this.btnFromFind.Text = "Search ...";
            this.btnFromFind.UseVisualStyleBackColor = true;
            this.btnFromFind.Click += new System.EventHandler(this.btnFromFind_Click);
            // 
            // txtFromId
            // 
            this.txtFromId.Location = new System.Drawing.Point(77, 13);
            this.txtFromId.Name = "txtFromId";
            this.txtFromId.Size = new System.Drawing.Size(117, 20);
            this.txtFromId.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Intersection:";
            // 
            // gbxTo
            // 
            this.gbxTo.Controls.Add(this.txtToCoord);
            this.gbxTo.Controls.Add(this.btnToView);
            this.gbxTo.Controls.Add(this.btnToFind);
            this.gbxTo.Controls.Add(this.txtToId);
            this.gbxTo.Controls.Add(this.label3);
            this.gbxTo.Location = new System.Drawing.Point(218, 32);
            this.gbxTo.Name = "gbxTo";
            this.gbxTo.Size = new System.Drawing.Size(200, 95);
            this.gbxTo.TabIndex = 4;
            this.gbxTo.TabStop = false;
            this.gbxTo.Text = "To";
            // 
            // txtToCoord
            // 
            this.txtToCoord.Location = new System.Drawing.Point(9, 68);
            this.txtToCoord.Name = "txtToCoord";
            this.txtToCoord.ReadOnly = true;
            this.txtToCoord.Size = new System.Drawing.Size(185, 20);
            this.txtToCoord.TabIndex = 4;
            // 
            // btnToView
            // 
            this.btnToView.Enabled = false;
            this.btnToView.Location = new System.Drawing.Point(90, 39);
            this.btnToView.Name = "btnToView";
            this.btnToView.Size = new System.Drawing.Size(104, 23);
            this.btnToView.TabIndex = 3;
            this.btnToView.Text = "Quick View...";
            this.btnToView.UseVisualStyleBackColor = true;
            this.btnToView.Click += new System.EventHandler(this.btnToView_Click);
            // 
            // btnToFind
            // 
            this.btnToFind.Enabled = false;
            this.btnToFind.Location = new System.Drawing.Point(9, 39);
            this.btnToFind.Name = "btnToFind";
            this.btnToFind.Size = new System.Drawing.Size(75, 23);
            this.btnToFind.TabIndex = 2;
            this.btnToFind.Text = "Search ...";
            this.btnToFind.UseVisualStyleBackColor = true;
            this.btnToFind.Click += new System.EventHandler(this.btnToFind_Click);
            // 
            // txtToId
            // 
            this.txtToId.Location = new System.Drawing.Point(77, 13);
            this.txtToId.Name = "txtToId";
            this.txtToId.Size = new System.Drawing.Size(117, 20);
            this.txtToId.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Intersection:";
            // 
            // tabResults
            // 
            this.tabResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabResults.Controls.Add(this.tabProgress);
            this.tabResults.Controls.Add(this.tabItinerary);
            this.tabResults.Location = new System.Drawing.Point(12, 133);
            this.tabResults.Name = "tabResults";
            this.tabResults.SelectedIndex = 0;
            this.tabResults.Size = new System.Drawing.Size(700, 339);
            this.tabResults.TabIndex = 5;
            // 
            // tabProgress
            // 
            this.tabProgress.Controls.Add(this.txtStats);
            this.tabProgress.Controls.Add(this.pbProgess);
            this.tabProgress.Location = new System.Drawing.Point(4, 22);
            this.tabProgress.Name = "tabProgress";
            this.tabProgress.Padding = new System.Windows.Forms.Padding(3);
            this.tabProgress.Size = new System.Drawing.Size(692, 313);
            this.tabProgress.TabIndex = 0;
            this.tabProgress.Text = "Progress";
            this.tabProgress.UseVisualStyleBackColor = true;
            // 
            // txtStats
            // 
            this.txtStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStats.Location = new System.Drawing.Point(0, 293);
            this.txtStats.Name = "txtStats";
            this.txtStats.ReadOnly = true;
            this.txtStats.Size = new System.Drawing.Size(692, 20);
            this.txtStats.TabIndex = 8;
            // 
            // pbProgess
            // 
            this.pbProgess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgess.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbProgess.Location = new System.Drawing.Point(3, 3);
            this.pbProgess.Name = "pbProgess";
            this.pbProgess.Size = new System.Drawing.Size(689, 284);
            this.pbProgess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbProgess.TabIndex = 0;
            this.pbProgess.TabStop = false;
            this.pbProgess.SizeChanged += new System.EventHandler(this.pbProgess_SizeChanged);
            // 
            // tabItinerary
            // 
            this.tabItinerary.Controls.Add(this.splitContainer1);
            this.tabItinerary.Location = new System.Drawing.Point(4, 22);
            this.tabItinerary.Name = "tabItinerary";
            this.tabItinerary.Padding = new System.Windows.Forms.Padding(3);
            this.tabItinerary.Size = new System.Drawing.Size(692, 313);
            this.tabItinerary.TabIndex = 1;
            this.tabItinerary.Text = "Itinerary";
            this.tabItinerary.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgItinary);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.webBrowser);
            this.splitContainer1.Size = new System.Drawing.Size(686, 307);
            this.splitContainer1.SplitterDistance = 356;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgItinary
            // 
            this.dgItinary.AllowUserToAddRows = false;
            this.dgItinary.AllowUserToDeleteRows = false;
            this.dgItinary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgItinary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgItinary.Location = new System.Drawing.Point(0, 0);
            this.dgItinary.Name = "dgItinary";
            this.dgItinary.Size = new System.Drawing.Size(356, 307);
            this.dgItinary.TabIndex = 0;
            this.dgItinary.CurrentCellChanged += new System.EventHandler(this.dgItinary_CurrentCellChanged);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(326, 307);
            this.webBrowser.TabIndex = 0;
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(424, 104);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(139, 23);
            this.btnRun.TabIndex = 6;
            this.btnRun.Text = "Find Shortest Route...";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // tmProgress
            // 
            this.tmProgress.Interval = 5000;
            this.tmProgress.Tick += new System.EventHandler(this.tmProgress_Tick);
            // 
            // cbClassicDijkstra
            // 
            this.cbClassicDijkstra.AutoSize = true;
            this.cbClassicDijkstra.Location = new System.Drawing.Point(424, 81);
            this.cbClassicDijkstra.Name = "cbClassicDijkstra";
            this.cbClassicDijkstra.Size = new System.Drawing.Size(118, 17);
            this.cbClassicDijkstra.TabIndex = 7;
            this.cbClassicDijkstra.Text = "Use classic Dijkstra";
            this.cbClassicDijkstra.UseVisualStyleBackColor = true;
            // 
            // cbDoubleSearch
            // 
            this.cbDoubleSearch.AutoSize = true;
            this.cbDoubleSearch.Checked = true;
            this.cbDoubleSearch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDoubleSearch.Location = new System.Drawing.Point(424, 58);
            this.cbDoubleSearch.Name = "cbDoubleSearch";
            this.cbDoubleSearch.Size = new System.Drawing.Size(158, 17);
            this.cbDoubleSearch.TabIndex = 8;
            this.cbDoubleSearch.Text = "Use double direction search";
            this.cbDoubleSearch.UseVisualStyleBackColor = true;
            // 
            // cbNoProgressView
            // 
            this.cbNoProgressView.AutoSize = true;
            this.cbNoProgressView.Location = new System.Drawing.Point(424, 35);
            this.cbNoProgressView.Name = "cbNoProgressView";
            this.cbNoProgressView.Size = new System.Drawing.Size(129, 17);
            this.cbNoProgressView.TabIndex = 9;
            this.cbNoProgressView.Text = "Don\'t display progress";
            this.cbNoProgressView.UseVisualStyleBackColor = true;
            // 
            // FormRouting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 484);
            this.Controls.Add(this.cbNoProgressView);
            this.Controls.Add(this.cbDoubleSearch);
            this.Controls.Add(this.cbClassicDijkstra);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.tabResults);
            this.Controls.Add(this.gbxTo);
            this.Controls.Add(this.gbxFrom);
            this.Controls.Add(this.btnOpenDatabase);
            this.Controls.Add(this.txtDatabaseFile);
            this.Controls.Add(this.label1);
            this.Name = "FormRouting";
            this.Text = "Laurent Dupuis\'s Map Routing Demo";
            this.gbxFrom.ResumeLayout(false);
            this.gbxFrom.PerformLayout();
            this.gbxTo.ResumeLayout(false);
            this.gbxTo.PerformLayout();
            this.tabResults.ResumeLayout(false);
            this.tabProgress.ResumeLayout(false);
            this.tabProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgess)).EndInit();
            this.tabItinerary.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgItinary)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDatabaseFile;
        private System.Windows.Forms.Button btnOpenDatabase;
        private System.Windows.Forms.GroupBox gbxFrom;
        private System.Windows.Forms.Button btnFromView;
        private System.Windows.Forms.Button btnFromFind;
        private System.Windows.Forms.TextBox txtFromId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFromCoord;
        private System.Windows.Forms.GroupBox gbxTo;
        private System.Windows.Forms.TextBox txtToCoord;
        private System.Windows.Forms.Button btnToView;
        private System.Windows.Forms.Button btnToFind;
        private System.Windows.Forms.TextBox txtToId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabResults;
        private System.Windows.Forms.TabPage tabProgress;
        private System.Windows.Forms.TabPage tabItinerary;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgItinary;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.PictureBox pbProgess;
        private System.Windows.Forms.Timer tmProgress;
        private System.Windows.Forms.CheckBox cbClassicDijkstra;
        private System.Windows.Forms.TextBox txtStats;
        private System.Windows.Forms.CheckBox cbDoubleSearch;
        private System.Windows.Forms.CheckBox cbNoProgressView;
    }
}

