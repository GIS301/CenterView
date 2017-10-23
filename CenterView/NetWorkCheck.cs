using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

namespace CenterView
{
    public class NetWorkCheck
    {
        BaseInfo baseInfo = new BaseInfo();

        public string GetnetconnectingInfo()
        {
         
           string url = "www.baidu.com;www.sina.com;www.cnblogs.com;www.google.com;www.163.com;www.csdn.com";
            string[] urls = url.Split(new char[] { ';' });

            var dd = CheckServeStatus(urls); 

            return CheckServeStatus(urls); ;
        }
        
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
                bool  isSucess=  MyPing(urls, out errCount);
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

        #region 网络检测

         private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;

        [System.Runtime.InteropServices.DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

        /// <summary>
        /// 判断本地的连接状态
         /// </summary>
        /// <returns></returns>
        public  bool LocalConnectionStatus()
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

        /// <summary>
        /// Ping命令检测网络是否畅通
        /// </summary>
        /// <param name="urls">URL数据</param>
        /// <param name="errorCount">ping时连接失败个数</param>
        /// <returns></returns>
        public bool  MyPing(string[] urls, out int errorCount)
        {  
             bool isconn = true;
            Ping ping = new Ping();
            errorCount = 0;
            try
            {
                PingReply pr;
                for (int i = 0; i < urls.Length; i++)
                {
                    pr = ping.Send(urls[i]);
                    if (pr.Status != IPStatus.Success)
                    {
                        isconn = false;
                        errorCount++;
                    }
                    Console.WriteLine("Ping " + urls[i] + "    " + pr.Status.ToString());
                }
            }
            catch
            {
                isconn = false;
                errorCount = urls.Length;
            }
            //if (errorCount > 0 && errorCount < 3)
            //  isconn = true;
            return isconn;
        }



        #endregion




        //检查网络链接
        public string GetNetConnectInfo()
        {
            string result = "";
            Ping p = new Ping();//创建Ping对象p
            PingReply pr = p.Send("123.145.68.1");//向指定IP或者主机名的计算机发送ICMP协议的ping数据包，多设置几个ip来检测，只要一个未成功则显示外网断开，
            if (pr.Status == IPStatus.Success)//如果ping成功
            {
                result += ("网络连接成功");

            }
            else
            {
                int times = 0;//重新连接次数;
                do
                {
                    if (times >= 12)
                    {
                        result += ("重新尝试连接超过12次,连接失败程序结束") + " ";

                    }
                    Thread.Sleep(600000);//等待十分钟(方便测试的话，你可以改为1000)
                    pr = p.Send("123.145.68.1");
                    result += (pr.Status);
                    times++;
                }
                while (pr.Status != IPStatus.Success);
                result += ("连接成功");

                times = 0;//连接成功，重新连接次数清为0;
            }
            return result;

     }

        //检测丢包率
        public string GetPingnetInfo( )
        {
            string result;
            int count = 4;
            IPAddress addr = IPAddress.Parse("14.215.177.39");
            Ping ping = new Ping();
            long timeSum = 0;
            int succCount = 0;
            //发送
         

            for (int i = 0; i < count; ++i)
            {
                PingReply pr = ping.Send(addr);
                if (pr.Status == IPStatus.TimedOut)
                {
                    result=("超时");
                  
                }
                else if (pr.Status == IPStatus.Success)
                {
                    result=string.Format("延时：{0}毫秒", pr.RoundtripTime);
                    
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
               result=("丢包率100%");
              
            }
            return result;
        }

        #region
        //定义允许的IP端，格式如下
        //"10.0.0.0 - 10.255.255.255", "172.16.0.0 - 172.31.255.255", "192.168.0.0 - 192.168.255.255"

         string[] AllowIPRanges = { "10.0.0.0 - 10.255.255.255", "172.16.0.0 - 172.31.255.255", "192.168.0.0 - 192.168.255.255" };

        //
        public bool GetIsInnerNetInfo(string ip)
        {

            //判断192.168.100.0这个ip是否在指定的IP范围段内
            //就这个范围而言，如果把IP转换成long型的 那么192.167.0.0这个IP 将在10.0.0.0-10.255.255.255这个范围内，但实际上这是错误的。还希望高手指点将ip转换为long的内幕
               bool isSuccess;
               isSuccess = (TheIpIsRange(ip, AllowIPRanges));


               return isSuccess;
           
        }

        //接口函数 参数分别是你要判断的IP  和 你允许的IP范围
        //（已经重载）
        //（允许同时指定多个数组）
         bool TheIpIsRange(string ip, params string[] ranges)
        {
            bool tmpRes = false;
            foreach (var item in ranges)
            {
                if (TheIpIsRange(ip, item))
                {
                    tmpRes = true; break;
                }
            }

            return tmpRes;
        }

        /// <summary>
        /// 判断指定的IP是否在指定的IP范围内   这里只能指定一个范围
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="ranges"></param>
        /// <returns></returns>
         bool TheIpIsRange(string ip, string ranges)
        {
            bool result = false;

            int count;
            string start_ip, end_ip;
            //检测指定的IP范围 是否合法
            TryParseRanges(ranges, out count, out start_ip, out end_ip);//检测ip范围格式是否有效

            if (ip == "::1") ip = "127.0.0.1";

            try
            {
                System.Net.IPAddress.Parse(ip);//判断指定要判断的IP是否合法
            }
            catch (Exception)
            {
                throw new ApplicationException("要检测的IP地址无效");
            }

            if (count == 1 && ip == start_ip) result = true;//如果指定的IP范围就是一个IP，那么直接匹配看是否相等
            else if (count == 2)//如果指定IP范围 是一个起始IP范围区间
            {
                byte[] start_ip_array = Get4Byte(start_ip);//将点分十进制 转换成 4个元素的字节数组
                byte[] end_ip_array = Get4Byte(end_ip);
                byte[] ip_array = Get4Byte(ip);

                bool tmpRes = true;
                for (int i = 0; i < 4; i++)
                {
                    //从左到右 依次比较 对应位置的 值的大小  ，一旦检测到不在对应的范围 那么说明IP不在指定的范围内 并将终止循环
                    if (ip_array[i] > end_ip_array[i] || ip_array[i] < start_ip_array[i])
                    {
                        tmpRes = false; break;
                    }
                }
                result = tmpRes;
            }

            return result;
        }

        //尝试解析IP范围  并获取闭区间的 起始IP   (包含)
        private  void TryParseRanges(string ranges, out int count, out string start_ip, out string end_ip)
        {
            string[] _r = ranges.Split('-');
            if (!(_r.Length == 2 || _r.Length == 1))
                throw new ApplicationException("IP范围指定格式不正确，可以指定一个IP，如果是一个范围请用“-”分隔");

            count = _r.Length;

            start_ip = _r[0];
            end_ip = "";
            try
            {
                //_r[0] 10.0.0.0 
                // start_ip = "10.0.0.0";
                System.Net.IPAddress.Parse("10.0.0.0");
                System.Net.IPAddress.Parse("172.16.0.0");
                System.Net.IPAddress.Parse("192.168.0.0");
            }
            catch (Exception)
            {
                throw new ApplicationException("IP地址无效");
            }

            if (_r.Length == 2)
            {
                end_ip = _r[1];
                try
                {
                    System.Net.IPAddress.Parse("10.255.255.255");
                    System.Net.IPAddress.Parse("172.16.255.255");
                    System.Net.IPAddress.Parse("192.168.255.255");
                }
                catch (Exception)
                {
                    throw new ApplicationException("IP地址无效");
                }
            }
        }
        /// <summary>
        /// 将IP四组值 转换成byte型
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
         byte[] Get4Byte(string ip)
        {
            string[] _i = ip.Split('.');

            List<byte> res = new List<byte>();
            foreach (var item in _i)
            {
                res.Add(Convert.ToByte(item));
            }

            return res.ToArray();
        }
        #endregion

    }

}
