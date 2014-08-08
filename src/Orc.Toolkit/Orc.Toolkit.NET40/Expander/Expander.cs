using System.Windows;

namespace Orc.Toolkit
{
    public enum ExpandDirection
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3,
    }

    public class Expander : HeaderedContentControl
    {
        private System.Windows.GridLength previousValue;

        public Expander()
        {
            this.DefaultStyleKey = typeof(Expander);
        }


        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Expander), new PropertyMetadata(false,
                OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Expander expander = d as Expander;
            if ((bool)e.NewValue == true)
                expander.OnExpanded();
            else
                expander.OnCollapsed();
        }
        

        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register("ExpandDirection", typeof(ExpandDirection), typeof(Expander), new PropertyMetadata(ExpandDirection.Left));
               

        public bool AutoResizeGrid
        {
            get { return (bool)GetValue(AutoResizeGridProperty); }
            set { SetValue(AutoResizeGridProperty, value); }
        }
        public static readonly DependencyProperty AutoResizeGridProperty =
            DependencyProperty.Register("AutoResizeGrid", typeof(bool), typeof(Expander), new PropertyMetadata(false));


        protected virtual void OnCollapsed()
        {
            if (!AutoResizeGrid) return;

            if (this.Parent is System.Windows.Controls.Grid)
            {
                System.Windows.Controls.Grid grid = this.Parent as System.Windows.Controls.Grid;
                switch (this.ExpandDirection)
                {
                    case ExpandDirection.Left:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            previousValue = grid.ColumnDefinitions[column].Width;
                            grid.ColumnDefinitions[column].Width = System.Windows.GridLength.Auto;
                            break;
                        }
                    case ExpandDirection.Right:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            previousValue = grid.ColumnDefinitions[column].Width;
                            grid.ColumnDefinitions[column].Width = System.Windows.GridLength.Auto;
                            break;
                        }
                    case ExpandDirection.Up:
                        {
                            int row = System.Windows.Controls.Grid.GetRow(this);
                            previousValue = grid.RowDefinitions[row].Height;
                            grid.RowDefinitions[row].Height = System.Windows.GridLength.Auto;
                            break;
                        }
                    case ExpandDirection.Down:
                        {
                            int row = System.Windows.Controls.Grid.GetRow(this);
                            previousValue = grid.RowDefinitions[row].Height;
                            grid.RowDefinitions[row].Height = System.Windows.GridLength.Auto;
                            break;
                        }
                }
            }
        }

        protected virtual void OnExpanded()
        {
            if (!AutoResizeGrid) return;

            if (this.Parent is System.Windows.Controls.Grid)
            {
                System.Windows.Controls.Grid grid = this.Parent as System.Windows.Controls.Grid;

                switch (this.ExpandDirection)
                {
                    case ExpandDirection.Left:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            if (previousValue != null)
                            {
                                grid.ColumnDefinitions[column].Width = previousValue;
                            }
                            break;
                        }
                    case ExpandDirection.Right:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            if (previousValue != null)
                            {
                                grid.ColumnDefinitions[column].Width = previousValue;
                            }
                            break;
                        }
                    case ExpandDirection.Up:
                        {
                            int row = System.Windows.Controls.Grid.GetRow(this);
                            if (previousValue != null)
                            {
                                grid.RowDefinitions[row].Height = previousValue;
                            }
                            break;
                        }
                    case ExpandDirection.Down:
                        {
                            int row = System.Windows.Controls.Grid.GetRow(this);
                            if (previousValue != null)
                            {
                                grid.RowDefinitions[row].Height = previousValue;
                            }
                            break;
                        }
                }
            }
        }
    }
}
