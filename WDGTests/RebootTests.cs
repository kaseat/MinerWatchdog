using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenHardwareMonitor.Hardware;
using Rhino.Mocks;
using WDG;
using WDG.Abstract;

namespace WDGTests
{
    public class Gpu
    {
        public ISensor Temperature { get; set; }
        public ISensor Load { get; set; }
    }

    [TestClass]
    public class RebootTests
    {
        private SuperviserParameters pr;
        private readonly Gpu gpu0 = new Gpu();
        private readonly Gpu gpu1 = new Gpu();

        [TestInitialize]
        public void Sta()
        {
            pr = new SuperviserParameters
            {
                Process = MockRepository.GenerateStub<IProcess>(),
                Args = new Arguments(),
                Computer = MockRepository.GenerateStub<IComputer>(),
                UtilityManager = MockRepository.GenerateStub<IUtilityManager>(),
                Timer = MockRepository.GenerateStub<ITimer>()
            };

            gpu0.Load = MockRepository.GenerateStub<ISensor>();
            gpu0.Temperature = MockRepository.GenerateStub<ISensor>();

            gpu1.Load = MockRepository.GenerateStub<ISensor>();
            gpu1.Temperature = MockRepository.GenerateStub<ISensor>();

            // sensor stubs
            gpu0.Temperature.Stub(x => x.SensorType).Return(SensorType.Temperature);
            gpu0.Load.Stub(x => x.SensorType).Return(SensorType.Load);
            gpu1.Temperature.Stub(x => x.SensorType).Return(SensorType.Temperature);
            gpu1.Load.Stub(x => x.SensorType).Return(SensorType.Load);

            // gpu stubs
            var g0 = MockRepository.GenerateMock<IHardware>();
            g0.Stub(x => x.HardwareType).Return(HardwareType.GpuNvidia);
            g0.Stub(x => x.Name).Return("NVIDIA 8800 GTS");
            g0.Stub(x => x.Identifier).Return(new Identifier("nvidiagpu", "0"));
            g0.Stub(x => x.Sensors).Return(new[] {gpu0.Temperature, gpu0.Load});

            var g1 = MockRepository.GenerateMock<IHardware>();
            g1.Stub(x => x.HardwareType).Return(HardwareType.GpuNvidia);
            g1.Stub(x => x.Name).Return("NVIDIA GeForce GTX 1070");
            g1.Stub(x => x.Identifier).Return(new Identifier("nvidiagpu", "1"));
            g1.Stub(x => x.Sensors).Return(new[] {gpu1.Temperature, gpu1.Load});

            // hardware stub
            pr.Computer.Stub(x => x.Hardware).Return(new[] {g0, g1});
        }

        [DataTestMethod]
        [DataRow(56, 100, 63, 95, false)] //No internrt; gpu0(ld=100%, t=56 C);  gpu1(ld=95%, t=63 C)
        [DataRow(57, 99, 28, 15, false)] //No internrt; gpu0(ld=99%, t=57 C);  gpu1(ld=15%, t=28 C)
        [DataRow(69, 100, 27, 3, true)] //Internet ok; gpu0(ld=100%, t=69 C);  gpu1(ld=3%, t=27 C)
        public void VerifySupervisedAppRestarts(Single t0, Single l0, Single t1, Single l1, Boolean ic)
        {
            pr.UtilityManager.Stub(x => x.IsOnline()).Return(ic);
            gpu0.Temperature.Stub(x => x.Value).Return(t0);
            gpu0.Load.Stub(x => x.Value).Return(l0);
            gpu1.Temperature.Stub(x => x.Value).Return(t1);
            gpu1.Load.Stub(x => x.Value).Return(l1);

            pr.Process.Stub(x => x.WaitForExit()).WhenCalled(x => Thread.Sleep(200));
            using (var tm = new System.Timers.Timer(10))
            {
                void OnElapsed(Object sender, System.Timers.ElapsedEventArgs e)
                    => pr.Timer.Raise(x => x.Elapsed += null);

                var superviser = new Superviser(pr);
                tm.Elapsed += OnElapsed;
                tm.Start();
                superviser.Start();

                tm.Elapsed -= OnElapsed;
            }

            pr.UtilityManager.AssertWasCalled(x => x.RestartProcess(Arg<IProcess>.Is.Anything));
        }

        [DataTestMethod]
        [DataRow(56, 100, 63, 95, true, 20)] //Internet ok; gpu0(ld=100%, t=56 C);  gpu1(ld=95%, t=63 C)
        [DataRow(58, 10, 62, 99, true, 9)] //Internet ok; gpu0(ld=10%, t=58 C);  gpu1(ld=99%, t=62 C);
        public void VerifySupervisedAppNotRestarts(Single t0, Single l0, Single t1, Single l1, Boolean ic, Int32 tr)
        {
            pr.Args.GpuLoadThreshold = Convert.ToByte(tr);
            pr.UtilityManager.Stub(x => x.IsOnline()).Return(ic);
            gpu0.Temperature.Stub(x => x.Value).Return(t0);
            gpu0.Load.Stub(x => x.Value).Return(l0);
            gpu1.Temperature.Stub(x => x.Value).Return(t1);
            gpu1.Load.Stub(x => x.Value).Return(l1);

            pr.Process.Stub(x => x.WaitForExit()).WhenCalled(x => Thread.Sleep(200));
            using (var tm = new System.Timers.Timer(10))
            {
                void OnElapsed(Object sender, System.Timers.ElapsedEventArgs e)
                    => pr.Timer.Raise(x => x.Elapsed += null);

                var superviser = new Superviser(pr);
                tm.Elapsed += OnElapsed;
                tm.Start();
                superviser.Start();

                tm.Elapsed -= OnElapsed;
            }

            pr.UtilityManager.AssertWasNotCalled(x => x.RestartProcess(Arg<IProcess>.Is.Anything));
        }
    }
}
