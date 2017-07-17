using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using OpenHardwareMonitor.Hardware;
using WDG.Abstract;
using WDG.Extensions;

namespace WDG
{
    /// <summary>
    /// Represents hardware monitor.
    /// </summary>
    public class Superviser
    {
        private const Int32 MaxItertions = 5;
        private Int32 iteration = MaxItertions;
        private Boolean isMonitoring;
        private readonly IProcess supervisedProcess;
        private readonly ITimer timer;
        private readonly IComputer computer;
        private readonly Arguments args;
        private readonly IUtilityManager utilMgr;
        /// <summary>
        /// Create instance of superviser.
        /// </summary>
        /// <param name="parameters">Superviser parameters.</param>
        public Superviser(SuperviserParameters parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            supervisedProcess = parameters.Process ?? throw new ArgumentNullException(nameof(parameters.Process));
            args = parameters.Args ?? throw new ArgumentNullException(nameof(parameters.Args));
            timer = parameters.Timer ?? throw new ArgumentNullException(nameof(parameters.Timer));
            computer = parameters.Computer ?? throw new ArgumentNullException(nameof(parameters.Computer));
            utilMgr = parameters.UtilityManager ?? throw new ArgumentNullException(nameof(parameters.UtilityManager));
        }

        /// <summary>
        /// Start superwised app and hardware monitor.
        /// </summary>
        public void Start()
        {
            if (isMonitoring) return;
            isMonitoring = true;

            supervisedProcess.StartInfo = new ProcessStartInfo(args.Miner)
            {
                UseShellExecute = false
            };

            if (args.MinerPath != null)
            {
                supervisedProcess.StartInfo.WorkingDirectory = args.MinerPath;
                supervisedProcess.StartInfo.FileName = $@"{args.MinerPath}\{args.Miner}";
            }
            if (args.MinerArgs != null) supervisedProcess.StartInfo.Arguments = args.MinerArgs;

            supervisedProcess.Start();
            timer.Interval = args.GpuUpdateRate;
            timer.Elapsed += OnElapse;
            timer.Start();

            Output.WriteLine("Superviser started", ConsoleColor.Green);
            var gpus = computer.Hardware.Where(x => x.HardwareType == HardwareType.GpuNvidia || x.HardwareType == HardwareType.GpuAti);

            foreach (var gpu in gpus)
                Output.WriteLine($"GPU {gpu.Name} (gpu{Regex.Match(gpu.Identifier.ToString(), @"\d+").Value}) found.", ConsoleColor.Green);

            supervisedProcess.WaitForExit();

            supervisedProcess.Kill();
            timer.Elapsed -= OnElapse;
            isMonitoring = false;
        }

        /// <summary>
        /// On timer elapse handler.
        /// </summary>
        /// <remarks>
        /// The main idea of this handler is to get GPU load values in order to
        /// make decision on restart supervised app. First we get all GPU load
        /// valuers and show them to user. After we calculate lowest one and if
        /// calculated value lowerer than the theshold supervised app restarts.
        /// We have 5 attempts.
        /// </remarks>
        private void OnElapse()
        {
            var hasInet = true;
            if (args.CheckInet)
            {
                hasInet = utilMgr.IsOnline();
                Output.Write(hasInet ? "Internet ok; " : "No internrt; ", ConsoleColor.Cyan);
            }
            var gpus = computer.Hardware.Where(x => x.HardwareType == HardwareType.GpuNvidia || x.HardwareType == HardwareType.GpuAti);
            foreach (var gpu in gpus)
            {
                gpu.Update();
                var load = gpu.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Load);
                var tmpr = gpu.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature);
                Output.Write($"gpu{Regex.Match(gpu.Identifier.ToString(), @"\d+").Value}(ld={load?.Value}%, t={tmpr?.Value} C);  ",ConsoleColor.Cyan);
            }
            Output.WriteLine();

            var minLoad = computer.Hardware.Min(x => x.Sensors.FirstOrDefault(y => y.SensorType == SensorType.Load)?.Value);

            if (minLoad < args.GpuLoadThreshold) Output.WriteLine($"GPU LOAD {minLoad}%, THERESHOLD {args.GpuLoadThreshold}! RESTART MINER IN {--iteration}",ConsoleColor.Red);
            else if(hasInet) iteration = MaxItertions;
            if (args.CheckInet)
            {
                if (!hasInet)
                    Output.WriteLine($"NO INTERNET CONNECTION! RESTART MINER IN {--iteration}", ConsoleColor.Red);
                else if (minLoad > args.GpuLoadThreshold) iteration = MaxItertions;
            }

            if (iteration > 0) return;

            timer.Enabled = false;

            Output.Write("RESTARTING MINER...", ConsoleColor.Yellow);
            utilMgr.RestartProcess(supervisedProcess);
            Output.WriteLine(" OK!", ConsoleColor.Green);

            timer.Enabled = true;
            iteration = MaxItertions;
        }
    }
}