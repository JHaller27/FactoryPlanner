using System;
using System.Collections.Generic;
using System.Linq;
using FactoryPlanner.scripts.resources;

namespace FactoryPlanner.scripts.machines
{
    public class Recipe
    {
        public string Id { get; private set; } = string.Empty;
        public string Name { get; set; }
        public ISet<string> Tags { get; set; } = new HashSet<string>();
        public IList<Throughput> Inputs { get; set; } = new List<Throughput>();
        public IList<Throughput> Outputs { get; set; } = new List<Throughput>();

        private static readonly IDictionary<string, Recipe> Recipes = new Dictionary<string, Recipe>();

        public static void AddRecipe(string key, Recipe recipe)
        {
            recipe.Id = key;
            Recipes.Add(key, recipe);
        }

        [Obsolete]
        public static void LoadRecipes()
        {
            if (Recipes.Count != 0)
            {
                throw new Exception("Cannot re-load Recipe list");
            }

            AddRecipe("MineIronOre", new Recipe
            {
                Name = "Iron Ore",
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("Iron Ore") } },
                Tags = new HashSet<string>{"Miner"},
            });
            AddRecipe("MineCopperOre", new Recipe
            {
                Name = "Copper Ore",
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("Copper Ore") } },
                Tags = new HashSet<string>{"Miner"},
            });
            AddRecipe("SmeltIronIngot", new Recipe
            {
                Name = "Iron Ingot",
                Inputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("Iron Ore") } },
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("Iron Ingot") } },
                Tags = new HashSet<string>{"Smelter"},
            });
            AddRecipe("SmeltCopperIngot", new Recipe
            {
                Name = "Copper Ingot",
                Inputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("Copper Ore") } },
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource("Copper Ingot") } },
                Tags = new HashSet<string>{"Smelter"},
            });
        }

        public static IEnumerable<Recipe> GetRecipesWithTags(params string[] tags)
        {
            return Recipes.Values
                .Where(r => tags.All(t => r.Tags.Contains(t)));
        }

        public static Recipe GetRecipe(string id)
        {
            return Recipes[id];
        }
    }
}
