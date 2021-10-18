using System;
using Godot;

namespace FactoryPlanner.scripts.machines
{
    public class MachineNode : GraphNode
    {
        protected static void AddEnumItems(OptionButton optionButton, Type enumType)
        {
            foreach (object val in Enum.GetValues(enumType))
            {
                optionButton.AddItem(val.ToString());
                optionButton.SetItemMetadata(optionButton.GetItemCount()-1, val);
            }
        }

        protected void _on_GraphNode_close_request()
        {
            this.QueueFree();
        }
    }
}
