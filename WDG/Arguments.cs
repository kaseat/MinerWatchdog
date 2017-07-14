using System;

namespace WDG
{
    /// <summary>
    /// Represents startup arguments.
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// Miner app name.
        /// </summary>
        public String Miner { get; set; }

        /// <summary>
        /// Miner app arguments.
        /// </summary>
        public String MinerArgs { get; set; }

        /// <summary>
        /// Miner app working derictory.
        /// </summary>
        public String MinerPath { get; set; }

        /// <summary>
        /// GPU load threshold app restarts after.
        /// </summary>
        public Byte GpuLoadThreshold { get; set; } = 20;

        /// <summary>
        /// GPU load values update rate.
        /// </summary>
        public Int32 GpuUpdateRate { get; set; } = 5000;
    }
}