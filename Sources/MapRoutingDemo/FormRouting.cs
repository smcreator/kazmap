using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MapRoutingDemo
{
    public partial class FormRouting : Form
    {
        SqlConnection _backEnd;
        ProgressQuickDraw _progress;

        public FormRouting()
        {
            InitializeComponent();
        }

        private void btnOpenDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                var dbf = new SqlConnection(@"Data Source=localhost\SQLEXPRESS;initial catalog=KazMap;integrated security=True;multipleactiveresultsets=True");
                dbf.Open();

                if (_backEnd != null)
                {
                    _backEnd.Dispose();
                    _backEnd = null;
                }
                _backEnd = dbf;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Exception while opening database : " + ex, "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            // Make sure that everything is active
            foreach (var control in new Control[]{btnFromView,btnFromFind,txtFromId,
                txtFromCoord,btnToView,btnToFind,txtToId,btnRun})
            {
                control.Enabled = true;
            }
        }

        private void btnFromView_Click(object sender, EventArgs e)
        {
            var pos = txtFromCoord.Text;
            if(string.IsNullOrEmpty(pos)) return;
            using (var qv = new FormQuickView(pos)) qv.ShowDialog();
        }

        private void btnFromFind_Click(object sender, EventArgs e)
        {
            using (var search = new FormSearch(_backEnd))
            {
                if(search.ShowDialog(this)!=DialogResult.OK) return;
                txtFromId.Text = search.SelectedElement.Value.ToString();
                txtFromCoord.Text = search.LastCoord;
            }
        }

        private void btnToFind_Click(object sender, EventArgs e)
        {
            using (var search = new FormSearch(_backEnd))
            {
                if (search.ShowDialog(this) != DialogResult.OK) return;
                txtToId.Text = search.SelectedElement.Value.ToString();
                txtToCoord.Text = search.LastCoord;
            }
        }

        private void btnToView_Click(object sender, EventArgs e)
        {
            var pos = txtFromCoord.Text;
            if (string.IsNullOrEmpty(pos)) return;
            using (var qv = new FormQuickView(pos)) qv.ShowDialog();
        }

        delegate void ShortPathCompleteDlg(object r);

        void ShortPathComplete(object r)
        {
            tmProgress.Enabled = false;
            if(_progress!=null)
            {
                string stats;
                pbProgess.Image = _progress.GetCurrent(pbProgess.Width, pbProgess.Height, out stats);
                txtStats.Text = stats;
                _progress = null;
            }
                

            foreach (var control in new Control[]{btnFromView,btnFromFind,txtFromId,
                txtFromCoord,btnToView,btnToFind,txtToId,btnRun,btnOpenDatabase,cbClassicDijkstra,cbDoubleSearch,cbNoProgressView})
            {
                control.Enabled = true;
            }


            if (r is Exception)
            {
                MessageBox.Show(this,r.ToString());
                return;
            }

            dgItinary.DataSource = r;
        }

        void ProcessShortPath(object p)
        {
            try
            {
                var prms = (long[])p;

                var shortRoute = new ShortPath.ShortPath(_backEnd);
                if(prms[2] == 1) shortRoute.UseClassicDijkstra = true;
                if (prms[3] == 1) shortRoute.UseTwoDirection = true;
                var road = shortRoute.FindPath(prms[0], prms[1], prms[4]!=1?_progress:null);
                _progress.DrawSolution(road);

                var itiTable = new DataTable();
                itiTable.Columns.Add("FromID", typeof(long));
                itiTable.Columns.Add("ToID", typeof(long));
                itiTable.Columns.Add("WayID", typeof(long));
                itiTable.Columns.Add("Name", typeof(string));
                itiTable.Columns.Add("Ref", typeof(string));
                itiTable.Columns.Add("Time", typeof(long));
                itiTable.Columns.Add("FromNode", typeof(long));
                itiTable.Columns.Add("FromLon", typeof(double));
                itiTable.Columns.Add("FromLat", typeof(double));
                itiTable.Columns.Add("ToNode", typeof(long));
                itiTable.Columns.Add("ToLon", typeof(double));
                itiTable.Columns.Add("ToLat", typeof(double));
                itiTable.Columns.Add("pos1", typeof(int));
                itiTable.Columns.Add("pos2", typeof(int));
                itiTable.Columns.Add("Distance", typeof(double));
                

                using (var cmd = _backEnd.CreateCommand())
                {
                    cmd.CommandText = @"
select p.weight, 
    w1.wayID, w1.nodeId [from],
    n1.Longitude flon,n1.Latitude flat,
    w2.nodeId [to],
    n2.Longitude tolat,n2.Latitude tolon,
    (select value 
        from WayTags
        inner join tagtypes t on TypeId=t.id 
        where wayID=w1.wayID and t.name='name') streetName,
    (select value 
        from WayTags 
        inner join tagtypes t on TypeId=t.id 
        where wayID=w1.wayID and t.name='ref') reference,
    w1.Position pos1, w2.Position pos2,
    p.Distance
from WayNodes w1
inner join Nodes n1 on w1.nodeID = n1.id
inner join Intersections i1 on i1.nodeId = w1.nodeID
inner join WayNodes w2 on w1.wayID = w2.wayID
inner join Nodes n2 on w2.nodeID = n2.id
inner join Intersections i2 on i2.nodeId = w2.nodeID
inner join path p on p.LeftId = i1.id and p.RightId = i2.id
where i1.id=@source and i2.id=@dest
";
                    var pSrc = cmd.Parameters.Add("@source", DbType.Int64);
                    var pDst = cmd.Parameters.Add("@dest", DbType.Int64);
                    //cmd.Prepare();

                    for (var i = 0; i < (road.Count - 1); ++i)
                    {
                        pSrc.Value = road[i];
                        pDst.Value = road[i + 1];
                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                itiTable.Rows.Add(
                                    road[i],
                                    road[i + 1],
                                    rdr.GetInt64(1),
                                    rdr.IsDBNull(8) ? "No name" : rdr.GetString(8),
                                    rdr.IsDBNull(9) ? "No ref" : rdr.GetString(9),
                                    rdr.GetInt32(0),
                                    rdr.GetInt64(2),
                                    rdr.GetDouble(3),
                                    rdr.GetDouble(4),
                                    rdr.GetInt64(5),
                                    rdr.GetDouble(6),
                                    rdr.GetDouble(7),
                                    rdr.GetInt32(10),
                                    rdr.GetInt32(11),
                                    rdr.GetDouble(12)
                                    );
                            }
                            else
                            {
                                itiTable.Rows.Add(road[i], road[i + 1]);
                            }
                        }

                    }
                }
                Invoke((ShortPathCompleteDlg)ShortPathComplete, itiTable);
            }
            catch (Exception ex)
            {
                Invoke((ShortPathCompleteDlg)ShortPathComplete,(object)ex);
            }
        }



        private void btnRun_Click(object sender, EventArgs e)
        {
            long start, end;
            if (!long.TryParse(txtFromId.Text,out start)) return;
            if (!long.TryParse(txtToId.Text, out end)) return;

            foreach (var control in new Control[]{btnFromView,btnFromFind,txtFromId,
                txtFromCoord,btnToView,btnToFind,txtToId,btnRun,btnOpenDatabase,cbClassicDijkstra,cbDoubleSearch,cbNoProgressView})
            {
                control.Enabled = false;
            }

            _progress = new ProgressQuickDraw();
            tmProgress.Enabled = true;
            System.Threading.ThreadPool.QueueUserWorkItem(ProcessShortPath, new long[] { start, end, 
                cbClassicDijkstra.Checked ? 1 : 0, 
                cbDoubleSearch.Checked ? 1 : 0,
                cbNoProgressView.Checked ? 1 : 0
            });
        }

        private void dgItinary_CurrentCellChanged(object sender, EventArgs e)
        {
            var cell = dgItinary.CurrentCell;
            if(cell==null) return;

            var fLon = (double)dgItinary.CurrentRow.Cells[7].Value;
            var fLat = (double)dgItinary.CurrentRow.Cells[8].Value;
            var tLon = (double)dgItinary.CurrentRow.Cells[10].Value;
            var tLat = (double)dgItinary.CurrentRow.Cells[11].Value;

            var mv = new MapDrawer(fLat, fLon, tLon, tLat);
            mv.Color = "green";
            mv.AppendMark(fLon, fLat);
            mv.Color = "red";
            mv.AppendMark(tLon, tLat);
            mv.Color = "blue";


            using (var point2Point = _backEnd.CreateCommand())
            {
                point2Point.CommandText = @"
select nd.Longitude,nd.Latitude 
from WayNodes
inner join Nodes nd on WayNodes.nodeID = nd.id 
where wayId = @wayid and @top <= Position and Position <= @bottom
";
                var pos1 = (int)dgItinary.CurrentRow.Cells[12].Value;
                var pos2 = (int)dgItinary.CurrentRow.Cells[13].Value;
                point2Point.Parameters.AddWithValue("@wayId", (long)dgItinary.CurrentRow.Cells[2].Value);
                point2Point.Parameters.AddWithValue("@top", Math.Min(pos1,pos2));
                point2Point.Parameters.AddWithValue("@bottom", Math.Max(pos1, pos2));

                using (var rdr = point2Point.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        var pLn = rdr.GetDouble(0);
                        var pLt = rdr.GetDouble(1);
                        while (rdr.Read())
                        {
                            var cLn = rdr.GetDouble(0);
                            var cLt = rdr.GetDouble(1);
                            mv.AppendLine(pLn, pLt, cLn, cLt);
                            pLn = cLn;
                            pLt = cLt;
                        }
                    }
                }
            }

            webBrowser.DocumentText = mv.ActiveHTML;  
        }

        private void tmProgress_Tick(object sender, EventArgs e)
        {
            if (_progress == null) {
                tmProgress.Enabled = false;
                return;
            }
            string stats;
            pbProgess.Image = _progress.GetCurrent(pbProgess.Width, pbProgess.Height,out stats);
            txtStats.Text = stats;
        }

        private void pbProgess_SizeChanged(object sender, EventArgs e)
        {
            if (_progress!=null)
            {
                string stats;
                pbProgess.Image = _progress.GetCurrent(pbProgess.Width, pbProgess.Height, out stats);
                txtStats.Text = stats;
            }
        }

    }
}
