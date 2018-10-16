using BDMVision.Model.ImageCalculation;
using BDMVision.Model.Log;
using BDMVision.Model.Notification;
using BDMVision.Model.Enum;
using Euresys.Open_eVision_2_0;
using EuresysTools.EROI.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using WaftechLibraries.Log;

namespace BDMVision.Model.PatternMatching
{
    public class PatternMatcherParameterHelper
    {   
        public static PatternMatcherParameters StandardPatternMatcherParameters
        {
            get 
            {
                return new PatternMatcherParameters()
                {
                    ContrastMode = EMatchContrastMode.Normal,
                    CorrelationMode = ECorrelationMode.Normalized,
                    FilteringMode = EFilteringMode.LowPass,

                    MinReducedArea = 64,
                    PixelDimension_Width = 1.00f,
                    PixelDimension_Height = 1.00f,

                    MaxPositions = 1,
                    MaxInitialPositions = 5,
                    MinScore = 0.80f,
                    FinalReduction = 0,
                    Interpolate = true,
                    MinAngle = 0f,
                    MaxAngle = 360f,
                    MinScale = 0.9f,
                    MaxScale = 1.1f,

                    MaxTranslateOffset_px = 1,
                    MaxAngleOffset_deg = 0.09f,                    

                    IsIgnorePattern = false,
                    IsIncludeFiducialTolerance = false,                    
                };
            }
        }           

        public static EuresysPatternMatcher CreatePatternMatcher(PatternMatcherParameters parameters)
        {
            EuresysPatternMatcher patternMatcher = new EuresysPatternMatcher(new EMatcher());
            patternMatcher.SetParameters(parameters);
            return patternMatcher; 
        }              
        public static EuresysDoublePatternMatcher CreateDoublePatternMatcher(PatternMatcherParameters parameters)
        {
            CorrectParameters(parameters);
            EuresysDoublePatternMatcher patternMatcher = new EuresysDoublePatternMatcher(new EMatcher(), new EMatcher());
            patternMatcher.SetParameters(parameters);
            return patternMatcher;
        }

        public static EuresysDoubleMatcherResults CreateDoublePatternMatcherResult(
            EuresysDoublePatternMatcher matcher,
            EROIBW8 matcherEROI,
            float originalXPos_Pattern1,
            float originalYPos_Pattern1,
            PointF waferCenterPoint,
            double refAngle,
            WaferOrientation patternOrientation)
        {
            if (matcher == null) throw new ArgumentNullException("matcher");
            if (!matcher.isPatternMatched_) throw new ArgumentException("Pattern is not matched");
            if (matcher.EMatcher1.NumPositions < 1) throw new ArgumentException("No occurance on EMatcher1");
            if (matcher.EMatcher2.NumPositions < 1) throw new ArgumentException("No occurance on EMatcher2");
            if (matcher.EMatcher1.NumPositions > 1) throw new ArgumentException("More than one occurance on EMatcher1");
            if (matcher.EMatcher2.NumPositions > 1) throw new ArgumentException("More than one occurance on EMatcher2");  
            if (matcher.EMatcher1.NumPositions != matcher.EMatcher2.NumPositions)
                    throw new ArgumentException("Occurance for EMatcher 1 and EMatcher 2 nota tally");
                     
            EuresysDoubleMatcherResults result = new EuresysDoubleMatcherResults()
            {
                Angle1 = matcher.EMatcher1.GetPosition(0).Angle,
                ScaleX1 = matcher.EMatcher1.GetPosition(0).ScaleX,
                ScaleY1 = matcher.EMatcher1.GetPosition(0).ScaleY,
                CenterX1 = matcher.EMatcher1.GetPosition(0).CenterX + (float)(matcherEROI.OrgX),
                CenterY1 = matcher.EMatcher1.GetPosition(0).CenterY + (float)(matcherEROI.OrgY),
                Score1 = matcher.EMatcher1.GetPosition(0).Score,
                Angle2 = matcher.EMatcher2.GetPosition(0).Angle,
                ScaleX2 = matcher.EMatcher2.GetPosition(0).ScaleX,
                ScaleY2 = matcher.EMatcher2.GetPosition(0).ScaleY,
                CenterX2 = matcher.EMatcher2.GetPosition(0).CenterX + (float)(matcherEROI.OrgX),
                CenterY2 = matcher.EMatcher2.GetPosition(0).CenterY + (float)(matcherEROI.OrgY),
                Score2 = matcher.EMatcher2.GetPosition(0).Score,
            };

            PointF p1 = new PointF(result.CenterX1, result.CenterY1);
            PointF p2 = new PointF(result.CenterX2, result.CenterY2);
            double angle = CalculateAngleBetween3Points.Execute(p1, p2, waferCenterPoint, refAngle, true, patternOrientation);
            if (angle > 45) throw new Exception("angle should not be higher than 45 degree");
            if (angle < -45) throw new Exception("angle should not be less than -45 degree");
            result.AngleBetweenResult = (float)angle;  
            result.XOffset = originalXPos_Pattern1 - result.CenterX1;
            result.YOffset = originalYPos_Pattern1 - result.CenterY1;
            return result;     
        }

        public static List<EuresysDoubleMatcherResults> CreateDoublePatternMatcherResults(
            EuresysDoublePatternMatcher matcher,
            List<EROIBW8> matcherEROIs,
            float originalXPos_Pattern1,
            float originalYPos_Pattern1,
            PointF centerPoint,
            double refTheta,
            WaferOrientation patternOrientation)
        {
            if (matcher == null) throw new ArgumentNullException("matcher");
            if (matcherEROIs == null) throw new ArgumentNullException("matcherEROIs");
            if (matcherEROIs.Count < 1) throw new ArgumentException("matcherEROIs count cannot be less than one");
            if (!matcher.isPatternMatched_) throw new ArgumentException("Pattern is not matched");
            if (matcher.EMatcher1.NumPositions < 1) throw new ArgumentException("No occurance on EMatcher1");
            if (matcher.EMatcher2.NumPositions < 1) throw new ArgumentException("No occurance on EMatcher2");
            if (matcher.EMatcher1.NumPositions != matcher.EMatcher2.NumPositions)
                throw new ArgumentException("Occurance for EMatcher 1 and EMatcher 2 nota tally");

            List<EuresysDoubleMatcherResults> result_list = new List<EuresysDoubleMatcherResults>();

            for (int i = 0; i < matcher.EMatcher1.NumPositions; i++)
            {
                EuresysDoubleMatcherResults result = new EuresysDoubleMatcherResults()
                {
                    Angle1 = matcher.EMatcher1.GetPosition(i).Angle,
                    ScaleX1 = matcher.EMatcher1.GetPosition(i).ScaleX,
                    ScaleY1 = matcher.EMatcher1.GetPosition(i).ScaleY,
                    //CenterX1 = matcher.EMatcher1.GetPosition(i).CenterX,
                    //CenterY1 = matcher.EMatcher1.GetPosition(i).CenterY,
                    CenterX1 = matcher.EMatcher1.GetPosition(i).CenterX + (float)(matcherEROIs[0].OrgX),
                    CenterY1 = matcher.EMatcher1.GetPosition(i).CenterY + (float)(matcherEROIs[0].OrgY),
                    Score1 = matcher.EMatcher1.GetPosition(i).Score,

                    Angle2 = matcher.EMatcher2.GetPosition(i).Angle,
                    ScaleX2 = matcher.EMatcher2.GetPosition(i).ScaleX,
                    ScaleY2 = matcher.EMatcher2.GetPosition(i).ScaleY,
                    //CenterX2 = matcher.EMatcher2.GetPosition(i).CenterX,
                    //CenterY2 = matcher.EMatcher2.GetPosition(i).CenterY,
                    CenterX2 = matcher.EMatcher2.GetPosition(i).CenterX + (float)(matcherEROIs[0].OrgX),
                    CenterY2 = matcher.EMatcher2.GetPosition(i).CenterY + (float)(matcherEROIs[0].OrgY),
                    Score2 = matcher.EMatcher2.GetPosition(i).Score,
                };

                PointF p1 = new PointF(result.CenterX1, result.CenterY1);
                PointF p2 = new PointF(result.CenterX2, result.CenterY2);
                double angle = CalculateAngleBetween3Points.Execute(p1, p2, centerPoint, refTheta, true, patternOrientation);
                result.AngleBetweenResult = (float)angle;         
                result.XOffset = originalXPos_Pattern1 - result.CenterX1;
                result.YOffset = originalYPos_Pattern1 - result.CenterY1;
                result_list.Add(result);
            }

            return result_list;
        }
                                
        private static void CorrectParameters(PatternMatcherParameters parameters)
        {
            if (parameters.MaxPositions == 0) parameters.MaxPositions = 1;
            if (parameters.MinReducedArea < 4) parameters.MinReducedArea = 64;
            if (parameters.PixelDimension_Width == 0) parameters.PixelDimension_Width = 1.00f;
            if (parameters.PixelDimension_Height == 0) parameters.PixelDimension_Height = 1.00f;
            if (parameters.MinScale == 0) parameters.MinScale = 1;
            if (parameters.MaxScale == 0) parameters.MaxScale = 1;
            if (parameters.MaxAngle == 0) parameters.MaxAngle = 360;      
        }   

        private static string PatternDirectory
        {
            get { return @"D:\Waftech\BDMVision\ModuleData (BDMVision)\Euresys\Patterns\"; }
        }
        public static bool CheckPatternFilePath(string patternFilePath)
        {
            if (!File.Exists(patternFilePath)) throw new IOException("patternFilePath " + patternFilePath + " does not exists");
            if (Path.GetExtension(patternFilePath) != ".MCH") throw new Exception("patternFilePath " + patternFilePath + " does not match the extension .MCH");
            return true;
        }
        public static bool CheckPatternImageFilePath(string patternImageFilePath)
        {
            if (!File.Exists(patternImageFilePath)) throw new IOException("patternFilePath " + patternImageFilePath + " does not exists");
            if (Path.GetExtension(patternImageFilePath) != ".jpg") throw new Exception("patternFilePath " + patternImageFilePath + " does not match the extension .jpg");
            return true;
        }
        public static string GetLeftPatternFilePath (string recipeName)
        {
            return PatternDirectory + recipeName + "\\" + recipeName + "_Left" + ".MCH";
        }       
        public static string GetRightPatternFilePath(string recipeName)
        {
            return PatternDirectory + recipeName + "\\" + recipeName + "_Right" + ".MCH";
        }     
        public static string GetLeftPatternImageFilePath (string recipeName)
        {
            return PatternDirectory + recipeName + "\\" + recipeName + "_Left" + ".jpg";
        } 
        public static string GetRightPatternImageFilePath (string recipeName)
        {
            return PatternDirectory + recipeName + "\\" + recipeName + "_Right" + ".jpg"; 
        }

        public static bool TeachTeacherROI(
            string RecipeName,
            EuresysDoublePatternMatcher PatternMatcher_,
            PatternMatcherParameters PatternMatcherParameters_,
            EROIBW8 eROIForPatternTeaching1_,
            EROIBW8 eROIForPatternTeaching2_,
            iEuresysROI MatcherEROI,
            PointF WaferCenterPoint,
            WaferOrientation patternOrientation)
        {
            string PatternFilePath_One = PatternMatcherParameterHelper.GetLeftPatternFilePath(RecipeName);
            string PatternFilePath_Two = PatternMatcherParameterHelper.GetRightPatternFilePath(RecipeName);
            string PatternImageFilePath_One = PatternMatcherParameterHelper.GetLeftPatternImageFilePath(RecipeName);
            string PatternImageFilePath_Two = PatternMatcherParameterHelper.GetRightPatternImageFilePath(RecipeName);

            PatternMatcher_.TeachAndSaveEMatcher(
                PatternMatcherParameters_,
                eROIForPatternTeaching1_,
                eROIForPatternTeaching2_,
                PatternFilePath_One,
                PatternFilePath_Two,
                PatternImageFilePath_One,
                PatternImageFilePath_Two);

            if (PatternMatcher_.Pattern1.IsVoid) goto Fail;
            if (PatternMatcher_.Pattern2.IsVoid) goto Fail;
            
            // Match
            EROIBW8 matcherROI = MatcherEROI.GetROI(0);
            PatternMatcher_.MatchPatterns(matcherROI);

            EMatcher eMatcher1 = PatternMatcher_.EMatcher1;
            EMatcher eMatcher2 = PatternMatcher_.EMatcher2;

            if (eMatcher1.NumPositions != 1)
            {
                string errorMessage = "Pattern 1: Number of patterns matched is not equal to one";
                VisionLogger.Log(LogType.Exception, "PatternMatcherManager", errorMessage);
                VisionNotifier.AddNotification(errorMessage);
                goto Fail;
            }

            if (eMatcher2.NumPositions != 1)
            {
                string errorMessage = "Pattern 2: Number of patterns matched is not equal to one";
                VisionLogger.Log(LogType.Exception, "PatternMatcherManager", errorMessage);
                VisionNotifier.AddNotification(errorMessage);
                goto Fail;
            }

            EROIBW8 matcherEROI_1 = MatcherEROI.GetROI(0);

            float OriginalXPos_pattern1 = eMatcher1.GetPosition(0).CenterX + (matcherEROI_1.OrgX);
            float OriginalYPos_pattern1 = eMatcher1.GetPosition(0).CenterY + (matcherEROI_1.OrgY);
            float OriginalXPos_pattern2 = eMatcher2.GetPosition(0).CenterX + (matcherEROI_1.OrgX);
            float OriginalYPos_pattern2 = eMatcher2.GetPosition(0).CenterY + (matcherEROI_1.OrgY);
            float WaferCenterXPos = WaferCenterPoint.X;
            float WaferCenterYPos = WaferCenterPoint.Y;

            PointF p1 = new PointF(OriginalXPos_pattern1, OriginalYPos_pattern1);
            PointF p2 = new PointF(OriginalXPos_pattern2, OriginalYPos_pattern2);
            float PatternDefaultAngleOffset = (float)CalculateAngleBetween3Points.Execute(
                p1,
                p2,
                WaferCenterPoint,
                0,
                true,
                patternOrientation);

            // Replace value
            PatternMatcherParameters_.OriginalXPos_pattern1 = OriginalXPos_pattern1;
            PatternMatcherParameters_.OriginalYPos_pattern1 = OriginalYPos_pattern1;
            PatternMatcherParameters_.OriginalXPos_pattern2 = OriginalXPos_pattern2;
            PatternMatcherParameters_.OriginalYPos_pattern2 = OriginalYPos_pattern2;
            PatternMatcherParameters_.WaferCenterXPos = WaferCenterXPos;
            PatternMatcherParameters_.WaferCenterYPos = WaferCenterYPos;
            PatternMatcherParameters_.DefaultAngleOffset = PatternDefaultAngleOffset;
            return true;

            Fail:
                return false;
        }

        public static void CopyPatterns(
            string existingRecipeName, 
            string newRecipeName,
            PatternMatcherParameters param
            )
        {
            var patternMatcher = CreateDoublePatternMatcher(param);

            string s1 = GetLeftPatternFilePath(existingRecipeName);
            string s2 = GetRightPatternFilePath(existingRecipeName);
            string s3 = GetLeftPatternImageFilePath(existingRecipeName);
            string s4 = GetRightPatternImageFilePath(existingRecipeName);

            CheckPatternFilePath(s1);
            CheckPatternFilePath(s2);
            CheckPatternImageFilePath(s3);
            CheckPatternImageFilePath(s4);

            patternMatcher.LoadPatterns(
                s1,
                s2,
                s3,
                s4);

            s1 = GetLeftPatternFilePath(newRecipeName);
            s2 = GetRightPatternFilePath(newRecipeName);
            s3 = GetLeftPatternImageFilePath(newRecipeName);
            s4 = GetRightPatternImageFilePath(newRecipeName);

            if (!System.IO.Directory.Exists(s1)) Directory.CreateDirectory(Path.GetDirectoryName(s1));

            patternMatcher.SavePatterns(
                s1,
                s2,
                s3,
                s4);
        }

        public static bool CheckTeacherROIOrientation(WaferOrientation patternOrientation, TeacherROIOrientaion teacherOrientation)
        {
            if (patternOrientation == WaferOrientation.Bottom ||
                patternOrientation == WaferOrientation.Top)
            {
                if (teacherOrientation == TeacherROIOrientaion.Vertical)
                    throw new Exception("Teacher ROI Orientation Mismatch. Please switch between Horizontal and Vertical");
            }

            else if (patternOrientation == WaferOrientation.Left ||
                     patternOrientation == WaferOrientation.Right)

            {
                if (teacherOrientation == TeacherROIOrientaion.Horizontal)
                    throw new Exception("Teacher ROI Orientation Mismatch. Please switch between Horizontal and Vertical");
            }
            return true;
        }
    }
}
