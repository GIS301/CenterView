using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CenterView
{
    class Repair
    {
        private List<string> _networkerror;
        private List<string> _citrixerror;
        private List<string> _citrixnormal;
        private List<string> _trustynormal;
        private List<string> _trustyerror;
        private List<string> errorlist;
       
        private List<string> _normallist;

        /// <summary>
        /// 构造函数初始化
        /// </summary>
        public Repair()
        {
            TrustyStation trust = new TrustyStation();
            TrustyNormal = trust.TrustyStationNormal();
            NormalList = new List<string>();
            NormalList.AddRange(TrustyNormal);
            TrustyError = trust.TrustyStationError();
            ErrorList = new List<string>();
            CitrixNormal = new List<string>();
            CitrixError = new List<string>();
            ErrorList.AddRange(TrustyError);
          if(!new CkCitrix().CheckCitrix())
          {
              CitrixError.Add("Citrix组件未安装");
          }
          else
          {
              CitrixNormal.Add("Citrix组件已安装");
          }
            
        }
        /// <summary>
        /// 网络错误ListL列表
        /// </summary>
        public List<string> NetworkError
        {
            set
            {
                _networkerror = value;
            }
            get
            {
                return _networkerror;
            }
        }
        /// <summary>
        /// Citrix组件检测错误列表
        /// </summary>
        public List<string> CitrixError
        {
            set { _citrixerror = value; }
            get { return _citrixerror; }
        }
        /// <summary>
        /// 授信站点检测正常列表
        /// </summary>
        public List<string>TrustyNormal
        {
            set { _trustynormal = value; }
            get{ return _trustynormal;}
        }
        /// <summary>
        /// 授信站点检测出错列表
        /// </summary>
        public List<string> TrustyError
        {
            set { _trustyerror = value; }
            get { return _trustyerror; }
        }
        /// <summary>
        /// 总的检测错误列表
        /// </summary>
        public List<string> ErrorList
        {
            set { errorlist = value; }
            get { return errorlist; }

        }
        /// <summary>
        /// Citrix检测正常列表
        /// </summary>
        public List<string>CitrixNormal
        {
            set { _citrixnormal = value; }
            get { return _citrixnormal; }
        }
        /// <summary>
        /// 检测正常的总列表
        /// </summary>
        public List<string>NormalList
        {
            set { _normallist = value; }
            get { return _normallist; }
        }
     

    }
}
