// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using REngine.DependencyTool;

Parameters parameters = new();
parameters.Collect(args);

Console.WriteLine("Downloading REngine Dependencies");

var outputPath = parameters.GetParam("-o") ?? parameters.GetParam("--output");
if (string.IsNullOrEmpty(outputPath))
    throw new Exception("Output parameter is required. Ex: '--output DOWNLOAD_PATH' or '-o DOWNLOAD_PATH'");
outputPath = Path.GetFullPath(outputPath);

var settingsData = File.ReadAllText(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "settings.json"));
var settings = JsonSerializer.Deserialize<Settings>(settingsData);
if (settings is null)
    throw new NullReferenceException("Error has occurred while is deserializing settings json");

void DownloadDependencies(string depOutputPath, Dictionary<string, string> dependencies)
{
    if (!Directory.Exists(depOutputPath))
        Directory.CreateDirectory(depOutputPath);
    
    using var client = new HttpClient();
    foreach (var pair in dependencies)
    {
        var downloadPath = Path.Join(depOutputPath, pair.Key);
        if (File.Exists(downloadPath))
        {
            Console.WriteLine($"File '{pair.Key}' has already downloaded. Skipping!");
            continue;
        }
        
        Console.WriteLine($"Downloading: {pair.Key}");
        Task.Run(async () =>
        {
            var res = await client.GetAsync(pair.Value);
            res.EnsureSuccessStatusCode();

            var data = await res.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(Path.Join(depOutputPath, pair.Key), data);
        }).Wait();
        Console.WriteLine($"Downloaded: {pair.Key}");
    }
}

void DownloadAndroidBinaries(string depOutputPath, AndroidLibs libs)
{
    Console.WriteLine("- Downloading Arm64V8A Binaries");
    DownloadDependencies(Path.Join(depOutputPath, "arm64-v8a"), libs.Arm64_V8A);
    Console.WriteLine("- Downloading Armeabi-V7A Binaries");
    DownloadDependencies(Path.Join(depOutputPath, "armeabi-v7a"), libs.Armeabi_V7A);
    Console.WriteLine("- Downloading X86 Binaries");
    DownloadDependencies(Path.Join(depOutputPath, "x86"), libs.X86);
    Console.WriteLine("- Downloading X86_64 Binaries");
    DownloadDependencies(Path.Join(depOutputPath, "x86_64"), libs.X86);
}

Console.WriteLine("Downloading Windows Binaries");
DownloadDependencies(Path.Join(outputPath, "runtimes/win-x64/native"), settings.Windows);
Console.WriteLine("Downloading Linux Binaries");
DownloadDependencies(Path.Join(outputPath, "runtimes/linux-x64/native"), settings.Linux);
Console.WriteLine("Downloading MacOS Binaries");
DownloadDependencies(Path.Join(outputPath, "runtimes/osx-x64/native"), settings.MacOS);
Console.WriteLine("Downloading Android Binaries");
DownloadAndroidBinaries(Path.Join(outputPath, "runtimes/android"), settings.Android);

Console.WriteLine("Finished!");