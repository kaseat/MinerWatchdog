using System;
using OpenHardwareMonitor.Hardware;
using WDG.Concrete;
using WDG.Extensions;

namespace WDG
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            try
            {
                // Init hw monitor.
                var computer = new Computer {GPUEnabled = true};
                Output.Write("Initializing hardware monitor...", ConsoleColor.Yellow);
                computer.Open();
                Output.WriteLine("OK!", ConsoleColor.Yellow);
                // Init superviser params.
                var prms = new SuperviserParameters
                {
                    Args = args.Parse(),
                    Process = new Process(),
                    Computer = computer,
                    Timer = new Timer(),
                    UtilityManager = new UtilityManager()
                };

                // Start superviser.
                new Superviser(prms).Start();
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message, ConsoleColor.Red);
            }
        }
    }
}

