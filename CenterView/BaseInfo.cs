using CenterView.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace CenterView
{
    class BaseInfo
    {
        public HardWareInfo hinfos = new HardWareInfo();
        public enum WMIPath
        {
            #region MyRegion

            // 硬件
            Win32_Processor,     // CPU 处理器
            Win32_PhysicalMemory,  // 物理内存条
            Win32_Keyboard,     // 键盘
            Win32_PointingDevice,  // 点输入设备，包括鼠标。
            Win32_FloppyDrive,    // 软盘驱动器
            Win32_DiskDrive,     // 硬盘驱动器
            Win32_CDROMDrive,    // 光盘驱动器
            Win32_BaseBoard,     // 主板
            Win32_BIOS,       // BIOS 芯片
            Win32_ParallelPort,   // 并口
            Win32_SerialPort,    // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice,    // 多媒体设置，一般指声卡。
            Win32_SystemSlot,    // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController,   // USB 控制器
            Win32_NetworkAdapter,  // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer,      // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob,     // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem,     // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor,  // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings,  // 显卡支持的显示模式。
            // 操作系统
            Win32_TimeZone,     // 时区
            Win32_SystemDriver,   // 驱动程序
            Win32_DiskPartition,  // 磁盘分区
            Win32_LogicalDisk,   // 逻辑磁盘
            Win32_LogicalDiskToPartition,   // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile,     // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem,  // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand,  // 系统自动启动程序
            Win32_Service,     // 系统安装的服务
            Win32_Group,      // 系统管理组
            Win32_GroupUser,    // 系统组帐号
            Win32_UserAccount,   // 用户帐号
            Win32_Process,     // 系统进程
            Win32_Thread,      // 系统线程
            Win32_Share,      // 共享
            Win32_NetworkClient,  // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            #endregion
        }

        /// <summary>
        /// 获取所有硬件信息
        /// </summary>
        public void GetAllBaseInfos()
        {
            hinfos.OSystem = GetOsInfo();
            //...补充其他的硬件
        }

        /// <summary>
        /// 电脑型号
        /// </summary>
        /*  public string GetVersions()
          {
              var result = "";
              try
              {

                  ManagementClass hardDisk = new ManagementClass("Win32_ComputerSystemProduct");
                  ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                  foreach (ManagementObject m in hardDiskC)
                  {

                      result += m["ComputerSystemProduct"].ToString();
                      break;
                  }
              }
              catch
              {

              }
              return result;
          }*/
        ///获取电脑制造商信息
        public string GetVersions()
        {
            var result = "";
            // create management class object
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            //collection to store all management objects
            ManagementObjectCollection moc = mc.GetInstances();
            if (moc.Count != 0)
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    // display general system information
                    result += string.Format("\n {0}",
                                      mo["Manufacturer"].ToString());
                }
            }
            //wait for user action
            return result;
        }
        /// <summary>
        /// 获取计算机类型
        /// </summary>
        public enum ChassisTypes
        {
            Other = 1,
            Unknown,
            Desktop,
            LowProfileDesktop,
            PizzaBox,
            MiniTower,
            Tower,
            Portable,
            Laptop,
            Notebook,
            Handheld,
            DockingStation,
            AllInOne,
            SubNotebook,
            SpaceSaving,
            LunchBox,
            MainSystemChassis,
            ExpansionChassis,
            SubChassis,
            BusExpansionChassis,
            PeripheralChassis,
            StorageChassis,
            RackMountChassis,
            SealedCasePC
        }

        public string GetChassisTypes()
        {
            var result = "";
            ManagementClass systemEnclosures = new ManagementClass("Win32_SystemEnclosure");
            foreach (ManagementObject obj in systemEnclosures.GetInstances())
            {
                foreach (int i in (UInt16[])(obj["ChassisTypes"]))
                {
                    if (i > 0 && i < 25)
                    {
                        return ((ChassisTypes)i).ToString();
                    }
                }
            }
            return ChassisTypes.Unknown.ToString();
        }
        /// <summary>
        /// 获取计算机名
        /// </summary>
        /// <returns></returns>
        public string GetMachineName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch (Exception e)
            {
                return "uMnNk";
            }
        }

        /// <summary>
        /// 检测操作系统信息
        /// </summary>
        public string GetOsInfo()
        {
            string result = "";
            try
            {
                ManagementClass mc = new ManagementClass(WMIPath.Win32_OperatingSystem.ToString());
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (var item in moc)
                {


                    var text = item.Properties["Name"].Value + "\r\n";
                    result += text.Split('|')[0];

                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        /// <summary> 
        /// 获取操作系统位数（x32/64） 
        /// </summary> 
        /// <returns>int</returns> 
        public string GetOSBit()
        {
            string result = "";
            try
            {
                ConnectionOptions mConnOption = new ConnectionOptions();
                ManagementScope mMs = new ManagementScope(@"\\localhost", mConnOption);
                ObjectQuery mQuery = new ObjectQuery("select AddressWidth from Win32_Processor");
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher(mMs, mQuery);
                ManagementObjectCollection mObjectCollection = mSearcher.Get();
                foreach (ManagementObject mObject in mObjectCollection)
                {
                    result += mObject["AddressWidth"].ToString() + "位" + "\r\n";
                }
                return result;
            }
            catch
            {

                return null;
            }
        }

        /// <summary>
        /// 检测Cpu信息
        /// </summary>
        /// <returns></returns>
        public string GetCpuInfo()
        {
            string result = "";
            try
            {
                ManagementClass mc = new ManagementClass(WMIPath.Win32_Processor.ToString());
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (var item in moc)
                {
                    result += item.Properties["Name"].Value + "\r\n";

                }
                return result;
            }
            catch
            {
                return null;
            }

        }
        ///检测内存

        public string GetMemoryInfo()
        {
            var result = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象 
            searcher.Query = new SelectQuery("Win32_PhysicalMemory ", "", new string[] { "Capacity" });//设置查询条件 
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            var theresult = (int)(capacity / 1024 / 1024);
            result = theresult.ToString();
            return result;
        }
        /// <summary>
        /// 检测硬盘信息
        /// </summary>


        public string GetDiskDriveInfo()
        {
            //string result = "";
            //System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_DiskDrive");
            //System.Management.ManagementObjectCollection moc = mc.GetInstances();
            //foreach (System.Management.ManagementObject mo in moc)
            //{
            //    if (result == "")
            //    {
            //        try
            //        {
            //            //result += mo["Manufacturer"].ToString()+" ";//制造商
            //            //result += mo["Signature"].ToString() + " ";//签名
            //            result += Convert.ToDouble(mo.Properties["Size"].Value) / (1024 * 1024 * 1024);

            //            break;
            //        }
            //        catch
            //        {
            //        }
            //    }

            //}
            //return result;




            var result = "";
            double Size = 0;
            try
            {
                ManagementClass mc = new ManagementClass(WMIPath.Win32_DiskDrive.ToString());
                ManagementObjectCollection theDiskDriveInfo = mc.GetInstances();
                foreach (var item in theDiskDriveInfo)
                {


                    Size += Convert.ToDouble(item.Properties["Size"].Value) / (1024 * 1024 * 1024);
                }
                if (Size > 1024)
                {
                    result = Math.Round((Size / 1024), 2) + "T";
                }
                else
                {
                    result = Math.Round(Size, 2) + "G";
                }

                return result;
            }
            catch (Exception)
            {

                return null;
            }


        }
        /// <summary>
        /// 显卡 芯片,显存大小
        /// </summary>
        public string GetVideoController()
        {
            var result = "";

            try
            {

                ManagementClass hardDisk = new ManagementClass("Win32_VideoController");
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    result += (m["VideoProcessor"].ToString().Replace("Family", "") + ToGB(Convert.ToInt64(m["AdapterRAM"].ToString()), 1024.0).ToString());
                    break;
                }
            }
            catch
            {

            }
            return result;
        }




        /// 检测主板信息
        public string GetMainBoardInfo()
        {
            var result = "";
            try
            {
                ManagementClass mc = new ManagementClass(WMIPath.Win32_BaseBoard.ToString());
                ManagementObjectCollection moc = mc.GetInstances();
                SelectQuery Query = new SelectQuery("SELECT * FROM Win32_BaseBoard");//查询主板
                ManagementObjectSearcher driveID = new ManagementObjectSearcher(Query);//创建WMI查询对象
                //获取查询结果
                ManagementObjectCollection.ManagementObjectEnumerator data = driveID.Get().GetEnumerator();
                data.MoveNext();//循环读取
                ManagementBaseObject board = data.Current;//获取当前主板

                foreach (var item in moc)
                {

                    result += item.Properties["Manufacturer"].Value.ToString() + " " + item.Properties["SerialNumber"].Value.ToString() + " " + item.Properties["Product"].Value + "\r\n";


                }


                return result;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 检测网络连接信息
        /// </summary>
        public string GetNetworkInfo()
        {
            var result = "";
            try
            {
                ManagementClass mc = new ManagementClass(WMIPath.Win32_NetworkAdapterConfiguration.ToString());
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (var item in moc)
                {

                    result += "IP地址：" + item.Properties["IPAddress"].Value + "\r\n";
                    //result += "默认网关：" + item.Properties[DefaultIPGateway"].Value + "\r\n";
                }
                return result;
            }
            catch
            {
                return null;
            }

        }
        /*/// <summary></summary>   
        /// 显示本机各网卡的详细信息   
        /// <summary></summary>   
        public string[] GetNetworkInterfaceMessage()
        {
            //var result = "";
            string[] result=new string[2];
            NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in fNetworkInterfaces)
            {   
                #region " 网卡类型 "
                string fCardType = "未知网卡";
                string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                if (rk != null)
                {
                    // 区分 PnpInstanceID    
                    // 如果前面有 PCI 就是本机的真实网卡   
                    // MediaSubType 为 01 则是常见网卡，02为无线网卡。   
                    string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                    int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                    if (fPnpInstanceID.Length > 3 &&
                        fPnpInstanceID.Substring(0, 3) == "PCI")
                        fCardType = "物理网卡";
                    else if (fMediaSubType == 1)
                        fCardType = "虚拟网卡";
                    else if (fMediaSubType == 2)
                        fCardType = "无线网卡";
                }
                #endregion
                #region " 网卡信息 "

                IPInterfaceProperties fIPInterfaceProperties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = fIPInterfaceProperties.UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                {
                    if (UnicastIPAddressInformation.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)//AddressFamily.InterNetwork)
                       result[1]=UnicastIPAddressInformation.Address.ToString();// Ip 地址   
                }
                result[0]= adapter.Name;
                    
                #endregion
                   
            }
            return result;
        }   */
        /// <summary>
        /// 获取授信站点列表
        /// </summary>
        /// <returns></returns>
        public string[] GetTrustyStations()
        {
            string[] subkeyNames;
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
            RegistryKey aimdir = software.OpenSubKey(@"Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains", true);
            subkeyNames = aimdir.GetSubKeyNames();
            //判断有没有域名前缀例如“www”
            for (int i = 0; i < subkeyNames.Length; i++)
            {
                RegistryKey temp = aimdir.OpenSubKey(subkeyNames[i], false);
                if (temp.GetSubKeyNames().Length == 1)
                {
                    subkeyNames[i] = temp.GetSubKeyNames()[0] + "." + subkeyNames[i];
                }

            }
            hkml.Close();
            return subkeyNames;
        }

        public string ToGB(double size, double mod)
        {
            String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size) + units[i];
        }
     
    }
}
