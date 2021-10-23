using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FactoryPlanner.scripts.machines
{
    public class MachineNode : GraphNode
    {
        private uint Efficiency { get; set; }
        private decimal EfficiencyPercentage => (decimal)this.Efficiency / 100;
        private decimal EfficiencyMult() => (decimal)this.Efficiency / (100 * Utils.Precision);

        private IList<Throughput> Inputs { get; }
        private IList<Throughput> Outputs { get; }

        private VBoxContainer InputContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(0);
        protected VBoxContainer ControlsContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1);
        private VBoxContainer OutputContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(2);
        public HSlider EfficiencySlider => this.ControlsContainer.GetChild<HSlider>(1);

        private Label ResourceNameLabel(VBoxContainer container, int slotId) => container.GetChild<VBoxContainer>(slotId).GetChild<Label>(0);
        private Label RateLabel(VBoxContainer container, int slotId) => container.GetChild<VBoxContainer>(slotId).GetChild<Label>(1);

        internal MachineNode(int numInputs, int numOutputs)
        {
            this.Inputs = new List<Throughput>();
            for (int i = 0; i < numInputs; i++)
            {
                this.Inputs.Add(new Input(0, this));
            }

            this.Outputs = new List<Throughput>();
            for (int i = 0; i < numOutputs; i++)
            {
                this.Outputs.Add(new Output(0, this));
            }
        }

        public override void _Ready()
        {
            // Add labels
            foreach (Throughput _ in this.Inputs)
            {
                VBoxContainer slotLabelContainer = new VBoxContainer();
                slotLabelContainer.AddChild(new Label());
                slotLabelContainer.AddChild(new Label());

                this.InputContainer.AddChild(slotLabelContainer);
            }

            foreach (Throughput _ in this.Outputs)
            {
                VBoxContainer slotLabelContainer = new VBoxContainer();
                slotLabelContainer.AddChild(new Label());
                slotLabelContainer.AddChild(new Label());

                this.OutputContainer.AddChild(slotLabelContainer);
            }
        }

        protected void UpdateRecipe(Recipe recipe)
        {
            for (int i = 0; i < recipe.Inputs.Count; i++)
            {
                this.Inputs[i].Resource = recipe.Inputs[i].Resource;
                this.Inputs[i].Capacity = recipe.Inputs[i].Capacity;
            }
            for (int i = 0; i < recipe.Outputs.Count; i++)
            {
                this.Outputs[i].Resource = recipe.Outputs[i].Resource;
                this.Outputs[i].Capacity = recipe.Outputs[i].Capacity;
            }

            this.UpdateFlowFromInputs();
        }

        protected void UpdateSlots()
        {
            int numInputs = this.Inputs.Count;
            int numOutputs = this.Outputs.Count;

            for (int slotId = 0; slotId < Math.Max(numInputs, numOutputs); slotId++)
            {
                Throughput input = slotId < numInputs ? this.Inputs[slotId] : null;
                Throughput output = slotId < numOutputs ? this.Outputs[slotId] : null;

                // Update slots
                this.SetSlot(slotId,
                    input != null, input?.TypeId ?? Resource.Any.TypeId, input?.Color ?? Resource.DefaultColor,
                    output != null, output?.TypeId ?? Resource.Any.TypeId, output?.Color ?? Resource.DefaultColor);

                // Update labels
                if (input != null)
                {
                    this.ResourceNameLabel(this.InputContainer, slotId).Text = input.Name;
                    this.RateLabel(this.InputContainer, slotId).Text = input.RateString;
                }

                if (output != null)
                {
                    this.ResourceNameLabel(this.OutputContainer, slotId).Text = output.Name;
                    this.RateLabel(this.OutputContainer, slotId).Text = output.RateString;
                }
            }

            this.EfficiencySlider.Value = (int)this.EfficiencyPercentage;
        }

        protected static void AddEnumItems(OptionButton optionButton, Type enumType)
        {
            foreach (object val in Enum.GetValues(enumType))
            {
                optionButton.AddItem(val.ToString());
                optionButton.SetItemMetadata(optionButton.GetItemCount()-1, val);
            }
        }

        protected static void AddOption<T>(OptionButton optionButton, string name, T metaData)
        {
            optionButton.AddItem(name);
            optionButton.SetItemMetadata(optionButton.GetItemCount()-1, metaData);
        }

        public void ConnectTo(int fromSlot, MachineNode toMachine, int toSlot)
        {
            this.Outputs[fromSlot].SetNeighbor(toMachine.Inputs[toSlot]);
            toMachine.Inputs[toSlot].SetNeighbor(this.Outputs[fromSlot]);

            toMachine.UpdateFlowFromInputs();
        }

        public void DisconnectFrom(int fromSlot, MachineNode toMachine, int toSlot)
        {
            this.Outputs[fromSlot].SetNeighbor(null);
            toMachine.Inputs[toSlot].SetNeighbor(null);

            this.UpdateFlowFromInputs();
            toMachine.UpdateFlowFromInputs();
        }

        private void UpdateFlowFromInputs()
        {
            // Calculate this Machine's efficiency where...
            // ...no inputs = 100% automatically
            if (!this.Inputs.Any())
            {
                this.Efficiency = 100 * Utils.Precision;
            }
            // ...at least 1 unconnected input = 0% automatically
            else if (this.Inputs.Any(i => i.Neighbor == null))
            {
                this.Efficiency = 0;
            }
            else
            {
                // ...back-fill inputs' flows
                foreach (Input input in this.Inputs)
                {
                    input.Neighbor.Parent.UpdateFlowFromInputs();
                }

                this.Efficiency = this.Inputs.Select(i => i.Efficiency()).Min();
            }

            // Once we have our efficiency, we can use it to calculate our outputs' flows
            foreach (Output output in this.Outputs)
            {
                output.CalculateFlow(this.EfficiencyMult());
            }

            this.UpdateSlots();
        }

        public void UpdateFlowFromOutputs()
        {
            this.Efficiency = this.Outputs.Select(o => o.Efficiency()).Min();
            foreach (Input input in this.Inputs)
            {
                input.CalculateFlow(this.EfficiencyMult());
            }
        }

        protected void _on_GraphNode_close_request()
        {
            this.QueueFree();
        }
    }
}
