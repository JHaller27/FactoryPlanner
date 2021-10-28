using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public class MachineNetwork
    {
        public static uint Precision { get; set;  }
        private IDictionary<int, Machine> RandomAccessList { get; } = new Dictionary<int, Machine>();
        private ISet<Machine> Roots { get; } = new HashSet<Machine>();
        private ISet<Machine> Leaves { get; } = new HashSet<Machine>();

        public int AddMachine(Machine machine)
        {
            int key = this.RandomAccessList.Count;
            this.RandomAccessList.Add(key, machine);
            this.Roots.Add(machine);
            this.Leaves.Add(machine);

            this.Recalculate();

            return key;
        }

        private Machine GetMachine(int idx)
        {
            return this.RandomAccessList[idx];
        }

        public void ConnectMachines(Machine from, int fromSlot, Machine to, int toSlot)
        {
            from.ConnectTo(fromSlot, to, toSlot);

            this.Leaves.Remove(from);
            this.Roots.Remove(to);

            this.Recalculate();
        }

        public void ConnectMachines(int fromId, int fromSlot, int toId, int toSlot)
        {
            this.ConnectMachines(this.GetMachine(fromId), fromSlot, this.GetMachine(toId), toSlot);
        }

        public void DisconnectMachines(Machine from, int fromSlot, Machine to, int toSlot)
        {
            from.DisconnectFrom(fromSlot, to, toSlot);

            if (!from.HasConnectedOutputs())
            {
                this.Leaves.Add(from);
            }
            if (!to.HasConnectedInputs())
            {
                this.Roots.Add(to);
            }

            this.Recalculate();
        }

        public void DisconnectMachines(int fromId, int fromSlot, int toId, int toSlot)
        {
            this.DisconnectMachines(this.GetMachine(fromId), fromSlot, this.GetMachine(toId), toSlot);
        }

        public void Recalculate()
        {
            foreach (Machine leaf in this.Leaves)
            {
                leaf.Update();
            }
        }

        public override string ToString()
        {
            return string.Join("\n", this.Roots.Select(r => r.ToString()));
        }
    }
}
