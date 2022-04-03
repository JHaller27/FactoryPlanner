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

        public IEnumerable<MachineBase> DisconnectAllOutputs()
        {
            for (int i = 0; i < this.CountOutputs(); i++)
            {
                if (!this.TryGetOutputSlot(i, out IThroughput output)) continue;

                output.SetNeighbor(null);
                yield return output.GetParent();
            }
        }

        public IEnumerable<MachineBase> DisconnectAllInputs()
        {
            for (int i = 0; i < this.CountOutputs(); i++)
            {
                if (!this.TryGetOutputSlot(i, out IThroughput output)) continue;

                output.SetNeighbor(null);
                yield return output.GetParent();
            }
        }

        public IEnumerable<MachineBase> GetOutputDestinations()
        {
            for (int i = 0; i < this.CountOutputs(); i++)
            {
                if (!this.TryGetOutputSlot(i, out IThroughput output)) continue;
                IThroughput input = output.Neighbor;

                if (input == null) continue;

                yield return input.GetParent();
            }
        }

        public abstract bool HasConnectedInputs();
        public abstract bool TryGetInputSlot(int idx, out IThroughput input);

        public abstract bool HasConnectedOutputs();
        public abstract bool TryGetOutputSlot(int idx, out IThroughput output);

        // Returns true if need to re-update, false otherwise
        public abstract bool Update();

        internal abstract void ReverseUpdate();
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
            .Select(i => i.Neighbor?.GetParent())
            .Where(n => n != null);

        public override bool Update()
        {
            // Determine my new efficiency based on my inputs
            if (!this.HasInputSlots())
            {
                this.Efficiency = 100 * MachineNetwork.Precision;
            }
            else if (this.HasDisconnectedInputs())
            {
                this.Efficiency = 0;
            }
            else
            {
                this.Efficiency = this.Inputs.Select(i => i.Efficiency()).Min();
            }

            // Update my outputs' flows with my new efficiency
            foreach (IEfficientThroughput output in this.Outputs)
            {
                bool canHandle = output.SetEfficiency(this.EfficiencyMult());
                if (canHandle) continue;

                this.ReverseUpdate();
                return true;
            }

            return false;
        }

        internal override void ReverseUpdate()
        {
            // To get here, this must have been called by an output-neighbor
            // Thus, we are guaranteed to have output-neighbors which already have an up-to-date efficiency
            this.Efficiency = this.Outputs.Select(i => i.Efficiency()).Min();

            // No need to update outputs' efficiency - that will be handled by the MachineNetwork

            foreach (IEfficientThroughput input in this.Inputs)
            {
                input.SetEfficiency(this.EfficiencyMult());
            }

            foreach (MachineBase inputMachine in this.InputMachines())
            {
                inputMachine.ReverseUpdate();
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

    public class Balancer : MachineBase
    {
        private IList<PassthroughThroughput> Inputs { get; } = new List<PassthroughThroughput>();
        private IList<PassthroughThroughput> Outputs { get; } = new List<PassthroughThroughput>();

        private string DefaultResourceId { get; }

        public override int CountInputs() => this.Inputs.Count;
        public override int CountOutputs() => this.Outputs.Count;

        private int CountConnectedInputs() => this.Inputs.Count(i => i.HasNeighbor());
        private int CountConnectedOutputs() => this.Outputs.Count(o => o.HasNeighbor());

        public Balancer(int numInputs, int numOutputs, string defaultResourceId)
        {
            this.DefaultResourceId = defaultResourceId;

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
            .Select(i => i.Neighbor?.GetParent())
            .Where(n => n != null);

        public void UpdateResource()
        {
            // Get the first ResourceId, error if any other slot has a different resource (this should be blocked by Godot)
            string resourceId = (this.Inputs.FirstOrDefault(i => i.HasNeighbor()) ?? this.Outputs.FirstOrDefault(o => o.HasNeighbor()))?.Neighbor.ResourceId ?? this.DefaultResourceId;

            foreach (PassthroughThroughput input in this.Inputs)
            {
                input.SetResource(resourceId);
            }

            foreach (PassthroughThroughput output in this.Outputs)
            {
                output.SetResource(resourceId);
            }
        }

        public override bool Update()
        {
            // Get new output flow based on total input flow & number of connected neighbors
            uint totalInputFlow = this.Inputs.Aggregate<PassthroughThroughput, uint>(0, (current, input) => current + input.FlowRate);
            bool hasNoConnectedOutput = this.CountConnectedOutputs() == 0;
            uint eachOutputFlow = hasNoConnectedOutput ? totalInputFlow : (uint)(totalInputFlow / this.CountConnectedOutputs());

            // Update my outputs' flows
            bool first = true;
            foreach (IThroughput output in this.Outputs)
            {
                if (hasNoConnectedOutput && first)
                {
                    output.SetFlow(eachOutputFlow);
                }
                else if (output.HasNeighbor())
                {
                    uint newFlow = output.Neighbor.SetFlow(eachOutputFlow);
                    // We can ignore the output (new flow) since our own outputs will always be able to handle anything we give them
                    _ = output.SetFlow(newFlow);

                    bool canHandle = newFlow == eachOutputFlow;
                    if (!canHandle)
                    {
                        this.ReverseUpdate();
                        return true;
                    }
                }
                else
                {
                    output.SetFlow(0);
                }

                first = false;
            }

            return false;
        }

        internal override void ReverseUpdate()
        {
            // To get here, this must have been called by an output-neighbor
            // Thus, we are guaranteed to have output-neighbors which already have an up-to-date efficiency
            uint totalOutputFlow = this.Outputs.Select(o => o.FlowRate).Aggregate((curr, acc) => curr + acc);

            // First, try to redistribute outputs
            List<PassthroughThroughput> outputsWithCapacity = this.Outputs
                .Where(o => o.HasNeighbor() && !o.Neighbor.IsAtCapacity())
                .ToList();

            // If we have outputs that can handle more capacity, update them to do so
            uint totalInputFlow = this.Inputs.Select(i => i.FlowRate).Aggregate((curr, acc) => curr + acc);
            uint remainingOutputFlow = totalInputFlow - totalOutputFlow;
            foreach (PassthroughThroughput output in outputsWithCapacity)
            {
                uint oldFlow = output.FlowRate;
                uint newFlow = output.SetFlow(remainingOutputFlow);
                output.Neighbor.SetFlow(newFlow);

                uint flowDelta = oldFlow - newFlow;
                remainingOutputFlow -= flowDelta;
            }

            // If we have no remaining flow, we're done
            if (remainingOutputFlow == 0)
            {
                return;
            }

            // If no outputs can handle more capacity, reverse-update inputs
            uint eachInputFlow = (uint)(totalOutputFlow / this.CountConnectedInputs());
            foreach (PassthroughThroughput input in this.Inputs.Where(i => i.HasNeighbor()))
            {
                uint newFlow = input.SetFlow(eachInputFlow);
                input.Neighbor.SetFlow(newFlow);
            }

            foreach (MachineBase inputMachine in this.InputMachines())
            {
                inputMachine.ReverseUpdate();
            }
        }

        public override string ToString()
        {
            return string.Concat("[", string.Join("+", this.Inputs.Select(i => i.RateString(false))), "]:[",
                string.Join("+", this.Outputs.Select(i => i.RateString(true))), "]");
        }
    }
}
