// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenUtils.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.TooltipControls.Helpers
{
    using System.Windows;
#if (!SILVERLIGHT)
    using System.Windows.Interop;
    using System.Windows.Media;
#endif

    public static class ScreenUtils
    {
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
                return BrowserInteropHelper.IsBrowserHosted ?
                    (BrowserInteropHelper.HostScript != null ? new Size(BrowserInteropHelper.HostScript.innerWidth, BrowserInteropHelper.HostScript.innerHeight) : new Size()) :
                    new Size(Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight);
            }
#endif
        }

#if (!SILVERLIGHT)
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
