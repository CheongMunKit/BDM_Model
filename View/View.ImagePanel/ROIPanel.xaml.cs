using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for ROIPanel.xaml
    /// </summary>
    public partial class ROIPanel : UserControl
    {
        public ROIPanel()
        {
            InitializeComponent();               
        }

        #region Public Methods

        public void StartDraw(double imageWidth, double imageHeight, ResizeOption resizeOption)
        {      
            WF_ImagePanel.StartDraw(imageWidth, imageHeight, resizeOption);
        }

        public void AddDraggingAction(Action<object, double, double, bool> dragAction)
        {
            WF_ImagePanel.AddDraggingAction(dragAction);
        }
        public void SetDraggingAction(Action<object, double, double, bool> dragAction)
        {
            WF_ImagePanel.SetDraggingAction(dragAction);
        }
        public void ClearDraggingActions(Action<object, double, double, double, double> dragAction)
        {
            WF_ImagePanel.ClearDraggingActions();
        }

        public void AddDrawingAction(Action<object, Graphics, Graphics, Bitmap, double, double, double> paintAction)
        {
            WF_ImagePanel.AddDrawingAction(paintAction);
        }
        public void SetDrawingAction(Action<object, Graphics, Graphics, Bitmap, double, double, double> paintAction)
        {
            WF_ImagePanel.SetDrawingAction(paintAction);
        }
        public void ClearDrawingActions()
        {
            WF_ImagePanel.ClearDrawingActions();
        }

        public void AddMouseLeftClickAction(Action<object, double, double> mouseClickAction)
        {
            WF_ImagePanel.AddMouseLeftClickAction(mouseClickAction);
        }
        public void SetMouseLeftClickAction(Action<object, double, double> mouseDownAction)
        {
            WF_ImagePanel.SetMouseLeftClickAction(mouseDownAction);
        }
        public void ClearMouseLeftClickActions()
        {
            WF_ImagePanel.ClearMouseLeftClickActions();
        }

        public void ClearPreviousImage()
        {
            WF_ImagePanel.ClearPreviousDrawing();
        }
        public void RefreshUI()
        {
            WF_ImagePanel.RefreshUI();
        }


        #endregion Public Methods  
    }
}
