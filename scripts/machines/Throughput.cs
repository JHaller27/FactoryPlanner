using System;
using Godot;

namespace FactoryPlanner.scripts.machines
{
    public abstract class Throughput
    {
        public uint BaseRate { get; set; }  // In hundreths of Parts Per Minute
        public Resource Resource { get; set; } = Resource.Any;
        protected Throughput Neighbor { get; set; }

        public int TypeId => this.Resource.TypeId;
        public string Name => this.Resource.Name;
        public Color Color => this.Resource.Color;

        public string RateString => $"{this.EffectiveRate() / 100:0.##} / {this.BaseRate / 100:0.##}";

        public abstract uint EffectiveRate();
        public abstract uint CalculateEfficiency();

        public void SetNeighbor(Throughput neighbor)
        {
            this.Neighbor = neighbor;
        }

        public void SetResource(string id)
        {
            Resource resource = Resource.GetResource(id);
            this.Resource = resource;
        }
    }

    public class Input : Throughput
    {
        public override uint EffectiveRate()
        {
            return Math.Min(this.Neighbor?.EffectiveRate() ?? 0, this.BaseRate);
        }

        public override uint CalculateEfficiency()
        {
            return this.EffectiveRate() * 10000 / this.BaseRate;
        }
    }

    public class Output : Throughput
    {
        private MachineNode Parent { get; }
        private uint Efficiency => this.Parent.Efficiency;

        public Output(MachineNode parent)
        {
            this.Parent = parent;
        }

        public override uint EffectiveRate()
        {
            const uint divisor = 100 * 100; // 100 for percent, then 100 for precision
            return this.BaseRate * this.Efficiency / divisor;
        }

        public override uint CalculateEfficiency()
        {
            return this.Neighbor == null ? 10000 : this.Neighbor.EffectiveRate() * 10000 / this.BaseRate;
        }
    }
}
