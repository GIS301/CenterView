using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterView;

namespace UnitTestCenterView
{
    [TestClass]
    public class UnitTestNetworkCheck
    {
        [TestMethod]
        public void TestCheckNetStatus()
        {
            NetworkCheck nc = new NetworkCheck();
            string ret = nc.CheckNetStatus();
            bool isConn = nc.LocalConnectionStatus();
            ret = nc.GetPingnetInfo();
            nc.MonitorNetSpeed();
        }
    }
}
