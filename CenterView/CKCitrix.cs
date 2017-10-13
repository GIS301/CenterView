using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CenterView
{
    public class CkCitrix
    {
        //检测citrix是否存在
        public static bool CheckCitrix()
        {
            List<string> subkeyNames = new List<string>();
            //当前用户 
            RegistryKey CurrentUserhkml = Registry.CurrentUser;
            RegistryKey CurrentUsersoftware = CurrentUserhkml.OpenSubKey("SOFTWARE");
            subkeyNames = CurrentUsersoftware.GetSubKeyNames().ToList();
            if (subkeyNames.Contains("Citrix"))
            {
                CurrentUserhkml.Close();
                return true;
            }
            else
            {
                CurrentUserhkml.Close();
            }
            //本机安装
            RegistryKey LocalMachinehkml = Registry.LocalMachine;
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
    }
}
