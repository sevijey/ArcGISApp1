using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArcGISApp1
{
    /// <summary>
    /// Lógica de interacción para DocumentViewer.xaml
    /// </summary>
    public partial class DocumentViewer : Window
    {
        public DocumentViewer()
        {
            InitializeComponent();
            webBrowser1.Source = new Uri($"file://{AppDomain.CurrentDomain.BaseDirectory}Resources\\Map_signed.pdf");
        }
    }
}
