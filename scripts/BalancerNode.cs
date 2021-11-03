using Godot;
using MachineNetwork;

namespace FactoryPlanner.scripts.machines
{
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
}
