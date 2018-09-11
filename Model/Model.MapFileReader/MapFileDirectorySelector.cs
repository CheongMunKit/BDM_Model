using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDMVision.Model.MapFile;

namespace BDMVision.Model.MapFileReader
{
    public static class MapFileDirectorySelector
    {
        public static string SelectDirectory(
            string customerName, 
            MapFileDirectoriesParameters parameters)
        {
            foreach (MapFileDirectoryVariables var in parameters.MapFileDirectoryVariable_List)
            {
                foreach(string name in var.CustomerNames)
                {
                    if (name.Equals(customerName, StringComparison.CurrentCultureIgnoreCase))
                        return var.FileDirectory;
                }
            }
            throw new Exception(customerName + " does not exists in any of the file path");
        }

        public static bool CheckDirectory(
            string customerName,
            MapFileDirectoriesParameters parameters)
        {
            foreach (MapFileDirectoryVariables var in parameters.MapFileDirectoryVariable_List)
            {
                foreach (string name in var.CustomerNames)
                {
                    if (name.Equals(customerName, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
            }
            return false;
        }
    }
}
