using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CenterView
{
    class RepairCitrix
    {
        //点击修复后执行代码
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //检测是否安装过Citrix
        //        var isExistCkCitrix = CkCitrix.CheckCitrix();
        //        if (isExistCkCitrix)
        //        {
        //            //需修改为检测后弹出信息.，已安装过Citrix--无问题
        //        }
        //        else
        //        {
        //            //检测是否下载过安装包，无，则开始下载
        //            if (File.Exists(@"..\\CitrixRecevier.exe"))
        //            {                       
        //                try
        //                {
        //                    MessageBox.Show("准备安装");
        //                    System.Diagnostics.Process.Start(@"..\\CitrixRecevier.exe");//未安装插件但存在安装包，自动安装插件
        //                }
        //                catch
        //                {
        //                    //安装失败，重新下载
        //                    string citrixPathName = @"..\\CitrixRecevier.exe"; //下载文件存储在上一级文件夹中，文件名字为Citrix Recevier
        //                    string citrixUrl = "https://downloadplugins.citrix.com/Windows/CitrixReceiver.exe";//需修改为读取配置文件   CitrixUrl
        //                    int flag=  Download.DownloadFile(citrixUrl, citrixPathName);
        //                    if (flag==1)
                            //{
                            //    MessageBox.Show("下载完成，准备安装");
                            //    System.Diagnostics.Process.Start(@"..\\CitrixRecevier.exe");
                            //}
                            //else
                            //{
        //                         MessageBox.Show("显示  CitrixUrl 要求用户手动下载");
        //                     }
        //                }
        //            }
        //                未检测到安装过Citrix
        //                else
        //                {
        //                    string citrixPathName = @"..\\CitrixRecevier.exe"; //下载文件存储在上一级文件夹中，文件名字为Citrix Recevier
        //                    string citrixUrl = "https://downloadplugins.citrix.com/Windows/CitrixReceiver.exe";//需修改为读取配置文件   CitrixUrl
        //                    int flag=  Download.DownloadFile(citrixUrl, citrixPathName);
                            //if (flag==1)
                            //{
                            //    MessageBox.Show("下载完成，准备安装");
                            //    System.Diagnostics.Process.Start(@"..\\CitrixRecevier.exe");
                            //}
                            //else
                            //{
        //                           MessageBox.Show("显示  CitrixUrl 要求用户手动下载");
                            //}
        //                }
        //            }
        //        }            
        //    catch
        //    {
        //        ////需修改为检测后弹出信息，修复失败，手动下载安装或联系客服
        //    }
        //}


        public int DownloadFile(string URL, string filename)
        {
            int flag = 0;//标识是否完成下载
            float percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate);//抛出错误可能原因，在c盘无write权限
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);
                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                }
                flag = 1;
                so.Close();
                st.Close();
                return flag;
            }
            catch
            {              
                return flag;
            }

        }
    }
}
