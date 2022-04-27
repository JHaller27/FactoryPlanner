using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public class MachineNetwork
    {
        private const int RetryCount = 1024;

        private static MachineNetwork _instance;
        public static MachineNetwork Instance => _instance ??= new();

        private HashSet<MachineBase> AllMachines { get; } = new();

        private HashSet<MachineBase> Roots { get; } = new();
        private HashSet<MachineBase> Leaves { get; } = new();

        private MachineNetwork()
        {
        }

        public void AddMachine(MachineBase machine)
        {
            this.AllMachines.Add(machine);
            this.Roots.Add(machine);
            this.Leaves.Add(machine);

            this.Recalculate();
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

            this.AllMachines.Remove(machineBase);

            this.Roots.Remove(machineBase);
            this.Leaves.Remove(machineBase);

            this.Recalculate();
        }

        public void ConnectMachines(MachineBase from, int fromSlot, MachineBase to, int toSlot)
        {
            from.ConnectTo(fromSlot, to, toSlot);

            this.Leaves.Remove(from);
            this.Roots.Remove(to);

            this.Recalculate();
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
            int count = 0;
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
            } while (needToRefresh && count++ < RetryCount);

            if (needToRefresh)
            {
                Console.WriteLine("ERROR: Exceeded retry count while recalculating");
            }
        }

        public override string ToString()
        {
            return string.Join("\n", this.Roots.Select(r => r.ToString()));
        }
    }
}
