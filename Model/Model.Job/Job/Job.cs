using System;
using Model.Protection;

namespace BDMVision.Model.Job
{
	public enum OnlineState
	{
		Offline,
		Online
	}
	public enum IdleState
	{
		Idle,
		Busy
	}
	public enum VisionJob
	{
		Inspection,
		Calibration
	}

	/// <summary>
	/// T1 is Image Type
	/// T2 is Result Type
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public class VisionJob<T1, T2>
    {
        public Func<T1> ImageAcquisition;
        public Func<T1, T1> Preprocess;
        public Func<T1, T2> Process;
        public Func<T2, dynamic> PostProcess;

		private OnlineState OnlineState = OnlineState.Offline;
		private IdleState IdleState = IdleState.Idle;			

		public dynamic Execute()
        {
			if (IdleState == IdleState.Busy) throw new Exception("Vision Engine is Busy");
            if (ImageAcquisition == null) throw new ArgumentNullException("ImageSource");
            if (Preprocess == null) throw new ArgumentNullException("Preprocess");
            if (Process == null) throw new ArgumentNullException("Process");
            if (PostProcess == null) throw new ArgumentNullException("PostProcess");

			IdleState = IdleState.Busy;
			dynamic result;
			try
            {
                LicenseManager.CheckLicense();
                result = PostProcess(Process(Preprocess(ImageAcquisition())));
            }
			catch(Exception ex)
			{
				IdleState = IdleState.Idle;
				throw ex;
			}					
			IdleState = IdleState.Idle;
			return result;
        }

		/// <summary>
		/// Return true if status is online
		/// </summary>
		/// <returns></returns>
		public bool GetOnlineState()
		{
			if (OnlineState == OnlineState.Online) return true;
			else return false;
		}

		/// <summary>
		/// Set Online Statet for Viision Engine
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public bool SetOnlineState(OnlineState state)
		{
			OnlineState = state;
			return true;
		}

		/// <summary>
		/// Return true if status is idle
		/// </summary>
		/// <returns></returns>
		public bool GetIdleState()
		{
			if (IdleState == IdleState.Idle) return true;
			else return false;
		}
    }

    //public class BDMVisionJob<T1, T2>
    //{
    //    public Func<T1> ImageAcquisition;
    //    public Func<T1, T1> Preprocess;
    //    public Func<T1, T2> Process;
    //    public Func<T2, dynamic> PostProcess;

    //    Task<T1> ImageAcquisition_Task() { return Task.FromResult(ImageAcquisition()); }
    //    Task<T1> ImagePreprocesing_Task(T1 t1) { return Task.FromResult(Preprocess(t1)); }
    

    //    private OnlineState OnlineState = OnlineState.Offline;
    //    private IdleState IdleState = IdleState.Idle;

    //    public async dynamic Execute()
    //    {
    //        if (IdleState == IdleState.Busy) throw new Exception("Vision Engine is Busy");
    //        if (ImageAcquisition == null) throw new ArgumentNullException("ImageSource");
    //        if (Preprocess == null) throw new ArgumentNullException("Preprocess");
    //        if (Process == null) throw new ArgumentNullException("Process");
    //        if (PostProcess == null) throw new ArgumentNullException("PostProcess");

    //        IdleState = IdleState.Busy;
    //        dynamic result;
    //        try
    //        {
    //            T1 Acquisition_Image = await ImageAcquisition_Task();
    //            T1 PreProcessed_Image = await ImagePreprocesing_Task(Acquisition_Image);
    //        }
    //        catch (Exception ex)
    //        {
    //            IdleState = IdleState.Idle;
    //            throw ex;
    //        }
    //        IdleState = IdleState.Idle;
    //        return result;
    //    }

   

    //    /// <summary>
    //    /// Return true if status is online
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool GetOnlineState()
    //    {
    //        if (OnlineState == OnlineState.Online) return true;
    //        else return false;
    //    }

    //    /// <summary>
    //    /// Set Online Statet for Viision Engine
    //    /// </summary>
    //    /// <param name="state"></param>
    //    /// <returns></returns>
    //    public bool SetOnlineState(OnlineState state)
    //    {
    //        OnlineState = state;
    //        return true;
    //    }

    //    /// <summary>
    //    /// Return true if status is idle
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool GetIdleState()
    //    {
    //        if (IdleState == IdleState.Idle) return true;
    //        else return false;
    //    }
    //}
}
