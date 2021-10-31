using System.Collections.Generic;
using System.Linq;

namespace MachineNetwork
{
    public abstract class MachineBase
    {
        public abstract int CountInputs();
        public abstract int CountOutputs();

        public void ConnectTo(int fromSlot, MachineBase toMachine, int toSlot)
        {
            this.TryGetOutputSlot(fromSlot, out IThroughput fromOutput);
            toMachine.TryGetInputSlot(toSlot, out IThroughput toInput);

            fromOutput.SetNeighbor(toInput);
            toInput.SetNeighbor(fromOutput);
        }

        public void DisconnectFrom(int fromSlot, MachineBase toMachine, int toSlot)
        {
            this.TryGetOutputSlot(fromSlot, out IThroughput output);
            toMachine.TryGetInputSlot(toSlot, out IThroughput input);

            output.SetNeighbor(null);
            input.SetNeighbor(null);
        }

        public abstract bool HasConnectedInputs();
        public abstract bool TryGetInputSlot(int idx, out IThroughput input);

        public abstract bool HasConnectedOutputs();
        public abstract bool TryGetOutputSlot(int idx, out IThroughput output);

        public abstract void Update();
        public abstract void Backfill();
    }

    public class EfficientMachine : MachineBase
    {
        private IList<IEfficientThroughput> Inputs { get; } = new List<IEfficientThroughput>();
        public override int CountInputs() => this.Inputs.Count;

        private IList<IEfficientThroughput> Outputs { get; } = new List<IEfficientThroughput>();
        public override int CountOutputs() => this.Outputs.Count;

        private uint Efficiency { get; set; }
        public decimal GetEfficiencyPercentage() => (decimal)this.Efficiency / 100;
        private decimal EfficiencyMult() => (decimal)this.Efficiency / (100 * MachineNetwork.Precision);

        public EfficientMachine(int numInputs, int numOutputs, string defaultResourceId)
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

        private bool HasInputSlots() => this.Inputs.Any();
        public override bool HasConnectedInputs() => this.HasInputSlots() && this.Inputs.Any(i => i.Neighbor != null);
        private bool HasDisconnectedInputs() => this.HasInputSlots() && this.Inputs.Any(i => i.Neighbor == null);
        public override bool TryGetInputSlot(int idx, out IThroughput input)
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
        public override bool HasConnectedOutputs() => this.HasOutputSlots() && this.Outputs.Any(i => i.Neighbor != null);

        public override bool TryGetOutputSlot(int idx, out IThroughput output)
        {
            if (idx < this.CountOutputs())
            {
                output = this.Outputs[idx];
                return true;
            }

            output = null;
            return false;
        }

        private IEnumerable<MachineBase> InputMachines() => this.Inputs
            .Select(i => i.Neighbor?.Parent)
            .Where(n => n != null);

        public override void Update()
        {
            // Update my input machines (recursively)
            foreach (MachineBase input in this.InputMachines())
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

        public override void Backfill()
        {
            // Determine my new efficiency based on my outputs
            this.Efficiency = this.Outputs.Any() ? this.Outputs.Select(o => o.Efficiency()).Min() : 100 * MachineNetwork.Precision;

            // Update my inputs' flows with my new efficiency
            foreach (IEfficientThroughput input in this.Inputs)
            {
                input.SetEfficiency(this.EfficiencyMult());
            }

            // Update my input machines (recursively)
            foreach (MachineBase inputMachine in this.InputMachines())
            {
                inputMachine.Backfill();
            }
        }

        public void SetInputRecipe(int idx, string resourceId, uint capacity)
        {
            this.Inputs[idx].SetRecipe(capacity, resourceId);
        }

        public void SetOutputRecipe(int idx, string resourceId, uint capacity)
        {
            this.Outputs[idx].SetRecipe(capacity, resourceId);
        }

        public override string ToString()
        {
            return string.Join(", ", this.Inputs.Select(i => i.ToString())) + $":{this.GetEfficiencyPercentage():0.##}%:" + string.Join(", ", this.Outputs.Select(o => o.ToString()));
        }
    }

        // private IList<IThroughput> Inputs { get; } = new List<IThroughput>();
        // private IList<IThroughput> Outputs { get; } = new List<IThroughput>();

    public class Balancer : MachineBase
    {
        private IList<PassthroughThroughput> Inputs { get; } = new List<PassthroughThroughput>();
        private IList<PassthroughThroughput> Outputs { get; } = new List<PassthroughThroughput>();

        public override int CountInputs() => this.Inputs.Count;
        public override int CountOutputs() => this.Outputs.Count;

        private int CountConnectedInputs() => this.Inputs.Count(i => i.HasNeighbor());
        private int CountConnectedOutputs() => this.Outputs.Count(o => o.HasNeighbor());

        public Balancer(int numInputs, int numOutputs, string defaultResourceId)
        {
            for (int i = 0; i < numInputs; i++)
            {
                this.Inputs.Add(new PassthroughThroughput(this, defaultResourceId));
            }
            for (int i = 0; i < numOutputs; i++)
            {
                this.Outputs.Add(new PassthroughThroughput(this, defaultResourceId));
            }
        }

        private bool HasInputSlots() => this.Inputs.Any();
        public override bool HasConnectedInputs() => this.HasInputSlots() && this.Inputs.Any(i => i.Neighbor != null);

        public override bool TryGetInputSlot(int idx, out IThroughput input)
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
        public override bool HasConnectedOutputs() => this.HasOutputSlots() && this.Outputs.Any(i => i.Neighbor != null);

        public override bool TryGetOutputSlot(int idx, out IThroughput output)
        {
            if (idx < this.CountOutputs())
            {
                output = this.Outputs[idx];
                return true;
            }

            output = null;
            return false;
        }

        private IEnumerable<MachineBase> InputMachines() => this.Inputs
            .Select(i => i.Neighbor?.Parent)
            .Where(n => n != null);

        public override void Update()
        {
            // Update my input machines (recursively)
            foreach (MachineBase input in this.InputMachines())
            {
                input.Update();
            }

            // Get new output flow based on total input flow & number of connected neighbors
            uint totalInputFlow = this.Inputs.Aggregate<PassthroughThroughput, uint>(0, (current, input) => current + input.FlowRate);
            bool hasNoConnectedOutput = this.CountConnectedOutputs() == 0;
            uint eachOutputFlow = hasNoConnectedOutput ? (uint)(totalInputFlow / this.CountOutputs()) : (uint)(totalInputFlow / this.CountConnectedOutputs());

            // Update my outputs' flows
            foreach (IThroughput output in this.Outputs)
            {
                if (hasNoConnectedOutput)
                {
                    output.SetFlow(eachOutputFlow);
                }
                else if (output.HasNeighbor())
                {
                    output.SetFlow(eachOutputFlow);
                    output.Neighbor.SetFlow(eachOutputFlow);
                }
                else
                {
                    output.SetFlow(0);
                }
            }

            // Update my efficiency again based on my outputs
            this.Backfill();
        }

        public override void Backfill()
        {
            // Get new input flow based on total output flow & number of connected neighbors
            uint totalOutputFlow = this.Outputs.Aggregate<PassthroughThroughput, uint>(0, (current, input) => current + input.FlowRate);
            uint eachInputFlow = this.CountConnectedInputs() == 0 ? 0 : (uint)(totalOutputFlow / this.CountConnectedInputs());

            // Update my inputs' flows with my new efficiency
            foreach (IThroughput input in this.Inputs)
            {
                if (input.HasNeighbor())
                {
                    input.SetFlow(eachInputFlow);
                    input.Neighbor.SetFlow(eachInputFlow);
                }
                else
                {
                    input.SetFlow(0);
                }
            }

            // Update my input machines (recursively)
            foreach (MachineBase inputMachine in this.InputMachines())
            {
                inputMachine.Backfill();
            }
        }

        public override string ToString()
        {
            return string.Concat("[", string.Join("+", this.Inputs.Select(i => i.RateString(false))), "]:[",
                string.Join("+", this.Outputs.Select(i => i.RateString(true))), "]");
        }
    }
}
