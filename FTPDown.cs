using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Forms;


namespace Student
{
    class FTPDownload
    {

        private string ftpUsername;//ftp用户名
        private string ftpPwd;//ftp用户名密码

        /// <summary>
        /// FTPTransfer构造函数
        /// </summary>
        /// <param name="_ftpUsername">ftp用户名</param>
        /// <param name="_ftpPwd">ftp用户名密码</param>
        public FTPDownload(string _ftpUsername, string _ftpPwd)
        {
            ftpUsername = _ftpUsername;
            ftpPwd = _ftpPwd;
        }

        #region 现在的代码
        /// <summary>  
        /// 列出FTP服务器上面当前目录的所有文件和目录  
        /// </summary>  
        /// <param name="ftpUri">FTP目录</param>  
        /// <returns></returns>  
        public List<FileStruct> ListFilesAndDirectories(string ftpUri)
        {
            WebResponse webresp = null;
            StreamReader ftpFileListReader = null;
            FtpWebRequest ftpRequest = null;
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpUri));
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpRequest.Credentials = new NetworkCredential(ftpUsername, ftpPwd);
                webresp = ftpRequest.GetResponse();
                ftpFileListReader = new StreamReader(webresp.GetResponseStream(), Encoding.Default);
            }
            catch (Exception ex)
            {
                throw new Exception("获取文件列表出错，错误信息如下：" + ex.ToString());
            }
            string Datastring = ftpFileListReader.ReadToEnd();
            return GetList(Datastring);

        }

        /// <summary>  
        /// 获得文件和目录列表  
        /// </summary>  
        /// <param name="datastring">FTP返回的列表字符信息</param>  
        private List<FileStruct> GetList(string datastring)
        {
            List<FileStruct> myListArray = new List<FileStruct>();
            string[] dataRecords = datastring.Split('\n');
            //FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
            FileListStyle _directoryListStyle = FileListStyle.UnixStyle;//替换为Unix方式
            foreach (string s in dataRecords)
            {
                if (_directoryListStyle != FileListStyle.Unknown && s != "")
                {
                    FileStruct f = new FileStruct();
                    f.Name = "..";
                    f = ParseFileStructFromUnixStyleRecord(s);
                    if (!(f.Name == "." || f.Name == ".."))
                    {
                        myListArray.Add(f);
                    }
                }
            }
            return myListArray;
        }

        /// <summary>  
        /// 从Unix格式中返回文件信息  
        /// </summary>  
        /// <param name="Record">文件信息</param>  
        private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分  
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分  
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)  //time  
            {
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;   //最后就是名称  
            return f;
        }

        /// <summary>  
        /// 按照一定的规则进行字符串截取  
        /// </summary>  
        /// <param name="s">截取的字符串</param>  
        /// <param name="c">查找的字符</param>  
        /// <param name="startIndex">查找的位置</param>  
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }

        /// <summary>    
        /// 从FTP下载整个文件夹    
        /// </summary>    
        /// <param name="ftpDir">FTP文件夹路径</param>    
        /// <param name="saveDir">保存的本地文件夹路径</param>    
        public void DownFtpDir(string ftpDir, string saveDir)
        {
            List<FileStruct> files = ListFilesAndDirectories(ftpDir);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            foreach (FileStruct f in files)
            {
                if (f.IsDirectory) //文件夹，递归查询  
                {
                    DownFtpDir(ftpDir + "/" + f.Name + "/", saveDir + "\\" + f.Name);
                }
                else //文件，直接下载  
                {
                    DownLoadFile(ftpDir + "/" + f.Name, saveDir + "\\" + f.Name);
                }
            }
        }

        /// <summary>  
        /// 从ftp下载文件到本地服务器  
        /// </summary>  
        /// <param name="downloadUrl">要下载的ftp文件路径，如ftp://192.168.1.104/capture-2.avi</param>  
        /// <param name="saveFileUrl">本地保存文件的路径，如(@"d:\capture-22.avi"</param>  
        public void DownLoadFile(string downloadUrl, string saveFileUrl)
        {
            #region 原有的
            // Stream responseStream = null;
            // FileStream fileStream = null;
            // StreamReader reader = null;
            //// FileStream outputStream = new FileStream(filePath, FileMode.Create);
            // try
            // {
            //     // string downloadUrl = "ftp://192.168.1.104/capture-2.avi";  

            //     FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(downloadUrl);
            //     downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            //     //string ftpUser = "yoyo";  
            //     //string ftpPassWord = "123456";  
            //     downloadRequest.Credentials = new NetworkCredential(ftpUsername, ftpPwd);

            //     FtpWebResponse downloadResponse = (FtpWebResponse)downloadRequest.GetResponse();
            //     responseStream = downloadResponse.GetResponseStream();

            //     fileStream = File.Create(saveFileUrl);
            //     byte[] buffer = new byte[1024];
            //     int bytesRead;
            //     while (true)
            //     {
            //         bytesRead = responseStream.Read(buffer, 0, buffer.Length);
            //         if (bytesRead == 0)
            //             break;
            //         fileStream.Write(buffer, 0, bytesRead);
            //     }
            // }
            // catch (Exception ex)
            // {
            //     throw new Exception("从ftp服务器下载文件出错，文件名：" + downloadUrl + "异常信息：" + ex.ToString());
            // }
            // finally
            // {
            //     if (reader != null)
            //     {
            //         reader.Close();
            //     }
            //     if (responseStream != null)
            //     {
            //         responseStream.Close();
            //     }
            //     if (fileStream != null)
            //     {
            //         fileStream.Close();
            //     }
            // }
            #endregion
            FileStream outputStream = new FileStream(saveFileUrl, FileMode.Create);
            FtpWebRequest testRequest = (FtpWebRequest)WebRequest.Create(downloadUrl);
            testRequest.Credentials = new NetworkCredential(ftpUsername, ftpPwd);
            testRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            try
            {
                FtpWebResponse testResponse = (FtpWebResponse)testRequest.GetResponse();
                Stream ftpstream = testResponse.GetResponseStream();
                long c1 = testResponse.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpstream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpstream.Read(buffer, 0, bufferSize);
                }
                ftpstream.Close();
                outputStream.Close();
                testResponse.Close();
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

        }
        #endregion

        /// <summary>  
        /// 上传文件到Ftp服务器  
        /// </summary>  
        /// <param name="uri">把上传的文件保存为ftp服务器文件的uri,如"ftp://192.168.1.104/capture-212.avi"</param>  
        /// <param name="upLoadFile">要上传的本地的文件路径，如D:\capture-2.avi</param>  
        public void UpLoadFile(string UpLoadUri, string upLoadFile)
        {
            Stream requestStream = null;
            FileStream fileStream = null;
            FtpWebResponse uploadResponse = null;

            try
            {
                Uri uri = new Uri(UpLoadUri);

                FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(uri);
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;

                uploadRequest.Credentials = new NetworkCredential(ftpUsername, ftpPwd);

                requestStream = uploadRequest.GetRequestStream();
                fileStream = File.Open(upLoadFile, FileMode.Open);

                byte[] buffer = new byte[1024];
                int bytesRead;
                while (true)
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    requestStream.Write(buffer, 0, bytesRead);
                }

                requestStream.Close();

                uploadResponse = (FtpWebResponse)uploadRequest.GetResponse();

            }
            catch (Exception ex)
            {
                throw new Exception("上传文件到ftp服务器出错，文件名：" + upLoadFile + "异常信息：" + ex.ToString());
            }
            finally
            {
                if (uploadResponse != null)
                    uploadResponse.Close();
                if (fileStream != null)
                    fileStream.Close();
                if (requestStream != null)
                    requestStream.Close();
            }
        } 
 
    }
    #region 文件信息结构
    public struct FileStruct
    {
        public string Flags;
        public string Owner;
        public string Group;
        public bool IsDirectory;
        public DateTime CreateTime;
        public string Name;
    }
    public enum FileListStyle
    {
        UnixStyle,
        WindowsStyle,
        Unknown
    }
    #endregion
}
