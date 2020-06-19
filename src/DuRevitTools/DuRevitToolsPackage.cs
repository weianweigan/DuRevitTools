using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace DuRevitTools
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(DuRevitToolsPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(RevitTools))]
    public sealed class DuRevitToolsPackage : AsyncPackage,IServiceProvider
    {
        /// <summary>
        /// DuRevitToolsPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "e57b192c-0214-4de9-966d-743de7d9d085";

        #region Properties

        public static Debugger2 Debugger { get; private set; }

        public static EnvDTE80.DTE2 VsDTE { get; private set; }

        public static IVsSolution VsSolution { get; private set; }

        #endregion

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            //求解程序集
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            //DTE和Debug服务
            VsDTE = await this.GetServiceAsync(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE)) as EnvDTE80.DTE2;
            Debugger = VsDTE?.Debugger as EnvDTE80.Debugger2;
            
            //解决方案服务
            VsSolution = await this.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;

            //初始化命令
            await RevitToolsCommand.InitializeAsync(this);
        }

        ///<summary>求解程序集，解决有些wpf程序集加载问题</summary>
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyPath = string.Empty;
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            try
            {
                assemblyPath = Path.Combine(AssemblyPath, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                else
                {
                    System.Diagnostics.Debug.Print($"Assembly Load Error{assemblyPath}");
                }

                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                assemblyPath = Path.Combine(assemblyDirectory, assemblyName);
                return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyName), ex);
            }
        }

        public string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        #endregion
    }
}
