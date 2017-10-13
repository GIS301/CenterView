using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CenterView
{
    class TrustyStation
    {
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
    }
}
