using System;
using System.Diagnostics;

namespace WDG.Abstract
{
    public interface IProcess
    {
        /// <summary>
        /// Starts (or reuses) the process resource that is specified by the
        ///  <see cref="System.Diagnostics.Process.StartInfo"/> property of
        /// this System.Diagnostics.Process component and associates it with the component.
        /// </summary>
        /// <returns>
        /// true if a process resource is started; false if no new process
        /// resource is started (for example, if an existing process is reused).
        /// </returns>
        /// <exception cref="System.InvalidOperationException">No file name was specified in the
        /// System.Diagnostics.Process component's System.Diagnostics.Process.StartInfo.-or- The
        /// System.Diagnostics.ProcessStartInfo.UseShellExecute member of the System.Diagnostics.Process.StartInfo
        /// property is true while System.Diagnostics.ProcessStartInfo.RedirectStandardInput,
        /// System.Diagnostics.ProcessStartInfo.RedirectStandardOutput, or
        /// System.Diagnostics.ProcessStartInfo.RedirectStandardError is true.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">There was an error in opening the associated file.</exception>
        /// <exception cref="System.ObjectDisposedException">The process object has already been disposed.</exception>
        Boolean Start();

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">The associated process could not be terminated.
        /// -or-The process is terminating.-or- The associated process is a Win16 executable.</exception>
        /// <exception cref="System.NotSupportedException">You are attempting to call System.Diagnostics.Process.Kill
        /// for a process that is running on a remote computer. The method is available only for processes
        /// running on the local computer.</exception>
        /// <exception cref="System.InvalidOperationException">The process has already exited.
        /// -or-There is no process associated with this System.Diagnostics.Process object.</exception>
        void Kill();

        /// <summary>
        /// Gets or sets the properties to pass to the System.Diagnostics.Process.Start method of the System.Diagnostics.Process.
        /// </summary>
        /// <returns>
        /// The System.Diagnostics.ProcessStartInfo that represents the data with which to start the process.
        ///  These arguments include the name of the executable file or document used to start the process.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The value that specifies the System.Diagnostics.Process.StartInfo is null.</exception>
        ProcessStartInfo StartInfo { get; set; }

        void WaitForExit();
    }
}