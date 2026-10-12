using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.Diagrams.Core.Models;
using GTDialogueViewer.Services;
using Point = Blazor.Diagrams.Core.Geometry.Point;

namespace GTDialogueViewer.Nodes;

public struct FText(string sourceString, string key, string table)
{
    public string SourceString { get; set; } = sourceString;
    public string Key { get; set; } = key;
    public string Table { get; set; } = table;

    public string GetText() => SourceString;

    public int GetPixelLength()
    {
        return GetText().Length * 9;
    }

    public bool IsLocalizable
    {
        get => !string.IsNullOrEmpty(Table) && !string.IsNullOrEmpty(Key);
    }
}

public class GTDialogueNodeModel(Point? position = null) : NodeModel(position)
{
    public Color AccentNodeColor { get; set; }
    public FText NodeTitle { get; set; }
    public FText NodeDescription { get; set; }
    public FText[] NodeOutputNames { get; set; } = [];
    public string IconPath { get; set; } = "";

    public static int MinWidth = 180;
    public static int MaxWidth = 400;

    public int GetNodeLength()
    {
        int length = int.Max(NodeTitle.GetPixelLength(), MinWidth);
        for (int i = 0; i < NodeOutputNames.Length; i++)
        {
            length = Math.Max(length, NodeOutputNames[i].GetPixelLength());
        }
        length = int.Min(length, MaxWidth);
        return length;
    }

    public string GetFullIconPath()
    {
        try
        {
            if (SettingsData.IconDBData == null)
            {
                Console.WriteLine("no db...");
                return string.Empty;
            }

            if (SettingsData.IconDBData?.Icons != null && SettingsData.IconDBData.Icons.TryGetValue(IconPath, out var relativeIconSubPath))
            {
                if (relativeIconSubPath != null)
                {
                    // Manually subtract the file name from the .gtspeakericons path to get its directory, then combine with the relative sub-path
                    int lastSlashIndex = SettingsData.SpeakerIconsFilePath.LastIndexOfAny(new[] { '\\', '/' });
                    if (lastSlashIndex < 0) return string.Empty;
                    var iconsDirectory = SettingsData.SpeakerIconsFilePath.Substring(0, lastSlashIndex);
                    string prefix = "http://asset.localhost";
                    return Path.Combine(prefix, iconsDirectory, relativeIconSubPath).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving full icon path: {ex.Message}");
        }

        return string.Empty;
    }
}

