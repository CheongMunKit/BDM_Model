using System;
using Euresys.Open_eVision_2_0;
using BDMVision.Model.Enum;

namespace BDMVision.Model.PatternMatching
{
    [Serializable]
    public class PatternMatcherParameters
    {
        // For Teaching
        public int MinReducedArea { get; set; }
        public EFilteringMode FilteringMode { get; set; }
        public float PixelDimension_Width { get; set; }
        public float PixelDimension_Height { get; set; }
        public TeacherROIOrientaion TeacherROI_Orientation { get; set; }

        // For Matching
        public int MaxPositions { get; set; }
        public int MaxInitialPositions { get; set; }
        public float MinScore { get; set; }
        public int FinalReduction { get; set; }
        public bool Interpolate { get; set; }
        public EMatchContrastMode ContrastMode { get; set; }
        public ECorrelationMode CorrelationMode { get; set; }
        public float MinAngle { get; set; }
        public float MaxAngle { get; set; }
        public float MinScale { get; set; }
        public float MaxScale { get; set; }
        public bool IsIgnorePattern { get; set; }
        public bool IsDisableNotification { get; set; }

        // For image translation and rotation
        public float OriginalXPos_pattern1 { get; set; }
        public float OriginalYPos_pattern1 { get; set; }
        public float OriginalXPos_pattern2 { get; set; }
        public float OriginalYPos_pattern2 { get; set; }          
        public float WaferCenterXPos { get; set; }
        public float WaferCenterYPos { get; set; }   
        public float DefaultAngleOffset { get; set; }   
        public float MaxAngleOffset_deg { get; set; }
        public float MaxTranslateOffset_px { get; set; }
        public bool IsIncludeFiducialTolerance { get; set; }

        public void ValidateParameters()
        {
           
        }
    }          
}
