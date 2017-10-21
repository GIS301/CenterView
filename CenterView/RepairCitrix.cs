using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CenterView
{
    class RepairCitrix
    {      
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
