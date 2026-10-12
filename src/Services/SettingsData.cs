using System.Text.Json;
using System.Text.Json.Serialization;

namespace GTDialogueViewer.Services;

public static class SettingsData
{
    public static string SpeakerIconsFilePath { get; set; } = string.Empty;
    public static SpeakerIconsRoot? IconDBData { get; set; } = new();

    public static void SetSpeakerIconsFilePath(string path, string jsonContent)
    {
        SpeakerIconsFilePath = path;
        Console.WriteLine($"{SpeakerIconsFilePath}");
        IconDBData = JsonSerializer.Deserialize<SpeakerIconsRoot>(jsonContent);
    }
}

// Supporting model classes for JSON deserialization
public class SpeakerIconsRoot
{
    [JsonPropertyName("Icons")]
    public Dictionary<string, string> Icons { get; set; } = new();
}