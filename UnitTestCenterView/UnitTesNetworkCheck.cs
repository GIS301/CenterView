using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterView;

namespace UnitTestCenterView
{
    [TestClass]
    public class UnitTesNetworkCheck
    {
        [TestMethod]
        public void TestCheckNetStatus()
        {
            NetworkCheck nc = new NetworkCheck();
            
            nc.CheckAllNetStatus();
        }
    }
}
