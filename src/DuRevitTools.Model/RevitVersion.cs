using System.IO;
using System.Threading;

namespace DuRevitTools.Model
{
    public class RevitVersion
    {
        public string DisplayIcon { get; set; }

        public string DisplayName { get; set; }

        public string DisplayVersion { get; set; }

        public string InstallDate { get; set; }

        public string InstallLocation { get; set; }

        public string UninstallPath { get; set; }

        public string UninstallString { get; set; }

        public string Publisher { get; set; }

        public int Version { get; set; }

        private const string RevitAPI = "RevitAPI.dll";
        private const string RevitAPIDoc = "RevitAPI.xml";
        private const string RevitAPIUI = "RevitAPIUI.dll";

        ///<summary>Get RevitAPI.dll Filepath</summary>
        public string GetRevitAPI()
        {
            return Path.Combine(InstallLocation, RevitAPI);
        }   

        ///<summary>TryGetRevit API Extension</summary>
        public bool TryGetRevitAPI(out string filePath)
        {
            filePath = GetRevitAPI();
            return File.Exists(filePath);
        }

        public string GetRevitAPIUI()
        {
            return Path.Combine(InstallLocation, RevitAPIUI);
        }

        public bool TryGetRevitAPIUI(out string filePath)
        {
            filePath = GetRevitAPIUI();
            return File.Exists(filePath);
        }
    }
}
