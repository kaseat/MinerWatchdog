using System;

namespace WDG.Abstract
{
    public interface ITimer
    {
        /// <summary>
        /// Occurs when the interval elapses.
        /// </summary>
        event Action Elapsed;

        /// <summary>
        /// Gets or sets a value indicating whether the System.Timers.Timer should
        /// raise the System.Timers.Timer.Elapsed event each time the specified
        ///  interval elapses or only after the first time it elapses.
        /// </summary>
        /// <returns>true if the System.Timers.Timer should raise the
        /// System.Timers.Timer.Elapsed event each time the interval elapses;
        /// false if it should raise the System.Timers.Timer.Elapsed event only once,
        /// after the first time the interval elapses. The default is true.</returns>
        Boolean AutoReset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the System.Timers.Timer should
        /// raise the System.Timers.Timer.Elapsed event.
        /// </summary>
        /// <returns>true if the System.Timers.Timer should raise the System.Timers.Timer.Elapsed event;
        /// otherwise, false. The default is false.</returns>
        /// <exception cref="System.ObjectDisposedException">This property cannot be set because the timer has been disposed.</exception>
        /// <exception cref="System.ArgumentException">The System.Timers.Timer.Interval property was
        /// set to a value greater than System.Int32.MaxValue before the timer was enabled.</exception>
        Boolean Enabled { get; set; }

        /// <summary>
        /// Gets or sets the interval at which to raise the System.Timers.Timer.Elapsed event.
        /// </summary>
        /// <returns>The time, in milliseconds, between System.Timers.Timer.Elapsed events. The value
        /// must be greater than zero, and less than or equal to System.Int32.MaxValue. The default is 100 milliseconds.</returns>
        /// <exception cref="System.ArgumentException">The interval is less than or equal to zero.-or-The interval
        /// is greater than System.Int32.MaxValue, and the timer is currently enabled. (If the timer is not
        /// currently enabled, no exception is thrown until it becomes enabled.)</exception>
        Double Interval { get; set; }

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that are issued when an interval has elapsed.
        /// </summary>
        /// <returns>The System.ComponentModel.ISynchronizeInvoke representing the object used to marshal
        /// the event-handler calls that are issued when an interval has elapsed. The default is null.</returns>
        System.ComponentModel.ISynchronizeInvoke SynchronizingObject { get; set; }

        /// <summary>
        /// Begins the run-time initialization of a System.Timers.Timer that is used on a form or by another component.
        /// </summary>
        void BeginInit();

        /// <summary>
        /// Releases the resources used by the System.Timers.Timer.
        /// </summary>
        void Close();

        /// <summary>
        /// Ends the run-time initialization of a System.Timers.Timer that is used on a form or by another component.
        /// </summary>
        void EndInit();

        /// <summary>
        /// Starts raising the System.Timers.Timer.Elapsed event by setting System.Timers.Timer.Enabled to true.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">The System.Timers.Timer is created with an
        /// interval equal to or greater than System.Int32.MaxValue + 1, or set to an interval less than zero.</exception>
        void Start();

        /// <summary>
        /// Stops raising the System.Timers.Timer.Elapsed event by setting System.Timers.Timer.Enabled to false.
        /// </summary>
        void Stop();
    }
}