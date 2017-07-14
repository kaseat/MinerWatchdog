using System;
using System.Net.NetworkInformation;
using WDG.Abstract;

namespace WDG.Concrete
{
    /// <summary>
    /// Online status provider.
    /// </summary>
    public class OnlineStatuaProvider : IUtilityManager
    {
        /// <summary>
        /// Test if current computer has an internet access.
        /// </summary>
        /// <returns>true if internet is available, otherwise false.</returns>
        public Boolean IsOnline()
        {
            try
            {
                using (var ping = new Ping())
                {
                    const String host = "google.com";
                    const Int32 timeout = 1000;
                    var buffer = new Byte[32];

                    var reply = ping.Send(host, timeout, buffer, new PingOptions());
                    return reply?.Status == IPStatus.Success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RestartProcess(IProcess process)
        {
            process.Kill();
            process.Start();
        }
    }
}