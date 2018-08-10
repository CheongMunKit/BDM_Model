using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euresys.Open_eVision_2_0;
using System.ComponentModel;

namespace BDMVision.Model.PatternMatching
{
    public class EuresysDoubleMatcherResults:INotifyPropertyChanged
    {
        private float AngleBetweenResult_;
        public float AngleBetweenResult
        {
            get { return AngleBetweenResult_; }
            set 
            {
                AngleBetweenResult_ = value;
                OnPropertyChanged("AngleBetweenResult");
            }
        }  
        private float XOffset_;
        public float XOffset
        {
            get { return XOffset_; }
            set
            {
                XOffset_ = value;
                OnPropertyChanged("XOffset");
            }
        }   
        private float YOffset_;
        public float YOffset
        {
            get { return YOffset_; }
            set
            {
                YOffset_ = value;
                OnPropertyChanged("YOffset");
            }
        }

        // Pattern 1
        private float Score1_;
        private float Angle1_;
        private float CenterX1_;
        private float CenterY1_;
        private float ScaleX1_;
        private float ScaleY1_;
        private bool IsInterpolate1_;

        public float Score1
        {
            get { return Score1_; }
            set
            {
                Score1_ = value;
                OnPropertyChanged("Score1");
            }
        }  
        public float Angle1
        {
            get { return Angle1_; }
            set
            {
                Angle1_ = value;
                OnPropertyChanged("Angle1");
            }
        } 
        public float CenterX1
        {
            get { return CenterX1_; }
            set
            {
                CenterX1_ = value;
                OnPropertyChanged("CenterX1");
            }
        }
        public float CenterY1
        {
            get { return CenterY1_; }
            set
            {
                CenterY1_ = value;
                OnPropertyChanged("CenterY1");
            }
        }  
        public float ScaleX1
        {
            get { return ScaleX1_; }
            set
            {
                ScaleX1_ = value;
                OnPropertyChanged("ScaleX1");
            }
        }
        public float ScaleY1
        {
            get { return ScaleY1_; }
            set
            {
                ScaleY1_ = value;
                OnPropertyChanged("ScaleY1");
            }
        }                        
        public bool IsInterpolate1
        {
            get { return IsInterpolate1_; }
            set
            {
                IsInterpolate1_ = value;
                OnPropertyChanged("IsInterpolate1");
            }
        }

        // Pattern 2
        private float Score2_;
        private float Angle2_;
        private float CenterX2_;
        private float CenterY2_;
        private float ScaleX2_;
        private float ScaleY2_;
        private bool IsInterpolate2_;

        public float Score2
        {
            get { return Score2_; }
            set
            {
                Score2_ = value;
                OnPropertyChanged("Score2");
            }
        }
        public float Angle2
        {
            get { return Angle2_; }
            set
            {
                Angle2_ = value;
                OnPropertyChanged("Angle2");
            }
        }
        public float CenterX2
        {
            get { return CenterX2_; }
            set
            {
                CenterX2_ = value;
                OnPropertyChanged("CenterX2");
            }
        }
        public float CenterY2
        {
            get { return CenterY2_; }
            set
            {
                CenterY2_ = value;
                OnPropertyChanged("CenterY2");
            }
        }
        public float ScaleX2
        {
            get { return ScaleX2_; }
            set
            {
                ScaleX2_ = value;
                OnPropertyChanged("ScaleX2");
            }
        }
        public float ScaleY2
        {
            get { return ScaleY2_; }
            set
            {
                ScaleY2_ = value;
                OnPropertyChanged("ScaleY2");
            }
        }
        public bool IsInterpolate2
        {
            get { return IsInterpolate2_; }
            set
            {
                IsInterpolate2_ = value;
                OnPropertyChanged("IsInterpolate2");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
