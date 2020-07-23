using DuRevitTools.IService;
using DuRevitTools.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DuRevitTools.Service
{
    ///<summary>Get Path Message of Revit in Regedit</summary>
    public class RevitPathProvider: IRevitPathProvider
    {
        ///<inheritdoc/>
        public static IRevitPathProvider Default => new RevitPathProvider();

        ///<inheritdoc/>
        public IEnumerable<RevitVersion> GetInstallVersion()
        {

            Microsoft.Win32.RegistryKey localMachine64 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
            using (RegistryKey key = localMachine64.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", false))
            {
                if (key == null)//判断对象存在
                {
                    yield break;
                }
                foreach (string keyName in key.GetSubKeyNames())//遍历子项名称的字符串数组
                {
                    if (Regex.IsMatch(keyName, "Autodesk Revit [0-9]+"))
                    {
                        using (RegistryKey key2 = key.OpenSubKey(keyName, false))//遍历子项节点
                        {
                            if (key2 == null)
                            {
                                break;
                            }
                            if (TryGetVersionData(key2, out RevitVersion data))
                            {
                                yield return data;
                            }
                        }
                    }
                }
            }
        }

        ///<summary>Get Data from key</summary>
        private bool TryGetVersionData(RegistryKey key,out RevitVersion data)
        {
            data = new RevitVersion();

            var name = key.GetValue("DisplayName", "")?.ToString();

            data.Version =int.Parse(Regex.Match(name, "[0-9]+").Value);

            data.DisplayIcon = key.GetValue("DisplayIcon", "")?.ToString();
            data.DisplayName = key.GetValue("DisplayName", "")?.ToString();
            data.DisplayVersion = key.GetValue("DisplayVersion", "")?.ToString();
            data.InstallDate = key.GetValue("InstallDate", "")?.ToString();
            data.InstallLocation = key.GetValue("InstallLocation", "")?.ToString();
            data.Publisher = key.GetValue("Publisher", "")?.ToString();
            data.UninstallPath = key.GetValue("UninstallPath", "")?.ToString();
            data.UninstallString = key.GetValue("UninstallString", "")?.ToString();

            return true;
        }
    }
}
