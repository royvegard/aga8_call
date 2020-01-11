using System;
using System.Runtime.InteropServices;

namespace aga8_call
{
    class Program
    {
        static public void Main()
        {
            var aga = new AGA8Detail();
            var gerg = new Gerg();
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
            Console.WriteLine("Object");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                aga.setup();
                gerg.setup();
                aga.setComposition(comp);
                gerg.setComposition(comp);
                aga.setPressure(press);
                gerg.setPressure(press);
                aga.setTemperature(tempr);
                gerg.setTemperature(tempr);
                aga.calculateDensity();
                gerg.calculateDensity();
            }

            double aga_result = aga.getDensity();
            double gerg_result = gerg.getDensity();

            aga.calculateProperties();
            gerg.calculateProperties();
            var aga_results_obj = aga.getProperties();
            var gerg_results_obj = gerg.getProperties();
            watch.Stop();
            System.Console.WriteLine("Runtime: {0}", watch.ElapsedMilliseconds);

            foreach (var field in aga_results_obj.GetType().GetFields())
            {
                Console.WriteLine("Aga: {0}: {1}", field.Name, field.GetValue(aga_results_obj));
            }

            foreach (var field in gerg_results_obj.GetType().GetFields())
            {
                Console.WriteLine("Gerg: {0}: {1}", field.Name, field.GetValue(gerg_results_obj));
            }

            // Using simple Rust function
            Console.WriteLine("\nSimple function");
            watch.Restart();
            var aga_results = Native.aga8_2017(comp, press, tempr);
            var gerg_results = Native.gerg_2008(comp, press, tempr);

            for (int i = 0; i < 1000; i++)
            {
                //aga_results = Native.aga8_2017(comp, press, tempr);
                gerg_results = Native.gerg_2008(comp, press, tempr);
            }

            watch.Stop();
            System.Console.WriteLine("Runtime: {0}", watch.ElapsedMilliseconds);

            foreach (var field in aga_results.GetType().GetFields())
            {
                Console.WriteLine("Aga: {0}: {1}", field.Name, field.GetValue(aga_results));
            }

            foreach (var field in gerg_results.GetType().GetFields())
            {
                Console.WriteLine("Gerg: {0}: {1}", field.Name, field.GetValue(gerg_results));
            }
        }
    }
    internal class Native
    {
        [DllImport("aga8")]
        internal static extern AGA8DetailHandle aga8_new();
        [DllImport("aga8")]
        internal static extern void aga8_free(IntPtr aga8);
        [DllImport("aga8")]
        internal static extern void aga8_setup(AGA8DetailHandle aga8);
        [DllImport("aga8")]
        internal static extern void aga8_set_composition(AGA8DetailHandle aga8, double[] composition);
        [DllImport("aga8")]
        internal static extern void aga8_set_pressure(AGA8DetailHandle aga8, double pressure);
        [DllImport("aga8")]
        internal static extern void aga8_set_temperature(AGA8DetailHandle aga8, double temperature);
        [DllImport("aga8")]
        internal static extern void aga8_calculate_density(AGA8DetailHandle aga8);
        [DllImport("aga8")]
        internal static extern double aga8_get_density(AGA8DetailHandle aga8);
        [DllImport("aga8")]
        internal static extern void aga8_calculate_properties(AGA8DetailHandle aga8);
        [DllImport("aga8")]
        internal static extern AGA8Detail.Properties aga8_get_properties(AGA8DetailHandle aga8);

        [DllImport("aga8")]
        internal static extern AGA8Detail.Properties aga8_2017(double[] composition, double pressure,
            double temperature);

        [DllImport("aga8")]
        internal static extern GergHandle gerg_new();
        [DllImport("aga8")]
        internal static extern void gerg_free(IntPtr gerg);
        [DllImport("aga8")]
        internal static extern void gerg_setup(GergHandle gerg);
        [DllImport("aga8")]
        internal static extern void gerg_set_composition(GergHandle gerg, double[] composition);
        [DllImport("aga8")]
        internal static extern void gerg_set_pressure(GergHandle gerg, double pressure);
        [DllImport("aga8")]
        internal static extern void gerg_set_temperature(GergHandle gerg, double temperature);
        [DllImport("aga8")]
        internal static extern void gerg_calculate_density(GergHandle gerg);
        [DllImport("aga8")]
        internal static extern double gerg_get_density(GergHandle aggerga8);
        [DllImport("aga8")]
        internal static extern void gerg_calculate_properties(GergHandle gerg);
        [DllImport("aga8")]
        internal static extern Gerg.Properties gerg_get_properties(GergHandle gerg);

        [DllImport("aga8")]
        internal static extern Gerg.Properties gerg_2008(double[] composition, double pressure,
            double temperature);
    }

    internal class AGA8DetailHandle : SafeHandle
    {
        public AGA8DetailHandle() : base(IntPtr.Zero, true) { }

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

        public Properties getProperties()
        {
            return Native.aga8_get_properties(aga8);
        }

        public void Dispose()
        {
            aga8.Dispose();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Properties
        {
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
    }

    internal class GergHandle : SafeHandle
    {
        public GergHandle() : base(IntPtr.Zero, true) { }

        public override bool IsInvalid
        {
            get { return false; }
        }

        protected override bool ReleaseHandle()
        {
            Native.gerg_free(handle);
            return true;
        }
    }

    public class Gerg : IDisposable
    {
        private GergHandle gerg;

        public Gerg()
        {
            gerg = Native.gerg_new();
        }

        public void setup()
        {
            Native.gerg_setup(gerg);
        }

        public void setComposition(double[] composition)
        {
            if (composition.Length != 21)
            {
                throw new System.ArgumentException("composition must be exactly length 21");
            }
            Native.gerg_set_composition(gerg, composition);
        }

        public void setPressure(double pressure)
        {
            Native.gerg_set_pressure(gerg, pressure);
        }

        public void setTemperature(double temperature)
        {
            Native.gerg_set_temperature(gerg, temperature);
        }

        public double getDensity()
        {
            return Native.gerg_get_density(gerg);
        }

        public void calculateDensity()
        {
            Native.gerg_calculate_density(gerg);
        }

        public void calculateProperties()
        {
            Native.gerg_calculate_properties(gerg);
        }

        public Properties getProperties()
        {
            return Native.gerg_get_properties(gerg);
        }

        public void Dispose()
        {
            gerg.Dispose();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Properties
        {
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
    }
}
