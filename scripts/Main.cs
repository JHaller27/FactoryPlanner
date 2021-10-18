using Godot;
using System;

public class Main : GraphEdit
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent inputEvent)
	{
		PackedScene graphScene = ResourceLoader.Load<PackedScene>("res://GraphNode.tscn");
		GraphNode graphNode = (GraphNode)graphScene.Instance();

		if (inputEvent is InputEventKey eventKey && eventKey.Pressed)
		{
			if (eventKey.Scancode == (int)KeyList.Space)
			{
				this.AddChild(graphNode);
			}
		}
	}

	private void _on_GraphEdit_connection_request(string from, int from_slot, string to, int to_slot)
	{
		this.ConnectNode(from, from_slot, to, to_slot);
	}
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
