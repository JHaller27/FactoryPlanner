using System;
using System.Collections.Generic;
using FactoryPlanner.scripts.resources;

namespace FactoryPlanner.scripts.machines
{
    public class Recipe
    {
        public string Id { get; private set; } = string.Empty;
        public string Name { get; set; }
        public IList<Throughput> Inputs { get; set; } = new List<Throughput>();
        public IList<Throughput> Outputs { get; set; } = new List<Throughput>();

        private static readonly IDictionary<string, Recipe> Recipes = new Dictionary<string, Recipe>();

        private static void AddRecipe(string key, Recipe recipe)
        {
            recipe.Id = key;
            Recipes.Add(key, recipe);
        }

        public static void LoadRecipes()
        {
            if (Recipes.Count != 0)
            {
                throw new Exception("Cannot re-load Recipe list");
            }

            AddRecipe("SmeltIronIngot", new Recipe
            {
                Name = "Iron Ingot",
                Inputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("IronOre") } },
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("IronIngot") } },
            });
            AddRecipe("SmeltCopperIngot", new Recipe
            {
                Name = "Copper Ingot",
                Inputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("CopperOre") } },
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("CopperIngot") } },
            });
        }

        public static Recipe GetRecipe(string id)
        {
            return Recipes[id];
        }
    }
}
