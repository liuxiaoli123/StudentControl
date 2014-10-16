//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Student
{
    public class FtpUpload
    {
        private string ftpUser = string.Empty;
        private string ftpPassword = string.Empty;
        private string ftpRootURL = string.Empty;
        private bool isFlag = true;
        private string baseFolderPath = null;

        public FtpUpload(string url, string userid, string password)
        {
            this.ftpUser = userid; 
            this.ftpPassword = password;
            this.ftpRootURL = url;
        }

        /// <summary>
        /// 文件夹上传
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder">ftpRootUrl + ftpPath</param>
        /// <returns></returns>
        public bool foldersUpload(string sourceFolder, string destFolder, string detailFolder)
        {
            bool isFolderFlag = false;
            if (isFlag)
            {
                baseFolderPath = sourceFolder.Substring(0, sourceFolder.LastIndexOf("\\"));
                isFlag = false;
            }

            string selectFolderName = sourceFolder.Replace(baseFolderPath, "").Replace("\\", "/");
           
            if (selectFolderName != null)
            {
                string ftpDirectory = destFolder + selectFolderName;
                if (ftpDirectory.LastIndexOf('/') < ftpDirectory.Length - 1)
                {
                    ftpDirectory = ftpDirectory + "/";
                }
                if (!FtpDirectoryIsNotExists(ftpDirectory))
                {
                    CreateFtpDirectory(ftpDirectory);
                }
            }

            try
            {
                string[] directories = Directory.EnumerateDirectories(sourceFolder).ToArray();
                if (directories.Length > 0)
                {
                    foreach (string d in directories)
                    {
                        foldersUpload(d, destFolder, sourceFolder.Replace(baseFolderPath, "").Replace("\\","/"));
                    }
                }

                string[] files = Directory.EnumerateFiles(sourceFolder).ToArray();
                if (files.Length > 0)
                {
                    foreach (string s in files)
                    {
                        
                        string fileName = s.Substring(s.LastIndexOf("\\")).Replace("\\", "/");

                        if(selectFolderName.Contains("/"))
                        {
                           if(selectFolderName.LastIndexOf('/') < selectFolderName.Length -1)
                           {
                               selectFolderName = selectFolderName + '/';
                           }
                    
                        }
                        ftpRootURL = destFolder;

                        fileUpload(new FileInfo(s), selectFolderName , fileName.Substring(1,fileName.Length -1));

                    }
                }
                isFolderFlag = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return isFolderFlag;
        }


        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="localFile">本地文件绝对路径</param>
        /// <param name="ftpPath">上传到ftp的路径</param>
        /// <param name="ftpFileName">上传到ftp的文件名</param>
        public bool fileUpload(FileInfo localFile, string ftpPath, string ftpFileName)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;

            FileStream localFileStream = null;
            Stream requestStream = null;

            try
            {
                // 检查FTP目标存放目录是否存在
                // 1.1 ftp 上目标目录
                string destFolderPath =  ftpRootURL + ftpPath;

                if (!FtpDirectoryIsNotExists(destFolderPath))
                {
                    CreateFtpDirectory(destFolderPath);
                }

                string uri = ftpRootURL + ftpPath + ftpFileName;
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.UseBinary = true;

                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpWebRequest.ContentLength = localFile.Length;

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;

                localFileStream = localFile.OpenRead();
                requestStream = ftpWebRequest.GetRequestStream();

                contentLen = localFileStream.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream
                    requestStream.Write(buff, 0, contentLen);
                    contentLen = localFileStream.Read(buff, 0, buffLength);
                }

                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
                if (localFileStream != null)
                {
                    localFileStream.Close();
                }
            }

            return success;
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localPath">本地文件地址(没有文件名)</param>
        /// <param name="localFileName">本地文件名</param>
        /// <param name="ftpPath">上传到ftp的路径</param>
        /// <param name="ftpFileName">上传到ftp的文件名</param>
        public bool fileUpload(string localPath, string localFileName, string ftpPath, string ftpFileName)
        {
            bool success = false;
            try
            {
                FileInfo localFile = new FileInfo(localPath + localFileName);
                if (localFile.Exists)
                {
                    success = fileUpload(localFile, ftpPath, ftpFileName);
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="localPath">本地文件地址(没有文件名)</param>
        /// <param name="localFileName">本地文件名</param>
        /// <param name="ftpPath">下载的ftp的路径</param>
        /// <param name="ftpFileName">下载的ftp的文件名</param>
        public bool fileDownload(string localPath, string localFileName, string ftpPath, string ftpFileName)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            FileStream outputStream = null;
            try
            {
                outputStream = new FileStream(localPath + localFileName, FileMode.Create);
                string uri = ftpRootURL + ftpPath + ftpFileName;
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpResponseStream = ftpWebResponse.GetResponseStream();
                long contentLength = ftpWebResponse.ContentLength;
                int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                int readCount;
                readCount = ftpResponseStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpResponseStream.Read(buffer, 0, bufferSize);
                }
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (outputStream != null)
                {
                    outputStream.Close();
                }
                if (ftpResponseStream != null)
                {
                    ftpResponseStream.Close();
                }
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }
            return success;
        }


        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="ftpPath">ftp文件路径</param>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        public bool fileRename(string ftpPath, string currentFileName, string newFileName)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            try
            {
                string uri = ftpRootURL + ftpPath + currentFileName;
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpWebRequest.RenameTo = newFileName;

                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpResponseStream = ftpWebResponse.GetResponseStream();

            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (ftpResponseStream != null)
                {
                    ftpResponseStream.Close();
                }
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }
            return success;
        }


        /// <summary>
        /// 消除文件
        /// </summary>
        /// <param name="filePath"></param>
        public bool fileDelete(string ftpPath, string ftpName)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            StreamReader streamReader = null;
            try
            {
                string uri = ftpRootURL + ftpPath + ftpName;
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                long size = ftpWebResponse.ContentLength;
                ftpResponseStream = ftpWebResponse.GetResponseStream();
                streamReader = new StreamReader(ftpResponseStream);
                string result = String.Empty;
                result = streamReader.ReadToEnd();

                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (ftpResponseStream != null)
                {
                    ftpResponseStream.Close();
                }
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }
            return success;
        }

        /// <summary>
        /// 文件存在检查
        /// </summary>
        public bool fileCheckExist(string destFolderPath, string fileName)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            WebResponse webResponse = null;
            StreamReader reader = null;
            try
            {


                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(destFolderPath));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpWebRequest.KeepAlive = false;
                webResponse = ftpWebRequest.GetResponse();
                reader = new StreamReader(webResponse.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    string ftpName = "test.jpg";
                    if (line == ftpName)
                    {
                        success = true;
                        break;
                    }
                    line = reader.ReadLine();
                }
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
            return success;
        }

       
        /// <summary>
        /// 创建FTP文件目录
        /// </summary>
        /// <param name="ftpDirectory">ftp服务器上的文件目录</param>
        public void CreateFtpDirectory(string ftpDirectory)
        {
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpDirectory));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse respFTP = (FtpWebResponse)ftpWebRequest.GetResponse();
                respFTP.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FTP创建目录失败" + ex.Message);
            }

        }




        /// <summary>
        /// 获取目录下的详细信息
        /// </summary>
        /// <param name="localDir">本机目录</param>
        /// <returns></returns>
        public List<List<string>> GetDirDetails(string localDir)
        {
            List<List<string>> infos = new List<List<string>>();
            try
            {
                infos.Add(Directory.GetFiles(localDir).ToList());
                infos.Add(Directory.GetDirectories(localDir).ToList());
                for (int i = 0; i < infos[0].Count; i++)
                {
                    int index = infos[1][i].LastIndexOf(@"\");
                    infos[1][i] = infos[1][i].Substring(index + 1);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return infos;
        }


        public void UploadDirectory(string localDir, string ftpPath, string dirName, string ftpUser, string ftpPassword)
        {
            if (ftpUser == null)
            {
                ftpUser = "";
            }
            if (ftpPassword == null)
            {
                ftpPassword = "";
            }

            string dir = localDir + dirName + @"\";

            if (!Directory.Exists(dir))
            {
                return;
            }

            //if (!CheckDirectoryExist(ftpPath, dirName))
            //{
            //    MakeDir(ftpPath, dirName);
             
            //}

            List<List<string>> infos = GetDirDetails(dir); //获取当前目录下的所有文件和文件夹
            //先上传文件
        //    MyLog.ShowMessage(dir + "下的文件数：" + infos[0].Count.ToString());
            for (int i = 0; i < infos[0].Count; i++)
            {
                Console.WriteLine(infos[0][i]);
             //   UpLoadFile(dir + infos[0][i], ftpPath + dirName + @"/" + infos[0][i], ftpUser, ftpPassword);
            }
            //再处理文件夹
          //  MyLog.ShowMessage(dir + "下的目录数：" + infos[1].Count.ToString());
            for (int i = 0; i < infos[1].Count; i++)
            {
                UploadDirectory(dir, ftpPath + dirName + @"/", infos[1][i], ftpUser, ftpPassword);
            }
        }

        /// <summary>
        /// 判断Ftp上待上传文件存放的（文件夹）目录是否存在
        /// 注意事项：目录结构的最后一个字符一定要是一个斜杠
        /// </summary>
        /// <param name="destFtpFolderPath">Ftp服务器上存放待上传文件的目录</param>
        private  bool FtpDirectoryIsNotExists(string destFolderPath)
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(destFolderPath);
                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return false;
                }
                else
                {
                    response.Close();
                }
            }
            return true;
        }

        /// <summary>
        /// 解析文件所在的路径（即当前文件所在的文件位置）
        /// </summary>
        /// <param name="destFilePath">需要存储在FTP服务器上的文件路径，如：ftp://192.168.1.100/LocalUser/picture1.jpg</param>
        /// <returns></returns>
        public string FtpParseDirectory(string destFilePath)
        {
            return destFilePath.Substring(0, destFilePath.LastIndexOf("/"));
        }


        // 验证文件类型
        public bool IsAllowableFileType(string fileName)
        {
            //从web.config读取判断文件类型限制
            string stringstrFileTypeLimit = string.Format(".jpeg|*.jpeg|*.*|All Files");
            //当前文件扩展名是否包含在这个字符串中
            if (stringstrFileTypeLimit.IndexOf(fileName.ToLower()) != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //文件大小
        public bool IsAllowableFileSize(long FileContentLength)
        {
            //从web.config读取判断文件大小的限制
            Int32 doubleiFileSizeLimit = 1232;

            //判断文件是否超出了限制
            if (doubleiFileSizeLimit > FileContentLength)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}