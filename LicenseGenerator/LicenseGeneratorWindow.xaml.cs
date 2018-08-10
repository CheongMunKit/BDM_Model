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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LicenseGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LicenseGeneratorWindow : Window
    {
        public LicenseGeneratorWindow()
        {
            InitializeComponent();
        }
        private void Button_GenerateLicense(object sender, RoutedEventArgs e)
        {
            PasswordCheckerDialogBox dialogBox = new PasswordCheckerDialogBox();
            if (dialogBox.ShowDialog() == true)
            {
                if (dialogBox.IsPasswordCorrect)
                {
                    Model.Protection.LicenseManager.CreateLicense();
                }
                MessageBox.Show("License Generated Successfully");
            }
        }
    }
}
