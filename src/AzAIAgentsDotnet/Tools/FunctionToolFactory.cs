using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace AzAIAgentsDotnet.Tools;

internal static class FunctionToolFactory
{
    internal static string GetCpuUsage()
    {
        // Get CPU model
        string cpuModel = GetCpuModel();

        // Get average CPU speed in GHz
        double averageCpuSpeed = GetAverageCpuSpeed();

        // Get CPU usage percentage
        double cpuUsage = GetCpuUsagePercentage();

        return $"CPU Usage: {cpuModel} {Math.Floor(averageCpuSpeed)}GHz {cpuUsage}%";
    }

    private static string GetCpuModel()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows implementation
            using var searcher = new ManagementObjectSearcher("select Name from Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                return obj["Name"]?.ToString() ?? "Unknown CPU";
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux implementation
            try
            {
                var cpuInfo = File.ReadAllLines("/proc/cpuinfo");
                var modelNameLine = cpuInfo.FirstOrDefault(line => line.StartsWith("model name"));
                if (modelNameLine != null)
                {
                    return modelNameLine.Split(':')[1].Trim();
                }
            }
            catch
            {
                // Fallback for errors
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS implementation
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"sysctl -n machdep.cpu.brand_string\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    if (!string.IsNullOrEmpty(output))
                    {
                        return output;
                    }
                }
            }
            catch
            {
                // Fallback for errors
            }
        }

        return "Unknown CPU";
    }

    private static double GetAverageCpuSpeed()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows implementation
            using var searcher = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor");
            var speeds = searcher.Get()
                .Cast<ManagementObject>()
                .Select(obj => Convert.ToDouble(obj["MaxClockSpeed"]))
                .ToList();

            return speeds.Count != 0 ? speeds.Average() / 1000.0 : 0.0; // Convert MHz to GHz
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux implementation
            try
            {
                var cpuInfo = File.ReadAllLines("/proc/cpuinfo");
                var cpuMHzLines = cpuInfo.Where(line => line.StartsWith("cpu MHz"));
                var speeds = cpuMHzLines
                    .Select(line => double.Parse(line.Split(':')[1].Trim()))
                    .ToList();

                return speeds.Count != 0 ? speeds.Average() / 1000.0 : 0.0; // Convert MHz to GHz
            }
            catch
            {
                // Fallback for errors
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS implementation
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"sysctl -n hw.cpufrequency\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    if (long.TryParse(output, out long frequency))
                    {
                        return frequency / 1_000_000_000.0; // Convert Hz to GHz
                    }
                }
            }
            catch
            {
                // Fallback for errors
            }
        }

        return 0.0;
    }

    private static double GetCpuUsagePercentage()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows implementation
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // First call returns 0, so we need to call it twice
            Thread.Sleep(500); // Wait for a short interval to get a valid reading
            return Math.Round(cpuCounter.NextValue(), 2);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux implementation
            try
            {
                var cpuStatLines = File.ReadAllLines("/proc/stat");
                var cpuLine = cpuStatLines.FirstOrDefault(line => line.StartsWith("cpu "));
                if (cpuLine != null)
                {
                    var values = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Skip(1)
                        .Select(long.Parse)
                        .ToArray();

                    long idle = values[3];
                    long total = values.Sum();

                    Thread.Sleep(500); // Wait for a short interval

                    cpuStatLines = File.ReadAllLines("/proc/stat");
                    cpuLine = cpuStatLines.FirstOrDefault(line => line.StartsWith("cpu "));
                    if (cpuLine != null)
                    {
                        values = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Skip(1)
                            .Select(long.Parse)
                            .ToArray();

                        long idle2 = values[3];
                        long total2 = values.Sum();

                        long idleDelta = idle2 - idle;
                        long totalDelta = total2 - total;

                        double cpuUsage = 100.0 * (1.0 - (double)idleDelta / totalDelta);
                        return Math.Round(cpuUsage, 2);
                    }
                }
            }
            catch
            {
                // Fallback for errors
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS implementation
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"top -l 1 | grep -E '^CPU' | awk '{print $3 + $5}'\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    if (double.TryParse(output, out double cpuUsage))
                    {
                        return Math.Round(cpuUsage, 2);
                    }
                }
            }
            catch
            {
                // Fallback for errors
            }
        }

        return 0.0;
    }
}