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
        public List<string> CitrixError
        {
            set { _citrixerror = value; }
            get { return _citrixerror; }
        }
        public List<string>TrustyNormal
        {
            set { _trustynormal = value; }
            get{ return _trustynormal;}
        }
        public List<string> TrustyError
        {
            set { _trustyerror = value; }
            get { return _trustyerror; }
        }
        public List<string> ErrorList
        {
            set { errorlist = value; }
            get { return errorlist; }

        }
        public List<string>CitrixNormal
        {
            set { _citrixnormal = value; }
            get { return _citrixnormal; }
        }
        public List<string>NormalList
        {
            set { _normallist = value; }
            get { return _normallist; }
        }
     

    }
}
