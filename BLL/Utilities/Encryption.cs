using System;
using System.Security;
using System.Security.Permissions;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Data;


/// <summary>
/// Summary description for Encryption
/// </summary>
public class EncryptPassword
{
    //private DataStore dStoreParameterInfo = new DataStore(ConfigurationSettings.AppSettings.Get("GeneralPBLCommon"), "d_parameter_info");
    private DataTable dtparameterinfo = new DataTable();
    public EncryptPassword()
    {
        //String help_path = System.Configuration.ConfigurationManager.AppSettings.Get("HelpPath") + "temp";
        //String temp = help_path + @"\login_parameter_info.txt";
        //if (File.Exists(temp))
        //{
        //    dtparameterinfo = SBSFunctionLibrary.TextFiletoDataTable(temp);
        //    //dStoreParameterInfo.ImportFile(ConfigurationSettings.AppSettings.Get("HelpPath") + "login_parameter_info.txt", FileSaveAsType.Text);
        //}
    }

    public string sbs_encrypt(string password)
    {
        int p = 11, q = 13;
        double n, z, d, e;
        d = e = 0;
        string encryptpassword = "";

        n = p * q;
        z = (p - 1) * (q - 1);

        //find value of d
        for (int i = 2; i < z; i++)
        {
            if ((z % i) != 0)
            {
                d = i;
                break;
            }
        }

        //find value of e
        for (int i = 1; i < z; i++)
        {
            if (((d * i) - 1) % z == 0)
            {
                e = i;
                break;
            }
        }

        //encrypt given string
        char[] chararray = password.ToCharArray();
        for (int i = 0; i < chararray.Length; i++)
        {
            double ascii = (byte)chararray[i];
            int ciphertext = Convert.ToInt16(Math.Pow(ascii, e) % n);
            //encryptpassword += Convert.ToChar(ciphertext).ToString();
            encryptpassword += ciphertext.ToString();
        }
        return (encryptpassword);
    }

    public bool ValidatePassword(String stPassword)
    {
        String passlength = "";
        String passalpha = "";
        String passnumber = "";
        if (dtparameterinfo.Rows.Count > 0)
        {
            passalpha = dtparameterinfo.Rows[0]["param_value"].ToString();
            passlength = dtparameterinfo.Rows[1]["param_value"].ToString();
            passnumber = dtparameterinfo.Rows[2]["param_value"].ToString();
        }

        int DigitCount = 0, CharCount = 0, SpecialCount = 0;
        char[] charPassword = stPassword.ToCharArray();

        if (charPassword.Length > Convert.ToInt16(passlength))
        {
            //SBSMessageBox.Show("Maximum Length of Your Password can not Exceed " + passlength + "Character");
            return false;
        }
        for (int i = 0; i < charPassword.Length; i++)
        {
            int ascii = (byte)charPassword[i];
            if (ascii >= 48 && ascii <= 57)
                DigitCount++;
            else if ((ascii >= 65 && ascii <= 90) || (ascii >= 97 && ascii <= 122))
                CharCount++;
            else
                SpecialCount++;
        }
        if ((CharCount > Convert.ToInt16(passalpha)) || (DigitCount > Convert.ToInt16(passnumber)))
        {
            return false;
        }
        else
            return true;

    }
}