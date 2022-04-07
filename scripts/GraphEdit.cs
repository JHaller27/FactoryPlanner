using System;
using FactoryPlanner.DataReader;
using Godot;
using FactoryPlanner.scripts.machines;
using Godot.Collections;
using Resource = FactoryPlanner.scripts.machines.Resource;
using Network = MachineNetwork.MachineNetwork;

public class GraphEdit : Godot.GraphEdit
{
	private MachineNode Selected { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// One-time load resources
		Reader.LoadData("res://data/Resources.json");

		// Set valid resources
		foreach (Resource resource in Resource.Resources.Values)
		{
			// "Any" is always a valid resource
			if (resource != Resource.Any)
			{
				this.AddValidConnectionType(Resource.Any.TypeId, resource.TypeId);
				this.AddValidConnectionType(resource.TypeId, Resource.Any.TypeId);
			}

			// A Resource can always match itself
			this.AddValidConnectionType(resource.TypeId, resource.TypeId);
		}
	}

	public void AddMachineAt(MachineNode node, Vector2 position)
	{
		this.AddChild(node);
		Network.Instance.AddMachine(node.GetMachineModel());
		node.UpdateSlots();

		node.Offset = position + this.ScrollOffset;

		this.SetSelected(node);
	}

	public void DuplicateSelected()
	{
		if (this.Selected == null) return;

		MachineNode dupe = (MachineNode)this.Selected.Duplicate();
		Vector2 mousePosition = GetGlobalMousePosition();
		this.AddMachineAt(dupe, mousePosition);
	}

	private void _on_GraphEdit_connection_request(string fromName, int fromSlot, string toName, int toSlot)
	{
		MachineNode fromNode = this.GetNode<MachineNode>(fromName);
		MachineNode toNode = this.GetNode<MachineNode>(toName);

		if (this.IsValidConnectionType(fromNode.GetSlotTypeRight(fromSlot+1), toNode.GetSlotTypeLeft(toSlot+1)) &&
			!this.HasInput(toName, toSlot) && !this.HasOutput(fromName, fromSlot))
		{
			this.ConnectNode(fromName, fromSlot, toName, toSlot);
			Network.Instance.ConnectMachines(fromNode.GetMachineModel(), fromSlot, toNode.GetMachineModel(), toSlot);

			this.UpdateAllMachines();
		}
	}

	private void UpdateAllMachines()
	{
		Console.WriteLine("====================");
		Console.WriteLine(Network.Instance);

		foreach (object obj in this.GetChildren())
		{
			if (obj is MachineNode machineNode)
			{
				machineNode.UpdateSlots();
			}
		}
	}

	private bool HasInput(string toName, int toSlot)
	{
		foreach (Dictionary x in this.GetConnectionList())
		{
			string checkName = (string)x["to"];
			int checkSlot = (int)x["to_port"];

			if (checkName == toName && checkSlot == toSlot) return true;
		}

		return false;
	}

	private bool HasOutput(string fromName, int fromSlot)
	{
		foreach (Dictionary x in this.GetConnectionList())
		{
			string checkName = (string)x["from"];
			int checkSlot = (int)x["from_port"];

			if (checkName == fromName && checkSlot == fromSlot) return true;
		}

		return false;
	}

	private void _on_GraphEdit_disconnection_request(string fromName, int fromSlot, string toName, int toSlot)
	{
		MachineNode fromNode = this.GetNode<MachineNode>(fromName);
		MachineNode toNode = this.GetNode<MachineNode>(toName);

		this.DisconnectNode(fromName, fromSlot, toName, toSlot);
		Network.Instance.DisconnectMachines(fromNode.GetMachineModel(), fromSlot, toNode.GetMachineModel(), toSlot);

		this.UpdateAllMachines();
	}

	private void _on_GraphEdit_delete_nodes_request()
	{
		Network.Instance.RemoveMachine(this.Selected.GetMachineModel());
	}

	private void _on_GraphEdit_node_selected(MachineNode node)
	{
		this.Selected = node;
	}

	private void _on_GraphEdit_node_unselected(object node)
	{
		this.Selected = null;
	}
}
