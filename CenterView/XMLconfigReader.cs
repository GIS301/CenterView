using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace CenterView
{
   public class XMLconfigReader
    {
       private double _internetspeed1;
       private double _internetspeed2;
       private string _citrixname;
       private string _citrixuri;

       public XMLconfigReader()
       {
           InternetSpeed1 = getInternetSpeed1();
           InternetSpeed2 = getInternetSpeed2();
           CitrixName = getCitrixName();
           CitrixUrl = getCitrixUrl();
       }
       /// <summary>
       /// 启动桌面云最低网速要求（网速单位kb）
       /// </summary>
       public double InternetSpeed1
       {
           set
           {
               _internetspeed1 = value;
           }
           get {return _internetspeed1;}
       }
       /// <summary>
       /// 启动应用程序最低网速要求（网速单位kb）
       /// </summary>
       public double InternetSpeed2
       {
           set { _internetspeed2 = value; }
           get { return _internetspeed2; }
       }
       /// <summary>
       /// Citrix组件名称
       /// </summary>
       public string CitrixName
       {
           set { _citrixname = value; }
           get { return _citrixname; }
       }
       /// <summary>
       /// Uri下载链接
       /// </summary>
       public string CitrixUrl
       {
           set {  _citrixuri=value;}
           get { return _citrixuri; }
       }
       /// <summary>
       /// 从XML配置文件中读取启动桌面云最低网速要求
       /// </summary>
       private double getInternetSpeed1()
       {
           XmlTextReader reader = new XmlTextReader("config.xml");
           double speed1 = 0;
           while(reader.Read())
           {
               if (reader.NodeType == XmlNodeType.Element)
               {
                   if (reader.Name == "InternetSpeed1")
                   {
                     string value = reader.ReadElementContentAsString();
                    speed1= Convert.ToDouble(value);
                    break;
                   }                  
               }
           }
           return speed1;

       }
       /// <summary>
       /// 从XML配置中读取启动应用程序最低网速要求
       /// </summary>
       /// <returns></returns>
       private double getInternetSpeed2()
       {
           XmlTextReader reader = new XmlTextReader("config.xml");
           double speed2 = 0;
           while (reader.Read())
           {
               if (reader.NodeType == XmlNodeType.Element)
               {
                   if (reader.Name == "InternetSpeed2")
                   {
                       string value = reader.ReadElementContentAsString();
                       speed2 = Convert.ToDouble(value);
                       break;
                   }
               }
           }
           return speed2;
       }
       /// <summary>
       /// 从XML配置文件中获取Citrix组件名称
       /// </summary>
       /// <returns></returns>
       private string getCitrixName()
       {
           XmlTextReader reader = new XmlTextReader("config.xml");
           string citrix = "";
           while (reader.Read())
           {
               if (reader.NodeType == XmlNodeType.Element)
               {
                   if (reader.Name == "CitrixName")
                   {
                       citrix = reader.ReadElementContentAsString();
                       break;
                   }

               }
           }
           return citrix;
       }
       /// <summary>
       /// 从XML配置文件中获取citrix下载地址
       /// </summary>
       /// <returns></returns>
       private string getCitrixUrl()
       {
           XmlTextReader reader = new XmlTextReader("config.xml");
           string url = "";
           while (reader.Read())
           {
               if (reader.NodeType == XmlNodeType.Element)
               {
                   if (reader.Name == "CitrixName")
                   {
                       url = reader.ReadElementContentAsString();
                       break;
                   }

               }
           }
           return url;
       }

       /// <summary>
       /// 根据XML文件名和节点名获取值
       /// </summary>
       /// <param name="fileName">XML文件名</param>
       /// <param name="name">节点名</param>
       /// <returns></returns>
       public static string getValbyName(string fileName, string name)
       {
           XmlTextReader reader = new XmlTextReader(fileName);
           string ret = "";
           while (reader.Read())
           {
               if (reader.NodeType == XmlNodeType.Element)
               {
                   if (reader.Name == name)
                   {
                       ret = reader.ReadElementContentAsString();
                       break;
                   }
               }
           }
           return ret;
       }

    }
}
