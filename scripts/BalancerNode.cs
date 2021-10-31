using System;
using FactoryPlanner.scripts.machines;
using Godot;
using MachineNetwork;
using Resource = FactoryPlanner.scripts.machines.Resource;

public class BalancerNode : MachineNode
{
    private HSplitContainer GetSlotContainer(int slotId) => this.GetChild<HSplitContainer>(slotId);

    protected override Label InputResourceNameLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(0).GetChild<Label>(0);
    protected override Label InputRateLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(0).GetChild<Label>(1);
    protected override Label OutputResourceNameLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(1).GetChild<Label>(0);
    protected override Label OutputRateLabel(int slotId) => this.GetSlotContainer(slotId).GetChild<VBoxContainer>(1).GetChild<Label>(1);

    private Balancer MachineModel { get; }

    internal BalancerNode()
    {
        this.MachineModel = new Balancer(3, 3, Resource.Any.Id);
    }

    protected override void SetupGUI()
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

    public override MachineBase GetMachineModel()
    {
        return this.MachineModel;
    }
}
