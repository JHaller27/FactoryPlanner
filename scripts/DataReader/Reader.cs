using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using MachineResource = FactoryPlanner.scripts.machines.Resource;
using MachineRecipe = FactoryPlanner.scripts.machines.Recipe;

namespace FactoryPlanner.DataReader
{
    public static class Reader
    {
        public static void LoadData(string path)
        {
            File file = new File();
            file.Open(path, File.ModeFlags.Read);

            try
            {
                string text = file.GetAsText();
                ResourceFileData fileData = JsonConvert.DeserializeObject<ResourceFileData>(text);
                Debug.Assert(fileData != null, nameof(fileData) + " != null");

                LoadResources(fileData.Resources);
                LoadRecipes(fileData.Recipes);
            }
            finally
            {
                file.Close();
            }
        }

        private static void LoadResources(IEnumerable<Resource> resources)
        {
            foreach (Resource resource in resources)
            {
                scripts.machines.Resource.AddResource(new scripts.machines.Resource(resource.Name, color: Color.ColorN(resource.Color)));
            }
        }

        private static void LoadRecipes(IEnumerable<Recipe> recipes)
        {
            foreach (Recipe recipe in recipes)
            {
                scripts.machines.Recipe factoryRecipe = new scripts.machines.Recipe
                {
                    Name = recipe.Name,
                    Tags = recipe.Tags.ToHashSet(),
                };
                if (recipe.Inputs != null)
                {
                    factoryRecipe.Inputs = recipe.Inputs.Select(i => new scripts.machines.Input(0, null)
                    {
                        Capacity = i.Rate,
                        Resource = scripts.machines.Resource.GetResource(i.Resource),
                    } as scripts.machines.Throughput).ToList();
                }
                if (recipe.Outputs != null)
                {
                    factoryRecipe.Outputs = recipe.Outputs.Select(o => new scripts.machines.Output(0, null)
                    {
                        Capacity = o.Rate,
                        Resource = scripts.machines.Resource.GetResource(o.Resource),
                    } as scripts.machines.Throughput).ToList();
                }

                scripts.machines.Recipe.AddRecipe(recipe.Key, factoryRecipe);
            }
        }
    }
}
