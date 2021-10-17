using System;
using Godot;

public class Camera2D : Godot.Camera2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Curve ZoomCurve = GD.Load<Curve>("res://zoom_curve.tres");
	private Vector2 MouseStartPos = Vector2.Zero;
	private Vector2 ScreenStartPos = Vector2.Zero;
	private bool Dragging = false;

	private float ZoomScale = 0.5f;
	private float ZoomIncrement = 0.01f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int)ButtonList.Right)
			{
				if (mouseButton.IsPressed())
				{
					Console.WriteLine("Pressed");
					this.MouseStartPos = mouseButton.Position;
					this.ScreenStartPos = this.Position;
					this.Dragging = true;
				}
				else
				{
					this.Dragging = false;
				}
			}
			else if (mouseButton.ButtonIndex == (int)ButtonList.WheelUp)
			{
				float newScale = this.ZoomScale - this.ZoomIncrement;
				if (newScale >= 0.0f)
				{
					this.ZoomScale = newScale;
					UpdateZoom();
				}
			}
			else if (mouseButton.ButtonIndex == (int)ButtonList.WheelDown)
			{
				float newScale = this.ZoomScale + this.ZoomIncrement;
				if (newScale <= 1.0f)
				{
					this.ZoomScale = newScale;
					UpdateZoom();
				}
			}
		}

		if (inputEvent is InputEventMouseMotion mouseMotion && this.Dragging)
		{
			Vector2 moveVector = this.MouseStartPos - mouseMotion.Position;
			Vector2 scaledMoveVector = moveVector * this.Zoom;

			this.Position = this.ScreenStartPos + scaledMoveVector;
		}
	}

	private void UpdateZoom()
	{
		float val = this.ZoomCurve.Interpolate(this.ZoomScale);
		this.Zoom = new Vector2(val, val);
	}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
