using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace okolo
{
    enum rowState4
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class equipform : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;
        public equipform()
        {
            InitializeComponent();
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_equipmnet", "Номер оборудования");
            dataGridView1.Columns.Add("name", "Наименование оборудования");
            dataGridView1.Columns.Add("brand", "Марка оборудования");
            dataGridView1.Columns.Add("description", "Описание оборудования");

        }
        private void ReadSingleRow(DataGridView dgv, IDataRecord record)
        {
            var id_equipmnet = record.GetInt32(0);
            var name = record.IsDBNull(1) ? null : record.GetValue(1).ToString();
            var brand = record.IsDBNull(2) ? null : record.GetValue(2).ToString();
            var description = record.IsDBNull(3) ? null : record.GetValue(3).ToString();
            dgv.Rows.Add(id_equipmnet, name, brand, description, rowState.ModifiedNew);
        }
        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"SELECT * FROM [equipment]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void equipform_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
