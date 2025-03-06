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

        public ObservableCollection<(double, double)> InnerInterpolated { get; set; } = new();
        public ObservableCollection<(double, double)> OuterInterpolated { get; set; } = new();

        public string InnerPath
        {
            get => _innerPath;
            set { _innerPath = value; OnPropertyChanged(); }
        }

        public string OuterPath
        {
            get => _outerPath;
            set { _outerPath = value; OnPropertyChanged(); }
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged();
            }
        }

        public void LoadFiles()
        {
            OpenFileDialog openFileDialog = new() { Multiselect = true, Filter = "Text files (*.txt)|*.txt" };
            if (openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length == 2)
            {
                InnerPath = openFileDialog.FileNames[0];
                OuterPath = openFileDialog.FileNames[1];
            }
        }

        public void Interpolate()
        {
            var innerCoords = TrackInterpolator.LoadCoordinates(InnerPath);
            var outerCoords = TrackInterpolator.LoadCoordinates(OuterPath);

            var innerInterp = TrackInterpolator.InterpolateLinear(innerCoords);
            var outerInterp = TrackInterpolator.InterpolateLinear(outerCoords);

           // TODO: Plot
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
