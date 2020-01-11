using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using aga8_call;

namespace Benchmark
{
    public class Aga
    {
        public AGA8Detail aga = new AGA8Detail();
        private readonly double[] comp = {
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
        private double press = 50000.0;
        private double tempr = 400.0;



        public Aga()
        {
            aga.setup();
        }

        [Benchmark]
        public void detail_setup()
        {
            aga.setup();
        }

        [Benchmark]
        public void detail_density()
        {
            aga.setComposition(comp);
            aga.setPressure(press);
            aga.setTemperature(tempr);
            aga.calculateDensity();

        }

        [Benchmark]
        public void detail_properties()
        {
            aga.setComposition(comp);
            aga.setPressure(press);
            aga.setTemperature(tempr);
            //aga.calculateDensity();
            aga.calculateProperties();
        }
    }

    public class Program
    {
        public static void Main()
        {
            System.Console.WriteLine("Hello world");
            var summary = BenchmarkRunner.Run<Aga>();
        }
    }
}