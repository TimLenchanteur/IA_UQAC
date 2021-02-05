using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace VacuumAgentWPF
{
    class Environment
    {
        // Possible object on the floor
        // These can be used as bitwise operation (Ex: if box state is 3 then it mean there is both dirt and a jewel)
        public const int NONE = 0;
        public const int DIRT = 1;
        public const int JEWEL = 2;

        // Cost for every action taken by the agent
        const int ACTION_COST = 1;
        // Performance score for the last time the agent executed his goal
        private static float _perfScore;

        // Has the grid been initiated
        public static bool _init = false;

        // Environenment dimensions
        public static Vector2 _gridDim = new Vector2(0, 0);
        // Environment modelisation
        public static int[,] _grid;

        public static void Init()
        {
            // Init environment grid (grid size has been decided in the instructions)
            _gridDim.X = 5;
            _gridDim.Y = 5;

            _grid = new int[_gridDim.X, _gridDim.Y];
            for (int x = 0; x < _gridDim.X; x++)
            {
                for (int y = 0; y < _gridDim.Y; y++)
                {
                    _grid[x, y] = NONE;
                }
            }

            _init = true;
        }

        public static void EnvironmentProc()
        {
            Init();

            int[] possibleGeneratedObject = { NONE, NONE, NONE, DIRT, DIRT, JEWEL };

            Random rand = new Random();
            while (true)
            {
                // Choose a random box in grid
                int randPosX = rand.Next(_gridDim.X);
                int randPosY = rand.Next(_gridDim.Y);

                // Choose a random object to place in this box (messed a bit with the array so that the chance to get each one is not the same)
                int randObjectIndex = rand.Next(possibleGeneratedObject.Length);
                int randObject = possibleGeneratedObject[randObjectIndex];

                // Check if there is no object of this type already placed here 
                if ((_grid[randPosX, randPosY] & randObject) == 0)
                {
                    //Add it to the box
                    _grid[randPosX, randPosY] += randObject;
                }

                // Choose a random amount of time to fill the grid again then wait
                int randTimeToWait = rand.Next(1000) + 2000;
                Thread.Sleep(randTimeToWait);
            }
        }


        public static void AddToPerf(float cost) {
            _perfScore += cost;
        }

        public static void ResetPerf() {
            _perfScore = 0.0f;
        }

        public static float GivePerf()
        {
            return _perfScore;
        }

        /// <summary>
        /// 
        /// 
        /// No new obstacle appear and the grid bound stay the same so the agent should always be able to move if he plan to
        /// </summary>
        public static void MoveAgent() {
            AddToPerf(ACTION_COST);
        }

        public static bool TryGrabbing(in Vector2 pos) {
            // Add cost either way
            AddToPerf(ACTION_COST);
            if ((_grid[pos.X, pos.Y] & JEWEL) == 1) {
                // If there is indeed a jewel, grab it 
                _grid[pos.X, pos.Y] -= JEWEL;
                return true;
            }
            return false;
            
        }

        public static bool CleanCell(in Vector2 pos)
        {
            float costMultiplier = 1.0f;
            if ((_grid[pos.X, pos.Y] & JEWEL) == 1) {
                // Add penalty because the agent is about to suck a jewel
                costMultiplier += 0.5f;
            }
            //Clean cell
            _grid[pos.X, pos.Y] = NONE;
            AddToPerf(ACTION_COST*costMultiplier);
            return true;
        }
    }
}
