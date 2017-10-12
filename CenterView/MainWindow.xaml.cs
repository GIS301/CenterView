using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Forms;

namespace CenterView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            menu1.IsOpen = true;
        }

        private void x_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // 下载于www.mycodes.net
        private void ___Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ksl_Click(object sender, RoutedEventArgs e)
        {
            //if (list1.Items.Count > 2)
            //{
            //    return;
            //}
            //list1.Items.Add("扫描c盘中...");
            //list1.Items.Add("扫描d盘中...");
            //list1.Items.Add("扫描e盘中...");
            //list1.Items.Add("扫描d盘中...");
            //list1.Items.Add("扫描g盘中...");
            //list1.Items.Add("扫描l盘中...");
            //list1.Items.Add("扫描i盘中...");
            //list1.Items.Add("扫描j盘中...");
            //list1.Items.Add("扫描k盘中...");
            //list1.Items.Add("扫描完毕");
        }

      
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            //显示citrix插件是否安装
            {
                var isExistCkCitrix = CkCitrix.CheckCitrix();
                if (isExistCkCitrix)
                {
                    citrix.Text = "Ctrix已安装";                  
                }
                else
                {
                    citrix.Text = "Citrix未安装";
                }
            }
        }

    }
}
