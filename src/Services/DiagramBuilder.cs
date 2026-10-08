using System.Drawing;
using System.Text.Json;
using Blazor.Diagrams;
using Blazor.Diagrams.Components;
using Blazor.Diagrams.Options;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Core.Options;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Behaviors;
using GTDialogueViewer.Components;
using Point = Blazor.Diagrams.Core.Geometry.Point;

namespace GTDialogueViewer.Services;

public struct DialogueAssetData
{
    public string ReaderType { get; set; }
    public string ContextComment { get; set; }
}

public class DiagramBuilder
{
    public static BlazorDiagram CreateInitializedDiagram(string FileTextContent, out DialogueAssetData DialogueAssetData)
    {
        DialogueAssetData = new DialogueAssetData();
        
        var options = new BlazorDiagramOptions
        {
            AllowMultiSelection = false,
            Zoom =
            {
                Enabled = true,
                Inverse = true,
            },
            Links =
            {
                DefaultColor = "#ffffff",
                DefaultRouter = new NormalRouter(),
                DefaultPathGenerator = new SmoothPathGenerator()
            },
            Constraints =
            {
                ShouldDeleteNode = _ => ValueTask.FromResult(false),
                ShouldDeleteLink = _ => ValueTask.FromResult(false),
                ShouldDeleteGroup = _ => ValueTask.FromResult(false)
            }
        };
        
        var diagram = new BlazorDiagram(options);
        diagram.UnregisterBehavior<DragNewLinkBehavior>();
        diagram.UnregisterBehavior<ZoomBehavior>();
        diagram.UnregisterBehavior<DragMovablesBehavior>();
        diagram.RegisterBehavior(new CtrlZoomBehavior(diagram));
        diagram.RegisterComponent<GTDialogueNodeModel, GTDialogueNode>();
        diagram.RegisterComponent<GTDialogueNodeRerouteModel, GTDialogueReroute>();
        double importPosScalar = 1.2;
        
        if (!string.IsNullOrWhiteSpace(FileTextContent))
        {
            try
            {
                var optionsJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var graphData = JsonSerializer.Deserialize<DialogueGraphData>(FileTextContent, optionsJson);
                DialogueAssetData.ReaderType = graphData?.ReaderType ?? "";
                DialogueAssetData.ContextComment = graphData?.ContextComment ?? "";

                if (graphData?.Graph?.Nodes != null)
                {
                    var portLookup = new Dictionary<string, PortModel>();
                    var pendingLinks = new List<(string SourcePinId, string TargetPinId)>();

                    foreach (var jsonNode in graphData.Graph.Nodes)
                    {
                        var position = new Point(importPosScalar*jsonNode.Position.X, importPosScalar*jsonNode.Position.Y);
                        NodeModel nodeModel;

                        // Check if the node is a reroute node based on its CppClass
                        bool isReroute = string.Equals(jsonNode.CppClass, "DialogueRerouteNodeInfo", StringComparison.OrdinalIgnoreCase);

                        if (isReroute)
                        {
                            nodeModel = new GTDialogueNodeRerouteModel(position);
                        }
                        else
                        {
                            // Instantiate standard dialogue node
                            var dialogueNode = new GTDialogueNodeModel(position)
                            {
                                NodeTitle = jsonNode.NodeTitle,
                                NodeDescription = jsonNode.NodeDescription,
                                NodeOutputNames = jsonNode.NodeOutputNames ?? [],
                                IconPath = jsonNode.IconPath ?? ""
                            };
                            if (jsonNode.AccentNodeColor != null)
                            {
                                dialogueNode.AccentNodeColor = Color.FromArgb(
                                    (int)(jsonNode.AccentNodeColor.A * 255),
                                    (int)(jsonNode.AccentNodeColor.R * 255), 
                                    (int)(jsonNode.AccentNodeColor.G * 255), 
                                    (int)(jsonNode.AccentNodeColor.B * 255)
                                );
                            }

                            if (string.Equals(jsonNode.CppClass, "DialogueRuntimeNode_Start", StringComparison.OrdinalIgnoreCase))
                            {
                                dialogueNode.NodeTitle = new FText("Start", "", "");
                                dialogueNode.AccentNodeColor = Color.FromArgb(
                                    255,
                                    255, 
                                    0, 
                                    0
                                );
                            }
                            
                            nodeModel = dialogueNode;
                        }

                        // 1. Create Input Port if present (works for both standard & reroute nodes)
                        if (jsonNode.InputPin != null && !string.IsNullOrEmpty(jsonNode.InputPin.PinId))
                        {
                            var inputPort = nodeModel.AddPort(PortAlignment.Left);
                            portLookup[jsonNode.InputPin.PinId] = inputPort;
                        }

                        // 2. Create Output Ports and record links
                        if (jsonNode.OutputPins != null)
                        {
                            foreach (var outPin in jsonNode.OutputPins)
                            {
                                if (!string.IsNullOrEmpty(outPin.PinId))
                                {
                                    var outputPort = nodeModel.AddPort(PortAlignment.Right);
                                    portLookup[outPin.PinId] = outputPort;

                                    if (!string.IsNullOrEmpty(outPin.ConnectedPinId))
                                    {
                                        pendingLinks.Add((outPin.PinId, outPin.ConnectedPinId));
                                    }
                                }
                            }
                        }

                        diagram.Nodes.Add(nodeModel);
                    }

                    // 3. Instantiate links based on matched Pin IDs
                    foreach (var (sourcePinId, targetPinId) in pendingLinks)
                    {
                        if (portLookup.TryGetValue(sourcePinId, out var sourcePort) &&
                            portLookup.TryGetValue(targetPinId, out var targetPort))
                        {
                            var sourceAnchor = new SinglePortAnchor(sourcePort);
                            var targetAnchor = new SinglePortAnchor(targetPort);
                            var link = diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));
                            link.TargetMarker = LinkMarker.Arrow;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse dialogue JSON: {ex.Message}");
            }
        }

        return diagram;
    }
}