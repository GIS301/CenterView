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
        public string GetPingnetInfo()
        {
            string result = "";
            int count = 4;
            IPAddress addr = IPAddress.Parse("14.215.177.39");
            Ping ping = new Ping();
            long timeSum = 0;
            int succCount = 0;
            if (timeSum != 0)
            {
                result += string.Format("丢包率{1}%", 1.0 * timeSum / succCount, (count - succCount) * 100.0 / count);
            }
            else
            {
                result = "丢包率为100%";
            }

            return result;

        }
        //检查网络链接
        public string GetNetConnectInfo()
        {
            string result = "";
            Ping p = new Ping();//创建Ping对象p
            PingReply pr = p.Send("123.145.68.1");//向指定IP或者主机名的计算机发送ICMP协议的ping数据包
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


        //10.0.0.0 - 10.255.255.255
        //172.16.0.0 - 172.31.255.255
        //192.168.0.0 - 192.168.255.255

    }

}
