using System.Windows;
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

        private void DelbinOptions_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Hide();
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
