using System;
using System.Collections.Generic;
using MachineNetwork;

namespace NetworkTester
{
    class Program
    {
        static void Main(string[] args)
        {
            MachineNetwork.MachineNetwork.Precision = 100;
            MachineNetwork.MachineNetwork network = new();

            Machine m1 = new(0, 1, "Any");
            UpdateRecipe(m1, null, new() { [0] = 6000 });
            network.AddMachine(m1);

            Machine m2 = new(1, 1, "Any");
            UpdateRecipe(m2, new() { [0] = 3000 }, new() { [0] = 3000 });
            network.AddMachine(m2);

            Machine m3 = new(1, 1, "Any");
            UpdateRecipe(m3, new() { [0] = 3000 }, new() { [0] = 2000 });
            network.AddMachine(m3);

            Console.WriteLine(network);

            Console.WriteLine("==================== Connecting... ====================");
            network.ConnectMachines(m2, 0, m3, 0);
            Console.WriteLine(network);

            Console.WriteLine("==================== Connecting... ====================");
            network.ConnectMachines(m1, 0, m2, 0);
            Console.WriteLine(network);
        }

        static void UpdateRecipe(Machine machine, Dictionary<int, uint> inputCapacities, Dictionary<int, uint> outputCapacities)
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
