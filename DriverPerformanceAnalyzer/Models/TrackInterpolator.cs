using System.Globalization;
using System.IO;
using MathNet.Numerics.Interpolation;

namespace DriverPerformanceAnalyzer.Models
{
    public class TrackInterpolator
    {
        /// <summary>
        /// Reads a file containing geographic coordinates and parses them into a list of tuples.
        /// </summary>
        /// <param name="filePath">The path to the file containing coordinates.</param>
        /// <returns>A list of tuples where each tuple contains longitude and latitude.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        public static (double[] longitudes, double[] latitudes) LoadCoordinates(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File {filePath} does not exist!");

            var longitudes = new List<double>();
            var latitudes = new List<double>();

            try
            {
                string content = File.ReadAllText(filePath);

                // Splitting file content into individual coordinate entries
                string[] entries = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (string entry in entries)
                {
                    string[] parts = entry.Split(',');

                    // Parsing longitude and latitude with invariant culture 
                    double longitude = double.Parse(parts[0], CultureInfo.InvariantCulture);
                    double latitude = double.Parse(parts[1], CultureInfo.InvariantCulture);

                    longitudes.Add(longitude);
                    latitudes.Add(latitude);
                }
            } catch(Exception ex) 
            {
                Console.WriteLine($"Error reading coordinates: {ex.Message}");
            }

            return (longitudes.ToArray(), latitudes.ToArray());
        }

        /// <summary>
        /// Interpolates a set of longitude and latitude points using cubic spline interpolation.
        /// </summary>
        /// <param name="longitudes">An array of longitude values.</param>
        /// <param name="latitudes">An array of latitude values.</param>
        /// <param name="numPoints">The number of points to generate in the interpolated path (default is 10000).</param>
        /// <returns>
        /// A list of tuples, where each tuple contains the interpolated longitude and latitude values.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if there are fewer than two points in the provided arrays, or if the number of longitudes and latitudes do not match.
        /// </exception>
        public static List<(double, double)> InterpolateCubicSpline(double[] longitudes, double[] latitudes, int numPoints = 10000)
        {
            // Check if there are enough points for interpolation
            if (longitudes.Length < 2 || latitudes.Length < 2)
                throw new ArgumentException("At least two points are required for interpolation.");

            // Check if the number of longitudes and latitudes are the same
            if (longitudes.Length != latitudes.Length)
                throw new ArgumentException("The number of longitude and latitude points must be the same.");

            // Array to store the parameter t for each point, representing cumulative distance
            double[] t = new double[longitudes.Length];
            t[0] = 0.0;

            // Calculate the cumulative distance for each point along the path
            for (int i = 1; i < longitudes.Length; i++)
            {
                double dx = longitudes[i] - longitudes[i - 1];
                double dy = latitudes[i] - latitudes[i - 1];
                t[i] = t[i - 1] + Math.Sqrt(dx * dx + dy * dy);
            }

            // Normalize t values to the range [0, 1]
            double tMax = t[t.Length - 1];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] /= tMax;
            }

            // Interpolate the longitudes and latitudes using Akima cubic spline
            var longitudeInterpolator = CubicSpline.InterpolateAkimaSorted(t, longitudes);
            var latitudeInterpolator = CubicSpline.InterpolateAkimaSorted(t, latitudes);

            // List to store the interpolated points
            List<(double, double)> interpolatedPoints = new List<(double, double)>();

            // Step size for interpolation
            double step = 1.0 / (numPoints - 1);

            // Generate the interpolated points
            for (int i = 0; i < numPoints; i++)
            {
                double parameter = i * step;
                parameter = Math.Min(1.0, parameter);

                double interpolatedLongitude = longitudeInterpolator.Interpolate(parameter);
                double interpolatedLatitude = latitudeInterpolator.Interpolate(parameter);

                interpolatedPoints.Add((interpolatedLongitude, interpolatedLatitude));
            }

            // If the first and last points are nearly the same, close the loop
            if (Math.Abs(longitudes[0] - longitudes[longitudes.Length - 1]) < 1e-6 &&
                Math.Abs(latitudes[0] - latitudes[latitudes.Length - 1]) < 1e-6)
            {
                interpolatedPoints[interpolatedPoints.Count - 1] = interpolatedPoints[0];
            }

            // Return the list of interpolated points
            return interpolatedPoints;
        }
    }
}
