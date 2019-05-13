using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace PositiveDecision
{
    class PDClass
    {
    }
    public static class PDBase
    {
        public static MySqlConnection myConnection = new MySqlConnection();
    }
    public class IniFile
    {
        public static string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>        
        /// <PARAM name="INIPath"></PARAM>
        /// Путь к файлу
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public static void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, IniFile.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public static string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(2000);
            int i = GetPrivateProfileString(Section, Key, "", temp, 2000, path);
            return temp.ToString();
        }
    }

    public class GVal
    {
        public const string sProgramName = "PositiveDecision";
    }

    public class GMessage
    {
        public static void MSG(string Message)
        {
            MessageBox.Show(Message, GVal.sProgramName);
        }
        public static void ErrMSG(string Message)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;
            result = MessageBox.Show(Message, GVal.sProgramName, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
        }
        public static void ErrMSG(Exception ex)
        {
            try
            {
                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\log.txt", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + ": " + ex.StackTrace + ": " + ex.Message);
            }
            catch
            {

            }
            MessageBox.Show("Обратитесь к разработчику" + AppDomain.CurrentDomain.BaseDirectory + "\\log.txt", GVal.sProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public class Comments
    {
        public static void WriteComment(string Commment, int idClient, string ManagersName)
        {
            if (idClient > 0)
                try
                {
                    if (PDBase.myConnection.State == ConnectionState.Closed)
                    {
                        PDBase.myConnection.Open();
                    }
                    MySqlCommand cmd = new MySqlCommand("", PDBase.myConnection);

                    cmd.Parameters.Add("@CommentsContent", MySqlDbType.VarChar, 500);
                    cmd.Parameters.Add("@idClients", MySqlDbType.Int32);
                    cmd.Parameters[0].Value = ManagersName + ": " + Commment;
                    cmd.Parameters[1].Value = idClient;
                    cmd.CommandText = "INSERT INTO `comments` (CommentsContent,Clients_idClients) values (@CommentsContent,@idClients)";
                    cmd.ExecuteNonQuery();
                    PDBase.myConnection.Close();
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 547)
                        GMessage.MSG("Заполните все поля!");
                    else
                        GMessage.ErrMSG(ex);
                }
                catch (FormatException)
                {
                    GMessage.MSG("Проверьте правильность заполнения полей");
                }
                catch (Exception ex)
                {
                    GMessage.ErrMSG(ex);
                }
        }
    }



}
