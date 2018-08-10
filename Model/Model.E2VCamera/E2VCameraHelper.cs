using BDMVision.Model.Log;
using BDMVision.Model.Notification;
using Euresys.MultiCam;
using Euresys.Open_eVision_2_0;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Model.Protection;

namespace BDMVision.Model.E2VCamera
{
    public static class E2VCameraHelper
    {
        static uint channel;
        static bool isImageReady = false;
        public static bool isCameraConnected = false;
        static Mutex imageMutex = new Mutex();
        static System.Drawing.Bitmap image;
        static ColorPalette imgpal = null;
        static EImageBW8 eImage;
        static string errorMessage;
        static uint currentSurface;
        static MC.CALLBACK multiCamCallback;
        static Func<bool> IsCameraTriggerOn_;

        public static LineScanE2VCamera MakeCamera(
            uint timeOut_ms,
            Func<bool> IsCameraTriggerOn)
        {
            try
            {
                LicenseManager.CheckLicense();

                MC.OpenDriver();
                MC.SetParam(MC.CONFIGURATION, "ErrorLog", "error.log");

                #region Grabber Conifguration 

                MC.Create("CHANNEL", out channel);
                MC.SetParam(channel, "DriverIndex", 0);
                MC.SetParam(MC.BOARD + 0, "BoardTopology", "MONO_DECA");
                MC.SetParam(channel, "Connector", "M");

                #endregion Grabber Configuration

                #region Camera Specification

                MC.SetParam(channel, "CamConfig", "LxxxxRG");
                string camFile = @"C:\Users\Public\Documents\Euresys\MultiCam\Cameras\ELIIXAp-16kCL\ELIIXAp-16kCL_L16384RG_new.cam";
                MC.SetParam(channel, "CamFile", camFile);

                #endregion Camera Specification

                #region Camera Features

                MC.SetParam(channel, "TapConfiguration", "DECA_10T8");

                #endregion Camera Features

                #region Acquisition Control

                MC.SetParam(channel, "AcquisitionMode", "PAGE");
                MC.SetParam(channel, "ActivityLength", 1);
                MC.SetParam(channel, "BreakEffect", "FINISH");
                MC.SetParam(channel, "PageLength_Ln", 16380);
                MC.SetParam(channel, "SeqLength_Pg", -1);
                MC.SetParam(channel, "SeqLength_Ln", -1);
                MC.SetParam(channel, "TrigMode", "COMBINED");
                MC.SetParam(channel, "NextTrigMode", "SAME");
                MC.SetParam(channel, "TrigRepeatCount", 0);
                MC.SetParam(channel, "TrigCtl", "ISO");
                MC.SetParam(channel, "TrigLine", "IIN1");
                MC.SetParam(channel, "TrigEdge", "GOHIGH");
                MC.SetParam(channel, "TrigFilter", "MEDIUM");

                #endregion Acquisition Control

                #region Exposure Control

                MC.SetParam(channel, "Expose_us", 200);

                #endregion Exposure Control

                #region Strobe Control

                MC.SetParam(channel, "StrobeMode", "NONE");

                #endregion Strobe Control

                #region Encoder Control

                MC.SetParam(channel, "EncoderPitch", 125);
                MC.SetParam(channel, "ForwardDirection", "A_LEADS_B");
                MC.SetParam(channel, "LineCaptureMode", "ALL");
                MC.SetParam(channel, "LinePitch", 2400);
                MC.SetParam(channel, "LineRateMode", "CONVERT");
                MC.SetParam(channel, "LineTrigCtl", "DIFF_PAIRED");
                MC.SetParam(channel, "LineTrigEdge", "ALL_A_B");
                MC.SetParam(channel, "LineTrigFilter", "MEDIUM");

                #endregion Encoder Control

                #region Grabber Timing 

                MC.SetParam(channel, "GrabWindow", "NOBLACK");

                #endregion Grabber Timing

                #region Cluster

                MC.SetParam(channel, "ColorFormat", "Y8");

                #endregion Clutser          

                #region Call Back

                // Register the callback function
                multiCamCallback = new MC.CALLBACK(MultiCamCallback);
                MC.RegisterCallback(channel, multiCamCallback, channel);

                // Enable the signals corresponding to the callback functions
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_SURFACE_PROCESSING, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_ACQUISITION_FAILURE, "ON");

                // Prepare the channel in order to minimize the acquisition sequence startup latency
                MC.SetParam(channel, "ChannelState", "READY");

                #endregion Call Back

                #region Camera Trigger

                IsCameraTriggerOn_ = IsCameraTriggerOn;

                #endregion Camera Trigger

                return new LineScanE2VCamera(timeOut_ms);
            }
            catch (Euresys.MultiCamException exc)
            {
                throw exc;
            }
        }

        #region Public Methods

        public static uint GetChannel()
        {
            return channel;
        }
        public static string GetErrorMessage()
        {
            return errorMessage;
        }
        public static bool IsImageReady()
        {
            return isImageReady;
        }
        public static void SetImageNotReady()
        {
            isImageReady = false;
        }
        public static EImageBW8 GetImage()
        {
            try
            {
                if (eImage == null) throw new ArgumentNullException("eImage");
                if (eImage.IsVoid) throw new Exception("eImage cannot be void");
                return eImage;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw ex;
            }  
        }
        public static void ReleaseImage()
        {
            eImage = null;
        }
        public static void TriggerSnapShot()
        {
            try
            {
                if (!isCameraConnected) throw new Exception("Camera Not Connected");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw ex;
            }

        }
        public static void Connect()
        {
            try
            {
                // Activating the channel
                string channelState;
                MC.GetParam(channel, "ChannelState", out channelState);
                if (channelState != "ACTIVE") MC.SetParam(channel, "ChannelState", "ACTIVE");
                isCameraConnected = true;
                VisionLogger.Log(WaftechLibraries.Log.LogType.Log, "E2VCameraHelper", "Camera Connected");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw ex;
            }

        }
        public static void Disconnect()
        {
            try
            {
                if (isCameraConnected && channel != 0)
                {
                    MC.SetParam(channel, "ChannelState", "IDLE");
                }
                VisionLogger.Log(WaftechLibraries.Log.LogType.Log, "E2VCameraHelper", "Camera Disconnected");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw ex;
            }
        }

        #endregion Public Methods

        #region CallBack

        private static void MultiCamCallback(ref MC.SIGNALINFO signalInfo)
        {
            try
            {
                switch (signalInfo.Signal)
                {
                    case MC.SIG_SURFACE_PROCESSING:
                        ProcessingCallback(signalInfo);
                        break;
                    case MC.SIG_ACQUISITION_FAILURE:
                        AcqFailureCallback(signalInfo);
                        break;
                    default:
                        throw new Euresys.MultiCamException("Unknown signal");
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw ex;
            }
        }
        private static void ProcessingCallback(MC.SIGNALINFO signalInfo)
        {
            isImageReady = false;

            UInt32 currentChannel = (UInt32)signalInfo.Context;
            currentSurface = signalInfo.SignalInfo;

            // + GrablinkSnapshotTrigger Sample Program

            try
            {
                // Update the image with the acquired image buffer data 
                Int32 width, height, bufferPitch;
                IntPtr bufferAddress;
                MC.GetParam(currentChannel, "ImageSizeX", out width);
                MC.GetParam(currentChannel, "ImageSizeY", out height);
                MC.GetParam(currentChannel, "BufferPitch", out bufferPitch);
                MC.GetParam(currentSurface, "SurfaceAddr", out bufferAddress);

                try
                {
                    imageMutex.WaitOne();

                    image = new System.Drawing.Bitmap(width, height, bufferPitch, PixelFormat.Format8bppIndexed, bufferAddress);
                    imgpal = image.Palette;

                    // Build bitmap palette Y8
                    for (uint i = 0; i < 256; i++)
                    {
                        imgpal.Entries[i] = Color.FromArgb(
                        (byte)0xFF,
                        (byte)i,
                        (byte)i,
                        (byte)i);
                    }

                    image.Palette = imgpal;

                    string path_directory = @"D:\Waftech\BDMVision\Log\Temp\";
                    System.IO.Directory.CreateDirectory(path_directory);
                    string fullPath = path_directory + "test.jpg";
                    
                    image.Save(fullPath);
                    eImage = new EImageBW8();
                    eImage.SetSize(image.Width, image.Height);
                    eImage.Load(fullPath);
                }
                finally
                {
                    imageMutex.ReleaseMutex();
                }

                isImageReady = true;

                // Retrieve the frame rate
                double frameRate_Hz;
                MC.GetParam(channel, "PerSecond_Fr", out frameRate_Hz);

                // Retrieve the channel state
                string channelState;
                MC.GetParam(channel, "ChannelState", out channelState);

                // Log frame rate and channel state
                VisionLogger.Log(WaftechLibraries.Log.LogType.Log, "E2VCameraHelper", string.Format("Frame Rate: {0:f2}, Channel State: {1}", frameRate_Hz, channelState));

            }
            catch (Euresys.MultiCamException ex)
            {
                VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, "E2VCameraHelper", ex);
                VisionNotifier.AddNotification("MultiCam Exception: " + ex.Message);
                errorMessage = "MultiCam Exception: " + ex.Message;
            }
            catch (System.Exception ex)
            {
                VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, "E2VCameraHelper", ex);
                VisionNotifier.AddNotification("System Exception: " + ex.Message);
                errorMessage = "System Exception: " + ex.Message;
            }
        }
        private static void AcqFailureCallback(MC.SIGNALINFO signalInfo)
        {
            uint currentChannel = (uint)signalInfo.Context;

            try
            {
                isImageReady = false;
            }
            catch (System.Exception ex)
            {
                VisionLogger.Log(WaftechLibraries.Log.LogType.Exception, "E2VCameraHelper", ex);
                VisionNotifier.AddNotification("System Exception: " + ex.Message);
                errorMessage = ex.Message;
            }
        }
        
        #endregion CallBack   
    }
}
