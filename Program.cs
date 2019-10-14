using System;
using System.Runtime.InteropServices;

namespace aga8_call
{
    class Program
    {
        [DllImport("aga8_2017", CallingConvention=CallingConvention.StdCall)]
        private static extern Aga8_Result aga8_2017(double[] composition, double pressure,
            double temperature);

        [StructLayout(LayoutKind.Sequential)]
        struct Aga8_Result {
            public double d; // Molar concentration [mol/l]
            public double mm;
            public double z;
            public double dp_dd;
            public double d2p_dd2;
            public double dp_dt;
            public double u;
            public double h;
            public double s;
            public double cv;
            public double cp;
            public double w;
            public double g;
            public double jt;
            public double kappa;
        }

        static void Main(string[] args)
        {
            double[] comp = {
                0.778240, // Methane
                0.020000, // Nitrogen
                0.060000, // Carbon dioxide
                0.080000, // Ethane
                0.030000, // Propane
                0.001500, // Isobutane
                0.003000, // n-Butane
                0.000500, // Isopentane
                0.001650, // n-Pentane
                0.002150, // Hexane
                0.000880, // Heptane
                0.000240, // Octane
                0.000150, // Nonane
                0.000090, // Decane
                0.004000, // Hydrogen
                0.005000, // Oxygen
                0.002000, // Carbon monoxide
                0.000100, // Water
                0.002500, // Hydrogen sulfide
                0.007000, // Helium
                0.001000 // Argon
            };

            double press = 50000.0;
            double tempr = 400.0;
            Aga8_Result resultat = aga8_2017(comp, press, tempr);

            Console.WriteLine(aga8_2017(comp, press, tempr).mm);
        }
    }
}
