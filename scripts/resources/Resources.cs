using System.Collections.Generic;
using Godot;

namespace FactoryPlanner.scripts.resources
{
    public enum ResourceList
    {
        Any,
        Iron,
        Copper,
    }

    public class Resource
    {
        public const int DefaultId = 0;
        public static readonly Color DefaultColor = Colors.White;

        private static int NextId { get; set; } = DefaultId;

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
            [ResourceList.Any] = new Resource("Any", DefaultColor),
            [ResourceList.Iron] = new Resource("Iron", Colors.Silver),
            [ResourceList.Copper] = new Resource("Copper", Colors.Chocolate),
        };

        public static bool TryGetResource(ResourceList val, out Resource resource)
        {
            return ResourceMap.TryGetValue(val, out resource);
        }

        public static Resource GetResource(ResourceList val)
        {
            if (TryGetResource(val, out Resource resource)) return resource;

            throw new KeyNotFoundException();
        }
    }
}
