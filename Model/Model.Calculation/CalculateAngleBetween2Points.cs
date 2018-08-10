using BDMVision.Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BDMVision.Model.ImageCalculation
{
    public static class CalculateAngleBetween2Points
    {
        /// <summary>
        /// Calculate angle from point 1 to point 2, point 1 as origin
        /// 
        /// Positive/Negative
        /// (0,0) -- | (X,0) +-
        /// (0,Y) -+ | (X,Y) ++ 
        /// 
        /// Quadrant
        /// 3rd | 4th
        /// 2nd | 1st
        /// 
        /// Rotation
        /// Positive offset -> rotateCCW
        /// Negative offset -> rotateCW
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Execute(
            Point p1, 
            Point p2, 
            bool IsConvertToDegree)
        {
            VisionLogger.Log(WaftechLibraries.Log.LogType.Log, "CalculateAngleBetweenTwoPoints", "Calculate Angle Between Two Points Started");

            double angle_rad = 0;             
            double angle_deg_CW = 0;
            double deltaX;
            double deltaY;
            
            // 1st quadrant 
            //      |
            //      |
            // ____ p1 ____
            //      |
            //      |    p2
            //
            // Rotate CCW to remove offset       
             if (p2.X >= p1.X && p2.Y >= p1.Y)
            {
                VisionLogger.Log(
                    WaftechLibraries.Log.LogType.Log,
                    "CalculateAngleBetweenTwoPoints",
                    "Point 2 falls on first quadrant");

                deltaY = p2.Y - p1.Y;
                deltaX = p2.X - p1.X;
                angle_rad = Math.Atan(deltaY / deltaX);
                if (angle_rad < 0) throw new Exception("angle_rad should be positive");
            }

            // 2nd quadrant 
            //      |
            //      |
            // ____ p1 ____
            //      |
            // p2   |    
            else if (p2.X < p1.X && p2.Y >= p1.Y)
            {
                VisionLogger.Log(
                    WaftechLibraries.Log.LogType.Log,
                    "CalculateAngleBetweenTwoPoints",
                    "Point 2 falls on second quadrant");

                throw new Exception("point 2 should not be in 2nd quadrant");
                //deltaY = p2.Y - p1.Y;
                //deltaX = -(p2.X - p1.X);
                //angle_rad_CW = Math.PI - Math.Atan(deltaY / deltaX);
                //if(angle_rad_CW > 0) throw new Exception("angle_rad_CW should be negative");
            }

            // 3rd quadrant 
            // p2   |    
            //      |
            // ____ p1 ____
            //      |
            //      |    
            else if (p2.X < p1.X && p2.Y < p1.Y)
            {
                VisionLogger.Log(
                    WaftechLibraries.Log.LogType.Log,
                    "CalculateAngleBetweenTwoPoints",
                    "Point 2 falls on third quadrant");

                throw new Exception("point 2 should not be in 3rd quadrant");

                //deltaY = p2.Y - p1.Y;
                //deltaX = p2.X - p1.X;
                //angle_rad_CW = Math.Atan(deltaY / deltaX);
                //if (angle_rad_CW > 0) throw new Exception("angle_rad_CW should be negative");
            }

            // 4th quadrant 
            //      |    p2
            //      |
            // ____ p1 ____
            //      |
            //      |    
            // Rotate CW to remove offset  
            else if (p2.X >= p1.X  && p2.Y - p1.Y < 0)
            {
                VisionLogger.Log(
                   WaftechLibraries.Log.LogType.Log,
                   "CalculateAngleBetweenTwoPoints",
                   "Point 2 falls on fourth quadrant");

                deltaY = -(p2.Y - p1.Y);
                deltaX = (p2.X - p1.X);
                angle_rad = -(Math.Atan(deltaY / deltaX));
                if (angle_rad > 0) throw new Exception("angle_rad should be negative");
            }

            angle_rad = -angle_rad;
            angle_deg_CW = ConvertToDeg(angle_rad);

            if (IsConvertToDegree) return angle_deg_CW;
            else return angle_rad;
        }

        private static double ConvertToDeg(double value)
        {
            value = value * 180 / Math.PI;
            return value;
        }
    }
}
