using Euresys.Open_eVision_2_0;
using BDMVision.Model.Camera;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BDMVision.Model.E2VCamera
{
    public class LineScanE2VCamera:ICamera<EImageBW8>
    {
        bool isSoftwareTriggerAllowed = false;

        public LineScanE2VCamera(uint timeOut)
        {
            this.timeOut_ms = (int)timeOut;
        }

        #region Camera Properties

        int timeOut_ms;
        public int CameraIndex
        {
            get
            {
                return (int)E2VCameraHelper.GetChannel();
            }
        }
        public string CameraName
        {
            get
            {
                return "LineScan E2V Camera + GrabLink";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsConnected
        {
            get
            {
                return E2VCameraHelper.isCameraConnected;
            }
        }
        public bool IsLive
        {
            get
            {
                return E2VCameraHelper.isCameraConnected;
            }
        }

        #endregion Camera Properties

        #region Acquisition Control

        private CapturingMode _CapturingMode = CapturingMode.SingleShot;
        public CapturingMode CapturingMode
        {
            get
            {
                return _CapturingMode;
            }
        }

        public EImageBW8 SingleShot()
        {
            if (isSoftwareTriggerAllowed)
            { E2VCameraHelper.TriggerSnapShot(); }
            Stopwatch SW = Stopwatch.StartNew();
            while (SW.ElapsedMilliseconds < timeOut_ms)
            {
                if (E2VCameraHelper.IsImageReady() == true)
                {
                    return E2VCameraHelper.GetImage();
                }
            }
            throw new TimeoutException("E2V camera Capture Time Out");
        }
        public void ContinuousShot()
        {
            throw new NotImplementedException("Continuous Shot Not Available");
        }
        public void StopContinuousShot()
        {
            throw new NotImplementedException("Stop Continuous Shot Not Available");
        }

        #endregion Acquisition Control

        #region Others

        public bool Connect()
        {
            E2VCameraHelper.Connect();
            return IsConnected;
        }
        public void Disconnect()
        {
            E2VCameraHelper.Disconnect();
        }
        public void Restart()
        {
            E2VCameraHelper.SetImageNotReady();
        }
        public void SetTimeOut(uint timeOut_ms)
        {
            this.timeOut_ms = (int)timeOut_ms;
        }
        public List<string> GetCameraList()
        {
            throw new NotImplementedException("Get Camera List not Implemented");
        }
        public bool ReleaseImage()
        {
            E2VCameraHelper.ReleaseImage();
            return true;
        }

        public string GetErrorMessage()
        {
            return E2VCameraHelper.GetErrorMessage();
        }
        public EImageBW8 GetLatestImage()
        {
            if (E2VCameraHelper.IsImageReady() == true)
            {
                return E2VCameraHelper.GetImage();
            }
            else { throw new Exception("Image Not Ready"); }
        }



        #endregion Others     
    }
}
