/************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2010-2012 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus edition at http://xceed.com/wpf_toolkit

   Visit http://xceed.com and follow @datagrid on Twitter

  **********************************************************************/

namespace Orc.Toolkit
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class WatermarkTextBox : TextBox
    {
        #region Properties

        #region SelectAllOnGotFocus

        public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.Register("SelectAllOnGotFocus", typeof(bool), typeof(WatermarkTextBox), new PropertyMetadata(false));
        public bool SelectAllOnGotFocus
        {
            get
            {
                return (bool)GetValue(SelectAllOnGotFocusProperty);
            }
            set
            {
                SetValue(SelectAllOnGotFocusProperty, value);
            }
        }

        #endregion //SelectAllOnGotFocus

        #region Watermark

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkTextBox), new PropertyMetadata(null));
        public object Watermark
        {
            get
            {
                return (object)GetValue(WatermarkProperty);
            }
            set
            {
                SetValue(WatermarkProperty, value);
            }
        }

        #endregion //Watermark

        #region WatermarkTemplate

        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkTextBox), new PropertyMetadata(null));
        public DataTemplate WatermarkTemplate
        {
            get
            {
                return (DataTemplate)GetValue(WatermarkTemplateProperty);
            }
            set
            {
                SetValue(WatermarkTemplateProperty, value);
            }
        }

        #endregion //WatermarkTemplate

        #endregion //Properties

        #region Constructors

        static WatermarkTextBox()
        {
            #if (!SILVERLIGHT)
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
            #endif
        }

        #if (SILVERLIGHT)
        public WatermarkTextBox()
            : base()
        {
            this.TextChanged += (sender, args) =>
                { this.GetBindingExpression(TextBox.TextProperty).UpdateSource(); };
        }
        #endif
        #endregion //Constructors

        #region Base Class Overrides

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (SelectAllOnGotFocus)
                SelectAll();
        }

        #if (!SILVERLIGHT)
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocused && SelectAllOnGotFocus)
            {
                e.Handled = true;
                Focus();
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }
        #endif

        #endregion //Base Class Overrides
    }
}
