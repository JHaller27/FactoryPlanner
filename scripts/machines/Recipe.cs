using System.Collections.Generic;

namespace FactoryPlanner.scripts.machines
{
    public class Recipe
    {
        public string Name { get; set; }
        public IList<Throughput> Inputs { get; set; } = new List<Throughput>();
        public IList<Throughput> Outputs { get; set; } = new List<Throughput>();
    }
}
