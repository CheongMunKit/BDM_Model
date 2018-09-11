using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BDMVision.Model.MapFile;
using BDMVision.Model.Enum;
using System.Text.RegularExpressions;

namespace BDMVision.Model.MapFile
{
    public static class MapFileReaderLibraries
    {
        #region ASE Method

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
        /// Row 9 : DUTMS
        /// Row 10: XDIES
        /// Row 11: YDIES
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static MapDataFromFile ASE(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");
            List<List<BDMMapFromFile>> mapListofList = new List<List<BDMMapFromFile>>();
            List<string> unknownDieTypes = new List<string>();
            List<string> goodDieTypes = new List<string>();
            List<string> badDieTypes = new List<string>();
            List<string> ignoreDieTypes = new List<string>();

            int goodDieCount = 0;
            int badDieCount = 0;
            int ignoreDieCount = 0;
            int unknownDieCount = 0;

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            // Get Recipe/Device Name
            string device = GetInfo_ASEMethod(AllLines, 0, "DEVICE:");

            // Get Lot
            string lot = GetInfo_ASEMethod(AllLines, 1, "LOT:");

            // Get Wafer
            string wafer = GetInfo_ASEMethod(AllLines, 2, "WAFER:");

            // Get Wafer ID
            string wafer_ID = System.IO.Path.GetFileNameWithoutExtension(filepath);

            // Get Flat Notch Orientation
            int waferOrientation = int.Parse((GetInfo_ASEMethod(AllLines, 3, "FNLOC:")));

            // Get Row Count and Column Count
            int RowCount = int.Parse((GetInfo_ASEMethod(AllLines, 4, "ROWCT:")));
            int ColumnCount = int.Parse((GetInfo_ASEMethod(AllLines, 5, "COLCT:")));

            // Get Row To Skip 
            int rowsToSkip = 12;

            // Get Lines that contains Die Info
            IEnumerable<string> lines = File.ReadLines(filepath).Skip(rowsToSkip).Take(RowCount);            
            string filter = "RowData:";

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
                        if (!unknownDieTypes.Contains(words[i])) unknownDieTypes.Add(words[i]);
                        unknownDieCount = unknownDieTypes.Count;
                    }

                    else if (EnumFromFile == FileMapCategory.GoodDie)
                    {
                        goodDieCount++;
                        if (!goodDieTypes.Contains(words[i])) goodDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.BadDie)
                    {
                        badDieCount++;
                        if (!badDieTypes.Contains(words[i])) badDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.IgnoreDie)
                    {
                        ignoreDieCount++;
                        if (!ignoreDieTypes.Contains(words[i])) ignoreDieTypes.Add(words[i]);
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

            List<string> unknownDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(unknownDieTypes, MapFileFormat.ASE);
            List<string> goodDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(goodDieTypes, MapFileFormat.ASE);
            List<string> badDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(badDieTypes, MapFileFormat.ASE);
            List<string> ignoreDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(ignoreDieTypes, MapFileFormat.ASE);
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
                GoodDiesType = goodDieTypes_Rearranged,
                BadDiesType = badDieTypes_Rearranged,
                IgnoreDiesType = ignoreDieTypes_Rearranged,
                GoodDieCount = goodDieCount,
                BadDieCount = badDieCount,
                IgnoreDieCount = ignoreDieCount,
                UnknownDieCount = unknownDieCount
            };
        }
        public static bool ASE_Check(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            try
            {
                // Get Recipe/Device Name
                string device = GetInfo_ASEMethod(AllLines, 0, "DEVICE:");

                // Get Flat Notch Orientation
                int waferOrientation = int.Parse((GetInfo_ASEMethod(AllLines, 3, "FNLOC:")));


                return true;
            }

            catch { return false; }
        }
        public static string ASE_RecipeName(string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");
            // 1. Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);
            string deviceName = GetInfo_ASEMethod(AllLines, 0, "DEVICE:");
            return deviceName;
        }
        private static string GetInfo_ASEMethod(string[] AllLines, int RowThatContainsTheInfo, string filter)
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

        #endregion ASE Method

        #region AMS Method

        /// <summary>
        /// Row 0 : WAFER MAP
        /// Row 1 : WAFER_ID
        /// Row 2 : PRODUCT_ID
        /// Row 3 : MAP_TYPE
        /// Row 4 : NULL_BIN
        /// Row 5 : ROWS
        /// Row 6 : COLUMNS
        /// Row 7 : FLAT_NOTCH
        /// Row 8 : CUSTOMER_NAME
        /// Row 9 : FORMAT_REVISION
        /// Row 10: SUPPLIER NAME
        /// Row 11: LOT_ID
        /// Row 12: WAFER_SIZE
        /// Row 13: DIES
        /// Row 14: BINS
        /// Row 15: BIN = "0" 
        /// Row 16: BIN = "1"
        /// Row 17: MAP        
        public static MapDataFromFile AMS(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");
            List<List<BDMMapFromFile>> mapListofList = new List<List<BDMMapFromFile>>();
            List<string> unknownDieTypes = new List<string>();
            List<string> goodDieTypes = new List<string>();
            List<string> badDieTypes = new List<string>();
            List<string> ignoreDieTypes = new List<string>();
            int goodDieCount = 0;
            int badDieCount = 0;
            int ignoreDieCount = 0;
            int unknownDieCount = 0;

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            // Get Recipe/Device Name
            string device = GetInfo_AMSMethod(AllLines, 2, "PRODUCT_ID = ");

            // Get Lot
            string lot = GetInfo_AMSMethod(AllLines, 11, "LOT_ID =");

            // Get Wafer
            string wafer = "";

            // Get Wafer ID
            string wafer_ID = GetInfo_AMSMethod(AllLines, 1, "WAFER_ID =");

            // Get Flat Notch Orientation
            int waferOrientation = int.Parse((GetInfo_AMSMethod(AllLines, 7, "FLAT_NOTCH =")));

            // Get Row Count and Column Count
            int RowCount = int.Parse((GetInfo_AMSMethod(AllLines, 5, "ROWS =")));
            int ColumnCount = int.Parse((GetInfo_AMSMethod(AllLines, 6, "COLUMNS =")));

            // Get Row to Skip
            int rowsToSkip = 18;

            // Get Lines that contains Die Info
            IEnumerable<string> lines = File.ReadLines(filepath).Skip(rowsToSkip).Take(RowCount);
            string filter = "";

            // Split lines into word
            foreach (string line in lines)
            {
                string trimmedLine;

                // trim unwanted string
                if (line.Contains(filter)) trimmedLine = line.TrimStart(filter.ToCharArray());
                else trimmedLine = line;
                List<BDMMapFromFile> MapList = new List<BDMMapFromFile>();

                // Separate a line into words
                char[] characters = trimmedLine.ToCharArray();
                string[] words = new string[characters.Length];

                // Set the strings
                for (int i = 0; i < characters.Length; i++)
                {
                    words[i] = characters[i].ToString();
                }

                for (int i = 0; i < words.Count(); i++)
                {
                    // Get Enum from file data
                    FileMapCategory EnumFromFile = SelectFileMapCategory(words[i], recipe);
                    if (EnumFromFile == FileMapCategory.UnKnown)
                    {
                        if (!unknownDieTypes.Contains(words[i])) unknownDieTypes.Add(words[i]);
                        unknownDieCount = unknownDieTypes.Count;
                    }

                    else if (EnumFromFile == FileMapCategory.GoodDie)
                    {
                        goodDieCount++;
                        if (!goodDieTypes.Contains(words[i])) goodDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.BadDie)
                    {
                        badDieCount++;
                        if (!badDieTypes.Contains(words[i])) badDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.IgnoreDie)
                    {
                        ignoreDieCount++;
                        if (!ignoreDieTypes.Contains(words[i])) ignoreDieTypes.Add(words[i]);
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

            List<string> unknownDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(unknownDieTypes, MapFileFormat.AMS);
            List<string> goodDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(goodDieTypes, MapFileFormat.AMS);
            List<string> badDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(badDieTypes, MapFileFormat.AMS);
            List<string> ignoreDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(ignoreDieTypes, MapFileFormat.AMS);
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
                GoodDiesType = goodDieTypes_Rearranged,
                BadDiesType = badDieTypes_Rearranged,
                IgnoreDiesType = ignoreDieTypes_Rearranged,
                GoodDieCount = goodDieCount,
                BadDieCount = badDieCount,
                IgnoreDieCount = ignoreDieCount,
                UnknownDieCount = unknownDieCount,
                
            };
        }
        public static bool AMS_Check(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            try
            {
                // Get Recipe/Device Name
                string device = GetInfo_AMSMethod(AllLines, 2, "PRODUCT_ID = ");

                // Get Flat Notch Orientation
                int waferOrientation = int.Parse((GetInfo_AMSMethod(AllLines, 7, "FLAT_NOTCH =")));

                return true;
            }

            catch { return false; }
        }
        public static string AMS_RecipeName(string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");
            // 1. Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);
            string deviceName = GetInfo_AMSMethod(AllLines, 2, "PRODUCT_ID = ");
            return deviceName;
        }
        private static string GetInfo_AMSMethod(string[] AllLines, int RowThatContainsTheInfo, string filter)
        {
            string line = AllLines[RowThatContainsTheInfo];
            string trimmedLine;
            if (line.Contains(filter))
            {
                string[] splittedLine = line.Split('=');

                if (splittedLine.Count() == 2)
                {
                    // trim empty spaces
                    string filteredString = splittedLine[1].Trim();
                    filteredString = filteredString.Replace('\"', ' ');
                    trimmedLine = filteredString;
                }

                else if (splittedLine.Count() == 1)
                {
                    trimmedLine = "";
                }

                else throw new Exception("Invalid splittedLine Count");


            }

            else throw new ArgumentException(
                string.Format("line does not contains the argument {0}", filter));
            return trimmedLine;
        }


        #endregion

        #region CIS A Method

        /// <summary>
        /// Row 0: Site
        /// Row 1: Lot_#
        /// Row 2: Wafer_ID
        /// Row 3: Prod_#
        /// Row 4: Die_#
        /// Row 5: Flat
        /// Row 6: Fab_Flow
        /// Row 7: Conv_Rev
        /// Row 8: Res2
        /// Row 9: Map_Matrix
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static MapDataFromFile CIS_A(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + "does not exist");
            List<List<BDMMapFromFile>> mapListofList = new List<List<BDMMapFromFile>>();
            List<string> unknownDieTypes = new List<string>();
            List<string> goodDieTypes = new List<string>();
            List<string> badDieTypes = new List<string>();
            List<string> ignoreDieTypes = new List<string>();
            int goodDieCount = 0;
            int badDieCount = 0;
            int ignoreDieCount = 0;
            int unknownDieCount = 0;

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            // Get Lot
            string lot = GetInfo_CIS_A_Method(AllLines, 1, "Lot_#:");

            // Get Wafer
            string wafer = "";

            // Get Wafer ID
            string wafer_ID = GetInfo_CIS_A_Method(AllLines, 2, "Wafer_ID");

            // Get Flat Notch Orientation
            int waferOrientation = int.Parse((GetInfo_CIS_A_Method(AllLines, 5, "Flat")));

            // Get Row to Skip
            int rowsToSkip = 10;

            // Get Row Count and Column Count
            int RowCount = AllLines.Count() - rowsToSkip;
            int ColumnCount = AllLines[rowsToSkip].ToCharArray().Count();

            // Get Recipe / Device Name
            string device = "CIS_A_Row" + RowCount + "_Col" + ColumnCount;

            // Get Lines that contains Die Info
            IEnumerable<string> lines = File.ReadLines(filepath).Skip(rowsToSkip).Take(RowCount);
            string filter = "";

            // Split lines into word
            foreach (string line in lines)
            {
                string trimmedLine;
                if (line.Contains(filter)) trimmedLine = line.TrimStart(filter.ToCharArray());
                else trimmedLine = line;
                List<BDMMapFromFile> MapList = new List<BDMMapFromFile>();

                // Separate a line into words
                char[] characters = trimmedLine.ToCharArray();
                string[] words = new string[characters.Length];

                // Set the strings
                for (int i = 0; i < characters.Length; i++)
                {
                    words[i] = characters[i].ToString();
                }

                for (int i = 0; i < characters.Length; i++)
                {
                    // Get Enum from file data
                    FileMapCategory EnumFromFile = SelectFileMapCategory(words[i], recipe);
                    if (EnumFromFile == FileMapCategory.UnKnown)
                    {
                        if (!unknownDieTypes.Contains(words[i])) unknownDieTypes.Add(words[i]);
                        unknownDieCount = unknownDieTypes.Count;
                    }

                    else if (EnumFromFile == FileMapCategory.GoodDie)
                    {
                        goodDieCount++;
                        if (!goodDieTypes.Contains(words[i])) goodDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.BadDie)
                    {
                        badDieCount++;
                        if (!badDieTypes.Contains(words[i])) badDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.IgnoreDie)
                    {
                        ignoreDieCount++;
                        if (!ignoreDieTypes.Contains(words[i])) ignoreDieTypes.Add(words[i]);
                    }

                    BDMMapFromFile map = new BDMMapFromFile()
                    {
                        mapCode = words[i],
                        mapCategory = EnumFromFile
                    };
                    MapList.Add(map);
                }
                mapListofList.Add(MapList);
            }

            List<string> unknownDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(unknownDieTypes, MapFileFormat.CIS_A);
            List<string> goodDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(goodDieTypes, MapFileFormat.CIS_A);
            List<string> badDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(badDieTypes, MapFileFormat.CIS_A);
            List<string> ignoreDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(ignoreDieTypes, MapFileFormat.CIS_A);
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
        public static bool CIS_A_Check(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            try
            {
                // Get Recipe/Device Name
                string device = GetInfo_CIS_A_Method(AllLines, 0, "Site:");

                // Get Flat Notch Orientation
                int waferOrientation = int.Parse((GetInfo_CIS_A_Method(AllLines, 5, "Flat")));

                return true;
            }

            catch { return false; }
        }
        public static string CIS_A_RecipeName(string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");
            // 1. Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);
            int rowsToSkip = 10;

            int RowCount = AllLines.Count() - rowsToSkip;
            int ColumnCount = AllLines[rowsToSkip].ToCharArray().Count();
            string deviceName = "CIS_A_Row" + RowCount + "_Col" + ColumnCount;
            return deviceName;
        }
        private static string GetInfo_CIS_A_Method(string[] AllLines, int RowThatContainsTheInfo, string filter)
        {
            string line = AllLines[RowThatContainsTheInfo];
            string trimmedLine;
            if (line.Contains(filter))
            {
                string[] splittedLine = line.Split(':');

                if (splittedLine.Count() == 2)
                {
                    // Remove all white space
                    trimmedLine = Regex.Replace(splittedLine[1], @"\s+", "");
                }
                else if (splittedLine.Count() == 1)
                {
                    trimmedLine = "";
                }

                else throw new Exception("Invalid splittedLine Count");
            }

            else throw new ArgumentException(
                string.Format("line does not contains the argument {0}", filter));
            return trimmedLine;
        }

        #endregion CIS A Method

        #region CIS B Method

        /// <summary>
        /// Row 0 : [WAFER DATA]
        /// Row 1 : mat_id
        /// Row 2 : lot_id
        /// Row 3 : rows
        /// Row 4 : cols
        /// Row 5 : notch_loc
        /// Row 6 : manufacturer_info
        /// Row 7 : process_bincodes
        /// Row 8 : ref_points
        /// Row 9 : converter_name
        /// Row 10: rel_notch_loc
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static MapDataFromFile CIS_B(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + "does not exist");
            List<List<BDMMapFromFile>> mapListofList = new List<List<BDMMapFromFile>>();
            List<string> unknownDieTypes = new List<string>();
            List<string> goodDieTypes = new List<string>();
            List<string> badDieTypes = new List<string>();
            List<string> ignoreDieTypes = new List<string>();
            int goodDieCount = 0;
            int badDieCount = 0;
            int ignoreDieCount = 0;
            int unknownDieCount = 0;

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            // Get Lot
            string lot = GetInfo_CIS_B_Method(AllLines, 2, "lot_id=");

            // Get Wafer
            string wafer = "";

            // Get Wafer ID
            string wafer_ID = "";

            // Get Flat Notch Orientation
            int waferOrientation = int.Parse((GetInfo_CIS_B_Method(AllLines, 5, "notch_loc=")));

            // Get Row Count and Column Count
            int RowCount = int.Parse(GetInfo_CIS_B_Method(AllLines, 3, "rows="));
            int ColumnCount = int.Parse(GetInfo_CIS_B_Method(AllLines, 4, "cols="));

            // Get Recipe / Device Name
            string device = "CIS_B_Row" + RowCount + "_Col" + ColumnCount;

            // Get Row to Skip
            int rowsToSkip = 11;

            // Get Lines that contains Die Info
            IEnumerable<string> lines = File.ReadLines(filepath).Skip(rowsToSkip).Take(RowCount);
            string filter = "";

            foreach(string line in lines)
            {
                string trimmedLine;

                // trim unwanted string
                if (line.Contains(filter)) trimmedLine = line.TrimStart(filter.ToCharArray());
                else trimmedLine = line;
                List<BDMMapFromFile> MapList = new List<BDMMapFromFile>();

                // Separate a line into words
                char[] characters = trimmedLine.ToCharArray();
                string[] words = new string[characters.Length];

                // Set the strings
                for (int i = 0; i < characters.Length; i++)
                {
                    words[i] = characters[i].ToString();
                }

                for (int i = 0; i < words.Count(); i++)
                {
                    // Get Enum from file data
                    FileMapCategory EnumFromFile = SelectFileMapCategory(words[i], recipe);
                    if (EnumFromFile == FileMapCategory.UnKnown)
                    {
                        if (!unknownDieTypes.Contains(words[i])) unknownDieTypes.Add(words[i]);
                        unknownDieCount = unknownDieTypes.Count;
                    }

                    else if (EnumFromFile == FileMapCategory.GoodDie)
                    {
                        goodDieCount++;
                        if (!goodDieTypes.Contains(words[i])) goodDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.BadDie)
                    {
                        badDieCount++;
                        if (!badDieTypes.Contains(words[i])) badDieTypes.Add(words[i]);
                    }

                    else if (EnumFromFile == FileMapCategory.IgnoreDie)
                    {
                        ignoreDieCount++;
                        if (!ignoreDieTypes.Contains(words[i])) ignoreDieTypes.Add(words[i]);
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

            List<string> unknownDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(unknownDieTypes, MapFileFormat.CIS_B);
            List<string> goodDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(goodDieTypes, MapFileFormat.CIS_B);
            List<string> badDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(badDieTypes, MapFileFormat.CIS_B);
            List<string> ignoreDieTypes_Rearranged = MapFileReader.MapFileReaderHelper.Rearrange(ignoreDieTypes, MapFileFormat.CIS_B);
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
        public static bool CIS_B_Check(MapFileParameters recipe, string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");

            // Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            try
            {
                // Get Recipe/Device Name
                string device = GetInfo_CIS_B_Method(AllLines, 1, "mat_id=");

                // Get Flat Notch Orientation
                int waferOrientation = int.Parse((GetInfo_CIS_B_Method(AllLines, 5, "notch_loc=")));

                return true;
            }

            catch { return false; }
        }
        public static string CIS_B_RecipeName(string filepath)
        {
            if (!File.Exists(filepath)) throw new IOException(filepath + " does not exist");
            // 1. Read All Lines
            string[] AllLines = File.ReadAllLines(filepath, Encoding.UTF8);

            // Get Row Count and Column Count
            int RowCount = int.Parse(GetInfo_CIS_B_Method(AllLines, 3, "rows="));
            int ColumnCount = int.Parse(GetInfo_CIS_B_Method(AllLines, 4, "cols="));

            // Get Recipe / Device Name
            string deviceName = "CIS_B_Row" + RowCount + "_Col" + ColumnCount;
            return deviceName;
        }
        private static string GetInfo_CIS_B_Method(string[] AllLines, int RowThatContainsTheInfo, string filter)
        {
            string line = AllLines[RowThatContainsTheInfo];
            string trimmedLine;
            if (line.Contains(filter))
            {
                string[] splittedLine = line.Split('=');

                if (splittedLine.Count() == 2)
                {
                    trimmedLine = splittedLine[1];
                }
                else if (splittedLine.Count() == 1)
                {
                    trimmedLine = "";
                }
                else throw new Exception("Invalid spliitedLine Count");
            }

            else throw new ArgumentException(
                string.Format("line does not contains the argument {0}", filter));
            return trimmedLine;
        }

        #endregion CIS B Method   

        public static List<Func<MapFileParameters, string, MapDataFromFile>> ListOfMapFileReaderFormat
        {
            get
            {
                return new List<Func<MapFileParameters, string, MapDataFromFile>>()
                {
                    ASE,
                    AMS,
                    CIS_A,
                    CIS_B
                };
            }
        }

        private static void ValidateResult(List<List<BDMMapFromFile>> mapListofList, int rowCount, int columnCount)
        {
            if (mapListofList.Count != rowCount) throw new Exception("Row Count not tally in Map File");
            if (mapListofList[0].Count != columnCount) throw new Exception("Column Count not tally in Map File");
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
