using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Behaviors;
using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Models;
using GTDialogueViewer.Components;

namespace GTDialogueViewer.Services;

public class CtrlZoomBehavior : Behavior
{
    private double layoutScale = 1.0;
    private double layoutScaleSpeedMultiplier = 2.0;
    
    public CtrlZoomBehavior(Diagram diagram)
        : base(diagram)
    {
        this.Diagram.Wheel += new Action<WheelEventArgs>(this.Diagram_Wheel);
    }
    
    private void Diagram_Wheel(WheelEventArgs e)
    {
        if (this.Diagram.Container == null || e.DeltaY == 0.0 || !this.Diagram.Options.Zoom.Enabled)
            return;

        if (e.CtrlKey)
        {
            // 1. Calculate mouse position in World/Graph coordinates
            double containerLeft = this.Diagram.Container.Left;
            double containerTop = this.Diagram.Container.Top;
            double currentZoom = this.Diagram.Zoom;
            double currentPanX = this.Diagram.Pan.X;
            double currentPanY = this.Diagram.Pan.Y;

            double mouseWorldX = (e.ClientX - containerLeft - currentPanX) / currentZoom;
            double mouseWorldY = (e.ClientY - containerTop - currentPanY) / currentZoom;

            // 2. Update layout scale
            double oldScale = layoutScale;
            double scaleDelta = (e.DeltaY < 0 ? 0.03 : -0.03)*layoutScaleSpeedMultiplier; // Smaller step for smoother scaling
            layoutScale = Math.Clamp(layoutScale + scaleDelta, 0.3, 3.0);

            if (layoutScale == oldScale)
                return;

            double scaleRatio = layoutScale / oldScale;

            // 3. Batch update node positions relative to the mouse pivot
            this.Diagram.Batch(() =>
            {
                foreach (var node in this.Diagram.Nodes.OfType<NodeModel>())
                {
                    // Scale position relative to the mouse world coordinate pivot
                    double newX = mouseWorldX + (node.Position.X - mouseWorldX) * scaleRatio;
                    double newY = mouseWorldY + (node.Position.Y - mouseWorldY) * scaleRatio;

                    node.SetPosition(newX, newY);
                }
            });

            return;
        }
        
        double scaleFactor = this.Diagram.Options.Zoom.ScaleFactor;
        double zoom = this.Diagram.Zoom;
        double newZoom = (this.Diagram.Options.Zoom.Inverse ? e.DeltaY * -1.0 : e.DeltaY) > 0.0 ? zoom * scaleFactor : zoom / scaleFactor;
        newZoom = Math.Clamp(newZoom, this.Diagram.Options.Zoom.Minimum, this.Diagram.Options.Zoom.Maximum);
        if (newZoom < 0.0 || newZoom == this.Diagram.Zoom)
            return;
        double width = this.Diagram.Container.Width;
        double height = this.Diagram.Container.Height;
        double num1 = width * newZoom - width * zoom;
        double num2 = height * newZoom - height * zoom;
        double num3 = e.ClientX - this.Diagram.Container.Left;
        double num4 = e.ClientY - this.Diagram.Container.Top;
        double x = this.Diagram.Pan.X;
        double num5 = (num3 - x) / zoom / width;
        double num6 = (num4 - this.Diagram.Pan.Y) / zoom / height;
        double newPanX = this.Diagram.Pan.X - num1 * num5;
        double newPanY = this.Diagram.Pan.Y - num2 * num6;
        this.Diagram.Batch((Action) (() =>
        {
            this.Diagram.SetPan(newPanX, newPanY);
            this.Diagram.SetZoom(newZoom);
        }));
    }

    public override void Dispose()
    {
        this.Diagram.Wheel -= new Action<WheelEventArgs>(this.Diagram_Wheel);
    }
}