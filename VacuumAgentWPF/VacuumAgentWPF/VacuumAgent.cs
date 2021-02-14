using System;
using System.Collections.Generic;
using System.Threading;

namespace VacuumAgentWPF
{
    class VacuumAgent
    {
        public enum VacuumAction
        {
            None = 0,
            GoRight = 1,
            GoLeft = 2,
            GoUp = 3,
            GoDown = 4,
            Clean = 5,
            GrabClean = 6
        }

        public enum Algorithm
        {
            BFS = 0,
            ASTAR = 1
        }

        public static int _learningCycle = 5;

        public static bool _init = false;

        public static Vector2 _pos;

        public static Algorithm _nextAlgo;
        public static Algorithm _currentAlgorithm;

        public static int _actionsCount;

        public static int _actionCycle;

        public static int _learningCount;

        public static int _optimalActionCycle;

        public static List<KeyValuePair<int, float>> _lastActionsCycleTrack;

        public static void Init() {
            // Choose random location in the grid as  starting point
            Random rand = new Random();
            _pos = new Vector2(rand.Next(Environment._gridDim.X), rand.Next(Environment._gridDim.Y));

            _init = true;

            _currentAlgorithm = Algorithm.BFS;

            _optimalActionCycle = 0;
            _lastActionsCycleTrack = new List<KeyValuePair<int, float>>();
            _actionsCount = 0;
            _learningCount = 0;
        }

        private static int WeightedRandom(int min, int max) {
            int baseNumber = 100;
            int interval = max - min;
            List<int> chooseInside = new List<int>();
            int numberLinked = min;
            for (int i = 0; i <= interval; i++) {
                int howManyInsideList = baseNumber / (i + 1);
                for (int j = 0; j < howManyInsideList; j++) {
                    chooseInside.Add(numberLinked);
                }
                numberLinked++;
            }
            Random rand = new Random();
            return chooseInside[rand.Next(0, chooseInside.Count)];
        }
        

        public static void VacuumProc()
        {
            // Wait for the environment to be ready
            while (!Environment._init) { }
            Init();

            Console.WriteLine(3 & Environment.DIRT);

            Stack<VacuumAction> intent = new Stack<VacuumAction>();
          
            _actionCycle = 0;
            while (true)
            {
                if (intent.Count == 0 || _actionsCount>=_actionCycle)
                {
                    // Get environment current state
                    int[,] belief = Environment._grid;
                    CustomEnvState currentState = new CustomEnvState(belief, _pos);
                    // The agent only move if at least one room is dirty
                    if (currentState.NbOfDirtyRoom > 0)
                    {
                        if (_nextAlgo != _currentAlgorithm)
                        {
                            _currentAlgorithm = _nextAlgo;
                            MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.UpdateAlgo(_currentAlgorithm.ToString()));
                        }

                        // Keep track of performances
                        if (_actionsCount != 0)
                        {
                            if (_learningCount >= _learningCycle - 1)
                            {
                                _optimalActionCycle = ComputeOptimalActionCycle();
                                MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.UpdateOptimalActions());
                                _learningCount = 0;
                            }
                            else _learningCount++;
                            _lastActionsCycleTrack.Add(new KeyValuePair<int, float>(_actionsCount, Environment.GivePerf()));
                            MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.AddLearnedAction(_actionsCount, Environment.GivePerf()));
                            Environment.ResetPerf();
                            _actionsCount = 0;
                        }

                        // Formulate Goal
                        // We define the goal for this agent as cleaning one dirty room
                        CustomEnvState wishedState = new CustomEnvState(belief, _pos);
                        wishedState.DefineWishedRoomDirtyAs(currentState.NbOfDirtyRoom - 1);
                        wishedState.MarkAttributeForEquality(CustomEnvState.NUMBER_DIRTY_ROOM_ATTRIBUTE);
                        // Formulate problem
                        Problem problem = new Problem(currentState, wishedState);
                        // Explore
                        intent = Explore(problem,_currentAlgorithm);
                        // Update optimal action cycle
                        _actionCycle = _optimalActionCycle == 0 ? intent.Count : _optimalActionCycle + WeightedRandom(0, Math.Max(intent.Count - _optimalActionCycle, 0)); 
                        MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.UpdateActionCycle());
                        MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.UpdateComputingState(""));
                    }
                }
                else if(_actionsCount<_actionCycle)
                {
                    _actionsCount++;
                    // Execute and remove one step of the action's plan
                    VacuumAction action = intent.Pop();
                    Execute(action);
                    Thread.Sleep(700);
                }
            }
        }

        static int ComputeOptimalActionCycle() {
            float result = 0;
            float coeff = 0;
            foreach (KeyValuePair<int, float> pair in _lastActionsCycleTrack) {
                result += pair.Key * pair.Value;
                coeff += pair.Value;
            }
            if (coeff != 0) result /= coeff;
            return (int)result;
        }

        public static void ChangeExplorationAlgo(Algorithm newAlgo)
        {
            _nextAlgo = newAlgo;
        }

        public static void ChangeLearningCycle(int newCycle) {
            _learningCycle = newCycle;
        }


        static Stack<VacuumAction> Explore(in Problem problem, in Algorithm algorithm)
        {
            Stack<VacuumAction> newintent = new Stack<VacuumAction>();
            switch (algorithm) {
                case Algorithm.BFS:
                    return BFSAlgo.ExecuteFor(problem);
                case Algorithm.ASTAR:
                    return AStarAlgo.ExecuteFor(problem);
                default:
                    break;
            }

            return newintent;
        }

        public static List<VacuumAction> PossibleActionFromThere(CustomEnvState state)
        {
            List<VacuumAction> actions = new List<VacuumAction>();
            Vector2 posAgent = state.Agent_Pos;
            if (state.IsDirty())
            {
                if (state.ContainJewel()) actions.Add(VacuumAction.GrabClean);
                else actions.Add(VacuumAction.Clean);
            }
            if ((posAgent.X - 1) >=  0) {
                actions.Add(VacuumAction.GoLeft);
            }
            if ((posAgent.X + 1) < Environment._gridDim.X)
            {
                actions.Add(VacuumAction.GoRight);
            }
            if ((posAgent.Y - 1) >= 0)
            {
                actions.Add(VacuumAction.GoDown);
            }
            if ((posAgent.Y + 1) < Environment._gridDim.Y)
            {
                actions.Add(VacuumAction.GoUp);
            }
            return actions;
        }

        static void Execute(VacuumAction action) {
            switch (action) {
                case VacuumAction.GoUp:
                    Environment.MoveAgent(_pos);
                    _pos.Y += 1;
                    break;
                case VacuumAction.GoDown:
                    Environment.MoveAgent(_pos);
                    _pos.Y -= 1;
                    break;
                case VacuumAction.GoRight:
                    Environment.MoveAgent(_pos);
                    _pos.X += 1;
                    break;
                case VacuumAction.GoLeft:
                    Environment.MoveAgent(_pos);
                    _pos.X -= 1;
                    break;
                case VacuumAction.Clean:
                    Environment.CleanCell(_pos);
                    MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.DisplayClean());
                    break;
                case VacuumAction.GrabClean:
                    Environment.TryGrabbing(_pos);
                    Environment.CleanCell(_pos);
                    MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.DisplayGrab());
                    break;
                default:
                    break;
            }
            MainWindow.Instance.Dispatcher.Invoke(()=>MainWindow.Instance.UpdateRobotPosition(_pos.X, _pos.Y));
            MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.UpdateEnvironment());
        }

    }
}
