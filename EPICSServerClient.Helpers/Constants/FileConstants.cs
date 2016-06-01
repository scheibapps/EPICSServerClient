using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPICSServerClient.Helpers.Constants
{
    public class FileConstants
    {
        public static string EpicsServerClientDirectory = GetApplicationFolder();
        public static string EpicsServerClientConnectionsFile = GetConnectionsFile();

        private static string GetApplicationFolder()
        {
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EPICSServerClient");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private static string GetConnectionsFile()
        {
            var filePath = Path.Combine(GetApplicationFolder(),"Connections.xml");
            return filePath;
        }
    }
}
