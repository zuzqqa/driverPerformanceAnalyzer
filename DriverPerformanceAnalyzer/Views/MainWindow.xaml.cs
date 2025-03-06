using System.Windows;
using DriverPerformanceAnalyzer.ViewModels;

namespace DriverPerformanceAnalyzer.Views
{
    public partial class MainWindow : Window
    {
        private readonly InterpolationViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new InterpolationViewModel();
            DataContext = _viewModel;
        }

        private void ChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadFiles();
        }

        private void Interpolate_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Interpolate();
        }
    }
}
