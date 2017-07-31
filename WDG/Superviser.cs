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
        private const Int32 MaxItertions = 6;
        private Int32 gpuLoadIterations = MaxItertions;
        private Int32 checkInetIterations = MaxItertions;
        private Boolean isMonitoring;
        private Boolean isStopped;
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
        /// Here is the behaviour logic depending on internet connection status.
        /// </summary>
        private void InternetActions()
        {
            if (!args.CheckInet) return;
            var hasInet = utilMgr.IsOnline();
            if (!isStopped) Output.Write(hasInet ? "Internet ok; " : "No internrt; ", ConsoleColor.Cyan);
            if (hasInet) checkInetIterations = MaxItertions;
            else if(!isStopped) Output.WriteLine($"NO INTERNET CONNECTION! STOPPING MINER IN {--checkInetIterations}", ConsoleColor.Red);
            if (checkInetIterations <= 0)
            {
                if (!isStopped)
                {
                    utilMgr.StopProcess(supervisedProcess);
                    Output.WriteLine("Miner stopped", ConsoleColor.Yellow);
                }
                isStopped = true;
            }
            else if (isStopped)
            {
                utilMgr.StartProcess(supervisedProcess);
                Output.WriteLine("Miner started", ConsoleColor.Yellow);
                isStopped = false;
            }
        }

        /// <summary>
        /// Here is the behaviour logic depending on internet GPU load status.
        /// </summary>
        private void LoadActions()
        {
            if(isStopped) return;

            var gpus = computer.Hardware.Where(x => x.HardwareType == HardwareType.GpuNvidia || x.HardwareType == HardwareType.GpuAti);
            var minLoad = Single.MaxValue;
            foreach (var gpu in gpus)
            {
                gpu.Update();
                var load = Single.NaN;
                var tmpr = Single.NaN;
                foreach (var sensor in gpu.Sensors)
                {
                    if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                        if (sensor.Value != null) load = sensor.Value.Value;
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Core")
                        if (sensor.Value != null) tmpr = sensor.Value.Value;
                    if (load < minLoad) minLoad = load;
                }
                Output.Write($"gpu{Regex.Match(gpu.Identifier.ToString(), @"\d+").Value}(ld={load}%, t={tmpr} C);  ", ConsoleColor.Cyan);
            }
            Output.WriteLine();

            if (minLoad < args.GpuLoadThreshold)
            {
                Output.WriteLine(args.Restart
                        ? $"GPU LOAD {minLoad}%, THRESHOLD {args.GpuLoadThreshold}! RESTART PC IN {--gpuLoadIterations}"
                        : $"GPU LOAD {minLoad}%, THRESHOLD {args.GpuLoadThreshold}! RESTART MINER IN {--gpuLoadIterations}",
                    ConsoleColor.Red);
            }

            else gpuLoadIterations = MaxItertions;

            if (gpuLoadIterations > 0) return;

            if (args.Restart)
            {
                Output.Write("RESTARTING PC IN 5 SECONDS", ConsoleColor.Yellow);
                utilMgr.RestartPc();
            }
            Output.Write("RESTARTING MINER...", ConsoleColor.Yellow);
            utilMgr.RestartProcess(supervisedProcess);
            Output.WriteLine(" OK!", ConsoleColor.Yellow);
            gpuLoadIterations = MaxItertions;
        }

        /// <summary>
        /// On timer elapse handler.
        /// </summary>
        private void OnElapse()
        {
            timer.Stop();

            InternetActions();
            LoadActions();

            timer.Start();
        }
    }
}