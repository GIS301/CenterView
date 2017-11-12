using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace CenterView
{
   public class TrustyStation
    {
        /// <summary>
        /// 获取config.xml文件的信任站点列表
        /// </summary>
        /// <returns></returns>
        public string[] TrustWebsite()
        {
            try
            {
                string[] temp = null;
                XmlTextReader reader = new XmlTextReader(System.Windows.Forms.Application.StartupPath + "//config.xml");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "TrustWebsite")
                        {
                            string sumValue = reader.ReadElementContentAsString().Trim();
                            temp = sumValue.Split(',');
                        }
                    }
                }
                return temp;
            }
            catch (Exception e)
            {
                
                throw e;
            }
           
        }

        /// <summary>
        /// 获取授信站点列表
        /// </summary>
        /// <returns></returns>
        public string[] GetTrustyStations()
        {
            try
            {
                List<string> subkeyNames = new List<string>();
                RegistryKey hkml = new BaseInfo().GetCurrenteReg(RegistryHive.CurrentUser);
                RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
                RegistryKey aimdir = software.OpenSubKey(@"Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains", true);
                string[] domains = aimdir.GetSubKeyNames();
                //判断有没有域名前缀例如“www”
                for (int i = 0; i < domains.Length; i++)
                {
                    RegistryKey temp = aimdir.OpenSubKey(domains[i], false);
                    if (temp.GetSubKeyNames().Length == 1)
                    {
                        domains[i] = temp.GetSubKeyNames()[0] + "." + domains[i];
                    }
                    else
                    {
                        domains[i] = "*." + domains[i];
                    }


                }
                subkeyNames.AddRange(domains);
                //判断可信任站点为ip地址时
                RegistryKey range = software.OpenSubKey(@"Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Ranges", true);
                //判断是否有IP地址

                string[] ranges = range.GetSubKeyNames();
                for (int i = 0; i < ranges.Length; i++)
                {
                    RegistryKey rangeTemp = range.OpenSubKey(ranges[i], false);
                    ranges[i] = rangeTemp.GetValue(":Range").ToString();
                }
                subkeyNames.AddRange(ranges);
                hkml.Close();
                return subkeyNames.ToArray();
            }
            catch (Exception e )
            {
                
                throw e;
            }
           
         
            
        }
       /// <summary>
       /// 判断授信站点检测问题数量
       /// </summary>
       /// <returns></returns>
       public int IdentifyErrorCount()
        {
            try
            {
                string[] xmlTrust = TrustWebsite();
                int count = 0;
                for (int i = 0; i < xmlTrust.Length; i++)
                {
                    if (!IdentifyTrusty(xmlTrust[i]))
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception e)
            {
                
                throw e;
            }
          
        }
       /// <summary>
       /// 判定输入值Input是否在本机的授信站点内
       /// </summary>
       /// <param name="input"></param>
       /// <returns></returns>
       public bool IdentifyTrusty(string input)
        {
            try
            {
                string[] localTrusty = GetTrustyStations();
                for (int i = 0; i < localTrusty.Length; i++)
                {
                    if (localTrusty[i].Substring(0, 2) == "*.")
                    {
                        string[] localCut = localTrusty[i].Split('.');
                        string[] trustyCut = input.Split('.');
                        if (trustyCut[trustyCut.Length - 1] == localCut[localCut.Length - 1] && trustyCut[trustyCut.Length - 2] == localCut[localCut.Length - 2])
                        {
                            return true;

                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                int a = Array.IndexOf(localTrusty, input);
                if (a == -1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
           
            }
            catch (Exception e)
            {
                
                throw e;
            }
         
        }
       /// <summary>
       /// 判定授信站点的问题项目列表
       /// </summary>
       /// <param name="error"></param>
       /// <param name="normal"></param>
       public List<string> TrustyStationError()
       {
           try
           {
               string[] trustyXMLlist = TrustWebsite();
               List<string> error = new List<string>();
               for (int i = 0; i < trustyXMLlist.Length; i++)
               {
                   if (!IdentifyTrusty(trustyXMLlist[i]))
                   {
                       error.Add(trustyXMLlist[i]);
                   }

               }
               return error;
           }
           catch (Exception e)
           {
               
               throw e;
           }
          
       }
       /// <summary>
       /// 判断授信站点正常列表
       /// </summary>
       /// <returns></returns>
       public List<string> TrustyStationNormal()
       {
           try
           {
               string[] trustyXMLlist = TrustWebsite();
               List<string> normal = new List<string>();
               for (int i = 0; i < trustyXMLlist.Length; i++)
               {
                   if (IdentifyTrusty(trustyXMLlist[i]))
                   {
                       normal.Add(trustyXMLlist[i]);
                   }

               }
               return normal;
           }
           catch (Exception e )
           {
               
               throw e;
           }
           
       }
       /// <summary>
       /// keyname
       /// </summary>
       /// <param name="input">输入值</param>
       /// <param name="keyname">注册表文件名</param>
       public void AddTrustyStation(string input,string keyname)
       {
           try
           {
               string[] temp = input.Split('.');
               string sum = "";
               RegistryKey hkml = new BaseInfo().GetCurrenteReg(RegistryHive.CurrentUser);
               foreach (string str in temp)
               {
                   sum += str;
               }
               if (Regex.IsMatch(sum, @"^\d+$"))
               {
                   //是数字，即为IP地址
                   string address = @"SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\INTERNET SETTINGS\ZONEMAP\RANGES";
                   RegistryKey key1 = hkml.OpenSubKey(address, true);
                   RegistryKey name1 = key1.CreateSubKey(keyname);
                   name1.SetValue(":Range", input, RegistryValueKind.String);
                   name1.SetValue("http", 0x2, RegistryValueKind.DWord);
               }
               else if (temp[0] == "www")
               {
                   string address = @"SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\INTERNET SETTINGS\ZONEMAP\Domains";
                   RegistryKey key1 = hkml.OpenSubKey(address, true);
                   RegistryKey value = key1.CreateSubKey(input.Substring(4,input.Length-4));
                   RegistryKey www = value.CreateSubKey("www");
                   value.SetValue("https", 0x2, RegistryValueKind.DWord);

               }
               else
               {
                   string address = @"SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\INTERNET SETTINGS\ZONEMAP\Domains";
                   RegistryKey key1 = hkml.OpenSubKey(address, true);
                   RegistryKey value = key1.CreateSubKey(input);
                   value.SetValue("https", 0x2, RegistryValueKind.DWord);
               }
               hkml.Close();

           }
           catch (Exception e)
           {
               
               throw e;
              
           }
          
       }

    }
}
