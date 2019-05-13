using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PositiveDecision
{
    public partial class LoginForm : Form
    {
        private void Login_Load()
        {
            IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "\\settings.ini");
            textBoxLogin.Text = IniFile.IniReadValue("Connection", "Login");
            if (textBoxLogin.Text == "") textBoxLogin.Text = "Operator";
        }
        public LoginForm()
        {
            InitializeComponent();
            Login_Load();

            this.AcceptButton = buttonConnect;
        }

        public string Password
        {
            get
            {
                return textBoxPass.Text;
            }
        }

        private void buttonConnectEdit_Click(object sender, EventArgs e)
        {
            ConnectEdit FRM = new ConnectEdit();
            FRM.ShowDialog();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string ManagersLogin = textBoxLogin.Text;
                string ServerName;
                string Port;
                string Base;
                IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "\\settings.ini");
                ServerName = IniFile.IniReadValue("Connection", "ServerName");
                if (ServerName == "")
                {
                    ServerName = "localhost";
                }
                Port = IniFile.IniReadValue("Connection", "Port");
                if (Port == "")
                {
                    Port = "3306";
                }
                Base = IniFile.IniReadValue("Connection", "Base");
                if (Base == "")
                {
                    Base = "dbcashback";
                }
                string ConnectionString = "server=" + ServerName + ";charset=utf8;User Id=" + ManagersLogin + ";password=" + Password + ";Persist Security Info=True;database=" + Base + ";port=" + Port;
                PDBase.myConnection = new MySqlConnection(ConnectionString);
                PDBase.myConnection.Close();
                PDBase.myConnection.Open();
                MySqlCommand dcmdClient = new MySqlCommand("Select * from manager", PDBase.myConnection);
                dcmdClient.ExecuteNonQuery();

                IniFile.IniWriteValue("Connection", "Login", ManagersLogin);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1045)
                    GMessage.ErrMSG("Доступ запрещён");
                else if (ex.Number == 1044)
                    GMessage.ErrMSG("Доступ запрещён");
                else if (ex.Number == 0)
                    GMessage.ErrMSG("Доступ запрещён");
                else if (ex.Number == 1142)
                    GMessage.ErrMSG("Пользователь отключен");
                else
                    GMessage.ErrMSG(ex);
            }
        }
    }
}
