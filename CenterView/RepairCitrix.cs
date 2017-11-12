using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CenterView
{
    public class RepairCitrix
    {
        private bool _status;//返回状态
        /// <summary>
        /// 修复状态属性（只读）
        /// </summary>
        public bool Status
        {

            get { return _status; }
        }
        /// <summary>
        /// 修复Citrix
        /// </summary>
        /// <param name="url">
        /// String.Empty或null:citrix.com默认的下载地址；
        /// 其他值:自己保存的http地址
        /// </param>
        public void CitrixRep()
        {

            XMLconfigReader xMLconfigReader = new XMLconfigReader();
            string url = xMLconfigReader.CitrixUrl;//读取配置文件里的下载链接
            string path = Application.StartupPath + "\\CitrixReciver.exe";
            string thePath = Application.StartupPath + "\\CitrixReciverComplete.exe";
            string fullPath = Path.GetFullPath(path);
            string thefullPath = Path.GetFullPath(thePath);
            bool isExistCitrix = CkCitrix.CheckCitrix();

            //判断路径下是否有Citrix.exe，损坏的
            //无good.exe和Citrix.exe，未下
            if (!isExistCitrix)
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Thread t = new Thread(Download);
                    t.Name = "downLoad";
                    t.Start();


                }
                else if (File.Exists(thefullPath))
                {
                    bool isComPlete = InstallCitrix(thefullPath);//尝试安装无损坏文件
                    if (!isComPlete)
                    {
                        MessageBox.Show("无法正常安装，请检查系统设置是否正常");
                        return;
                    }
                }
                else
                {
                    Thread t = new Thread(Download);
                    t.Name = "downLoad";
                    t.Start();
                }
                //try
                //{
                //   System.Diagnostics.Process.Start(fullPath);


                //}
                //catch(Exception ex)
                //{
                //    MessageBox.Show("安装文件损坏,即将开始重新下载，请耐心等待");

                //    throw ex;


                //}

                //{
                //        try
                //        {
                //            File.Delete(fullPath);
                //            Thread t = new Thread(Download);
                //            t.Start();
                //            //RepairCitrix downloadCitrix = new RepairCitrix();
                //            //bool flag = downloadCitrix.Download(url, fullPath);
                //            //if (flag)
                //            //{
                //            //    System.Diagnostics.Process.Start(fullPath);
                //            //}
                //        }
                //        catch
                //        {
                //            MessageBox.Show("自动下载失败，请手动下载插件");
                //        }
                //    }

                //}

                //    {
                //        try
                //        {
                //            MessageBox.Show("即将开始下载插件安装包，请耐心等待");
                //            Thread t = new Thread(Download);
                //            t.Start();
                //            //bool flag = Download(url, fullPath);
                //            //if (flag)
                //            //{
                //            //   System.Diagnostics.Process.Start(fullPath);
                //            //}
                //        }
                //        catch
                //        {
                //            MessageBox.Show("自动下载失败，请手动下载插件");
                //        }
                //    }
                //}
            }
        }
        private bool InstallCitrix(string path)
        {
            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;

        }
        private void Download()
        {
            XMLconfigReader xMLconfigReader = new XMLconfigReader();
            string url = xMLconfigReader.CitrixUrl;//读取配置文件里的下载链接



            string path = Application.StartupPath + "\\CitrixReciver.exe";
            string fullPath = Path.GetFullPath(path);
            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream; // 写入本地文件流对象

            // 判断要下载的文件夹是否存在
            if (File.Exists(fullPath))
            {
                writeStream = File.OpenWrite(fullPath);             // 存在则打开要下载的文件
                startPosition = writeStream.Length;                  // 获取已经下载的长度
                writeStream.Seek(startPosition, SeekOrigin.Current); // 本地文件写入位置定位
            }
            else
            {
                writeStream = new FileStream(fullPath, FileMode.Create);// 文件不保存创建一个文件
                startPosition = 0;
            }


            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);// 打开网络连接

                if (startPosition > 0)
                {
                    myRequest.AddRange((int)startPosition);// 设置Range值,与上面的writeStream.Seek用意相同,是为了定义远程文件读取位置
                }


                Stream readStream = myRequest.GetResponse().GetResponseStream();// 向服务器请求,获得服务器的回应数据流





                byte[] btArray = new byte[512];// 定义一个字节数据,用来向readStream读取内容和向writeStream写入内容
                int contentSize = readStream.Read(btArray, 0, btArray.Length);// 向远程文件读第一次

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                long totalBytes = res.ContentLength;
                res.Close();
                while (contentSize > 0)// 如果读取长度大于零则继续读
                {
                    writeStream.Write(btArray, 0, contentSize);// 写入本地文件
                    contentSize = readStream.Read(btArray, 0, btArray.Length);// 继续向远程文件读取

                    FileInfo finfo = new FileInfo(path);
                    long currentSize = finfo.Length;
                    ThereferenceConst.repairProgressBarValue = Convert.ToInt32(currentSize / totalBytes * 100);
                }

                //更改文件名为good
                //关闭流
                writeStream.Close();
                readStream.Close();
                string newPath;
                int index = fullPath.LastIndexOf('\\');
                string theFrontPath = fullPath.Substring(0, index);
                newPath = theFrontPath + "\\CitrixReciverComplete.exe";
                File.Move(fullPath, newPath);
                _status = true;        //返回true下载成功

                bool isComPlete = InstallCitrix(newPath);//尝试安装无损坏文件
                if (!isComPlete)
                {
                    MessageBox.Show("无法正常安装，请检查系统设置是否正常");
                    return;
                }
            }
            catch (Exception)
            {
                writeStream.Close();
                _status = false;       //返回false下载失败
                MessageBox.Show("下载失败，请检查网络");
            }
            ThereferenceConst.strDownload = "已下载";

        }
    }
    //private void button1_Click(object sender, EventArgs e)//修复流程
    //{
    //    string URL = "https://downloadplugins.citrix.com/Windows/CitrixReceiver.exe";//需更改为读取配置文件包含的链接
    //    string path = @"..//CitrixReciver.exe";
    //    string fullPath = Path.GetFullPath(path);
    //    bool isExistCitrix = new CkCitrix().CheckCitrix();
    //    if(!isExistCitrix)
    //    {
    //    try
    //    {
    //        if (File.Exists(fullPath))
    //        {
    //            MessageBox.Show("已下载安装包,即将开始安装");
    //            System.Diagnostics.Process.Start(fullPath);
    //        }
    //        else
    //        {
    //            MessageBox.Show("即将开始下载，请耐心等待");
    //            RepairCitrix downloadCitrix = new RepairCitrix();
    //            bool flag = downloadCitrix.Download(URL, fullPath);
    //            if (flag)
    //            {
    //                MessageBox.Show("下载完成，即将开始安装");
    //                System.Diagnostics.Process.Start(fullPath);
    //            }
    //        }
    //    }
    //    catch
    //    {
    //        MessageBox.Show("自动修复失败，请手动下载Citrix Receiver");
    //    }
    //   }
    //}
}
