using System.Collections.Generic;
using Godot;

namespace FactoryPlanner.scripts.resources
{
    public enum ResourceList
    {
        Iron,
        Copper,
    }

    public class Resource
    {
        private static int NextId { get; set; } = 0;

        public int Id { get; }
        public string Name { get; }
        public Color Color { get; }

        private Resource(string name, Color? color = null)
        {
            this.Name = name;
            this.Color = color ?? Colors.Gray;
            this.Id = NextId++;
        }

        private static readonly Dictionary<ResourceList, Resource> ResourceMap = new Dictionary<ResourceList, Resource>
        {
            [ResourceList.Iron] = new Resource("Iron", Colors.Silver),
            [ResourceList.Copper] = new Resource("Copper", Colors.Chocolate),
        };

        public static bool TryGetResource(ResourceList val, out Resource resource)
        {
            return ResourceMap.TryGetValue(val, out resource);
        }
    }
}
