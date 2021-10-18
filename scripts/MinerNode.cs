using System;
using FactoryPlanner.scripts;
using FactoryPlanner.scripts.resources;
using Godot;
using MinerMachine = FactoryPlanner.scripts.machines.Miner;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class MinerNode : MachineNode
{
    private OptionButton MkOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<HBoxContainer>(2).GetChild<OptionButton>(0);
    private OptionButton PurityOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<HBoxContainer>(2).GetChild<OptionButton>(1);
    private OptionButton ResourceOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<OptionButton>(3);
    private Label OutputLabel => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(2).GetChild<Label>(0);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddEnumItems(this.MkOptionButton, typeof(MinerMachine.LevelList));
        AddEnumItems(this.PurityOptionButton, typeof(MinerMachine.PurityList));
        AddEnumItems(this.ResourceOptionButton, typeof(ResourceList));

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        ResourceList resourceEnumVal = (ResourceList)this.ResourceOptionButton.GetItemMetadata(index);
        if (!Resource.TryGetResource(resourceEnumVal, out Resource fromMeta)) return;

        this.SetSlot(0, this.IsSlotEnabledLeft(0), -1, Colors.White, true, fromMeta.Id, fromMeta.Color);
        this.OutputLabel.Text = fromMeta.Name;
    }
}
