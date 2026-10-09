using System.Drawing;
using Blazor.Diagrams.Core.Models;
using Point = Blazor.Diagrams.Core.Geometry.Point;

namespace GTDialogueViewer.Nodes;

public class GTCommentNodeModel(Point? position = null) : NodeModel(position)
{
    public Color AccentNodeColor { get; set; }
    public string Comment { get; set; } = "";
    public Point NodeSize { get; set; } = new(100, 50);
}