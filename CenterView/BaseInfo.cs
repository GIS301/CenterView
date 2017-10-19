using CenterView.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

        public void GetAllBaseInfos()
        {

            hinfos.LogoPath = "无";
            hinfos.Trademark = GetTrademarkInfo(); //主机  制造商名称+名称+版本名称+类型（笔记本、台式机）（无版本）
            hinfos.OSystem = GetOsInfo();//系统  系统名+版本+位数
            hinfos.CPU = GetCpuInfo();//Cpu  制造商+名字+版本+频率+核心数（无核心数）
            hinfos.Memory = GetMemoryInfo();//内存   制造商+名字+版本+容量大小+转速+串口类型（无制造商 转速 串口类型 ）
            hinfos.HardDisk = GetDiskDriveInfo();//硬盘  制造商+名字+版本+容量大小+转速+串口类型（无制造商 名字 版本 转速）
            hinfos.GraphicsCard = GetGraphicsCardInfo();// 显卡  制造商+名字+版本+显存大小 (无厂商)
            hinfos.MainBoard = GetMainBoardInfo();// 主板 制造商+名字+版本
            hinfos.NetworkCard = ShowNetworkInterfaceMessage();//网卡  制造商+名字+版本+芯片名字
            hinfos.WIFI = "无";//无线网卡  制造商+名字+版本+芯片名字
            hinfos.Gateway = "无";
            hinfos.IP = "无";
            hinfos.DNS = "无";




        }


        /// <summary>
        /// 电脑厂商商标
        /// </summary>
        /// <returns></returns>
        public string GetTrademarkInfo()
        {

            var result = GetManufacturerInfo() + " ";
            result += GetMachineName() + " ";
            result += GetChassisTypes() + " ";
            return result;

        }


        /// <summary>
        /// 电脑型号
        /// </summary>
        public string GetVersions()
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
        }



        ///获取电脑制造商信息
        public string GetManufacturerInfo()
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
        /// 检测操作系统信息与位数（x32/64）
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
                return result + " " + GetOSBit() + " 位";
            }
            catch
            {
                return null;
            }
        }
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
                    result += mObject["AddressWidth"].ToString();
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
        /// <summary>    
        /// 物理内存    
        /// </summary>    
        //public string GetPhysicalMemory()
        //{
        //    string result = "";
        //    ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        //    ManagementObjectCollection thePhysicalMemory = mc.GetInstances();
        //    foreach (var item in thePhysicalMemory)
        //    {
        //        result = item["TotalPhysicalMemory"].ToString();                
        //    }
        //    return result;
        //}  
        /// <summary>
        /// 获取内存信息
        /// </summary>
        /// <returns></returns>
        //public string GetMemoryInfo()
        //{
        //    StringBuilder sr = new StringBuilder();
        //    try
        //    {
        //        long capacity = 0;
        //        var query = WmiDict[WmiType.Win32_PhysicalMemory.ToString()];
        //        int index = 1;
        //        foreach (var obj in query)
        //        {
        //            sr.Append("内存" + index + "频率:" + obj["ConfiguredClockSpeed"] + ";");
        //            capacity += Convert.ToInt64(obj["Capacity"]);
        //            index++;
        //        }
        //        sr.Append("总物理内存:");
        //        sr.Append(capacity / 1024 / 1024 + "MB;");

        //        query = WmiDict[WmiType.Win32_PerfFormattedData_PerfOS_Memory.ToString()];
        //        sr.Append("总可用内存:");
        //        long available = 0;
        //        foreach (var obj in query)
        //        {
        //            available += Convert.ToInt64(obj.Properties["AvailableMBytes"].Value);
        //        }
        //        sr.Append(available + "MB;");
        //        sr.AppendFormat("{0:F2}%可用; ", (double)available / (capacity / 1024 / 1024) * 100);
        //    }
        //    catch (Exception ex)
        //    {
        //        sr.Append("异常信息:" + ex.Message);
        //    }

        //    return sr.ToString();
        //}


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
        public string GetGraphicsCardInfo()
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



        /// <summary>
        /// 获取显卡类型
        /// </summary>
        /// <returns></returns>
        public string GetVideoPNPID()
        {
            string st = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_VideoController");
            foreach (ManagementObject mo in mos.Get())
            {
                st = mo["PNPDeviceID"].ToString();
            }
            return st;
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
        /// 检测网络配置
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


        /// <summary></summary>   
        /// 检测本地网卡
        /// <summary></summary>   
        public string GetNetworkInterfaceInfo()
        {

            string result = "";
            string second = "";
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
                        second = UnicastIPAddressInformation.Address.ToString();// Ip 地址   
                }
                result = adapter.Name;
                result += second;

                #endregion

            }
            return result;
        }
        private List<string> ShowAdapterInfo()
        {
            List<string> lst_NetworkAdapter = new List<string>();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            lst_NetworkAdapter.Add("适配器个数：" + adapters.Length);
            int index = 0;

            foreach (NetworkInterface adapter in adapters)
            {
                index++;
                //显示网络适配器描述信息、名称、类型、速度、MAC 地址   
                lst_NetworkAdapter.Add("---------------------第" + index + "个适配器信息---------------------");
                lst_NetworkAdapter.Add("描述信息：" + adapter.Name);
                lst_NetworkAdapter.Add("类型：" + adapter.NetworkInterfaceType);
                lst_NetworkAdapter.Add("速度：" + adapter.Speed / 1000 / 1000 + "MB");
                lst_NetworkAdapter.Add("MAC 地址：" + adapter.GetPhysicalAddress());

                //获取IPInterfaceProperties实例 

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();

                //获取并显示DNS服务器IP地址信息   
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                if (dnsServers.Count > 0)
                {
                    foreach (var dns in dnsServers)
                    {
                        lst_NetworkAdapter.Add("DNS 服务器IP地址：" + dns + "\n");
                    }
                }
                else
                {
                    lst_NetworkAdapter.Add("DNS 服务器IP地址：" + "\n");
                }
            }
            return lst_NetworkAdapter;

        }

        /// <summary>
        /// 检测有线网卡
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ShowNetworkInterfaceMessage()
        {

            string result = "";
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
                if (adapter.Name == "以太网")
                {
                    //result.Add("-----------------------------------------------------------");
                    //result.Add("-- " + fCardType);
                    //result.Add("-----------------------------------------------------------");
                    //result.Add(string.Format("Id .................. : {0}", adapter.Id)); // 获取网络适配器的标识符   
                    //result.Add(string.Format("Name ................ : {0}", adapter.Name)); // 获取网络适配器的名称               
                    result += (adapter.Description) + "\r\n"; // 获取接口的描述   
                    //result.Add(string.Format("Interface type ...... : {0}", adapter.NetworkInterfaceType)); // 获取接口类型   
                    //result.Add(string.Format("Is receive only...... : {0}", adapter.IsReceiveOnly)); // 获取 Boolean 值，该值指示网络接口是否设置为仅接收数据包。   
                    //result.Add(string.Format("Multicast............ : {0}", adapter.SupportsMulticast)); // 获取 Boolean 值，该值指示是否启用网络接口以接收多路广播数据包。   
                    //result.Add(string.Format("Speed ............... : {0}", adapter.Speed)); // 网络接口的速度   
                    //result.Add(string.Format("Physical Address .... : {0}", adapter.GetPhysicalAddress().ToString())); // MAC 地址   
                    //IPInterfaceProperties fIPInterfaceProperties = adapter.GetIPProperties();
                    //UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = fIPInterfaceProperties.UnicastAddresses;
                    //foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                    //{
                    //    if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    //        result.Add(string.Format("Ip Address .......... : {0}", UnicastIPAddressInformation.Address)); // Ip 地址   
                    //}
                }


                #endregion
            }
            return result;
        }



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
