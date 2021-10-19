using System;
using System.Collections.Generic;
using Godot;
using Resource = FactoryPlanner.scripts.resources.Resource;

namespace FactoryPlanner.scripts.machines
{
    public class MachineNode : GraphNode
    {
        private IList<Throughput> Inputs { get; }
        protected IList<Throughput> Outputs { get; }

        private VBoxContainer InputContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(0);
        protected VBoxContainer ControlsContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1);
        private VBoxContainer OutputContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(2);

        private Label ResourceNameLabel(VBoxContainer container, int slotId) => container.GetChild<VBoxContainer>(slotId).GetChild<Label>(0);
        private Label RateLabel(VBoxContainer container, int slotId) => container.GetChild<VBoxContainer>(slotId).GetChild<Label>(1);

        internal MachineNode(int numInputs, int numOutputs)
        {
            this.Inputs = new List<Throughput>();
            for (int i = 0; i < numInputs; i++)
            {
                this.Inputs.Add(new Throughput());
            }

            this.Outputs = new List<Throughput>();
            for (int i = 0; i < numOutputs; i++)
            {
                this.Outputs.Add(new Throughput());
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

            this.UpdateSlots();
        }

        protected void UpdateRecipe(Recipe recipe)
        {
            for (int i = 0; i < recipe.Inputs.Count; i++)
            {
                this.Inputs[i].Resource = recipe.Inputs[i].Resource;
                this.Inputs[i].Rate = recipe.Inputs[i].Rate;
            }
            for (int i = 0; i < recipe.Outputs.Count; i++)
            {
                this.Outputs[i].Resource = recipe.Outputs[i].Resource;
                this.Outputs[i].Rate = recipe.Outputs[i].Rate;
            }

            this.UpdateSlots();
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

        protected void _on_GraphNode_close_request()
        {
            this.QueueFree();
        }
    }
}
