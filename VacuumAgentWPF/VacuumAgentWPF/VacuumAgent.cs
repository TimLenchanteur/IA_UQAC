using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            IDS = 1,
            ASTAR = 2,
            RBFS = 3
        }

        const int LEARNING_ROUND = 3;

        public static bool _init = false;

        public static Vector2 _pos;

        public static Algorithm _currentAlgorithm;

        public static int _actionsCount;

        public static int _learningCount;

        public static int _optimalActionCycle;

        public static List<KeyValuePair<int, float>> _lastActionsCycleTrack;

        public static void Init() {
            // Choose random location in the grid as  starting point
            Random rand = new Random(5051);
            _pos = new Vector2(rand.Next(Environment._gridDim.X), rand.Next(Environment._gridDim.Y));

            _init = true;

            _currentAlgorithm = Algorithm.ASTAR;

            _optimalActionCycle = 0;
            _lastActionsCycleTrack = new List<KeyValuePair<int, float>>();
            _actionsCount = 0;
            _learningCount = 0;
        }

        public static void VacuumProc()
        {
            // Wait for the environment to be ready
            while (!Environment._init) { }
            Init();

            Console.WriteLine(3 & Environment.DIRT);

            Stack<VacuumAction> intent = new Stack<VacuumAction>();

            Random rand = new Random(8138);
            int _actionCycle = 0;
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
                        Console.WriteLine("Initial State");
                        Environment.Print();

                        if (_actionsCount != 0) {
                            _lastActionsCycleTrack.Add(new KeyValuePair<int, float>(_actionsCount, Environment.GivePerf()));
                            Environment.ResetPerf();
                            _actionsCount = 0;
                            if (_learningCount >= LEARNING_ROUND-1) _optimalActionCycle = ComputeOptimalActionCycle();
                            else _learningCount++;
                        }

                        // Formulate Goal
                        // We define the goal for this agent as cleaning one dirty room
                        CustomEnvState wishedState = new CustomEnvState(belief, _pos);
                        wishedState.DefineWishedRoomDirtyAs(currentState.NbOfDirtyRoom - 1);
                        wishedState.MarkStateForEquality(CustomEnvState.ROOM_STATE);
                        // Formulate problem
                        Problem problem = new Problem(currentState, wishedState);
                        // Explore
                        intent = Explore(problem,_currentAlgorithm);
                        _actionCycle = _optimalActionCycle == 0 ? intent.Count : _optimalActionCycle + rand.Next(0, Math.Max(intent.Count - _optimalActionCycle,0));
                        Console.WriteLine("Optimal Action Cycle = " + _optimalActionCycle);
                        Console.WriteLine("Next Action Cycle = " + _actionCycle);
                    }
                }
                else if(_actionsCount<_actionCycle)
                {
                    _actionsCount++;
                    // Execute and remove one step of the action's plan
                    //Environment.Print();
                    VacuumAction action = intent.Pop();
                    Console.WriteLine("Next Action = " + action);
                    Execute(action);
                    Thread.Sleep(1000);
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

        static void ChangeExplorationAlgo(int newAlgo) {
            if(newAlgo>0 && newAlgo < 4) _currentAlgorithm = (Algorithm)newAlgo;
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
                    break;
                case VacuumAction.GrabClean:
                    Environment.TryGrabbing(_pos);
                    Environment.CleanCell(_pos);
                    break;
                default:
                    break;
            }
            Console.WriteLine(_pos);
            MainWindow.Instance.Dispatcher.Invoke(()=>MainWindow.Instance.UpdateRobotPosition(_pos.X, _pos.Y));
            MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.UpdateEnvironment());
        }

    }
}
