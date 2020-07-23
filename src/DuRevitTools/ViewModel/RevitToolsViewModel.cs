using DuRevitTools.Service;
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
        private RelayCommand _openRevitPath;
        private RelayCommand _documentCommand;
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

        ///<summary>*.addin 文件生成器</summary>
        public RelayCommand AddinGeneratorCommand => _addinGenerator ?? (_addinGenerator = new RelayCommand(AddinGeneratorClick));

        ///<summary>附加到revit进程</summary>
        public RelayCommand AttachToRevitCommand => _attachToRevitCommand ?? (_attachToRevitCommand = new RelayCommand(AttachToRevitClick, true));

        public RelayCommand DocumentCommand => _documentCommand ?? (_documentCommand = new RelayCommand(DocuemntClick, true));

        public RelayCommand OpenRevitPath => _openRevitPath ?? (_openRevitPath = new RelayCommand(OpenRevitPathClick,true));

        #endregion

        #region Private Methods

        ///<summary>Open revit location path</summary>
        private void OpenRevitPathClick()
        {
            var revitVersions = RevitPathProvider.Default.GetInstallVersion();
            foreach (var ver in revitVersions)
            {
                OpenFolder(ver.InstallLocation);
            }
        }

        [Obsolete]
        ///<summary>Open docuemt</summary>
        private void DocuemntClick()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var sln = DuRevitToolsPackage.VsDTE.Solution;

            foreach (Project project in sln.Projects)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    Debug.Print(item.Name);
                }
            }
        }

        ///<summary>附加进程</summary>
        private void AttachToRevitClick()
        {
            try
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

                //Check runing state
                var revitPtr = GetRevitProcessPtr(out var process);
                if (revitPtr == null)
                {
                    MessageBox.Show("Revit.exe Not Found In Processes");
                    return;
                }

                var dbg = DuRevitTools.DuRevitToolsPackage.Debugger;

                if (dbg == null)
                {
                    return;
                }

                EnvDTE80.Transport trans = null;

                //使用默认的连接类型
                foreach (EnvDTE80.Transport transItem in dbg.Transports)
                {
                    if (trans == null)
                    {
                        trans = transItem;
                    }
                    if (transItem.Name == "defualt" || transItem.Name == "默认值")
                    {
                        trans = transItem;
                        break;
                    }
                }

                if (trans == null)
                {
                    MessageBox.Show("Transfort Not Found");
                }

                //使用本机进程
                var proc2 = dbg.GetProcesses(trans, Environment.MachineName).Item("Revit.exe") as EnvDTE80.Process2;

                //附加到进程
                proc2?.Attach();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Attach to Revit process failed:{ex.Message}");
            }
        }

        ///<summary>生成器</summary>
        private void AddinGeneratorClick()
        {
            MessageBox.Show("Developing,Visit https://github.com/weianweigan/DuRevitTools for more");
        }

        ///<summary>在线帮助</summary>
        private void OnlineHelpClick()
        {
            StartUrl("https://www.autodesk.com/developer-network/platform-technologies/revit");
        }

        ///<summary>获取版本</summary>
        private string[] GetVersions()
        {
            var versions = Directory.GetDirectories(AddinsPath).Select(p => p.Split('\\').LastOrDefault()).ToArray();
            if (versions != null && versions.Length > 0)
            {
                SelectVersion = versions.First();
            }
            return versions;
        }

        ///<summary>打开文件夹</summary>
        private void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        #endregion

        #region Public Methods

        ///<summary>获取revit进程</summary><param name="process">进程对象</param>
        public IntPtr? GetRevitProcessPtr(out Process process)
        {
            Process[] processes = Process.GetProcesses();

            process = processes?.Where(p => p.ProcessName == "Revit").FirstOrDefault();

            return process?.Handle;
        }

        ///<summary>查看路径</summary>
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

        ///<summary>启动某个路径</summary>
        public void StartUrl(string path)
        {
            System.Diagnostics.Process.Start(path);
        }

        #endregion
    }
}
