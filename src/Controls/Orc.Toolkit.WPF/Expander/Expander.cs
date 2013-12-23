using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orc.Toolkit
{
    

    public class Expander : System.Windows.Controls.Expander
    {
        private System.Windows.GridLength previousValue;

        public bool AutoResizeGrid
        {
            get { return (bool)GetValue(AutoResizeGridProperty); }
            set { SetValue(AutoResizeGridProperty, value); }
        }
        public static readonly DependencyProperty AutoResizeGridProperty =
            DependencyProperty.Register("AutoResizeGrid", typeof(bool), typeof(Expander), new PropertyMetadata(false));
        
        protected override void OnCollapsed()
        {
            base.OnCollapsed();

            if (!AutoResizeGrid) return;

            if (this.Parent is System.Windows.Controls.Grid)
            {
                System.Windows.Controls.Grid grid = this.Parent as System.Windows.Controls.Grid;
                switch (this.ExpandDirection)
                {
                    case System.Windows.Controls.ExpandDirection.Left:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            previousValue = grid.ColumnDefinitions[column].Width;
                            grid.ColumnDefinitions[column].Width = System.Windows.GridLength.Auto;
                            break;
                        }
                    case System.Windows.Controls.ExpandDirection.Right:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            previousValue = grid.ColumnDefinitions[column].Width;
                            grid.ColumnDefinitions[column].Width = System.Windows.GridLength.Auto;
                            break;
                        }
                    case System.Windows.Controls.ExpandDirection.Up:
                        {
                            int row = System.Windows.Controls.Grid.GetRow(this);
                            previousValue = grid.RowDefinitions[row].Height;
                            grid.RowDefinitions[row].Height = System.Windows.GridLength.Auto;
                            break;
                        }
                    case System.Windows.Controls.ExpandDirection.Down:
                        {
                            int row = System.Windows.Controls.Grid.GetRow(this);
                            previousValue = grid.RowDefinitions[row].Height;
                            grid.RowDefinitions[row].Height = System.Windows.GridLength.Auto;
                            break;
                        }
                }
            }
        }

        protected override void OnExpanded()
        {
            base.OnExpanded();

            if (!AutoResizeGrid) return;

            if (this.Parent is System.Windows.Controls.Grid)
            {
                System.Windows.Controls.Grid grid = this.Parent as System.Windows.Controls.Grid;

                switch (this.ExpandDirection)
                {
                    case System.Windows.Controls.ExpandDirection.Left:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            if (previousValue != null)
                            {
                                grid.ColumnDefinitions[column].Width = previousValue;
                            }
                            break;
                        }
                    case System.Windows.Controls.ExpandDirection.Right:
                        {
                            int column = System.Windows.Controls.Grid.GetColumn(this);
                            if (previousValue != null)
                            {
                                grid.ColumnDefinitions[column].Width = previousValue;
                            }
                            break;
                        }
                    case System.Windows.Controls.ExpandDirection.Up:
                        {
                            int row = System.Windows.Controls.Grid.GetRow(this);
                            if (previousValue != null)
                            {
                                grid.RowDefinitions[row].Height = previousValue;
                            }
                            break;
                        }
                    case System.Windows.Controls.ExpandDirection.Down:
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
