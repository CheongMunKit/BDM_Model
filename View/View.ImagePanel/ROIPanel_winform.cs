using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Input;
using Vision.View.ImagePanel;
using System.Collections.Generic;

namespace Vision.View.ImagePanel
{
    public partial class ROIPanel_winform : UserControl
    {
        public ROIPanel_winform()
        {
            InitializeComponent();
        }

        #region Private/Public Fields

        const float zoom_Maximum = 3.0f;
        const float zoom_Original = 1.0f;
        const float zoom_Minimum = 0.04f;
        const float zoom_Increment = 1.5f;
        public float zoom_Current = 1.0f;
        float Zoom
        {
            get { return zoom_Current; }
            set
            {
                if (value < 0.001f) value = 0.001f;
                zoom_Current = value;
                Invalidate();
            }
        }

        Size canvasSize = new Size(60, 40);
        public Size CanvasSize
        {
            get { return canvasSize; }
            set
            {
                canvasSize = value;
                Invalidate();
            }
        }

        bool isDrag_ = false;
        bool isNewClick_ = false;
        PointF StartingPoint = new PointF();

        bool isClearGraphics_ = false;
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


        InterpolationMode interMode = InterpolationMode.HighQualityBilinear;
        public InterpolationMode InterpolationMode
        {
            get { return interMode; }
            set { interMode = value; }
        }

        public int DrawingActionCount
        {
            get { return DrawingActions_.Count; }
        }

        #endregion Private/Public Fields

        #region Delegates

        List<Action<object, Graphics, Graphics, Bitmap, double, double, double>> DrawingActions_ = new List<Action<object, Graphics, Graphics, Bitmap, double, double, double>>();
        List<Action<object, double, double, bool>> DraggingActions_ = new List<Action<object, double, double, bool>>();
        List<Action<object, double, double>> MouseLeftClickActions_ = new List<Action<object, double, double>>();
        Action<int, int> UpdatePixelCoordinates_;

        #endregion Delegates

        #region Public Methods

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
            isClearGraphics_ = true;
            Invalidate();
        }

        public void AddDraggingAction(Action<object, double, double, bool> dragAction)
        {
            DraggingActions_.Add(dragAction);
        }
        public void SetDraggingAction(Action<object, double, double, bool> dragAction)
        {
            DraggingActions_.Clear();
            DraggingActions_.Add(dragAction);
        }
        public void ClearDraggingActions()
        {
            DraggingActions_.Clear();
        }

        public void AddMouseLeftClickAction(Action<object, double, double> mouseClickAction)
        {
            MouseLeftClickActions_.Add(mouseClickAction);
        }
        public void SetMouseLeftClickAction(Action<object, double, double> mouseClickAction)
        {
            MouseLeftClickActions_.Clear();
            MouseLeftClickActions_.Add(mouseClickAction);
        }
        public void ClearMouseLeftClickActions()
        {
            MouseLeftClickActions_.Clear();
        }

        //public void SetImage(Bitmap image, ResizeOption resizeOption)
        //{
        //    Image = image;

        //    if (resizeOption == ResizeOption.FitToFrame) Zoom = CalculateZoomFit();
        //    else if (resizeOption == ResizeOption.Maximize) Zoom = 1.0f;
        //}
        public void StartDraw(double imageWidth, double imageHeight, ResizeOption resizeOption)
        {
            imageWidth_ = imageWidth;
            imageHeight_ = imageHeight;

            if (resizeOption == ResizeOption.FitToFrame) Zoom = CalculateZoomFit();
            else if (resizeOption == ResizeOption.Maximize) Zoom = 1.0f;
        }

        public void SetZoom(float zoom)
        {
            Zoom = zoom;
        }
        public void RefreshUI()
        {
            this.Invalidate();
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

        public void FitSize()
        {
            float zoom = CalculateZoomFit();
            zoom_Current = zoom;
            SetZoom(zoom_Current);
        }

        public void SetPixelCoordinatesActions(Action<int, int> updateMouseHoveringPositions)
        {
            this.UpdatePixelCoordinates_ = updateMouseHoveringPositions;
        }
        public void SetInitialSize(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }


        #endregion Public Methods

        #region UI_Handlers

        private void ImagePanel_winform_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void ImagePanel_winform_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double offsetX = (e.X - StartingPoint.X) / zoom_Current;
            double offsetY = (e.Y - StartingPoint.Y) / zoom_Current;

            foreach (Action<object, double, double, bool> dragAction in DraggingActions_)
            {
                if (isDrag_)
                {
                    dragAction?.Invoke(sender, offsetX, offsetY, isNewClick_);
                    if (isNewClick_) isNewClick_ = false;
                }

            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //draw image
            if (imageHeight_ > 0 && imageWidth_ > 0)
            {
                Rectangle srcRect, distRect;
                Point pt = new Point(0,0);
                srcRect = new Rectangle(0, 0, canvasSize.Width, canvasSize.Height);  // view all image            
                distRect = new Rectangle((int)(-srcRect.Width / 2), -srcRect.Height / 2, srcRect.Width, srcRect.Height); // the center of apparent image is on origin

                Matrix mx = new Matrix(); // create an identity matrix
                mx.Scale(zoom_Current, zoom_Current); // zoom image

                Graphics graphicsUI = e.Graphics;
                graphicsUI.InterpolationMode = interMode;
                graphicsUI.Transform = mx;

                foreach (Action<object, Graphics, Graphics, Bitmap, double, double, double> action in DrawingActions_)
                {
                    action?.Invoke(this, graphicsUI, graphicsUI, null, zoom_Current, 0,0);
                }

                if (isClearGraphics_)
                {
                    graphicsUI.FillRectangle(Brushes.Black,
                        new RectangleF(
                            graphicsUI.ClipBounds.Left,
                            graphicsUI.ClipBounds.Top,
                            graphicsUI.ClipBounds.Width,
                            graphicsUI.ClipBounds.Height));
                    isClearGraphics_ = false;
                }

            }

        }               

        #endregion UI_Handlers   
    }
}
