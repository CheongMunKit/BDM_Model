using BDMVision.Model.MapFile;
using BDMVision.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMVision.Model.MapFileReader
{
    public static class MapFileFormatSelector
    {               
        public static Tuple<Func<MapFileParameters, string, MapDataFromFile>, Func<string, string>, MapFileFormat> ChooseFormat(
            List<Func<MapFileParameters, string, MapDataFromFile>> listofMethods,
            MapFileParameters mapFileParameter,
            string fullFilePath,
            MapFileFormat currentMethod)
        {
            if (currentMethod == MapFileFormat.ASE) { if (CheckIf_ASEMethod(mapFileParameter, fullFilePath))
                    return new Tuple<
                        Func<MapFileParameters, string, MapDataFromFile>, 
                        Func<string, string>,
                        MapFileFormat>
                        (MapFileReaderLibraries.ASE, MapFileReaderLibraries.ASE_RecipeName, MapFileFormat.ASE); }

            else if (currentMethod == MapFileFormat.AMS) { if (CheckIf_AMSMethod(mapFileParameter, fullFilePath))
                    return new Tuple<
                        Func<MapFileParameters, string, MapDataFromFile>, 
                        Func<string, string>,
                        MapFileFormat>(MapFileReaderLibraries.AMS, MapFileReaderLibraries.AMS_RecipeName, MapFileFormat.AMS); }

            else if (currentMethod == MapFileFormat.CIS_A) { if (CheckIf_CIS_A_Method(mapFileParameter, fullFilePath))
                    return new Tuple<
                        Func<MapFileParameters, string, MapDataFromFile>,
                        Func<string, string>,
                        MapFileFormat>(MapFileReaderLibraries.CIS_A, MapFileReaderLibraries.CIS_A_RecipeName, MapFileFormat.CIS_A); }

            else if (currentMethod == MapFileFormat.CIS_B) { if (CheckIf_CIS_B_Method(mapFileParameter, fullFilePath))
                    return new Tuple<
                        Func<MapFileParameters, string, MapDataFromFile>, 
                        Func<string, string>,
                        MapFileFormat>(MapFileReaderLibraries.CIS_B, MapFileReaderLibraries.CIS_B_RecipeName, MapFileFormat.CIS_B); }

            if (CheckIf_ASEMethod(mapFileParameter, fullFilePath))
                return new Tuple<
                    Func<MapFileParameters, string, MapDataFromFile>,
                    Func<string, string>,
                    MapFileFormat>
                    (MapFileReaderLibraries.ASE, MapFileReaderLibraries.ASE_RecipeName, MapFileFormat.ASE);
            

            else if (CheckIf_AMSMethod(mapFileParameter, fullFilePath))
                return new Tuple<
                    Func<MapFileParameters, string, MapDataFromFile>,
                    Func<string, string>,
                    MapFileFormat>(MapFileReaderLibraries.AMS, MapFileReaderLibraries.AMS_RecipeName, MapFileFormat.AMS);

            else if (CheckIf_CIS_A_Method(mapFileParameter, fullFilePath))
                return new Tuple<
                    Func<MapFileParameters, string, MapDataFromFile>,
                    Func<string, string>,
                    MapFileFormat>(MapFileReaderLibraries.CIS_A, MapFileReaderLibraries.CIS_A_RecipeName, MapFileFormat.CIS_A);

            else if (CheckIf_CIS_B_Method(mapFileParameter, fullFilePath))
                return new Tuple<
                    Func<MapFileParameters, string, MapDataFromFile>,
                    Func<string, string>,
                    MapFileFormat>(MapFileReaderLibraries.CIS_B, MapFileReaderLibraries.CIS_B_RecipeName, MapFileFormat.CIS_B);
            else
            {
                throw new Exception("Library does not support the current Map File Format");
            }
        }

        static bool CheckIf_ASEMethod(MapFileParameters mapFileParameter, string fullFilePath)
        {
            return MapFileReaderLibraries.ASE_Check(mapFileParameter, fullFilePath);
        }
        static bool CheckIf_AMSMethod(MapFileParameters mapFileParameter, string fullFilePath)
        {
            return MapFileReaderLibraries.AMS_Check(mapFileParameter, fullFilePath);
        }
        static bool CheckIf_CIS_A_Method(MapFileParameters mapFileParameter, string fullFilePath)
        {
            return MapFileReaderLibraries.CIS_A_Check(mapFileParameter, fullFilePath);
        }
        static bool CheckIf_CIS_B_Method(MapFileParameters mapFileParameter, string fullFilePath)
        {
            return MapFileReaderLibraries.CIS_B_Check(mapFileParameter, fullFilePath);
        }
    }
}
