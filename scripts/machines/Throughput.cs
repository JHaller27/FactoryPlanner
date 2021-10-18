using FactoryPlanner.scripts.resources;
using Godot;
using Resource = FactoryPlanner.scripts.resources.Resource;

namespace FactoryPlanner.scripts.machines
{
    public class Throughput
    {
        public int Rate { get; set; } = 0;  // In hundreths of Parts Per Minute
        public Resource Resource { get; set; } = Resource.GetResource(ResourceList.Any);

        public int TypeId => this.Resource.Id;
        public string Name => this.Resource.Name;
        public Color Color => this.Resource.Color;

        public string RateString => $"{this.Rate / 100:0.##}";

        public bool SetResource(ResourceList resourceEnum)
        {
            if (!Resource.TryGetResource(resourceEnum, out Resource resource)) return false;

            this.Resource = resource;
            return true;
        }
    }
}
