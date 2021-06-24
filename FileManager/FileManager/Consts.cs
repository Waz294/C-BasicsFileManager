using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    static class Consts
    {
        public static string lastDirFilepath = @".\lastDir.txt";
        public static string ErrorCataloguePath = @".\Errors\";

        public static string[] CommandList =
        {
            "help",
            "cp",
            "del",
            "info",
            "cd",
            "clr"
        };

        public static string[] CommandsDescriptions =
        {
            "help command_name - Output list of commands.\n\t- command_name - output info about a command.",

            "cp source destination - Copy file or directory from source to destination.\n\t- source - current file or directory path. Can be relative.\n\t- destrination - path for a new copy of a file or directory. Can be relative.",

            "del path - Delete file or directory.\n\t- path - file or directory path. Can be relative.",

            "info path - Show info of file or directory.\n\t- path - file or directory path. Can be relative.",

            "cd path - Move to new catalogue.\n\t- path - directory path. Can be relative.",

            "clr - Clear output window."
        };
    }
}
