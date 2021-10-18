using System.Collections.Generic;
using Godot;
using FactoryPlanner.scripts.machines;
using Resource = FactoryPlanner.scripts.resources.Resource;

public class SmelterNode : MachineNode
{
    private OptionButton RecipeOptionButton => this.ControlsContainer.GetChild<OptionButton>(2);
    private IList<Recipe> Recipes { get; }

    internal SmelterNode() : base(1, 1)
    {
        this.Recipes = new List<Recipe>
        {
            new Recipe
            {
                Name = "Iron Ingot",
                Inputs = new List<Throughput>{ new Throughput{Rate = 3000, Resource = Resource.GetResource(1)}},
                Outputs = new List<Throughput>{ new Throughput{Rate = 3000, Resource = Resource.GetResource(3)}},
            },
            new Recipe
            {
                Name = "Copper Ingot",
                Inputs = new List<Throughput>{ new Throughput{Rate = 3000, Resource = Resource.GetResource(2)}},
                Outputs = new List<Throughput>{ new Throughput{Rate = 3000, Resource = Resource.GetResource(4)}},
            },
        };
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        for (int i = 0; i < this.Recipes.Count; i++)
        {
            Recipe recipe = this.Recipes[i];
            AddOption(this.RecipeOptionButton, recipe.Name, i);
        }

        this._on_Resource_Selected(0);
    }

    private void _on_Resource_Selected(int index)
    {
        int recipeIdx = (int)this.RecipeOptionButton.GetItemMetadata(index);
        Recipe recipe = this.Recipes[recipeIdx];

        this.UpdateRecipe(recipe);
    }
}
