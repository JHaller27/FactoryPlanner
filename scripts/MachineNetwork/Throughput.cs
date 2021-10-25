namespace FactoryPlanner.scripts.MachineNetwork
{
    public abstract class Throughput
    {
        public uint Capacity { get; }
        public uint Flow { get; set; }
        public uint Efficiency() => this.Flow * 100 * Machine.Precision / this.Capacity;
        public Throughput Neighbor { get; private set; }
        public Machine Parent { get; }

        protected Throughput(uint capacity, Machine parent)
        {
            this.Capacity = capacity;
            this.Parent = parent;

            this.Flow = this.Capacity;
        }

        public void SetNeighbor(Throughput neighbor)
        {
            this.Neighbor = neighbor;
        }

        public void SetFlow(uint efficiency)
        {
            this.Flow = this.Capacity * efficiency / 100 * Machine.Precision;

            if (this.Neighbor == null) return;
            if (this.Neighbor.Capacity >= this.Flow)
            {
                this.Neighbor.Flow = this.Flow;
                return;
            }

            this.Flow = this.Neighbor.Capacity;
            this.Neighbor.Flow = this.Flow;
        }

        public override string ToString()
        {
            return $"({this.Flow / Machine.Precision}/{this.Capacity / Machine.Precision})";
        }
    }

    public class Input : Throughput
    {
        public Input(uint capacity, Machine parent) : base(capacity, parent)
        {
        }
    }

    public class Output : Throughput
    {
        public Output(uint capacity, Machine parent) : base(capacity, parent)
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
