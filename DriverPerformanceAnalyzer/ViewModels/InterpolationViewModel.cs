using OxyPlot;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using DriverPerformanceAnalyzer.Models;

namespace DriverPerformanceAnalyzer.ViewModels
{
    public class InterpolationViewModel : INotifyPropertyChanged
    {
        private string _innerPath;
        private string _outerPath;
        private PlotModel _plotModel;

        /// <summary>
        /// Gets or sets the collection of interpolated points for the inner path.
        /// </summary>
        public ObservableCollection<(double, double)> InnerInterpolated { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of interpolated points for the outer path.
        /// </summary>
        public ObservableCollection<(double, double)> OuterInterpolated { get; set; } = new();

        /// <summary>
        /// Gets or sets the file path for the inner path coordinates.
        /// </summary>
        public string InnerPath
        {
            get => _innerPath;
            set { _innerPath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the file path for the outer path coordinates.
        /// </summary>
        public string OuterPath
        {
            get => _outerPath;
            set { _outerPath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the plot model used for rendering the chart.
        /// </summary>
        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Opens a file dialog to select the files containing the coordinates for the inner and outer paths.
        /// </summary>
        public void LoadFiles()
        {
            OpenFileDialog openFileDialog = new() { Multiselect = true, Filter = "Text files (*.txt)|*.txt" };
            if (openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length == 2)
            {
                InnerPath = openFileDialog.FileNames[0];
                OuterPath = openFileDialog.FileNames[1];
            }
        }

        /// <summary>
        /// Loads coordinates from the selected files, performs cubic spline interpolation, 
        /// and updates the list of interpolated points.
        /// </summary>
        public void Interpolate()
        {
            var (longitudesInnerPath, latitudesInnerPath) = TrackInterpolator.LoadCoordinates(InnerPath);
            var (longitudesOuterPath, latitudesOuterPath) = TrackInterpolator.LoadCoordinates(OuterPath);
            var innerInterp = TrackInterpolator.InterpolateCubicSpline(longitudesInnerPath, latitudesInnerPath);
            var outerInterp = TrackInterpolator.InterpolateCubicSpline(longitudesOuterPath, latitudesOuterPath);

            InnerInterpolated.Clear();
            OuterInterpolated.Clear();

            // Add interpolated points to the respective collections
            foreach (var point in innerInterp)
            {
                InnerInterpolated.Add(point);
            }

            foreach (var point in outerInterp)
            {
                OuterInterpolated.Add(point);
            }

            // Create the plot model for rendering the paths
            CreatePlotModel(innerInterp, outerInterp);
        }

        /// <summary>
        /// Creates the plot model that will render the race track with inner and outer paths.
        /// </summary>
        /// <param name="innerPath">The list of interpolated points for the inner path.</param>
        /// <param name="outerPath">The list of interpolated points for the outer path.</param>
        private void CreatePlotModel(List<(double, double)> innerPath, List<(double, double)> outerPath)
        {
            var plotModel = new PlotModel { Title = "Race track", PlotType = PlotType.Cartesian };

            // Add X and Y axes to the plot
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                MajorGridlineStyle = OxyPlot.LineStyle.Solid,
                MinorGridlineStyle = OxyPlot.LineStyle.Dot
            });

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                MajorGridlineStyle = OxyPlot.LineStyle.Solid,
                MinorGridlineStyle = OxyPlot.LineStyle.Dot
            });

            // Create series for the inner and outer paths
            var innerSeries = new LineSeries
            {
                Title = "Inner path",
                Color = OxyPlot.OxyColors.Blue,
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };

            var outerSeries = new LineSeries
            {
                Title = "Outer path",
                Color = OxyPlot.OxyColors.Red,
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };

            // Add data points to each series
            foreach (var point in innerPath)
            {
                innerSeries.Points.Add(new OxyPlot.DataPoint(point.Item1, point.Item2));
            }

            foreach (var point in outerPath)
            {
                outerSeries.Points.Add(new OxyPlot.DataPoint(point.Item1, point.Item2));
            }

            // Add the series to the plot
            plotModel.Series.Add(innerSeries);
            plotModel.Series.Add(outerSeries);

            plotModel.PlotType = PlotType.XY;

            // Determine the plot limits based on the data points
            double minX = Math.Min(innerPath.Min(p => p.Item1), outerPath.Min(p => p.Item1));
            double maxX = Math.Max(innerPath.Max(p => p.Item1), outerPath.Max(p => p.Item1));
            double minY = Math.Min(innerPath.Min(p => p.Item2), outerPath.Min(p => p.Item2));
            double maxY = Math.Max(innerPath.Max(p => p.Item2), outerPath.Max(p => p.Item2));

            double marginX = (maxX - minX) * 0.05;
            double marginY = (maxY - minY) * 0.05;

            // Set axis limits with a margin
            plotModel.Axes[0].Minimum = minX - marginX;
            plotModel.Axes[0].Maximum = maxX + marginX;
            plotModel.Axes[1].Minimum = minY - marginY;
            plotModel.Axes[1].Maximum = maxY + marginY;

            // Set the plot model for display
            PlotModel = plotModel;
        }

        /// <summary>
        /// Event raised when a property changes in the ViewModel.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies that a property has changed, enabling data binding updates.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed (optional).</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
