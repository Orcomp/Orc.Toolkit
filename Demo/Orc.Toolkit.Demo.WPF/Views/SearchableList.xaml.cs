using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orc.Toolkit.Demo.Views
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interaction logic for SearchableList.xaml
    /// </summary>
    public partial class SearchableList : UserControl
    {
        public SearchableList()
        {
            InitializeComponent();
            this.FillList();
        }

        private void FillList()
        {
            var list = new ObservableCollection<string>() { "London", "Paris", "San Francisco", "Sydney", "Moscow" };
            this.searchableListBox.ItemsSource = list;
        }
    }
}
