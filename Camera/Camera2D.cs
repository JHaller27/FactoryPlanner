using System;
using FactoryPlanner.Camera;
using Godot;

public class Camera2D : Godot.Camera2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Vector2 MouseStartPos = Vector2.Zero;
	private Vector2 ScreenStartPos = Vector2.Zero;
	private bool Dragging = false;

	private Zoom ZoomHandler { get; }

	public Camera2D()
	{
		this.ZoomHandler = new Zoom(this, 50, 5);
	}

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
			else if (mouseButton.ButtonIndex == (int)ButtonList.WheelUp && mouseButton.IsPressed())
			{
				this.ZoomHandler.ZoomIn();
			}
			else if (mouseButton.ButtonIndex == (int)ButtonList.WheelDown && mouseButton.IsPressed())
			{
				this.ZoomHandler.ZoomOut();
			}
		}

		if (inputEvent is InputEventMouseMotion mouseMotion && this.Dragging)
		{
			Vector2 moveVector = this.MouseStartPos - mouseMotion.Position;
			Vector2 scaledMoveVector = moveVector * this.Zoom;

			this.Position = this.ScreenStartPos + scaledMoveVector;
		}
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
