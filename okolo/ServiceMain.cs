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
    enum rowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class ServiceMain : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;
        public ServiceMain()
        {
            InitializeComponent();
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_service", "Номер услуги");
            dataGridView1.Columns.Add("name", "Наименование услуги");
            dataGridView1.Columns.Add("salary", "Стоимость услуги");
            dataGridView1.Columns.Add("description", "Описание услуги");
        }
        private void CreateColumns2()
        {
            dataGridView2.Columns.Add("id_order", "Номер заказа");
            dataGridView2.Columns.Add("service_name", "Услуга");
            dataGridView2.Columns.Add("squad_name", "Бригада");
            dataGridView2.Columns.Add("client_name", "Клиент");
            dataGridView2.Columns.Add("equipment_name", "Оборудование");
            dataGridView2.Columns.Add("date_of_order", "Дата заказа");
            dataGridView2.Columns.Add("date_of_work", "Дата выполнения");
            dataGridView2.Columns.Add("condition", "Состояние");
        }

        private string GetServiceName(int service_id)
        {
            string query = "SELECT name FROM service WHERE id_service = @id_service";
            SqlConnection connection = new SqlConnection(new DataBase().getConnection().ConnectionString);
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_service", service_id);
                connection.Open();
                var name = (string)command.ExecuteScalar();
                return name;
            }
        }

        private string GetSquadName(int squad_id)
        {
            string query = "SELECT name FROM squad WHERE id_squad = @id_squad";
            SqlConnection connection = new SqlConnection(new DataBase().getConnection().ConnectionString);
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_squad", squad_id);
                connection.Open();
                var name = (string)command.ExecuteScalar();
                return name;
            }
        }

        private string GetClientName(int client_id)
        {
            string query = "SELECT full_name FROM client WHERE id_client = @id_client";
            SqlConnection connection = new SqlConnection(new DataBase().getConnection().ConnectionString);
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_client", client_id);
                connection.Open();
                var full_name = (string)command.ExecuteScalar();
                return full_name;
            }
        }

        private string GetEquipmentName(int equipment_id)
        {
            string query = "SELECT name FROM equipment WHERE id_equipmnet = @id_equipmnet";
            SqlConnection connection = new SqlConnection(new DataBase().getConnection().ConnectionString);
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_equipmnet", equipment_id);
                connection.Open();
                var name = (string)command.ExecuteScalar();
                return name;
            }
        }

        private void ReadSingleRow2(DataGridView dgv, IDataRecord record)
        {
            var id_order = record.GetInt32(0);
            var id_service = record.GetInt32(1);
            var id_squad = record.GetInt32(2);
            var id_client = record.GetInt32(3);
            var id_equipment = record.GetInt32(4);
            var date_of_order = record.GetDateTime(5);
            var date_of_work = record.GetDateTime(6);
            var condition = record.GetString(7);

            var service_name = GetServiceName(id_service);
            var squad_name = GetSquadName(id_squad);
            var client_name = GetClientName(id_client);
            var equipment_name = GetEquipmentName(id_equipment);

            dgv.Rows.Add(id_order, service_name, squad_name, client_name, equipment_name, date_of_order, date_of_work, condition, rowState.ModifiedNew);
        }
        private void ReadSingleRow(DataGridView dgv, IDataRecord record)
        {
            var id_service = record.GetInt32(0);
            var name = record.IsDBNull(1) ? null : record.GetValue(1).ToString();
            var salary = record.IsDBNull(2) ? null : record.GetValue(2).ToString();
            var description = record.IsDBNull(3) ? null : Convert.ToString(record.GetValue(3));
            dgv.Rows.Add(id_service, name, salary, description, rowState.ModifiedNew);
        }
        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"SELECT * FROM [service]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        private void RefreshDataGrid2(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"SELECT * FROM [order]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow2(dgw, reader);
            }
            reader.Close();
        }
        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void справкаToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void справкаToolStripMenuItem_Click_2(object sender, EventArgs e)
        {
            Info frmo = new Info();
            frmo.Show();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void ServiceMain_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            CreateColumns2();
            RefreshDataGrid2(dataGridView2);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();

            tabPage3.BackColor = Color.Snow;
        }

        private void RefreshBox_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
            RefreshDataGrid2(dataGridView2);
        }
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from [service] where ";

            if (checkBox1.Checked)
            {
                searchString += $"(id_service) like '%{textBox11.Text}%'";
            }
            else if (checkBox2.Checked)
            {
                searchString += $"(name) like '%{textBox11.Text}%'";
            }
            else if (checkBox3.Checked)
            {
                searchString += $"(salary) like '%{textBox11.Text}%'";
            }
            else if (checkBox4.Checked)
            {
                searchString += $"(description) like '%{textBox11.Text}%'";
            }
            else
            {
                searchString += $"concat(id_service, name, salary, description) like '%{textBox11.Text}%'";
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
        private void Search2(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from [order] where ";

            if (checkBox8.Checked)
            {
                searchString += $"(id_order) like '%{SearchBox.Text}%'";
            }
            else if (checkBox7.Checked)
            {
                searchString += $"(id_service) like '%{SearchBox.Text}%'";
            }
            else if (checkBox6.Checked)
            {
                searchString += $"(id_squad) like '%{SearchBox.Text}%'";
            }
            else if (checkBox5.Checked)
            {
                searchString += $"(id_client) like '%{SearchBox.Text}%'";
            }
            else if (checkBox9.Checked)
            {
                searchString += $"(id_equipment) like '%{SearchBox.Text}%'";
            }
            else if (checkBox10.Checked)
            {
                searchString += $"(date_of_order) like '%{SearchBox.Text}%'";
            }
            else if (checkBox11.Checked)
            {
                searchString += $"(date_of_work) like '%{SearchBox.Text}%'";
            }
            else if (checkBox12.Checked)
            {
                searchString += $"(condition) like '%{SearchBox.Text}%'";
            }
            else
            {
                searchString += $"concat(id_order, id_service, id_squad, id_client, id_equipment, date_of_order, date_of_work, condition) like '%{SearchBox.Text}%'";
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
            Search2(dataGridView2);
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

            }
        }

        private void DeleteBox_Click(object sender, EventArgs e)
        {
            SearchBox.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            dataGridView1.ClearSelection();
        }
        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[index].Cells[3].Value = rowState.Deleted;
                return;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            deleteRow();
        }
        private void update()
        {
            dataBase.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count - 1; index++)
            {
                if (dataGridView1.Rows[index].Cells[3].Value is rowState RowState)
                {
                    if (RowState == rowState.Existed)
                        continue;
                    if (RowState == rowState.Deleted)
                    {
                        if (int.TryParse(dataGridView1.Rows[index].Cells[0].Value.ToString(), out int id))
                        {
                            var deleteQuery = $"delete from [service] where id_service = '{id}'";
                            var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                            command.ExecuteNonQuery();
                        }
                    }
                    if (RowState == rowState.Modified)
                    {
                        var id_service = dataGridView1.Rows[index].Cells[0].Value.ToString();
                        var name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                        var salary = dataGridView1.Rows[index].Cells[2].Value.ToString();
                        var description = dataGridView1.Rows[index].Cells[3].Value.ToString();
                       

                        var changeQuery = $"update [service] set name = '{name}', salary = '{salary}', description = '{description}' where id_service = '{id_service}'";

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
        private void Change()
        {
            {
                dataBase.openConnection();
                int index = dataGridView1.CurrentCell.RowIndex;
                var id_service = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                var selectRowIndex = dataGridView1.CurrentCell.RowIndex;
                var name = textBox2.Text;
                var salary = textBox3.Text;
                var description = textBox4.Text;

                if (dataGridView1.Rows[selectRowIndex].Cells[0].Value.ToString() != string.Empty)
                {

                    dataGridView1.Rows[selectRowIndex].SetValues(id_service, name, salary, description);
                    //dataGridView1.Rows[selectRowIndex].Cells[4].Value = rowState.Modified;
                    var changeQuery = $"update [service] set name = '{name}', salary ='{salary}', description ='{description}' where id_service ='{id_service}'";
                    var command = new SqlCommand(changeQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();

                }
                dataBase.closeConnection();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void сохранитьВсёToolStripMenuItem_Click(object sender, EventArgs e)
        {
            update();
            MessageBox.Show("Данные успешно сохранены!");
        }

        private void сменитьПользователяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            auth frm = new auth();
            frm.Show();
            this.Hide();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            clientform frmm = new clientform();
            frmm.Show();
            this.Hide();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dataGridView2.CurrentCell.RowIndex;
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow roww = dataGridView2.Rows[selectedRow];

                textBox6.Text = roww.Cells[0].Value.ToString();
                textBox7.Text = roww.Cells[1].Value.ToString();
                textBox8.Text = roww.Cells[2].Value.ToString();
                textBox5.Text = roww.Cells[3].Value.ToString();
                textBox9.Text = roww.Cells[4].Value.ToString();
                dateTimePicker1.Text = roww.Cells[5].Value.ToString();
                dateTimePicker2.Text = roww.Cells[6].Value.ToString();
                textBox10.Text = roww.Cells[7].Value.ToString();

            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            workerform frmmmm = new workerform();
            frmmmm.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();

            var id_service = textBox1.Text;
            var name = textBox2.Text;
            var salary = textBox3.Text;
            string description = "";

            if (!string.IsNullOrEmpty(textBox4.Text))
            {
                description = textBox4.Text;
                var addQuery = $"insert into [service] (id_service, name, salary, description) values ('{id_service}','{name}','{salary}','{description}')";

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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
            RefreshDataGrid2(dataGridView2);
            textBox11.Clear();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            equipform frrr = new equipform();
            frrr.Show();
            this.Hide();
        }

        //private void услугиToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    var helper = new WordHelper("Flex.docx");
        //    var items = new Dictionary<string, string>
        //        {
        //        { "<ID>",textBox1.Text },
        //        { "<NM>",textBox2.Text },
        //        { "<SR>",textBox3.Text },
        //        { "<DNR>",textBox4.Text },
        //        };

        //    helper.Process(items);
        //    otchetopen frm_sign = new otchetopen();
        //    frm_sign.Show();
        //}

        private void cОтчетамиПоУслугамToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\Users\User\Desktop\okolo\okolo\bin\Debug"; // путь к папке, которую нужно открыть
            System.Diagnostics.Process.Start(folderPath);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox11.Clear();
        }

        private void бригадаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            squadform frmmmmm = new squadform();
            frmmmmm.Show();
            this.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1 fr3 = new Form1();
            fr3.Show();
            this.Hide();
        }
    }
    }
