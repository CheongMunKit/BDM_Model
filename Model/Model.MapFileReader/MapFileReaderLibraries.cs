using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BDMVision.Model.MapFile;
using BDMVision.Model.Enum;

namespace BDMVision.Model.MapFile
{
    public static class MapFileReaderLibraries
    {
        /// <summary>
        /// Row 0 : DEVICE
        /// Row 1 : LOT
        /// Row 2 : WAFER
        /// Row 3 : FNLOC
        /// Row 4 : ROWCT
        /// Row 5 : COLCT
        /// Row 6 : BCEQU
        /// Row 7 : REFPX
        /// Row 8 : REFPY
        /// Row 9: DUTMS
        /// Row 10: XDIES
        /// Row 11: YDIES
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static MapDataFromFile ASE_Method(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");

            List<List<BDMMapFromFile>> mapListofList = new List<List<BDMMapFromFile>>();
            List<string> unknownDieTypes = new List<string>();
            int goodDieCount = 0;
            int badDieCount = 0;
            int ignoreDieCount = 0;
            int unknownDieCount = 0;

            string filter = "RowData:";

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath);
            
            // Get Row To Skip 
            int rowsToSkip = 12;

            // Get Recipe/Device Name
            string device = GetInfo(AllLines, 0, "DEVICE:");

            // Get Lot
            string lot = GetInfo(AllLines, 1, "LOT:");

            // Get Wafer
            string wafer = GetInfo(AllLines, 2, "WAFER:");

            // Get Wafer ID
            string wafer_ID = System.IO.Path.GetFileNameWithoutExtension(filepath);

            // Get Flat Notch Orientation
            int waferOrientation = int.Parse((GetInfo(AllLines, 3, "FNLOC:")));

            // Get Row Count and Column Count
            int RowCount = int.Parse((GetInfo(AllLines, 4, "ROWCT:")));
            int ColumnCount = int.Parse((GetInfo(AllLines, 5, "COLCT:")));

            // Get Lines that contains Maps Info
            IEnumerable <string> lines = File.ReadLines(filepath).Skip(rowsToSkip).Take(RowCount);

            // Split lines into word
            foreach (string line in lines)
            {
                string trimmedLine;

                // trim unwanted string
                if (line.Contains(filter)) trimmedLine = line.TrimStart(filter.ToCharArray());
                else trimmedLine = line;
                List<BDMMapFromFile> MapList = new List<BDMMapFromFile>();

                // Separate a line into words
                string[] words = trimmedLine.Split(' ');

                for (int i = 0; i < words.Count(); i++)
                {
                    // Get Enum from file data
                    FileMapCategory EnumFromFile = SelectFileMapCategory(words[i], recipe);
                    if (EnumFromFile == FileMapCategory.UnKnown)
                    {
                        if (!unknownDieTypes.Contains(words[i]))
                        {
                            unknownDieTypes.Add(words[i]);
                            unknownDieCount++;
                        }
                    }

                    else if (EnumFromFile == FileMapCategory.GoodDie)
                    {
                        goodDieCount++;
                    }

                    else if (EnumFromFile == FileMapCategory.BadDie)
                    {
                        badDieCount++;
                    }

                    else if (EnumFromFile == FileMapCategory.IgnoreDie)
                    {
                        ignoreDieCount++;
                    }

                    BDMMapFromFile map = new BDMMapFromFile()
                    {
                        mapCode = words[i],
                        mapCategory = EnumFromFile,
                    };
                    MapList.Add(map);
                }
                mapListofList.Add(MapList);
            }

            List<string> unknownDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(unknownDieTypes);
            ValidateResult(mapListofList, RowCount, ColumnCount);

            return new MapDataFromFile()
            {
                Device = device,
                Lot = lot,
                Wafer = wafer,
                Batch_ID = "",
                Wafer_ID = wafer_ID,
                MapsFromFile = mapListofList,
                WaferOrientation = waferOrientation,
                UnknownDiesType = unknownDieTypes_Rearranged,
                GoodDieCount = goodDieCount,
                BadDieCount = badDieCount,
                IgnoreDieCount = ignoreDieCount,
                UnknownDieCount = unknownDieCount               
            };
        }

        public static string GetRecipeName_ASE_Method(string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");
            // 1. Read All Lines
            string[] AllLines = File.ReadAllLines(filepath);
            string deviceName = GetInfo(AllLines, 0, "DEVICE:");
            return deviceName;
        }
       

        private static void ValidateResult(List<List<BDMMapFromFile>> mapListofList, int rowCount, int columnCount)
        {
            if (mapListofList.Count != rowCount) throw new Exception("Row Count not tally in Map File");
            if (mapListofList[0].Count != columnCount) throw new Exception("Column Count not tally in Map File");
        }   
        private static string GetInfo(string[] AllLines, int RowThatContainsTheInfo, string filter)
        {
            string line = AllLines[RowThatContainsTheInfo];

            string trimmedLine;
            if (line.Contains(filter))
            {
                string[] splittedLine = line.Split(':');
                trimmedLine = splittedLine[1];
            }

            else throw new ArgumentException(
                string.Format("line does not contains the argument {0}", filter));    
            return trimmedLine;
        } 
        private static FileMapCategory SelectFileMapCategory(string word, MapFileParameters recipe)
        {
            List<string> blankDies = recipe.BlankDies;
            List<string> GoodDies = recipe.GoodDies;
            List<string> BadDies = recipe.BadDies;
            List<string> IgnoreDies = recipe.IgnoreDies;
            foreach (string blankDie in blankDies)
            {
                if (blankDie.Contains(word)) { return FileMapCategory.Blank; }
            }
            foreach (string goodDie in GoodDies)
            {
                if (goodDie.Contains(word)) { return FileMapCategory.GoodDie; }
            }
            foreach (string badDie in BadDies)
            {
                if (badDie.Contains(word)) { return FileMapCategory.BadDie; }
            }
            foreach (string ignoreDie in IgnoreDies)
            {
                if (ignoreDie.Contains(word)) { return FileMapCategory.IgnoreDie; }
            }
            return FileMapCategory.UnKnown;       
        }                                                                                                             
    }
}
