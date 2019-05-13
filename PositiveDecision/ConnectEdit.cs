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
    public partial class ConnectEdit : Form
    {
        public ConnectEdit()
        {
            InitializeComponent();
            Settings_Load();
            this.AcceptButton = buttonEditeOk;
        }
        private void Settings_Load()
        {
            IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "\\settings.ini");
            textBoxServerName.Text = IniFile.IniReadValue("Connection", "ServerName");
            if (textBoxServerName.Text == "")
            {
                textBoxServerName.Text = "localhost";
            }
            textBoxPort.Text = IniFile.IniReadValue("Connection", "Port");
            if (textBoxPort.Text == "")
            {
                textBoxPort.Text = "3306";
            }
            textBoxDataBaseName.Text = IniFile.IniReadValue("Connection", "Base");
            if (textBoxDataBaseName.Text == "")
            {
                textBoxDataBaseName.Text = "mydbpositivedecision";
            }
        }
        private void buttonEditeOkClick()
        {
            IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "\\settings.ini");
            IniFile.IniWriteValue("Connection", "ServerName", textBoxServerName.Text);
            IniFile.IniWriteValue("Connection", "Port", textBoxPort.Text);
            IniFile.IniWriteValue("Connection", "Base", textBoxDataBaseName.Text);
            Close();
        }

        private void buttonEditeOk_Click(object sender, EventArgs e)
        {
            buttonEditeOkClick();
        }
    }
}
