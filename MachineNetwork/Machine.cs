using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public interface IMachine
    {
        IList<IThroughput> Inputs { get; }
        int CountInputs { get; }

        IList<IThroughput> Outputs { get; }
        int CountOutputs { get; }

        decimal EfficiencyPercentage { get; }

        void ConnectTo(int fromSlot, IMachine toMachine, int toSlot);

        void DisconnectFrom(int fromSlot, IMachine toMachine, int toSlot);

        bool HasInputSlots();
        bool HasConnectedInputs();
        bool HasDisconnectedInputs();
        bool TryGetInputSlot(int idx, out IThroughput input);

        bool HasOutputSlots();
        bool HasConnectedOutputs();
        bool HasDisconnectedOutputs();

        bool TryGetOutputSlot(int idx, out IThroughput output);

        IEnumerable<IMachine> InputMachines();
        IEnumerable<IMachine> OutputMachines();

        void Update();
        void Backfill();

        void SetInput(int idx, string resourceId, uint capacity);

        void SetOutput(int idx, string resourceId, uint capacity);
    }

    public class Machine : IMachine
    {
        public IList<IThroughput> Inputs { get; } = new List<IThroughput>();
        public int CountInputs => this.Inputs.Count;

        public IList<IThroughput> Outputs { get; } = new List<IThroughput>();
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

        public void ConnectTo(int fromSlot, IMachine toMachine, int toSlot)
        {
            this.Outputs[fromSlot].SetNeighbor(toMachine.Inputs[toSlot]);
            toMachine.Inputs[toSlot].SetNeighbor(this.Outputs[fromSlot]);
        }

        public void DisconnectFrom(int fromSlot, IMachine toMachine, int toSlot)
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

        public IEnumerable<IMachine> InputMachines() => this.Inputs
            .Select(i => i.Neighbor?.Parent)
            .Where(n => n != null);
        public IEnumerable<IMachine> OutputMachines() => this.Outputs
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
