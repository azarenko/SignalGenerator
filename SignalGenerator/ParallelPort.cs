using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SignalGenerator
{
    class ParallelPort
    {
        public static int[] PortAddress = new int[]
        {
            0x3bc,
            0x378,
            0x278
        };

        [DllImport("inpout32.dll", EntryPoint = "Out32")]
        public static extern void Output(int address, int value);
    }
}
