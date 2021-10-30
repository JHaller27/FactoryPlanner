using Godot;
using FactoryPlanner.scripts.machines;

public class SmelterNode : EfficientMachineNode
{
    private OptionButton RecipeOptionButton => this.ControlsContainer.GetChild<OptionButton>(2);

    internal SmelterNode() : base(1, 1)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        foreach (Recipe recipe in Recipe.GetRecipesWithTags("Smelter"))
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
