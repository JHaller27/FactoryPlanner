using System;
using System.Collections.Generic;
using Godot;

namespace FactoryPlanner.scripts.resources
{
    public class Resource
    {
        public static readonly Color DefaultColor = Colors.Black;
        public static readonly Resource Any = new Resource("Any", Colors.White);

        public int Id { get; private set; }
        public string Name { get; }
        public Color Color { get; }

        private Resource(string name, Color? color = null)
        {
            this.Name = name;
            this.Color = color ?? Colors.Gray;
        }

        public static readonly IList<Resource> Resources = new List<Resource>();

        public static void AddResource(Resource resource)
        {
            resource.Id = Resources.Count;
            Resources.Add(resource);
        }

        public static void LoadResources()
        {
            if (Resources.Count != 0)
            {
                throw new Exception("Cannot re-load FactoryResource list");
            }

            AddResource(Any);
            AddResource(new Resource("Iron Ore", Colors.Silver));
            AddResource(new Resource("Copper Ore", Colors.Chocolate));
            AddResource(new Resource("Iron Ingot", Colors.Silver));
            AddResource(new Resource("Copper Ingot", Colors.Chocolate));
        }

        public static Resource GetResource(int idx)
        {
            return Resources[idx];
        }
    }
}
