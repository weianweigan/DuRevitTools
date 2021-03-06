﻿using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DuRevitTools
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("ca14bc6a-efae-4d77-81be-efc3bbdd7866")]
    public class RevitTools : ToolWindowPane
    {
        public RevitToolsControl RevitToolsControl { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RevitTools"/> class.
        /// </summary>
        public RevitTools() : base(null)
        {
            this.Caption = "RevitTools";
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.

            RevitToolsControl = new RevitToolsControl();


            //view.ViewModel.VsDebugger = debugger;

            this.Content = RevitToolsControl;
        }

        internal void InjectService(IVsDebugger vsDebugger)
        {
            if (RevitToolsControl != null)
            {
                RevitToolsControl.ViewModel.VsDebugger = vsDebugger;
            }
        }
    }
}
