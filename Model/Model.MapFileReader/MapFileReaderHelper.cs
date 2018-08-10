using System;
using System.Collections.Generic;
using System.Reflection;
using BDMVision.Model.MapFile;
using System.Linq;

namespace BDMVision.Model.MapFileReader
{
    public static class MapFileReaderHelper
    {
        public static List<string> GetPublicMethodsName(Type T)
        {
            if (T == null) throw new ArgumentNullException("Class type cannot be null");
            List<string> methodList = new List<string>();

            foreach (MethodInfo method in T.GetMethods())
            {
                if (method.IsPublic)
                {
                    if (method.Name != "ToString"   &&
                        method.Name != "Equals"     &&
                        method.Name != "GetHashCode" &&
                        method.Name != "GetType")
                    methodList.Add(method.Name);
                }                  
            }

            return methodList;
        }

        public static string ChooseRecipe(
            List<string> mapReaderMethodList,
            string mapReaderRecipeName)
        {
            foreach (string recipe in mapReaderMethodList)
            {
                if (mapReaderRecipeName == recipe) return recipe;
            }
            return null;
        }

        public static string ChooseRecipe(Type T, string mapReaderRecipeName)
        {
            List<string> mapReaderMethodList =  GetPublicMethodsName(T);
            return ChooseRecipe(mapReaderMethodList, mapReaderRecipeName); 
        }  

        public static bool CheckIfMethodExist(Type T, string ReaderMethod_string)
        {
            List<string> mapReaderMethodList = GetPublicMethodsName(T);  
            foreach (string methodName in mapReaderMethodList)
            {
                if (ReaderMethod_string == methodName) return true;
            }
            return false;
        }


        public static List<List<string>> GetMapsCode(List<List<BDMMapFromFile>> BDMMapFromFileListofList)
        {
            List<List<string>> BDMMapCodeListofList = new List<List<string>>();

            foreach(List<BDMMapFromFile> BDMMapFromFileList in BDMMapFromFileListofList)
            {
                List<string> BDMMapCodeList = new List<string>();
                foreach (BDMMapFromFile map in BDMMapFromFileList)
                {
                    BDMMapCodeList.Add(map.mapCode);
                }
                BDMMapCodeListofList.Add(BDMMapCodeList);
            }
            return BDMMapCodeListofList;
        }

        public static List<string> Rearrange(List<string> unknownDieTypes)
        {
            List<int> IntDieTypes = new List<int>();
            List<string> nonIntDieTypes = new List<string>();
            foreach (string dieType in unknownDieTypes)
            {
                int temp;
                if (int.TryParse(dieType, out temp))
                {
                    IntDieTypes.Add(temp);
                }
                else
                {
                    nonIntDieTypes.Add(dieType);
                }
            }
            var IntDieTypes_Ascending = IntDieTypes.OrderBy(i => i);
            List<string> RearrangedIntStrTypes = new List<string>();

            foreach (int type in IntDieTypes_Ascending)
            {
                string temp;

                // If after parse the die has 2 digits
                if (Math.Floor(Math.Log10(type) + 1) == 2)
                {
                    temp = "0" + type.ToString();
                }
                else if (Math.Floor(Math.Log10(type) + 1) == 1)
                {
                    temp = "0" + "0" + type.ToString();
                }

                else
                {
                    temp = type.ToString();
                }
                RearrangedIntStrTypes.Add(temp);
            }

            List<string> RearrangedTypes_Combined = new List<string>();
            RearrangedTypes_Combined.AddRange(nonIntDieTypes);
            RearrangedTypes_Combined.AddRange(RearrangedIntStrTypes);
            return RearrangedTypes_Combined;
        }
    }
}
