using System.ComponentModel;
using FactoryPlanner.scripts.resources;

namespace FactoryPlanner.scripts.machines
{
    public class Miner
    {
        public enum LevelList
        {
            [Description("Mk. 1")]
            Mk1,

            [Description("Mk. 2")]
            Mk2,

            [Description("Mk. 3")]
            Mk3,
        }

        public enum PurityList
        {
            Pure,
            Normal,
            Impure,
        }

        public Resource Resource { get; set; } = Resource.GetResource(ResourceList.Iron);
        public int Efficiency { get; set; } = 100;
        public LevelList Level { get; set; } = LevelList.Mk1;
        public PurityList Purity { get; set; } = PurityList.Normal;
    }
}
