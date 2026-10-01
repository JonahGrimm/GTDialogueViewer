using Blazor.Diagrams;
using Blazor.Diagrams.Components;
using Blazor.Diagrams.Options;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Options;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Behaviors;
using GTDialogueViewer.Components;

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
        diagram.RegisterComponent<GTDialogueNodeModel, GTDialogueNode>();
        
        // (Optional) Populate default test nodes or load nodes based on file content here later:
        var firstNode = diagram.Nodes.Add(new GTDialogueNodeModel(position: new Point(50, 50))
        {
            Title = "Node 1"
        });
        var secondNode = diagram.Nodes.Add(new GTDialogueNodeModel(position: new Point(200, 100))
        {
            Title = "Node 2"
        });
        var leftPort = secondNode.AddPort(PortAlignment.Left);
        var rightPort = firstNode.AddPort(PortAlignment.Right);
        var sourceAnchor = new SinglePortAnchor(rightPort);
        var targetAnchor = new SinglePortAnchor(leftPort);
        diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));

        return diagram;
    }
}