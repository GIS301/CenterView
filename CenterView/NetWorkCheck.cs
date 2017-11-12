using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Management;
using NetworkMonitor;

namespace CenterView
{
    public class NetworkCheck
    {
        //BaseInfo baseInfo = new BaseInfo();

        /// <summary>
        /// 外网地址
        /// </summary>
        private const string CTrustWebsite = "TrustWebsite3";
        /// <summary>
        /// 目标地址
        /// </summary>
        private const string CTrustWebsite2 = "TrustWebsite2";
        /// <summary>
        /// 启动桌面云最低网速要求
        /// </summary>
        private const string CInternetSpeed1 = "InternetSpeed1";
        /// <summary>
        ///启动应用程序最低网络要求
        /// </summary>
        private const string CInternetSpeed2 = "InternetSpeed2";

        private const string CErrConfig = "Config错误，请重新初始化系统，建议联系客服人员";
        /// <summary>
        /// config.xml的绝对路径
        /// </summary>
        private string _configPath = System.Windows.Forms.Application.StartupPath + "\\config.xml";

        public NetworkCheck()
        {
            //暂时测试用???
            //_configPath = @"E:\Project\GitRepos\CenterView\bin\Debug\config.xml";
            //获取Config.xml的TrustWebsite，以及TrustWebsite2
            GetTrustWebSite();
        }

        /// <summary>
        /// 检测除网速以外的网络状态、检测内网、外网连接状态、检测丢包率
        /// </summary>
        /// <returns></returns>
        public IList<string> CheckAllNetStatus()
        {
            IList<string> retlist = new List<string>();//每种检测保存一个string
            string temp = String.Empty;
            //检测网络是否断开
            temp = CheckConnectionStatus();
            if (temp.Equals(String.Empty))
            {
                //检测内网、外网连接状态
                //-1:Config.xml错误；0：外网无法联通；1：外网、目标地址均正常；2外网通，目标地址不正常
                short s = CheckNetStatus(ref temp);
                switch (s)
                {
                    case -1:
                        retlist.Add(CErrConfig);
                        break;
                    case 0: retlist.Add(CNetWebErr1);
                           break;
                    case 2:
                        retlist.Add(temp);
                        break;
                    case 1:
                         retlist.Add("外网、目标地址正常，正在测试网络性能...");//假如出现了情况1，retlist的Count=2
                        //检测网络速度
                        //       ();//开始网络检测 added by jeff 2017/10/24
                        //检测丢包率
                        retlist.Add("正在检测网速和丢包率");
                       // retlist.Add(GetPingnetInfo());
                        break;
                    default:

                        break;
                }
            }
            else
            {
                retlist.Add(CTipBadNet);
            }
            return retlist;
        }

        #region 网络连接检测

        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;
        private const string CTipBadNet = "网络断开";
        private const string CTipOkNet = "网络连接状态正常";
        [System.Runtime.InteropServices.DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

        private string CheckConnectionStatus()
        {
            string ret = String.Empty;
            bool isCon = LocalConnectionStatus();
            if (!isCon)
                ret = CTipBadNet;
            return ret;
        }
        /// <summary>
        /// 判断本地的连接状态
        /// </summary>
        /// <returns></returns>
        private bool LocalConnectionStatus()
        {
            System.Int32 dwFlag = new Int32();
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {

                return false;
            }
            else
            {
                if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
                {
                    //采用调制解调器上网
                    return true;
                }
                else if ((dwFlag & INTERNET_CONNECTION_LAN) != 0)
                {
                    //采用网卡上网
                    return true;
                }
            }
            return false;

        }
        #endregion

        #region 网络与目标主机的连通性检测


        private const string CNetWeb1 = "外网连接正常；";
        private const string CNetWebErr1 = "外网连接失败";

        private const string CNetWeb2 = "目标地址连接正常";
        private const string CNetWebErr2 = "无法连接目标地址，建议联系客服人员";

        /// <summary>
        /// Config.xml的外网地址
        /// </summary>
        private string[] _strWeb1s = null;
        /// <summary>
        /// Config.xml的目标地址
        /// </summary>
        public string[] _strWeb2s = null;


        /// <summary>
        /// 诊断网络状态
        /// </summary>
        /// <param name="statusTip">状态提示</param>
        /// <returns>-1:Config.xml错误；0：外网无法联通；1：外网、目标地址均正常；2外网通，目标地址不正常</returns>
        private short CheckNetStatus(ref string statusTip)
        {
            short ret = -1;

            if (_strWeb1s == null || _strWeb2s == null)
            {
                statusTip = "Config.xml缺少外网地址和目标地址";
                return ret;
            }

            List<string> errorUrls = null;
            //检测外网是否联通
            if (GetUrlsStatus(_strWeb1s, out errorUrls))
            {
                statusTip = CNetWeb1;
            }
            else
            {
                statusTip = CNetWebErr1 + ":";
                foreach (string err in errorUrls)
                {
                    statusTip += err;
                }
                statusTip += "；";
                ret = 0;
                return ret;//外网无法联通直接返回
            }

            //检测目标地址联通性
            if (GetUrlsStatus(_strWeb2s, out errorUrls))
            {
                statusTip += CNetWeb2;
                ret = 1;
            }
            else
            {
                statusTip += CNetWebErr2 + ":"; ;
                foreach (string err in errorUrls)
                {
                    statusTip += err;
                }
                statusTip += "；";
                ret = 2;
            }
            return ret;
        }

        /// <summary>
        /// 获取外网地址、目标地址
        /// </summary>
        private void GetTrustWebSite()
        {
            string strWebsite1 = XMLconfigReader.getValbyName(_configPath, CTrustWebsite);
            string strWebsite2 = XMLconfigReader.getValbyName(_configPath, CTrustWebsite2);

            _strWeb1s = strWebsite1.Split(new char[] { ',' });
            _strWeb2s = strWebsite2.Split(new char[] { ',' });
        }
        /// <summary>
        /// 判断域名是否可以访问，模拟浏览器的方法
        /// </summary>
        /// <param name="Url">域名地址</param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool GetResponse(string Url, string type = "UTF-8")
        {
            try
            {
                System.Net.HttpWebRequest wReq = (HttpWebRequest)System.Net.HttpWebRequest.Create(Url);
                wReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0";     // 模仿浏览器参数
                wReq.Timeout = 4000;//设置响应时间TimeOut
                System.Net.HttpWebResponse wResp = (HttpWebResponse)wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                return false;
                throw ex;
            }

        }
        /// <summary>
        /// 检测网络是否畅通
        /// </summary>
        /// <param name="urls">URL数据</param>
        /// <param name="errorCount">ping时连接失败个数</param>
        /// <returns>true:成功；false:失败</returns>
        private bool GetUrlsStatus(string[] urls, out List<string> errorUrls)
        {
            bool isconn = true;
            errorUrls = new List<string>();
            try
            {
                for (int i = 0; i < urls.Length; i++)
                {
                    if (!GetResponse("http://" + urls[i]))
                    {
                        isconn = false;
                        errorUrls.Add(urls[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                isconn = false;
                throw ex;
            }
            return isconn;

        }

        /// <summary>
        /// Ping命令检测网络是否畅通
        /// </summary>
        /// <param name="urls">URL数据</param>
        /// <param name="errorCount">ping时连接失败个数</param>
        /// <returns>true:成功；false:失败</returns>
        //private bool PingUrls(string[] urls, out List<string> errorUrls)
        //{
        //    bool isconn = true;
        //    Ping ping = new Ping();
        //    errorUrls = new List<string>();
        //    try
        //    {
        //        PingReply pr;
        //        for (int i = 0; i < urls.Length; i++)
        //        {
        //            pr = ping.Send(urls[i],1000);//11-1 1000表示等待毫秒数
        //            if (pr.Status != IPStatus.Success)
        //            {
        //                isconn = false;
        //                errorUrls.Add(urls[i]);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        isconn = false;
        //    }
        //    return isconn;
        //}
        #endregion

        #region 检测网络连接速度
        //private NetworkAdapter[] adapters;
        private NetworkMonitor.NetworkMonitor _monitor = null;
        private static int _elaspeT = 0;
        private NetworkAdapter _curAdapter;

        public NetworkAdapter CurAdapter
        {
            get { return _curAdapter; }
        }

        private double _InnerSpeed1;//默认最大速度

        public double InnerSpeed1
        {
            get { return _InnerSpeed1; }
            set { _InnerSpeed1 = value; }
        }
        private double _InnerSpeed2;//默认最小速度

        public double InnerSpeed2
        {
            get { return _InnerSpeed2; }
            set { _InnerSpeed2 = value; }
        }
        private double _MaxInnerSpeed = 0.0;//最大速度
        /// <summary>
        /// 30秒内最大速度
        /// </summary>
        public double MaxInnerSpeed
        {
            get { return _MaxInnerSpeed; }
            set { _MaxInnerSpeed = value; }
        }
        private double _MinInnerSpeed = 0.0;//最小速度
        /// <summary>
        /// 30秒内最小速度
        /// </summary>
        public double MinInnerSpeed
        {
            get { return _MinInnerSpeed; }
            set { _MinInnerSpeed = value; }
        }
        public NetworkAdapter getCurrentAdapter()
        {
            _monitor = new NetworkMonitor.NetworkMonitor();
            string strAdapter = GetActivatedAdapter();
            if (strAdapter != null && strAdapter != String.Empty)
            {
                strAdapter = strAdapter.Substring(strAdapter.IndexOf(')') + 1, strAdapter.Length - strAdapter.IndexOf(')') - 1);
                strAdapter.Trim();
            }
            foreach (NetworkAdapter adapter in _monitor.Adapters)
            {
                string temp = adapter.Name;
                temp = temp.Substring(temp.IndexOf(']') + 1, temp.Length - temp.IndexOf(']') - 1);
                temp.Trim();
                if (temp.Substring(0, temp.Length - 4) == strAdapter.Substring(0, strAdapter.Length - 4))
                {
                    _curAdapter = adapter;//得到当前的网络Adapter
                    //_monitor.StopMonitoring();
                    //_monitor.StartMonitoring(adapter);
                    return _curAdapter;
                }

            }
            return null;

        }
        /// <summary>
        /// 网络速度测试
        /// </summary>
        public void MonitorNetSpeed()
        {
            Double.TryParse(XMLconfigReader.getValbyName(_configPath, CInternetSpeed1), out _InnerSpeed1);
            Double.TryParse(XMLconfigReader.getValbyName(_configPath, CInternetSpeed2), out _InnerSpeed2);

            _monitor = new NetworkMonitor.NetworkMonitor();
            //_monitor.NetworkSpeedChange += new NetworkMonitor.NetworkAdapterRefreshHandle(monitor_NetworkSpeedChange);
            string strAdapter = GetActivatedAdapter();
            if (strAdapter != null && strAdapter != String.Empty)
            {
                strAdapter = strAdapter.Substring(strAdapter.IndexOf(')') + 1, strAdapter.Length - strAdapter.IndexOf(')') - 1);
                strAdapter.Trim();
            }
            foreach (NetworkAdapter adapter in _monitor.Adapters)
            {
                string temp = adapter.Name;
                temp = temp.Substring(temp.IndexOf(']') + 1, temp.Length - temp.IndexOf(']') - 1);
                temp.Trim();
                if (temp.Equals(strAdapter))
                {
                    _curAdapter = adapter;//得到当前的网络Adapter
                    _monitor.StopMonitoring();
                    _monitor.StartMonitoring(adapter);

                }
            }
        }

        private void monitor_NetworkSpeedChange(object sender, EventArgs e)
        {
            //_elaspeT++;
            //if (_elaspeT >= 30)//循环30次
            //{
            //    _elaspeT = 0;
            //    _monitor.StopMonitoring();
            //}
            //else
            //{
            //    if (_MinInnerSpeed == 0.0) _MinInnerSpeed = curAdapter.DownloadSpeedKbps;
            //    _MaxInnerSpeed = curAdapter.DownloadSpeedKbps > _MaxInnerSpeed ? curAdapter.DownloadSpeedKbps : _MaxInnerSpeed;
            //    _MinInnerSpeed = curAdapter.DownloadSpeedKbps < _MaxInnerSpeed ? curAdapter.DownloadSpeedKbps : _MaxInnerSpeed;
            //}
            //Console.Out.WriteLine(String.Format("{0:n} kbps", _MaxInnerSpeed));
            //Console.Out.WriteLine(String.Format("{0:n} kbps", _MinInnerSpeed));
        }

        private string GetActivatedAdapter()
        {
            string ret = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["IPEnabled"].ToString() == "True")
                {
                    ret = mo["Description"].ToString();
                    break;
                }
            }
            return ret;
        }


        #endregion
        #region 检测丢包率
        /// <summary>
        /// 检测丢包率
        /// </summary>
        /// <returns></returns>
        private string GetPingnetInfo()
        {


            string result = "";
            if (_strWeb1s == null || (_strWeb2s == null && _strWeb2s.Length <= 0))
                return result;


            int count = 200;

            Ping ping = new Ping();
            long timeSum = 0;
            int succCount = 0;            //发送

            for (int i = 0; i < count; ++i)
            {
                PingReply pr = ping.Send(_strWeb2s[0]);
                if (pr.Status == IPStatus.TimedOut)
                {
                    result = CNetWebErr2;
                }
                else if (pr.Status == IPStatus.Success)
                {
                    result = string.Format("延时：{0}毫秒", pr.RoundtripTime);

                    ++succCount;
                    timeSum += pr.RoundtripTime;
                }
            }
            if (timeSum != 0)
            {
                result = string.Format("平均延时{0}毫秒，丢包率{1}%", 1.0 * timeSum / succCount, (count - succCount) * 100.0 / count);
            }
            else
            {
                result = ("丢包率100%");
            }
            return result;
        }
        #endregion


    }

}









//   //检查网络链接
//   public string GetNetConnectInfo()
//   {
//       string result = "";
//       Ping p = new Ping();//创建Ping对象p
//       PingReply pr = p.Send("123.145.68.1");//向指定IP或者主机名的计算机发送ICMP协议的ping数据包，多设置几个ip来检测，只要一个未成功则显示外网断开，
//       if (pr.Status == IPStatus.Success)//如果ping成功
//       {
//           result += ("网络连接成功");

//       }
//       else
//       {
//           int times = 0;//重新连接次数;
//           do
//           {
//               if (times >= 12)
//               {
//                   result += ("重新尝试连接超过12次,连接失败程序结束") + " ";

//               }
//               Thread.Sleep(600000);//等待十分钟(方便测试的话，你可以改为1000)
//               pr = p.Send("123.145.68.1");
//               result += (pr.Status);
//               times++;
//           }
//           while (pr.Status != IPStatus.Success);
//           result += ("连接成功");

//           times = 0;//连接成功，重新连接次数清为0;
//       }
//       return result;
//}


//   #region
//   //定义允许的IP端，格式如下
//   //"10.0.0.0 - 10.255.255.255", "172.16.0.0 - 172.31.255.255", "192.168.0.0 - 192.168.255.255"

//  string[] AllowIPRanges = { "10.0.0.0 - 10.255.255.255", "172.16.0.0 - 172.31.255.255", "192.168.0.0 - 192.168.255.255" };

//   //
//   public bool GetIsInnerNetInfo(string ip)
//   {

//       //判断192.168.100.0这个ip是否在指定的IP范围段内
//       //就这个范围而言，如果把IP转换成long型的 那么192.167.0.0这个IP 将在10.0.0.0-10.255.255.255这个范围内，但实际上这是错误的。还希望高手指点将ip转换为long的内幕
//          bool isSuccess;
//          isSuccess = (TheIpIsRange(ip, AllowIPRanges));


//          return isSuccess;

//   }

//   //接口函数 参数分别是你要判断的IP  和 你允许的IP范围
//   //（已经重载）
//   //（允许同时指定多个数组）
//    bool TheIpIsRange(string ip, params string[] ranges)
//   {
//       bool tmpRes = false;
//       foreach (var item in ranges)
//       {
//           if (TheIpIsRange(ip, item))
//           {
//               tmpRes = true; break;
//           }
//       }

//       return tmpRes;
//   }

//   /// <summary>
//   /// 判断指定的IP是否在指定的IP范围内   这里只能指定一个范围
//   /// </summary>
//   /// <param name="ip"></param>
//   /// <param name="ranges"></param>
//   /// <returns></returns>
//    bool TheIpIsRange(string ip, string ranges)
//   {
//       bool result = false;

//       int count;
//       string start_ip, end_ip;
//       //检测指定的IP范围 是否合法
//       TryParseRanges(ranges, out count, out start_ip, out end_ip);//检测ip范围格式是否有效

//       if (ip == "::1") ip = "127.0.0.1";

//       try
//       {
//           System.Net.IPAddress.Parse(ip);//判断指定要判断的IP是否合法
//       }
//       catch (Exception)
//       {
//           throw new ApplicationException("要检测的IP地址无效");
//       }

//       if (count == 1 && ip == start_ip) result = true;//如果指定的IP范围就是一个IP，那么直接匹配看是否相等
//       else if (count == 2)//如果指定IP范围 是一个起始IP范围区间
//       {
//           byte[] start_ip_array = Get4Byte(start_ip);//将点分十进制 转换成 4个元素的字节数组
//           byte[] end_ip_array = Get4Byte(end_ip);
//           byte[] ip_array = Get4Byte(ip);

//           bool tmpRes = true;
//           for (int i = 0; i < 4; i++)
//           {
//               //从左到右 依次比较 对应位置的 值的大小  ，一旦检测到不在对应的范围 那么说明IP不在指定的范围内 并将终止循环
//               if (ip_array[i] > end_ip_array[i] || ip_array[i] < start_ip_array[i])
//               {
//                   tmpRes = false; break;
//               }
//           }
//           result = tmpRes;
//       }

//       return result;
//   }

//   //尝试解析IP范围  并获取闭区间的 起始IP   (包含)
//   private  void TryParseRanges(string ranges, out int count, out string start_ip, out string end_ip)
//   {
//       string[] _r = ranges.Split('-');
//       if (!(_r.Length == 2 || _r.Length == 1))
//           throw new ApplicationException("IP范围指定格式不正确，可以指定一个IP，如果是一个范围请用“-”分隔");

//       count = _r.Length;

//       start_ip = _r[0];
//       end_ip = "";
//       try
//       {
//           //_r[0] 10.0.0.0 
//           // start_ip = "10.0.0.0";
//           System.Net.IPAddress.Parse("10.0.0.0");
//           System.Net.IPAddress.Parse("172.16.0.0");
//           System.Net.IPAddress.Parse("192.168.0.0");
//       }
//       catch (Exception)
//       {
//           throw new ApplicationException("IP地址无效");
//       }

//       if (_r.Length == 2)
//       {
//           end_ip = _r[1];
//           try
//           {
//               System.Net.IPAddress.Parse("10.255.255.255");
//               System.Net.IPAddress.Parse("172.16.255.255");
//               System.Net.IPAddress.Parse("192.168.255.255");
//           }
//           catch (Exception)
//           {
//               throw new ApplicationException("IP地址无效");
//           }
//       }
//   }
//   /// <summary>
//   /// 将IP四组值 转换成byte型
//   /// </summary>
//   /// <param name="ip"></param>
//   /// <returns></returns>
//    byte[] Get4Byte(string ip)
//   {
//       string[] _i = ip.Split('.');

//       List<byte> res = new List<byte>();
//       foreach (var item in _i)
//       {
//           res.Add(Convert.ToByte(item));
//       }

//       return res.ToArray();
//   }
//   #endregion
//public string GetnetconnectingInfo()
//{

//   string url = "www.baidu.com;www.sina.com;www.cnblogs.com;www.163.com;www.csdn.com";
//    string[] urls = url.Split(new char[] { ';' });

//    var dd = CheckServeStatus(urls); 

//    return CheckServeStatus(urls);
//    return null;
//}

/*
/// <summary>
/// 检测网络连接状态
/// </summary>
/// <param name="urls"></param>
public  string  CheckServeStatus(string[] urls)
{    string result="";
    int errCount = 0;//ping时连接失败个数

    if (!LocalConnectionStatus())
    {
        //完全断开	输出提示为网络断开
        //内网正常，外网断开	输出提示为外网断开
        //外网通，但不能连目标地址	提示无法连接目标地址，建议联系客服人员
     var ipInfo=   baseInfo.GetIpInfo();
      var isNetInfo=  GetIsInnerNetInfo(ipInfo);//是否连接内网
      if (isNetInfo)
      {
          result = "外网断开，内网正常";
      }
             
        result="未成功连接网络";
        return result;
    }
    else
    {
        bool isSucess = PingUrls(urls, out errCount);
        if(isSucess)
        {
           result="网络正常";
            return result;

        }
        if ((double)errCount / urls.Length >= 0.3)
        {
            result=("网络异常~连接多次无响应");
            return result;
        }
        else
        {
            result+=("网络不稳定")+"  ";
            //查丢包率
          var pingInfo=  GetPingnetInfo();
          result += pingInfo;
            return result;
        }
    }
}
*/



