using System.Drawing;
using Blazor.Diagrams.Core.Models;
using Point = Blazor.Diagrams.Core.Geometry.Point;

namespace GTDialogueViewer.Components;

public class GTDialogueNodeModel : NodeModel
{
    public GTDialogueNodeModel(Point? position = null) : base(position) { }

    public Color AccentColor { get; set; }
    public string TitleKey { get; set; }
    public string TitleTable { get; set; }
}