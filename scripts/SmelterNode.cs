using Godot;
using System;
using FactoryPlanner.scripts;
using FactoryPlanner.scripts.resources;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class SmelterNode : MachineNode
{
    private OptionButton ResourceOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<OptionButton>(2);
    private Label InputLabel => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(0).GetChild<Label>(0);
    private Label OutputLabel => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(2).GetChild<Label>(0);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddEnumItems(this.ResourceOptionButton, typeof(ResourceList));

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        ResourceList resourceEnumVal = (ResourceList)this.ResourceOptionButton.GetItemMetadata(index);
        if (!Resource.TryGetResource(resourceEnumVal, out Resource fromMeta)) return;

        this.SetSlot(0, this.IsSlotEnabledLeft(0), this.GetSlotTypeLeft(0), Colors.White, true, fromMeta.Id, fromMeta.Color);
        this.OutputLabel.Text = fromMeta.Name;
    }
}
