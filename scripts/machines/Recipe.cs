using System.Collections.Generic;
using FactoryPlanner.scripts.resources;

namespace FactoryPlanner.scripts.machines
{
    public class Recipe
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public IList<Throughput> Inputs { get; set; } = new List<Throughput>();
        public IList<Throughput> Outputs { get; set; } = new List<Throughput>();

        private static readonly IList<Recipe> Recipes = new List<Recipe>();

        private static void AddRecipe(Recipe resource)
        {
            resource.Id = Recipes.Count;
            Recipes.Add(resource);
        }

        public static void LoadRecipes()
        {
            AddRecipe(new Recipe
            {
                Name = "Iron Ingot",
                Inputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource(1) } },
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource(3) } },
            });
            AddRecipe(new Recipe
            {
                Name = "Copper Ingot",
                Inputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource(2) } },
                Outputs = new List<Throughput> { new Throughput { Rate = 3000, Resource = Resource.GetResource(4) } },
            });
        }

        public static Recipe GetRecipe(int idx)
        {
            return Recipes[idx];
        }
    }
}
