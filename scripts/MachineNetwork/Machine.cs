using System.Collections.Generic;
using System.Linq;

namespace FactoryPlanner.scripts.MachineNetwork
{
    public class Machine
    {
        public const uint Precision = 100;

        private IList<Input> Inputs { get; }
        private IList<Output> Outputs { get; }
        private uint Efficiency { get; set; }
        private decimal EfficiencyPercentage => (decimal)this.Efficiency / 100;
        private decimal EfficiencyMult() => (decimal)this.Efficiency / (100 * Machine.Precision);

        public Machine(IEnumerable<int> inputCapacities, IEnumerable<int> outputCapacities)
        {
            this.Inputs = inputCapacities.Select(c => new Input((uint)c * Machine.Precision, this)).ToList();
            this.Outputs = outputCapacities.Select(c => new Output((uint)c * Machine.Precision, this)).ToList();
        }

        public void ConnectTo(int fromSlot, Machine toMachine, int toSlot)
        {
            this.Outputs[fromSlot].SetNeighbor(toMachine.Inputs[toSlot]);
            toMachine.Inputs[toSlot].SetNeighbor(this.Outputs[fromSlot]);
        }

        public void DisconnectFrom(int fromSlot, Machine toMachine, int toSlot)
        {
            this.Outputs[fromSlot].SetNeighbor(null);
            toMachine.Inputs[toSlot].SetNeighbor(null);
        }

        public bool HasInputSlots() => this.Inputs.Any();
        public bool HasConnectedInputs() => this.HasInputSlots() && this.Inputs.Any(i => i.Neighbor != null);
        public bool HasDisconnectedInputs() => this.HasInputSlots() && this.Inputs.Any(i => i.Neighbor == null);

        public bool HasOutputSlots() => this.Outputs.Any();
        public bool HasConnectedOutputs() => this.HasOutputSlots() && this.Outputs.Any(i => i.Neighbor != null);
        public bool HasDisconnectedOutputs() => this.HasOutputSlots() && this.Outputs.Any(i => i.Neighbor == null);

        public IEnumerable<Machine> InputMachines() => this.Inputs
            .Select(i => i.Neighbor?.Parent)
            .Where(n => n != null);
        public IEnumerable<Machine> OutputMachines() => this.Outputs
            .Select(i => i.Neighbor?.Parent)
            .Where(n => n != null);

        public void Update()
        {
            foreach (Machine input in this.InputMachines())
            {
                input.Update();
            }

            uint newEfficiency;
            if (!this.HasInputSlots())
            {
                newEfficiency = 100 * Machine.Precision;
            }
            else if (this.HasDisconnectedInputs())
            {
                newEfficiency = 0;
            }
            else
            {
                newEfficiency = this.Inputs.Select(i => i.Efficiency()).Min();
            }

            if (newEfficiency == this.Efficiency) return;

            this.Efficiency = newEfficiency;
            foreach (Output output in this.Outputs)
            {
                output.SetFlow(this.Efficiency);
            }
            this.Backfill();
        }

        private void Backfill()
        {
            this.Efficiency = this.Outputs.Select(o => o.Efficiency()).Min();
            foreach (Input input in this.Inputs)
            {
                input.SetFlow(this.Efficiency);
            }

            foreach (Machine inputMachine in this.InputMachines())
            {
                inputMachine.Backfill();
            }
        }

        public override string ToString()
        {
            return string.Join(", ", this.Inputs.Select(i => i.ToString())) + $":{this.EfficiencyPercentage:0.##}%:" + string.Join(", ", this.Outputs.Select(o => o.ToString()));
        }
    }
}
