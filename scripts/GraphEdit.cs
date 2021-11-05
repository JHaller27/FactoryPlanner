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
        [(int)KeyList.Key4] = "res://Assembler.tscn",
        [(int)KeyList.Key5] = "res://Balancer.tscn",
    };

    private MachineNode Selected { get; set; }
    private bool CtrlHeld { get; set; }
    private bool JustDuped { get; set; }

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
        if (inputEvent is InputEventKey eventKey)
        {
            if (KeyMachinePathMap.TryGetValue(eventKey.Scancode, out string path) && eventKey.Pressed)
            {
                PackedScene graphScene = ResourceLoader.Load<PackedScene>(path);
                MachineNode graphNode = (MachineNode)graphScene.Instance();

                this.AddChild(graphNode);
                Network.Instance.AddMachine(graphNode.GetMachineModel());
                graphNode.UpdateSlots();

                Vector2 mousePosition = GetGlobalMousePosition();
                graphNode.Offset = mousePosition + this.ScrollOffset;

                this.SetSelected(graphNode);
            }
            else if (eventKey.Scancode == (int)KeyList.Control)
            {
                this.CtrlHeld = eventKey.Pressed;
                if (!this.CtrlHeld)
                {
                    this.JustDuped = false;
                }
            }
            else if (eventKey.Scancode == (int)KeyList.D && this.CtrlHeld)
            {
                if (this.Selected == null)
                {
                }
                else if (eventKey.Pressed && !this.JustDuped)
                {
                    this.JustDuped = true;

                    MachineNode dupe = (MachineNode)this.Selected.Duplicate();
                    this.AddChild(dupe);
                    Network.Instance.AddMachine(dupe.GetMachineModel());
                    dupe.UpdateSlots();

                    Vector2 mousePosition = GetGlobalMousePosition();
                    dupe.Offset = mousePosition + this.ScrollOffset;
                    this.SetSelected(dupe);
                }
                else
                {
                    this.JustDuped = false;
                }
            }
        }
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

    private void _on_GraphEdit_node_selected(MachineNode node)
    {
        this.Selected = node;
    }

    private void _on_GraphEdit_node_unselected(object node)
    {
        this.Selected = null;
    }
}
