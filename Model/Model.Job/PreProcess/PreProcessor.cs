using Euresys.Open_eVision_2_0;
using EuresysTools.ImagePreProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using BDMVision.Model.Image;
using BDMVision.Model.Log;
using WaftechLibraries.Log;

namespace BDMVision.Model.Job
{
    public class PreProcessor
    {
        public List<Func<EImageBW8, EImageBW8>> PreProcessQueue { get; set; }
		public string PreProcessImageFilePath { get; set; }

        public PreProcessor()
        { }

		//public void AddImageCalibrator(EuresysDotGridCalibrator DotGridCalibrator)
		//{
		//	this.DotGridCalibrator = DotGridCalibrator;
		//}

		public void AddPreProcesingListToPreProcessQueue(
			List<object> ImagePreProcessingList)
		{
			foreach (object preprocessMethod in ImagePreProcessingList)
			{
				if (preprocessMethod.GetType() == typeof(Dilate))
				{
					Dilate Dilater = (Dilate)preprocessMethod;
					PreProcessQueue.Add(Dilater.Execute);
				}

				else if (preprocessMethod.GetType() == typeof(Erode))
				{
					Erode Eroder = (Erode)preprocessMethod;
					PreProcessQueue.Add(Eroder.Execute);
				}

				else if (preprocessMethod.GetType() == typeof(Close))
				{
					Close Closer = (Close)preprocessMethod;
					PreProcessQueue.Add(Closer.Execute);
				}

				else if (preprocessMethod.GetType() == typeof(Threshold))
				{
					Threshold Thresholder = (Threshold)preprocessMethod;
					PreProcessQueue.Add(Thresholder.Execute);
				}
			}
		}

		public BDM_Images Execute(BDM_Images imageBeforeProcess)
		{
			VisionLogger.Log(LogType.Sequence, this.GetType().ToString(), "Image PreProcessing Started");

			Stopwatch Stopwatch_ImagePreprocessing = Stopwatch.StartNew();
			if (PreProcessQueue == null) throw new ArgumentNullException("PreProcessQueue");
			if (imageBeforeProcess == null) throw new ArgumentNullException("imageBeforeProcess");
			if (imageBeforeProcess.EImage == null) throw new ArgumentNullException("imageBeforeProcess.EImage");

            EImageBW8 imageDuringProcess = null;
			imageDuringProcess = new EImageBW8(imageBeforeProcess.EImage);
			double previousProcessTime = imageBeforeProcess.ProcessTime_ms;

			// 1. Image Preprocesing
			EImageBW8 imageAfterProcess;
			if (PreProcessQueue.Count > 0)
			{ 
				foreach (var item in PreProcessQueue)
				{
					imageDuringProcess = item(imageDuringProcess);
					VisionLogger.Log(LogType.Sequence, GetType().ToString(), item.Target.ToString() + " Executed");
				}

				imageAfterProcess = new EImageBW8(imageDuringProcess);
				imageDuringProcess.Dispose();
			}

			else
			{
				imageAfterProcess = new EImageBW8(imageBeforeProcess.EImage);
				imageDuringProcess.Dispose();
			}		

			Stopwatch_ImagePreprocessing.Stop();
			VisionLogger.Log(LogType.Sequence, this.GetType().ToString(), string.Format("Image PreProcessing Completed, TimeElapsed: {0} ms", Stopwatch_ImagePreprocessing.ElapsedMilliseconds));

			return new BDM_Images()
			{
				bmpImage = imageBeforeProcess.bmpImage,
				EImage = imageAfterProcess,
				ProcessTime_ms = previousProcessTime + Stopwatch_ImagePreprocessing.ElapsedMilliseconds,
			};
        }

        public BDM_Images ExceuteWithoutProcessor(BDM_Images imageBeforeProcess)
        {
            VisionLogger.Log(LogType.Sequence, this.GetType().ToString(), "Image PreProcessing Without Processor Started");
            VisionLogger.Log(LogType.Sequence, this.GetType().ToString(), "Image PreProcessing Without Processor Ended");
            return imageBeforeProcess;
        }
    }
}
