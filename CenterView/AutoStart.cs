using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CenterView
{
    class AutoStart
    {

        ////选择是否开机启动
        //private void checkBox1_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string AppName = "Center View";
        //        string AppFile = Application.ExecutablePath;
        //        if (checkBox1.Checked)
        //        {
        //            // string commonStartup = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        //            //if (AutoStart.Create(commonStartup, AppName, AppFile))
        //            //    MessageBox.Show("添加 全局用户开始菜单启动 成功");
        //            //else
        //            //    MessageBox.Show("添加 全局用户开始菜单启动 失败");
        //            string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        //            if (AutoStart.Create(startup, AppName, AppFile))
        //                MessageBox.Show("添加 当前用户开始菜单启动 成功");
        //            else
        //                MessageBox.Show("添加 当前用户开始菜单启动 失败");
        //        }
        //        else
        //        {
        //            //string commonStartup = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        //            //if (AutoStart.Delete(commonStartup, AppName))
        //            //    MessageBox.Show("删除 全局用户开始菜单启动 成功");
        //            //else
        //            //    MessageBox.Show("删除 全局用户开始菜单启动 失败");
        //            string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        //            if (AutoStart.Delete(startup, AppName))
        //                MessageBox.Show("删除 当前用户开始菜单启动 成功");
        //            else
        //                MessageBox.Show("删除 当前用户开始菜单启动 失败");
        //        }
        //    }
        //    catch 
        //    {

        //    }
        //}

        //开机启动本软件
        public static bool Create(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
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

        public static bool Delete(string directory, string shortcutName)
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
