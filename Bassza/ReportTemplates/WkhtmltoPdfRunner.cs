using System.Diagnostics;

namespace Bassza.ReportTemplates
{
    public static class WkhtmltoPdfRunner
    {
        private static readonly string DefaultArguments = "--page-size A4 ";
        
        public static void Run(string wkhtmltopdfPath, string arguments)
        {
            
            // Create a new ProcessStartInfo instance
            var startInfo = new ProcessStartInfo
            {
                // Set the file name to the path of the wkhtmltopdf binary
                FileName = wkhtmltopdfPath,

                // Set the arguments for wkhtmltopdf
                Arguments = DefaultArguments + arguments,

                // Set the UseShellExecute property to false
                // This is required to redirect the output of the process
                UseShellExecute = false,

                // Set the RedirectStandardOutput property to true
                // This is required to redirect the output of the process
                RedirectStandardOutput = true
            };

            // Start the process
            using var process = Process.Start(startInfo);
            // Read the output of the process
            var output = process?.StandardOutput.ReadToEnd();

            // Wait for the process to exit
            process?.WaitForExit();

            // Check the exit code of the process
            // A value of 0 indicates that the process completed successfully
            if (process is {ExitCode: 0})
            {
                Console.WriteLine("Success!");
            }
            else
            {
                if (process != null) Console.WriteLine("Error running wkhtmltopdf. Exit code: " + process.ExitCode);
                Console.WriteLine(output);
            }
        }
    }
}