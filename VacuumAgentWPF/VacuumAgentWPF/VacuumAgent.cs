﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Grab = 6
        }

        public enum Algorithm
        {
            BFS = 0,
            IDS = 1,
            ASTAR = 2,
            RBFS = 3
        }

        public static bool _init = false;

        public static Vector2 _pos;

        public static Algorithm _currentAlgorithm;

        public static void Init() {
            // Choose random location in the grid as  starting point
            Random rand = new Random(5051);
            _pos = new Vector2(rand.Next(Environment._gridDim.X), rand.Next(Environment._gridDim.Y));

            _init = true;

            _currentAlgorithm = Algorithm.ASTAR;
        }

        public static void VacuumProc()
        {
            // Wait for the environment to be ready
            while (!Environment._init) { }
            Init();

            Console.WriteLine(3 & Environment.DIRT);

            Stack<VacuumAction> intent = new Stack<VacuumAction>();
            while (true)
            {
                if (intent.Count == 0)
                {
                    // Get environment current state
                    int[,] belief = Environment._grid;
                    CustomEnvState currentState = new CustomEnvState(belief, _pos);
                    // The agent only move if at least one room is dirty
                    if (currentState.NbOfDirtyRoom > 0)
                    {
                        // Formulate Goal
                        // We define the goal for this agent as cleaning one dirty room
                        CustomEnvState wishedState = new CustomEnvState(belief, _pos);
                        wishedState.DefineWishedRoomDirtyAs(currentState.NbOfDirtyRoom - 1);
                        wishedState.MarkStateForEquality(CustomEnvState.ROOM_STATE);
                        // Formulate problem
                        Problem problem = new Problem(currentState, wishedState);
                        // Explore
                        intent = Explore(problem,_currentAlgorithm);
                    }
                }
                else
                {
                    // Execute and remove one step of the action's plan
                    Environment.Print();
                    VacuumAction action = intent.Pop();
                    Console.WriteLine("Next Action = " + action);
                    Execute(action);
                    Thread.Sleep(1000);
                }
            }
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
            if (state.ContainJewel())
            {
                actions.Add(VacuumAction.Grab);
            }
            if (state.IsDirty()) {
                actions.Add(VacuumAction.Clean);
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
                    _pos.Y += 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAction.GoDown:
                    _pos.Y -= 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAction.GoRight:
                    _pos.X += 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAction.GoLeft:
                    _pos.X -= 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAction.Clean:
                    Environment.CleanCell(_pos);
                    break;
                case VacuumAction.Grab:
                    Environment.TryGrabbing(_pos);
                    break;
                default:
                    break;
            }
        
        }

    }
}
