using FactoryPlanner.scripts.machines;
using MachineNetwork;
using Resource = FactoryPlanner.scripts.machines.Resource;

public class BalancerNode : MachineNode
{
    private Balancer MachineModel { get; }

    internal BalancerNode()
    {
        this.MachineModel = new Balancer(3, 3, Resource.Any.Id);
    }

    public override MachineBase GetMachineModel()
    {
        return this.MachineModel;
    }
}
