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

        private Recipe Copy()
        {
            return new Recipe
            {
                Id = this.Id,
                Name = this.Name,
                Tags = this.Tags.ToHashSet(),
                Inputs = this.Inputs.Select(t => new Tuple<Resource, uint>(t.Item1, t.Item2)).ToList(),
                Outputs = this.Outputs.Select(t => new Tuple<Resource, uint>(t.Item1, t.Item2)).ToList(),
            };
        }

        public Recipe ModifyInputCapacity(int idx, Func<uint, uint> modFunc)
        {
            Recipe cpy = this.Copy();
            cpy.Inputs[idx] = new Tuple<Resource, uint>(cpy.Inputs[idx].Item1, modFunc(cpy.Inputs[idx].Item2));

            return cpy;
        }

        public Recipe ModifyOutputCapacity(int idx, Func<uint, uint> modFunc)
        {
            Recipe cpy = this.Copy();
            cpy.Outputs[idx] = new Tuple<Resource, uint>(cpy.Outputs[idx].Item1, modFunc(cpy.Outputs[idx].Item2));

            return cpy;
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
