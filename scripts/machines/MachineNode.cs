using System;
using MachineNetwork;
using Godot;
using Network = MachineNetwork.MachineNetwork;

namespace FactoryPlanner.scripts.machines
{
    public abstract class MachineNode : GraphNode
    {
        private HSplitContainer GetSlotContainer(int slotId) => this.GetChild<HSplitContainer>(slotId+1);

        private Label InputResourceNameLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(0).GetChild<Label>(0);
        private Label InputRateLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(0).GetChild<Label>(1);
        private Label OutputResourceNameLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(1).GetChild<Label>(0);
        private Label OutputRateLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(1).GetChild<Label>(1);


        public abstract MachineBase GetMachineModel();

        protected virtual void SetupGUI()
        {
            // Add HSplitContainers
            for (int i = 0; i < Math.Max(this.GetMachineModel().CountInputs(), this.GetMachineModel().CountOutputs()); i++)
            {
                HSplitContainer container = new HSplitContainer();

                // Add left VBox
                VBoxContainer leftVBox = new VBoxContainer();
                Label leftLabel1 = new Label();
                Label leftLabel2 = new Label();
                leftLabel1.Align = Label.AlignEnum.Left;
                leftLabel2.Align = Label.AlignEnum.Left;
                leftVBox.AddChild(leftLabel1);
                leftVBox.AddChild(leftLabel2);

                // Add right VBox
                VBoxContainer rightVBox = new VBoxContainer();
                Label rightLabel1 = new Label();
                Label rightLabel2 = new Label();
                rightLabel1.Align = Label.AlignEnum.Right;
                rightLabel2.Align = Label.AlignEnum.Right;
                rightVBox.AddChild(rightLabel1);
                rightVBox.AddChild(rightLabel2);

                container.AddChild(leftVBox);
                container.AddChild(rightVBox);
                this.AddChild(container);
            }
        }

        public override void _Ready()
        {
            this.SetupGUI();
            this.UpdateSlots();
        }

        public virtual void UpdateSlots()
        {
            int numInputs = this.GetMachineModel().CountInputs();
            int numOutputs = this.GetMachineModel().CountOutputs();

            for (int slotId = 0; slotId < Math.Max(numInputs, numOutputs); slotId++)
            {
                bool hasInput = this.GetMachineModel().TryGetInputSlot(slotId, out IThroughput input);
                Resource inputResource = Resource.Any;
                bool hasOutput = this.GetMachineModel().TryGetOutputSlot(slotId, out IThroughput output);
                Resource outputResource = Resource.Any;

                // Update labels
                if (hasInput)
                {
                    inputResource = Resource.GetResource(input.ResourceId);

                    this.InputResourceNameLabel(slotId).Text = inputResource.Name;
                    this.InputRateLabel(slotId).Text = input.RateString();
                }

                if (hasOutput)
                {
                    outputResource = Resource.GetResource(output.ResourceId);

                    this.OutputResourceNameLabel(slotId).Text = outputResource.Name;
                    this.OutputRateLabel(slotId).Text = output.RateString();
                }

                // Update slots
                this.SetSlot(slotId+1,
                    input != null, inputResource.TypeId, inputResource.Color,
                    output != null, outputResource.TypeId, outputResource.Color);
            }
        }

        protected void _on_GraphNode_close_request()
        {
            this.QueueFree();
        }
    }

    public class EfficientMachineNode : MachineNode
    {
        protected VBoxContainer ControlsContainer => this.GetChild<HBoxContainer>(0).GetChild<VBoxContainer>(1);
        private HSlider EfficiencySlider => this.ControlsContainer.GetChild<HSlider>(1);

        private EfficientMachine MachineModel { get; }

        public override MachineBase GetMachineModel()
        {
            return this.MachineModel;
        }

        protected string RecipeId { get; set; }

        internal EfficientMachineNode(int numInputs, int numOutputs)
        {
            this.MachineModel = new EfficientMachine(numInputs, numOutputs, Resource.Any.Id);
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
                this.MachineModel.SetInputRecipe(idx, resourceId, capacity);
            }
            foreach ((int idx, string resourceId, uint capacity) in recipe.ListOutputs())
            {
                this.MachineModel.SetOutputRecipe(idx, resourceId, capacity);
            }

            Network.Instance.Recalculate();
            this.UpdateSlots();
        }

        public override void UpdateSlots()
        {
            base.UpdateSlots();
            this.EfficiencySlider.Value = (int)this.MachineModel.GetEfficiencyPercentage();
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
