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
using System.Globalization;

namespace okolo
{
    enum rowState2
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class clientform : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;
        public clientform()
        {
            InitializeComponent();
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_client", "Номер клиента");
            dataGridView1.Columns.Add("full_name", "Ф.И.О. клиента");
            dataGridView1.Columns.Add("address", "Адрес клиента");
            dataGridView1.Columns.Add("phone_number", "Номер телефона клиента");
            dataGridView1.Columns.Add("date_of_birth", "Дата рождения клиента");
        }
        private void ReadSingleRow(DataGridView dgv, IDataRecord record)
        {
            var id_client = record.GetInt32(0);
            var full_name = record.GetString(1);
            var address = record.GetString(2);
            var phone_number = record.GetString(3);
            var date_of_birth = record.GetDateTime(4);
            dgv.Rows.Add(id_client, full_name, address, phone_number, date_of_birth, rowState.ModifiedNew);
        }
        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"SELECT * FROM [client]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        private void sposobbb(int clientID)
        {
            string query = $"select * from [order] where id_client = {clientID}";
            dataBase.openConnection();
            SqlCommand command = new SqlCommand(query, dataBase.getConnection());
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            DataTable dataTable2 = new DataTable();
            var name = new DataColumn();
            name.DataType = typeof(string);
            name.ColumnName = "name";
            dataTable2.Columns.Add(name);
            var id_order = new DataColumn();
            id_order.DataType = typeof(int);
            id_order.ColumnName = "id_order";
            dataTable2.Columns.Add(id_order);

            foreach (DataRow row in dataTable.Rows)
            {
                string query2 = $"select name from service inner join [order] on [order].id_service = [service].id_service where [order].id_order = {row["id_order"]}";
                SqlCommand command2 = new SqlCommand(query2, dataBase.getConnection());
                SqlDataReader reader2 = command2.ExecuteReader();
                while (reader2.Read())
                {

                    DataRow newRow = dataTable2.NewRow();
                    newRow["name"] = reader2.GetString(0);
                    dataTable2.Rows.Add(newRow);
                }
                reader2.Close();
            }
            dataGridView2.DataSource = dataTable2.DefaultView.ToTable(false, "name");
            dataBase.closeConnection();
        }
        private void clientform_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from [client] where ";

            if (checkBox1.Checked)
            {
                searchString += $"(id_client) like '%{SearchBox.Text}%'";
            }
            else if (checkBox2.Checked)
            {
                searchString += $"(full_name) like '%{SearchBox.Text}%'";
            }
            else if (checkBox3.Checked)
            {
                searchString += $"(address) like '%{SearchBox.Text}%'";
            }
            else if (checkBox4.Checked)
            {
                searchString += $"(phone_number) like '%{SearchBox.Text}%'";
            }
            else if (checkBox5.Checked)
            {
                searchString += $"(date_of_birth) like '%{SearchBox.Text}%'";
            }
            else
            {
                searchString += $"concat(id_client, full_name, address, phone_number, date_of_birth) like '%{SearchBox.Text}%'";
            }

            SqlCommand com = new SqlCommand(searchString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
        }
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dataGridView1.CurrentCell.RowIndex;

            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow roww = dataGridView1.Rows[selectedRow];

                textBox1.Text = roww.Cells[0].Value.ToString();
                textBox2.Text = roww.Cells[1].Value.ToString();
                textBox3.Text = roww.Cells[2].Value.ToString();
                textBox4.Text = roww.Cells[3].Value.ToString();
                dateTimePicker1.Text = roww.Cells[4].Value.ToString();

            }
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                int clientId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id_client"].Value);
                sposobbb(clientId);
            }
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            Info frmmm = new Info();
            frmmm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            //dateTimePicker1.Hide();
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();
            //

        }
        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[index].Cells[4].Value = rowState.Deleted;
                return;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();

            var id_client = textBox1.Text;
            var full_name = textBox2.Text;
            var address = textBox3.Text;
            var phone_number = textBox4.Text;

            DateTime date_of_birth;

            if (DateTime.TryParse(dateTimePicker1.Text, out date_of_birth))
            {
                var addQuery = $"insert into [client] (id_client, full_name, address, phone_number, date_of_birth) values ('{id_client}','{full_name}','{address}','{phone_number}','{date_of_birth}')";

                var command = new SqlCommand(addQuery, dataBase.getConnection());
                command.ExecuteNonQuery();

                MessageBox.Show("Запись успешно создана!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Вы не заполнили данные!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            dataBase.closeConnection();
        }
        private void Change()
        {
            {
                dataBase.openConnection();
                int index = dataGridView1.CurrentCell.RowIndex;
                var id_client = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                var full_name = textBox2.Text;
                var address = textBox3.Text;
                var phone_number = textBox4.Text;
                var date_of_birth = dateTimePicker1.Value;

                if (dataGridView1.Rows[index].Cells[0].Value != null)
                {
                    dataGridView1.Rows[index].SetValues(id_client, full_name, address, phone_number, date_of_birth);

                    var changeQuery = "UPDATE [client] SET full_name = @full_name, address = @address, phone_number = @phone_number, date_of_birth = @date_of_birth WHERE id_client = @id_client";
                    var command = new SqlCommand(changeQuery, dataBase.getConnection());
                    command.Parameters.AddWithValue("@id_client", id_client);
                    command.Parameters.AddWithValue("@full_name", full_name);
                    command.Parameters.AddWithValue("@address", address);
                    command.Parameters.AddWithValue("@phone_number", phone_number);
                    command.Parameters.AddWithValue("@date_of_birth", date_of_birth);

                    command.ExecuteNonQuery();
                }
                dataBase.closeConnection();
            }
        }
            private void button2_Click(object sender, EventArgs e)
        {
            Change();
        }
        private void update()
        {
            dataBase.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count - 1; index++)
            {
                if (dataGridView1.Rows[index].Cells[4].Value is rowState RowState)
                {
                    if (RowState == rowState.Existed)
                        continue;
                    if (RowState == rowState.Deleted)
                    {
                        if (int.TryParse(dataGridView1.Rows[index].Cells[0].Value.ToString(), out int id))
                        {
                            var deleteQuery = $"delete from [client] where id_client = '{id}'";
                            var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                            command.ExecuteNonQuery();
                        }
                    }
                    if (RowState == rowState.Modified)
                    {
                        var id_client = dataGridView1.Rows[index].Cells[0].Value.ToString();
                        var full_name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                        var address = dataGridView1.Rows[index].Cells[2].Value.ToString();
                        var phone_number = dataGridView1.Rows[index].Cells[3].Value.ToString();
                        var date_of_birth = dataGridView1.Rows[index].Cells[4].Value.ToString();


                        var changeQuery = $"update [client] set full_name = '{full_name}', address ='{address}', phone_number ='{phone_number}', date_of_birth = '{date_of_birth}' where id_client ='{id_client}'";

                        var command = new SqlCommand(changeQuery, dataBase.getConnection());
                        command.ExecuteNonQuery();
                    }
                }



            }
            dataBase.closeConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            update();
            MessageBox.Show("Данные успешно сохранены!");
        }
        private void RefreshDataGrid3(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"SELECT * FROM [client]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        private void RefreshBox_Click(object sender, EventArgs e)
        {
            RefreshDataGrid3(dataGridView1);
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            workerform frmmm = new workerform();
            frmmm.Show();
            this.Hide();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    } }
