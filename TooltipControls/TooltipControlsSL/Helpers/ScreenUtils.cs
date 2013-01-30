// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenUtils.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The screen utilities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.TooltipControls.Helpers
{
    using System.Windows;
    
#if (!SILVERLIGHT)
    using System.Windows.Interop;
    using System.Windows.Media;
#endif

    /// <summary>
    /// The screen utilities.
    /// </summary>
    public static class ScreenUtils
    {
        /// <summary>
        /// The get window size.
        /// </summary>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        public static Size GetWindowSize()
        {
#if (SILVERLIGHT)
            {
                return Application.Current.IsRunningOutOfBrowser ?
                    new Size(Application.Current.MainWindow.Width, Application.Current.MainWindow.Height) :
                    new Size(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
            }
#else
            {
                return BrowserInteropHelper.IsBrowserHosted
                           ? (BrowserInteropHelper.HostScript != null
                                  ? new Size(
                                        BrowserInteropHelper.HostScript.innerWidth, 
                                        BrowserInteropHelper.HostScript.innerHeight)
                                  : new Size())
                           : new Size(
                                 Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight);
            }

#endif
        }

#if (!SILVERLIGHT)

        /// <summary>
        /// The is parent of.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsParentOf(DependencyObject parent, DependencyObject child)
        {
            if (parent == null || child == null)
            {
                return false;
            }

            DependencyObject element;
            do
            {
                if (child is Visual)
                {
                    element = VisualTreeHelper.GetParent(child);
                }
                else
                {
                    element = LogicalTreeHelper.GetParent(child);
                }

                if (ReferenceEquals(element, parent))
                {
                    return true;
                }

                child = element;
            }
            while (element != null);

            return false;
        }

#endif
    }
}