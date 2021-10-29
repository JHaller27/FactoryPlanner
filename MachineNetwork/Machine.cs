using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public class Machine
    {
        private IList<IThroughput> Inputs { get; } = new List<IThroughput>();
        public int CountInputs => this.Inputs.Count;

        private IList<IThroughput> Outputs { get; } = new List<IThroughput>();
        public int CountOutputs => this.Outputs.Count;

        private uint Efficiency { get; set; }
        public decimal EfficiencyPercentage => (decimal)this.Efficiency / 100;
        private decimal EfficiencyMult() => (decimal)this.Efficiency / (100 * MachineNetwork.Precision);

        public Machine(int numInputs, int numOutputs, string defaultResourceId)
        {
            for (int i = 0; i < numInputs; i++)
            {
                this.Inputs.Add(new Input(this, defaultResourceId));
            }
            for (int i = 0; i < numOutputs; i++)
            {
                this.Outputs.Add(new Output(this, defaultResourceId));
            }
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
        public bool TryGetInputSlot(int idx, out IThroughput input)
        {
            if (idx < this.Inputs.Count)
            {
                input = this.Inputs[idx];
                return true;
            }

            input = null;
            return false;
        }

        public bool HasOutputSlots() => this.Outputs.Any();
        public bool HasConnectedOutputs() => this.HasOutputSlots() && this.Outputs.Any(i => i.Neighbor != null);
        public bool HasDisconnectedOutputs() => this.HasOutputSlots() && this.Outputs.Any(i => i.Neighbor == null);

        public bool TryGetOutputSlot(int idx, out IThroughput output)
        {
            if (idx < this.Outputs.Count)
            {
                output = this.Outputs[idx];
                return true;
            }

            output = null;
            return false;
        }

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
                newEfficiency = 100 * MachineNetwork.Precision;
            }
            else if (this.HasDisconnectedInputs())
            {
                newEfficiency = 0;
            }
            else
            {
                newEfficiency = this.Inputs.Select(i => i.Efficiency()).Min();
            }

            this.Efficiency = newEfficiency;
            foreach (IThroughput output in this.Outputs)
            {
                output.SetEfficiency(this.EfficiencyMult());
            }
            this.Backfill();
        }

        private void Backfill()
        {
            this.Efficiency = this.Outputs.Any() ? this.Outputs.Select(o => o.Efficiency()).Min() : 100 * MachineNetwork.Precision;
            foreach (IThroughput input in this.Inputs)
            {
                input.SetEfficiency(this.EfficiencyMult());
            }

            foreach (Machine inputMachine in this.InputMachines())
            {
                inputMachine.Backfill();
            }
        }

        public void SetInput(int idx, string resourceId, uint capacity)
        {
            this.Inputs[idx].SetRecipe(capacity, resourceId);
        }

        public void SetOutput(int idx, string resourceId, uint capacity)
        {
            this.Outputs[idx].SetRecipe(capacity, resourceId);
        }

        public override string ToString()
        {
            return string.Join(", ", this.Inputs.Select(i => i.ToString())) + $":{this.EfficiencyPercentage:0.##}%:" + string.Join(", ", this.Outputs.Select(o => o.ToString()));
        }
    }
}
