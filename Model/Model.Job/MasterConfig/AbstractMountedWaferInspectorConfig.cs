using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMVision.Model.Job
{
    [Serializable]
    public abstract class AbstractBDMVisionConfig : INotifyPropertyChanged
    {
        private string _imageAcquisitionParameterFilePath;
        public string imageAcquisitionParameterFilePath
        {
            get { return _imageAcquisitionParameterFilePath; }
            set { imageAcquisitionParameterFilePath = value; OnPropertyChanged("imageAcquisitionParameterFilePath"); }
        }

        private string _imagePreProcessingParameterFilePath;
        public string imagePreProcessingParameterFilePath
        {
            get { return _imagePreProcessingParameterFilePath; }
            set { imagePreProcessingParameterFilePath = value; OnPropertyChanged("imagePreProcessingParameterFilePath"); }
        }

        private string _waferGaugeParameterFilePath;
        public string waferGaugeParameterFilePath
        {
            get { return _waferGaugeParameterFilePath; }
            set { _waferGaugeParameterFilePath = value; OnPropertyChanged("waferGaugeParameterFilePath"); }
        }

        private string _orientationSearchParameterFilePath;
        public string orientationSearchParameterFilePath
        {
            get { return _orientationSearchParameterFilePath; }
            set { _orientationSearchParameterFilePath = value; OnPropertyChanged("orientationSearchParameterFilePath"); }
        }

        private string _measurementReferenceParameterFilePath;
        public string measurementReferenceParameterFilePath
        {
            get { return _measurementReferenceParameterFilePath; }
            set { _measurementReferenceParameterFilePath = value; OnPropertyChanged("measurementReferenceParameterFilePath"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;       
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
