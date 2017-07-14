using System;
using System.Diagnostics;
using WDG.Abstract;

namespace WDG.Concrete
{
    public class Process : IProcess, IDisposable
    {
        private readonly System.Diagnostics.Process process;

        /// <summary>
        /// Initializes a new instance of the System.Diagnostics.Process class.
        /// </summary>
        public Process() => process = new System.Diagnostics.Process();

        public Boolean Start() => process.Start();

        public void Kill() => process.Kill();

        public ProcessStartInfo StartInfo
        {
            get => process.StartInfo;
            set => process.StartInfo = value;
        }

        public void WaitForExit()
        {
            for (;;) if (Console.ReadKey().Key == ConsoleKey.Escape) break;
        }

        public void Dispose()
        {
            process.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}