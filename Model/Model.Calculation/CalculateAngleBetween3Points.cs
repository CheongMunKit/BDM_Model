using BDMVision.Model.Enum;
using BDMVision.Model.Log;
using System;
using System.Drawing;

namespace BDMVision.Model.ImageCalculation
{
    public static class CalculateAngleBetween3Points
    {
        /// <summary>
        /// Calculate the midpoint of point1 and point2 
        /// Draw a line from the midpoint to referencePoint
        /// 
        /// CCW -> positive
        /// CW  -> negative
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Execute(
            PointF p1,
            PointF p2,
            PointF centerpoint,
            double referenceAngle_deg,
            bool IsConvertToDegree,
            WaferOrientation patternOrientation)
        {
            VisionLogger.Log(WaftechLibraries.Log.LogType.Log,
                "CalculateAngleBetween3Points",
                "Calculate Angle Between Three Points Started");

            double midPointX = (p1.X + p2.X) / 2.0;
            double midPointY = (p1.Y + p2.Y) / 2.0;
            double centerX = centerpoint.X;
            double centerY = centerpoint.Y;

            double deltaX = midPointX - centerX;
            double deltaY = midPointY - centerY;
            double theta_rad;

            if (patternOrientation == WaferOrientation.Bottom)
            {
                if (deltaX < 0) { theta_rad = Math.Atan(Math.Abs(deltaX / deltaY)); }     // CCW
                else { theta_rad = -Math.Atan(Math.Abs(deltaX / deltaY)); }               // CW
            }
            else if (patternOrientation == WaferOrientation.Right)
            {
                if (deltaY > 0) { theta_rad = Math.Atan(Math.Abs(deltaY / deltaX)); }    // CCW
                else { theta_rad = -Math.Atan(Math.Abs(deltaY / deltaX)); }              // CW    
            }
            else if (patternOrientation == WaferOrientation.Top)
            {
                if (deltaX > 0) { theta_rad = Math.Atan(Math.Abs(deltaX / deltaY)); }     // CCW
                else { theta_rad = -Math.Atan(Math.Abs(deltaX / deltaY)); }               // CW
            }

            else if (patternOrientation == WaferOrientation.Left)
            {
                if (deltaY < 0) { theta_rad = Math.Atan(Math.Abs(deltaY / deltaX)); }    // CCW
                else { theta_rad = -Math.Atan(Math.Abs(deltaY / deltaX)); }              // CW   
            }

            else throw new Exception("Invalid notch Orientation");

            double thetaOffset_rad;
            thetaOffset_rad = theta_rad - ConvertToRad(referenceAngle_deg);
            if (IsConvertToDegree) return ConvertToDeg(thetaOffset_rad);
            else return thetaOffset_rad;
        }

        private static double ConvertToDeg(double value)
        {
            value = value * 180.0 / Math.PI;
            return value;
        }

        private static double ConvertToRad(double value)
        {
            value = value * Math.PI / 180;
            return value;
        }
    }
}
