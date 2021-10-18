using Godot;
using System;

public class GraphNode : Godot.GraphNode
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.SetSlot(0, true, 3, Colors.Blue, true, 3, Colors.White);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
