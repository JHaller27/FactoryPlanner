using System;
using System.Collections.Generic;
using Godot;

namespace FactoryPlanner.scripts.machines
{
    public class Resource
    {
        public static readonly Color DefaultColor = Colors.Black;
        public static readonly Resource Any = new Resource("Any", "Any", Colors.White) { TypeId = -1 };

        public int TypeId { get; private set; }
        public string Id { get; private set; }
        public string Name { get; }
        public Color Color { get; }

        public Resource(string name, string id = null, Color? color = null)
        {
            this.Name = name;
            this.Id = id ?? this.Name;
            this.Color = color ?? Colors.Gray;
        }

        public static readonly IDictionary<string, Resource> Resources = new Dictionary<string, Resource>();

        public static void AddResource(Resource resource)
        {
            resource.TypeId = Resources.Count;
            Resources.Add(resource.Id, resource);
        }

        public static Resource GetResource(string id)
        {
            return Resources[id];
        }
    }
}
