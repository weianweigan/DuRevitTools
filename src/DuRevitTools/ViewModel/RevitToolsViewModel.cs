using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DuRevitTools
{
    public class RevitToolsViewModel : ViewModelBase
    {
        private RelayCommand _addinPathCommand;
        private string[] _revitVersions;
        private string _selectVersion;
        private const string AddinsPath = @"C:\ProgramData\Autodesk\Revit\Addins";
        private RelayCommand _addinGenerator;
        private RelayCommand _onlineHelpCommand;

        #region .ctor

        public RevitToolsViewModel()
        {
        }

        #endregion

        #region Properties

        public IEnumerable<string> RevitVersions => _revitVersions ?? (_revitVersions = GetVersions());

        public string SelectVersion { get => _selectVersion; set => Set(ref _selectVersion, value); }

        #endregion

        #region Commands

        public RelayCommand AddinPathCommand { get => _addinPathCommand ?? (_addinPathCommand = new RelayCommand(AddinPathClick)); }

        public RelayCommand OnlineHelpCommand => _onlineHelpCommand ?? (_onlineHelpCommand = new RelayCommand(OnlineHelpClick));

        public RelayCommand AddinGenerator => _addinGenerator ?? (_addinGenerator = new RelayCommand(AddinGeneratorClick));
        #endregion

        #region Private Methods

        private void AddinGeneratorClick()
        {
            MessageBox.Show("Developing,Visit https://github.com/weianweigan/DuRevitTools for more");
        }

        private void OnlineHelpClick()
        {
            StartUrl("https://www.autodesk.com/developer-network/platform-technologies/revit");
        }

        private string[] GetVersions()
        {
            var versions = Directory.GetDirectories(AddinsPath).Select(p => p.Split('\\').LastOrDefault()).ToArray();
            if (versions != null && versions.Length > 0)
            {
                SelectVersion = versions.First();
            }
            return versions;
        }

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

        private void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        public void StartUrl(string path)
        {
            System.Diagnostics.Process.Start(path);
        }

        #endregion
    }
}
