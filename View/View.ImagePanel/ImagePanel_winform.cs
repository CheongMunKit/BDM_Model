using BDMVision.Model.Log;
using BDMVision.Model.Notification;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Vision.View.ImagePanel
{
    public enum ResizeOption
    {
        FitToFrame,
        Maximize
    }
    public enum ZoomOption
    {
        MouseMove,
        MouseDown,
        Scroll,
    }

    public partial class ImagePanel_winform : UserControl
    {   
        public ImagePanel_winform()
        {
            InitializeComponent();

            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
              ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            #region Mouse Event Handler

            this.MouseEnter += ImagePanel_winform_MouseEnter;
            this.MouseWheel += ImagePanel_winform_MouseWheel;
            this.MouseMove += ImagePanel_winform_MouseMove;
            this.MouseDown += ImagePanel_winform_MouseDown;
            this.MouseUp += ImagePanel_winform_MouseUp;

            #endregion Mouse Event Handler
        }

        #region Private/Public Fields

        int viewRectWidth, viewRectHeight; // view window width and height
        public int PanX { get { return -hScrollBar1.Value; } }
        public int PanY { get { return -vScrollBar1.Value; } }
        const float zoom_Maximum = 3.0f;
        const float zoom_Original = 1.0f;
        const float zoom_Minimum = 0.04f;
        const float zoom_Increment = 1.25f;
        public float zoom_Current = 1.0f;
        float Zoom
        {
            get { return zoom_Current; }
            set
            {
                if (value < 0.001f) value = 0.001f;
                zoom_Current = value;
                displayScrollbar();
                setScrollbarValues();
                Invalidate();
            }
        }
        ZoomOption zoomOption;

        Size canvasSize = new Size(60, 40);     
        public Size CanvasSize
        {
            get { return canvasSize; }
            set
            {
                canvasSize = value;
                displayScrollbar();
                setScrollbarValues();
                Invalidate();
            }
        }

        bool isDrag_ = false;
        bool isNewClick_ = false;
        PointF StartingPoint = new PointF();

        public bool IsClearGraphics = false;    
        double imageWidth_;
        double imageHeight_;
        Size imageSize_
        {
            get
            {
                return new Size(
                    (int)Math.Round(imageWidth_, 0),
                    (int)Math.Round(imageHeight_, 0));
            }
        }

        double HScrollBarPosRatio_;
        double HScrollBarPosRatio
        {
            get { return HScrollBarPosRatio_; }
            set
            {
                if (value > 1) value = 1;
                if (value < 0) value = 0;
                HScrollBarPosRatio_ = value;
            }
        }
        double VScrollBarPosRatio_;
        double VScrollBarPosRatio
        {
            get { return VScrollBarPosRatio_; }
            set
            {
                if (value > 1) value = 1;
                if (value < 0) value = 0;
                VScrollBarPosRatio_ = value;
            }
        }
        double ImagePosX_mouseMove;
        double ImagePosY_mouseMove;
        double ImagePosX_mouseDown;
        double ImagePosY_mouseDown;

        InterpolationMode interMode = InterpolationMode.HighQualityBilinear;
        public InterpolationMode InterpolationMode
        {
            get { return interMode; }
            set { interMode = value; }
        }   

        #endregion Private/Public Fields

        #region Delegates

        List<Action<object, Graphics, Graphics, Bitmap, double, double, double>> DrawingActions_ = new List<Action<object, Graphics, Graphics, Bitmap, double, double, double>>();
        List<Action<object, double, double, double, double, bool, MouseEventArgs>> DraggingActions_ = new List<Action<object, double, double, double, double, bool, MouseEventArgs>>();
        List<Action<object, double, double, MouseEventArgs>> MouseLeftDownActions_ = new List<Action<object, double, double, MouseEventArgs>>();
        List<Action<object, double, double, MouseEventArgs>> MouseLeftUpActions_ = new List<Action<object,double, double, MouseEventArgs>>(); 

        Action<int, int> UpdatePixelCoordinates_;

        #endregion Delegates

        #region Public Methods

        #region Drawing / Paint

        public void StartDraw(double imageWidth, double imageHeight, ResizeOption resizeOption)
        {
            imageWidth_ = imageWidth;
            imageHeight_ = imageHeight;
            if (IsClearGraphics) IsClearGraphics = false;
            if (resizeOption == ResizeOption.FitToFrame) Zoom = CalculateZoomFit();
            else if (resizeOption == ResizeOption.Maximize) Zoom = 1.0f;
        }
        public void AddDrawingAction(Action<object, Graphics, Graphics, Bitmap, double, double, double> newDrawingAction)
        {
            DrawingActions_.Add(newDrawingAction);
        }   
        public void SetDrawingAction(Action<object, Graphics, Graphics, Bitmap, double, double, double> newDrawingAction)
        {
            DrawingActions_.Clear();
            DrawingActions_.Add(newDrawingAction);
        } 
        public void ClearDrawingActions()
        {
            DrawingActions_.Clear();
        }  
        public void ClearPreviousDrawing()
        {
            RefreshUI();
        }

        delegate void RefreshUI_Delegate();
        public void RefreshUI()
        {
            if (this.InvokeRequired)
            {
                RefreshUI_Delegate d = new RefreshUI_Delegate(RefreshUI);
                this.Invoke(d);
            }
            else { this.Refresh(); }
        }       

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            //draw image
            if (imageHeight_ > 0 && imageWidth_ > 0)
            {
                Rectangle srcRect, distRect;
                Point pt = new Point((int)(hScrollBar1.Value / zoom_Current), (int)(vScrollBar1.Value / zoom_Current));
                if (canvasSize.Width * zoom_Current < viewRectWidth && canvasSize.Height * zoom_Current < viewRectHeight)
                    srcRect = new Rectangle(0, 0, canvasSize.Width, canvasSize.Height);  // view all image
                else srcRect = new Rectangle(pt, new Size((int)(viewRectWidth / zoom_Current), (int)(viewRectHeight / zoom_Current))); // view a portion of image

                distRect = new Rectangle((int)(-srcRect.Width / 2), -srcRect.Height / 2, srcRect.Width, srcRect.Height); // the center of apparent image is on origin

                Matrix mx = new Matrix(); // create an identity matrix
                mx.Scale(zoom_Current, zoom_Current); // zoom image
                mx.Translate(viewRectWidth / 2.0f, viewRectHeight / 2.0f, MatrixOrder.Append); // move image to view window center

                Graphics graphicsUI = e.Graphics;
                graphicsUI.InterpolationMode = interMode;
                graphicsUI.Transform = mx;
                //graphicsUI.DrawImage(image,distRect,srcRect, GraphicsUnit.Pixel);

                foreach (Action<object, Graphics, Graphics, Bitmap, double, double, double> action in DrawingActions_)
                {
                    action?.Invoke(this, graphicsUI, graphicsUI, null, zoom_Current, -hScrollBar1.Value, -vScrollBar1.Value);
                }

                if (IsClearGraphics)
                {
                    graphicsUI.FillRectangle(Brushes.Black,
                        new RectangleF(
                            graphicsUI.ClipBounds.Left,
                            graphicsUI.ClipBounds.Top,
                            graphicsUI.ClipBounds.Width,
                            graphicsUI.ClipBounds.Height));
                    IsClearGraphics = false;
                }

            }

        }

        #endregion Drawing / Paint

        #region Dragging / MouseMove

        public void AddDraggingAction(Action<object, double, double, double, double, bool, MouseEventArgs> dragAction)
        {
            DraggingActions_.Add(dragAction);
        }
        public void SetDraggingAction(Action<object, double ,double, double, double, bool, MouseEventArgs> dragAction)
        {
            DraggingActions_.Clear();
            DraggingActions_.Add(dragAction);
        }
        public void ClearDraggingActions()
        {
            DraggingActions_.Clear();
        }

        private void ImagePanel_winform_MouseMove(object sender, MouseEventArgs e)
        {
            double imagePosX = (e.X + hScrollBar1.Value) / zoom_Current;
            double imagePosY = (e.Y + vScrollBar1.Value) / zoom_Current;
            this.ImagePosX_mouseMove = imagePosX;
            this.ImagePosY_mouseMove = imagePosY;

            UpdatePixelCoordinates_?.Invoke((int)(Math.Round(imagePosX)), (int)Math.Round(imagePosY));

            double offsetX = (e.X - StartingPoint.X) / zoom_Current;
            double offsetY = (e.Y - StartingPoint.Y) / zoom_Current;

            foreach (Action<object, double, double, double, double, bool, MouseEventArgs> dragAction in DraggingActions_)
            {
                if (isDrag_)
                {
                    dragAction?.Invoke(sender, imagePosX, imagePosY, offsetX, offsetY, isNewClick_, e);
                    if (isNewClick_) isNewClick_ = false;
                }

            }
        }

        #endregion Dragging / MouseMove

        #region Mouse Left Down

        public void AddMouseLeftDownAction(Action<object, double, double, MouseEventArgs> mouseLeftDownAction)
        {
            MouseLeftDownActions_.Add(mouseLeftDownAction);
        }  
        public void SetMouseLeftDownAction(Action<object, double, double, MouseEventArgs> mouseLeftDownAction)
        {
            MouseLeftDownActions_.Clear();
            MouseLeftDownActions_.Add(mouseLeftDownAction);
        }  
        public void ClearMouseLeftDownActions()
        {
            MouseLeftDownActions_.Clear();
        }

        private void ImagePanel_winform_MouseDown(object sender, MouseEventArgs e)
        {
            zoomOption = ZoomOption.MouseDown;
            double imagePosX = (e.X + hScrollBar1.Value) / zoom_Current;
            double imagePosY = (e.Y + vScrollBar1.Value) / zoom_Current;
            ImagePosX_mouseDown = imagePosX;
            ImagePosY_mouseDown = imagePosY;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                StartingPoint = e.Location;
                isDrag_ = true;
                isNewClick_ = true;

                foreach (Action<object, double, double, MouseEventArgs> action in MouseLeftDownActions_)
                {
                    action?.Invoke(sender, imagePosX, imagePosY, e);
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                
            }
        }

        #endregion Mouse Left Down

        #region Mouse Left Up

        public void AddMouseLeftUpAction(Action<object, double, double, MouseEventArgs> mouseLeftUpAction)
        {
            MouseLeftUpActions_.Add(mouseLeftUpAction);
        }

        public void SetMouseLeftUpAction(Action<object,double, double, MouseEventArgs> mouseLeftUpAction)
        {
            MouseLeftUpActions_.Clear();
            MouseLeftUpActions_.Add(mouseLeftUpAction);
        }

        public void ClearMouseLeftUpAction()
        {
            MouseLeftUpActions_.Clear();
        }

        private void ImagePanel_winform_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrag_) isDrag_ = false;
            double imagePosX = (e.X + hScrollBar1.Value) / zoom_Current;
            double imagePosY = (e.Y + vScrollBar1.Value) / zoom_Current;

            foreach (Action<object, double, double, MouseEventArgs> action in MouseLeftUpActions_)
            {
                action?.Invoke(sender, imagePosX, imagePosY, e);
            }

        }

        #endregion Mouse Left Up

        #region Mouse Wheel

        private void ImagePanel_winform_MouseWheel(object sender, MouseEventArgs e)
        {
            zoomOption = ZoomOption.MouseMove;

            // If control key is pressed, ZOOM in or out
            if ((System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) > 0)
            {
                // Change ScrollBar Value Based on mouse hover location
                try
                {
                    // Don't zoomIn if already maximum
                    if (e.Delta > 0 && zoom_Current == zoom_Maximum) return;

                    // Don't zoomOut if already minimum
                    if (e.Delta < 0 && zoom_Current == zoom_Minimum) return;

                    VScrollBarPosRatio = ((double)e.Y + vScrollBar1.Value) / (imageHeight_ * zoom_Current);
                    vScrollBar1.Value = (int)Math.Round(VScrollBarPosRatio * vScrollBar1.Maximum, 0);

                    HScrollBarPosRatio = ((double)e.X + hScrollBar1.Value) / (imageWidth_ * zoom_Current);
                    hScrollBar1.Value = (int)Math.Round(HScrollBarPosRatio * hScrollBar1.Maximum, 0);
                }

                catch { }


                // Wheel up, zoom out
                if (e.Delta > 0) IncreaseSize(false);
                else DecreaseSize(false);

            }

            // if control key is not pressed
            else
            {
                if (vScrollBar1.Visible)
                {
                    // Wheel up, Scroll up
                    if (e.Delta > 0)
                    {

                        if (vScrollBar1.Value - vScrollBar1.SmallChange >= vScrollBar1.Minimum)
                            vScrollBar1.Value -= vScrollBar1.SmallChange;
                        else
                            vScrollBar1.Value = vScrollBar1.Minimum;
                    }
                    // Wheel down, Scroll down
                    else
                    {
                        if (vScrollBar1.Value + vScrollBar1.SmallChange <= vScrollBar1.Maximum)
                            vScrollBar1.Value += vScrollBar1.SmallChange;
                        else
                            vScrollBar1.Value = vScrollBar1.Maximum;
                    }

                    Invalidate();
                }
            }
        }

        #endregion Mouse Wheel

        #region Zoom

        public void SetZoom(float zoom)
        {
            Zoom = zoom;
        }

        public float CalculateZoomFit()
        {
            CalculateZoomFactor CalculateZoomFactor = new CalculateZoomFactor();
            return (float)CalculateZoomFactor.Execute(imageHeight_, Height);
        }
        public float CalculateZoomIncrease()
        {
            // Only Increase Size if the current zoom is less than original zoom
            if (zoom_Current == zoom_Maximum) return zoom_Current;
            else if (zoom_Current * zoom_Increment > zoom_Original) zoom_Current = zoom_Maximum;
            else zoom_Current = zoom_Current * zoom_Increment;
            return zoom_Current;

        }
        public float CalculateZoomDecrease()
        {
            // Only Decrease Size if the current zoom is more than original zoom
            if (zoom_Current == zoom_Minimum) return zoom_Current;
            else if (zoom_Current / zoom_Increment < zoom_Minimum) zoom_Current = zoom_Minimum;
            else zoom_Current = zoom_Current / zoom_Increment;
            return zoom_Current;
        }

        public void IncreaseSize()
        {
            IncreaseSize(true);
        }
        public void IncreaseSize(bool IsSetZoomLocation)
        {
            if (IsSetZoomLocation) SetScrollBarLocationBeforeZoom();
            float zoomIncrease = CalculateZoomIncrease();
            SetZoom(zoomIncrease);
        }
        public void DecreaseSize()
        {
            DecreaseSize(true);
        }
        public void DecreaseSize(bool IsSetZoomLocation)
        {
            if (IsSetZoomLocation) SetScrollBarLocationBeforeZoom();
            float zoomDecrease = CalculateZoomDecrease();
            SetZoom(zoom_Current);
        }
        public void FullSize()
        {
            FullSize(true);
        }
        public void FullSize(bool IsSetZoomLocation)
        {
            if (IsSetZoomLocation) SetScrollBarLocationBeforeZoom();
            zoom_Current = zoom_Maximum;
            SetZoom(zoom_Current);
        }
        public void FitSize()
        {
            FitSize(true);
        }
        public void FitSize(bool IsSetZoomLocation)
        {
            if (IsSetZoomLocation) SetScrollBarLocationBeforeZoom();
            float zoom = CalculateZoomFit();
            zoom_Current = zoom;
            SetZoom(zoom_Current);
        }

        protected override void OnResize(EventArgs e)
        {
            displayScrollbar();
            setScrollbarValues();
            base.OnResize(e);
        }
        private void MenuItem_Zoom_Click(object sender, EventArgs e)
        {
            try
            {
                zoomOption = ZoomOption.MouseMove;
                if (sender == MenuItem_ZoomFitSize) FitSize(true);
                else if (sender == MenuItem_ZoomFullSize) FullSize(true);
                else
                {
                    SetScrollBarLocationBeforeZoom();
                    string value_Str = ((ToolStripMenuItem)sender).Text;
                    float  zoomValue = float.Parse(Regex.Match(value_Str, @"\d+").Value);
                    zoomValue = zoomValue / 100;
                    SetZoom(zoomValue);
                }
            }
            catch (Exception ex)
            {
                VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, sender.ToString(), ex);
                VisionNotifier.AddNotification(ex.Message);
            }
        }

        private void SetScrollBarLocationBeforeZoom()
        {
            if (zoomOption == ZoomOption.MouseMove) SetZoomLocationBasedOnScrollMouseLocation(this.ImagePosX_mouseMove, this.ImagePosY_mouseMove);
            else if (zoomOption == ZoomOption.MouseDown) SetZoomLocationBasedOnScrollMouseLocation(this.ImagePosX_mouseDown, this.ImagePosY_mouseDown);
            else if (zoomOption == ZoomOption.Scroll) SetZoomLocationBasedOnScrollBarLocation();
        }
        private void SetZoomLocationBasedOnScrollMouseLocation(double posX, double poxY)
        {
            // Set ScrollBar Position According to image pixel position
            double currentImagePixel_X = posX;
            double currentImagePixel_Y = poxY;

            VScrollBarPosRatio = currentImagePixel_Y / (imageHeight_);
            vScrollBar1.Value = (int)Math.Round(VScrollBarPosRatio * vScrollBar1.Maximum, 0);

            HScrollBarPosRatio = currentImagePixel_X / (imageWidth_);
            hScrollBar1.Value = (int)Math.Round(HScrollBarPosRatio * hScrollBar1.Maximum, 0);
        }
        private void SetZoomLocationBasedOnScrollBarLocation()
        {
            if (vScrollBar1.Visible)
            {
                HScrollBarPosRatio = (double)hScrollBar1.Value / (CanvasSize.Width * Zoom - viewRectWidth + (double)vScrollBar1.Width);

                if (HScrollBarPosRatio > 1) HScrollBarPosRatio = 1;
                else if (HScrollBarPosRatio < 0) HScrollBarPosRatio = 0;
                else if (HScrollBarPosRatio > 0.995) HScrollBarPosRatio = 1;
                else if (HScrollBarPosRatio < 0.005) HScrollBarPosRatio = 0;
            }

            if (hScrollBar1.Visible)
            {
                VScrollBarPosRatio = (double)vScrollBar1.Value / (CanvasSize.Height * Zoom - viewRectHeight + (double)hScrollBar1.Height);

                if (VScrollBarPosRatio > 1) VScrollBarPosRatio = 1;
                else if (VScrollBarPosRatio < 0) VScrollBarPosRatio = 0;
                else if (VScrollBarPosRatio > 0.9995)
                    VScrollBarPosRatio = 1;
                else if (VScrollBarPosRatio < 0.005)
                    VScrollBarPosRatio = 0;
            }
        }

        #endregion Zoom 

        #region Mouse Cursor

        private void ImagePanel_winform_MouseEnter(object sender, EventArgs e)
        {
            //this.Focus();
        }
        public void SetPixelCoordinatesActions(Action<int, int> updateMouseHoveringPositions)
        {
            this.UpdatePixelCoordinates_ = updateMouseHoveringPositions;
        }

        #endregion Mouse Cursor

        #region Scroll Bar

        private void displayScrollbar()
        {
            viewRectWidth = this.Width;
            viewRectHeight = this.Height;
            canvasSize = imageSize_;

            // If the zoomed image is wider than view window, show the HScrollBar and adjust the view window
            if (viewRectWidth >= canvasSize.Width * zoom_Current)
            {
                hScrollBar1.Visible = false;
                hScrollBar1.Value = 0;
                viewRectHeight = Height;
                //viewRectHeight = Height_;
            }
            else
            {
                hScrollBar1.Visible = true;
                viewRectHeight = Height - hScrollBar1.Height;
                //viewRectHeight = Height_ - hScrollBar1.Height;
            }

            // If the zoomed image is taller than view window, show the VScrollBar and adjust the view window
            if (viewRectHeight >= canvasSize.Height * zoom_Current)
            {
                vScrollBar1.Visible = false;
                vScrollBar1.Value = 0;
                viewRectWidth = Width;
                //viewRectWidth = Width_;
            }
            else
            {
                vScrollBar1.Visible = true;
                viewRectWidth = Width - vScrollBar1.Width;
            }

            // Set up scrollbars
            hScrollBar1.Location = new Point(0, Height - hScrollBar1.Height);
            //hScrollBar1.Location = new Point(0, Height_ - hScrollBar1.Height);
            hScrollBar1.Width = viewRectWidth;
            vScrollBar1.Location = new Point(Width - vScrollBar1.Width, 0);
            //vScrollBar1.Location = new Point(Width_ - vScrollBar1.Width, 0);
            vScrollBar1.Height = viewRectHeight;
        }
        private void setScrollbarValues()
        {
            // Set the Maximum, Minimum, LargeChange and SmallChange properties.
            this.vScrollBar1.Minimum = 0;
            this.hScrollBar1.Minimum = 0;

            // If the offset does not make the Maximum less than zero, set its value. 
            if ((canvasSize.Width * zoom_Current - viewRectWidth) > 0)
            {
                this.hScrollBar1.Maximum = (int)(canvasSize.Width * zoom_Current) - viewRectWidth;
            }

            // If the VScrollBar is visible, adjust the Maximum of the 
            // HSCrollBar to account for the width of the VScrollBar.  
            if (this.vScrollBar1.Visible)
            {
                this.hScrollBar1.Maximum += this.vScrollBar1.Width;
            }

            if (hScrollBar1.Visible)
            {
                try
                {
                    hScrollBar1.Value = (int)Math.Round(HScrollBarPosRatio * hScrollBar1.Maximum, 0);
                }
                catch (Exception)
                {
                    hScrollBar1.Value = 0;
                }
            }

            this.hScrollBar1.LargeChange = this.hScrollBar1.Maximum / 10;
            this.hScrollBar1.SmallChange = this.hScrollBar1.Maximum / 200;

            // Adjust the Maximum value to make the raw Maximum value 
            // attainable by user interaction.
            this.hScrollBar1.Maximum += this.hScrollBar1.LargeChange;

            // If the offset does not make the Maximum less than zero, set its value.    
            if ((canvasSize.Height * zoom_Current - viewRectHeight) > 0)
            {
                this.vScrollBar1.Maximum = (int)(canvasSize.Height * zoom_Current) - viewRectHeight;
            }

            // If the HScrollBar is visible, adjust the Maximum of the 
            // VSCrollBar to account for the width of the HScrollBar.
            if (this.hScrollBar1.Visible)
            {
                this.vScrollBar1.Maximum += this.hScrollBar1.Height;
            }

            if (vScrollBar1.Visible)
            {
                try
                {
                    vScrollBar1.Value = (int)Math.Round(VScrollBarPosRatio * vScrollBar1.Maximum, 0);
                }
                catch
                {
                    vScrollBar1.Value = 0;
                }
               
            }

            this.vScrollBar1.LargeChange = this.vScrollBar1.Maximum / 10;
            this.vScrollBar1.SmallChange = this.vScrollBar1.Maximum / 200;

            // Adjust the Maximum value to make the raw Maximum value 
            // attainable by user interaction.
            this.vScrollBar1.Maximum += this.vScrollBar1.LargeChange;
        }
        private void OnScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            this.zoomOption = ZoomOption.Scroll;
            Invalidate();
        }
        
        #endregion Scroll Bar

        #endregion Public Methods

        #region UI_Handlers

        protected override void OnLoad(EventArgs e)
        {
            displayScrollbar();
            setScrollbarValues();
            base.OnLoad(e);
        }

        #endregion UI_Handlers   
    }
}
