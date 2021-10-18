using Godot;
using FactoryPlanner.scripts.machines;
using FactoryPlanner.scripts.resources;

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
        this.Outputs[0].SetResource(resourceEnumVal);

        this.UpdateSlots();
    }
}
