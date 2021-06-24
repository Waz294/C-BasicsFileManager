using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    static class ExceptionHelper
    {
        public static void ClearExceptionFolder()
        {

            DirectoryInfo di = new DirectoryInfo(Consts.ErrorCataloguePath);

            if (!di.Exists)
            {
                di.Create();
                return;
            }

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public static void WriteException(Exception e, string name)
        {
            var filePath = Consts.ErrorCataloguePath + e.GetType() + "_" + name + ".txt";

            File.WriteAllText(filePath, e.ToString());
        }
    }
}
