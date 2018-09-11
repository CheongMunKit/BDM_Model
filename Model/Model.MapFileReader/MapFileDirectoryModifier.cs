using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMVision.Model.MapFileReader
{
    public enum FileExtensionModificationMode
    {
        Add,
        Change
    }

    public static class MapFileDirectoryModifier
    {
        public static void RenameAllFilesIntoExtension(
            string FileDirectory, 
            string   extension,
            string[] extensionForReplacement)
        {
            string[] files = Directory.GetFiles(FileDirectory, "*", SearchOption.AllDirectories);            
            foreach(var file in files)
            {
                string currentExtension = Path.GetExtension(file);
                if (currentExtension != extension)
                {
                    // Replace extension
                    if (extensionForReplacement.Contains(currentExtension))
                    {
                        File.Move(file, Path.ChangeExtension(file, extension));
                    }
                    // Add extension
                    else
                    {
                        string newFileName = file + extension;
                        File.Move(file, newFileName);
                    }
                }
            }
        }
    }
}
