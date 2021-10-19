using System;
using System.Collections.Generic;
using Godot;

namespace FactoryPlanner.scripts.resources
{
    public class Resource
    {
        public static readonly Color DefaultColor = Colors.Black;
        public static readonly Resource Any = new Resource("Any", "Any", Colors.White);

        public int TypeId { get; private set; }
        public string Id { get; private set; }
        public string Name { get; }
        public Color Color { get; }

        private Resource(string id, string name, Color? color = null)
        {
            this.Id = id;
            this.Name = name;
            this.Color = color ?? Colors.Gray;
        }

        public static readonly IDictionary<string, Resource> Resources = new Dictionary<string, Resource>();

        private static void AddResource(Resource resource)
        {
            resource.TypeId = Resources.Count;
            Resources.Add(resource.Id, resource);
        }

        public static void LoadResources()
        {
            if (Resources.Count != 0)
            {
                throw new Exception("Cannot re-load FactoryResource list");
            }

            AddResource(Any);
            AddResource(new Resource("IronOre", "Iron Ore", Colors.Silver));
            AddResource(new Resource("CopperOre", "Copper Ore", Colors.Chocolate));
            AddResource(new Resource("IronIngot", "Iron Ingot", Colors.Silver));
            AddResource(new Resource("CopperIngot", "Copper Ingot", Colors.Chocolate));
        }

        public static Resource GetResource(string id)
        {
            return Resources[id];
        }
    }
}
