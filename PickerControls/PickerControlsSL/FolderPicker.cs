// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderPicker.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The folder picker control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.PickerControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Orc.Toolkit.PickerControls.Commands;
    using Orc.Toolkit.PickerControls.Dialogs;
#if (!SILVERLIGHT)
    using System.Windows.Interop;
#endif

    /// <summary>
    /// The folder picker control.
    /// </summary>
    public class FolderPicker : Control
    {
        #region Static Fields

        /// <summary>
        /// The folder browser dialog service property.
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
        /// Gets or sets the folder browser dialog service.
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
        /// The get owner window.
        /// </summary>
        /// <returns>
        /// The <see cref="Window"/>.
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

    /// <summary>
    ///     Interface for custom browse dialogs.
    /// </summary>
    public interface IFolderBrowserDialogService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The show folder browser dialog.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <param name="showNewFolderButton">
        /// The show new folder button.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="useDescriptionForTitle">
        /// The use description for title.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ShowFolderBrowserDialog(
            ref string folder, 
            bool showNewFolderButton = true, 
            string description = null, 
            bool useDescriptionForTitle = true);

        #endregion
    }
}