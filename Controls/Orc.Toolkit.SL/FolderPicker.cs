// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderPicker.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The folder picker control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;

    using Orc.Toolkit.Commands;
    using Orc.Toolkit.Dialogs;
#if (!SILVERLIGHT)
#endif

    /// <summary>
    ///     The folder picker control.
    /// </summary>
    public class FolderPicker : Control
    {
        #region Static Fields

        /// <summary>
        ///     The folder browser dialog service property.
        /// </summary>
        public static readonly DependencyProperty FolderBrowserDialogServiceProperty =
            DependencyProperty.Register(
                "FolderBrowserDialogService", typeof(IFolderBrowserDialogService), typeof(FolderPicker), null);

        /// <summary>
        ///     The Folder property.
        /// </summary>
        public static readonly DependencyProperty FolderProperty = DependencyProperty.Register(
            "Folder", typeof(string), typeof(FolderPicker), null);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderPicker" /> class.
        /// </summary>
        public FolderPicker()
        {
            this.BrowseCommand = new DelegateCommand(this.Browse, this.CanBrowse);
            this.DefaultStyleKey = typeof(FolderPicker);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets BrowseCommand.
        /// </summary>
        public ICommand BrowseCommand { get; set; }

        /// <summary>
        ///     Gets or sets Folder.
        /// </summary>
        public string Folder
        {
            get
            {
                return (string)this.GetValue(FolderProperty);
            }

            set
            {
                this.SetValue(FolderProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the folder browser dialog service.
        /// </summary>
        public IFolderBrowserDialogService FolderBrowserDialogService
        {
            get
            {
                return (IFolderBrowserDialogService)this.GetValue(FolderBrowserDialogServiceProperty);
            }

            set
            {
                this.SetValue(FolderBrowserDialogServiceProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Browse operation.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        private void Browse(object parameter)
        {
            if (this.FolderBrowserDialogService != null)
            {
                string dir = this.Folder;
                if (this.FolderBrowserDialogService.ShowFolderBrowserDialog(ref dir))
                {
                    this.Folder = dir;
                }
            }
            else
            {
                // use default win32 dialog
                var d = new BrowseForFolderDialog { InitialFolder = this.Folder };

                if (true == d.ShowDialog(this.GetOwnerWindow()))
                {
                    this.Folder = d.SelectedFolder;
                }
            }
        }

        /// <summary>
        /// The can browse.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanBrowse(object parameter)
        {
            return true;
        }

        /// <summary>
        ///     The get owner window.
        /// </summary>
        /// <returns>
        ///     The <see cref="Window" />.
        /// </returns>
        private Window GetOwnerWindow()
        {
#if (SILVERLIGHT)
            return Application.Current.IsRunningOutOfBrowser ? Application.Current.MainWindow : null;
#else
            return BrowserInteropHelper.IsBrowserHosted ? null : Application.Current.MainWindow;
#endif
        }

        #endregion
    }
}