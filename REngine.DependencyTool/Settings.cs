namespace REngine.DependencyTool;

public class AndroidLibs
{
    public Dictionary<string, string> Arm64_V8A { get; set; } = new();
    public Dictionary<string, string> Armeabi_V7A { get; set; } = new();
    public Dictionary<string, string> X86_64 { get; set; } = new();
    public Dictionary<string, string> X86 { get; set; } = new();
}
public class Settings
{
    public Dictionary<string, string> Windows { get; set; } = new();
    public Dictionary<string, string> Linux { get; set; } = new();
    public Dictionary<string, string> MacOS { get; set; } = new();
    public AndroidLibs Android { get; set; } = new();
}