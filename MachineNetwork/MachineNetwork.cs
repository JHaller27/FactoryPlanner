using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public class MachineNetwork
    {
        private static MachineNetwork _instance = null;
        public static MachineNetwork Instance => _instance ?? (_instance = new MachineNetwork());

        public static uint Precision { get; set;  }
        private IDictionary<int, IMachine> RandomAccessList { get; } = new Dictionary<int, IMachine>();
        private ISet<IMachine> Roots { get; } = new HashSet<IMachine>();
        private ISet<IMachine> Leaves { get; } = new HashSet<IMachine>();

        private MachineNetwork()
        {
        }

        public int AddMachine(IMachine machine)
        {
            int key = this.RandomAccessList.Count;
            this.RandomAccessList.Add(key, machine);
            this.Roots.Add(machine);
            this.Leaves.Add(machine);

            this.Recalculate();

            return key;
        }

        private IMachine GetMachine(int idx)
        {
            return this.RandomAccessList[idx];
        }

        public void ConnectMachines(IMachine from, int fromSlot, IMachine to, int toSlot)
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

        public void DisconnectMachines(IMachine from, int fromSlot, IMachine to, int toSlot)
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
            foreach (IMachine leaf in this.Leaves)
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
