using Godot;
using FactoryPlanner.scripts.machines;

public class SmelterNode : MachineNode
{
    private OptionButton RecipeOptionButton => this.ControlsContainer.GetChild<OptionButton>(2);

    private static readonly string[] RecipeIds = { "SmeltIronIngot", "SmeltCopperIngot" };

    internal SmelterNode() : base(1, 1)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        foreach (string recipeId in RecipeIds)
        {
            Recipe recipe = Recipe.GetRecipe(recipeId);
            AddOption(this.RecipeOptionButton, recipe.Name, recipe.Id);
        }

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        string recipeId = (string)this.RecipeOptionButton.GetItemMetadata(index);
        Recipe recipe = Recipe.GetRecipe(recipeId);

        this.UpdateRecipe(recipe);
    }
}
