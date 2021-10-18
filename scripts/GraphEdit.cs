using Godot;
using System;
using FactoryPlanner.scripts;
using FactoryPlanner.scripts.machines;
using FactoryPlanner.scripts.resources;
using Godot.Collections;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class GraphEdit : Godot.GraphEdit
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // One-time load resources
        Resource.LoadResources();

        // Set valid resources
        foreach (Resource resource in Resource.Resources)
        {
            // "Any" is always a valid resource
            if (resource != Resource.Any)
            {
                this.AddValidConnectionType(Resource.Any.Id, resource.Id);
                this.AddValidConnectionType(resource.Id, Resource.Any.Id);
            }

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

    private void _on_GraphEdit_connection_request(string fromName, int fromSlot, string toName, int toSlot)
    {
        MachineNode fromNode = this.GetNode<MachineNode>(fromName);
        MachineNode toNode = this.GetNode<MachineNode>(toName);

        if (this.IsValidConnectionType(fromNode.GetSlotTypeRight(fromSlot), toNode.GetSlotTypeLeft(toSlot)) &&
            !this.HasInput(toName, toSlot) && !this.HasOutput(fromName, fromSlot))
        {
            this.ConnectNode(fromName, fromSlot, toName, toSlot);
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

    private void _on_GraphEdit_disconnection_request(string from, int fromSlot, string to, int toSlot)
    {
        this.DisconnectNode(from, fromSlot, to, toSlot);
    }
}
