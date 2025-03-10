using System.Windows;
using DriverPerformanceAnalyzer.ViewModels;

namespace DriverPerformanceAnalyzer.Views
{
    public partial class MainWindow : Window
    {
        private readonly InterpolationViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new InterpolationViewModel();
            DataContext = _viewModel;

            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Top;

            this.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Handles the event when the "Choose Files" button is clicked.
        /// Opens the file dialog to select the coordinate files for interpolation.
        /// </summary>
        private void ChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadFiles();
        }

        /// <summary>
        /// Handles the event when the "Interpolate" button is clicked.
        /// Executes the interpolation of the inner and outer paths and updates the UI visibility.
        /// </summary>
        private void Interpolate_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Interpolate();

            // Hide the file selection and interpolation controls after interpolation starts
            InnerFileLabel.Visibility = Visibility.Collapsed;
            InnerFileTextBox.Visibility = Visibility.Collapsed;
            OuterFileLabel.Visibility = Visibility.Collapsed;
            OuterFileTextBox.Visibility = Visibility.Collapsed;
            ChooseFilesButton.Visibility = Visibility.Collapsed;
            InterpolateButton.Visibility = Visibility.Collapsed;

            // Display the plot view
            PlotView.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the event when the "Interpolate Track" button is clicked.
        /// Displays the file selection and interpolation controls again.
        /// </summary>
        private void InterpolateTrack_Click(object sender, RoutedEventArgs e)
        {
            // Show the file selection and interpolation controls
            InnerFileLabel.Visibility = Visibility.Visible;
            InnerFileTextBox.Visibility = Visibility.Visible;
            OuterFileLabel.Visibility = Visibility.Visible;
            OuterFileTextBox.Visibility = Visibility.Visible;
            ChooseFilesButton.Visibility = Visibility.Visible;
            InterpolateButton.Visibility = Visibility.Visible;

            // Hide the plot view
            PlotView.Visibility = Visibility.Collapsed;
        }
    }
}
