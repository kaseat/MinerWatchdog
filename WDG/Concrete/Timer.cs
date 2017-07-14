using System;
using System.ComponentModel;
using System.Timers;
using WDG.Abstract;

namespace WDG.Concrete
{
    public class Timer : ITimer, IDisposable
    {
        private readonly System.Timers.Timer timer;

        /// <summary>
        /// Initializes a new instance of the System.Timers.Timer class, and sets all the properties to their initial values.
        /// </summary>
        public Timer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += OnElapse;
        }

        /// <summary>
        /// Initializes a new instance of the System.Timers.Timer class, and sets the System.Timers.Timer.Interval property to the specified number of milliseconds.
        /// </summary>
        /// <param name="interval">The time, in milliseconds, between events. The value must be greater than zero and less than or equal to System.Int32.MaxValue.</param>
        /// <exception cref="System.ArgumentException">The value of the interval parameter is less than or equal to zero, or greater than System.Int32.MaxValue.</exception>
        public Timer(Double interval)
        {
            timer = new System.Timers.Timer(interval);
            timer.Elapsed += OnElapse;
        }

        private void OnElapse(Object sender, ElapsedEventArgs e) => Elapsed?.Invoke();

        public event Action Elapsed = delegate { };

        public Boolean AutoReset
        {
            get => timer.AutoReset;
            set => timer.AutoReset = value;
        }

        public Boolean Enabled
        {
            get => timer.Enabled;
            set => timer.Enabled = value;
        }

        public Double Interval
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get => timer.SynchronizingObject;
            set => timer.SynchronizingObject = value;
        }

        public void BeginInit() => timer.BeginInit();

        public void Close() => timer.Close();

        public void Dispose()
        {
            timer.Elapsed -= OnElapse;
            timer.Dispose();
            GC.SuppressFinalize(this);
        }

        public void EndInit() => timer.EndInit();

        public void Start() => timer.Start();

        public void Stop() => timer.Stop();
    }
}