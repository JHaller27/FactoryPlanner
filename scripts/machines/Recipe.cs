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

        private IList<Tuple<Resource, uint>> Inputs = new List<Tuple<Resource, uint>>();
        public IEnumerable<Tuple<int, string, uint>> ListInputs() => ListThroughput(this.Inputs);

        private IList<Tuple<Resource, uint>> Outputs = new List<Tuple<Resource, uint>>();
        public IEnumerable<Tuple<int, string, uint>> ListOutputs() => ListThroughput(this.Outputs);

        private static IEnumerable<Tuple<int, string, uint>> ListThroughput(IEnumerable<Tuple<Resource, uint>> resources)
        {
            Tuple<Resource,uint>[] resourceArray = resources as Tuple<Resource, uint>[] ?? resources.ToArray();
            for (int i = 0; i < resourceArray.Count(); i++)
            {
                yield return new Tuple<int, string, uint>(i, resourceArray[i].Item1.Id, resourceArray[i].Item2);
            }
        }

        public void AddInputResource(Resource resource, uint capacity)
        {
            this.Inputs.Add(new Tuple<Resource, uint>(resource, capacity));
        }

        public void AddOutputResource(Resource resource, uint capacity)
        {
            this.Outputs.Add(new Tuple<Resource, uint>(resource, capacity));
        }

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
