// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooltipTimer.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.TooltipControls
{
    using System;
    using System.Windows.Threading;

    internal sealed class TooltipTimer : DispatcherTimer
    {
        private const int TimerInterval = 100;

        public TooltipTimer(TimeSpan maximumTicks, TimeSpan initialDelay)
        {
            this.InitialDelay = initialDelay;
            this.MaximumTicks = maximumTicks;
            this.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            this.Tick += this.OnTick;
        }

        /// <summary>
        /// This event occurs when the timer has stopped.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Gets the maximum number of seconds for this timer.
        /// When the maximum number of ticks is hit, the timer will stop itself.
        /// </summary>
        /// <remarks>The minimum number of seconds is 1.</remarks>
        public TimeSpan MaximumTicks { get; private set; }

        /// <summary>
        /// Gets the initial delay for this timer in seconds.
        /// When the maximum number of ticks is hit, the timer will stop itself.
        /// </summary>
        /// <remarks>The default delay is 0 seconds.</remarks>
        public TimeSpan InitialDelay { get; private set; }

        /// <summary>
        /// Gets the current tick of the ToolTipTimer.
        /// </summary>
        public int CurrentTick { get; private set; }

        /// <summary>
        /// Stops the ToolTipTimer.
        /// </summary>
        public new void Stop()
        {
            base.Stop();
            if (this.Stopped != null)
            {
                this.Stopped(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Resets the ToolTipTimer and starts it.
        /// </summary>
        public void StartAndReset()
        {
            this.CurrentTick = 0;
            this.Start();
        }

        /// <summary>
        /// Stops the ToolTipTimer and resets its tick count.
        /// </summary>
        public void StopAndReset()
        {
            this.Stop();
            this.CurrentTick = 0;
        }

        private void OnTick(object sender, EventArgs e)
        {
            this.CurrentTick += TimerInterval;
            if (this.CurrentTick >= (this.MaximumTicks.TotalMilliseconds + this.InitialDelay.TotalMilliseconds))
            {
                this.Stop();
            }
        }
    }
}
