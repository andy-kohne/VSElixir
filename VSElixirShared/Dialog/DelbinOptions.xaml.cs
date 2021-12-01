using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VSElixir.Dialog
{
    public partial class DelbinOptions
    {
        public bool SolutionLoaded { get; set; }

        public DelbinOptions()
        {
            InitializeComponent();
        }

        public Dictionary<string, bool> Options
        {
            get => new Dictionary<string, bool>
            {
                {  nameof(chkResetIIS), chkResetIIS.IsChecked.GetValueOrDefault() },
                {  nameof(chkKillWebDev), chkKillWebDev.IsChecked.GetValueOrDefault() },
                {  nameof(chkPackages), chkPackages.IsChecked.GetValueOrDefault() },
                {  nameof(chkTempAsp), chkTempAsp.IsChecked.GetValueOrDefault() },
                {  nameof(chkTemp), chkTemp.IsChecked.GetValueOrDefault() },
                {  nameof(chkReflectedSchemas), chkReflectedSchemas.IsChecked.GetValueOrDefault() },
                {  nameof(chkBin), chkBin.IsChecked.GetValueOrDefault() },
                {  nameof(chkWebsiteCache), chkWebsiteCache.IsChecked.GetValueOrDefault() },
                {  nameof(chkBuild), chkBuild.IsChecked.GetValueOrDefault() },
            };
            set
            {
                if (value == null) return;
                if (value.ContainsKey(nameof(chkResetIIS))) chkResetIIS.IsChecked = value[nameof(chkResetIIS)];
                if (value.ContainsKey(nameof(chkKillWebDev))) chkKillWebDev.IsChecked = value[nameof(chkKillWebDev)];
                if (value.ContainsKey(nameof(chkPackages))) chkPackages.IsChecked = value[nameof(chkPackages)];
                if (value.ContainsKey(nameof(chkTempAsp))) chkTempAsp.IsChecked = value[nameof(chkTempAsp)];
                if (value.ContainsKey(nameof(chkTemp))) chkTemp.IsChecked = value[nameof(chkTemp)];
                if (value.ContainsKey(nameof(chkReflectedSchemas))) chkReflectedSchemas.IsChecked = value[nameof(chkReflectedSchemas)];
                if (value.ContainsKey(nameof(chkBin))) chkBin.IsChecked = value[nameof(chkBin)];
                if (value.ContainsKey(nameof(chkWebsiteCache))) chkWebsiteCache.IsChecked = value[nameof(chkWebsiteCache)];
                if (value.ContainsKey(nameof(chkBuild))) chkBuild.IsChecked = value[nameof(chkBuild)];
            }
        }

        private void DelbinOptions_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    DialogResult = false;
                    Hide();
                    break;
                case Key.Enter:
                    delBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
            }
         
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SolutionLoaded) return;

            chkBuild.IsEnabled = false;
            chkBuild.IsChecked = false;

            chkPackages.IsEnabled = false;
            chkPackages.IsChecked = false;

            chkBin.IsEnabled = false;
            chkBin.IsChecked = false;
        }
    }
}
