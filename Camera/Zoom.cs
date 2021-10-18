using System;
using Godot;

namespace FactoryPlanner.Camera
{
    public class Zoom
    {
        private Godot.Camera2D Camera { get; }
        private readonly Curve ZoomCurve = GD.Load<Curve>("res://Camera/zoom_curve.tres");
        private int ZoomScale { get; set; }
        private int ZoomIncrement { get; }

        public Zoom(Godot.Camera2D camera, int initialZoomScale, int zoomIncrement)
        {
            this.Camera = camera;
            this.ZoomScale = initialZoomScale;
            this.ZoomIncrement = zoomIncrement;
            
            this.UpdateZoom();
        }

        public void ZoomIn(int times = 1)
        {
            int newScale = this.ZoomScale - this.ZoomIncrement * times;
            if (newScale > 0)
            {
                this.ZoomScale = newScale;
            }

            UpdateZoom();
        }

        public void ZoomOut(int times = 1)
        {
            int newScale = this.ZoomScale + this.ZoomIncrement * times;
            if (newScale <= 100)
            {
                this.ZoomScale = newScale;
            }

            UpdateZoom();
        }

        private void UpdateZoom()
        {
            float zoomScale = this.ZoomScale / 100f;
            float val = this.ZoomCurve.Interpolate(zoomScale);

            Console.WriteLine(this.ZoomScale + " => " + val);

            if ((int)(val * 1000) == 0)
            {
                return;
            }

            this.Camera.Zoom = new Vector2(val, val);
        }
    }
}
