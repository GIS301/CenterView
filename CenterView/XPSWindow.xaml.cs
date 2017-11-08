using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace CenterView
{
    /// <summary>
    /// XPSWindow.xaml 的交互逻辑
    /// </summary>
    public partial class XPSWindow : Window
    {
        /// <summary>
        /// 文档路径
        /// </summary>
        public string documentPath { set; get; }
        public XPSWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                XpsDocument doc = new XpsDocument(documentPath, System.IO.FileAccess.Read);
                documentViewer.Document = doc.GetFixedDocumentSequence();
            }
            catch (Exception ex)
            {

                MessageBox.Show("历史案例文档丢失");
            }
           
        }
    }
}
