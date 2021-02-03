using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VacuumAgent
{
    class Environment
    {
        //Possible object on the floor
        //These can be used as bitwise operation (Ex: if box state is 3 then it mean there is both dirt and a jewel)
        const int NONE = 0;
        const int DIRT = 1;
        const int JEWEL = 2;
        const int BOT = 4;

        public static int _gridWidth;
        public static int _gridHeight;
        public static int[,] _grid;

        public static void Init(Thread agent)
        {
            // Init environment grid (grid size has been decided in the instructions)
            _gridWidth = 5;
            _gridHeight = 5;

            _grid = new int[_gridWidth, _gridHeight];
            for(int x = 0; x< _gridWidth; x++)
            {
                for(int y = 0; y<_gridHeight; y++)
                {
                    _grid[x, y] = NONE;
                }
            }

            Console.Write("Launching Project");

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

            int[] possibleGeneratedObject = { NONE , NONE, NONE, DIRT, DIRT, JEWEL };

            Random rand = new Random();
            while (true)
            {
                // Choose a random box in grid
                int randPosX = rand.Next(_gridWidth);
                int randPosY = rand.Next(_gridHeight);

                // Choose a random object to place in this box (messed a bit with the array so that the chance to get each one is not the same)
                int randObjectIndex = rand.Next(possibleGeneratedObject.Length);
                int randObject = possibleGeneratedObject[randObjectIndex];

                // Check if there is no object of this type already placed here 
                if((_grid[randPosX, randPosY] & randObject) == 0)
                {
                    //Add it to the box
                    _grid[randPosX, randPosY] += randObject;
                }

                // Choose a random amount of time to fill the grid again then wait
                int randTimeToWait = rand.Next(1000) + 2000;
                Thread.Sleep(randTimeToWait);
            }
        }
    }
}
