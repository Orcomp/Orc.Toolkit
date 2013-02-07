// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooltipTimer.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   A timer used to open and close tooltips.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    ///     A timer used to open and close tooltips.
    /// </summary>
    internal sealed class TooltipTimer : DispatcherTimer
    {
        #region Constants

        /// <summary>
        ///     The timer interval.
        /// </summary>
        private const int TimerInterval = 100;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TooltipTimer"/> class.
        /// </summary>
        /// <param name="maximumTicks">
        /// The maximum ticks.
        /// </param>
        /// <param name="initialDelay">
        /// The initial delay.
        /// </param>
        public TooltipTimer(TimeSpan maximumTicks, TimeSpan initialDelay)
        {
            this.InitialDelay = initialDelay;
            this.MaximumTicks = maximumTicks;
            this.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            this.Tick += this.OnTick;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     This event occurs when the timer has stopped.
        /// </summary>
        public event EventHandler Stopped;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the current tick of the ToolTipTimer.
        /// </summary>
        public int CurrentTick { get; private set; }

        /// <summary>
        ///     Gets the initial delay for this timer in seconds.
        ///     When the maximum number of ticks is hit, the timer will stop itself.
        /// </summary>
        /// <remarks>The default delay is 0 seconds.</remarks>
        public TimeSpan InitialDelay { get; private set; }

        /// <summary>
        ///     Gets the maximum number of seconds for this timer.
        ///     When the maximum number of ticks is hit, the timer will stop itself.
        /// </summary>
        /// <remarks>The minimum number of seconds is 1.</remarks>
        public TimeSpan MaximumTicks { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Resets the ToolTipTimer and starts it.
        /// </summary>
        public void StartAndReset()
        {
            this.CurrentTick = 0;
            this.Start();
        }

        /// <summary>
        ///     Stops the ToolTipTimer.
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
        ///     Stops the ToolTipTimer and resets its tick count.
        /// </summary>
        public void StopAndReset()
        {
            this.Stop();
            this.CurrentTick = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnTick(object sender, EventArgs e)
        {
            this.CurrentTick += TimerInterval;
            if (this.CurrentTick >= (this.MaximumTicks.TotalMilliseconds + this.InitialDelay.TotalMilliseconds))
            {
                this.Stop();
            }
        }

        #endregion
    }
}