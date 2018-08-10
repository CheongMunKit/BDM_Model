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

namespace Vision.View.ImagePanel
{
    /// <summary>
    /// Interaction logic for ResizeBox.xaml
    /// </summary>
    public partial class ResizePanel : UserControl
    {
        Action increaseSize;
        Action decreaseSize;
        Action fitSize;
        Action fullSize;
        Action lockSize;
        Action unlockSize;
        Action saveImage;        
        
        public ResizePanel()
        {
            InitializeComponent();
        }

        public void SetResizePanelActions(
            Action IncreaseSize,
            Action DecreaseSize,
            Action FitSize,
            Action FullSize,
            Action LockSize,
            Action UnlockSize,
            Action SaveImage)
        {
            increaseSize = IncreaseSize;
            decreaseSize = DecreaseSize;
            fitSize = FitSize;
            fullSize = FullSize;
            lockSize = LockSize;
            unlockSize = UnlockSize;
            saveImage = SaveImage;
        }

        private void btnInceaseSize_Click(object sender, RoutedEventArgs e)
        {
            increaseSize?.Invoke();
        }

        private void btnDecreaseSize_Click(object sender, RoutedEventArgs e)
        {
            decreaseSize?.Invoke();
        }        

        private void btnFitSize_Click(object sender, RoutedEventArgs e)
        {
            fitSize?.Invoke();
        }

        private void btnFullSize_Click(object sender, RoutedEventArgs e)
        {
            fullSize?.Invoke();
        }

        private void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            saveImage?.Invoke();
        }

        private void TogglebtnLockSize_Checked(object sender, RoutedEventArgs e)
        {
            lockSize?.Invoke();
        }

        private void TogglebtnLockSize_Unchecked(object sender, RoutedEventArgs e)
        {
            unlockSize?.Invoke();
        } 
    }
}
