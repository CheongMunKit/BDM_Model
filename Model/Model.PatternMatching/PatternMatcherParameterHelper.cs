using BDMVision.Model.Enum;
using BDMVision.Model.ImageTransformation;
using Euresys.Open_eVision_2_0;
using EuresysTools.EROI.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BDMVision.Model.PatternMatching
{
    public class PatternMatchingTransformation
    {
        public static PatternMatchingTransformationResult Execute(
            EImageBW8 eImage,
            float max_AngleOffset,
            float max_XTranslateOffset,
            float max_YTranslateOffset,
            int maxNumberOfTrial,
            EuresysDoublePatternMatcher eMatcher,
            PatternMatcherParameters patternMatcherParameters,
            EROIBW8 matcherROI,
            ELineGauge gauge1,
            ELineGauge gauge2,
            ELineGauge gauge3,
            ELineGauge gauge4,
            double filterTolerance,
            double fiducialOffset,
            WaferOrientation fiducialOrientation,
            bool isIncludeFiducialTolerance,
            Func<ELineGauge,
            ELineGauge,
            ELineGauge,
            ELineGauge,
            EImageBW8,
            double,
            double,
            WaferOrientation,
            bool,
            PointF> FindCenter,
            PointF TeacherMidPoint)
            {
                List<string> messages = new List<string>();
                float thetaOffset = -1;
                float xOffset = -1;
                float yOffset = -1;

                bool isPass = false;
                bool isThetaOffsetWithinTolerance = false;
                bool isXOffsetWithinTolerance = false;
                bool isYOffsetWithinTolerance = false;

                int xTranslationCount = -1;
                int yTranslationCount = -1;
                int thetaTranslationCount = -1;

                PointF calibratedCenterPoint = new PointF(
                    patternMatcherParameters.WaferCenterXPos,
                    patternMatcherParameters.WaferCenterYPos);
                PointF currentWaferCenterPoint = new PointF(-1, -1);
                EuresysDoubleMatcherResults matchedResult = null;

                messages.Add("Maximum NumberOfTrial is " + maxNumberOfTrial);
                messages.Add("Accepted AngleOffset is below " + max_AngleOffset);
                messages.Add("Accepted X Offset is below " + max_XTranslateOffset);
                messages.Add("Accepted Y Offset is below " + max_YTranslateOffset);

                #region X Offset

                for (int i = 0; i < maxNumberOfTrial; i++)
                {
                    // Find X Offset
                    currentWaferCenterPoint = FindCenter.Invoke(
                        gauge1,
                        gauge2,
                        gauge3,
                        gauge4,
                        eImage,
                        filterTolerance,
                        fiducialOffset,
                        fiducialOrientation,
                        isIncludeFiducialTolerance);

                    xOffset = calibratedCenterPoint.X - currentWaferCenterPoint.X;

                    if (Math.Abs(xOffset) < max_XTranslateOffset)
                    {
                        isXOffsetWithinTolerance = true;
                        xTranslationCount = i;
                        messages.Add("XOffset within tolerance");
                        messages.Add("Number of X tranlation performed = " + xTranslationCount);
                        break;
                    }

                    else
                    {
                        isXOffsetWithinTolerance = false;
                        messages.Add(string.Format("XOffset: {0} out of tolerance", xOffset));
                        eImage = ImageTransformer.TranslateImage_X(eImage, xOffset);
                        messages.Add("Image X Translated by " + xOffset);
                    }

                    if (i == maxNumberOfTrial)
                    {
                        xTranslationCount = i;
                        messages.Add("Maximum number of trials for XOffset reached");
                    }
                }

                #endregion XOffset

                #region Y Offset

                for (int i = 0; i <= maxNumberOfTrial; i++)
                {
                    // Find Y Offset
                    currentWaferCenterPoint = FindCenter.Invoke(
                        gauge1,
                        gauge2,
                        gauge3,
                        gauge4,
                        eImage,
                        filterTolerance,
                        fiducialOffset,
                        fiducialOrientation,
                        isIncludeFiducialTolerance);
                    yOffset = calibratedCenterPoint.Y - currentWaferCenterPoint.Y;

                    if (Math.Abs(yOffset) < max_YTranslateOffset)
                    {
                        isYOffsetWithinTolerance = true;
                        yTranslationCount = i;
                        messages.Add("YOffset within tolerance");
                        messages.Add("Number of Y tranlation performed = " + yTranslationCount);
                        break;
                    }

                    else
                    {
                        isYOffsetWithinTolerance = false;
                        messages.Add(string.Format("YOffset: {0} out of tolerance", yOffset));
                        eImage = ImageTransformer.TranslateImage_Y(eImage, yOffset);
                        messages.Add("Image Y Translated by " + yOffset);
                    }

                    if (i == maxNumberOfTrial)
                    {
                        yTranslationCount = i;
                        messages.Add("Maximum number of trials for YOffset reached");
                    }
                }

                #endregion Y offset

                #region Theta Offset

                if (isXOffsetWithinTolerance && isYOffsetWithinTolerance)
                {
                    for (int i = 0; i <= maxNumberOfTrial; i++)
                    {

                        currentWaferCenterPoint = FindCenter.Invoke(
                                               gauge1,
                                               gauge2,
                                               gauge3,
                                               gauge4,
                                               eImage,
                                               filterTolerance,
                                               fiducialOffset,
                                               fiducialOrientation,
                                               isIncludeFiducialTolerance);

                            EuresysEROIHelper.AttachROI(eImage, matcherROI);
                            eMatcher.MatchPatterns(matcherROI);


                        WaferOrientation patternOrientation = FindWaferOrientation(currentWaferCenterPoint, TeacherMidPoint);


                            matchedResult = PatternMatcherParameterHelper.CreateDoublePatternMatcherResult(
                                eMatcher,
                                matcherROI,
                                patternMatcherParameters.OriginalXPos_pattern1,
                                patternMatcherParameters.OriginalYPos_pattern1,
                                currentWaferCenterPoint,
                                patternMatcherParameters.DefaultAngleOffset,
                                patternOrientation);


                            thetaOffset = matchedResult.AngleBetweenResult;
                            if (thetaOffset > 180) throw new ArgumentOutOfRangeException("theta Offset must not be higher than 180");
                            else if (thetaOffset < -180) throw new ArgumentOutOfRangeException("theta Offset must not be lesser than 180");

                            if (Math.Abs(thetaOffset) < max_AngleOffset)
                            {
                                isThetaOffsetWithinTolerance = true;
                                thetaTranslationCount = i;
                                messages.Add("Theta Offset within tolerance");
                                messages.Add("Number of Theta tranlation performed = " + thetaTranslationCount);
                                break;
                            }
                            else
                            {
                                isThetaOffsetWithinTolerance = false;
                                messages.Add(string.Format("Theta Offset: {0} out of tolerance", thetaOffset));
                                eImage = ImageTransformer.RotateImage(eImage, thetaOffset, calibratedCenterPoint);
                                messages.Add("Image Theta Rotated by " + thetaOffset + " around calibrated Center Point");
                            }

                            if (i == maxNumberOfTrial)
                            {
                                thetaTranslationCount = i;
                                messages.Add("Maximum number of trials for Theta Offset reached");
                            }
                        }
                    }

                    else
                    {
                        if (!isXOffsetWithinTolerance)
                        {
                            messages.Add("Theta offset correction skipped due to X Offset out of tolerance");
                        }

                        if (!isYOffsetWithinTolerance)
                        {
                            messages.Add("Theta offset correction skipped due to X Offset out of tolerance");
                        }
                    }

                    #endregion Theta Offset   

                #region Final Result

                messages.Add("Final Angle is " + Math.Round(thetaOffset, 4));
                messages.Add("Final X Offset is " + Math.Round(xOffset, 4));
                messages.Add("Final Y Offset is " + Math.Round(yOffset, 4));

                if (isThetaOffsetWithinTolerance &&
                    isXOffsetWithinTolerance &&
                    isYOffsetWithinTolerance)
                {
                    isPass = true;
                    messages.Add("Result is Pass");
                }

                else
                {
                    isPass = false;
                    messages.Add("Result is False");
                }

                return new PatternMatchingTransformationResult()
                {
                    MatchedResult = matchedResult,
                    eImageAfterTransformation = eImage,
                    FinalAngleOffset = thetaOffset,
                    FinalXOffset = xOffset,
                    FinalYOffset = yOffset,
                    IsPass = isPass,
                    IsThetaOffsetWithinTolerance = isThetaOffsetWithinTolerance,
                    IsXOffsetWithinTolerance = isXOffsetWithinTolerance,
                    IsYOffsetWithinTolerance = isXOffsetWithinTolerance,
                    XTranlastionCount = xTranslationCount,
                    YTranslationCount = yTranslationCount,
                    ThetaTranslationCount = thetaTranslationCount,
                    FinalWaferCenter = currentWaferCenterPoint,
                    Messages = messages,                    
                };

                #endregion Final Result
        }

        public static PatternMatchingTransformationResult ExecuteWithoutRotation(
            EImageBW8 eImage,
            float max_XTranslateOffset,
            float max_YTranslateOffset,
            int maxNumberOfTrial,
            PatternMatcherParameters patternMatcherParameters,
            ELineGauge gauge1,
            ELineGauge gauge2,
            ELineGauge gauge3,
            ELineGauge gauge4,
            double filterTolerance,
            double fiducialOffset,
            WaferOrientation fiducialOrientation,
            bool isIncludeFiducialTolerance,
            Func<ELineGauge,
                ELineGauge,
                ELineGauge,
                ELineGauge,
                EImageBW8,
                double,
                double,
                WaferOrientation,
                bool,
                PointF> FindCenter)
        {
            List<string> messages = new List<string>();
            float thetaOffset = -1;
            float xOffset = -1;
            float yOffset = -1;

            bool isPass = false;
            bool isXOffsetWithinTolerance = false;
            bool isYOffsetWithinTolerance = false;

            int xTranslationCount = -1;
            int yTranslationCount = -1;

            PointF calibratedCenterPoint = new PointF(
                patternMatcherParameters.WaferCenterXPos,
                patternMatcherParameters.WaferCenterYPos);
            PointF currentWaferCenterPoint = new PointF(-1, -1);
            EuresysDoubleMatcherResults matchedResult = null;

            messages.Add("Maximum NumberOfTrial is " + maxNumberOfTrial);
            messages.Add("Accepted X Offset is below " + max_XTranslateOffset);
            messages.Add("Accepted Y Offset is below " + max_YTranslateOffset);

            #region X Offset

            for (int i = 0; i < maxNumberOfTrial; i++)
            {
                // Find X Offset
                currentWaferCenterPoint = FindCenter.Invoke(
                    gauge1,
                    gauge2,
                    gauge3,
                    gauge4,
                    eImage,
                    filterTolerance,
                    fiducialOffset,
                    fiducialOrientation,
                    isIncludeFiducialTolerance);

                xOffset = calibratedCenterPoint.X - currentWaferCenterPoint.X;

                if (Math.Abs(xOffset) < max_XTranslateOffset)
                {
                    isXOffsetWithinTolerance = true;
                    xTranslationCount = i;
                    messages.Add("XOffset within tolerance");
                    messages.Add("Number of X tranlation performed = " + xTranslationCount);
                    break;
                }

                else
                {
                    isXOffsetWithinTolerance = false;
                    messages.Add(string.Format("XOffset: {0} out of tolerance", xOffset));
                    eImage = ImageTransformer.TranslateImage_X(eImage, xOffset);
                    messages.Add("Image X Translated by " + xOffset);
                }

                if (i == maxNumberOfTrial)
                {
                    xTranslationCount = i;
                    messages.Add("Maximum number of trials for XOffset reached");
                }
            }

            #endregion XOffset

            #region Y Offset

            for (int i = 0; i <= maxNumberOfTrial; i++)
            {
                // Find Y Offset
                currentWaferCenterPoint = FindCenter.Invoke(
                    gauge1,
                    gauge2,
                    gauge3,
                    gauge4,
                    eImage,
                    filterTolerance,
                    fiducialOffset,
                    fiducialOrientation,
                    isIncludeFiducialTolerance);
                yOffset = calibratedCenterPoint.Y - currentWaferCenterPoint.Y;

                if (Math.Abs(yOffset) < max_YTranslateOffset)
                {
                    isYOffsetWithinTolerance = true;
                    yTranslationCount = i;
                    messages.Add("YOffset within tolerance");
                    messages.Add("Number of Y tranlation performed = " + yTranslationCount);
                    break;
                }

                else
                {
                    isYOffsetWithinTolerance = false;
                    messages.Add(string.Format("YOffset: {0} out of tolerance", yOffset));
                    eImage = ImageTransformer.TranslateImage_Y(eImage, yOffset);
                    messages.Add("Image Y Translated by " + yOffset);
                }

                if (i == maxNumberOfTrial)
                {
                    yTranslationCount = i;
                    messages.Add("Maximum number of trials for YOffset reached");
                }
            }

            #endregion Y offset

            #region Theta Offset

            thetaOffset = 0;
            messages.Add("Theta Offset is skipped");

            #endregion Theta Offset   

            #region Final Result

            messages.Add("Final Angle is " + Math.Round(thetaOffset, 4));
            messages.Add("Final X Offset is " + Math.Round(xOffset, 4));
            messages.Add("Final Y Offset is " + Math.Round(yOffset, 4));

            if (isXOffsetWithinTolerance &&
                isYOffsetWithinTolerance)
            {
                isPass = true;
                messages.Add("Result is Pass");
            }

            else
            {
                isPass = false;
                messages.Add("Result is False");
            }

            return new PatternMatchingTransformationResult()
            {
                MatchedResult = matchedResult,
                eImageAfterTransformation = eImage,
                FinalAngleOffset = thetaOffset,
                FinalXOffset = xOffset,
                FinalYOffset = yOffset,
                IsPass = isPass,
                IsThetaOffsetWithinTolerance = true,
                IsXOffsetWithinTolerance = isXOffsetWithinTolerance,
                IsYOffsetWithinTolerance = isXOffsetWithinTolerance,
                XTranlastionCount = xTranslationCount,
                YTranslationCount = yTranslationCount,
                ThetaTranslationCount = 0,
                FinalWaferCenter = currentWaferCenterPoint,
                Messages = messages,
            };

            #endregion Final Result
        }

        public static WaferOrientation FindWaferOrientation (
            PointF WaferCenterPoint,
            PointF TeacherROI_MidPoint)
        {
            double deltaX = TeacherROI_MidPoint.X - WaferCenterPoint.X;
            double deltaY = TeacherROI_MidPoint.Y - WaferCenterPoint.Y;

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                if (deltaX > 0) return WaferOrientation.Right;
                else return WaferOrientation.Left;
            }

            else if (Math.Abs(deltaX) < Math.Abs(deltaY))
            {
                if (deltaY > 0) return WaferOrientation.Bottom;
                else return WaferOrientation.Top;
            }
            else throw new Exception("Unexpected Algorithm Error in " + "PatternMatchingTransformation");
        }
    }
}
