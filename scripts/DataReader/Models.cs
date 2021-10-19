using System.Collections.Generic;

namespace FactoryPlanner.DataReader
{
    public class Resource
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class Throughput
    {
        public string Resource { get; set; }
        public int Rate { get; set; }
    }

    public class Recipe
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<Throughput> Inputs { get; set; }
        public IEnumerable<Throughput> Outputs { get; set; }
    }

    public class ResourceFileData
    {
        public IEnumerable<Resource> Resources { get; set; }
        public IEnumerable<Recipe> Recipes { get; set; }
    }
}
