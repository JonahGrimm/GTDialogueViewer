using System.Text.Json.Serialization;
using GTDialogueViewer.Nodes;

namespace GTDialogueViewer.Services;

public class DialogueGraphData
{
    [JsonPropertyName("ReaderType")] public string ReaderType { get; set; } = "";
    [JsonPropertyName("ContextComment")] public string ContextComment { get; set; } = "";
    [JsonPropertyName("Graph")] public GraphData Graph { get; set; } = new();
}

public class GraphData
{
    [JsonPropertyName("Nodes")] public List<DialogueJsonNode> Nodes { get; set; } = [];
    [JsonPropertyName("Comments")] public List<CommentJsonNode> Comments { get; set; } = [];
}

public class DialogueJsonNode
{
    [JsonPropertyName("CppClass")] public string CppClass { get; set; } = "";
    [JsonPropertyName("Position")] public PositionData Position { get; set; } = new();
    [JsonPropertyName("NodeTitle")] public FText NodeTitle { get; set; }
    [JsonPropertyName("NodeDescription")] public FText NodeDescription { get; set; }
    [JsonPropertyName("NodeOutputNames")] public FText[] NodeOutputNames { get; set; } = [];
    [JsonPropertyName("IconPath")] public string? IconPath { get; set; } = "";
    [JsonPropertyName("AccentNodeColor")] public AccentColor AccentNodeColor { get; set; } = new();
    
    [JsonPropertyName("InputPin")] public PinData? InputPin { get; set; }
    [JsonPropertyName("OutputPins")] public PinData[] OutputPins { get; set; } = [];
}

public class CommentJsonNode
{
    [JsonPropertyName("Comment")] public string CommentText { get; set; } = "";
    [JsonPropertyName("Position")] public PositionData Position { get; set; } = new();
    [JsonPropertyName("Size")] public PositionData Size { get; set; } = new();
    [JsonPropertyName("CommentColor")] public AccentColor CommentColor { get; set; } = new();
}

public class PinData
{
    [JsonPropertyName("PinName")] public string PinName { get; set; } = "";
    [JsonPropertyName("PinId")] public string PinId { get; set; } = "";
    [JsonPropertyName("ConnectedPinId")] public string? ConnectedPinId { get; set; }
}

public class PositionData
{
    [JsonPropertyName("X")] public double X { get; set; }
    [JsonPropertyName("Y")] public double Y { get; set; }
}

public class AccentColor
{
    [JsonPropertyName("R")] public float R { get; set; }
    [JsonPropertyName("G")] public float G { get; set; }
    [JsonPropertyName("B")] public float B { get; set; }
    [JsonPropertyName("A")] public float A { get; set; }
}