using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FactoryPlanner.scripts.machines
{
    public static class KnownInputActions
    {
        public const string Duplicate = "duplicate";
        public const string FocusAddMenu = "ui_focus_add_menu";
        public const string UISelect1 = "ui_select_1";
        public const string UISelect2 = "ui_select_2";
        public const string UISelect3 = "ui_select_3";
        public const string UISelect4 = "ui_select_4";
        public const string UISelect5 = "ui_select_5";
        public const string UISelect6 = "ui_select_6";
        public const string UISelect7 = "ui_select_7";
        public const string UISelect8 = "ui_select_8";
        public const string UISelect9 = "ui_select_9";
        public const string UISelect0 = "ui_select_0";

        private static readonly List<string> UISelectList = new()
        {
            UISelect0,
            UISelect1,
            UISelect2,
            UISelect3,
            UISelect4,
            UISelect5,
            UISelect6,
            UISelect7,
            UISelect8,
            UISelect9,
        };

        public static int? UISelectIdxPressed(InputEvent inputEvent, bool zeroAtEnd = true)
        {
            int? idx = UISelectList
                .Select((name, idx) => new { name, idx })
                .FirstOrDefault(t => inputEvent.IsActionPressed(t.name))?
                .idx;

            if (idx is null)
            {
                return null;
            }

            if (!zeroAtEnd)
            {
                return idx;
            }

            if (idx == 0)
            {
                return 9;
            }

            return idx - 1;

        }
    }
}
