// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderedContentControl.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The headered content control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The headered content control.
    /// </summary>
    public class HeaderedContentControl : ContentControl
    {
        #region Static Fields

        /// <summary>
        /// The header property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(HeaderedContentControl), new PropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContentControl"/> class.
        /// </summary>
        public HeaderedContentControl()
        {
            this.DefaultStyleKey = typeof(HeaderedContentControl);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        [System.ComponentModel.Bindable(true)]
        public object Header
        {
            get
            {
                return (object)this.GetValue(HeaderProperty);
            }

            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        #endregion
        

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedContentControl), new PropertyMetadata(null));


    }
}