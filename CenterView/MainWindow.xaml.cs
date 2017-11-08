using CenterView.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        DispatcherTimer timer_installCitrix;//定义安装Citrxi的定时器
        DispatcherTimer timer_repairTrusty;//定义修复授信站点的计时器；
        DispatcherTimer timer_download;//下载Citrix的计时器；

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
        string networkNormal = "";//记录网络正常


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
            try
            {
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
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
          
            checkingStatus = false;


        }
        /// <summary>
        /// 检测是否扫面完成计时器的Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingOver(object sender, EventArgs e)
        {
            try
            {
                if (checkSuccessTrusty && checkSuccessCitrix && checkSuccessNetwork)
                {
                    if (TrustCheckBox.IsChecked == true)
                    {
                        List<string> trustyError = new Repair().TrustyError;
                        List<string> trustNorml = new Repair().TrustyNormal;
                        foreach (string str in trustyError)
                        {
                            this.ErrorList.Items.Add("站点"+"“" + str + "”" + "未在受信列表中");

                        }
                        foreach (string mal in trustNorml)
                        {
                            this.NormalList.Items.Add("受信站点：" + "“" + mal + "”" + "正常");
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
                        if (networkError.Count == 0)
                        {
                            this.NormalList.Items.Add(networkNormal);
                        }
                        else
                        {
                            foreach (string error in networkError)
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
            catch (Exception ex)
            {

                MessageBox.Show("无法读取注册表，请确定用户级别"); 
            }
           
        }
        void Tick_identifyTimer(object sender, EventArgs e)
        {
            try
            {
                this.checkingTimeTxt.Text = (DateTime.Now - Last).Minutes.ToString("") + ":" + (DateTime.Now - Last).Seconds.ToString();
                checkovertime = checkingTimeTxt.Text;
                IdentifySuccessed();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);//11-1 没问题
            }
           
        }
        /// <summary>
        /// 正在检测Citrix组件Timer处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingCitrix(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
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
            try
            {
                networkError = new List<string>();
                this.checkingNetworkTxt.Text = "正在检查您的网络";

                IList<string> retlist = new List<string>();

                retlist = _netCheck.CheckAllNetStatus();

                if (retlist.Count > 1)//网络慢
                {
                   // timer_checkingNetwork.Interval = TimeSpan.FromSeconds(30);
                    checkingNetworkStatusTxt.Text = retlist[0];
                    losebagResultTxt.Text = retlist[1];
                  
                    // networkError.Add(retlist[1]);
                    losebagInfo = retlist[1];
                    monitor.StopMonitoring();
                    monitor.StartMonitoring(currentAdapter);
                  
                    Thread thread = new Thread(TestSpeed);
                    thread.Start();
                 
                    
                    hardwareInfo.CheckingCount++;
                    hardwareInfo.CheckingErrorCount++;
                    timer_checkingNetwork.Stop();

                }
                else if (retlist.Count == 0)//网络正常
                {
                    checkingNetworkStatusTxt.Text = "网络正常";
                    networkNormal = "网络正常";
                    checkingNetworkStatusTxt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                    hardwareInfo.CheckingCount++;
                    SuccessedCount++;
                    checkSuccessNetwork = true;
                    timer_checkingNetwork.Stop();

                }

                else
                {
                    this.checkingNetworkStatusTxt.Text = retlist[0];
                    networkError.Add(retlist[0]);
                    checkingNetworkStatusTxt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                    checkSuccessNetwork = true;
                    hardwareInfo.CheckingCount++;
                    hardwareInfo.CheckingErrorCount++;
                    SuccessedCount++;
                    timer_checkingNetwork.Stop();
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
            

            //if (this._netCheck.MinInnerSpeed == 0.0)
            //    _netCheck.MinInnerSpeed = _netCheck.CurAdapter.DownloadSpeedKbps;
            //_netCheck.MaxInnerSpeed = _netCheck.CurAdapter.DownloadSpeedKbps > _netCheck.MaxInnerSpeed ? _netCheck.CurAdapter.DownloadSpeedKbps : _netCheck.MaxInnerSpeed;
            //_netCheck.MinInnerSpeed = _netCheck.CurAdapter.DownloadSpeedKbps < _netCheck.MaxInnerSpeed ? _netCheck.CurAdapter.DownloadSpeedKbps : _netCheck.MaxInnerSpeed;
            //_netCheck.MonitorNetSpeed();


        }
        /// <summary>
        /// 测试网速
        /// </summary>
        private void TestSpeed()
        {
            timer_networkSpeed.Tick += new EventHandler(Tick_networkSpeed);
            timer_networkSpeed.Interval = TimeSpan.FromSeconds(1.0);
            timer_networkSpeed.Start();
        }
        private int times = 0;//循环次数 30次
        private double MindownloadSpeed = 0;
        private double MaxdowinloadSpeed = 0;
        void Tick_networkSpeed(object sender, EventArgs e)
        {
            try
            {
                if (times == 1)
                {
                    MindownloadSpeed = MaxdowinloadSpeed = currentAdapter.DownloadSpeedKbps;
                }
                else
                {
                    MaxdowinloadSpeed = currentAdapter.DownloadSpeedKbps > MaxdowinloadSpeed ? currentAdapter.DownloadSpeedKbps : MaxdowinloadSpeed;
                    MindownloadSpeed = currentAdapter.DownloadSpeedKbps < MindownloadSpeed ? currentAdapter.DownloadSpeedKbps : MindownloadSpeed;
                }


                //this.NetworkSpeedResultTxt.Text = String.Format("{0:n}kbps", currentAdapter.UploadSpeedKbps);
                times++;
                if (times > 30)
                {
                    timer_networkSpeed.Stop();
                    checkSuccessNetwork = true;
                    this.NetworkSpeedResultTxt.Text = "最大网速：" + MaxdowinloadSpeed + ";" + "\n" + "最小网速：" + MindownloadSpeed + ";";
                    //hardwareInfo.CheckingCount++;
                    //hardwareInfo.CheckingErrorCount++;
                  
                    SuccessedCount++;
                    networkError.Add(losebagInfo);
                    networkError.Add("网速慢，最大网速为：" + String.Format("{0:F}", MaxdowinloadSpeed) + "kb/s");
                    times = 0;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("测试失败，请确定本机性能监视器是否打开");
                timer_networkSpeed.Stop();

            }
         
        }
        /// <summary>
        /// 正在检测授信站点Timer事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkingTrusty(object sender, EventArgs e)
        {
            try
            {
                if (this.a > checkingTrustyCount - 2)
                {
                    //hardwareInfo.CheckingCount++;//11-1
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
           

        }
        /// <summary>
        /// 判断XML内的元素是否在本机授信站列表中存在
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string IdentifyResult(string input)
        {
            try
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
                return null;
            }
            

        }
        /// <summary>
        /// 判断字体颜色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private SolidColorBrush IdentifyColor(string input)
        {
            try
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
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
            try
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
         

        }
        /// <summary>
        /// 判断选择的检测项目数是否和完成的检测项目数相同
        /// </summary>
        private void IdentifySuccessed()
        {
            try
            {
                if (SuccessedCount > 0 || checkboxCount > 0)
                {
                    if (checkboxCount == SuccessedCount)
                    {
                        this.timer_identifyTime.Stop();

                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
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
            try
            {
                InitializedCheckingGrid();
                hardwareInfo.CheckingCount = hardwareInfo.CheckingErrorCount = 0;
                this.ErrorList.Items.Clear();//11-1
                this.NormalList.Items.Clear();//11-1
              
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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
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
            this.ErrorList.Items.Clear();//11-1
            this.NormalList.Items.Clear();//11-1
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
           = checkingCitrixResult_Txt1.Text = checkingTimeTxt.Text = checkingNetworkTxt.Text
           = checkingNetworkStatusTxt.Text = checkingNetworkTxt2.Text = checkingNetworkTxt3.Text = losebagResultTxt.Text = NetworkSpeedResultTxt.Text = "";
           
            checkSuccessTrusty = checkSuccessCitrix = checkSuccessNetwork = false;
          


        }
        /// <summary>
        /// 扫面完成后“扫描完成界面”Focus事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckOverTab_GotFocus(object sender, RoutedEventArgs e)
        {
            InitializedCheckingGrid();
            if(repairReady)
            {
                this.RepairTab.Focus();
            }
            else
            {
                this.CheckOverTab.Focus();
            }
            if(hardwareInfo.CheckingErrorCount==0)
            {
                RepairBtn.Visibility = Visibility.Hidden; 
            }
            else {
                RepairBtn.Visibility = Visibility.Visible; 
            }

        }
        /// <summary>
        /// 重新扫描按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RescanBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializedCheckingGrid();
            this.ErrorList.Items.Clear();//11-1
            this.NormalList.Items.Clear();//11-1
            checkingStatus = false;
            checkedOver = false;
            this.ReadyCheckGrid.Focus();
            this.ErrorList.Items.Clear();
            this.NormalList.Items.Clear();
            hardwareInfo.CheckingCount = hardwareInfo.CheckingErrorCount = 0;
        }
        /// <summary>
        /// 修复按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepairBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.RepairTab.Focus();
                repairReady = true;
                InitializedCheckingGrid();
                this.ErrorList.Items.Clear();//11-1
                this.NormalList.Items.Clear();//11-1

                if (this.TrustCheckBox.IsChecked == true && this.CitrixCheckBox.IsChecked == true)//授信和组件都选择
                {
                    repairProgressBar.Maximum = new Repair().TrustyError.Count + new Repair().CitrixError.Count;
                    timer_installCitrix = new DispatcherTimer();
                    timer_installCitrix.Interval = TimeSpan.FromSeconds(2.0);
                    timer_installCitrix.Tick += new EventHandler(Tick_installCitrix);
                    timer_installCitrix.Start();
                    timer_checkExistCitrix = new DispatcherTimer();
                    timer_checkExistCitrix.Interval = TimeSpan.FromSeconds(0.5);
                    timer_checkExistCitrix.Tick += new EventHandler(Tick_checkExistCitrix);
                    timer_checkExistCitrix.Start();

                }
                else if (this.TrustCheckBox.IsChecked == true && this.CitrixCheckBox.IsChecked == false)//只选择了授信站点CheckBox
                {
                    repairProgressBar.Maximum = new Repair().TrustyError.Count;
                    timer_repairTrusty = new DispatcherTimer();
                    timer_repairTrusty.Interval = TimeSpan.FromSeconds(1.6);
                    timer_repairTrusty.Tick += new EventHandler(Tick_repairTrusty);
                    timer_repairTrusty.Start();
                }
                else if (this.NetworkCheckBox.IsChecked == true && this.TrustCheckBox.IsChecked == false && this.CitrixCheckBox.IsChecked == false)
                {
                    MessageBox.Show("网络异常问题无法修复，请查看本机网络环境！");
                    InitializedCheckingGrid();
                    this.ErrorList.Items.Clear();//11-1
                    this.NormalList.Items.Clear();//11-1
                    checkingStatus = false;
                    checkedOver = false;
                    this.ReadyCheckGrid.Focus();
                    this.ErrorList.Items.Clear();
                    this.NormalList.Items.Clear();
                    hardwareInfo.CheckingCount = hardwareInfo.CheckingErrorCount = 0;
                }
                else if (this.TrustCheckBox.IsChecked == false && this.CitrixCheckBox.IsChecked == true)//只选择了组件
                {
                    repairProgressBar.Maximum = new Repair().CitrixError.Count;
                    timer_installCitrix = new DispatcherTimer();
                    timer_installCitrix.Interval = TimeSpan.FromSeconds(2.0);
                    timer_installCitrix.Tick += new EventHandler(Tick_installCitrix);
                    timer_installCitrix.Start();
                    timer_checkExistCitrix = new DispatcherTimer();
                    timer_checkExistCitrix.Interval = TimeSpan.FromSeconds(0.5);
                    timer_checkExistCitrix.Tick += new EventHandler(Tick_checkExistCitrix);
                    timer_checkExistCitrix.Start();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
            
           
            //repairProgressBar.Maximum = new Repair().CitrixError.Count + new Repair().TrustyError.Count;
           
            

        }


        /// <summary>
        /// Citrix组件修复是否完成的计时器Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_checkExistCitrix(object sender, EventArgs e)
        {
            try
            {
                if (CkCitrix.CheckCitrix())
                {
                    timer_installCitrix.Stop();
                    this.TxtCitrix.Text = CkCitrix.CheckCitrix() ? "Citrix组件已安装" : "Citrix组件未安装";
                  
                    repairKey++;
                    timer_checkExistCitrix.Stop();
                    if (this.TrustCheckBox.IsChecked == true)
                    {
                        timer_repairTrusty = new DispatcherTimer();
                        timer_repairTrusty.Interval = TimeSpan.FromSeconds(1.6);
                        timer_repairTrusty.Tick += new EventHandler(Tick_repairTrusty);
                        timer_repairTrusty.Start();
                    }
                    else
                    {
                        this.RepairingGrid.Visibility = Visibility.Collapsed;
                        this.repairOverGrid.Visibility = Visibility.Visible;
                        this.repairCitrixOverGrid.Visibility = Visibility.Visible;
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
           
        }
        private int trustRepairIndex = 0;
        void Tick_repairTrusty(object sender,EventArgs e)
        {
            try
            {
                if (trustRepairIndex >= trustyError.Length)
                {
                    timer_repairTrusty.Stop();
                    this.RepairingGrid.Visibility = Visibility.Collapsed;
                    this.repairOverGrid.Visibility = Visibility.Visible;
                    this.repairTrustyOverGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    repairingErrorTxt.Text = trustyError[trustRepairIndex] + "未授信";
                    new TrustyStation().AddTrustyStation(trustyError[trustRepairIndex], trustyError[trustRepairIndex]);
                    repairKey++;
                    trustRepairIndex++;
                    repairProgressBar.Value = repairKey;
                }
            
            
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message); 
            }
            


        }
        private int repairKey = 0;
        private string[] trustyError = new Repair().TrustyError.ToArray();
        /// <summary>
        /// 安装Citrix的Tick事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_installCitrix(object sender, EventArgs e)
        {
            try
            {
                if (this.CitrixCheckBox.IsChecked == true)
                {
                    repairingErrorTxt.Text = "Citrix组件未安装，正在为您安装...";
                    RepairCitrix repair = new RepairCitrix();
                    repair.CitrixRep();
                 
                  
                       repairProgressBar.Value = repairKey;
                       timer_installCitrix.Stop();
                  
                  
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
            //repairingErrorTxt.Text = trustyError[repairKey]+"未授信";

            //new TrustyStation().AddTrustyStation(trustyError[repairKey]);
            //repairProgressBar.Value = repairKey;
            //repairKey++;
            //if(repairKey>trustyError.Length-1&&new Repair().CitrixError.Count>0)
            //{
            //    repairingErrorTxt.Text = "Citrix组件未安装，正在为您安装...";
            //    new RepairCitrix().CitrixRep();
            //    repairProgressBar.Value = repairKey;
            //    this.timer_installCitrix.Stop();  

            //}


        }

        private void Q1_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "//XPSdocument//Q1.xps";
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
        /// <summary>
        /// 修复完成后重新扫描
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repairToScanBtn_Click(object sender, RoutedEventArgs e)
        {
            repairReady = false;
            InitializedCheckingGrid();
            this.ErrorList.Items.Clear();//11-1
            this.NormalList.Items.Clear();//11-1
            checkingStatus = false;
            checkedOver = false;
         
            this.NormalList.Items.Clear();
            hardwareInfo.CheckingCount = hardwareInfo.CheckingErrorCount = 0;
            this.repairProgressBar.Value = 0;
            repairingErrorTxt.Text = "";
            this.repairOverGrid.Visibility = Visibility.Collapsed;
            this.RepairingGrid.Visibility = Visibility.Visible;
            this.ReadyCheckGrid.Focus();
        }
        /// <summary>
        /// 自动启动开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoStarCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                   string AppName = "CenterView";
                  string AppFile = System.Windows.Forms.Application.ExecutablePath;
                    string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                    if (!AutoStart.Create(startup, AppName, AppFile))

                    { MessageBox.Show("添加 当前用户开始菜单启动 失败"); }
                        
              
                //else
                //{
                //    //string commonStartup = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
                //    //if (AutoStart.Delete(commonStartup, AppName))
                //    //    MessageBox.Show("删除 全局用户开始菜单启动 成功");
                //    //else
                //    //    MessageBox.Show("删除 全局用户开始菜单启动 失败");
                //    string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                //    if (AutoStart.Delete(startup, AppName))
                //        MessageBox.Show("删除 当前用户开始菜单启动 成功");
                //    else
                //        MessageBox.Show("删除 当前用户开始菜单启动 失败");
                //}
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 自动启动关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoStarCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                string AppName = "CenterView";
                string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                if (!AutoStart.Delete(startup, AppName))

                { MessageBox.Show("删除 当前用户开始菜单启动 失败"); }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

       
        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       
        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutUsBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("软件名称：CenterView监控"+"\n"+"软件作者：JXUST-GIS");
        }

        private void window_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }





    }
}
