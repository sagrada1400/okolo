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
    enum rowState3
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class workerform : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;
        public workerform()
        {
            InitializeComponent();
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_worker", "Номер работника");
            dataGridView1.Columns.Add("id_position", "Наименование должности");
            dataGridView1.Columns.Add("id_squad", "Наименование бригады");
            dataGridView1.Columns.Add("full_name", "Ф.И.О. работника");
            dataGridView1.Columns.Add("address", "Адрес работника");
            dataGridView1.Columns.Add("phone_number", "Телефон работника");
            dataGridView1.Columns.Add("date_of_birth", "Дата рождения работника");
        }
        private void ReadSingleRow(DataGridView dgv, IDataRecord record)
        {
            var id_worker = record.GetInt32(0);
            var id_position = record.IsDBNull(1) ? null : (int?)record.GetInt32(1);
            var id_squad = record.GetInt32(2);
            var full_name = record.GetString(3);
            var address = record.GetString(4);
            var phone_number = record.GetString(5);
            var date_of_birth = record.GetDateTime(6);

            var position_name = id_position.HasValue ? GetPositionName(id_position.Value) : string.Empty;
            var squad_name = GetSquadName(id_squad);

            dgv.Rows.Add(id_worker, position_name, squad_name, full_name, address, phone_number, date_of_birth, rowState.ModifiedNew);
        }
        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"SELECT worker.id_worker, worker.id_position, worker.id_squad, worker.full_name, worker.address, worker.phone_number, worker.date_of_birth FROM worker";

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
        private string GetPositionName(int position_id)
        {
            string query = "SELECT name FROM position WHERE id_position = @id_position";
            SqlConnection connection = new SqlConnection(dataBase.getConnection().ConnectionString);
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_position", position_id);
                connection.Open();
                var name = (string)command.ExecuteScalar();
                return name;
            }
        }
        private string GetSquadName(int squad_id)
        {
            string query = "SELECT name FROM squad WHERE id_squad = @id_squad";
            SqlConnection connection = new SqlConnection(dataBase.getConnection().ConnectionString);
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_squad", squad_id);
                connection.Open();
                var name = (string)command.ExecuteScalar();
                return name;
            }
        }

        private void workerform_Load(object sender, EventArgs e)
        {
            UpdateSquadCondition2();
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            Info frmo = new Info();
            frmo.Show();
        }
        private void UpdateSquadCondition2()
        {
            string updateQuery = $"UPDATE squad SET condition = 'Не работает' WHERE id_squad IN (SELECT w.id_squad FROM worker w INNER JOIN sickleave s ON w.id_worker = s.id_worker WHERE s.condition = 'Болен')";
            dataBase.openConnection();
            SqlCommand command = new SqlCommand(updateQuery, dataBase.getConnection());
            command.ExecuteNonQuery();
            dataBase.closeConnection();
        }
        private void UpdateSquadCondition(int workerID)
        {
            // Проверяем значение condition в таблице sickleave
            string sickLeaveQuery = $"SELECT condition FROM sickleave WHERE id_worker = {workerID}";
            dataBase.openConnection();
            SqlCommand sickLeaveCommand = new SqlCommand(sickLeaveQuery, dataBase.getConnection());
            string sickLeaveCondition = sickLeaveCommand.ExecuteScalar()?.ToString();
            dataBase.closeConnection();

            // Если condition равно "Болен", обновляем значение condition в таблице squad
            if (sickLeaveCondition == "Болен")
            {
                string squadUpdateQuery = $"UPDATE squad SET condition = 'Не работает' WHERE id_squad = (SELECT id_squad FROM worker WHERE id_worker = {workerID})";
                dataBase.openConnection();
                SqlCommand squadUpdateCommand = new SqlCommand(squadUpdateQuery, dataBase.getConnection());
                squadUpdateCommand.ExecuteNonQuery();
                dataBase.closeConnection();
            }

            // Получаем данные condition из таблицы squad
            string squadQuery = $"SELECT condition FROM squad WHERE id_squad = (SELECT id_squad FROM worker WHERE id_worker = {workerID})";
            dataBase.openConnection();
            SqlCommand squadCommand = new SqlCommand(squadQuery, dataBase.getConnection());
            string squadCondition = squadCommand.ExecuteScalar()?.ToString();
            dataBase.closeConnection();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("condition");
            dataTable.Rows.Add(squadCondition);

            dataGridView3.DataSource = dataTable;
        }
        private void sposobbb(int workerID)
        {
            string query = $"SELECT sl.condition FROM sickleave sl WHERE sl.id_worker = {workerID}";
            dataBase.openConnection();
            SqlCommand command = new SqlCommand(query, dataBase.getConnection());
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            dataBase.closeConnection();

            dataGridView2.DataSource = dataTable;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                int clientId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id_worker"].Value);
                sposobbb(clientId);
            }
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                int workerID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id_worker"].Value);
                UpdateSquadCondition(workerID);
            }
        }
    }
}
