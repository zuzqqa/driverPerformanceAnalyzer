using System.Configuration;
using System.Data;
using System.Windows;

namespace DriverPerformanceAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Importowanie funkcji do alokowania konsoli
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
    }

}
