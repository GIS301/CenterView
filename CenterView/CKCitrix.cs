using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
namespace CenterView
{
    public class CkCitrix
    {
        /// <summary>
        /// 检测citrix是否存在
        /// </summary>
        /// <returns></returns>

        public static bool CheckCitrix()
        {
            string startPath = @"C:\Program Files (x86)\Citrix\ICA Client";
            List<string> subkeyNames = new List<string>();
            //当前用户 
            RegistryKey CurrentUserhkml = new BaseInfo().GetCurrenteReg(RegistryHive.CurrentUser);
            RegistryKey CurrentUsersoftware = CurrentUserhkml.OpenSubKey("SOFTWARE");
            subkeyNames = CurrentUsersoftware.GetSubKeyNames().ToList();
            if (subkeyNames.Contains("Citrix"))
            {
                //注册表存在citrix的值，但可能是卸载残余，通过启动citrix再检测进程确定是否安装
                try
                {
                    if (Directory.Exists(startPath))
                    {
                        CurrentUserhkml.Close();
                        return true;
                    }
                    else
                    {
                        CurrentUserhkml.Close();
                        return false;
                    }
                }
                catch
                { }
            }
            else
            {
                CurrentUserhkml.Close();
                return false;
            }
            //本机安装
            RegistryKey LocalMachinehkml = new BaseInfo().GetCurrenteReg(RegistryHive.LocalMachine);
            RegistryKey LocalMachinesoftware = LocalMachinehkml.OpenSubKey("SOFTWARE");
            subkeyNames = LocalMachinesoftware.GetSubKeyNames().ToList();
            if (subkeyNames.Contains("Citrix"))
            {
                LocalMachinehkml.Close();
                return true;
            }
            else
            {
                LocalMachinehkml.Close();
                return false;
            }
        }

        //检查cx是否在运行
        public static bool IsProcessStarted()
        {
            Process[] temp = Process.GetProcessesByName("wfcrun32");
            if (temp.Length > 0)
                return true;
            else
                return false;
        }
    }
}
