using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CenterView.Model
{
   public partial class HardWareInfo
    {
        public HardWareInfo()
		{}
		#region Model
		private string _logopath;
		private string _trademark;
		private string _osystem;
		private string _cpu;
		private string _memory;
		private string _harddisk;
		private string _graphicscard;
		private string _mainboard;
		private string _networkcard;
		private string _wifi;
		private string _gateway;
		private string _ip;
		private string _dns;
		/// <summary>
		/// 品牌logo路径
		/// </summary>
		public string LogoPath
		{
			set{ _logopath=value;}
			get{return _logopath;}
		}
		/// <summary>
		/// 品牌型号
		/// </summary>
		public string Trademark
		{
			set{ _trademark=value;}
			get{return _trademark;}
		}
		/// <summary>
		/// 操作系统    
		/// </summary>
		public string OSystem
		{
			set{ _osystem=value;}
			get{return _osystem;}
		}
		/// <summary>
		/// CPU信息
		/// </summary>
		public string CPU
		{
			set{ _cpu=value;}
			get{return _cpu;}
		}
		/// <summary>
		/// 内存信息
		/// </summary>
		public string Memory
		{
			set{ _memory=value;}
			get{return _memory;}
		}
		/// <summary>
		/// 硬件信息
		/// </summary>
		public string HardDisk
		{
			set{ _harddisk=value;}
			get{return _harddisk;}
		}
		/// <summary>
		/// 显卡信息    
		/// </summary>
		public string GraphicsCard
		{
			set{ _graphicscard=value;}
			get{return _graphicscard;}
		}
		/// <summary>
		/// 主板信息    
		/// </summary>
		public string MainBoard
		{
			set{ _mainboard=value;}
			get{return _mainboard;}
		}
		/// <summary>
		/// 网卡信息
		/// </summary>
		public string NetworkCard
		{
			set{ _networkcard=value;}
			get{return _networkcard;}
		}
		/// <summary>
		/// 无线网卡
		/// </summary>
		public string WIFI
		{
			set{ _wifi=value;}
			get{return _wifi;}
		}
		/// <summary>
		/// 网关信息    
		/// </summary>
		public string Gateway
		{
			set{ _gateway=value;}
			get{return _gateway;}
		}
		/// <summary>
		/// IP
		/// </summary>
		public string IP
		{
			set{ _ip=value;}
			get{return _ip;}
		}
		/// <summary>
		/// DNS地址
		/// </summary>
		public string DNS
		{
			set{ _dns=value;}
			get{return _dns;}
		}
		#endregion Model
    }
}
