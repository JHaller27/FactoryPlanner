using System;
using Network = MachineNetwork.MachineNetwork;

namespace MachineNetwork
{
    public interface IThroughput
    {
        string ResourceId { get; }
        IThroughput Neighbor { get; }
        Machine Parent { get; }

        void SetNeighbor(IEfficientThroughput neighbor);
        uint SetFlow(uint flow);
        uint Efficiency();
        void SetRecipe(uint capacity, string resourceId);
        string RateString();
    }

    public interface IEfficientThroughput : IThroughput
    {
        void SetEfficiency(decimal efficiencyMult);
    }

    public abstract class EfficientThroughputBase : IEfficientThroughput
    {
        private uint Flow { get; set; }
        private uint Capacity { get; set; }
        public uint Efficiency() => this.Capacity == 0 ? 100 * MachineNetwork.Precision : this.Flow * 100 * MachineNetwork.Precision / this.Capacity;
        public string ResourceId { get; private set; }
        public IThroughput Neighbor { get; private set; }
        public Machine Parent { get; }

        protected EfficientThroughputBase(Machine parent, string resourceId)
        {
            this.Parent = parent;
            this.ResourceId = resourceId;
        }

        public void SetNeighbor(IEfficientThroughput neighbor)
        {
            this.Neighbor = neighbor;
        }

        public void SetEfficiency(decimal efficiencyMult)
        {
            this.Flow = (uint)(this.Capacity * efficiencyMult);

            if (this.Neighbor == null) return;
            this.Flow = this.Neighbor.SetFlow(this.Flow);
        }

        public uint SetFlow(uint flow)
        {
            this.Flow = Math.Min(this.Capacity, flow);
            return this.Flow;
        }

        public void SetRecipe(uint capacity, string resourceId)
        {
            this.Capacity = capacity;
            this.ResourceId = resourceId;
        }

        public string RateString()
        {
            return $"{this.Flow / Network.Precision:0.##} / {this.Capacity / Network.Precision:0.##}";
        }

        public override string ToString()
        {
            return $"({this.Flow / MachineNetwork.Precision}/{this.Capacity / MachineNetwork.Precision})";
        }
    }

    public class Input : EfficientThroughputBase
    {
        public Input(Machine parent, string resourceId) : base(parent, resourceId)
        {
        }
    }

    public class Output : EfficientThroughputBase
    {
        public Output(Machine parent, string resourceId) : base(parent, resourceId)
        {
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

    public class PassthroughThroughput : IThroughput
    {
        public string ResourceId { get; private set; }
        public IThroughput Neighbor { get; private set; }
        public Machine Parent { get; }
        private uint FlowRate { get; set; }

        public PassthroughThroughput(Machine parent, string resourceId)
        {
            this.Parent = parent;
            this.ResourceId = resourceId;
        }

        public void SetNeighbor(IEfficientThroughput neighbor)
        {
            this.Neighbor = neighbor;
        }

        public uint SetFlow(uint flow)
        {
            return this.FlowRate = flow;
        }

        public uint Efficiency()
        {
            return 100 * Network.Precision;
        }

        public void SetRecipe(uint capacity, string resourceId)
        {
            this.FlowRate = capacity;
            this.ResourceId = resourceId;
        }

        public string RateString()
        {
            return $"{this.FlowRate / Network.Precision:0.##}";
        }
    }
}
