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
    public partial class Operator : Form
    {
        public void VisibleUI(bool visible)
        {
            panelMain.Visible = visible;
        }
        public Int32 idClients
        {
            get
            {
                if (gridView1.GetSelectedRows().Length > 0)
                {
                    try
                    {
                        return (Int32)gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "idClients");
                    }
                    catch
                    {
                        return 0;
                    }
                }
                else
                    return 0;
            }
        }

        ManagerClass Manager = new ManagerClass();

        public Operator()
        {
            InitializeComponent();
            LoginForm FRM = new LoginForm();
            FRM.ShowDialog();
            if(FRM.DialogResult == DialogResult.OK)
            {
                VisibleUI(true);
                LoadManager();
                LoadClient();
            }
            else
            {
                VisibleUI(false);
            }
        }
        private void LoadManager()
        {
            MySqlCommand dcmdManager = new MySqlCommand("", PDBase.myConnection);
            MySqlDataReader reader = null;
            try
            {
                dcmdManager.CommandText = "select m.idManager,ManagerLogin,ManagerName,ManagerRule from manager as m where m.ManagerLogin=SUBSTRING_INDEX(USER(),'@',1)";
                dcmdManager.ExecuteNonQuery();
                reader = dcmdManager.ExecuteReader();
                object oidManagers = DBNull.Value;
                while (reader.Read())
                {
                    oidManagers = reader[0];
                    if (reader[1] != DBNull.Value) Manager.ManagerLogin = reader[1].ToString();
                    if (reader[2] != DBNull.Value) Manager.ManagerName = reader[2].ToString();
                    if (reader[3] != DBNull.Value) Manager.ManagerRule = reader[3].ToString();
                }
                reader.Close();
                this.Text = Manager.ManagerLogin + " (Оператор " + Manager.ManagerName + " " + Manager.ManagerRule + ")";
                if (oidManagers != DBNull.Value)
                {
                    Manager.ManagerId = Convert.ToInt32(oidManagers);
                }
                if (Manager.ManagerRule == "root" || Manager.ManagerRule == "lawyer")
                {
                    //VisibleInterfaceButtonDelDoc(true);
                }
                else
                {
                    //VisibleInterfaceButtonDelDoc(false);
                }
                if (Manager.ManagerRule == "root")
                {
                    linkLabelAdmin.Enabled = true;
                    comboBoxManager.Enabled = true;
                }
                else
                {
                    linkLabelAdmin.Enabled = false;
                    comboBoxManager.Enabled = false;                   
                }
            }
            catch (Exception ex)
            {
                GMessage.ErrMSG(ex);                
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                PDBase.myConnection.Close();
            }
        }

        void SetRadioGroupStatus(int Status) //выбор цвета(статуса) клиента и фильтр по этому признаку
        {
            switch (Status)
            {
                case 1:
                    {
                        radioGroup1.SelectedIndex = 0;
                        break;
                    }
                case 2:
                    {
                        radioGroup1.SelectedIndex = 1;
                        break;
                    }
                case 3:
                    {
                        radioGroup1.SelectedIndex = 2;
                        break;
                    }
                case 4:
                    {
                        radioGroup1.SelectedIndex = 3;
                        break;
                    }
                case 5:
                    {
                        radioGroup1.SelectedIndex = 4;
                        break;
                    }
                case 6:
                    {
                        radioGroup1.SelectedIndex = 5;
                        break;
                    }
                case 7:
                    {
                        radioGroup1.SelectedIndex = 6;
                        break;
                    }
                case 8:
                    {
                        radioGroup1.SelectedIndex = 7;
                        break;
                    }
                case 9:
                    {
                        radioGroup1.SelectedIndex = 8;
                        break;
                    }
                default:
                    {
                        radioGroup1.SelectedIndex = -1;
                        break;
                    }
            }
        }

        int GetRadioGroupStatus()
        {
            switch (radioGroup1.SelectedIndex)
            {
                case 0:
                    {
                        return 1;
                    }
                case 1:
                    {
                        return 2;
                    }
                case 2:
                    {
                        return 3;
                    }
                case 3:
                    {
                        return 4;
                    }
                case 4:
                    {
                        return 5;
                    }

                case 5:
                    {
                        return 6;
                    }
                case 6:
                    {
                        return 7;
                    }
                case 7:
                    {
                        return 8;
                    }
                case 8:
                    {
                        return 9;
                    }
                default:
                    {
                        return -1;
                    }
            }
        }

        private void LoadClient()
        {
            DataTable ClientsData = new DataTable();
            if (PDBase.myConnection.State == ConnectionState.Closed)
            {
                PDBase.myConnection.Open();
            }
            MySqlCommand cmd = new MySqlCommand("", PDBase.myConnection);
            
            if (Manager.ManagerRule == "root")
            {
                cmd.CommandText = "SELECT idClients,ClientsFIO,ClientsTel,Manager_idManager,ClientsIndexSMS,ManagerName FROM clients INNER JOIN manager on (clients.Manager_idManager = manager.idManager)";                
            }
            else
            {
                cmd.Parameters.Add("@idManager", MySqlDbType.Int32);
                cmd.Parameters[0].Value = Manager.ManagerId;
                cmd.CommandText = "SELECT idClients,ClientsFIO,ClientsTel,Manager_idManager,ClientsIndexSMS,ManagerName FROM clients INNER JOIN manager on (clients.Manager_idManager = manager.idManagers) WHERE Manager_idManager=@idManager ";
                gridView1.Columns["ManagerName"].Visible = false;
            }
            cmd.ExecuteNonQuery();
            MySqlDataReader ReadClients = cmd.ExecuteReader();
            ClientsData.Load(ReadClients);
            bindingSource1.DataSource = null;
            bindingSource1.DataSource = ClientsData;
            ReadClients.Close();

            MySqlDataAdapter daSt = new MySqlDataAdapter("SELECT * FROM status", PDBase.myConnection);
            DataSet dsSt = new DataSet();
            daSt.Fill(dsSt);
            comboBoxStatusClient.DataSource = dsSt.Tables[0];
            comboBoxStatusClient.DisplayMember = "Status";
            comboBoxStatusClient.ValueMember = "statusId";

            //MySqlDataAdapter daReg = new MySqlDataAdapter("SELECT * FROM region", PDBase.myConnection);
            //DataSet dsReg = new DataSet();
            //daReg.Fill(dsReg);
            //comboBoxReg.DataSource = dsReg.Tables[0];
            //comboBoxReg.DisplayMember = "RegionName";
            //comboBoxReg.ValueMember = "idRegion";

            MySqlDataAdapter daMan = new MySqlDataAdapter("SELECT * FROM manager", PDBase.myConnection);
            DataSet dsMan = new DataSet();
            daMan.Fill(dsMan);
            comboBoxManager.DataSource = dsMan.Tables[0];
            comboBoxManager.DisplayMember = "ManagerName";
            comboBoxManager.ValueMember = "idManager";
            PDBase.myConnection.Close();
        }

        private void SelectClient()
        {
            SetRadioGroupStatus(-1);
            try
            {
                if (PDBase.myConnection.State == ConnectionState.Closed) PDBase.myConnection.Open();
                if (gridView1.RowCount > 0 && idClients > 0)
                {
                    MySqlDataAdapter da = new MySqlDataAdapter("", PDBase.myConnection);
                    DataTable dtClient = new DataTable();
                    da.SelectCommand.Parameters.Add("@idClients", MySqlDbType.Int32);
                    da.SelectCommand.Parameters[0].Value = idClients;

                    da.SelectCommand.CommandText = "SELECT * FROM clients WHERE idClients=@idClients";
                    da.Fill(dtClient);
                    da.SelectCommand.ExecuteNonQuery();

                    if (dtClient.Rows[0]["ClientsFIO"] != DBNull.Value)
                    {
                        labelFIO.Text = dtClient.Rows[0]["ClientsFIO"].ToString();
                    }
                    else
                    {
                        labelFIO.Text = "";
                    }

                    if (dtClient.Rows[0]["ClientsTel"] != DBNull.Value)
                    {
                        labelTelNum.Text = dtClient.Rows[0]["ClientsTel"].ToString();
                    }
                    else
                    {
                        labelTelNum.Text = "";
                    }

                    if (dtClient.Rows[0]["ClientsIndexSMS"] != DBNull.Value)
                    {
                        labelSms.Text = dtClient.Rows[0]["ClientsIndexSMS"].ToString();
                    }
                    else
                    {
                        labelSms.Text = "";
                    }

                    int ind = dtClient.Rows[0]["ClientsStatus"] == DBNull.Value ? -1 : (Int32)dtClient.Rows[0]["ClientsStatus"];
                    SetRadioGroupStatus(ind);                    
                    }
                }
            catch (Exception ex)
            {
                GMessage.ErrMSG(ex);
            }
        }

        //private void LoadComment()
        //{
        //    DataTable CommentData = new DataTable();
        //    if (CashBackBase.myConnection.State == ConnectionState.Closed)
        //    {
        //        CashBackBase.myConnection.Open();
        //    }
        //    try
        //    {
        //        if (idClients > 0)
        //        {
        //            MySqlCommand cmd = new MySqlCommand("", CashBackBase.myConnection);
        //            cmd.Parameters.Add("@idClients", MySqlDbType.Int32);
        //            cmd.Parameters[0].Value = idClients;
        //            cmd.CommandText = "SELECT idComments,CommentsDate,CommentsContent  FROM comments WHERE Clients_idClients=@idClients";
        //            cmd.ExecuteNonQuery();
        //            MySqlDataReader Reader = cmd.ExecuteReader();
        //            CommentData.Load(Reader);
        //            bindingSource2.DataSource = null;
        //            bindingSource2.DataSource = CommentData;
        //            Reader.Close();
        //            CashBackBase.myConnection.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GMessage.ErrMSG(ex);
        //    }
        //}
        private void LoadComment()
        {
            DataTable CommentData = new DataTable();
            if (PDBase.myConnection.State == ConnectionState.Closed) PDBase.myConnection.Open();
            try
            {
                if (idClients > 0)
                {
                    MySqlCommand cmd = new MySqlCommand("", PDBase.myConnection);
                    cmd.Parameters.Add("@idClients", MySqlDbType.Int32);
                    cmd.Parameters[0].Value = idClients;
                    cmd.CommandText = "SELECT * FROM comments WHERE Clients_idClients=@idClients";
                    cmd.ExecuteNonQuery();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    CommentData.Load(reader);
                    bindingSource2.DataSource = null;
                    bindingSource2.DataSource = CommentData;
                    reader.Close();
                    PDBase.myConnection.Close();
                }
            }
            catch(Exception ex)
            {
                GMessage.ErrMSG(ex);
            }
        }





        #region button
        private void linkLabelAdmin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Admin FRM = new Admin();
            FRM.ShowDialog();
        }

        private void buttonAddEvent_Click(object sender, EventArgs e)
        {
            EventForm FRM = new EventForm();
            FRM.ShowDialog();
        }
        #endregion

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (gridView1.RowCount > 0)
            {
                SelectClient();
                LoadComment();
            }
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
