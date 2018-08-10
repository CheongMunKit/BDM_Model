using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Model.Protection;

namespace Vision.View.ImagePanel
{
    /// <summary>
    /// Interaction logic for imagePanel.xaml
    /// </summary>
    public partial class ImagePanel : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public ImagePanel()
        {
            InitializeComponent();
            SetResizePanelActions();
            SetPixelCoordinatesActions();
            this.DataContext = this;
        }

        #region Properties  

        int PixelX_ = 0;
        public int PixelX
        {
            get { return PixelX_; }
            set
            {
                PixelX_ = value;
                OnPropertyChanged("PixelX");
            }
        }

        int PixelY_ = 0;
        public int PixelY
        {
            get { return PixelY_; }
            set
            {
                PixelY_ = value;
                OnPropertyChanged("PixelY");
            }
        }

        #endregion Properties

        #region Public Methods    

        #region Draw / Paint

        public void StartDraw(double imageWidth, double imageHeight, ResizeOption resizeOption)
        {
            WF_ImagePanel.StartDraw(imageWidth, imageHeight, resizeOption);
        }

        public void AddDrawingAction(Action<object, Graphics, Graphics, Bitmap, double, double, double> paintAction)
        {
            WF_ImagePanel.AddDrawingAction(paintAction);
        }
        public void SetDrawingAction(Action<object, Graphics, Graphics, Bitmap, double, double, double> paintAction)
        {
            Model.Protection.LicenseManager.CheckLicense();
            WF_ImagePanel.SetDrawingAction(paintAction);
        }
        public void ClearDrawingActions()
        {
            WF_ImagePanel.ClearDrawingActions();
        }

        public void ClearPreviousImage()
        {
            WF_ImagePanel.IsClearGraphics = true;
            WF_ImagePanel.ClearPreviousDrawing();
            CheckIfPreviousImageAlreadyCleared(3000);
        }

        public void RefreshUI()
        {
            WF_ImagePanel.RefreshUI();
        }


        #endregion Draw / Paint

        #region Drag

        public void AddDraggingAction(Action<object, double, double, double, double, bool, MouseEventArgs> dragAction)
        {
            WF_ImagePanel.AddDraggingAction(dragAction);
        }
        public void SetDraggingAction(Action<object, double, double, double, double, bool, MouseEventArgs> dragAction)
        {
            WF_ImagePanel.SetDraggingAction(dragAction);
        }

        public void ClearDraggingActions()
        {
            WF_ImagePanel.ClearDraggingActions();
        }

        #endregion Drag

        #region Mouse Left Down

        public void AddMouseLeftDownAction(Action<object, double, double, MouseEventArgs> mouseLeftDownAction)
        {
            WF_ImagePanel.AddMouseLeftDownAction(mouseLeftDownAction);
        }
        public void SetMouseLeftDownAction(Action<object, double, double, MouseEventArgs> mouseLeftDownAction)
        {
            WF_ImagePanel.SetMouseLeftDownAction(mouseLeftDownAction);
        }
        public void ClearMouseLeftDownActions()
        {
            WF_ImagePanel.ClearMouseLeftDownActions();
        }

        #endregion Mouse Left Down

        #region Mouse Left Up

        public void AddMouseLeftUpAction(Action<object, double, double, MouseEventArgs> mouseLeftUpAction)
        {
            WF_ImagePanel.AddMouseLeftUpAction(mouseLeftUpAction);
        }        

        public void SetMouseLeftUpAction(Action<object, double, double, MouseEventArgs> mouseLeftUpAction)
        {
            WF_ImagePanel.SetMouseLeftUpAction(mouseLeftUpAction);
        }        

        public void ClearMouseLeftUpActions()
        {
            WF_ImagePanel.ClearMouseLeftUpAction();
        }

        #endregion Mouse Left Up

        #region Mouse Cursor

        public System.Drawing.Point GetCurrentMousePosition()
        {
            return new System.Drawing.Point(PixelX, PixelY);
        }

        #endregion Mouse Cursor

        #region Dispose

        public void Dispose()
        {
            ClearPreviousImage();
            ClearDrawingActions();
            ClearDraggingActions();
            ClearMouseLeftDownActions();
            ClearMouseLeftUpActions();
        }

        #endregion Dispose

        #endregion Public Methods

        #region Private Methods

        bool CheckIfPreviousImageAlreadyCleared(int timeOut_ms)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (WF_ImagePanel.IsClearGraphics)
            {
                if (sw.ElapsedMilliseconds > timeOut_ms)
                    return false;
            }
            return true;
        }
        private void SetResizePanelActions()
        {
            resizePanel.SetResizePanelActions(
                WF_ImagePanel.IncreaseSize,
                WF_ImagePanel.DecreaseSize,
                WF_ImagePanel.FitSize,
                WF_ImagePanel.FullSize,
                null,
                null,
                null);
        }  
        private void SetPixelCoordinatesActions()
        {
            WF_ImagePanel.SetPixelCoordinatesActions(UpdateMouseHoveringPositions);
        }  
        private void UpdateMouseHoveringPositions(int pixelX, int pixelY)
        {
            this.PixelX = pixelX;
            this.PixelY = pixelY;
        }             

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion Private Methods
    }    
}
