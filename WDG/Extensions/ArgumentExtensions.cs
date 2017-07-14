using System;

namespace WDG.Extensions
{
    public static class ArgumentExtensions
    {
        public static Arguments Parse(this String[] args)
        {
            var ar = new Arguments();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-miner")
                    if (i < args.Length - 1)
                        ar.Miner = args[++i];
                if (args[i] == "-minerArgs")
                    if (i < args.Length - 1)
                        ar.MinerArgs = args[++i];
                if (args[i] == "-minerPath")
                    if (i < args.Length - 1)
                        ar.MinerPath = args[++i];
                if (args[i] == "-loadThreshold")
                    if (i < args.Length - 1)
                        ar.GpuLoadThreshold = Byte.Parse(args[++i]);
                if (args[i] == "-uprate")
                    if (i < args.Length - 1)
                        ar.GpuUpdateRate = Int32.Parse(args[++i]);
            }

            if (ar.Miner == null)
            {
                throw new ArgumentException("No miner app specified, parameter \"-miner <MINERAPP.EXE>\" required.", nameof(ar.Miner));
            }

            return ar;
        }
    }
}
