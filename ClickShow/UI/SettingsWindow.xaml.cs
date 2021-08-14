using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClickShow.Settings;
using Button = System.Windows.Controls.Button;

namespace ClickShow.UI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly AppSetting _settings;

        public SettingsWindow(AppSetting appSetting)
        {
            _settings = appSetting;
            InitializeComponent();

            this.DataContext = appSetting;
        }

        

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnRestoreDefault_OnClick(object sender, RoutedEventArgs e)
        {
            var defaultSetting = new AppSetting();

            _settings.IndicatorSize = defaultSetting.IndicatorSize;
            foreach (var key in _settings.MouseButtonSettings.Keys.ToList())
            {
                _settings.MouseButtonSettings[key].IsEnabled = defaultSetting.MouseButtonSettings[key].IsEnabled;
                _settings.MouseButtonSettings[key].Color = defaultSetting.MouseButtonSettings[key].Color;
            }

            _settings.HoverDotSize = defaultSetting.HoverDotSize;
            _settings.HoverDotFill = defaultSetting.HoverDotFill;
        }
    }
}
