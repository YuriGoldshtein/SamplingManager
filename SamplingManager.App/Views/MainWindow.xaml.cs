using SamplingManager.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamplingManager.App.Views
{
    internal interface IMainWindow
    {
        void Show();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow(IMainViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

            ((INotifyCollectionChanged)ExperimentDataGrid.Items).CollectionChanged += ExperimentDataGrid_CollectionChanged;
        }

        private void ExperimentDataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ExperimentDataGrid.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(ExperimentDataGrid, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }
    }
}
