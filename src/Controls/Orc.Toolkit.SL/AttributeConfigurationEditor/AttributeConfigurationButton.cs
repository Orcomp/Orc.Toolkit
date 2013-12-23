using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Orc.Toolkit
{
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    public class AttributeConfigurationButton : HeaderedContentControl
    {
        public AttributeConfigurationButton()
        {
            DefaultStyleKey = typeof(AttributeConfigurationButton);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, "MouseOver", false);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, "Normal", false);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            VisualStateManager.GoToState(this, "Pressed", false);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            VisualStateManager.GoToState(this, "Normal", false);
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(AttributeConfigurationButton), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChange)));

        private static void OnIsSelectedPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var attributeConfigurationButton = d as AttributeConfigurationButton;
            if (attributeConfigurationButton != null)
            {
                if ((bool)e.NewValue == true)
                {
                    VisualStateManager.GoToState(attributeConfigurationButton, "Selected", false);
                }
                else
                {
                    VisualStateManager.GoToState(attributeConfigurationButton, "Unselected", false);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselected", true);
            }
        }
    }
}
