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
    enum rowState5
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class squadform : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;
        public squadform()
        {
            InitializeComponent();
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_squad", "Номер бригады");
            dataGridView1.Columns.Add("name", "Наименование бригады");
            dataGridView1.Columns.Add("condition", "Состояние бригады");
            dataGridView1.Columns.Add("description", "Описание бригады");
        }
        private void ReadSingleRow(DataGridView dgv, IDataRecord record)
        {
            var id_squad = record.GetInt32(0);
            var name = record.IsDBNull(1) ? null : record.GetValue(1).ToString();
            var condition = record.IsDBNull(2) ? null : record.GetValue(2).ToString();
            var description = record.IsDBNull(3) ? null : Convert.ToString(record.GetValue(3));
            dgv.Rows.Add(id_squad, name, condition, description, rowState.ModifiedNew);
        }
        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"SELECT * FROM [squad]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        private void squadform_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ClearSelection();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from [squad] where ";

            if (checkBox1.Checked)
            {
                searchString += $"(id_squad) like '%{textBox11.Text}%'";
            }
            else if (checkBox2.Checked)
            {
                searchString += $"(name) like '%{textBox11.Text}%'";
            }
            else if (checkBox3.Checked)
            {
                searchString += $"(condition) like '%{textBox11.Text}%'";
            }
            else if (checkBox4.Checked)
            {
                searchString += $"(description) like '%{textBox11.Text}%'";
            }
            else
            {
                searchString += $"concat(id_squad, name, condition, description) like '%{textBox11.Text}%'";
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

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox11.Clear();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();

            var id_squad = textBox1.Text;
            var name = textBox2.Text;
            var condition = textBox3.Text;
            string description = "";

            if (!string.IsNullOrEmpty(textBox4.Text))
            {
                description = textBox4.Text;
                var addQuery = $"insert into [squad] (id_squad, name, condition, description) values ('{id_squad}','{name}','{condition}','{description}')";

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

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox4.Clear();
            textBox3.Clear();
            textBox2.Clear();
            dataGridView1.ClearSelection();
        }
        private void Change()
        {
            {
                dataBase.openConnection();
                int index = dataGridView1.CurrentCell.RowIndex;
                var id_squad = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                var selectRowIndex = dataGridView1.CurrentCell.RowIndex;
                var name = textBox2.Text;
                var condition = textBox3.Text;
                var description = textBox4.Text;

                if (dataGridView1.Rows[selectRowIndex].Cells[0].Value.ToString() != string.Empty)
                {

                    dataGridView1.Rows[selectRowIndex].SetValues(id_squad, name, condition, description);
                    //dataGridView1.Rows[selectRowIndex].Cells[4].Value = rowState.Modified;
                    var changeQuery = $"update [squad] set name = '{name}', condition ='{condition}', description ='{description}' where id_squad ='{id_squad}'";
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
                            var deleteQuery = $"delete from [squad] where id_squad = '{id}'";
                            var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                            command.ExecuteNonQuery();
                        }
                    }
                    if (RowState == rowState.Modified)
                    {
                        var id_squad = dataGridView1.Rows[index].Cells[0].Value.ToString();
                        var name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                        var condition = dataGridView1.Rows[index].Cells[2].Value.ToString();
                        var description = dataGridView1.Rows[index].Cells[3].Value.ToString();


                        var changeQuery = $"update [squad] set name = '{name}', condition = '{condition}', description = '{description}' where id_ssquad = '{id_squad}'";

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
    }
}
