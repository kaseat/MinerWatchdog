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
    }
}