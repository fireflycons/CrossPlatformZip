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
            var path = file.FullName;

            if (path.Contains(" "))
            {
                path = $"\"{path}\"";
            }

            var startInfo = new ProcessStartInfo
                                {
                                    FileName = "ls",
                                    Arguments = $"-lnd {path}",
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data) && PosixPermissionsParser.FileListOutputRegex.IsMatch(e.Data))
                        {
                            output = e.Data;
                        }
                    };

                // Seems ls can also write the result to the error stream
                process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data) && PosixPermissionsParser.FileListOutputRegex.IsMatch(e.Data))
                        {
                            output = e.Data;
                        }
                    };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                return process.ExitCode != 0 ? null : output;
            }
        }
    }
}