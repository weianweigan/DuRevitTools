using System;

namespace DuRevitTools.IService
{
    public interface IRevitPathProvider
    {
        ///<summary>Get Version Data from Regedit</summary>
        System.Collections.Generic.IEnumerable<DuRevitTools.Model.RevitVersion> GetInstallVersion();

    }
}
