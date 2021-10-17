using System;
using Godot;

namespace FactoryPlanner.Camera
{
    public class Zoom
    {
        private readonly Curve ZoomCurve = GD.Load<Curve>("res://Camera/zoom_curve.tres");
        private int ZoomScale { get; set; }
        private int ZoomIncrement { get; }

        public Zoom(int initialZoomScale, int zoomIncrement)
        {
            this.ZoomScale = initialZoomScale;
            this.ZoomIncrement = zoomIncrement;
        }

        public Vector2 ZoomIn(int times = 1)
        {
            int newScale = this.ZoomScale - this.ZoomIncrement * times;
            if (newScale > 0)
            {
                this.ZoomScale = newScale;
            }

            return CalculateZoom();
        }

        public Vector2 ZoomOut(int times = 1)
        {
            int newScale = this.ZoomScale + this.ZoomIncrement * times;
            if (newScale <= 100)
            {
                this.ZoomScale = newScale;
            }

            return CalculateZoom();
        }

        private Vector2 CalculateZoom()
        {
            float zoomScale = this.ZoomScale / 100f;
            float val = this.ZoomCurve.Interpolate(zoomScale);

            Console.WriteLine(this.ZoomScale + " => " + val);

            return new Vector2(val, val);
        }
    }
}
