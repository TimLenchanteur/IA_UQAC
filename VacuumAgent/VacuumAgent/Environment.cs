using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VacuumAgent
{
    class Environment
    {
        public static int gridWidth;
        public static int gridHeight;
        public static int[,] grid;

        public static void Init(Thread agent)
        {
            // Init environment grid 
            /* gridWidth;
               gridHeight;
             */
            grid = new int[gridHeight,gridWidth];

            if (agent != null) agent.Start();
        }

        public static void EnvironmentProc(Object obj)
        {
            Thread agent;
            try
            {
                agent = (Thread)obj;
            }
            catch (InvalidCastException)
            {
                agent = null;
            }

            Init(agent);

            while (true)
            {

            }

        }
    }
}
