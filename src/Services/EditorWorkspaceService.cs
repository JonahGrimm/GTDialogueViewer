using Microsoft.JSInterop;
using System.IO;
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

namespace GTDialogueViewer.Services;

public class EditorTab
{
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public BlazorDiagram Diagram { get; set; } = null!;
    public string GetFileName()
    {
        if (string.IsNullOrEmpty(FilePath)) return string.Empty;
        // Splits by either slash type and grabs the last segment
        return FilePath.Split(new[] { '/', '\\' }).Last().Split('.').First();
    }
}

public class EditorWorkspaceService
{
    private readonly IJSRuntime _js;

    public List<EditorTab> OpenTabs { get; } = new();
    public EditorTab? ActiveTab { get; private set; }

    public event Action? OnChange;

    public EditorWorkspaceService(IJSRuntime js)
    {
        _js = js;
    }

    public void SwitchTab(EditorTab tab)
    {
        ActiveTab = tab;
        NotifyStateChanged();
    }

    public void CloseTab(EditorTab tab)
    {
        int index = OpenTabs.IndexOf(tab);
        OpenTabs.Remove(tab);
        if (ActiveTab == tab)
        {
            ActiveTab = OpenTabs.Count > 0 ? OpenTabs[Math.Max(0, index - 1)] : null;
        }
        NotifyStateChanged();
    }

    public void ReorderTabs(int oldIndex, int newIndex)
    {
        if (oldIndex == newIndex || oldIndex < 0 || newIndex < 0 || oldIndex >= OpenTabs.Count || newIndex >= OpenTabs.Count)
            return;

        var item = OpenTabs[oldIndex];
        OpenTabs.RemoveAt(oldIndex);
        OpenTabs.Insert(newIndex, item);
        NotifyStateChanged();
    }

    public async Task HandleExternalFilesInternal(List<string> args)
    {
        foreach (var arg in args)
        {
            if (!arg.StartsWith("-") && (arg.EndsWith(".gtdialogue", StringComparison.OrdinalIgnoreCase)))
            {
                if (File.Exists(arg) || await CheckFileExistsBackend(arg))
                {
                    await OpenFile(arg);
                }
            }
        }
    }

    private async Task<bool> CheckFileExistsBackend(string path)
    {
        try { return true; } catch { return false; }
    }

    public async Task OpenFile(string path)
    {
        var existing = OpenTabs.FirstOrDefault(t => t.FilePath.Equals(path, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            SwitchTab(existing);
            return;
        }

        try
        {
            string text = await _js.InvokeAsync<string>("window.__TAURI__.core.invoke", "read_file_content", new { path });
        
            var newTab = new EditorTab
            {
                FilePath = path,
                Content = text,
                Diagram = CreateInitializedDiagram()
            };
            OpenTabs.Add(newTab);
            SwitchTab(newTab);
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            await _js.InvokeVoidAsync("alert", $"Error opening file: {ex.Message}");
        }
    }
    
    private BlazorDiagram CreateInitializedDiagram()
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
                DefaultRouter = new NormalRouter(),
                DefaultPathGenerator = new SmoothPathGenerator()
            },
        };
        
        var diagram = new BlazorDiagram(options);
        
        // (Optional) Populate default test nodes or load nodes based on file content here later:
        var firstNode = diagram.Nodes.Add(new NodeModel(position: new Point(50, 50))
        {
            Title = "Node 1"
        });
        var secondNode = diagram.Nodes.Add(new NodeModel(position: new Point(200, 100))
        {
            Title = "Node 2"
        });
        var leftPort = secondNode.AddPort(PortAlignment.Left);
        var sourceAnchor = new ShapeIntersectionAnchor(firstNode);
        var targetAnchor = new SinglePortAnchor(leftPort);
        diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));

        return diagram;
    }

    public async Task OpenFilePrompt()
    {
        var path = await _js.InvokeAsync<string?>("window.__TAURI__.core.invoke", "open_file_dialog");

        if (!string.IsNullOrEmpty(path))
        {
            await OpenFile(path);
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}