using System;
using System.Diagnostics;
using BDMVision.View.Tools.ScreenShot;
using BDMVision.Model.VisionResult;
using BDMVision.Model.Log;

namespace BDMVision.Model.Job
{
    public class PostProcessor<ImageType>
    {
        Action CloseCamera_;
        Action CloseLighting_;

        public void Setup(
            Action CloseCamera,
            Action CloseLighting)
        {
            this.CloseCamera_ = CloseCamera;
            this.CloseLighting_ = CloseLighting;
        }

		public dynamic Execute(Result<ImageType> result)
        {
            if (result.IsAngleWithinTolerance == true && result.IsCenterWithinTolerance == true)
            {
                result.Pass = true;
                result.Remarks = "OK... Image Displayed";
                return result;
            }

            else
            {
                result.Pass = false;
                result.Remarks = "Oh no... Wrong Result";
                return result;
            }
        }

		public dynamic Execute(InspectionResult<ImageType> result)
		{
			Stopwatch Stopwatch_PostProcessing = new Stopwatch();
			Stopwatch_PostProcessing.Start();
			if (result.Pass) result.Remarks = "Inspection Job Completed";
			Stopwatch_PostProcessing.Stop();
			Console.WriteLine("Post Preprocessing Time Elapsed: {0} ms", Stopwatch_PostProcessing.ElapsedMilliseconds);
			return result;
		}

        public dynamic Execute(BDMInspectionResult result)
        {
            VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Post Processor Started");
            CloseCamera_?.Invoke();
            CloseLighting_?.Invoke();
            VisionLogger.Log(WaftechLibraries.Log.LogType.Sequence, this.GetType().ToString(), "Post Processor Completed");

            return result;
        }

    }    
}
