using Godot;
using System;
using FactoryPlanner.scripts;
using FactoryPlanner.scripts.resources;
using Godot.Collections;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class GraphEdit : Godot.GraphEdit
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Set valid resources
        foreach (ResourceList val in Enum.GetValues(typeof(ResourceList)))
        {
            if (!Resource.TryGetResource(val, out Resource resource)) continue;

            // "Any" is always a valid resource
            this.AddValidConnectionType(0, resource.Id);
            this.AddValidConnectionType(resource.Id, 0);

            // A Resource can always match itself
            this.AddValidConnectionType(resource.Id, resource.Id);
        }
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey eventKey && eventKey.Pressed)
        {
            if (eventKey.Scancode == (int)KeyList.Key1)
            {
                PackedScene graphScene = ResourceLoader.Load<PackedScene>("res://Miner.tscn");
                GraphNode graphNode = (GraphNode)graphScene.Instance();

                this.AddChild(graphNode);
            }
            else if (eventKey.Scancode == (int)KeyList.Key2)
            {
                PackedScene graphScene = ResourceLoader.Load<PackedScene>("res://Smelter.tscn");
                GraphNode graphNode = (GraphNode)graphScene.Instance();

                this.AddChild(graphNode);
            }
        }
    }

    private void _on_GraphEdit_connection_request(string from, int from_slot, string to, int to_slot)
    {
        MachineNode fromNode = this.GetNode<MachineNode>(from);
        MachineNode toNode = this.GetNode<MachineNode>(to);

        if (this.IsValidConnectionType(fromNode.GetSlotTypeRight(from_slot), toNode.GetSlotTypeLeft(to_slot)) &&
            !this.HasInput(to, to_slot) && !this.HasOutput(from, from_slot))
        {
            this.ConnectNode(from, from_slot, to, to_slot);
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

    private void _on_GraphEdit_disconnection_request(string from, int from_slot, string to, int to_slot)
    {
        this.DisconnectNode(from, from_slot, to, to_slot);
    }
}
