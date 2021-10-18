using System.Collections.Generic;
using Godot;
using FactoryPlanner.scripts.machines;

public class SmelterNode : MachineNode
{
    private OptionButton RecipeOptionButton => this.ControlsContainer.GetChild<OptionButton>(2);

    private static readonly int[] RecipeIds = { 1, 2 };

    internal SmelterNode() : base(1, 1)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        foreach (int resourceId in RecipeIds)
        {
            Recipe recipe = Recipe.GetRecipe(resourceId);
            AddOption(this.RecipeOptionButton, recipe.Name, recipe.Id);
        }

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        int recipeIdx = (int)this.RecipeOptionButton.GetItemMetadata(index);
        Recipe recipe = Recipe.GetRecipe(recipeIdx);

        this.UpdateRecipe(recipe);
    }
}
