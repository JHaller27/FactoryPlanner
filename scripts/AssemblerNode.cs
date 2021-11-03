using Godot;
using FactoryPlanner.scripts.machines;

public class AssemblerNode : EfficientMachineNode
{
    private OptionButton RecipeOptionButton => this.ControlsContainer.GetChild<OptionButton>(2);

    internal AssemblerNode() : base(2, 1)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        foreach (Recipe recipe in Recipe.GetRecipesWithTags("Assembler"))
        {
            AddOption(this.RecipeOptionButton, recipe.Name, recipe.Id);
        }

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        this.RecipeId = (string)this.RecipeOptionButton.GetItemMetadata(index);
        this.UpdateRecipe();
    }
}
