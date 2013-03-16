using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MapRoutingDemo
{
    public partial class FormSearch : Form
    {
        readonly SqlConnection _backend;
        public FormSearch(SqlConnection backend)
        {
            InitializeComponent();

            _backend = backend;
            SelectedElement = null;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var fullText = txtSearchText.Text.Trim();
            if(string.IsNullOrEmpty(fullText)) return;

            var resultTable = new DataTable();
            resultTable.Columns.Add("Text", typeof (string));
            resultTable.Columns.Add("WayID", typeof(long));
            resultTable.Columns.Add("Matrix Pos", typeof(long));
            resultTable.Columns.Add("NodeID", typeof(long));
            resultTable.Columns.Add("Longitude", typeof(double));
            resultTable.Columns.Add("Lattitude", typeof(double));

            using (var cmd = _backend.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT wn.value,wnds.wayid,i.id,i.nodeId,n.Longitude lon,n.Latitude lat
FROM WayNames wn 
inner join WayNamesReferences wnr on wn.NameId = wnr.NameId
inner join WayNodes wnds on wnr.wayid = wnds.wayid
inner join Intersections i on i.nodeId = wnds.nodeId
inner join Nodes n on i.nodeId = n.id
where Value like @fulltext";
                cmd.Parameters.AddWithValue("@fulltext", txtSearchText.Text);
                using (var dta = cmd.ExecuteReader())
                {
                    while (dta.Read())
                    {
                        resultTable.Rows.Add(
                            dta.GetString(0),
                            dta.GetInt64(1),
                            dta.GetInt64(2),
                            dta.GetInt64(3),
                            dta.GetDouble(4),
                            dta.GetDouble(5)
                            );
                    }
                }
            }
            dgResults.DataSource = resultTable;
        }

        public long? SelectedElement { get; private set; }
        public string LastCoord { get; private set; }

        private void dgResults_CurrentCellChanged(object sender, EventArgs e)
        {
            if(dgResults.CurrentRow==null) return;
            var lon = (double) dgResults.CurrentRow.Cells[4].Value;
            var lat = (double) dgResults.CurrentRow.Cells[5].Value;

            LastCoord = string.Format("{0},{1}", lat, lon);

            SelectedElement = (long)dgResults.CurrentRow.Cells[2].Value;


            var mv = new MapDrawer(lon, lat, 16);
            mv.AppendMark(lon, lat);
            webShowPos.DocumentText = mv.ActiveHTML;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(!SelectedElement.HasValue)
            {
                MessageBox.Show(this, "Please select a row first");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
