using System;
using System.Collections.Generic;
using System.Drawing;
using Euresys.Open_eVision_2_0;
using System.IO;
using System.Threading.Tasks;

namespace BDMVision.Model.PatternMatching
{
    public class EuresysPatternMatcher
    {
        EMatcher EMatcher_;
        bool isPatternLearned_ = false;
        bool isParametersSet_ = false;

        #region Public Methods

        public EuresysPatternMatcher(EMatcher eMatcher)
        {
            this.EMatcher_ = eMatcher;
        }

        public void LearnPattern(
            EROIBW8 patternforTeaching)
        {
            if (patternforTeaching == null) throw new ArgumentNullException("patternforTeaching");
            EMatcher_.ClearImage();
            EMatcher_.LearnPattern(patternforTeaching);
            isPatternLearned_ = true;
        }

        public void SetParameters(PatternMatcherParameters parameters)
        {
            EMatcher_.MinReducedArea = parameters.MinReducedArea;
            EMatcher_.FilteringMode = parameters.FilteringMode;
            EMatcher_.SetPixelDimensions(parameters.PixelDimension_Width, parameters.PixelDimension_Height);
            EMatcher_.MaxPositions = parameters.MaxPositions;
            EMatcher_.MaxInitialPositions = parameters.MaxInitialPositions;
            EMatcher_.MinScore = parameters.MinScore;
            EMatcher_.FinalReduction = parameters.FinalReduction;
            EMatcher_.Interpolate = parameters.Interpolate;
            EMatcher_.ContrastMode = parameters.ContrastMode;
            EMatcher_.CorrelationMode = parameters.CorrelationMode;
            EMatcher_.MinAngle = parameters.MinAngle;
            EMatcher_.MaxAngle = parameters.MaxAngle;
            EMatcher_.MinScale = parameters.MinScale;
            EMatcher_.MaxScale = parameters.MaxScale;
            isParametersSet_ = true;
        }
        public List<EMatchPosition> MatchPattern(EROIBW8 ROIforMatching)
        {
            if (ROIforMatching == null) throw new ArgumentNullException("ROIforMatching");
            if (!isPatternLearned_) throw new Exception("Pattern is not learned");
            if (!isParametersSet_) throw new Exception("Pattern Matching paramters is not set");

            EMatcher_.Match(ROIforMatching);

            List<EMatchPosition> matchedPositions = new List<EMatchPosition>();

            for (int i = 0; i < EMatcher_.NumPositions; i++)
            {
                matchedPositions.Add(EMatcher_.GetPosition(i));
            }
            return matchedPositions;
        }

        #endregion Public Methods
    }

    public class EuresysDoublePatternMatcher
    {
        EMatcher EMatcher1_;
        public EMatcher EMatcher1
        {
            get { return EMatcher1_; }
            set { EMatcher1_ = value; }            
        }
        EMatcher EMatcher2_;
        public EMatcher EMatcher2
        {
            get { return EMatcher2_; }
            set { EMatcher2_ = value; }
        }

        public EImageBW8 Pattern1;
        public EImageBW8 Pattern2;

        public bool isPatternReady = false;
        public bool isParametersSet = false;
        public bool isPatternMatched_ = false;

        #region Public Methods

        public EuresysDoublePatternMatcher(EMatcher eMatcher1, EMatcher eMatcher2)
        {
            this.EMatcher1_ = eMatcher1;
            this.EMatcher2_ = eMatcher2;
        }

        public void SetParameters(PatternMatcherParameters parameters)
        {
            EMatcher1_.MinReducedArea = parameters.MinReducedArea;
            EMatcher1_.FilteringMode = parameters.FilteringMode;
            EMatcher1_.SetPixelDimensions(parameters.PixelDimension_Width, parameters.PixelDimension_Height);
            EMatcher1_.MaxPositions = parameters.MaxPositions;
            EMatcher1_.MaxInitialPositions = parameters.MaxInitialPositions;
            EMatcher1_.MinScore = parameters.MinScore;
            EMatcher1_.FinalReduction = parameters.FinalReduction;
            EMatcher1_.Interpolate = parameters.Interpolate;
            EMatcher1_.ContrastMode = parameters.ContrastMode;
            EMatcher1_.CorrelationMode = parameters.CorrelationMode;
            EMatcher1_.MinAngle = parameters.MinAngle;
            EMatcher1_.MaxAngle = parameters.MaxAngle;
            EMatcher1_.MinScale = parameters.MinScale;
            EMatcher1_.MaxScale = parameters.MaxScale;

            EMatcher2_.MinReducedArea = parameters.MinReducedArea;
            EMatcher2_.FilteringMode = parameters.FilteringMode;
            EMatcher2_.SetPixelDimensions(parameters.PixelDimension_Width, parameters.PixelDimension_Height);
            EMatcher2_.MaxPositions = parameters.MaxPositions;
            EMatcher2_.MaxInitialPositions = parameters.MaxInitialPositions;
            EMatcher2_.MinScore = parameters.MinScore;
            EMatcher2_.FinalReduction = parameters.FinalReduction;
            EMatcher2_.Interpolate = parameters.Interpolate;
            EMatcher2_.ContrastMode = parameters.ContrastMode;
            EMatcher2_.CorrelationMode = parameters.CorrelationMode;
            EMatcher2_.MinAngle = parameters.MinAngle;
            EMatcher2_.MaxAngle = parameters.MaxAngle;
            EMatcher2_.MinScale = parameters.MinScale;
            EMatcher2_.MaxScale = parameters.MaxScale;

            isParametersSet = true;
        }

        public void GetParametersFromPattern(PatternMatcherParameters param)
        {
            float pixelDimension_Height = 0;
            float pixelDimension_Width = 0;

            EMatcher1_.GetPixelDimensions(out pixelDimension_Width, out pixelDimension_Height);
            param.MinReducedArea = EMatcher1_.MinReducedArea;
            param.FilteringMode = EMatcher1_.FilteringMode;
            param.PixelDimension_Width = pixelDimension_Width;
            param.PixelDimension_Height = pixelDimension_Height;
            param.MaxPositions = EMatcher1_.MaxPositions;
            param.MaxInitialPositions = EMatcher1_.MaxInitialPositions;
            param.MinScore = EMatcher1_.MinScore;
            param.FinalReduction = EMatcher1_.FinalReduction;
            param.Interpolate = EMatcher1_.Interpolate;
            param.ContrastMode = EMatcher1_.ContrastMode;
            param.CorrelationMode = EMatcher1_.CorrelationMode;
            param.MinAngle = EMatcher1_.MinAngle;
            param.MaxAngle = EMatcher2_.MaxAngle;
            param.MinScale = EMatcher2_.MinScale;
            param.MaxScale = EMatcher2_.MaxScale;
        }

        public void LoadPatterns(
            string pattern1_FilePath, 
            string pattern2_FilePath,
            string pattern1_ImageFilePath,
            string pattern2_ImageFilePath)
        {
            EMatcher1_.Load(pattern1_FilePath);
            EMatcher2_.Load(pattern2_FilePath);
            if (Pattern1 == null) Pattern1 = new EImageBW8();
            if (Pattern2 == null) Pattern2 = new EImageBW8();
            Pattern1.Load(pattern1_ImageFilePath);
            Pattern2.Load(pattern2_ImageFilePath);
            isParametersSet = true;
            isPatternReady = true;
        }

        //public void TeachAndSaveEMatcher(
        //    PatternMatcherParameters param,
        //    EROIBW8 patternforTeaching1,
        //    EROIBW8 patternforTeaching2,
        //    string pattern1_FilePath, 
        //    string pattern2_FilePath)
        //{
        //    SetParameters(param);
        //    LearningPatternTask(patternforTeaching1, patternforTeaching2);

        //    if (!Directory.Exists(pattern1_FilePath)) Directory.CreateDirectory(Path.GetDirectoryName(pattern1_FilePath));
        //    if (!Directory.Exists(pattern2_FilePath)) Directory.CreateDirectory(Path.GetDirectoryName(pattern2_FilePath));

        //    EMatcher1_.Save(pattern1_FilePath);
        //    EMatcher2_.Save(pattern2_FilePath);            
        //}

        public void TeachAndSaveEMatcher(
            PatternMatcherParameters param,
            EROIBW8 patternforTeaching1,
            EROIBW8 patternforTeaching2,
            string pattern1_FilePath,
            string pattern2_FilePath,
            string pattern1_ImageFilePath,
            string pattern2_ImageFilePath)
        {
            SetParameters(param);
            LearningPatternTask(patternforTeaching1, patternforTeaching2);

            Directory.CreateDirectory(Path.GetDirectoryName(pattern1_FilePath));
            Directory.CreateDirectory(Path.GetDirectoryName(pattern2_FilePath));

            EMatcher1_.Save(pattern1_FilePath);
            EMatcher2_.Save(pattern2_FilePath);
            patternforTeaching1.Save(pattern1_ImageFilePath);
            patternforTeaching2.Save(pattern2_ImageFilePath);
        }

        public void SaveEMatcher_Blank(
            PatternMatcherParameters param,
            string pattern1_FilePath,
            string pattern2_FilePath,
            string pattern1_ImageFilePath,
            string pattern2_ImageFilePath)
        {
            EImageBW8 blackImage = new EImageBW8(512, 512);
            EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), blackImage);   // make it black
            EROIBW8 blackEROI = new EROIBW8();
            blackEROI.OrgX = 0;
            blackEROI.OrgY = 0;
            blackEROI.Width = 512;
            blackEROI.Height = 512;
            blackEROI.Attach(blackImage);
            TeachAndSaveEMatcher(
                param, 
                blackEROI, 
                blackEROI, 
                pattern1_FilePath, 
                pattern2_FilePath,
                pattern1_ImageFilePath,
                pattern2_ImageFilePath);
        }
        
        public Task LearningPatternTask(
            EROIBW8 patternforTeaching1, 
            EROIBW8 patternforTeaching2)
        {
           return Task.Run(() =>
           {
               LearnPattern(patternforTeaching1, patternforTeaching2);
           });
        }
        public void LearnPattern(
            EROIBW8 patternforTeaching1,
            EROIBW8 patternforTeaching2)
        {
            if (patternforTeaching1 == null) throw new ArgumentNullException("patternforTeaching1");
            if (patternforTeaching2 == null) throw new ArgumentNullException("patternforTeaching2");
            //EMatcher1_.ClearImage();
            EMatcher1_.LearnPattern(patternforTeaching1);

            //EMatcher2_.ClearImage();
            EMatcher2_.LearnPattern(patternforTeaching2);
            isPatternReady = true;
        }                             
     
        public Tuple<List<EMatchPosition>, List<EMatchPosition>> MatchPatterns(
           EROIBW8 EROI_forMatching)
        {
            if (EROI_forMatching == null) throw new ArgumentNullException("ROI1forMatching");
            if (!isPatternReady) throw new Exception("Pattern is not learned");
            if (!isParametersSet) throw new Exception("Pattern Matching paramters is not set");

            EMatcher1_.Match(EROI_forMatching);
            EMatcher2_.Match(EROI_forMatching);

            List<EMatchPosition> matchedPositions_1 = new List<EMatchPosition>();
            List<EMatchPosition> matchedPositions_2 = new List<EMatchPosition>();

            for (int i = 0; i < EMatcher1_.NumPositions; i++)
            {
                matchedPositions_1.Add(EMatcher1_.GetPosition(i));
            }
            for (int i = 0; i < EMatcher2_.NumPositions; i++)
            {
                matchedPositions_2.Add(EMatcher2.GetPosition(i));
            }

            isPatternMatched_ = true;
            return new Tuple<List<EMatchPosition>, List<EMatchPosition>>(matchedPositions_1, matchedPositions_2);
        }

        public void DrawPositions(
            Graphics graphics,
            ERGBColor color1,
            ERGBColor color2,
            bool isDrawCorner,
            float zoomX,
            float zoomY,
            float panX,
            float panY)
        {
            EMatcher1_.DrawPositions(graphics, color1, isDrawCorner, zoomX, zoomY, panX, panY);
            EMatcher2_.DrawPositions(graphics, color2, isDrawCorner, zoomX, zoomY, panX, panY);
        }    

        #endregion Public Methods
    }
}
