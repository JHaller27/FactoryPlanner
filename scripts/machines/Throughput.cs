using System;
using Godot;

namespace FactoryPlanner.scripts.machines
{
    public class Throughput
    {
        public int Rate { get; set; } = 0;  // In hundreths of Parts Per Minute
        public Resource Resource { get; set; } = Resource.Any;
        public MachineNode Neighbor { get; set; }

        public int TypeId => this.Resource.TypeId;
        public string Name => this.Resource.Name;
        public Color Color => this.Resource.Color;

        public string RateString => $"{this.Rate / 100:0.##}";

        public void SetResource(string id)
        {
            Resource resource = Resource.GetResource(id);
            this.Resource = resource;
        }
    }
}
