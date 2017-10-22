using CenterView.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Windows;


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

        public HardWareInfo GetAllBaseInfos()
        {
            HardWareInfo hinfos = new HardWareInfo();
            hinfos.LogoPath = GetLogoPath();
            hinfos.Trademark = GetTrademarkInfo(); //主机  品牌logo+制造商名称+名称+版本名称+类型（笔记本、台式机）（已完成制造商名称，电脑名称，版本信息，类型）
            hinfos.OSystem = GetOsInfo();//系统  系统名+版本+位数（已完成）
            hinfos.CPU = GetCpuInfo();//Cpu  制造商+名字+版本+频率+核心数（已完成）
            hinfos.Memory = GetMemoryInfo();//内存   制造商+名字+版本+容量大小+转速+串口类型（已完成制造商，内存大小）
            hinfos.HardDisk = GetDiskDriveInfo();//硬盘  制造商+名字+版本+容量大小+转速+串口类型（已完成硬盘大小，名字，版本，串口类型）
            hinfos.GraphicsCard = GetGraphicsCardInfo();// 显卡  制造商+名字+版本+显存大小 (已完成)
            hinfos.MainBoard = GetMainBoardInfo();// 主板 制造商+名字+版本（已完成）
            hinfos.NetworkCard = GetNetworkInterfaceMessage();//网卡  制造商+名字+版本+芯片名字（已完成）
            hinfos.WIFI = GetWIFI();//无线网卡  制造商+名字+版本+芯片名字（已完成）
            hinfos.Gateway = GetGateway();//获取默认网关（已完成）
            hinfos.IP = GetIpInfo();//获取默认IP（已完成）
            hinfos.DNS = GetDNSInfo();//获取所有DNS（已完成）
            return hinfos;


        }

        /// <summary>
        /// 获取当前操作系统的注册表
        /// </summary>
        /// <param name="type">RegistryHive/</param>
        /// <returns></returns>
        public RegistryKey GetCurrenteReg(RegistryHive type)
        {
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(type, RegistryView.Registry64);//RegistryHive.LocalMachine
            else
                localKey = RegistryKey.OpenBaseKey(type, RegistryView.Registry32);//RegistryHive.LocalMachine
            return localKey;
        }
        /// <summary>
        /// 获取主机信息
        /// </summary>
        private string GetLogoPath()
        {
            try
            {
                //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation
                //Manufacturer、Model以及Logo 
                RegistryKey lm = GetCurrenteReg(RegistryHive.LocalMachine);
                RegistryKey software = lm.OpenSubKey("SOFTWARE", true);
                RegistryKey oem = software.OpenSubKey(@"Microsoft\Windows\CurrentVersion\OEMInformation", true);
                hinfos.LogoPath = oem.GetValue("Logo") != null ? oem.GetValue("Logo").ToString() : null;

                hinfos.Trademark = oem.GetValue("Manufacturer") != null ? oem.GetValue("Manufacturer").ToString() : null;
                hinfos.Trademark += oem.GetValue("Model") != null ? oem.GetValue("Model").ToString() : null;
                return hinfos.Trademark;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return "";
            }

        }

        /// 电脑主机信息
        public string GetTrademarkInfo()
        {
            var result = GetManufacturerInfo() + " ";
            result += GetMachineName() + " ";
            result += GetChassisTypes() + " ";
            result += GetVersions() + " ";
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
                    result += string.Format(mo["Manufacturer"].ToString());
                }
            }
            //wait for user action
            return result;
        }
        /// 获取电脑型号(无效)
        public string GetVersions()
        {
            var result = "";
            try
            {

                ManagementClass hardDisk = new ManagementClass("Win32_ComputerSystemProduct");
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    result += string.Format(m["ComputerSystemProduct"].ToString());
                    break;
                }
            }
            catch
            {

            }
            return result;
        }
        /// 获取计算机类型与计算机名
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





        /// 检测操作系统信息与位数（x32/64）
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





        /// 检测Cpu信息
        public string GetCpuInfo()
        {
            string result = "";
            try
            {
                ManagementClass mc = new ManagementClass(WMIPath.Win32_Processor.ToString());
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (var item in moc)
                {
                    result += item.Properties["Name"].Value + " ";
                    int a = Convert.ToInt32(item.Properties["NumberOfCores"].Value);
                    string data;
                    switch (a)
                    {
                        case 1:
                            data = "单";
                            break;
                        case 2:
                            data = "双";
                            break;
                        case 4:
                            data = "四";
                            break;
                        case 8:
                            data = "八";
                            break;
                        case 16:
                            data = "十六";
                            break;
                        default:
                            data = a.ToString();
                            break;

                    }

                    result += data + "核";



                }

                return result;
            }
            catch
            {
                return null;
            }

        }







        ///检测物理内存信息
        public string GetMemoryInfo()
        {
            string result = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象 
            searcher.Query = new SelectQuery("Win32_PhysicalMemory ", "", new string[] { "Capacity", "manufacturer" });//查询内存大小 
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量与厂商 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();
            long capacity = 0;
            string manufacturer = "";
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;

                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        
                        manufacturer = baseObj.Properties["Manufacturer"].Value.ToString();
                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            var theresult = (int)((capacity / 1024 / 1024)) / 1024 + "G;";
            result += manufacturer + "  " + theresult.ToString();
            return result;
        }





        /// 获取硬盘信息
        public string GetDiskDriveInfo()
        {

            var result = "";
            double Size = 0;
            var type = " ";
            string caption = " ";
            try
            {
                ManagementClass mc = new ManagementClass(WMIPath.Win32_DiskDrive.ToString());
                ManagementObjectCollection theDiskDriveInfo = mc.GetInstances();
                foreach (var item in theDiskDriveInfo)
                {
                    caption += item["caption"].ToString() + "   ";//硬盘名称与类型
                    type = item["InterfaceType"].ToString();//硬盘串口类型
                    Size += Convert.ToDouble(item.Properties["Size"].Value) / (1024 * 1024 * 1024);//硬盘大小
                    
                }
                if (Size > 1024)
                {
                    result = Math.Round((Size / 1024), 2) + "T";
                }
                else
                {
                    result = Math.Round(Size, 2) + "G";
                }
                result += " "+caption + "  "  + "  " + type;
                return result;

            }

            catch (Exception)
            {

                return null;
            }

        }






        /// 获取显卡信息
        public string GetGraphicsCardInfo()
        {
            var result = "";
            string Caption = "";
            try
            {

                ManagementClass hardDisk = new ManagementClass("Win32_VideoController");
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    Caption += m["Caption"].ToString()+"";
                    result += (m["Name"].ToString().Replace("Family", "  ") + "(" + ToGB(Convert.ToInt64(m["AdapterRAM"].ToString()), 1024.0).ToString() + ")"+" ");
                     
                   


                }
            }
            catch
            {
            }
            return result;
        }





        /// 获取主板信息
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
                    result += item.Properties["Manufacturer"].Value.ToString() + " " + item.Properties["SerialNumber"].Value.ToString() + " " + item.Properties["Product"].Value + "  ";
                }
                return result;
            }
            catch
            {
                return null;
            }

        }




        ///获取有线网卡
        public static string GetNetworkInterfaceMessage()
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

                    result += (adapter.Description); // 获取接口的描述   

                }
                #endregion
            }
            return result;
        }


        /// 获取无线网卡
        public static string GetWIFI()
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
                if (adapter.Name == "WLAN")
                {

                    result += (adapter.Description) + ""; // 获取接口的描述   

                }

                #endregion
            }
            return result;
        }




        /// 获取默认网关地址
        public string GetGateway()
        {
            string result = "";
            CIpInformation aa = new CIpInformation();

            var dd = aa._m_CurrentAdapterInformationList;
            var ff = dd.m_AdapterInformation;
            result += (ff.m_Gateway);
            return result;

        }




        /// 获取IP地址
        public  string GetIpInfo()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];
            return localaddr.ToString();
        }



        /// 获取DNS地址（若有多个只取一个）
        public string GetDNSInfo()
        {
            string result = "";
            CIpInformation aa = new CIpInformation();

            var dd = aa._m_CurrentAdapterInformationList;
            var ff = dd.m_AdapterInformation;
            result = (ff.m_Dns1) ;
           
            return result;

        }




        /// 网络配置
        public struct ADAPTERINFORM
        {
            public string m_Name;
            public string m_IP;
            public string m_Mask;
            public string m_Gateway;
            public string m_Dns1;
            public string m_Dns2;
            public string m_Physical;
        }
        /// 网络适配器基本信息链表
        public class CAdapterInformationList
        {
            //public CAdapterInformationList	m_LastAdapter;
            public CAdapterInformationList m_NextAdapter;
            public ADAPTERINFORM m_AdapterInformation;
            public static int count;
        }
        #region CIpInformation类
        public class CIpInformation
        {
            public CAdapterInformationList _m_AdapterInformationList;
            public CAdapterInformationList _m_CurrentAdapterInformationList;

            public CAdapterInformationList m_AdapterInformationList
            {
                get
                {
                    return _m_AdapterInformationList;
                }
            }


            public CIpInformation()
            {
                _m_AdapterInformationList = null;
                _m_CurrentAdapterInformationList = null;
                LoadIpConfigInformation();

            }

            public void LoadIpConfigInformation()
            {
                _m_AdapterInformationList = null;
                _m_CurrentAdapterInformationList = null;
                CAdapterInformationList.count = 0;

                //清理链表
                /*
                while(_m_AdapterInformationList != null)
                {
                    _m_CurrentAdapterInformationList = m_AdapterInformationList;
                    _m_AdapterInformationList = _m_AdapterInformationList.m_NextAdapter;

                }*/
                //获取所有网络接口放在adapters中。
                System.Net.NetworkInformation.NetworkInterface[] adapters = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (System.Net.NetworkInformation.NetworkInterface adapter in adapters)
                {
                    //未启用的网络接口不要
                    if (adapter.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up)
                    {
                        continue;
                    }

                    //不是以太网和无线网的网络接口不要
                    if (adapter.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Ethernet &&
                        adapter.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211 &&
                        adapter.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Ppp)
                    {
                        continue;
                    }

                    //虚拟机的网络接口不要
                    if (adapter.Name.IndexOf("VMware") != -1 || adapter.Name.IndexOf("Virtual") != -1)
                    {
                        continue;
                    }

                    CAdapterInformationList currentAdapter = new CAdapterInformationList();
                    //获取适配器名称
                    currentAdapter.m_AdapterInformation.m_Name = adapter.Name;
                    //获取物理地址字节
                    byte[] physicalAddress = adapter.GetPhysicalAddress().GetAddressBytes();
                    if (physicalAddress.Length == 0 || physicalAddress.Length != 6)
                    {
                        currentAdapter.m_AdapterInformation.m_Physical = "";
                    }
                    else
                    {
                        try
                        {

                            currentAdapter.m_AdapterInformation.m_Physical = String.Format("{0:x2}:{1:x2}:{2:x2}:{3:x2}:{4:x2}:{5:x2}",
                                                                                    physicalAddress[0],
                                                                                    physicalAddress[1],
                                                                                    physicalAddress[2],
                                                                                    physicalAddress[3],
                                                                                    physicalAddress[4],
                                                                                    physicalAddress[5]).ToUpper();
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                    //获取IP地址和Mask地址
                    System.Net.NetworkInformation.IPInterfaceProperties ipif = adapter.GetIPProperties();
                    System.Net.NetworkInformation.UnicastIPAddressInformationCollection ipifCollection = ipif.UnicastAddresses;

                    foreach (System.Net.NetworkInformation.UnicastIPAddressInformation ipInformation in ipifCollection)
                    {
                        if (ipInformation.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            currentAdapter.m_AdapterInformation.m_IP = ipInformation.Address.ToString();
                            currentAdapter.m_AdapterInformation.m_Mask = ipInformation.IPv4Mask.ToString();
                        }
                    }

                    //获取网关地址，网关一般只有一个
                    if (ipif.GatewayAddresses != null && ipif.GatewayAddresses.Count != 0)
                    {
                        currentAdapter.m_AdapterInformation.m_Gateway = ((System.Net.NetworkInformation.GatewayIPAddressInformation)(ipif.GatewayAddresses[0])).Address.ToString();
                    }

                    /*
                    foreach (System.Net.NetworkInformation.GatewayIPAddressInformation gwInformation in ipif.GatewayAddresses)
                    {
                        _m_Gateway = gwInformation.Address.ToString();
                    }
                    */

                    //获取DNS地址，DNS地址一般是两个，有可能没有或一个，或超过两个的情况
                    //如果超过两个只取前两个
                    StringBuilder dnsStr = new StringBuilder(30);
                    foreach (System.Net.IPAddress dnsInformation in ipif.DnsAddresses)
                    {
                        dnsStr.Append(dnsInformation.ToString());
                        dnsStr.Append(',');
                    }
                    dnsStr.Remove(dnsStr.Length - 1, 1);
                    string[] dnsArr = dnsStr.ToString().Split(',');
                    #region switch折叠
                    switch (dnsArr.Length)
                    {
                        case 0:
                            {
                                currentAdapter.m_AdapterInformation.m_Dns1 = "";
                                currentAdapter.m_AdapterInformation.m_Dns2 = "";
                                break;
                            }
                        case 1:
                            {
                                currentAdapter.m_AdapterInformation.m_Dns1 = dnsArr[0];
                                currentAdapter.m_AdapterInformation.m_Dns2 = "";
                                break;
                            }
                        case 2:
                            {
                                currentAdapter.m_AdapterInformation.m_Dns1 = dnsArr[0];
                                currentAdapter.m_AdapterInformation.m_Dns2 = dnsArr[1];
                                break;
                            }
                        default:
                            {
                                currentAdapter.m_AdapterInformation.m_Dns1 = dnsArr[0];
                                currentAdapter.m_AdapterInformation.m_Dns2 = dnsArr[1];
                                break;
                            }
                    }

                    if (_m_AdapterInformationList == null)
                    {
                        _m_AdapterInformationList = currentAdapter;
                        ++CAdapterInformationList.count;
                        //_m_AdapterInformationList.CAdapterInformationList = null;
                        _m_AdapterInformationList.m_NextAdapter = null;
                        _m_CurrentAdapterInformationList = _m_AdapterInformationList;
                    }
                    else
                    {
                        _m_CurrentAdapterInformationList.m_NextAdapter = currentAdapter;
                        ++CAdapterInformationList.count;
                        _m_CurrentAdapterInformationList = currentAdapter;
                    }

                    #endregion
                }
            }

        }
        #endregion






        /// 获取授信站点列表
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
