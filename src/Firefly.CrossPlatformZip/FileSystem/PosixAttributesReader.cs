namespace Firefly.CrossPlatformZip.FileSystem
{
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Concrete implementation of <see cref="IPosixAttributesReader"/>
    /// </summary>
    internal class PosixAttributesReader : IPosixAttributesReader
    {
        /// <inheritdoc />
        public string InvokeLs(FileSystemInfo file)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Can't run this on Windows
                return null;
            }

            string output = null;

            // Escape spaces in paths
            var path = file.FullName.Replace(" ", @"\ ");

            var startInfo = new ProcessStartInfo
                                {
                                    FileName = "ls",
                                    Arguments = $"-ln {path}",
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            output = e.Data;
                        }
                    };

                process.Start();

                // Asynchronously read the standard output of the spawned process.
                // This raises OutputDataReceived events for each line of output.
                process.BeginOutputReadLine();
                process.WaitForExit();

                return process.ExitCode != 0 ? null : output;
            }
        }
    }
}