using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CenterView
{
    class AutoStart
    {

        

        
        /// <summary>
        /// 开机启动本软件
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="shortcutName"></param>
        /// <param name="targetPath"></param>
        /// <param name="description"></param>
        /// <param name="iconLocation"></param>
        /// <returns></returns>
        public  static bool Create(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //添加引用 Com 中搜索 Windows Script Host Object Model
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
                shortcut.TargetPath = targetPath;//指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
                shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
                shortcut.Description = description;//设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;//设置图标路径
                shortcut.Save();//保存快捷方式

                return true;
            }
            catch
            { }
            return false;
        }
        /// <summary>
        /// 去除开机启动
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="shortcutName"></param>
        /// <returns></returns>
        public  static bool Delete(string directory, string shortcutName)
        {
            try
            {
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }
                return true;
            }
            catch { }
            return false;
        }
    }
}
