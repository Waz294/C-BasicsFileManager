using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    static class ManagerHelper
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
      
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }

        }

        public static string NormalizePath(string input, string currDir)
        {
            var inputArr = input.Split('\\');

            if (inputArr[0].Contains(':'))
                return input;

            var tmpDir = currDir;
            foreach (var item in inputArr)
            {
                if (item == "..")
                {
                    if (tmpDir.Substring(tmpDir.Length - 2) != ":\\")
                    {
                        if (tmpDir.Substring(tmpDir.Length - 1) == "\\")
                            tmpDir = tmpDir.Remove(tmpDir.Length - 1);

                        tmpDir = string.Concat(tmpDir.Reverse().SkipWhile(x => x != '\\').Reverse());
                    }
                }
                else
                {
                    if (tmpDir.Last() != '\\')
                    {
                        tmpDir += '\\' + item;
                    }
                    else
                    {
                        tmpDir += item;
                    }
                }
            }

            return tmpDir;
        }
    }
}
