using FactoryPlanner.scripts.resources;
using Godot;
using Resource = FactoryPlanner.scripts.resources.Resource;

namespace FactoryPlanner.scripts.machines
{
    public class Throughput
    {
        public int Rate { get; set; } = 0;  // In hundreths of Parts Per Minute
        public Resource Resource { get; set; } = Resource.Any;

        public int TypeId => this.Resource.Id;
        public string Name => this.Resource.Name;
        public Color Color => this.Resource.Color;

        public string RateString => $"{this.Rate / 100:0.##}";

        public void SetResource(int idx)
        {
            Resource resource = Resource.GetResource(idx);
            this.Resource = resource;
        }
    }
}
