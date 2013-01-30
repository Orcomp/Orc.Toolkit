// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderPicker.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.PickerControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
#if (!SILVERLIGHT)
    using System.Windows.Interop;
#endif

    using Orc.Toolkit.PickerControls.Commands;
    using Orc.Toolkit.PickerControls.Dialogs;

    public class FolderPicker : Control
    {
        /// <summary>
        /// The Folder property.
        /// </summary>
        public static readonly DependencyProperty FolderProperty = 
            DependencyProperty.Register("Folder", typeof(string), typeof(FolderPicker), null);
                                        
        public static readonly DependencyProperty FolderBrowserDialogServiceProperty =
            DependencyProperty.Register("FolderBrowserDialogService", typeof(IFolderBrowserDialogService), typeof(FolderPicker), null);

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderPicker"/> class.
        /// </summary>
        public FolderPicker()
        {
            BrowseCommand = new DelegateCommand(Browse, CanBrowse);
            this.DefaultStyleKey = typeof(FolderPicker);
        }

        /// <summary>
        /// Gets or sets Folder.
        /// </summary>
        public string Folder
        {
            get
            {
                return (string)GetValue(FolderProperty);
            }

            set
            {
                SetValue(FolderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets BrowseCommand.
        /// </summary>
        public ICommand BrowseCommand { get; set; }

        public IFolderBrowserDialogService FolderBrowserDialogService
        {
            get { return (IFolderBrowserDialogService)GetValue(FolderBrowserDialogServiceProperty); }
            set { SetValue(FolderBrowserDialogServiceProperty, value); }
        }

        /// <summary>
        /// Browse operation.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        private void Browse(object parameter)
        {
            if (FolderBrowserDialogService != null)
            {
                var dir = Folder;
                if (FolderBrowserDialogService.ShowFolderBrowserDialog(ref dir))
                {
                    Folder = dir;
                }
            }
            else
            {
                // use default win32 dialog
                var d = new BrowseForFolderDialog { InitialFolder = this.Folder };

                if (true == d.ShowDialog(GetOwnerWindow()))
                {
                    Folder = d.SelectedFolder;
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

        private Window GetOwnerWindow()
        {
#if (SILVERLIGHT)
            return Application.Current.IsRunningOutOfBrowser ? Application.Current.MainWindow : null;
#else
            return BrowserInteropHelper.IsBrowserHosted ? null : Application.Current.MainWindow;
#endif
        }
    }

    /// <summary>
    /// Interface for custom browse dialogs.
    /// </summary>
    public interface IFolderBrowserDialogService
    {
        bool ShowFolderBrowserDialog(ref string folder, bool showNewFolderButton = true, string description = null, bool useDescriptionForTitle = true);
    }
}
