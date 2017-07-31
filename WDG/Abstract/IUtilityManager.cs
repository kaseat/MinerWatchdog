using System;

namespace WDG.Abstract
{
    /// <summary>
    /// Provides internet status interface.
    /// </summary>
    public interface IUtilityManager
    {
        /// <summary>
        /// Test if current computer has an internet access.
        /// </summary>
        /// <returns>true if internet is available, otherwise false.</returns>
        Boolean IsOnline();

        /// <summary>
        /// Restart process.
        /// </summary>
        /// <param name="process">Process to be restarted.</param>
        void RestartProcess(IProcess process);

        /// <summary>
        /// Start process.
        /// </summary>
        /// <param name="process">Process to be started.</param>
        void StartProcess(IProcess process);

        /// <summary>
        /// Stop process.
        /// </summary>
        /// <param name="process">Process to be stopped.</param>
        void StopProcess(IProcess process);

        /// <summary>
        /// Restart PC.
        /// </summary>
        void RestartPc();
    }
}