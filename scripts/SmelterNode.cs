using System.Collections.Generic;
using Godot;
using FactoryPlanner.scripts.machines;
using FactoryPlanner.scripts.resources;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class SmelterNode : MachineNode
{
    private OptionButton ResourceOptionButton => this.ControlsContainer.GetChild<OptionButton>(2);

    internal SmelterNode() : base(1, 1)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        AddEnumItems(this.ResourceOptionButton, typeof(ResourceList));

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        ResourceList resourceEnumVal = (ResourceList)this.ResourceOptionButton.GetItemMetadata(index);
        this.UpdateRecipe(new Recipe
        {
            Name = "Iron Ingot",
            Inputs = new List<Throughput> {new Throughput {Rate = 3000, Resource = Resource.GetResource(resourceEnumVal)}},
            Outputs = new List<Throughput> {new Throughput {Rate = 4500, Resource = Resource.GetResource(ResourceList.Copper)}},
        });

        this.UpdateSlots();
    }
}
