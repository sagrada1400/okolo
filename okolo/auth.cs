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
    public partial class auth : Form
    {
        DataBase dataBase = new DataBase();
        public auth()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var loginUser = textBox1.Text;
            var passwrodUser = textBox2.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string querystring = $"select id_auth, login_user, password_user from [user] where login_user = '{loginUser}' and password_user ='{passwrodUser}'";

            SqlCommand command = new SqlCommand(querystring, dataBase.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {

                string cmd = $"SELECT * from position";
                dataBase.openConnection();
                SqlCommand sqlCommand = new SqlCommand(cmd, dataBase.getConnection());
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Position.List.Add(new Position()
                    {
                        id_position = reader.GetInt32(0),
                        name = reader.GetString(1),
                        salary = reader.GetInt32(2),
                        decription = reader.GetString(3)
                    });
                }
                reader.Close();
                MessageBox.Show("Вы успешно вошли!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ServiceMain frma = new ServiceMain();
                frma.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Такого аккаунта не существует!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void auth_Load(object sender, EventArgs e)
        {

        }
    }
}

