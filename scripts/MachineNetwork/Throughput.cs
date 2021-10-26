namespace FactoryPlanner.scripts.MachineNetwork
{
    public abstract class Throughput
    {
        public uint Flow { get; private set; }
        public uint Capacity { get; private set; }
        public uint Efficiency() => this.Flow * 100 * Utils.Precision / this.Capacity;
        public string ResourceId { get; private set; }
        public Throughput Neighbor { get; private set; }
        public Machine Parent { get; }

        protected Throughput(Machine parent, string resourceId)
        {
            this.Parent = parent;
            this.ResourceId = resourceId;
        }

        public void SetNeighbor(Throughput neighbor)
        {
            this.Neighbor = neighbor;
        }

        public void SetFlow(decimal efficiencyMult)
        {
            this.Flow = (uint)(this.Capacity * efficiencyMult);

            if (this.Neighbor == null) return;
            if (this.Neighbor.Capacity >= this.Flow)
            {
                this.Neighbor.Flow = this.Flow;
                return;
            }

            this.Flow = this.Neighbor.Capacity;
            this.Neighbor.Flow = this.Flow;
        }

        public void SetRecipe(uint capacity, string resourceId)
        {
            this.Capacity = capacity;
            this.ResourceId = resourceId;
        }

        public override string ToString()
        {
            return $"({this.Flow / Utils.Precision}/{this.Capacity / Utils.Precision})";
        }
    }

    public class Input : Throughput
    {
        public Input(Machine parent, string resourceId) : base(parent, resourceId)
        {
        }
    }

    public class Output : Throughput
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
}
