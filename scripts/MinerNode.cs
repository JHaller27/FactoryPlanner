using System;
using FactoryPlanner.scripts.resources;
using Godot;
using MinerMachine = FactoryPlanner.scripts.machines.Miner;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class MinerNode : Godot.GraphNode
{
    private readonly MinerMachine Machine = new MinerMachine();
    private OptionButton MkOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<HBoxContainer>(2).GetChild<OptionButton>(0);
    private OptionButton PurityOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<HBoxContainer>(2).GetChild<OptionButton>(1);
    private OptionButton ResourceOptionButton => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1).GetChild<OptionButton>(3);
    private Label OutputLabel => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(2).GetChild<Label>(0);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (MinerMachine.LevelList level in Enum.GetValues(typeof(MinerMachine.LevelList)))
        {
            this.MkOptionButton.AddItem(level.ToString());
            this.MkOptionButton.SetItemMetadata(this.MkOptionButton.GetItemCount()-1, level);
        }
        foreach (MinerMachine.PurityList purity in Enum.GetValues(typeof(MinerMachine.PurityList)))
        {
            this.PurityOptionButton.AddItem(purity.ToString());
            this.PurityOptionButton.SetItemMetadata(this.PurityOptionButton.GetItemCount()-1, purity);
        }
        foreach (ResourceList resourceEnumVal in Enum.GetValues(typeof(ResourceList)))
        {
            if (!Resource.TryGetResource(resourceEnumVal, out Resource resource)) continue;

            this.ResourceOptionButton.AddItem(resource.Name);
            this.ResourceOptionButton.SetItemMetadata(this.ResourceOptionButton.GetItemCount()-1, resourceEnumVal);
        }
    }

    private void _on_Resource_Selected(int index)
    {
        ResourceList resourceEnumVal = (ResourceList)this.ResourceOptionButton.GetSelectedMetadata();
        if (!Resource.TryGetResource(resourceEnumVal, out Resource fromMeta)) return;

        this.SetSlot(0, false, -1, Colors.White, true, fromMeta.Id, fromMeta.Color);
        this.OutputLabel.Text = fromMeta.Name;
    }
}
