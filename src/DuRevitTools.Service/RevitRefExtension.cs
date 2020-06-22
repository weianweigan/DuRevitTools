
/*
 * Created by WeiGan 2020/6/22
 * 
 * Some Extension Method to Find *.dll for Revit
 */

using System.IO;

namespace DuRevitTools.Service
{
    ///<summary>Extension Methods for <see cref="RevitVersion"/></summary>
    public static class RevitRefExtension
    {
        private const string RevitAPI = "RevitAPI.dll";
        private const string RevitAPIUI = "RevitAPIUI.dll";

        ///<summary>Get RevitAPI.dll Filepath</summary>
        public static string GetRevitAPI(this RevitVersion revit)
        {
            return Path.Combine(revit.InstallLocation, RevitAPI);
        }

        ///<summary>TryGetRevit API Extension</summary>
        public static bool TryGetRevitAPI(this RevitVersion revit,out string filePath)
        {
            filePath = revit.GetRevitAPI();
            return File.Exists(filePath);
        }

        public static string GetRevitAPIUI(this RevitVersion revit)
        {
            return Path.Combine(revit.InstallLocation, RevitAPIUI);
        }

        public static bool TryGetRevitAPIUI(this RevitVersion revit, out string filePath)
        {
            filePath = revit.GetRevitAPIUI();
            return File.Exists(filePath);
        }
    }
}
