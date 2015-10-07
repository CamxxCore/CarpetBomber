using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;

namespace GTAV_CarpetBomber
{
    public class ControlMonitor : Script
    {
        private static List<Control> disabledControls;

        public ControlMonitor()
        {
            Tick += OnTick;
            disabledControls = new List<Control>();
        }

        private void OnTick(object sender, EventArgs e)
        {
            for (int i = 0; i < disabledControls.Count; i++)
            {
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)disabledControls[i], true);
            }
        }

        public static void DisableControl(Control control)
        {
            if (!disabledControls.Contains(control))
                disabledControls.Add(control);
        }

        public static void EnableControl(Control control)
        {
            if (disabledControls.Contains(control))
                disabledControls.Remove(control);
        }
    }
}
