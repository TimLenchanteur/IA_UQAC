using System;
using System.Threading;

namespace VacuumAgentWPF
{
    class Environment
    {
        // Objets possible sur le sol
        // Peuvent être utilisés pour des comparaison bit à bit
        public const int NONE = 0;
        public const int DIRT = 1;
        public const int JEWEL = 2;

        // Coût de chaque action prise par l'agent
        const int ACTION_COST = 1;
        const float PENALTY = 1.2f;
        const float BONUS = 0.95f; 

        // Score de performance de l'agent la dernière fois qu'elle a atteint son but (plus le score est bas, plus la performance est bonne)
        private static float _perfScore;

        // La grille a-t-elle été initialisée ?
        public static bool _init = false;

        // Dimensions de l'environnement
        public static Vector2 _gridDim = new Vector2(0, 0);
        // Modélisation de l'environnement
        public static int[,] _grid;

        public static void Init()
        {
            // Initialisation de l'environnement
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

            MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.InitializeEnvironmentImage());
            _init = true;
        }

        public static void EnvironmentProc()
        {
            Init();

            int[] possibleGeneratedObject = { NONE, DIRT, NONE, JEWEL, NONE,  DIRT, DIRT};

            Random rand = new Random();
            while (true)
            {
                // Choix d'une pièce aléatoire
                int randPosX = rand.Next(_gridDim.X);
                int randPosY = rand.Next(_gridDim.Y);

                // Choix d'un objet aléatoire à placer dans la pièce
                int randObjectIndex = rand.Next(possibleGeneratedObject.Length);
                int randObject = possibleGeneratedObject[randObjectIndex];

                // Vérification qu'il n'y a pas déjà un objet de ce type dans la pièce
                if ((_grid[randPosX, randPosY] & randObject) == 0)
                {
                    //Ajout à la pièce
                    _grid[randPosX, randPosY] += randObject;
                }

                // Choix d'un temps aléatoire avant de recommencer
                int randTimeToWait = rand.Next(1000) + 500;
                MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.UpdateEnvironment());
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
            return 1/_perfScore;
        }

        // Aucun nouvel obstacle n'apparaît, l'agent est donc toujours libre de se déplacer où il le souhaite
        public static void MoveAgent(in Vector2 pos) {
            float perf = ACTION_COST;
            if ((_grid[pos.X, pos.Y]& DIRT) == DIRT) perf *= PENALTY;
            AddToPerf(perf);
        }

        public static bool TryGrabbing(in Vector2 pos) {
            // Ajout du coût quoi qu'il arrive
            AddToPerf(ACTION_COST);
            if ((_grid[pos.X, pos.Y] & JEWEL) == JEWEL) {
                // S'il y a effectivement un bijou, on le ramasse
                _grid[pos.X, pos.Y] -= JEWEL;
                return true;
            }
            return false;
        }

        public static bool CleanCell(in Vector2 pos)
        {
            float perf = ACTION_COST;
            if ((_grid[pos.X, pos.Y] & JEWEL) == JEWEL) {
                // Ajout d'une pénalité car l'agent s'apprête à aspirer un bijou
                perf *= PENALTY;
            }
            perf *= BONUS;
            //Clean cell
            _grid[pos.X, pos.Y] = NONE;
            AddToPerf(perf);
            return true;
        }

        public static void Print()
        {
            Console.WriteLine("--------------------------------------------");
            for (int x = 0; x < _gridDim.X; x++)
            {
                for (int y = 0; y < _gridDim.Y; y++)
                {
                    if(VacuumAgent._pos.Equals(new Vector2(x, y)))
                    {
                        Console.Write("R");
                        continue;
                    }
                    if(_grid[x, y] == NONE)
                    {
                        Console.Write("0");
                    }
                    else if (_grid[x, y] == DIRT)
                    {
                        Console.Write("%");
                    }
                    else if (_grid[x, y] == JEWEL)
                    {
                        Console.Write("$");
                    }
                    else if (_grid[x, y] == 3)
                    {
                        Console.Write("*");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
