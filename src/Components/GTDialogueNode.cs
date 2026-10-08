using System.Drawing;
using Blazor.Diagrams.Core.Models;
using Point = Blazor.Diagrams.Core.Geometry.Point;

namespace GTDialogueViewer.Components;

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
}