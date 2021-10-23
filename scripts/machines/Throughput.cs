using System;
using Godot;

namespace FactoryPlanner.scripts.machines
{
    public abstract class Throughput
    {
        public Resource Resource { get; set; } = Resource.Any;

        public uint Capacity { get; set; }
        public uint Flow { get; set; }
        public Throughput Neighbor { get; private set; }
        public MachineNode Parent { get; }

        public uint Efficiency() => this.Flow * 100 * Utils.Precision / this.Capacity;

        public int TypeId => this.Resource.TypeId;
        public string Name => this.Resource.Name;
        public Color Color => this.Resource.Color;

        public string RateString => $"{this.Flow / Utils.Precision:0.##} / {this.Capacity / Utils.Precision:0.##}";

        protected Throughput(uint capacity, MachineNode parent)
        {
            this.Capacity = capacity;
            this.Parent = parent;

            this.Flow = this.Capacity;
        }

        public void SetNeighbor(Throughput neighbor)
        {
            this.Neighbor = neighbor;
        }

        public virtual void CalculateFlow(decimal efficiencyMult)
        {
            this.Flow = (uint)(this.Capacity * efficiencyMult);
        }

        public void SetResource(string id)
        {
            Resource resource = Resource.GetResource(id);
            this.Resource = resource;
        }

        public override string ToString() => this.RateString;
    }

    public class Input : Throughput
    {
        public Input(uint capacity, MachineNode parent) : base(capacity, parent)
        {
        }

        public override void CalculateFlow(decimal efficiencyMult)
        {
            base.CalculateFlow(efficiencyMult);

            if (this.Neighbor == null) return;

            this.Neighbor.Flow = this.Flow;
            this.Neighbor.Parent.UpdateFlowFromOutputs();
        }
    }

    public class Output : Throughput
    {
        public Output(uint capacity, MachineNode parent) : base(capacity, parent)
        {
        }

        public override void CalculateFlow(decimal efficiencyMult)
        {
            base.CalculateFlow(efficiencyMult);
            if (this.Neighbor == null) return;

            if (this.Neighbor.Capacity >= this.Flow)
            {
                this.Neighbor.Flow = this.Flow;
                return;
            }

            this.Flow = this.Neighbor.Capacity;
            this.Neighbor.Flow = this.Neighbor.Capacity;
            this.Parent.UpdateFlowFromOutputs();
        }

        public override string ToString()
        {
            string s = base.ToString();
            if (this.Neighbor != null)
            {
                s += " -> " + this.Neighbor.Parent;
            }

            return s;
        }
    }
}
