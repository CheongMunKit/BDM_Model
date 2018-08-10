using BDMVision.Model.MapFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMVision.Model.MapFileReader
{
    public class BDMMapFileReader
    {
        public static MapDataFromFile MapDataFromFile = null;
        public static bool IsMapDataPresent;
 
        public Func<MapFileParameters, string, MapDataFromFile> ReadFile_;
        public Func<string, string> ReadDeviceName_;

        public BDMMapFileReader(
            Func<MapFileParameters, string , MapDataFromFile> readFile,
            Func<string, string> readDeviceName)
        {
            this.ReadFile_ = readFile;
            this.ReadDeviceName_ = readDeviceName;
        }          

        public static void SetParameters(MapDataFromFile mapDataFromFile)
        {
            if (mapDataFromFile == null) throw new ArgumentNullException("mapDataFromFile");
            MapDataFromFile = mapDataFromFile;
            IsMapDataPresent = true;
        }
        public static MapDataFromFile GetParameters()
        {
            return MapDataFromFile;
        }
        public static void ClearParameters()
        {
            MapDataFromFile = null;
            IsMapDataPresent = false;
        }
    }
}
