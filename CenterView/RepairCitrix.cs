using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CenterView
{
    class RepairCitrix
    {
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


        public bool Download(string url, string localfile)
        {
            bool flag = false;
            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream; // 写入本地文件流对象

            // 判断要下载的文件夹是否存在
            if (File.Exists(localfile))
            {

                writeStream = File.OpenWrite(localfile);             // 存在则打开要下载的文件
                startPosition = writeStream.Length;                  // 获取已经下载的长度
                writeStream.Seek(startPosition, SeekOrigin.Current); // 本地文件写入位置定位
            }
            else
            {
                writeStream = new FileStream(localfile, FileMode.Create);// 文件不保存创建一个文件
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

                while (contentSize > 0)// 如果读取长度大于零则继续读
                {
                    writeStream.Write(btArray, 0, contentSize);// 写入本地文件
                    contentSize = readStream.Read(btArray, 0, btArray.Length);// 继续向远程文件读取
                }

                //关闭流
                writeStream.Close();
                readStream.Close();

                flag = true;        //返回true下载成功
            }
            catch (Exception)
            {
                writeStream.Close();
                flag = false;       //返回false下载失败
            }

            return flag;
        }
    }
}
