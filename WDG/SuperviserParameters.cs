using OpenHardwareMonitor.Hardware;
using WDG.Abstract;

namespace WDG
{
    public class SuperviserParameters
    {
        public Arguments Args { get; set; }
        public IComputer Computer { get; set; }
        public IUtilityManager UtilityManager { get; set; }
        public IProcess Process { get; set; }
        public ITimer Timer { get; set; }
    }
}