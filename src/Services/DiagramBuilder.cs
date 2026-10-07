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

public class DiagramBuilder
{
    public static BlazorDiagram CreateInitializedDiagram(string FileTextContent)
    {
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
        
        if (!string.IsNullOrWhiteSpace(FileTextContent))
        {
            try
            {
                var optionsJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var graphData = JsonSerializer.Deserialize<DialogueGraphData>(FileTextContent, optionsJson);

                if (graphData?.Graph?.Nodes != null)
                {
                    // Dictionary to map PinId strings to actual Blazor.Diagrams PortModels
                    var portLookup = new Dictionary<string, PortModel>();
                    var pendingLinks = new List<(string SourcePinId, string TargetPinId)>();

                    foreach (var jsonNode in graphData.Graph.Nodes)
                    {
                        var position = new Point(jsonNode.Position.X, jsonNode.Position.Y);

                        var nodeModel = new GTDialogueNodeModel(position)
                        {
                            NodeTitle = jsonNode.NodeTitle,
                            NodeDescription = jsonNode.NodeDescription,
                            NodeOutputNames = jsonNode.NodeOutputNames ?? [],
                            IconPath = jsonNode.IconPath ?? ""
                        };

                        if (jsonNode.AccentNodeColor != null)
                        {
                            nodeModel.AccentNodeColor = Color.FromArgb(
                                (int)(jsonNode.AccentNodeColor.A * 255),
                                (int)(jsonNode.AccentNodeColor.R * 255), 
                                (int)(jsonNode.AccentNodeColor.G * 255), 
                                (int)(jsonNode.AccentNodeColor.B * 255)
                            );
                        }

                        // 1. Create Input Port if present
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