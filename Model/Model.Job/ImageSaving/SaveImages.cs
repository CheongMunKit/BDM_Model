using System;
using System.Drawing;
using System.IO;
using BDMVision.Model.Image;
using BDMVision.Model.Log;
using BDMVision.Model.MasterConfig;
using BDMVision.Model.VisionResult;
using WaftechLibraries.Log;

namespace BDMVision.Model.Job
{
	public enum ImageType
	{
		Original,
		Destructive,
		Calibration
	}

	public class SaveImagesToFile
	{
		/// <summary>
		/// Saving Inspection Images
		/// Delete Obsolete Images
		/// </summary>
		/// <param name="result"></param>
		public void SaveInspectionImage(
			InspectionResult<BDM_Images> result)
		{
			if (result == null) throw new ArgumentNullException("result");
			if (result.FinalImage == null) throw new ArgumentNullException("result.FinalImage");
			if (result.FinalImage.bmpImage == null) throw new ArgumentNullException("result.FinalImage.bmpImage");
			if (result.FinalImage.bmpImage_destructive == null) throw new ArgumentNullException("result.FinalImage.bmpImage_destructive");
			if (result.ImageSavingParameters == null) throw new ArgumentNullException("result.ImageSavingParameters");

			SaveInspectionImage(
				result.FinalImage,
				result.ImageSavingParameters.IsSavePassInspectionImage,
				result.ImageSavingParameters.IsSaveFailInspectionImage,
				result.ImageSavingParameters.IsReduceImageQuality,
				result.ImageSavingParameters.IsSaveFailImageInSeparatedDirectory,
				result.ImageSavingParameters.InspectionImageFileDirectory,
				result.ImageSavingParameters.ExtensionPathForSeparatedDirectory,
				result.ImageSavingParameters.IsSaveInBothDirectories,
				result.ImageSavingParameters.ImageQualityReductionMultiplier,
				result.ImageSavingParameters.SavedImageFormat,
				result.ResultIndex);
		}

		private void SaveInspectionImage(
			BDM_Images finalImage,
			bool isSavePassInspectionImage,
			bool isSaveFailInspectionImage,
			bool isReduceImageQuality,
			bool isSaveFailImageInSeparatedDirectory,
			string inspectionImageFileDirectory,
			string extensionPathForSeparatedDirectory,
			bool isSaveInBothDirectories,
			ImageQualityReductionMultiplier imageQualityReductionMultiplier,
			ImageFormat imageFormat, 
			int resultIndex)
		{
			// Get Time
			string hourSecond = DateTime.Now.ToString("HHmmssff");

			// Reduce Image Quality
			if (isReduceImageQuality)
			{
				double reductionRatio = 1.0;

				if (imageQualityReductionMultiplier == ImageQualityReductionMultiplier.two) reductionRatio = 2;
				else if (imageQualityReductionMultiplier == ImageQualityReductionMultiplier.four) reductionRatio = 4;
				else if (imageQualityReductionMultiplier == ImageQualityReductionMultiplier.eight) reductionRatio = 8;
				else if (imageQualityReductionMultiplier == ImageQualityReductionMultiplier.sixteen) reductionRatio = 16;

				finalImage.bmpImage = ReduceImageResolution(finalImage.bmpImage, reductionRatio);
				finalImage.bmpImage_destructive = ReduceImageResolution(finalImage.bmpImage_destructive, reductionRatio);
			}

			// Check Result
			bool isResultPass;
			if (resultIndex == 1) isResultPass = true;
			else isResultPass = false;

			// Result is Pass
			if (isResultPass)
			{
				// Save Pass Inspection Image
				if (isSavePassInspectionImage)
				{
					string subDirectory = CreateSubDirectory(inspectionImageFileDirectory);
					string originalImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Original, imageFormat);
					string destructiveImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Destructive, imageFormat);
					SaveToPath(finalImage.bmpImage, originalImageFilePath, imageFormat);
					SaveToPath(finalImage.bmpImage_destructive, destructiveImageFilePath, imageFormat);
					VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Original Image Saved at " + originalImageFilePath);
					VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Destructive Image Saved at " + destructiveImageFilePath);
				}
			}

			// Result is Fail
			else 
			{
				// Save Fail Inspection Image
				if (isSaveFailInspectionImage)
				{
					// Save Fail Image in Different Folder
					if (isSaveFailImageInSeparatedDirectory)
					{
						string subDirectory = CreateSubDirectory(inspectionImageFileDirectory, extensionPathForSeparatedDirectory);
						string originalImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Original, imageFormat);
						string destructiveImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Destructive, imageFormat);
						SaveToPath(finalImage.bmpImage, originalImageFilePath, imageFormat);
						SaveToPath(finalImage.bmpImage_destructive, destructiveImageFilePath, imageFormat);
						VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Original Image Saved at " + originalImageFilePath);
						VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Destructive Image Saved at " + destructiveImageFilePath);

						// Save in Both Folders
						if (isSaveInBothDirectories)
						{
							subDirectory = CreateSubDirectory(inspectionImageFileDirectory);
							originalImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Original, imageFormat);
							destructiveImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Destructive, imageFormat);
							SaveToPath(finalImage.bmpImage, originalImageFilePath, imageFormat);
							SaveToPath(finalImage.bmpImage_destructive, destructiveImageFilePath, imageFormat);
							VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Original Image Saved at " + originalImageFilePath);
							VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Destructive Image Saved at " + destructiveImageFilePath);
						}
					}

					// Save Fail Image in Original Folder
					else
					{
						string subDirectory = CreateSubDirectory(inspectionImageFileDirectory);
						string originalImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Original, imageFormat);
						string destructiveImageFilePath = GenerateFilePath(subDirectory, resultIndex, hourSecond, ImageType.Destructive, imageFormat);
						SaveToPath(finalImage.bmpImage, originalImageFilePath, imageFormat);
						SaveToPath(finalImage.bmpImage_destructive, destructiveImageFilePath, imageFormat);
						VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Original Image Saved at " + originalImageFilePath);
						VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Destructive Image Saved at " + destructiveImageFilePath);
					}
				}				
			}		
		}

		private Bitmap ReduceImageResolution(Bitmap bmpImage, double ratio)
		{
			if (bmpImage == null) throw new ArgumentNullException("bmpImage");
			if (ratio < 1) throw new ArgumentOutOfRangeException("ratio must be greater than 1");

			Bitmap resizedbmp;
			double newWidth = bmpImage.Width / ratio;
			double newHeight = bmpImage.Height / ratio;

			resizedbmp = new Bitmap(bmpImage, new Size((int)newWidth, (int)newHeight));
			bmpImage.Dispose();
			return resizedbmp;
		}

		private void SaveCalibrationImage(
			BDM_Images images,
			string calibrationImageDirectory,
			int calibrationIndex,
			ImageFormat imageFormat)
		{
			string calibrationImageFilePath = GenerateFilePath(
				calibrationImageDirectory,
				calibrationIndex,
				string.Empty,
				ImageType.Calibration,
				imageFormat);

			SaveToPath(images.bmpImage, calibrationImageFilePath, imageFormat);
			VisionLogger.Log(LogType.Sequence, GetType().ToString(), "Calibration Image Saved at " + calibrationImageFilePath);
		}

		/// <summary>
		/// Original - o
		/// Destructive - d
		/// Calibration - c
		/// </summary>
		/// <param name="imageDirectoryPath"></param>
		/// <param name="resultIndex"></param>
		/// <param name="imageType"></param>
		/// <returns></returns>
		private string GenerateFilePath(
			string imageDirectoryPath,
			int resultIndex,
			string hourSecond,
			ImageType imageType,
			ImageFormat imageFormat)
		{			
			string imageType_ = string.Empty;
			string imageFormat_ = string.Empty;

			if (imageType == ImageType.Original) imageType_ = "o";
			else if (imageType == ImageType.Destructive) imageType_ = "d";
			else if (imageType == ImageType.Calibration) imageType_ = "c";

			if (imageFormat == ImageFormat.Bitmap) imageFormat_ = ".bmp";
			else if (imageFormat == ImageFormat.Gif) imageFormat_ = ".gif";
			else if (imageFormat == ImageFormat.Jpeg) imageFormat_ = ".jpg";
			else if (imageFormat == ImageFormat.Png) imageFormat_ = ".png";
			else if (imageFormat == ImageFormat.Tiff) imageFormat_ = ".tiff";

			string savePath = imageDirectoryPath + hourSecond + "_" + imageType_ + resultIndex.ToString() + imageFormat_;
			return savePath;
		}

		/// <summary>
		/// Create subDirectory with date time and 
		/// returns the the string points to the created subdirectory
		/// </summary>
		/// <param name="MainDirectory"></param>
		/// <returns></returns>
		private string CreateSubDirectory(
			string MainDirectory)
		{
			string SubDirectoryPath = MainDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";
			if (!Directory.Exists(SubDirectoryPath))
				Directory.CreateDirectory(SubDirectoryPath);
			return SubDirectoryPath;
		}

		private string CreateSubDirectory(
			string MainDirectory,
			string ExtensionDirectory)
		{
			string SubDirectoryPath = MainDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + ExtensionDirectory + "\\";
			if (!Directory.Exists(SubDirectoryPath))
				Directory.CreateDirectory(SubDirectoryPath);
			return SubDirectoryPath;
		}

		private void SaveToPath(
			Bitmap bmpImage,
			string imagePath,
			ImageFormat imageFormat)
		{
			if (imageFormat == ImageFormat.Bitmap)
				bmpImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Bmp);

			else if (imageFormat == ImageFormat.Gif)
				bmpImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Gif);

			else if (imageFormat == ImageFormat.Jpeg)
				bmpImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

			else if (imageFormat == ImageFormat.Png)
				bmpImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);

			else if (imageFormat == ImageFormat.Tiff)
				bmpImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Tiff);

			Console.WriteLine("Image saved at {0} in format {1}", imagePath, imageFormat);		
		}
	}
}
