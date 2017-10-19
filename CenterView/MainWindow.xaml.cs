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
using System.Windows.Threading;

namespace CenterView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer_checkingTrusty = new DispatcherTimer();
        DispatcherTimer timer_checkingNetwork = new DispatcherTimer();
        DispatcherTimer timer_checkingCitrix = new DispatcherTimer();
        int a = 0;//定义正在检测过程序号
        int checkingTrustyCount = 0;//定义正在检测授信站点项目数
        string[] trustyStations;

        public MainWindow()
        {
            InitializeComponent();
            timer_checkingCitrix.Tick += new EventHandler(Tick_checkingCitrix);
            timer_checkingNetwork.Tick += new EventHandler(Tick_checkingNetwork);
          //  timer_checkingTrusty.Tick += new EventHandler(Tick_checkingTrusty);
            trustyStations = new TrustyStation().TrustWebsite();
            checkingTrustyCount = trustyStations.Length;
        }
        /// <summary>
        /// 正在检测Citrix组件Timer处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingCitrix(object sender, EventArgs e)
        {
            bool result = new CkCitrix().CheckCitrix();
            if(result)
            {
                this.checkingCitrix_Txt1.Text = "Citrix组件";
                this.checkingCitrixResult_Txt1.Text = "已安装";
                this.checkingCitrixResult_Txt1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
            }
            else
            {
                this.checkingCitrix_Txt1.Text = "Citrix组件";
                this.checkingCitrixResult_Txt1.Text = "未安装";
                this.checkingCitrixResult_Txt1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));

            }
           
        }
        /// <summary>
        /// 正在检测网络连通Timer处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingNetwork(object sender, EventArgs e)
        { 

        }
        /// <summary>
        /// 正在检测授信站点Timer事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingTrusty(object sender, EventArgs e)
        {
          if(this.a>checkingTrustyCount-2)
          {
              timer_checkingTrusty.Stop();
          }
           switch(this.a)
           {
               case 0: this.checkingTrusty_Txt1.Text = trustyStations[a];
                        this.checkingTrustyResult_Txt1.Text=IdentifyResult(trustyStations[a]);
                        this.checkingTrustyResult_Txt1.Foreground = IdentifyColor(trustyStations[a]);
                        this.a++;
                        break;
               case 1: this.checkingTrusty_Txt1.Text = trustyStations[a - 1];
                        this.checkingTrustyResult_Txt1.Text = IdentifyResult(trustyStations[a-1]);
                        this.checkingTrustyResult_Txt1.Foreground = IdentifyColor(trustyStations[a - 1]);
                       this.checkingTrusty_Txt2.Text = trustyStations[a];
                       this.checkingTrustyResult_Txt2.Text = IdentifyResult(trustyStations[a ]);
                       this.checkingTrustyResult_Txt2.Foreground = IdentifyColor(trustyStations[a]);
                       this.a++; break;
               default: this.checkingTrusty_Txt1.Text = trustyStations[a - 2];

                   this.checkingTrustyResult_Txt1.Text = IdentifyResult(trustyStations[a-2]);
                   this.checkingTrustyResult_Txt1.Foreground = IdentifyColor(trustyStations[a-2]);
                   this.checkingTrusty_Txt2.Text = trustyStations[a - 1];
                   this.checkingTrustyResult_Txt2.Text = IdentifyResult(trustyStations[a - 1]);
                   this.checkingTrustyResult_Txt2.Foreground = IdentifyColor(trustyStations[a-1]);
                   this.checkingTrusty_Txt3.Text = trustyStations[a];
                   this.checkingTrustyResult_Txt3.Text = IdentifyResult(trustyStations[a ]);
                   this.checkingTrustyResult_Txt3.Foreground = IdentifyColor(trustyStations[a]);
                   this.a++; break;

           }

        }
        /// <summary>
        /// 判断XML内的元素是否在本机授信站列表中存在
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string IdentifyResult( string input)
        {
            string result = "";
            if(new TrustyStation().IdentifyTrusty(input))
            {
                return "OK";
            }
            else
            {
                return "未授信";
            }

        }
        /// <summary>
        /// 判断字体颜色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private SolidColorBrush IdentifyColor(string input)
        {
            if(new TrustyStation().IdentifyTrusty(input))
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
            }
        }
       

        /// <summary>
        /// Load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    //获取所有主机信息并显示
            //    SetBaseinfos();

            //    //显示citrix插件是否安装
            //    {
            //        var isExistCkCitrix = new CkCitrix.CheckCitrix();
            //        if (isExistCkCitrix)
            //        {
            //            this.TxtCitrix.Text = "Ctrix Receiver已安装";
            //            //判断citrix是否运行
            //            var isWorkCitrix = CkCitrix.IsProcessStarted();
            //            if (isWorkCitrix)
            //            {
            //                TxtCitrix2.Text = "Citrix Receiver正在运行";
            //            }
            //            else
            //            {
            //                TxtCitrix2.Text = "为您启动Citrix Receiver";
            //                try
            //                {
            //                    System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Citrix\\ICA Client\\wfcrun32.exe");//启动cx,软件安装位置用户无法选择
            //                }
            //                catch
            //                {
            //                    TxtCitrix2.Text = "Citrix Receiver启动失败";
            //                }
            //            }
            //        }
            //        else
            //        {
            //            this.TxtCitrix.Text = "Citrix未安装";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //错误提示MessageBox，四个参数
            //    MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            ////测试授信站点获取IP或者域名
            ////TrustyStation TS = new TrustyStation();
            ////TS.GetTrustyStations();
        }

        #region 界面信息绑定
        /// <summary>
        /// 显示所有信息到主界面
        /// </summary>
        private void SetBaseinfos()
        {
            BaseInfo info = new BaseInfo();
            info.GetAllBaseInfos();
            //操作系统类型
            this.TxtOSystem.Text = info.hinfos.OSystem;
            //请补充所有其他信息CPU...
        }
        #endregion


        #region 菜单响应
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
        private void ___Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region 主界面button操作
        private void TrustCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.TrustBGimg.Source = new BitmapImage(new Uri(@"logoIMG/SCheckYes1.png", UriKind.Relative));
        }

        private void TrustCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.TrustBGimg.Source = new BitmapImage(new Uri(@"logoIMG/SCheckNO1.png", UriKind.Relative));
        }

        private void NetworkCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.NetworkBGimg.Source = new BitmapImage(new Uri("logoIMG/NetworkChecked.png", UriKind.Relative));
        }

        private void NetworkCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.NetworkBGimg.Source = new BitmapImage(new Uri("logoIMG/NetworkUnChecked.png", UriKind.Relative));
        }

        private void CitrixCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.CitrixBGimg.Source = new BitmapImage(new Uri("logoIMG/CitrixChecked.png", UriKind.Relative));
        }

        private void CitrixCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.CitrixBGimg.Source = new BitmapImage(new Uri("logoIMG/CitrixUnChecked.png", UriKind.Relative));

        }

       

      
        #endregion

        private void StartScanBtn_Click(object sender, RoutedEventArgs e)
        {
            this.CheckingTab.IsSelected = true;
        }

        private void CheckingTab_GotFocus(object sender, RoutedEventArgs e)
        {   //授信站点检测时间间隔Timer
            timer_checkingTrusty.Tick += new EventHandler(Tick_checkingTrusty);
            timer_checkingTrusty.Interval = TimeSpan.FromSeconds(2.0);
            timer_checkingTrusty.Start();
            //Citrix组件检测时间间隔Timer
            timer_checkingCitrix.Tick += new EventHandler(Tick_checkingCitrix);
            timer_checkingCitrix.Interval = TimeSpan.FromSeconds(2.5);
            timer_checkingCitrix.Start();

           
        }

    }
}
