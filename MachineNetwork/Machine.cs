using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public interface IMachine
    {
        int CountInputs();

        int CountOutputs();

        decimal EfficiencyPercentage { get; }

        void ConnectTo(int fromSlot, IMachine toMachine, int toSlot);

        void DisconnectFrom(int fromSlot, IMachine toMachine, int toSlot);

        bool HasConnectedInputs();
        bool TryGetInputSlot(int idx, out IThroughput input);

        bool HasConnectedOutputs();

        bool TryGetOutputSlot(int idx, out IThroughput output);

        void Update();
        void Backfill();

        void SetInput(int idx, string resourceId, uint capacity);

        void SetOutput(int idx, string resourceId, uint capacity);
    }

    public class Machine : IMachine
    {
        private IList<IEfficientThroughput> Inputs { get; } = new List<IEfficientThroughput>();
        public int CountInputs() => this.Inputs.Count;

        private IList<IEfficientThroughput> Outputs { get; } = new List<IEfficientThroughput>();
        public int CountOutputs() => this.Outputs.Count;

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

        public void ConnectTo(int fromSlot, IMachine toMachine, int toSlot)
        {
            this.TryGetOutputSlot(fromSlot, out IThroughput fromOutput);
            toMachine.TryGetInputSlot(toSlot, out IThroughput toInput);

            fromOutput.SetNeighbor(toInput);
            toInput.SetNeighbor(fromOutput);
        }

        public void DisconnectFrom(int fromSlot, IMachine toMachine, int toSlot)
        {
            this.TryGetOutputSlot(fromSlot, out IThroughput output);
            toMachine.TryGetInputSlot(toSlot, out IThroughput input);

            output.SetNeighbor(null);
            input.SetNeighbor(null);
        }

        private bool HasInputSlots() => this.Inputs.Any();
        public bool HasConnectedInputs() => this.HasInputSlots() && this.Inputs.Any(i => i.Neighbor != null);
        private bool HasDisconnectedInputs() => this.HasInputSlots() && this.Inputs.Any(i => i.Neighbor == null);
        public bool TryGetInputSlot(int idx, out IThroughput input)
        {
            if (idx < this.CountInputs())
            {
                input = this.Inputs[idx];
                return true;
            }

            input = null;
            return false;
        }

        private bool HasOutputSlots() => this.Outputs.Any();
        public bool HasConnectedOutputs() => this.HasOutputSlots() && this.Outputs.Any(i => i.Neighbor != null);

        public bool TryGetOutputSlot(int idx, out IThroughput output)
        {
            if (idx < this.CountOutputs())
            {
                output = this.Outputs[idx];
                return true;
            }

            output = null;
            return false;
        }

        private IEnumerable<IMachine> InputMachines() => this.Inputs
            .Select(i => i.Neighbor?.Parent)
            .Where(n => n != null);

        public void Update()
        {
            // Update my input machines (recursively)
            foreach (IMachine input in this.InputMachines())
            {
                input.Update();
            }

            // Determine my new efficiency based on my inputs
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

            // Update my outputs' flows with my new efficiency
            foreach (IEfficientThroughput output in this.Outputs)
            {
                output.SetEfficiency(this.EfficiencyMult());
            }

            // Update my efficiency again based on my outputs
            this.Backfill();
        }

        public void Backfill()
        {
            // Determine my new efficiency based on my outputs
            this.Efficiency = this.Outputs.Any() ? this.Outputs.Select(o => o.Efficiency()).Min() : 100 * MachineNetwork.Precision;

            // Update my inputs' flows with my new efficiency
            foreach (IEfficientThroughput input in this.Inputs)
            {
                input.SetEfficiency(this.EfficiencyMult());
            }

            // Update my input machines (recursively)
            foreach (IMachine inputMachine in this.InputMachines())
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
