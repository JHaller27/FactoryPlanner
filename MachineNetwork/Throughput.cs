using System;
using Network = MachineNetwork.MachineNetwork;

namespace MachineNetwork
{
    public interface IThroughput
    {
        string ResourceId { get; }
        IThroughput Neighbor { get; }
        MachineBase GetParent();

        void SetNeighbor(IThroughput neighbor);
        bool HasNeighbor();
        decimal SetFlow(decimal flow);
        string RateString();
        bool IsAtCapacity();
    }

    public interface IEfficientThroughput : IThroughput
    {
        bool SetEfficiency(decimal efficiencyMult);
        decimal Efficiency();
        void SetRecipe(decimal capacity, string resourceId);
    }

    public abstract class EfficientThroughputBase : IEfficientThroughput
    {
        private readonly MachineBase Parent1;
        private decimal Flow { get; set; }
        private decimal Capacity { get; set; }
        public decimal Efficiency() => this.Capacity == 0 ? 1 : this.Flow / this.Capacity;
        public string ResourceId { get; private set; }
        public IThroughput Neighbor { get; private set; }

        public MachineBase GetParent()
        {
            return Parent1;
        }

        protected EfficientThroughputBase(MachineBase parent, string resourceId)
        {
            this.Parent1 = parent;
            this.ResourceId = resourceId;
        }

        public void SetNeighbor(IThroughput neighbor)
        {
            this.Neighbor = neighbor;
        }

        public bool HasNeighbor() => this.Neighbor != null;

        public bool SetEfficiency(decimal efficiencyMult)
        {
            decimal newFlow = Math.Round(this.Capacity * efficiencyMult, 2);
            if (newFlow == (uint)newFlow)
            {
                newFlow = (uint)newFlow;
            }

            this.Flow = newFlow;

            if (this.Neighbor == null) return true;

            decimal neighborNewFlow = this.Neighbor.SetFlow(this.Flow);
            bool canHandle = neighborNewFlow == this.Flow;
            this.Flow = neighborNewFlow;

            return canHandle;
        }

        public decimal SetFlow(decimal flow)
        {
            this.Flow = Math.Min(this.Capacity, flow);
            return this.Flow;
        }

        public void SetRecipe(decimal capacity, string resourceId)
        {
            this.Capacity = capacity;
            this.ResourceId = resourceId;
        }

        public string RateString()
        {
            return $"{this.Flow:0.##} / {this.Capacity:0.##}";
        }

        public bool IsAtCapacity()
        {
            return this.Flow == this.Capacity;
        }

        public override string ToString()
        {
            return $"({this.Flow}/{this.Capacity})";
        }
    }

    public class Input : EfficientThroughputBase
    {
        public Input(MachineBase parent, string resourceId) : base(parent, resourceId)
        {
        }
    }

    public class Output : EfficientThroughputBase
    {
        public Output(MachineBase parent, string resourceId) : base(parent, resourceId)
        {
        }

        public override string ToString()
        {
            string s = base.ToString();
            if (this.Neighbor != null)
            {
                s += " -> " + this.Neighbor.GetParent();
            }

            return s;
        }
    }

    public class PassthroughThroughput : IThroughput
    {
        private Balancer Parent { get; }
        public string ResourceId { get; private set; }
        public IThroughput Neighbor { get; private set; }

        public MachineBase GetParent()
        {
            return Parent;
        }

        public decimal FlowRate { get; set; }

        public PassthroughThroughput(Balancer parent, string resourceId)
        {
            this.Parent = parent;
            this.ResourceId = resourceId;
        }

        public void SetNeighbor(IThroughput neighbor)
        {
            this.Neighbor = neighbor;
            this.Parent.UpdateResource();
        }

        public bool HasNeighbor() => this.Neighbor != null;

        public decimal SetFlow(decimal flow)
        {
            return this.FlowRate = flow;
        }

        public string RateString()
        {
            return $"{this.FlowRate:0.##}";
        }

        public bool IsAtCapacity()
        {
            return true;
        }

        public string RateString(bool showNeighbor)
        {
            string s = this.RateString();

            if (showNeighbor && this.Neighbor != null)
            {
                s += " -> " + this.Neighbor.GetParent();
            }

            return s;
        }

        public void SetResource(string resourceId)
        {
            this.ResourceId = resourceId;
        }
    }
}
