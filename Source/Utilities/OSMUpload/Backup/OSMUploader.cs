using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace OSMUpload
{
    public partial class OSMUploader : Form
    {
        readonly Bitmap _world;
        double _top, _left, _right, _bottom;
        readonly object _lock = new object();

        public OSMUploader()
        {
            InitializeComponent();

            // ReSharper disable AssignNullToNotNullAttribute
            _world = new Bitmap(
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OSMUpload.world.jpg")
            );
            // ReSharper restore AssignNullToNotNullAttribute
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            var fdlg = new OpenFileDialog
                           {
                               AddExtension = false,
                               DefaultExt = "osm.bz2",
                               Filter = "Compressed OSM (*.osm.bz2)|*.osm.bz2",
                               CheckFileExists = true
                           };
            if(fdlg.ShowDialog(this)!=System.Windows.Forms.DialogResult.OK) return;
            txtSourceFile.Text = fdlg.FileName;
        }

        private void btnBrowseTarget_Click(object sender, EventArgs e)
        {
            var fdlg = new OpenFileDialog
            {
                AddExtension = false,
                DefaultExt = "db",
                Filter = "SQLite database (*.db)|*.db",
                CheckFileExists = false
            };
            if (fdlg.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            txtTargetFile.Text = fdlg.FileName;
        }

        private delegate void ProcessCompletedDlg(object result);
        private delegate void UpdateCountersDlg(long totalNodes,long totalWays,long totalRels,int permil);
        private delegate void UpdateStatusTxtDlg(string text);
        private delegate void UpdateMapDlg(double top, double left, double bottom, double right);

        void ProcessCompleted(object result)
        {
            if(result is Exception)
            {
                MessageBox.Show(this, "Failed : " + result.ToString(), "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                tbStatus.Text = "Process failed !";
            }
            else
            {
                tbStatus.Text = "Process completed ...";
            }
            txtSourceFile.Enabled = true;
            txtTargetFile.Enabled = true;
            btnBrowseSource.Enabled = true;
            btnBrowseTarget.Enabled = true;
            btnRun.Enabled = true;
            cbCreateRouting.Enabled = true;
            btnBuildMatrix.Enabled = true;
        }

        void UpdateCounters(long totalNodes, long totalWays, long totalRels, int permil)
        {
            txtNbNodes.Text = totalNodes.ToString();
            txtNbWays.Text = totalWays.ToString();
            txtNbRels.Text = totalRels.ToString();
            tbCurrentProgress.Value = permil;
        }

        void RefreshMap()
        {
            var yTop = ((180 - (_top + 90)) * _world.Height)/180;
            var yBottom = ((180 - (_bottom + 90)) * _world.Height) / 180;
            var xLeft = ((180 + _left) * _world.Width)/360;
            var xRight = ((180 + _right) * _world.Width) / 360;

            var pts = new Point();

            var pbX = pbxMap.Width-10;
            var pbY = pbxMap.Height-10;

            pts.X = (xRight - xLeft) < pbX
                        ? Math.Max(0, (int)(xLeft - (pbX - (xRight - xLeft)) / 2))
                        : Math.Max(0, (int) xLeft - 5);
            pts.Y = (yBottom - yTop) < pbY
                        ? Math.Max(0, (int)(yTop - (pbY - (yBottom - yTop)) / 2))
                        : Math.Max(0, (int) yTop - 5);

            var sze = new Size();

            sze.Width = Math.Min(_world.Width - pts.X, Math.Max(pbX, (int)(xRight - xLeft) + 10));
            sze.Height = Math.Min(_world.Height - pts.Y, Math.Max(pbY, (int)(yBottom - yTop) + 10));

            var pic = _world.Clone(new Rectangle(pts,sze), _world.PixelFormat);
            using(var fx=Graphics.FromImage(pic))
            {
                fx.DrawRectangle(Pens.Red, (int)xLeft - pts.X, (int)yTop - pts.Y, (int)(xRight - xLeft), (int)(yBottom - yTop));
            }
            pbxMap.Image = pic;

        }

        void UpdateMap(double top, double left, double bottom, double right)
        {
            lock (_lock)
            {
                _top = top;
                _left = left;
                _right = right;
                _bottom = bottom;
            }
            RefreshMap();
        }


        private void ProgressNotify(OSMReader process)
        {
            Invoke((UpdateCountersDlg) UpdateCounters, process.TotalNodes, 
                process.TotalWays, process.TotalRelations, process.PermilCompletion);

            bool refreshMap;
            lock(_lock)
            {
                refreshMap = (_top != process.Top || _left != process.Left ||
                              _bottom != process.Bottom || _right != process.Right);
            }
            if (refreshMap)
            {
                Invoke((UpdateMapDlg)UpdateMap, process.Top,
                    process.Left, process.Bottom, process.Right);
                
            }
        }

        void UpdateStatusTxt(string text)
        {
            tbStatus.Text = text;
        }

        void NotifyProgress(string text)
        {
            Invoke((UpdateStatusTxtDlg) UpdateStatusTxt, text);
        }

        private void BackgroundRun(object @params)
        {
            var files = (string[]) @params;
            try
            {
                using (var dbf = new SQLiteConnection(
                    (new SQLiteConnectionStringBuilder
                    {
                        DataSource = files[1],
                        SyncMode = SynchronizationModes.Off,
                    }).ConnectionString
                    ))
                {
                    dbf.Open();
                    if(files[0]!=null)
                    {
                        NotifyProgress("Uploading OSM file ...");
                        OSMReader.RunUpload(dbf, files[0], ProgressNotify);
                    }
                    if(files[2]!=null)
                    {
                        NotifyProgress("Building routing matrix ...");
                        MatrixBuilder.Build(dbf, NotifyProgress); 
                    }
                }
                Invoke((ProcessCompletedDlg)ProcessCompleted, true);
            }
            catch (Exception ex)
            {
                Invoke((ProcessCompletedDlg) ProcessCompleted, ex);
            }
        }



        private void btnRun_Click(object sender, EventArgs e)
        {
            var sourceFile = txtSourceFile.Text.Trim();
            if (!System.IO.File.Exists(sourceFile))
            {
                MessageBox.Show(this, "Source file doesn't exist");
                txtSourceFile.Focus();
                return;
            }
            var destFile = txtTargetFile.Text.Trim();
            if (System.IO.File.Exists(destFile))
            {
                if (MessageBox.Show(this,
                                    "A target database file already exists. If you continue, the file will be regenerated",
                                    "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                System.IO.File.Delete(destFile);
            }
            else
            {
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(destFile)))
                {
                    MessageBox.Show(this, "Destination file directory doesn't exist");
                    txtSourceFile.Focus();
                    return;
                }
            }

            // - Ok, we are all go now
            txtSourceFile.Enabled = false;
            txtTargetFile.Enabled = false;
            btnBrowseSource.Enabled = false;
            btnBrowseTarget.Enabled = false;
            btnRun.Enabled = false;
            cbCreateRouting.Enabled = false;
            btnBuildMatrix.Enabled = false;

            tbCurrentProgress.Maximum = 1000;
            tbCurrentProgress.Minimum = 0;
            tbCurrentProgress.Value = 0;
            tbStatus.Text = "Start processing ...";

            System.Threading.ThreadPool.QueueUserWorkItem(BackgroundRun,
                                                          new[]{sourceFile, destFile, cbCreateRouting.Checked ? "":null});
        }

        private void pbxMap_SizeChanged(object sender, EventArgs e)
        {
            if(btnRun.Enabled) return;
            RefreshMap();
        }

        private void btnBuildMatrix_Click(object sender, EventArgs e)
        {
            var sourceFile = txtTargetFile.Text.Trim();
            if (!System.IO.File.Exists(sourceFile))
            {
                MessageBox.Show(this, "Source database file doesn't exist");
                txtSourceFile.Focus();
                return;
            }

            // - Ok, we are all go now
            txtSourceFile.Enabled = false;
            txtTargetFile.Enabled = false;
            btnBrowseSource.Enabled = false;
            btnBrowseTarget.Enabled = false;
            btnRun.Enabled = false;
            cbCreateRouting.Enabled = false;
            btnBuildMatrix.Enabled = false;

            tbCurrentProgress.Maximum = 1000;
            tbCurrentProgress.Minimum = 0;
            tbCurrentProgress.Value = 0;
            tbStatus.Text = "Start processing ...";

            System.Threading.ThreadPool.QueueUserWorkItem(BackgroundRun,
                                                          new[] { null, sourceFile, "" });

        }
    }
}
