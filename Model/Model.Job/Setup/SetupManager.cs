using BDMVision.Model.Camera;
using BDMVision.Model.Enum;
using BDMVision.Model.Image;
using BDMVision.Model.ImageAcquisition;
using BDMVision.Model.LightingController;
using BDMVision.Model.Log;
using BDMVision.Model.VisionResult;
using Euresys.Open_eVision_2_0;
using System;
using System.Collections.Generic;
using WaftechLibraries.Log;

namespace BDMVision.Model.Job
{
    public class InspectionJob_SetupManager
	{
		private MasterRecipe.MasterRecipe BDMVisionMasterRecipe { get; set; }
		private MasterConfig.MasterConfig BDMVisionMasterConfig { get; set; }		
		public VisionJob<BDM_Images, BDMInspectionResult> Setup(
            MasterRecipe.MasterRecipe BDMVisionMasterRecipe,
            MasterConfig.MasterConfig BDMVisionMasterConfig,
            string masterRecipeFileDirectory,
            VisionJob<BDM_Images, BDMInspectionResult> InspectionJob,
            Action<bool, string, SequenceCategory> setSequencePassFail,           
            ICamera<EImageBW8> Camera,
            iLightingController LightingController)
		{
			VisionLogger.Log(LogType.Sequence, this.GetType().ToString(), "Inspection Job Setup Started");

			if (BDMVisionMasterRecipe == null) throw new ArgumentNullException("BDMVisionMasterRecipe");
			if (BDMVisionMasterConfig == null) throw new ArgumentNullException("BDMVisionMasterConfig");
			if (InspectionJob == null) throw new ArgumentNullException("Inspection Job");

			this.BDMVisionMasterRecipe = BDMVisionMasterRecipe;

			// Image Acquisition
			ImageAcquisitionParameter ImageAcquisitionParameter = BDMVisionMasterConfig.ImageAcquisitionParameter;
            float resizeRatio = BDMVisionMasterConfig.ImageLogParameters.SizeMultiplier;

            if (ImageAcquisitionParameter == null) throw new ArgumentNullException("ImageAcquisitionParameter");

            if (ImageAcquisitionParameter.ImageAcquisitionMethod == ImageAcquisitionMethod.LoadFromFile)
			{
                LoadFromFile LoadFromFile = new LoadFromFile();
                LoadFromFile.Setup(
                    ImageAcquisitionParameter.ImageFilePath,
                    setSequencePassFail, 
                    ImageStorage.ImageStorage.UpdateImages,
                    ImageAcquisitionParameter.IsResizeImage,
                    resizeRatio);
                InspectionJob.ImageAcquisition = LoadFromFile.LoadEImageOnly;
			}

			else if (ImageAcquisitionParameter.ImageAcquisitionMethod == ImageAcquisitionMethod.Camera)
			{
				LoadFromCamera LoadFromCamera = new LoadFromCamera();
                LoadFromCamera.SetupCamera(
                    ImageAcquisitionParameter.TimeOut_ms,
                    Camera,
                    setSequencePassFail,
                    ImageStorage.ImageStorage.UpdateImages);
                InspectionJob.ImageAcquisition = LoadFromCamera.LoadEImage;
			}

			else throw new NotImplementedException("Image Acquisiton Method not valid");

			// PreProcess
			PreProcessor PreProcessor = new PreProcessor();
			InspectionJob.Preprocess = PreProcessor.ExceuteWithoutProcessor;

			// Process
			InspectionJob_Processor<BDM_Images> Processor = new InspectionJob_Processor<BDM_Images>();

            // Acquire Processor Parameters
            Processor.Subject(
                BDMVisionMasterRecipe.WaferCenterParameters,
                BDMVisionMasterRecipe.CorrectionParameters,
                BDMVisionMasterRecipe.EROIforTeachingParameters,
                BDMVisionMasterRecipe.EROIforMatchingParameters,
                BDMVisionMasterRecipe.PatternMatcherParameters,
                BDMVisionMasterConfig.MapColorCodeParameters,
                BDMVisionMasterRecipe.BDMMapResultConfig,
                BDMVisionMasterRecipe.MapVisionParameters,
                ImageStorage.ImageStorage.UpdateImages,
                setSequencePassFail,
                MasterRecipeHelper.SerializeSingleRecipe,
                BDMVisionMasterRecipe,
                masterRecipeFileDirectory);
            InspectionJob.Process = Processor.Execute;

            // Post Process
            PostProcessor<BDM_Images> PostProcessor = new PostProcessor<BDM_Images>();
            Action CloseCamera = null;
            Action SetLightOff = null;
            if (Camera != null) CloseCamera = Camera.Disconnect;
            if (LightingController != null &&
                LightingController.IsLightON)
            {
                SetLightOff = LightingController.SetLightOff;
            }
            PostProcessor.Setup(
                CloseCamera,
                SetLightOff);

			InspectionJob.PostProcess = PostProcessor.Execute;

			VisionLogger.Log(LogType.Sequence, this.GetType().ToString(), "Inspection Job Setup Completed");
			return InspectionJob;
		}
	}

}

