using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DongBoListVip
{
    public class LHLog
    {

        public const string SecurityKey = "!lac@hong#media$";
        private string sFormat;
        private string sTime;
        private string path = "";
        public LHLog()
        {
            sFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " --- ";

            sTime = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
        }
        public void Error(Exception ex)
        {
            // log.Error(ex);
            ErrorLog(ex.Message, ex.StackTrace);
        }

        public void Error(string message)
        {
            //  log.Error(message);

            ErrorLog(message, "");
        }
        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file

            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = SecurityKey;
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            //if (useHashing)
            //{
            //    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            //    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //    //Always release the resources and flush data
            //    //of the Cryptographic service provide. Best Practice

            //    hashmd5.Clear();
            //}
            //else
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = SecurityKey;

            //if (useHashing)
            //{
            //    //if hashing was used get the hash code with regards to your key
            //    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            //    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //    //release any resource held by the MD5CryptoServiceProvider

            //    hashmd5.Clear();
            //}
            //else
            //{
            //if hashing was not implemented get the byte code of the key
            keyArray = UTF8Encoding.UTF8.GetBytes(key);
            //}

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        //public static string Decrypt(string stringToDecrypt)
        //{
        //    string sEncryptionKey = "!lac@hong#media$";
        //    byte[] key = { };
        //    byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
        //    byte[] inputByteArray = new byte[stringToDecrypt.Length];
        //    try
        //    {
        //        key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
        //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //        inputByteArray = Convert.FromBase64String(stringToDecrypt);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();
        //        Encoding encoding = Encoding.UTF8;
        //        return encoding.GetString(ms.ToArray());
        //    }
        //    catch (Exception)
        //    {
        //        return (string.Empty);
        //    }
        //}
        public void ErrorLog(string sErrMsg, string stackTrace)
        {
            try
            {
                string name = "";
                string user = "";
                if (Environment.MachineName != null)
                    name = Environment.MachineName;
                if (Environment.UserName != null)
                    user = Environment.UserName;

                string curFileName = "HNF_LHSVGetAll";
                string folPath = Directory.GetCurrentDirectory() + "\\ErrorLog";
                if (!Directory.Exists(folPath))
                {
                    System.IO.Directory.CreateDirectory(folPath);
                    path = folPath;
                }
                else
                {
                    path = folPath;
                }


                bool _FileUse = false;
                while (!_FileUse)
                {
                    try
                    {

                        string filePath = path + "\\" + sTime + "_" + curFileName + "_Error.txt";
                        string Content = "";
                        if (File.Exists(filePath))
                        {
                            StreamReader sr = new StreamReader(filePath);
                            Content = sr.ReadToEnd();
                            sr.Close();
                        }
                        string Err = "";
                        Err += name + "/" + user + "####" + sFormat + " " + sErrMsg;
                        Err += "\r\n" + "Stack Trace :" + stackTrace;
                        Err += "\r\n";

                        Err += Content;
                        StreamWriter strWriter = new StreamWriter(filePath, false);
                        strWriter.Write(Err);

                        strWriter.AutoFlush = true;
                        strWriter.Flush();
                        strWriter.Dispose();
                        strWriter.Close();

                        _FileUse = true;

                    }
                    catch (Exception ex)
                    {
                        _FileUse = false;
                        //throw ex;
                    }
                }






            }
            catch (Exception ex)
            {

            }
        }
    }
}
