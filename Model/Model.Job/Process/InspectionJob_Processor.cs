using BDMVision.Model.Correction;
using BDMVision.Model.Enum;
using BDMVision.Model.Image;
using BDMVision.Model.ImageTransformation;
using BDMVision.Model.Log;
using BDMVision.Model.MapFile;
using BDMVision.Model.MapFileReader;
using BDMVision.Model.MapResult;
using BDMVision.Model.MapVision;
using BDMVision.Model.MapVisionReader;
using BDMVision.Model.MapColor;
using BDMVision.Model.PatternMatching;
using BDMVision.Model.VisionResult;
using BDMVision.Model.WaferCenter;
using Euresys.Open_eVision_2_0;
using EuresysTools.EROI.Model;
using System;
using System.Diagnostics;
using System.Drawing;
using BDMVision.Model.Notification;

namespace BDMVision.Model.Job
{
    public partial class InspectionJob_Processor<T>
	{
        WaferCenterParameters waferCenterParameters_;
        CorrectionParameters correctionParameters_;
        EROIParameters eROIforTeachingParameters_;
        EROIParameters eROIforMatchingParameters_;
        PatternMatcherParameters PatternMatcherParameters_;
        MapColorCodeParameters MapColorCodeParameters_;
        BDMMapResultConfig BDMMapResultConfig_;
        MapVisionParameters MapVisionParameters_;
        Action<object, BDM_Images, ImageCategory> updateImages_;
        Action<bool, string, SequenceCategory> setSequencePassFail_;

        BDM_Images ImagesForUpdate_;
        EImageBW8 eImageTransformed_;

        Action<MasterRecipe.MasterRecipe, string> saveRecipeAction_;
        MasterRecipe.MasterRecipe selectedMasterRecipe_;
        string recipeFileDirectory_;

        //public void Subject(
        //	EuresysCircleGauge EuresysCircleGauge,
        //	WaferParameter WaferParameter,
        //	FiducialSearchParameter FiducialSearchParameter,
        //	GoldenCalibrationParameters GoldenCalibrationResult,
        //	ImageSavingParameters ImageSavingParameters,
        //	ImageDrawingParamaters ImageDrawingParameters,
        //	double PixelPerMM_X,
        //	double PixelPerMM_Y
        //	)
        //{
        //	if (EuresysCircleGauge == null) throw new ArgumentNullException("CircleGauge");
        //	if (WaferParameter == null) throw new ArgumentNullException("Wafer Parameter");
        //	if (FiducialSearchParameter == null) throw new ArgumentNullException("FiducialSearchParameters");
        //	if (GoldenCalibrationResult == null) throw new ArgumentNullException("Golden Calibration Result");
        //	if (ImageSavingParameters == null) throw new ArgumentNullException("ImageSavingParameters");
        //	if (ImageDrawingParameters == null) throw new ArgumentNullException("ImageDrawingParameters");

        //	this.EuresysCircleGauge = EuresysCircleGauge;
        //	this.WaferParameter = WaferParameter;
        //	this.FiducialSearchParameter = FiducialSearchParameter;
        //	this.GoldenCalibrationResult = GoldenCalibrationResult;
        //	this.ImageSavingParameters = ImageSavingParameters;
        //	this.ImageDrawingParameters = ImageDrawingParameters;
        //	this.FiducialTolerance =
        //		new Tuple<double, double>(WaferParameter.MinFiducialLength, WaferParameter.MaxFiducialLength);
        //	this.PixelPerMM_X = PixelPerMM_X;
        //	this.PixelPerMM_Y = PixelPerMM_Y;
        //}

        public void Subject(
            WaferCenterParameters waferCenterParameters,
            CorrectionParameters correctionParameters,
            EROIParameters EROIforTeachingParameters,
            EROIParameters EROIforMatchingParameters,
            PatternMatcherParameters PatternMatcherParameters,
            MapColorCodeParameters MapColorCodeParameters,
            BDMMapResultConfig BDMMapResultConfig,
            MapVisionParameters MapVisionParameters,
            Action<object, BDM_Images, ImageCategory> updateImages,
            Action<bool, string, SequenceCategory> setSequencePassFail,
            Action<MasterRecipe.MasterRecipe, string> saveRecipeAction,
            MasterRecipe.MasterRecipe selectedRecipe,
            string recipeFileDirectory)
        {
            this.waferCenterParameters_ = waferCenterParameters;
            this.correctionParameters_ = correctionParameters;
            this.eROIforTeachingParameters_ = EROIforTeachingParameters;
            this.eROIforMatchingParameters_ = EROIforMatchingParameters;
            this.PatternMatcherParameters_ = PatternMatcherParameters;
            this.MapColorCodeParameters_ = MapColorCodeParameters;
            this.BDMMapResultConfig_ = BDMMapResultConfig;
            this.MapVisionParameters_ = MapVisionParameters;
            this.updateImages_ = updateImages;
            this.saveRecipeAction_ = saveRecipeAction;
            this.selectedMasterRecipe_ = selectedRecipe;
            this.recipeFileDirectory_ = recipeFileDirectory;
            this.setSequencePassFail_ = setSequencePassFail;
        }

        //public InspectionResult<T> Execute(T Image)
        //{
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Image Processing (Inspection) Started");

        //	// Prepare
        //	#region Prepare
        //	Stopwatch Stopwatch_Processing = Stopwatch.StartNew();
        //	if (typeof(T) != typeof(BDM_Images)) throw new Exception("Processor: Image type not tally");
        //	BDM_Images Images = Image.CastToType<BDM_Images>();
        //	EImageBW8 EImage = Images.EImage;			
        //	ECircleGauge WaferGauge = new ECircleGauge(EuresysCircleGauge.ECircleGauge);
        //	double previousProcessTime = Images.ProcessTime_ms;
        //	#endregion Prepare

        //	// Reset Result
        //	#region Reset Result
        //	int ResultIndex = 0;
        //	bool Pass = false;
        //	string Remarks = string.Empty;
        //	double XOffset_mm = 0;
        //	double YOffset_mm = 0;
        //	double ThetaOffset_deg = 0;
        //	double AbsoluteDistance_mm = 0;
        //	double waferDiameter_mm = 0;
        //	Point FiducialPoint = new Point();
        //	Point WaferCenterPoint = new Point();
        //	Point ChuckCenterPoint = new Point();

        //	#endregion			

        //	// Step 1: Find Wafer Circle
        //	#region Find Circle			
        //	try
        //	{
        //		// Step 1: Find Circle
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Circle Started");
        //		FindEuresysCircle FindWaferCircle = new FindEuresysCircle();
        //		WaferType WaferType = WaferParameter.WaferType;
        //		if (WaferType == WaferType.None) throw new ArgumentException("Processor: Wafer Type cannot be None");
        //		WaferGauge = FindWaferCircle.Execute(EImage, WaferGauge, WaferType, true);
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Circle Complete");


        //	}
        //	catch
        //	{
        //		// -11: Wafer not Found
        //		ResultIndex = -11;
        //		Remarks = "Wafer Not Found";
        //		Pass = false;
        //		goto EndProcessing;
        //	}

        //	#endregion Find Circle	

        //	// Step 2: Find Wafer Centre Point (P1)
        //	#region Find Center
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Centre Point Started");
        //	FindEuresysCircleCenterPoint FindWaferCenterPoint = new FindEuresysCircleCenterPoint();
        //	WaferCenterPoint = FindWaferCenterPoint.Execute(WaferGauge);
        //	ChuckCenterPoint = GoldenCalibrationResult.ChuckCenter;
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Centre Point Complete");
        //	#endregion Find Center

        //	// Step 3: Find Wafer Diameter in MM
        //	#region Find Wafer Diameter
        //	waferDiameter_mm = WaferGauge.MeasuredCircle.Diameter / PixelPerMM_Average;
        //	#endregion Find Wafer Diameter

        //	// Step 4: Find XY Offset
        //	#region Find XYOffset
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find XY Offset Started");		

        //	InspectionJob_Helper help = new InspectionJob_Helper();
        //	Tuple<double, double> XYOffset = help.FindXYOffset(
        //		WaferCenterPoint,
        //		ChuckCenterPoint,
        //		GoldenCalibrationResult.ThetaXOffset_rad,
        //		GoldenCalibrationResult.ThetaYOffset_rad,
        //		GoldenCalibrationResult.XAlignDirection,
        //		GoldenCalibrationResult.YAlignDirection);

        //	double XOffset = XYOffset.Item1;
        //	double YOffset = XYOffset.Item2;
        //	XOffset_mm = XOffset / PixelPerMM_X;
        //	YOffset_mm = YOffset / PixelPerMM_Y;


        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find XY Offset Completed");
        //	#endregion Find XYOffset

        //	// Step 5: Find Absolute Offset
        //	#region Find Absolute Offset 
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Absolute Offset Started");
        //	double AbsDistance = help.FindAbsoluteOffset(XOffset, YOffset);
        //	AbsoluteDistance_mm = AbsDistance / PixelPerMM_Average;

        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Absolute Offset Completed");
        //	#endregion Find Absolute Offset

        //	// Step 6: Find Fiducial
        //	#region Find Fiducial
        //	FiducialResult FiducialResult = null;
        //	FindFiducialResult FindFiducialResult = new FindFiducialResult();
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find FiducialResult Started");

        //	goto first_algorithm;	
        //	first_algorithm:
        //	if (FiducialSearchParameter.IsEdgeRadiusAlgorithm)
        //	{
        //		#region NotchDetect Algorithm

        //		FiducialResult = FindFiducialResult.FindUsingEdgeRadiusAlgorithm(
        //			EImage,
        //			WaferGauge,
        //			WaferCenterPoint,
        //			FiducialSearchParameter.EdgeRadiusParameters);

        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find FiducialResult (EdgeRadius Algorithm) Executed");
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Number of Fiducial Found: " + FiducialResult.Count);

        //		#endregion NotchSearchAlgorithm

        //		if (FiducialResult.Count == 1) goto end_FiducialSearch;
        //		else goto second_algorithm;
        //	}

        //	second_algorithm:
        //	if (FiducialSearchParameter.IsSignalProcessingAlgorithm)
        //	{
        //		#region SignalProcessing Algorithm 

        //		// Fiducial Result
        //		FiducialResult = FindFiducialResult.FindUsingSignalProcessing(
        //			EImage,
        //			WaferGauge,
        //			WaferCenterPoint,
        //			FiducialSearchParameter.SignalProcessingSearchParameter,
        //			false,
        //			false,
        //			true);

        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find FiducialResult (Signal Processing) Executed");
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Number of Fiducial Found: " + FiducialResult.Count);

        //		#endregion SignalProcessing Algorithm 

        //		if (FiducialResult.Count == 1) goto end_FiducialSearch;
        //		else goto thrid_algorithm;
        //	}

        //	thrid_algorithm:
        //	if (FiducialSearchParameter.IsMinimumDistanceAlgorithm)
        //	{
        //		#region Minimum Distance Algorithm

        //		FiducialResult = FindFiducialResult.FindUsingMinimumDistanceAlgorithm(
        //			EImage,
        //			WaferGauge,
        //			WaferCenterPoint);

        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find FiducialResult (Minimum Distance) Executed");
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Number of Fiducial Found: " + FiducialResult.Count);

        //		#endregion Minimum Distance Algorithm

        //		if (FiducialResult.Count == 1) goto end_FiducialSearch;
        //		else goto end_FiducialSearch;
        //	}

        //	end_FiducialSearch:
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find FiducialResult Completed");

        //	#endregion Find Fiducial

        //	// Step 7: Set Fiducial Index from Fiducial Result			
        //	#region Set Fiducial Index

        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Fiducial Index Started");
        //	FindFiducialIndex FindFiducialIndex = new FindFiducialIndex();
        //	int FiducialIndex;
        //	try
        //	{
        //		FiducialIndex = FindFiducialIndex.Execute(FiducialResult);
        //	}

        //	catch (Exception e)
        //	{
        //		Pass = false;
        //		ResultIndex = -31;
        //		Remarks = e.Message;
        //		goto EndProcessing;
        //	}

        //	#endregion Set Fiducial Index

        //	// Step 8: Find Fiducial Point (P2)
        //	#region Find Fiducial Point
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Fiducial Point Started");

        //	FindFiducialPoint FindFiducialPoint = new FindFiducialPoint();
        //	FiducialPoint = FindFiducialPoint.Execute(EuresysCircleGauge.ECircleGauge, EImage, FiducialIndex);


        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Fiducial Point Completed");
        //	#endregion Find Fiducial Point

        //	// Step 9: Find Fiducial Radius
        //	#region Find Fiducial Radius
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Fiducial Radius Started");

        //	FindRadiusBetween2Points FindFiducialRadius = new FindRadiusBetween2Points();
        //	double fiducialRadius = FindFiducialRadius.Execute(FiducialPoint, WaferCenterPoint);
        //	double fifucialRadius_mm = fiducialRadius / PixelPerMM_Average;			

        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format( "Fiducial Radius is {0} mm" ,fiducialRadius));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Fiducial Radius Completed");
        //	#endregion Find Fiducial Radius

        //	// Step 10: Check Fiducial Length
        //	#region Check Fiducial Length
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Check Fiducial Length Started");

        //	double fiducialLength_mm;

        //	if (WaferParameter.WaferType == WaferType.Flat)
        //	{
        //		// Calculate Fiducial Length
        //		fiducialLength_mm = Math.Sqrt(
        //			(Math.Pow(EuresysCircleGauge.MeasuredDiameter / 2, 2) -
        //			(Math.Pow(fiducialRadius, 2))));

        //		double minLength = FiducialTolerance.Item1;
        //		double maxLength = FiducialTolerance.Item2;

        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Min Fiducial Length is " + minLength);
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Max Fiducial Length is " + maxLength);
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Fiducial Length is " + fiducialLength_mm);

        //		// Check Tolerance
        //		if ((fiducialLength_mm <= minLength || fiducialLength_mm >= maxLength))
        //		{
        //			Pass = false;
        //			ResultIndex = -21;
        //			Remarks = "Fiducial Out of Tolerance";
        //			goto EndProcessing;
        //		}
        //	}

        //	if (WaferParameter.WaferType == WaferType.Notch)
        //	{
        //		VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Wafer Type is Notch, Check Fiducial Length Skipped");
        //	}

        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Check Fiducial Length Completed");

        //	#endregion Find Fiducial Length

        //	// Step 11: Find Theta Offset
        //	#region Find Theta Offset
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Theta Offset Started");				
        //	ThetaOffset_deg = help.FindThetaOffset(
        //								WaferCenterPoint,
        //								FiducialPoint,
        //								ChuckCenterPoint,
        //								GoldenCalibrationResult.ThetaXOffset_rad,
        //								GoldenCalibrationResult.ThetaYOffset_rad,
        //								GoldenCalibrationResult.XAlignDirection,
        //								GoldenCalibrationResult.YAlignDirection,
        //								GoldenCalibrationResult.RotationDirection);

        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Find Theta Offset Completed");
        //	#endregion Find Theta Offset				

        //	// Pass
        //	#region SetResultPass
        //	Pass = true;
        //	ResultIndex = 1;
        //	Remarks = "Process Completed";
        //	#endregion SetResultPass

        //	// End
        //	#region End Processing
        //	EndProcessing:
        //	Stopwatch_Processing.Stop();
        //	double TotalElapsedTime = previousProcessTime + Stopwatch_Processing.ElapsedMilliseconds;
        //	#endregion End Processing 

        //	// Draw Destructive Image
        //	#region Draw Destructive Image

        //	bmptemp = new System.Drawing.Bitmap(Images.bmpImage.Width, Images.bmpImage.Height);
        //	double zoom = 1;

        //	// Draw Destructive 
        //	using (System.Drawing.Graphics g_destructive = System.Drawing.Graphics.FromImage(bmptemp))
        //	{
        //		#region Draw EImage

        //		Images.EImage.Draw(g_destructive, (float)zoom);

        //		#endregion

        //		#region Draw VisionFrameAxes

        //		DrawFrameAxes DrawVisionFrameAxes = new DrawFrameAxes();
        //		DrawVisionFrameAxes.DrawVisionAxes(
        //			g_destructive,
        //			bmptemp.Width,
        //			bmptemp.Height,
        //			ImageDrawingParameters.VisionAxesThickness,
        //			ImageDrawingParameters.VisionAxesColor);

        //		#endregion Draw VisionFrameAxes

        //		#region Draw MachineFrameAxes

        //		if (GoldenCalibrationResult.ChuckCenter.X == 0 && GoldenCalibrationResult.ChuckCenter.Y == 0)
        //		{
        //			double imgCenterX = bmptemp.Width / 2;
        //			double imgCenterY = bmptemp.Height / 2;
        //			GoldenCalibrationResult.ChuckCenter = new System.Windows.Point(imgCenterX, imgCenterY);
        //		}

        //		DrawVisionFrameAxes.DrawMachineAxes(
        //			g_destructive,
        //			bmptemp.Width,
        //			bmptemp.Height,
        //			ImageDrawingParameters.MachineAxesThickness,
        //			ImageDrawingParameters.MachineAxesColor,
        //			GoldenCalibrationResult,
        //			zoom);

        //		#endregion Draw MachineFrameAxes

        //		#region Draw Fiducial Line

        //		if (Pass)
        //		{
        //			DrawLineFromPointToPoint DrawLineFromPointToPoint = new DrawLineFromPointToPoint();
        //			DrawLineFromPointToPoint.Execute(
        //				g_destructive,
        //				WaferCenterPoint,
        //				FiducialPoint,
        //				ImageDrawingParameters.FiducialLineColor,
        //				ImageDrawingParameters.FiducialLineThickness,
        //				zoom);
        //		}

        //		#endregion Draw Fiducial Line

        //		#region Draw Pass or Fail

        //		DrawString DrawResultString = new DrawString();
        //		string PassOrFail;
        //		System.Drawing.Color stringColor;

        //		if (Pass)
        //		{
        //			PassOrFail = "PASS";
        //			stringColor = System.Drawing.Color.LightGreen;
        //		}
        //		else
        //		{
        //			PassOrFail = "FAIL";
        //			stringColor = System.Drawing.Color.Red;
        //		}

        //		DrawResultString.Execute(
        //			g_destructive,
        //			PassOrFail,
        //			stringColor,
        //			ImageDrawingParameters.ResultPositionX,
        //			ImageDrawingParameters.ResultPositionY,
        //			ImageDrawingParameters.ResultStringSize,
        //			zoom);
        //		#endregion Draw Pass or Fail

        //		#region Draw Details			

        //		DrawBackedText DrawBackedText = new DrawBackedText();
        //		double textSize = ImageDrawingParameters.DetailsStringSize;
        //		double details_posX = ImageDrawingParameters.DetailsPositionX;
        //		double details_posY = ImageDrawingParameters.DetailsPositionY;

        //		// Draw Filled BackText if Result Failed

        //		if (!Pass || 
        //			(ImageDrawingParameters.IsDrawDetails && ImageDrawingParameters.IsFillDetails))
        //		{
        //			#region Draw Filled Text

        //			// Draw Result Index
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"Result Index      : " + ResultIndex,
        //				textSize,
        //				details_posX,
        //				details_posY,
        //				zoom);

        //			// Draw Remarks
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"Remarks           : " + Remarks,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 1 * 2,
        //				zoom);

        //			// Draw Center X Offset
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"X Offset (mm)     : " + XOffset_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 2 * 2,
        //				zoom);

        //			// Draw Center Y Offset
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"Y Offset (mm)     : " + YOffset_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 3 * 2,
        //				zoom);

        //			// Draw Theta Offset
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"Theta Offset (deg): " + ThetaOffset_deg,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 4 * 2,
        //				zoom);

        //			// Draw Center Absolute Offset
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"Abs Offset (mm)   : " + AbsoluteDistance_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 5 * 2,
        //				zoom);

        //			// Draw Detected Wafer Size
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"Wafer Size (mm)   : " + waferDiameter_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 6 * 2,
        //				zoom);

        //			// Draw PixelPerMM_X 
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"PixelPerMM_X      : " + PixelPerMM_X,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 7 * 2,
        //				zoom);

        //			// Draw PixelPerMM_Y 
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"PixelPerMM_Y      : " + PixelPerMM_Y,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 8 * 2,
        //				zoom);

        //			// Draw Process Time
        //			DrawBackedText.DrawFillText(
        //				g_destructive,
        //				"Process Time (ms): " + TotalElapsedTime,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 9 * 2,
        //				zoom);

        //			#endregion Draw Filled Text
        //		}

        //		else if (ImageDrawingParameters.IsDrawDetails && !ImageDrawingParameters.IsFillDetails)
        //		{
        //			#region Draw Normal Text
        //			System.Drawing.Color textColor = ImageDrawingParameters.DetailsColor;

        //			// Draw Result Index
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"Result Index      : " + ResultIndex,
        //				textSize,
        //				details_posX,
        //				details_posY,
        //				zoom);

        //			// Draw Remarks
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"Remarks           : " + Remarks,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 1 * 2,
        //				zoom);

        //			// Draw Center X Offset
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"X Offset (mm)     : " + XOffset_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 2 * 2,
        //				zoom);

        //			// Draw Center Y Offset
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"Y Offset (mm)     : " + YOffset_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 3 * 2,
        //				zoom);

        //			// Draw Theta Offset
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"Theta Offset (deg): " + ThetaOffset_deg,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 4 * 2,
        //				zoom);

        //			// Draw Center Absolute Offset
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"Abs Offset (mm)   : " + AbsoluteDistance_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 5 * 2,
        //				zoom);

        //			// Draw Detected Wafer Size
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"Wafer Size (mm)   : " + waferDiameter_mm,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 6 * 2,
        //				zoom);

        //			// Draw PixelPerMM_X 
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"PixelPerMM_X      : " + PixelPerMM_X,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 7 * 2,
        //				zoom);

        //			// Draw PixelPerMM_Y 
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"PixelPerMM_Y      : " + PixelPerMM_Y,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 8 * 2,
        //				zoom);

        //			// Draw Process Time
        //			DrawBackedText.DrawNormalText(
        //				g_destructive,
        //				textColor,
        //				"Process Time (ms): " + TotalElapsedTime,
        //				textSize,
        //				details_posX,
        //				details_posY + textSize * 9 * 2,
        //				zoom);
        //			#endregion
        //		}
        //		#endregion
        //	}

        //	Images.bmpImage_destructive = bmptemp;
        //	#endregion Draw Destructive Image

        //	// Log
        //	#region LogResult

        //	// Log Result
        //	LogType temp = LogType.Log;
        //	if (ResultIndex != 1) temp = LogType.Exception;

        //	VisionLogger.Log(temp, GetType().ToString(), string.Format("IsResult Pass: {0}", Pass));
        //	VisionLogger.Log(temp, GetType().ToString(), string.Format("Remarks: {0}", Remarks));
        //	VisionLogger.Log(temp, GetType().ToString(), string.Format("Error Code is {0}" , ResultIndex));

        //	// Log Offset
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("X Offset is {0} mm", XOffset_mm));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Y Offset is {0} mm", YOffset_mm));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Absolute Offset is {0} mm", AbsoluteDistance_mm));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Theta Offset is {0} deg", ThetaOffset_deg));

        //	// Log Pixel Per MM
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("PixelPerMM X is {0} px", PixelPerMM_X));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("PixelPerMM Y is {0} px", PixelPerMM_Y));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("PixelPerMM Average is {0} px", PixelPerMM_Average));

        //	// Log Wafer Parameters
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Wafer Diameter is {0} mm", waferDiameter_mm));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Wafer Center Point X is {0} px", WaferCenterPoint.X));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Wafer Center Point Y is {0} px", WaferCenterPoint.Y));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Fiducial Point X is {0} px", FiducialPoint.X));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Fiducial Point Y is {0} px", FiducialPoint.Y));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Chuck Point X is {0} px", ChuckCenterPoint.X));
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Chuck Point Y is {0} px", ChuckCenterPoint.Y));

        //	// Log Processing Time
        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), string.Format("Processing Time Elapsed: {0} ms", TotalElapsedTime));

        //	#endregion LogResult

        //	VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Image Processing (Inspection) Completed");

        //	// Specifiy all results needed here
        //	return new InspectionResult<T>()
        //	{
        //		Pass = Pass,
        //		Remarks = Remarks,
        //		ResultIndex = ResultIndex,
        //		XOffset_mm = XOffset_mm,
        //		YOffset_mm = YOffset_mm,
        //		AbsoluteDistance_mm = AbsoluteDistance_mm,
        //		ThetaOffset_deg = ThetaOffset_deg,
        //		WaferSize_Detected_mm = waferDiameter_mm,
        //		WaferCenterPoint = WaferCenterPoint,
        //		FiducialPoint = FiducialPoint,
        //		ChuckCenterPoint = ChuckCenterPoint,				
        //		GoldenCalibrationParameters = GoldenCalibrationResult,
        //		ImageSavingParameters = ImageSavingParameters,
        //		FinalImage = Image,
        //		PixelPerMM_X = PixelPerMM_X,
        //		PixelPerMM_Y = PixelPerMM_Y,
        //		ProcessTime_ms = TotalElapsedTime,
        //	};
        //}

        /// <summary>
        /// Error Code Definition
        /// Default                 : -1
        /// WaferCenter             : -2
        /// Correction              : -3
        /// Teaching                : -4
        /// Matching                : -5
        /// Rotation By Map File    : -6
        /// Collect Map Vision Data : -7
        /// Collect BDM Result      : -8
        /// Result out of tolerance : -9
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        public BDMInspectionResult Execute(BDM_Images Image)
        {
            int errorCode = -1;
            string remarks = string.Empty;
            SequenceCategory currentSequence = SequenceCategory.Undefined;

            try
            {
                #region Intialize

                Stopwatch SW = Stopwatch.StartNew();
                currentSequence = SequenceCategory.Initialize;
                remarks = "Intialize";
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Processor Started");

                setSequencePassFail_?.Invoke(false, "Reset", SequenceCategory.MapFile);
                setSequencePassFail_?.Invoke(false, "Reset", SequenceCategory.ImageAcquisition);
                setSequencePassFail_?.Invoke(false, "Reset", SequenceCategory.MapColor);
                setSequencePassFail_?.Invoke(false, "Reset", SequenceCategory.PatternMatching);
                setSequencePassFail_?.Invoke(false, "Reset", SequenceCategory.MapVision);

                #endregion Initialize

                #region Map File

                currentSequence = SequenceCategory.MapFile;

                remarks = "Preparing Map File";

                if (!BDMMapFileReader.IsMapDataPresent) throw new Exception("Map File Not Loaded");
                MapDataFromFile mapDataFromFile = BDMMapFileReader.MapDataFromFile;
                setSequencePassFail_?.Invoke(true, "Map File Loaded", SequenceCategory.MapFile);

                #endregion Map File

                #region Preparing Image

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Prepare Images Started");
                currentSequence = SequenceCategory.ImageAcquisition;
                remarks = "Preparing Image";
                EImageBW8 eImage = Image.EImage;
                if (eImageTransformed_ != null) eImageTransformed_.Dispose();
                eImageTransformed_ = new EImageBW8(eImage);
                if (ImagesForUpdate_ == null) ImagesForUpdate_ = new BDM_Images();
                else if (ImagesForUpdate_ != null) ImagesForUpdate_.Dispose();
                setSequencePassFail_?.Invoke(true, "Image Ready", SequenceCategory.ImageAcquisition);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Prepare Images Completed");

                #endregion Preparing Image

                #region Map Color

                currentSequence = SequenceCategory.MapColor;
                setSequencePassFail_?.Invoke(true, "", SequenceCategory.MapColor);

                #endregion Map Color

                #region Wafer Center

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Find Wafer Center Started");
                currentSequence = SequenceCategory.PatternMatching;
                remarks = "Finding Wafer Center";
                errorCode = -2;
                const float filterTolerance = 100;
                const float fiducialOffset = 10;

                ELineGauge LineGauge_Top = WaferCenterParametersHelper.MakeELineGauge(waferCenterParameters_.LineGaugeParameters_1);
                ELineGauge LineGauge_Right = WaferCenterParametersHelper.MakeELineGauge(waferCenterParameters_.LineGaugeParameters_2);
                ELineGauge LineGauge_Bottom = WaferCenterParametersHelper.MakeELineGauge(waferCenterParameters_.LineGaugeParameters_3);
                ELineGauge LineGauge_Left = WaferCenterParametersHelper.MakeELineGauge(waferCenterParameters_.LineGaugeParameters_4);

                if (BDMMapFileReader.IsMapDataPresent == false) throw new Exception("Map Data is not present.");
                WaferOrientation fiducialOrientation;
                fiducialOrientation = (WaferOrientation)mapDataFromFile.WaferOrientation;

                PointF WaferCenterPoint = WaferCenterParametersHelper.FindCenter_MainMethod(
                    LineGauge_Top,
                    LineGauge_Right,
                    LineGauge_Bottom,
                    LineGauge_Left,
                    eImage,
                    filterTolerance,
                    fiducialOffset,
                    fiducialOrientation,
                    false);

                WaferCenterAndCornerStorage.SetImageCenter(WaferCenterPoint);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Wafer Center X is " + WaferCenterPoint.X);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Wafer Center Y is " + WaferCenterPoint.Y);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Find Wafer Center Completed");

                #endregion Wafer Center

                #region Find Corners

                PointF topLeftCorner = WaferCenterParametersHelper.FindTopLeftCornerPoint(
                    LineGauge_Top,
                    LineGauge_Left,
                    eImageTransformed_,
                    filterTolerance);

                PointF bottomRightCorner = WaferCenterParametersHelper.FindBottomRightCornerPoint(
                    LineGauge_Bottom,
                    LineGauge_Right,
                    eImageTransformed_,
                    filterTolerance);

                WaferCenterAndCornerStorage.SetTopLeftCorner(topLeftCorner);
                WaferCenterAndCornerStorage.SetBottomRightCorner(bottomRightCorner);

                #endregion Find Corners

                #region Correction

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Correction Started");
                remarks = "Image Correction";
                errorCode = -3;
                eImageTransformed_ = ImageTransformer.RotateImage(
                    eImageTransformed_,
                    (float)correctionParameters_.RotationOffset,
                    WaferCenterPoint);

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Image Rotataion By " + correctionParameters_.RotationOffset);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Correction Completed");

                #endregion Correction

                #region Teaching

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), " Pattern Teaching/Loading Started");
                remarks = "Patern teaching / Loading";
                errorCode = -4;

                PatternMatcherParameters patternMatcherParameters = PatternMatcherParameters_;
                TeacherROIOrientaion teacherOrientation = patternMatcherParameters.TeacherROI_Orientation;
                iEuresysROI TeacherEROI = EuresysEROIHelper.CreateROIwithDoubleSubROI(eROIforTeachingParameters_, eImageTransformed_, teacherOrientation);
                iEuresysROI MatcherEROI = EuresysEROIHelper.CreateEuresysROI(eROIforMatchingParameters_, eImageTransformed_);

                string PatternFilePath_One = PatternMatcherParameterHelper.GetLeftPatternFilePath(selectedMasterRecipe_.Name);
                string PatternFilePath_Two = PatternMatcherParameterHelper.GetRightPatternFilePath(selectedMasterRecipe_.Name);
                string PatternImageFilePath_One = PatternMatcherParameterHelper.GetLeftPatternImageFilePath(selectedMasterRecipe_.Name);
                string PatternImageFilePath_Two = PatternMatcherParameterHelper.GetRightPatternImageFilePath(selectedMasterRecipe_.Name);

                EuresysDoublePatternMatcher PatternMatcher_ = null;
                PointF waferCenterPoint = WaferCenterPoint;
                PointF teacherMidPoint =
                    new PointF(TeacherEROI.OriginX + TeacherEROI.Width / 2, TeacherEROI.OriginY + TeacherEROI.Height / 2);
                WaferOrientation patternOrientation = PatternMatchingTransformation.FindWaferOrientation(waferCenterPoint, teacherMidPoint);
                PatternMatchingTransformationResult tf_result = null;
                bool IsIgnorePattern = patternMatcherParameters.IsIgnorePattern;
                bool IsDisableNotification = patternMatcherParameters.IsDisableNotification;

                // Try Load, if fail create new Pattern (blank)              
                try
                {
                    VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Try to Load Patern");

                    PatternMatcherParameterHelper.CheckPatternFilePath(PatternFilePath_One);
                    PatternMatcherParameterHelper.CheckPatternFilePath(PatternFilePath_Two);
                    PatternMatcherParameterHelper.CheckPatternImageFilePath(PatternImageFilePath_One);
                    PatternMatcherParameterHelper.CheckPatternImageFilePath(PatternImageFilePath_Two);
                    PatternMatcher_ = PatternMatcherParameterHelper.CreateDoublePatternMatcher(PatternMatcherParameters_);
                    PatternMatcher_.LoadPatterns(PatternFilePath_One, PatternFilePath_Two, PatternImageFilePath_One, PatternImageFilePath_Two);

                    VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Pattern Loaded Sucessfully");
                }
                catch (Exception)
                {
                    VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, this.GetType().ToString(), "Pattern Failed to Load, Creating Blank Pattern");

                    PatternMatcher_ = PatternMatcherParameterHelper.CreateDoublePatternMatcher(PatternMatcherParameters_);
                    PatternMatcher_.SaveEMatcher_Blank(
                        PatternMatcherParameterHelper.StandardPatternMatcherParameters,
                        PatternFilePath_One,
                        PatternFilePath_Two,
                        PatternImageFilePath_One,
                        PatternImageFilePath_Two);
                    PatternMatcher_.LoadPatterns(PatternFilePath_One, PatternFilePath_Two, PatternImageFilePath_One, PatternImageFilePath_Two);
                    IsIgnorePattern = true;

                    VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, this.GetType().ToString(), "Ignore Pattern Set to True due to Blank Pattern");
                }               

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), " Pattern Teaching/Loading Completed");

                #endregion Teaching 

                #region Matching

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), " Pattern Matching Started");
                remarks = "Pattern Matching";
                errorCode = -5;
                EROIBW8 matcherROI = MatcherEROI.GetROI(0);

                // Match Pattern to Decide whether to ignore Pattern
                try
                {
                    VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), " Matching Pattern to Check Whether Pattern Exists");               
                    var result = PatternMatcher_.MatchPatterns(matcherROI);
                    if (result.Item1.Count != 1) throw new Exception("Pattern 1 Not Found");
                    if (result.Item2.Count != 1) throw new Exception("Pattern 2 Not Found");
                }
                catch (Exception ex)
                {
                    if (!IsDisableNotification) VisionNotifier.AddNotification("Warning: " + ex.Message);
                    VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, GetType().ToString(), ex);
                    VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, this.GetType().ToString(), ex);
                    IsIgnorePattern = true;
                }                

                // XY Transformation without Rotation
                if (IsIgnorePattern)
                {
                    #region Ignore Pattern

                    VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Executing Image Transformation without Rotation");

                    // AT = autoteach
                    float maxTranslationOffset_AT = patternMatcherParameters.MaxTranslateOffset_px;
                    tf_result = PatternMatchingTransformation.ExecuteWithoutRotation(
                       eImageTransformed_,
                       maxTranslationOffset_AT,
                       maxTranslationOffset_AT,
                       5,
                       patternMatcherParameters,
                       LineGauge_Top,
                       LineGauge_Right,
                       LineGauge_Bottom,
                       LineGauge_Left,
                       filterTolerance,
                       fiducialOffset,
                       patternOrientation,
                       patternMatcherParameters.IsIncludeFiducialTolerance,
                       WaferCenterParametersHelper.FindCenter_MainMethod);

                    VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Image Transformation without Rotation Executed");

                    #endregion Ignore Pattern
                }

                else
                {
                    #region Use Pattern

                    VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Executing Image Transformation With Pattern");
                    float maxAngleOffset = patternMatcherParameters.MaxAngleOffset_deg;
                    float maxTranslationOffset = patternMatcherParameters.MaxTranslateOffset_px;

                    tf_result = PatternMatchingTransformation.Execute(
                                eImageTransformed_,
                                maxAngleOffset,
                                maxTranslationOffset,
                                maxTranslationOffset,
                                5,
                                PatternMatcher_,
                                patternMatcherParameters,
                                matcherROI,
                                LineGauge_Top,
                                LineGauge_Right,
                                LineGauge_Bottom,
                                LineGauge_Left,
                                filterTolerance,
                                fiducialOffset,
                                patternOrientation,
                                patternMatcherParameters.IsIncludeFiducialTolerance,
                                WaferCenterParametersHelper.FindCenter_MainMethod,
                                teacherMidPoint);

                    VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Image Transformation With Pattern Executed");


                    #endregion UsePattern
                }

                eImageTransformed_ = tf_result.eImageAfterTransformation;
                float FinalAngleOffset = tf_result.FinalAngleOffset;
                float FinalXOffset = tf_result.FinalXOffset;
                float FinalYOffset = tf_result.FinalYOffset;
                bool IsXOffsetWithinTolerance = tf_result.IsXOffsetWithinTolerance;
                bool IsYOffsetWithinTolerance = tf_result.IsYOffsetWithinTolerance;
                bool IsThetaWithinTolerance = tf_result.IsThetaOffsetWithinTolerance;
                bool IsPatternMatchingSucessful = tf_result.IsPass;

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Final X Offset is " + FinalXOffset.ToString("F3"));
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Final Y Offset is " + FinalYOffset.ToString("F3"));
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Final Theta Offset is " + FinalAngleOffset.ToString("F3"));
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Result Pass is " + IsPatternMatchingSucessful);

                if (!IsXOffsetWithinTolerance) throw new Exception("X Offset out of tolerance");
                if (!IsYOffsetWithinTolerance) throw new Exception("Y Offset out of tolerance");
                if (!IsThetaWithinTolerance) throw new Exception("Theta Offset out of tolerance");

                if (IsPatternMatchingSucessful) setSequencePassFail_?.Invoke(true, "Pattern Matching Pass", SequenceCategory.PatternMatching);
                BDM_Images PatternMatching_Image = new BDM_Images()
                {
                    EImage = new EImageBW8(eImageTransformed_),
                };
                updateImages_?.Invoke(this, PatternMatching_Image, ImageCategory.PatternMatching);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), " Pattern Matching Completed");

                #endregion Matching        

                #region Rotation by Map File

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Rotation by User Defined Orientation");
                currentSequence = SequenceCategory.MapVision;
                int UserDefineWaferOrientation = MapVisionParameters_.WaferOrientation;
                PointF ImageCenter = new PointF(eImageTransformed_.Width / 2, eImageTransformed_.Height / 2);
                eImageTransformed_ = ImageTransformer.RotateImage(eImageTransformed_, UserDefineWaferOrientation, ImageCenter);
                remarks = "Rotation by User Defined Orientation";                
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Rotation by User Defined Orientation");

                #endregion Rotation by Map File

                #region Collect Map Data From Vision 

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Collect Map Data From Vision Started");
                remarks = "Collect Map Data From Vision";
                errorCode = -7;
                Func<MapVisionParameters, EImageBW8, MapDataFromVision> mapVisionReaderMethod = MapVisionReaderLibraries.ReadImage;
                MapVisionReader.MapVisionReader MapVisionReader = new MapVisionReader.MapVisionReader(mapVisionReaderMethod);
                MapDataFromVision MapDataFromVision = MapVisionReader.ReadImage_(
                    MapVisionParameters_,
                    eImageTransformed_);

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Collect Map Data From Vision Completed");

                #endregion Collect Map Data From Vision 

                #region Collect BDM Result
                
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Collect BDM Result Started");
                remarks = "Collect BDM Result";
                errorCode = -8;
                MapResultMatcher matcher = new MapResultMatcher(MapResultMatcherLibraries.Match);

                MapResult.BDMResult BDMResults = matcher.Execute
                    (mapDataFromFile,
                    MapDataFromVision,
                    this.MapColorCodeParameters_, 
                    this.BDMMapResultConfig_);            
 

                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Good Die Yield is " + BDMResults.GoodDieYield);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Bad Die Yield is "  + BDMResults.BadDieYield);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Ignore Die Yield is " + BDMResults.IgnoreDieYield);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Overall Result Pass is " + BDMResults.IsOverallResultPass);               
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Collect BDM Result Completed");

                bool isOverallResultPass = BDMResults.IsOverallResultPass;
                if (isOverallResultPass)
                {
                    setSequencePassFail_?.Invoke(true, "OVerall Result Pass", SequenceCategory.MapVision);
                    remarks = "Pass";
                    errorCode = 0; 
                }
                else
                {
                    if (!BDMResults.IsGoodDieYieldAcceptable)           remarks = "Good Die Yield Not Acceptable";
                    else if (!BDMResults.IsBadDieYieldAcceptable)       remarks = "Bad Die Yield Not Acceptable";
                    else if (!BDMResults.IsIgnoreDieYieldAcceptable)    remarks = "Ignore Die Yield Not Acceptable";
                    setSequencePassFail_?.Invoke(false, remarks, SequenceCategory.MapVision);
                    errorCode = -9;
                }

                #endregion Collect BDM Result

                #region Ending

                SW.Stop();
                double ProcessTime_ms = SW.ElapsedMilliseconds;
         
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Result Index is " + errorCode);
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Processor Completed");
                VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, GetType().ToString(), "Processor Prcoess Time is " + ProcessTime_ms.ToString("F3"));

                return new BDMInspectionResult()
                {
                    Pass = isOverallResultPass,
                    Remarks = remarks,
                    ProcessTime_ms = ProcessTime_ms,
                    ResultIndex = errorCode,
                    BDMResult = BDMResults,
                    Sequence = currentSequence,
                };

                #endregion Ending 
            }

            catch (Exception ex)
            {
                VisionLogger.Log(
                    WaftechLibraries.Log.LogType.Exception,
                    this.GetType().ToString(),
                    ex.Message + " Error Code is: " + errorCode);

                VisionNotifier.AddNotification(ex.Message + " Error Code is: " + errorCode);

                throw ex;
            }
        }
    }
}
