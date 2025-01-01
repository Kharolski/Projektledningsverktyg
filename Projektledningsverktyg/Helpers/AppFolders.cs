using System;
using System.IO;

namespace Projektledningsverktyg.Helpers
{
    public static class AppFolders
    {
        public static string GetUserImagesPath()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FamilyPlanner",
                "Images",
                "Profiles"
            );

            Directory.CreateDirectory(appDataPath);
            return appDataPath;
        }
    }

}
