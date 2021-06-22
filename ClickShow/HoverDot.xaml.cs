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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClickShow
{
    /// <summary>
    /// Interaction logic for HoverDot.xaml
    /// </summary>
    public partial class HoverDot : Window
    {
        public HoverDot()
        {
            InitializeComponent();

            SourceInitialized += OnSourceInitialized;
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            WindowHelper.SetWindowExTransparent(new WindowInteropHelper(this).Handle);
        }
    }
}
