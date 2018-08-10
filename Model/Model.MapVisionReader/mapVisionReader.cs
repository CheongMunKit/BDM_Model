using BDMVision.Model.MapVision;
using Euresys.Open_eVision_2_0;
using System;

namespace BDMVision.Model.MapVisionReader
{
    public class MapVisionReader
    {
        public Func<MapVisionParameters, EImageBW8, MapDataFromVision> ReadImage_;

        public MapVisionReader(Func<MapVisionParameters, EImageBW8, MapDataFromVision> ReadImage)
        {
            this.ReadImage_ = ReadImage;
        }  
    }
}
