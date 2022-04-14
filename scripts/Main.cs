using System.Collections.Generic;
using FactoryPlanner.scripts.machines;
using Godot;

public class Main : Control
{
	private static readonly List<PackedScene> MachineSceneMap = new()
	{
		ResourceLoader.Load<PackedScene>("res://scenes/machines/Miner.tscn"),
		ResourceLoader.Load<PackedScene>("res://scenes/machines/Smelter.tscn"),
		ResourceLoader.Load<PackedScene>("res://scenes/machines/Constructor.tscn"),
		ResourceLoader.Load<PackedScene>("res://scenes/machines/Assembler.tscn"),
		ResourceLoader.Load<PackedScene>("res://scenes/machines/Balancer.tscn"),
	};

	private Container ButtonContainer { get; set; }

	private GraphEdit GraphEdit { get; set; }

	public override void _Ready()
	{
		this.GraphEdit = this.GetNode<GraphEdit>("GraphEdit");
		this.ButtonContainer = this.GetNode<Container>("Buttons");
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed(KnownInputActions.FocusAddMenu))
		{
			this.GetNode("Buttons").GetChild<Control>(0).GrabFocus();
		}

		if (inputEvent.IsActionPressed(KnownInputActions.Duplicate))
		{
			this.GraphEdit.DuplicateSelected();
			return;
		}

		int? keyIdx = KnownInputActions.UISelectIdxPressed(inputEvent);
		if (keyIdx == null) return;

		Vector2 mousePosition = GetGlobalMousePosition();
		this.AddMachineAt(keyIdx.Value, mousePosition);
	}

	// ReSharper disable once UnusedMember.Local - Signal
	private void AddMachine(int idx)
	{
		Vector2 position = this.GraphEdit.ScrollOffset + this.GetViewportRect().Size / 2;
		this.AddMachineAt(idx, position);
	}

	private void AddMachineAt(int idx, Vector2 position)
	{
		PackedScene graphScene = MachineSceneMap[idx];
		MachineNode graphNode = graphScene.Instance<MachineNode>();

		this.GraphEdit.AddMachineAt(graphNode, position);
	}
}
