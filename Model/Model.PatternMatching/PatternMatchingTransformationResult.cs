using Euresys.Open_eVision_2_0;
using System.Collections.Generic;
using System.Drawing;

namespace BDMVision.Model.PatternMatching
{
    public class PatternMatchingTransformationResult
    {
        public bool IsPass;
        public bool IsXOffsetWithinTolerance;
        public bool IsYOffsetWithinTolerance;
        public bool IsThetaOffsetWithinTolerance;

        public int XTranlastionCount;
        public int YTranslationCount;
        public int ThetaTranslationCount;

        public List<string> Messages;
        public float FinalAngleOffset;
        public float FinalXOffset;
        public float FinalYOffset;

        public float TopPoint;
        public float RightPoint;
        public float BottomPoint;
        public float LeftPoint;

        public EuresysDoubleMatcherResults MatchedResult;
        public EImageBW8 eImageAfterTransformation;
        public PointF FinalWaferCenter; 
    }
}
