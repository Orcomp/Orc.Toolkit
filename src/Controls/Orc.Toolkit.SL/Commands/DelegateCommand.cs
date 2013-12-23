// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The DelegateCommand class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.Commands
{
    using System;
    using System.Windows.Input;

    /// <summary>
    ///     The DelegateCommand class.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Fields

        /// <summary>
        ///     The can execute.
        /// </summary>
        private readonly Func<object, bool> canExecute;

        /// <summary>
        ///     The execute action.
        /// </summary>
        private readonly Action<object> executeAction;

        /// <summary>
        ///     The can execute cache.
        /// </summary>
        private bool canExecuteCache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeAction">
        /// The execute action.
        /// </param>
        /// <param name="canExecute">
        /// The can execute.
        /// </param>
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            bool temp = this.canExecute(parameter);

            if (this.canExecuteCache != temp)
            {
                this.canExecuteCache = temp;
                if (this.CanExecuteChanged != null)
                {
                    this.CanExecuteChanged(this, new EventArgs());
                }
            }

            return this.canExecuteCache;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.executeAction(parameter);
        }

        #endregion
    }
}