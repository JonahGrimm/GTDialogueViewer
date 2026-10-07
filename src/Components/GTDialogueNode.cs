using System.Drawing;
using Blazor.Diagrams.Core.Models;
using Point = Blazor.Diagrams.Core.Geometry.Point;

namespace GTDialogueViewer.Components;

public struct FText
{
    public string SourceString { get; set; }
    public string Key { get; set; }
    public string Table { get; set; }
    
    public string GetText() => SourceString;

    public bool IsLocalizable
    {
        get => !string.IsNullOrEmpty(Table) && !string.IsNullOrEmpty(Key);
    }
}

public class GTDialogueNodeModel : NodeModel
{
    public Point BasePosition { get; set; }
    public Color AccentNodeColor { get; set; }
    public FText NodeTitle { get; set; }
    public FText NodeDescription { get; set; }
    public FText[] NodeOutputNames { get; set; } = [];
    public string IconPath { get; set; } = "";
    
    public GTDialogueNodeModel(Point? position = null) : base(position)
    {
        // Store the initial position as the base/real position
        BasePosition = position ?? new Point(0, 0);
    }
}