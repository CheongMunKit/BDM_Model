using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.View.ImagePanel
{
    public class CalculateZoomFactor
    {
        public double Execute(double imageHeight, double refHeight)
        {
            if (refHeight <= 0)
                throw new ArgumentException("CalculateZoomFactor: reference height cannot be Zero");

            return (refHeight / imageHeight);
        }

        public double Execute(float imageHeight, float refHeight)
        {
            if (refHeight <= 0)
                throw new ArgumentException("CalculateZoomFactor: reference height cannot be Zero");

            return (refHeight / imageHeight);
        }
    }
}
