using EnvDTE;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Process = System.Diagnostics.Process;

namespace DuRevitTools
{
    public class RevitToolsViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _addinPathCommand;
        private string[] _revitVersions;
        private string _selectVersion;
        private const string AddinsPath = @"C:\ProgramData\Autodesk\Revit\Addins";
        private RelayCommand _attachToRevitCommand;
        private RelayCommand _addinGenerator;
        private RelayCommand _onlineHelpCommand;

        #endregion

        #region .ctor

        public RevitToolsViewModel()
        {
        }

        #endregion

        #region Properties

        public IEnumerable<string> RevitVersions => _revitVersions ?? (_revitVersions = GetVersions());

        public string SelectVersion { get => _selectVersion; set => Set(ref _selectVersion, value); }

        public IVsDebugger VsDebugger { get; set; }

        #endregion

        #region Commands

        //插件路径
        public RelayCommand AddinPathCommand { get => _addinPathCommand ?? (_addinPathCommand = new RelayCommand(AddinPathClick)); }

        //在线帮助
        public RelayCommand OnlineHelpCommand => _onlineHelpCommand ?? (_onlineHelpCommand = new RelayCommand(OnlineHelpClick));

        //*.addin 文件生成器
        public RelayCommand AddinGeneratorCommand => _addinGenerator ?? (_addinGenerator = new RelayCommand(AddinGeneratorClick));

        //附加到revit进程
        public RelayCommand AttachToRevitCommand => _attachToRevitCommand ?? (_attachToRevitCommand = new RelayCommand(AttachToRevitClick,true));

        #endregion

        #region Private Methods

        //附加进程
        private void AttachToRevitClick()
        {
            try
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                var revitPtr = GetRevitProcessPtr();
                if (revitPtr == null)
                {
                    MessageBox.Show("Revit.exe Not Found In Process");
                }
                DuRevitToolsPackage.VsDebugger?.LaunchDebugTargets(1, revitPtr.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //获取revit进程
        public IntPtr? GetRevitProcessPtr()
        {
            Process[] processes = Process.GetProcesses();

            return processes.Where(p => p.ProcessName == "Revit").FirstOrDefault()?.Handle;
        }

        //生成器
        private void AddinGeneratorClick()
        {
            MessageBox.Show("Developing,Visit https://github.com/weianweigan/DuRevitTools for more");
        }

        //在线帮助
        private void OnlineHelpClick()
        {
            StartUrl("https://www.autodesk.com/developer-network/platform-technologies/revit");
        }

        //获取版本
        private string[] GetVersions()
        {
            var versions = Directory.GetDirectories(AddinsPath).Select(p => p.Split('\\').LastOrDefault()).ToArray();
            if (versions != null && versions.Length > 0)
            {
                SelectVersion = versions.First();
            }
            return versions;
        }

        //查看路径
        public void AddinPathClick()
        {
            if (string.IsNullOrEmpty(SelectVersion))
            {
                MessageBox.Show("Choose a revit version please");
                return;
            }
            var path = $"{AddinsPath}\\{SelectVersion}";
            OpenFolder(path);
        }

        //打开文件夹
        private void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        
        //启动某个路径
        public void StartUrl(string path)
        {
            System.Diagnostics.Process.Start(path);
        }

        #endregion
    }
}
