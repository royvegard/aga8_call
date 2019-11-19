using System;
using System.Runtime.InteropServices;

namespace aga8_call
{
    internal class Native
    {
        [DllImport("aga8_2017")]
        internal static extern AGA8DetailHandle aga8_new();
        [DllImport("aga8_2017")]
        internal static extern void aga8_free(IntPtr aga8);
        [DllImport("aga8_2017")]
        internal static extern void aga8_setup(AGA8DetailHandle aga8);
        [DllImport("aga8_2017")]
        internal static extern void aga8_set_composition(AGA8DetailHandle aga8, double[] composition);
        [DllImport("aga8_2017")]
        internal static extern void aga8_set_pressure(AGA8DetailHandle aga8, double pressure);
        [DllImport("aga8_2017")]
        internal static extern void aga8_set_temperature(AGA8DetailHandle aga8, double temperature);
        [DllImport("aga8_2017")]
        internal static extern void aga8_calculate_density(AGA8DetailHandle aga8);
        [DllImport("aga8_2017")]
        internal static extern double aga8_get_density(AGA8DetailHandle aga8);
        [DllImport("aga8_2017")]
        internal static extern void aga8_calculate_properties(AGA8DetailHandle aga8);
        [DllImport("aga8_2017")]
        internal static extern AGA8Detail.Aga8_Result aga8_get_properties(AGA8DetailHandle aga8);

        [DllImport("aga8_2017")]
        internal static extern AGA8Detail.Aga8_Result aga8_2017(double[] composition, double pressure,
            double temperature);
    }

    internal class AGA8DetailHandle : SafeHandle
    {
        public AGA8DetailHandle() : base(IntPtr.Zero, true) {}

        public override bool IsInvalid
        {
            get { return false; }
        }

        protected override bool ReleaseHandle()
        {
            Native.aga8_free(handle);
            return true;
        }
    }

    public class AGA8Detail : IDisposable
    {
        private AGA8DetailHandle aga8;

        public AGA8Detail()
        {
            aga8 = Native.aga8_new();
        }

        public void setup()
        {
            Native.aga8_setup(aga8);
        }
        
        public void setComposition(double[] composition)
        {
            if (composition.Length != 21)
            {
                throw new System.ArgumentException("composition must be exactly length 21");
            }
            Native.aga8_set_composition(aga8, composition);
        }

        public void setPressure(double pressure)
        {
            Native.aga8_set_pressure(aga8, pressure);
        }

        public void setTemperature(double temperature)
        {
            Native.aga8_set_temperature(aga8, temperature);
        }

        public double getDensity()
        {
            return Native.aga8_get_density(aga8);
        }

        public void calculateDensity()
        {
            Native.aga8_calculate_density(aga8);
        }

        public void calculateProperties()
        {
            Native.aga8_calculate_properties(aga8);
        }

        public Aga8_Result getProperties()
        {
            return Native.aga8_get_properties(aga8);
        }

        public void Dispose()
        {
            aga8.Dispose();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Aga8_Result {
            public double d; // Molar concentration [mol/l]
            public double mm; // Molar mass (g/mol)
            public double z; // Compressibility factor
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

        static public void Main()
        {
            var aga = new AGA8Detail();
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

            // Using Rust object methods
            aga.setup();
            aga.setComposition(comp);
            aga.setPressure(press);
            aga.setTemperature(tempr);
            aga.calculateDensity();

            double result = aga.getDensity();
            Console.WriteLine("Object");
            Console.WriteLine("Density {0}", result);

            aga.calculateProperties();
            var results_obj = aga.getProperties();

            foreach (var field in results_obj.GetType().GetFields())
            {
                Console.WriteLine("{0}: {1}", field.Name, field.GetValue(results_obj));
            }

            // Using simple Rust function
            var results = Native.aga8_2017(comp, press, tempr);

            Console.WriteLine("\nSimple function");
            foreach (var field in results.GetType().GetFields())
            {
                Console.WriteLine("{0}: {1}", field.Name, field.GetValue(results));
            }
        }
    }
}
