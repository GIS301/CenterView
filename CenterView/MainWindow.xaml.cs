using CenterView.Model;
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
        NetworkCheck _netCheck = new NetworkCheck();//网络监测类

        DispatcherTimer timer_checkingTrusty;
        DispatcherTimer timer_checkingNetwork;
        DispatcherTimer timer_checkingCitrix;
        DispatcherTimer timer_identifyTime;
        DispatcherTimer timer_checkExistCitrix;//定义修复Citrix是否完成的定时器
        //定义一个计时器监听三个检测是不是都检测完毕了
        DispatcherTimer timer_checkingOver;
        DispatcherTimer timer_networkSpeed;//定义测试网速的Timer
        DispatcherTimer timer_progress ;//定义修复进度条Timer
        DateTime Last;
        int a = 0;//定义正在检测过程序号
        int checkingTrustyCount = 0;//定义正在检测授信站点项目数
        string[] trustyStations;
        int checkboxCount = 0;//定义选择checkbox的数量；
        int SuccessedCount = 0;//定义完成检测模块的数量；
        HardWareInfo hardwareInfo = new HardWareInfo();
        bool checkingStatus;//定义是在检测过程还是在开始检测准备阶段；
        bool checkSuccessTrusty, checkSuccessCitrix, checkSuccessNetwork;//定义三种检测的状态是否完成？
        string checkovertime = "";//检测消耗的时间；
        bool checkedOver = false;//扫描完成
        bool repairReady = false;//准备修复
        List<string> networkError;//记录网络错误列表
        string networkNormal="";//记录网络正常
        public MainWindow()
        {
            InitializeComponent();

            

            timer_checkingTrusty = new DispatcherTimer();
            timer_checkingNetwork = new DispatcherTimer();
            timer_checkingCitrix = new DispatcherTimer();
            timer_identifyTime = new DispatcherTimer();
            timer_checkingOver = new DispatcherTimer();
            timer_networkSpeed = new DispatcherTimer();
            timer_checkExistCitrix = new DispatcherTimer();
            checkSuccessTrusty = checkSuccessCitrix = false;
            checkSuccessNetwork = false;
            hardwareInfo = new BaseInfo().GetAllBaseInfos();
            this.DataContext = hardwareInfo;
            monitor = new NetworkMonitor.NetworkMonitor();
            networkError = new List<string>();
            currentAdapter = _netCheck.getCurrentAdapter();

            // timer_checkingCitrix.Tick += new EventHandler(Tick_checkingCitrix);
          //  timer_checkingNetwork.Tick += new EventHandler(Tick_checkingNetwork);
            //  timer_checkingTrusty.Tick += new EventHandler(Tick_checkingTrusty);
            trustyStations = new TrustyStation().TrustWebsite();
            checkingTrustyCount = trustyStations.Length;
            checkingStatus = false;

      
        }
        void Tick_checkingOver(object sender, EventArgs e)
        {
            if (checkSuccessTrusty && checkSuccessCitrix && checkSuccessNetwork)
            {
                if (TrustCheckBox.IsChecked == true)
                {
                    List<string> trustyError = new Repair().TrustyError;
                    List<string> trustNorml = new Repair().TrustyNormal;
                    foreach (string str in trustyError)
                    {
                        this.ErrorList.Items.Add("授信站点：" + "“" + str + "”" + "有问题");

                    }
                    foreach (string mal in trustNorml)
                    {
                        this.NormalList.Items.Add("授信站点：" + "“" + mal + "”" + "正常");
                    }
                }
                if (CitrixCheckBox.IsChecked == true)
                {
                    List<string> citrixError = new Repair().CitrixError;
                    List<string> citrixNormal = new Repair().CitrixNormal;
                    foreach (string s in citrixError)
                    {
                        this.ErrorList.Items.Add(s);
                    }
                    foreach (string mal in citrixNormal)
                    {
                        this.NormalList.Items.Add(mal);
                    }
                }
                if (NetworkCheckBox.IsChecked == true)
                {
                    if(networkError.Count==0)
                    {
                        this.NormalList.Items.Add(networkNormal);
                    }
                    else
                    {
                        foreach( string error in networkError)
                        {
                            this.ErrorList.Items.Add(error);
                        }
                    }

                }

                //当扫描完成，获取问题


                checkedOver = true;
                hardwareInfo.NoProblemCount = hardwareInfo.CheckingCount - hardwareInfo.CheckingErrorCount;
                CheckOverTab.Focus();
                timer_identifyTime.Stop();
                timer_checkingOver.Stop();
                checkedOvertimeTxt.Text = checkovertime;



            }
        }
        void Tick_identifyTimer(object sender, EventArgs e)
        {
            this.checkingTimeTxt.Text = (DateTime.Now - Last).Minutes.ToString("") + ":" + (DateTime.Now - Last).Seconds.ToString();
            checkovertime = checkingTimeTxt.Text;
            IdentifySuccessed();
        }
        /// <summary>
        /// 正在检测Citrix组件Timer处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingCitrix(object sender, EventArgs e)
        {
            bool result = CkCitrix.CheckCitrix();
            if (result)
            {
                this.checkingCitrix_Txt1.Text = "Citrix组件";
                this.checkingCitrixResult_Txt1.Text = "已安装";
                this.checkingCitrixResult_Txt1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                timer_checkingCitrix.Stop();//Citrix组件检测只有一个，所以检测完后立刻关闭计时器
                checkSuccessCitrix = true;
                hardwareInfo.CheckingCount++;
                SuccessedCount++;
            }
            else
            {
                this.checkingCitrix_Txt1.Text = "Citrix组件";
                this.checkingCitrixResult_Txt1.Text = "未安装";
                this.checkingCitrixResult_Txt1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                hardwareInfo.CheckingCount++;
                hardwareInfo.CheckingErrorCount++;
                timer_checkingCitrix.Stop();
                checkSuccessCitrix = true;
                SuccessedCount++;

            }

        }

        /// <summary>
        /// 正在检测网络连通Timer处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private NetworkMonitor.NetworkMonitor monitor;
        private NetworkMonitor.NetworkAdapter currentAdapter;
        private string losebagInfo = "";//丢包信息
        void Tick_checkingNetwork(object sender, EventArgs e)
        {
            this.checkingNetworkTxt.Text = "正在检查您的网络";

            IList<string> retlist = new List<string>();

            retlist = _netCheck.CheckAllNetStatus();
            
           if(retlist.Count>1)//网络慢
           {
               timer_checkingNetwork.Interval = TimeSpan.FromSeconds(30);
               checkingNetworkStatusTxt.Text = retlist[0];
               losebagResultTxt.Text = retlist[1];
              
              // networkError.Add(retlist[1]);
               losebagInfo = retlist[1];
               monitor.StopMonitoring();
               monitor.StartMonitoring(currentAdapter);

               timer_networkSpeed.Tick += new EventHandler(Tick_networkSpeed);
               timer_networkSpeed.Interval = TimeSpan.FromSeconds(1.0);
               timer_networkSpeed.Start();
              
               
           }
           else if(retlist.Count==0)//网络正常
           {
               checkingNetworkStatusTxt.Text = "网络正常";
               networkNormal = "网络正常";
               checkingNetworkStatusTxt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
               hardwareInfo.CheckingCount++;
               SuccessedCount++;
               checkSuccessNetwork = true;

           }

           else
           {
               this.checkingNetworkStatusTxt.Text = retlist[0];
               networkError.Add(retlist[0]);
               checkingNetworkStatusTxt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
               checkSuccessNetwork = true;
               hardwareInfo.CheckingCount++;
               SuccessedCount++;
               timer_checkingNetwork.Stop();
           }

            //if (this._netCheck.MinInnerSpeed == 0.0)
            //    _netCheck.MinInnerSpeed = _netCheck.CurAdapter.DownloadSpeedKbps;
            //_netCheck.MaxInnerSpeed = _netCheck.CurAdapter.DownloadSpeedKbps > _netCheck.MaxInnerSpeed ? _netCheck.CurAdapter.DownloadSpeedKbps : _netCheck.MaxInnerSpeed;
            //_netCheck.MinInnerSpeed = _netCheck.CurAdapter.DownloadSpeedKbps < _netCheck.MaxInnerSpeed ? _netCheck.CurAdapter.DownloadSpeedKbps : _netCheck.MaxInnerSpeed;
           //_netCheck.MonitorNetSpeed();

         
        }
        private int times = 0;//循环次数 30次
        private double MindownloadSpeed = 0;
        private double MaxdowinloadSpeed = 0;
        void Tick_networkSpeed(object sender,EventArgs e)
        { 
            if(times==1)
            {
                MindownloadSpeed=MaxdowinloadSpeed=currentAdapter.DownloadSpeedKbps;
            }
            else{
                MaxdowinloadSpeed = currentAdapter.DownloadSpeedKbps > MaxdowinloadSpeed ? currentAdapter.DownloadSpeedKbps : MaxdowinloadSpeed;
                MindownloadSpeed=currentAdapter.DownloadSpeedKbps<MindownloadSpeed ? currentAdapter.DownloadSpeedKbps :MindownloadSpeed;
            }
            
          
            //this.NetworkSpeedResultTxt.Text = String.Format("{0:n}kbps", currentAdapter.UploadSpeedKbps);
            times++;
            if(times>30)
            {
                timer_networkSpeed.Stop();
                checkSuccessNetwork = true;
                this.NetworkSpeedResultTxt.Text = "最大网速：" + MaxdowinloadSpeed + ";"+"\n"+"最小网速：" + MindownloadSpeed + ";";
                hardwareInfo.CheckingErrorCount++;
                hardwareInfo.CheckingCount++;
                SuccessedCount++;
                networkError.Add(losebagInfo);
                networkError.Add("网速慢，最大网速为：" + MaxdowinloadSpeed+"kb/s");
            }
           
        }
        /// <summary>
        /// 正在检测授信站点Timer事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingTrusty(object sender, EventArgs e)
        {
            if (this.a > checkingTrustyCount - 2)
            {
                hardwareInfo.CheckingErrorCount += new TrustyStation().IdentifyErrorCount();
                timer_checkingTrusty.Stop();
                checkSuccessTrusty = true;
                SuccessedCount++;

            }
            switch (this.a)
            {
                case 0: this.checkingTrusty_Txt1.Text = trustyStations[a];
                    this.checkingTrustyResult_Txt1.Text = IdentifyResult(trustyStations[a]);
                    this.checkingTrustyResult_Txt1.Foreground = IdentifyColor(trustyStations[a]);
                    this.a++;
                    hardwareInfo.CheckingCount++;
                    break;
                case 1: this.checkingTrusty_Txt1.Text = trustyStations[a - 1];
                    this.checkingTrustyResult_Txt1.Text = IdentifyResult(trustyStations[a - 1]);
                    this.checkingTrustyResult_Txt1.Foreground = IdentifyColor(trustyStations[a - 1]);
                    this.checkingTrusty_Txt2.Text = trustyStations[a];
                    this.checkingTrustyResult_Txt2.Text = IdentifyResult(trustyStations[a]);
                    this.checkingTrustyResult_Txt2.Foreground = IdentifyColor(trustyStations[a]);
                    hardwareInfo.CheckingCount++;
                    this.a++; break;
                default: this.checkingTrusty_Txt1.Text = trustyStations[a - 2];

                    this.checkingTrustyResult_Txt1.Text = IdentifyResult(trustyStations[a - 2]);
                    this.checkingTrustyResult_Txt1.Foreground = IdentifyColor(trustyStations[a - 2]);
                    this.checkingTrusty_Txt2.Text = trustyStations[a - 1];
                    this.checkingTrustyResult_Txt2.Text = IdentifyResult(trustyStations[a - 1]);
                    this.checkingTrustyResult_Txt2.Foreground = IdentifyColor(trustyStations[a - 1]);
                    this.checkingTrusty_Txt3.Text = trustyStations[a];
                    this.checkingTrustyResult_Txt3.Text = IdentifyResult(trustyStations[a]);
                    this.checkingTrustyResult_Txt3.Foreground = IdentifyColor(trustyStations[a]);
                    hardwareInfo.CheckingCount++;
                    this.a++; break;

            }

        }
        /// <summary>
        /// 判断XML内的元素是否在本机授信站列表中存在
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string IdentifyResult(string input)
        {
            string result = "";
            if (new TrustyStation().IdentifyTrusty(input))
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
            if (new TrustyStation().IdentifyTrusty(input))
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
            monitor = new NetworkMonitor.NetworkMonitor();
            currentAdapter = _netCheck.getCurrentAdapter();
            if (CkCitrix.CheckCitrix())
            {
                this.TxtCitrix.Text = "Citrix已安装";
            }
            else
            {
                this.TxtCitrix.Text = "Citrix未安装";
            }
            string[] trustlist = new TrustyStation().GetTrustyStations();
            switch (trustlist.Length)
            {
                case 0:
                    this.Txt_trust1.Text = this.Txt_trust2.Text = this.Txt_trust3.Text = "";
                    break;
                case 1:
                    this.Txt_trust1.Text = trustlist[0];
                    this.Txt_trust2.Text = this.Txt_trust3.Text = "";
                    break;
                case 2:
                    this.Txt_trust1.Text = trustlist[0];
                    this.Txt_trust2.Text = trustlist[1];
                    this.Txt_trust3.Text = "";
                    break;
                default:
                    this.Txt_trust1.Text = trustlist[0];
                    this.Txt_trust2.Text = trustlist[1];
                    this.Txt_trust3.Text = trustlist[2];
                    break;
            }


        }
        /// <summary>
        /// 判断选择的检测项目数是否和完成的检测项目数相同
        /// </summary>
        private void IdentifySuccessed()
        {
            if (SuccessedCount > 0 || checkboxCount > 0)
            {
                if (checkboxCount == SuccessedCount)
                {
                    this.timer_identifyTime.Stop();

                }

            }
        }




        #region 菜单响应
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
                
                
            }
                
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
            InitializedCheckingGrid();
            hardwareInfo.CheckingCount = hardwareInfo.CheckingErrorCount = 0;
            timer_identifyTime.Tick += new EventHandler(Tick_identifyTimer);
            timer_identifyTime.Interval = TimeSpan.FromSeconds(1.0);
            timer_identifyTime.Start();
            timer_checkingOver.Tick += new EventHandler(Tick_checkingOver);
            timer_checkingOver.Interval = TimeSpan.FromSeconds(0.4);
            timer_checkingOver.Start();

            Last = DateTime.Now;
            checkingStatus = true;
            this.CheckingTab.IsSelected = true;
            if (this.TrustCheckBox.IsChecked == false)
            {
                this.CheckingTrustyGrid.Visibility = Visibility.Collapsed;
                checkSuccessTrusty = true;
            }
            else
            {
                this.CheckingTrustyGrid.Visibility = Visibility.Visible;
                checkboxCount++;
                //授信站点检测时间间隔Timer
                timer_checkingTrusty.Tick += new EventHandler(Tick_checkingTrusty);
                timer_checkingTrusty.Interval = TimeSpan.FromSeconds(2.0);
                timer_checkingTrusty.Start();


            }
            if (this.NetworkCheckBox.IsChecked == false)
            {
                this.CheckingNetworkGrid.Visibility = Visibility.Collapsed;
                checkSuccessNetwork = true;
            }
            else 
            {

                this.CheckingNetworkGrid.Visibility = Visibility.Visible;
                checkboxCount++;
                timer_checkingNetwork.Tick += new EventHandler(Tick_checkingNetwork);
                timer_checkingNetwork.Interval = TimeSpan.FromSeconds(1.0);
              
                timer_checkingNetwork.Start();
            }

            if (this.CitrixCheckBox.IsChecked == false)
            {
                checkSuccessCitrix = true;
                this.CheckingCitrixGrid.Visibility = Visibility.Collapsed;
            }
           else
            {
                this.CheckingCitrixGrid.Visibility = Visibility.Visible;
                checkboxCount++;
                //Citrix组件检测时间间隔Timer
                timer_checkingCitrix.Tick += new EventHandler(Tick_checkingCitrix);
                timer_checkingCitrix.Interval = TimeSpan.FromSeconds(2.5);
                timer_checkingCitrix.Start();
            }
        }

        private void CheckingTab_GotFocus(object sender, RoutedEventArgs e)
        {
            checkingStatus = true;
            if (checkedOver)
            {
                CheckOverTab.Focus();
            }


        }
        /// <summary>
        /// 正在检测界面失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckingTab_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void ReadyCheckGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (checkingStatus)
            {
                this.CheckingTab.Focus();
            }


        }
        /// <summary>
        /// 取消扫描按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelScanBtn_Click(object sender, RoutedEventArgs e)
        {
            checkingStatus = false;
            InitializedCheckingGrid();
            this.ReadyCheckGrid.Focus();
        }
        //初始化正在扫面界面的值
        private void InitializedCheckingGrid()
        {
            timer_checkingCitrix.Stop();
            timer_checkingNetwork.Stop();
            timer_checkingTrusty.Stop();
            timer_identifyTime.Stop();
            timer_checkingOver.Stop();
            timer_networkSpeed.Stop();
            timer_checkingTrusty = new DispatcherTimer();
            timer_checkingNetwork = new DispatcherTimer();
            timer_checkingCitrix = new DispatcherTimer();
            timer_identifyTime = new DispatcherTimer();
            timer_checkingOver = new DispatcherTimer();
            timer_networkSpeed = new DispatcherTimer();
            checkboxCount = 0;//定义选择checkbox的数量；
            SuccessedCount = 0;//定义完成检测模块的数量；
            a = 0;
            checkingTimeTxt.Text = checkingTrusty_Txt1.Text
           = checkingTrustyResult_Txt1.Text = checkingTrusty_Txt2.Text
           = checkingTrustyResult_Txt2.Text = checkingTrusty_Txt3.Text
           = checkingTrustyResult_Txt3.Text = checkingCitrix_Txt1.Text
           = checkingCitrixResult_Txt1.Text = checkingTimeTxt.Text =checkingNetworkTxt.Text
           = checkingNetworkStatusTxt .Text= checkingNetworkTxt2.Text=checkingNetworkTxt3.Text=losebagResultTxt.Text=NetworkSpeedResultTxt.Text="";

            checkSuccessTrusty = checkSuccessCitrix = checkSuccessNetwork = false;


        }

        private void CheckOverTab_GotFocus(object sender, RoutedEventArgs e)
        {
            InitializedCheckingGrid();

        }

        private void RescanBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializedCheckingGrid();
            checkingStatus = false;
            checkedOver = false;
            this.ReadyCheckGrid.Focus();
            this.ErrorList.Items.Clear();
            this.NormalList.Items.Clear();
            hardwareInfo.CheckingCount = hardwareInfo.CheckingErrorCount = 0;
        }

        private void RepairBtn_Click(object sender, RoutedEventArgs e)
        {
            this.RepairTab.Focus();
            repairReady = true;
            InitializedCheckingGrid();
           
            repairProgressBar.Maximum = new Repair().CitrixError.Count + new Repair().TrustyError.Count;
            timer_progress = new DispatcherTimer();
            timer_progress.Interval = TimeSpan.FromSeconds(2.0);
            timer_progress.Tick += new EventHandler(Tick_repairProgress);
            timer_progress.Start();

        } 
        private int repairKey = 0;
        private string[] trustyError = new Repair().TrustyError.ToArray();
        void Tick_repairProgress(object sender, EventArgs e)
        {
            repairingErrorTxt.Text = trustyError[repairKey]+"未授信";
            
            new TrustyStation().AddTrustyStation(trustyError[repairKey]);
            repairProgressBar.Value = repairKey;
            repairKey++;
            if(repairKey>trustyError.Length-1&&new Repair().CitrixError.Count>0)
            {
                repairingErrorTxt.Text = "Citrix组件未安装，正在为您安装...";
                new RepairCitrix().CitrixRep();
                repairProgressBar.Value = repairKey;
                this.timer_progress.Stop();  
                
            }

            
        }

        private void Q1_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = System.Windows.Forms. Application.StartupPath + "//XPSdocument//Q1.xps";
            XPSWindow win = new XPSWindow();
            win.documentPath = path;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
        }

        private void Q2_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "//XPSdocument//Q2.xps";
            XPSWindow win = new XPSWindow();
            win.documentPath = path;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
        }

        private void Q3_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "//XPSdocument//Q3.xps";
            XPSWindow win = new XPSWindow();
            win.documentPath = path;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
        }

        private void Q4_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "//XPSdocument//Q4.xps";
            XPSWindow win = new XPSWindow();
            win.documentPath = path;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
        }

        private void Q5_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "//XPSdocument//Q5.xps";
            XPSWindow win = new XPSWindow();
            win.documentPath = path;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
        }

        private void Q6_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "//XPSdocument//Q6.xps";
            XPSWindow win = new XPSWindow();
            win.documentPath = path;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
        }
       




    }
}
