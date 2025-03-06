using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.Interpolation;

namespace DriverPerformanceAnalyzer.Models
{
    public class TrackInterpolator
    {
        public static List<(double, double)> LoadCoordinates(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File {filePath} does not exist!");

            var coordinates = new List<(double, double)>();

            foreach (var line in File.ReadLines(filePath))
            {
                Console.WriteLine(line);
                
                // TODO: split a single line
            }

            return coordinates;
        }

        public static List<(double, double)> InterpolateLinear(List<(double, double)> coords, int numPoints = 10000)
        {
            // TODO: Interpolate coordinates
        }
    }
}
