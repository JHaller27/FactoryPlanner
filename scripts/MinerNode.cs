using System;
using System.ComponentModel;
using FactoryPlanner.scripts;
using FactoryPlanner.scripts.machines;
using FactoryPlanner.scripts.resources;
using Godot;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class MinerNode : MachineNode
{
    private OptionButton MkOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<HBoxContainer>(2).GetChild<OptionButton>(0);
    private OptionButton PurityOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<HBoxContainer>(2).GetChild<OptionButton>(1);
    private OptionButton ResourceOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<OptionButton>(3);
    private Label OutputLabel => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(2).GetChild<Label>(0);

    private enum LevelList
    {
        [Description("Mk. 1")]
        Mk1,

        [Description("Mk. 2")]
        Mk2,

        [Description("Mk. 3")]
        Mk3,
    }

    private enum PurityList
    {
        Pure,
        Normal,
        Impure,
    }

    public MinerNode() : base(0, 1)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        AddEnumItems(this.MkOptionButton, typeof(LevelList));
        AddEnumItems(this.PurityOptionButton, typeof(PurityList));
        AddEnumItems(this.ResourceOptionButton, typeof(ResourceList));

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        ResourceList resourceEnumVal = (ResourceList)this.ResourceOptionButton.GetItemMetadata(index);
        this.Outputs[0].SetResource(resourceEnumVal);

        this.UpdateSlots();
    }
}
