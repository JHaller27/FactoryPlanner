using System;
using System.ComponentModel;
using FactoryPlanner.scripts;
using FactoryPlanner.scripts.machines;
using FactoryPlanner.scripts.resources;
using Godot;

public class MinerNode : MachineNode
{
    private OptionButton MkOptionButton => this.ControlsContainer.GetChild<HBoxContainer>(2).GetChild<OptionButton>(0);
    private OptionButton PurityOptionButton => this.ControlsContainer.GetChild<HBoxContainer>(2).GetChild<OptionButton>(1);
    private OptionButton ResourceOptionButton => this.ControlsContainer.GetChild<OptionButton>(3);

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
