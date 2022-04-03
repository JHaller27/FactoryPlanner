using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public class MachineNetwork
    {
        private static MachineNetwork _instance = null;
        public static MachineNetwork Instance => _instance ?? (_instance = new MachineNetwork());

        public static uint Precision { get; set;  }
        private Dictionary<int, MachineBase> RandomAccessList { get; } = new();
        private Dictionary<MachineBase, int> ReverseRandomAccessList { get; } = new();

        private HashSet<MachineBase> Roots { get; } = new();
        private HashSet<MachineBase> Leaves { get; } = new();

        private MachineNetwork()
        {
        }

        public int AddMachine(MachineBase machine)
        {
            int key = this.RandomAccessList.Count;
            this.RandomAccessList.Add(key, machine);
            this.ReverseRandomAccessList.Add(machine, key);
            this.Roots.Add(machine);
            this.Leaves.Add(machine);

            this.Recalculate();

            return key;
        }

        public void RemoveMachine(MachineBase machineBase)
        {
            foreach (MachineBase neighbor in machineBase.DisconnectAllInputs())
            {
                this.DisconnectMachines(neighbor, machineBase);
            }
            foreach (MachineBase neighbor in machineBase.DisconnectAllOutputs())
            {
                this.DisconnectMachines(machineBase, neighbor);
            }

            int key = this.ReverseRandomAccessList[machineBase];
            this.ReverseRandomAccessList.Remove(machineBase);
            this.RandomAccessList.Remove(key);

            this.Roots.Remove(machineBase);
            this.Leaves.Remove(machineBase);

            this.Recalculate();
        }

        private MachineBase GetMachine(int idx)
        {
            return this.RandomAccessList[idx];
        }

        public void ConnectMachines(MachineBase from, int fromSlot, MachineBase to, int toSlot)
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

        private void DisconnectMachines(MachineBase from, MachineBase to)
        {
            if (!from.HasConnectedOutputs())
            {
                this.Leaves.Add(from);
            }
            if (!to.HasConnectedInputs())
            {
                this.Roots.Add(to);
            }
        }

        public void DisconnectMachines(MachineBase from, int fromSlot, MachineBase to, int toSlot)
        {
            from.DisconnectFrom(fromSlot, to, toSlot);
            this.DisconnectMachines(from, to);

            this.Recalculate();
        }

        public void DisconnectMachines(int fromId, int fromSlot, int toId, int toSlot)
        {
            this.DisconnectMachines(this.GetMachine(fromId), fromSlot, this.GetMachine(toId), toSlot);
        }

        private static IEnumerable<MachineBase> Traverse(IEnumerable<MachineBase> start)
        {
            Queue<MachineBase> queue = new(start.ToList());
            while (queue.Count > 0)
            {
                MachineBase curr = queue.Dequeue();
                yield return curr;

                foreach (MachineBase outputDestination in curr.GetOutputDestinations())
                {
                    queue.Enqueue(outputDestination);
                }
            }
        }

        public void Recalculate()
        {
            // Update roots first
            List<MachineBase> firstAfterRoots = new();
            foreach (MachineBase root in this.Roots)
            {
                root.Update();
                firstAfterRoots.AddRange(root.GetOutputDestinations());
            }

            bool needToRefresh;
            do
            {
                HashSet<MachineBase> seen = new();
                needToRefresh = false;

                List<MachineBase> machineOrder = Traverse(firstAfterRoots).ToList();
                foreach (MachineBase curr in machineOrder.Where(curr => !seen.Contains(curr)))
                {
                    seen.Add(curr);

                    needToRefresh = curr.Update();
                    if (needToRefresh) break;
                }
            } while (needToRefresh);
        }

        public override string ToString()
        {
            return string.Join("\n", this.Roots.Select(r => r.ToString()));
        }
    }
}
