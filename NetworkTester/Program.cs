using System;
using System.Collections.Generic;
using MachineNetwork;
using Network = MachineNetwork.MachineNetwork;
// ReSharper disable InconsistentNaming

namespace NetworkTester
{
    class Program
    {
        static void Main(string[] args)
        {
            MachineNetwork.MachineNetwork.Precision = 100;

            EfficientMachine m1 = new(0, 1, "Any");
            UpdateRecipe(m1, null, new() { [0] = 12000 });
            Network.Instance.AddMachine(m1);

            EfficientMachine m2 = new(1, 1, "Any");
            UpdateRecipe(m2, new() { [0] = 3000 }, new() { [0] = 3000 });
            Network.Instance.AddMachine(m2);

            EfficientMachine m3 = new(1, 1, "Any");
            UpdateRecipe(m3, new() { [0] = 3000 }, new() { [0] = 3000 });
            Network.Instance.AddMachine(m3);

            Balancer balancer = new(3, 3, "Any");
            Network.Instance.AddMachine(balancer);

            Console.WriteLine(Network.Instance);

            // Connect miner to balancer
            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(m1, 0, balancer, 0);
            Console.WriteLine(Network.Instance);

            // Connect balancer to first smelter
            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(balancer, 0, m2, 0);
            Console.WriteLine(Network.Instance);

            // Connect balancer to second smelter
            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(balancer, 1, m3, 0);
            Console.WriteLine(Network.Instance);
        }

        static void UpdateRecipe(EfficientMachine machine, Dictionary<int, uint> inputCapacities, Dictionary<int, uint> outputCapacities)
        {
            if (inputCapacities != null)
            {
                foreach ((int idx, uint capacity) in inputCapacities)
                {
                    machine.SetInputRecipe(idx, "Foo", capacity);
                }
            }

            if (outputCapacities != null)
            {
                foreach ((int idx, uint capacity) in outputCapacities)
                {
                    machine.SetOutputRecipe(idx, "Foo", capacity);
                }
            }
        }
    }
}
