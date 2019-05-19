
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TP5.Core.NetCore.Extensions
{
    public static class FTPExtension
    {
        /// <summary>
        /// FTP伺服器IP或DNS名稱
        /// </summary>
        public static string FTPServerIP { get; set; }
        /// <summary>
        /// 登入帳號
        /// </summary>
        public static string UserName { get; set; }
        /// <summary>
        /// 登入密碼
        /// </summary>
        public static string PassWord { get; set; }
        /// <summary>
        /// 遠端目錄路徑
        /// </summary>
        public static string DirName { get; set; }
        /// <summary>
        /// 判斷是否為資料夾
        /// </summary>
        public static string FileOrDir { get; set; }
        /// <summary>
        /// 遠端檔名
        /// </summary>
        public static string FromFileName { get; set; }
        /// <summary>
        /// 本地檔名
        /// </summary>
        public static string ToFileName { get; set; }
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public static string FileName { get; set; }
        /// <summary>
        /// 失敗重試次數
        /// </summary>
        public static int FTPReTry { get; set; } = 3;
        /// <summary>
        /// 使否啟用 TLS/SSL 通訊(俗稱FTPS)
        /// </summary>
        public static bool EnabledSSL { get; set; }

        public static Array FTPQuery()
        {
            try
            {
                //DirName = DirName.Replace("\\", "/");
                //string sURI = "FTP://" + FTPServerIP + "/" + DirName;
                string sURI = GetFTPURL();
                FtpWebRequest myFTP = (FtpWebRequest)WebRequest.Create(sURI); //建立FTP連線
                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線
                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;
                myFTP.Credentials = new NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.Timeout = 2000; //等待時間
                myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                myFTP.Method = System.Net.WebRequestMethods.Ftp.ListDirectory; //取得檔案清單

                StreamReader myReadStream = new StreamReader(myFTP.GetResponse().GetResponseStream(), Encoding.Default); //取得FTP請求回應

                //檔案清單
                string sFTPFile; StringBuilder sbResult = new StringBuilder(); //,string[] sDownloadFiles;
                while (!(myReadStream.EndOfStream))
                {
                    sFTPFile = myReadStream.ReadLine();
                    sbResult.Append(sFTPFile + "\n");
                    //Console.WriteLine("{0}", FTPFile);
                }
                myReadStream.Close();
                myReadStream.Dispose();
                sFTPFile = null;
                sbResult.Remove(sbResult.ToString().LastIndexOf("\n"), 1); //檔案清單查詢結果
                //Console.WriteLine("Result:" + "\n" + "{0}", sResult);
                return sbResult.ToString().Split('\n'); //回傳至字串陣列
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP File Query Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message , null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPQuery();
                }
                else
                {
                    return null;
                }
            }
        }

        private static bool AcceptAllCertificatePolicy(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;    //不驗證伺服器端的X509憑證
        }

        public static List<string> FTPDetailQuery()
        {
            try
            {
                //DirName = DirName.Replace("\\", "/");
                //string sURI = "FTP://" + FTPServerIP + "/" + DirName;
                string sURI = GetFTPURL();
                FtpWebRequest myFTP = (FtpWebRequest)WebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new System.Net.NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.Timeout = 2000; //等待時間
                myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                myFTP.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails; //取得詳細檔案清單

                StreamReader myReadStream = new StreamReader(myFTP.GetResponse().GetResponseStream(), Encoding.Default); //取得FTP請求回應
                //目錄清單
                string sFileQuery;
                string[] sFileList;
                StringBuilder sbResult = new StringBuilder();
                List<string> lFileResult = new List<string>();
                while (!(myReadStream.EndOfStream))
                {
                    sFileQuery = myReadStream.ReadLine();
                    sbResult.Append(sFileQuery + "\n");
                    //Console.WriteLine("{0}", sFTPFile);
                }
                myReadStream.Close();
                myReadStream.Dispose();
                sFileQuery = null;
                sbResult.Remove(sbResult.ToString().LastIndexOf("\n"), 1); //檔案清單查詢結果
                //Console.WriteLine("Result:" & vbNewLine & "{0}", sResult);
                sFileList = sbResult.ToString().Split('\n'); //檔案清單轉換為字串陣列
                //判斷是否為資料夾
                if (FileOrDir.ToLower() == "file")
                {
                    FileOrDir = "-rw-r--r--";
                    //sFileOrDir = "-r--r--r--";
                }
                else
                {
                    FileOrDir = "drwxr-xr-x";
                }
                //解析資料夾
                foreach (string myFileInfo in sFileList)
                {
                    if (myFileInfo.IndexOf(FileOrDir) >= 0)
                    {
                        string[] sInfoStr = myFileInfo.Split(' ');
                        string sDirStr = null;
                        int iFileStr = 0;
                        //解析字元陣列
                        for (int myFileChar = 0; myFileChar < sInfoStr.Length; myFileChar++)
                        {
                            //字元陣列非空項則設為字串
                            if (sInfoStr[myFileChar] != null)
                                iFileStr++;

                            //字串陣列第9個為FTP資料夾名稱
                            if (iFileStr > 8)
                                sDirStr = sInfoStr[myFileChar] + " ";

                        }
                        sDirStr = sDirStr.Trim(); //去除字元空格
                        if (sDirStr != "." && sDirStr != "..")
                        {
                            lFileResult.Add(sDirStr);
                            //Console.WriteLine("sDownloadFiles:{0}", DownloadFiles[DownloadFiles.Count-1] );
                        }
                    }
                }
                return lFileResult;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP Dictionar Query Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPDetailQuery();
                }
                else
                {
                    return null;
                }
            }
        }

        public static DateTime FTPGetFileDate()
        {
            try
            {
                DirName = DirName.Replace("\\", "/");
                string sURI = "FTP://" + FTPServerIP + "/" + DirName + "/" + FileName;

                FtpWebRequest myFTP = (FtpWebRequest)WebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.Timeout = 2000; //等待時間
                myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                myFTP.Method = WebRequestMethods.Ftp.GetDateTimestamp; //取得資料修改日期

                FtpWebResponse myFTPFileDate = (FtpWebResponse)myFTP.GetResponse(); //取得FTP請求回應
                return myFTPFileDate.LastModified;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FTP Dictionar Query Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPGetFileDate();
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        public static int FTPGetFileSize()
        {
            try
            {
                DirName = DirName.Replace("\\", "/");
                string sURI = "FTP://" + FTPServerIP + "/" + DirName + "/" + FileName;
                FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new System.Net.NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.Timeout = 2000; //等待時間
                myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                myFTP.Method = System.Net.WebRequestMethods.Ftp.GetFileSize; //取得資料容量大小

                System.Net.FtpWebResponse myFTPFileSize = (System.Net.FtpWebResponse)myFTP.GetResponse(); //取得FTP請求回應
                return (int)myFTPFileSize.ContentLength;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP File Size Query Fail" + "\n" + "{0}", ex.Message)
                //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPGetFileSize();
                }
                else
                {
                    return 0;
                }
            }
        }

        public static bool FTPUploadFile()
        {
            try
            {
                //DirName = DirName.Replace("\\", "/");
                //string sURI = "FTP://" + FTPServerIP + "/" + DirName + "/" + ToFileName;
                string sURI = GetFTPURL();
                FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.KeepAlive = false; //關閉/保持 連線
                myFTP.Timeout = 2000; //等待時間
                myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                myFTP.UsePassive = false; //通訊埠接聽並等待連接
                myFTP.Method = System.Net.WebRequestMethods.Ftp.UploadFile; //下傳檔案
                /* proxy setting (不使用proxy) */
                myFTP.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                myFTP.Proxy = null;

                //上傳檔案
                FileStream myReadStream = new System.IO.FileStream(FromFileName, FileMode.Open, FileAccess.Read); //檔案設為讀取模式
                System.IO.Stream myWriteStream = myFTP.GetRequestStream(); //資料串流設為上傳至FTP
                byte[] bBuffer = new byte[2047]; int iRead = 0; //傳輸位元初始化
                do
                {
                    iRead = myReadStream.Read(bBuffer, 0, bBuffer.Length); //讀取上傳檔案
                    myWriteStream.Write(bBuffer, 0, iRead); //傳送資料串流
                    //Console.WriteLine("Buffer: {0} Byte", iRead);
                } while (!(iRead == 0));

                myReadStream.Flush();
                myReadStream.Close();
                myReadStream.Dispose();
                myWriteStream.Flush();
                myWriteStream.Close();
                myWriteStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP Upload Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPUploadFile();
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool FTPDownloadFile()
        {
            try
            {
                //DirName = DirName.Replace("\\", "/");
                //string sURI = "FTP://" + FTPServerIP + "/" + DirName + "/" + FromFileName;
                string sURI = GetFTPURL();
                FtpWebRequest myFTP = (FtpWebRequest)WebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.Timeout = 2000; //等待時間
                myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                myFTP.UsePassive = false; //通訊埠接聽並等待連接
                myFTP.Method = WebRequestMethods.Ftp.DownloadFile; //下傳檔案

                FtpWebResponse myFTPResponse = (FtpWebResponse)myFTP.GetResponse(); //取得FTP回應
                //下載檔案
                FileStream myWriteStream = new FileStream(ToFileName, FileMode.Create, FileAccess.Write); //檔案設為寫入模式
                Stream myReadStream = myFTPResponse.GetResponseStream(); //資料串流設為接收FTP回應下載
                byte[] bBuffer = new byte[2047]; int iRead = 0; //傳輸位元初始化
                do
                {
                    iRead = myReadStream.Read(bBuffer, 0, bBuffer.Length); //接收資料串流
                    myWriteStream.Write(bBuffer, 0, iRead); //寫入下載檔案
                    //Console.WriteLine("bBuffer: {0} Byte", iRead);
                } while (!(iRead == 0));

                myReadStream.Flush();
                myReadStream.Close();
                myReadStream.Dispose();
                myWriteStream.Flush();
                myWriteStream.Close();
                myWriteStream.Dispose();
                myFTPResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP Download Fail" & vbNewLine & "{0}", ex.Message)
                //MessageBox.Show(ex.Message , null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPDownloadFile();
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool FTPCreateDir()
        {
            try
            {
                //DirName = DirName.Replace("\\", "/");
                //string sURI = "FTP://" + FTPServerIP + "/" + DirName;
                string sURI = GetFTPURL();
                FtpWebRequest myFTP = (FtpWebRequest)WebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.KeepAlive = false; //關閉/保持 連線
                myFTP.Timeout = 2000; //等待時間
                myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                myFTP.Method = WebRequestMethods.Ftp.MakeDirectory; //建立目錄模式

                FtpWebResponse myFtpResponse = (FtpWebResponse)myFTP.GetResponse(); //創建目錄
                myFtpResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP Directory Create Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message , null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPCreateDir();
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool FTPDeleteFile()
        {
            try
            {
                //DirName = DirName.Replace("\\", "/");
                //string sURI = "FTP://" + FTPServerIP + "/" + DirName + FileName;
                string sURI = GetFTPURL();
                System.Net.FtpWebRequest myFTP = (FtpWebRequest)WebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new System.Net.NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.KeepAlive = false; //關閉/保持 連線
                myFTP.Timeout = 2000; //等待時間
                myFTP.Method = System.Net.WebRequestMethods.Ftp.DeleteFile; //刪除檔案

                System.Net.FtpWebResponse myFtpResponse = (FtpWebResponse)myFTP.GetResponse(); //刪除檔案/資料夾
                myFtpResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP File Seach Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message , null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPDeleteFile();
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool FTPRemoveDirectory()
        {
            try
            {
                //DirName = DirName.Replace("\\", "/");
                //string sURI = "FTP://" + FTPServerIP + "/" + DirName;
                string sURI = GetFTPURL();
                FtpWebRequest myFTP = (FtpWebRequest)WebRequest.Create(sURI); //建立FTP連線

                //設定連線模式及相關參數
                myFTP.EnableSsl = EnabledSSL;   //設定是否使用安全連線

                if (EnabledSSL)
                    ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;

                myFTP.Credentials = new NetworkCredential(UserName, PassWord); //帳密驗證
                myFTP.KeepAlive = false; //關閉/保持 連線
                myFTP.Timeout = 2000; //等待時間
                myFTP.Method = WebRequestMethods.Ftp.RemoveDirectory; //移除資料夾

                FtpWebResponse myFtpResponse = (FtpWebResponse)myFTP.GetResponse(); //刪除檔案/資料夾
                myFtpResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FTP File Seach Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                FTPReTry--;
                if (FTPReTry >= 0)
                {
                    return FTPRemoveDirectory();
                }
                else
                {
                    return false;
                }
            }
        }

        public static void cmdFTPDownloadFile()
        {
            try
            {
                DirName = DirName.Replace("\\", "/");
                FileStream myFTPCommand = new System.IO.FileStream("FTPCommand.txt", FileMode.Create, FileAccess.ReadWrite);
                StreamWriter myCommand = new StreamWriter(myFTPCommand);
                myCommand.BaseStream.Seek(0, SeekOrigin.Begin);
                myCommand.WriteLine("open" + " " + FTPServerIP + " ");
                myCommand.WriteLine(UserName);
                myCommand.WriteLine(PassWord);
                myCommand.WriteLine("get" + " " + DirName + "\"" + FromFileName + "\"" + " " + "\"" + ToFileName + "\"");
                myCommand.WriteLine("bye");
                myCommand.Flush();
                myCommand.Close();
                myCommand.Dispose();
                Process.Start(Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\ftp.exe", "-s:\""+Path.GetFullPath("FTPCommand.txt")+"\"").WaitForExit();
                File.Delete("D:\\FTPCommand.txt");
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP File Seach Fail" + "\n" + "{0}", ex.Message);
                //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
            }
        }

        public static List<string> cmdFTPQuery()
        {
            try
            {
                DirName = DirName.Replace("\\", "/");
                FileStream myFTPCommand = new FileStream("FTPCommand.txt", FileMode.Create, FileAccess.ReadWrite);
                StreamWriter myCommand = new StreamWriter(myFTPCommand);
                myCommand.BaseStream.Seek(0, SeekOrigin.Begin);
                myCommand.WriteLine("open" + " " + FTPServerIP + "\t");
                myCommand.WriteLine(UserName);
                myCommand.WriteLine(PassWord);
                myCommand.WriteLine("cd" + " " + "\"" + DirName + "\"");
                myCommand.WriteLine("ls" + " " + "*" + FileName + "*");
                myCommand.WriteLine("bye");
                myCommand.Flush();
                myCommand.Close();
                myCommand.Dispose();
                //建立cmd執行緒
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = System.Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\cmd.exe";
                //myProcess.StartInfo.Arguments = "/c " + Command(); //設定程式執行參數
                myProcess.StartInfo.UseShellExecute = false; //關閉Shell的使用
                myProcess.StartInfo.RedirectStandardInput = true; //重新導向標準輸入
                myProcess.StartInfo.RedirectStandardOutput = true; //重新導向標準輸出
                myProcess.StartInfo.RedirectStandardError = true; //重新導向錯誤輸出
                myProcess.StartInfo.CreateNoWindow = true; //設定不顯示視窗
                myProcess.Start();
                myProcess.StandardInput.WriteLine("ftp -s:\""+ Path.GetFullPath("FTPCommand.txt") +"\"");
                myProcess.StandardInput.WriteLine("exit");
                //解析檔案清單
                string sFileQuery;
                string[] sFileList;
                List<string> lFileResult = new List<string>();
                //sFileQuery = UrlEncode(myProcess.StandardOutput.ReadToEnd()); //從輸出流取得命令執行結果，解決中文亂碼的問題
                //Application.DoEvents();
                sFileQuery = myProcess.StandardOutput.ReadToEnd();
                sFileQuery = sFileQuery.Replace("\n", null);
                sFileList = sFileQuery.Split('\n');
                foreach (string myFile in sFileList)
                {
                    if (myFile.IndexOf(FileName) >= 0 && myFile.IndexOf(DirName) == 0)
                    {
                        string myStr = null;
                        if (myFile.IndexOf("版本") >= 0 && myFile.IndexOf(FTPServerIP) > 0)
                        {
                            char[] myChar = myFile.ToCharArray();
                            Array.Reverse(myChar);
                            myStr = new string(myChar);
                            myStr = myFile.Substring(myFile.Length - myStr.IndexOf("\t"), myStr.IndexOf("\t"));
                        }
                        lFileResult.Add(myStr);
                    }
                }
                File.Delete("FTPCommand.txt");
                return lFileResult;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("FTP File Seach Fail" & vbNewLine & "{0}", ex.Message)
                //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                return null;
            }
        }

        public static string UrlEncode(string Str)
        {
            if (Str == null)
                return null;

            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Str));
            //return Encoding.GetEncoding("utf-8").GetString(Encoding.GetEncoding("utf-8").GetBytes(Str));
        }

        private static string GetFTPURL()
        {
            StringBuilder stringBuilder = new StringBuilder("ftp://");

            if (!string.IsNullOrEmpty(FTPServerIP))
            {
                stringBuilder.Append(FTPServerIP);
            }

            if (!string.IsNullOrEmpty(DirName))
            {
                DirName.Replace("\\", "/");
                DirName = DirName.TrimStart('/');
                DirName = DirName.TrimEnd('/');
                stringBuilder.Append("/").Append(DirName);
            }

            if (!string.IsNullOrEmpty(FromFileName))
            {
                FileName = FromFileName.TrimStart('/');
                FileName = Path.GetFileName(FileName);
                if (!string.IsNullOrEmpty(DirName))
                {
                    stringBuilder.Append("/");
                }
                stringBuilder.Append(FileName);
            }
            else
            {
                if (!string.IsNullOrEmpty(ToFileName))
                {
                    FileName = ToFileName.TrimStart('/');
                    FileName = Path.GetFileName(FileName);
                    if (!string.IsNullOrEmpty(DirName))
                    {
                        stringBuilder.Append("/");
                    }
                    stringBuilder.Append(FileName);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
