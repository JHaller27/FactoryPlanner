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
            UpdateRecipe(m1, null, new() { [0] = 6000 });
            Network.Instance.AddMachine(m1);

            EfficientMachine m2a = new(1, 1, "Any");
            UpdateRecipe(m2a, new() { [0] = 3000 }, new() { [0] = 3000 });
            Network.Instance.AddMachine(m2a);

            EfficientMachine m2b = new(1, 1, "Any");
            UpdateRecipe(m2b, new() { [0] = 3000 }, new() { [0] = 3000 });
            Network.Instance.AddMachine(m2b);

            Balancer splitter2 = new(3, 3, "Any");
            Network.Instance.AddMachine(splitter2);

            EfficientMachine m3a = new(1, 1, "Any");
            UpdateRecipe(m3a, new() { [0] = 3000 }, new() { [0] = 2000 });
            Network.Instance.AddMachine(m3a);

            EfficientMachine m3b = new(1, 1, "Any");
            UpdateRecipe(m3b, new() { [0] = 3000 }, new() { [0] = 2000 });
            Network.Instance.AddMachine(m3b);

            Balancer merger = new(3, 3, "Any");
            Network.Instance.AddMachine(merger);

            Console.WriteLine(Network.Instance);

            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(m2a, 0, m3a, 0);
            Network.Instance.ConnectMachines(m2b, 0, m3b, 0);

            Network.Instance.ConnectMachines(m1, 0, splitter2, 0);

            Console.WriteLine(Network.Instance);

            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(splitter2, 0, m2a, 0);
            Network.Instance.ConnectMachines(splitter2, 1, m2b, 0);
            Console.WriteLine(Network.Instance);

            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(splitter2, 0, m2a, 0);
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
