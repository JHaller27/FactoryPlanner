using System;
using System.Collections.Generic;
using System.Linq;

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
