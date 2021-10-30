using System;
using System.Collections.Generic;
using MachineNetwork;
using Network = MachineNetwork.MachineNetwork;

namespace NetworkTester
{
    class Program
    {
        static void Main(string[] args)
        {
            MachineNetwork.MachineNetwork.Precision = 100;

            Machine m1 = new(0, 1, "Any");
            UpdateRecipe(m1, null, new() { [0] = 6000 });
            Network.Instance.AddMachine(m1);

            Machine m2 = new(1, 1, "Any");
            UpdateRecipe(m2, new() { [0] = 3000 }, new() { [0] = 3000 });
            Network.Instance.AddMachine(m2);

            Machine m3 = new(1, 1, "Any");
            UpdateRecipe(m3, new() { [0] = 3000 }, new() { [0] = 2000 });
            Network.Instance.AddMachine(m3);

            Console.WriteLine(Network.Instance);

            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(m2, 0, m3, 0);
            Console.WriteLine(Network.Instance);

            Console.WriteLine("==================== Connecting... ====================");
            Network.Instance.ConnectMachines(m1, 0, m2, 0);
            Console.WriteLine(Network.Instance);
        }

        static void UpdateRecipe(IMachine machine, Dictionary<int, uint> inputCapacities, Dictionary<int, uint> outputCapacities)
        {
            if (inputCapacities != null)
            {
                foreach ((int idx, uint capacity) in inputCapacities)
                {
                    machine.SetInput(idx, "Foo", capacity);
                }
            }

            if (outputCapacities != null)
            {
                foreach ((int idx, uint capacity) in outputCapacities)
                {
                    machine.SetOutput(idx, "Foo", capacity);
                }
            }
        }
    }
}
