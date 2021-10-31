using System;
using System.Collections.Generic;
using FactoryPlanner.DataReader;
using Godot;
using FactoryPlanner.scripts.machines;
using Godot.Collections;
using Resource = FactoryPlanner.scripts.machines.Resource;
using Network = MachineNetwork.MachineNetwork;

public class GraphEdit : Godot.GraphEdit
{
    private static readonly IDictionary<uint, string> KeyMachinePathMap = new Godot.Collections.Dictionary<uint, string>
    {
        [(int)KeyList.Key1] = "res://Miner.tscn",
        [(int)KeyList.Key2] = "res://Smelter.tscn",
        [(int)KeyList.Key3] = "res://Constructor.tscn",
        [(int)KeyList.Key4] = "res://Balancer.tscn",
    };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MachineNetwork.MachineNetwork.Precision = 100;

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

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey eventKey && eventKey.Pressed)
        {
            if (KeyMachinePathMap.TryGetValue(eventKey.Scancode, out string path))
            {
                PackedScene graphScene = ResourceLoader.Load<PackedScene>(path);
                MachineNode graphNode = (MachineNode)graphScene.Instance();

                this.AddChild(graphNode);
                Network.Instance.AddMachine(graphNode.GetMachineModel());
                graphNode.UpdateSlots();

                Vector2 mousePosition = GetGlobalMousePosition();
                graphNode.Offset = mousePosition;
            }
        }
    }

    private void _on_GraphEdit_connection_request(string fromName, int fromSlot, string toName, int toSlot)
    {
        EfficientMachineNode fromNode = this.GetNode<EfficientMachineNode>(fromName);
        EfficientMachineNode toNode = this.GetNode<EfficientMachineNode>(toName);

        if (this.IsValidConnectionType(fromNode.GetSlotTypeRight(fromSlot), toNode.GetSlotTypeLeft(toSlot)) &&
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
            if (obj is EfficientMachineNode machineNode)
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
        EfficientMachineNode fromNode = this.GetNode<EfficientMachineNode>(fromName);
        EfficientMachineNode toNode = this.GetNode<EfficientMachineNode>(toName);

        this.DisconnectNode(fromName, fromSlot, toName, toSlot);
        Network.Instance.DisconnectMachines(fromNode.GetMachineModel(), fromSlot, toNode.GetMachineModel(), toSlot);

        this.UpdateAllMachines();
    }
}
