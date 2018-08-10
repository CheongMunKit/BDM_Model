using Euresys.Open_eVision_2_0;
using System;
using System.Drawing;

namespace Vision.View.ImagePanel
{
    public class ResizeImage
    {
        public Bitmap Execute(Bitmap original, double zoomFactor)
        {
            try
            {
                Size newSize = new Size((int)(original.Width * zoomFactor), (int)(original.Height * zoomFactor));
                Bitmap resized = new Bitmap(original, newSize);
                return resized;
            }

            catch
            {
                //Maximum Size Reached
                return original;
            }
        }
    }

    public class ResizeEImage
    {
        public EImageBW8 Execute(EImageBW8 original, double zoomFactor)
        {
            try
            {
                EImageBW8 resized = new EImageBW8(original);
                resized.SetSize((int)(original.Width * zoomFactor), (int)(original.Height * zoomFactor));             
                return resized;
            }

            catch (Exception e)
            {
                //Maximum Size Reached
                throw new Exception("ResizeImage Failed" + e.Message);
            }
        }
    }
}
