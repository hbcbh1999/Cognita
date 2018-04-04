﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cognita;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            double Dx = 128;
            double Dy = 10;
            double xo = 64;
            double yo = 5;
            double a = 10;
            double k = 10;
            double kx = k / Dx;
            double ky = k / Dy;
            double px = 1;
            double py = 1;
            
            Func<AdaptiveGrid<double[]>.IVoxel, double[]> ignitor = 
                (V) => new double[] { V.Origin[0] + 0.5 * V.GridStep[0], V.Origin[1] + 0.5 * V.GridStep[1] };

            Func<AdaptiveGrid<double>.IVoxel, double> pulse = delegate (AdaptiveGrid<double>.IVoxel V)
            {
                double x = V.Origin[0] + 0.5 * V.GridStep[0];
                double y = V.Origin[1] + 0.5 * V.GridStep[1];

                x = kx * Math.Pow(x - xo, 2 * px);
                y = ky * Math.Pow(y - yo, 2 * py);

                return a / (x + y + 1);
            };

            int TESTnO = 0;

            #region 0_Simple_2dGrid
            if (TESTnO == 0)
            {
                double[,] shape = new double[2, 3] { { 0, Dx, 4 }, { 0, Dy, 4 } };
                var agrid = new AdaptiveGrid<double>(32, shape, new int[] { 2, 2 }, 4, pulse);

                // --- Traversal ---
                var iV = agrid.GetInnermost(agrid.InternalArray[0]);
                int index = 0;

                Console.WriteLine("{3,3},   {0,4}, {1,4}, {2,6}", "x", "y", "value", "ind");

                do
                {
                    Console.WriteLine("{3,3},   {0,4}, {1,4}, {2,6:N2}", iV.Origin[0], iV.Origin[1], iV.Data, index++);
                }
                while (agrid.NextInnermost(ref iV));

                Console.WriteLine("----------------------------------------\n\r");


                // --- Tessellation ---
                agrid.Tessellate(new double[] { 60, 4 }, initializer: pulse);

                iV = agrid.GetInnermost(agrid.InternalArray[0]);
                index = 0;

                do
                {
                    Console.WriteLine("{3,3},   {0,4}, {1,4}, {2,6:N2}", iV.Origin[0], iV.Origin[1], iV.Data, index++);
                }
                while (agrid.NextInnermost(ref iV));

                Console.WriteLine("----------------------------------------\n\r");


                // --- Tessellation ---
                agrid.Tessellate(new double[] { 100, 6 }, initializer: pulse);

                iV = agrid.GetInnermost(agrid.InternalArray[0]);
                index = 0;

                do
                {
                    Console.WriteLine("{3,3},   {0,4}, {1,4}, {2,6:N2}", iV.Origin[0], iV.Origin[1], iV.Data, index++);
                }
                while (agrid.NextInnermost(ref iV));

                Console.WriteLine("----------------------------------------\n\r");


                // --- Tessellation ---
                agrid.Tessellate(new double[] { 60, 4 }, initializer: pulse);
                agrid.Tessellate(new double[] { 60, 4 }, initializer: pulse);
                agrid.Tessellate(new double[] { 60, 4 }, initializer: pulse);
                agrid.Tessellate(new double[] { 60, 6 }, initializer: pulse);

                iV = agrid.GetInnermost(agrid.InternalArray[0]);
                index = 0;

                do
                {
                    Console.WriteLine("{3,3},   {0,4}, {1,4}, {2,6:N2}", iV.Origin[0], iV.Origin[1], iV.Data, index++);
                }
                while (agrid.NextInnermost(ref iV));

                Console.WriteLine("----------------------------------------\n\r");


                // --- Merge ---
                agrid.InternalArray[5].Merge_R(true, pulse);

                iV = agrid.GetInnermost(agrid.InternalArray[0]);
                index = 0;

                do
                {
                    Console.WriteLine("{3,3},   {0,4}, {1,4}, {2,6:N2}", iV.Origin[0], iV.Origin[1], iV.Data, index++);
                }
                while (agrid.NextInnermost(ref iV));

                Console.WriteLine("----------------------------------------\n\r");


                // --- Traversal of the specified level ---
                bool tessFlag = false;
                tessFlag = agrid.Tessellate(new double[] { 0, 0 }, initializer: pulse);
                tessFlag = agrid.Tessellate(new double[] { 0, 0 }, initializer: pulse);
                tessFlag = agrid.Tessellate(new double[] { 0, 0 }, initializer: pulse);
                tessFlag = agrid.Tessellate(new double[] { 0, 0 }, initializer: pulse);

                tessFlag = agrid.Tessellate(new double[] { 97, 8 }, initializer: pulse);
                tessFlag = agrid.Tessellate(new double[] { 97, 8 }, initializer: pulse);
                tessFlag = agrid.Tessellate(new double[] { 97, 8 }, initializer: pulse);
                tessFlag = agrid.Tessellate(new double[] { 97, 8 }, initializer: pulse);

                int ilevel = 4;
                index = 0;

                if (!agrid.FirstAtLevel(ilevel, out iV))
                {
                    Console.WriteLine("There are no voxels at {0} level", ilevel);
                    Console.WriteLine("----------------------------------------\n\r");
                    goto END;
                }

                do
                {
                    Console.WriteLine("{3,3},   {0,4}, {1,4}, {2,6:N2}", iV.Origin[0], iV.Origin[1], iV.Data, index++);
                }
                while (agrid.NextAtLevel(ref iV));

                Console.WriteLine("----------------------------------------\n\r");
            }
            #endregion


            #region 1_Dense_2dGrid
            if (TESTnO == 1)
            {
                double[,] shape = new double[2, 3] { { 0, Dx, 16 }, { 0, Dy, 16 } };
                var agrid = new AdaptiveGrid<double>(2*16*16, shape, null, 4, pulse);

                // --- Tessellation ---
                agrid.Tessellate(new double[] { 62, 4.5 }, initializer: pulse);
                agrid.Tessellate(new double[] { 65, 4.5 }, initializer: pulse);
                agrid.Tessellate(new double[] { 62, 5.1 }, initializer: pulse);
                agrid.Tessellate(new double[] { 65, 5.1 }, initializer: pulse);

                var iV = agrid.GetInnermost(agrid.InternalArray[0]);
                int index = 0;

                StringBuilder SB = new StringBuilder(33 * 1000);
                SB.AppendFormat("{3,3},   {0,8}, {1,8}, {2,8}\n\r", "x", "y", "value", "ind");

                do
                {
                    SB.AppendFormat("{3,3},   {0,8:G6}, {1,8:G6}, {2,8:G6}\n\r", iV.Origin[0], iV.Origin[1], iV.Data, index++);
                }
                while (agrid.NextInnermost(ref iV));

                SB.AppendLine("----------------------------------------");
                Console.WriteLine(SB.ToString());
            }
        #endregion

        END:
            Console.WriteLine(">> Press any key to exit <<");
            Console.ReadKey();
        }
    }
}