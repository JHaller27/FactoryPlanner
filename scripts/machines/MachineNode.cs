using System;
using MachineNetwork;
using Godot;
using Network = MachineNetwork.MachineNetwork;

namespace FactoryPlanner.scripts.machines
{
    public class MachineNode : GraphNode
    {
        private VBoxContainer InputContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(0);
        protected VBoxContainer ControlsContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1);
        private VBoxContainer OutputContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(2);
        public HSlider EfficiencySlider => this.ControlsContainer.GetChild<HSlider>(1);

        private Label ResourceNameLabel(VBoxContainer container, int slotId) => container.GetChild<VBoxContainer>(slotId).GetChild<Label>(0);
        private Label RateLabel(VBoxContainer container, int slotId) => container.GetChild<VBoxContainer>(slotId).GetChild<Label>(1);

        protected string RecipeId { get; set; }

        public Machine MachineModel { get; }

        internal MachineNode(int numInputs, int numOutputs)
        {
            this.MachineModel = new Machine(numInputs, numOutputs, Resource.Any.Id);
        }

        public override void _Ready()
        {
            // Add labels
            for (int i = 0; i < this.MachineModel.CountInputs; i++)
            {
                VBoxContainer slotLabelContainer = new VBoxContainer();
                slotLabelContainer.AddChild(new Label());
                slotLabelContainer.AddChild(new Label());

                this.InputContainer.AddChild(slotLabelContainer);
            }

            for (int i = 0; i < this.MachineModel.CountOutputs; i++)
            {
                VBoxContainer slotLabelContainer = new VBoxContainer();
                slotLabelContainer.AddChild(new Label());
                slotLabelContainer.AddChild(new Label());

                this.OutputContainer.AddChild(slotLabelContainer);
            }

            this.UpdateSlots();
        }

        protected virtual Recipe ChooseRecipe()
        {
            return Recipe.GetRecipe(this.RecipeId);
        }

        protected void UpdateRecipe()
        {
            Recipe recipe = this.ChooseRecipe();

            foreach ((int idx, string resourceId, uint capacity) in recipe.ListInputs())
            {
                this.MachineModel.SetInput(idx, resourceId, capacity);
            }
            foreach ((int idx, string resourceId, uint capacity) in recipe.ListOutputs())
            {
                this.MachineModel.SetOutput(idx, resourceId, capacity);
            }

            Network.Instance.Recalculate();
            this.UpdateSlots();
        }

        public void UpdateSlots()
        {
            int numInputs = this.MachineModel.CountInputs;
            int numOutputs = this.MachineModel.CountOutputs;

            for (int slotId = 0; slotId < Math.Max(numInputs, numOutputs); slotId++)
            {
                bool hasInput = this.MachineModel.TryGetInputSlot(slotId, out Throughput input);
                Resource inputResource = Resource.Any;
                bool hasOutput = this.MachineModel.TryGetOutputSlot(slotId, out Throughput output);
                Resource outputResource = Resource.Any;

                // Update labels
                if (hasInput)
                {
                    inputResource = Resource.GetResource(input.ResourceId);

                    this.ResourceNameLabel(this.InputContainer, slotId).Text = inputResource.Name;
                    this.RateLabel(this.InputContainer, slotId).Text = this.RateString(input);
                }

                if (hasOutput)
                {
                    outputResource = Resource.GetResource(output.ResourceId);

                    this.ResourceNameLabel(this.OutputContainer, slotId).Text = outputResource.Name;
                    this.RateLabel(this.OutputContainer, slotId).Text = this.RateString(output);
                }

                // Update slots
                this.SetSlot(slotId,
                    input != null, inputResource.TypeId, inputResource.Color,
                    output != null, outputResource.TypeId, outputResource.Color);
            }

            this.EfficiencySlider.Value = (int)this.MachineModel.EfficiencyPercentage;
        }

        private string RateString(Throughput throughput)
        {
            return $"{throughput.Flow / Utils.Precision:0.##} / {throughput.Capacity / Utils.Precision:0.##}";
        }

        protected static void AddEnumItems(OptionButton optionButton, Type enumType, int defaultIdx = 0)
        {
            foreach (object val in Enum.GetValues(enumType))
            {
                optionButton.AddItem(val.ToString());
                optionButton.SetItemMetadata(optionButton.GetItemCount()-1, val);
            }

            optionButton.Selected = defaultIdx;
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
