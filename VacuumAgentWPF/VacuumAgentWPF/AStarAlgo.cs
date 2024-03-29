﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class AStarAlgo
    {

        class AStarNode : IComparable<AStarNode>
        {
            public CustomEnvState _state;
            public VacuumAgent.VacuumAction _action;
            public AStarNode _parentNode = null;
            public float _costToReachSolution;
            public int _costToRechNode;

            public AStarNode(CustomEnvState initialState)
            {
                _state = initialState;
                _costToRechNode = 0;
                _costToReachSolution = _state.EuclidianActionHeuristic();
            }

            public AStarNode(AStarNode previousNode, VacuumAgent.VacuumAction action)
            {
                _action = action;
                _state = new CustomEnvState(previousNode._state, action);
                /// Calcul du cout associe a ce noeud
                /// Le cout pour atteindre ce noeud est donnee par le coup des actions precedente 
                _costToRechNode = previousNode._costToRechNode;
                if(previousNode._action == VacuumAgent.VacuumAction.GrabClean)
                {
                    /// On ajoute 1 si l'action precedente est grab clean car cela represente deux actions
                    _costToRechNode += 2;
                }
                else
                {
                    _costToRechNode += 1;
                }

                _costToReachSolution = _costToRechNode + (action == VacuumAgent.VacuumAction.Clean ?  0 : (action == VacuumAgent.VacuumAction.GrabClean ? 1 : _state.EuclidianActionHeuristic()));
            }

            public int CompareTo(AStarNode other)
            {
                return _costToReachSolution.CompareTo(other._costToReachSolution);
            }
        }

        public static Stack<VacuumAgent.VacuumAction> ExecuteFor(Problem problem)
        {
            Stack<VacuumAgent.VacuumAction> vacuumActions = new Stack<VacuumAgent.VacuumAction>();

            // Lancement de l'algorithme pour trouver un noeud solution du problème
            AStarNode lastNode = RunAlgo(problem);
            if (lastNode == null)
            {
                return vacuumActions;
            }

            // Récupération de toutes les actions conduisant à ce noeud
            while (lastNode._parentNode != null)
            {
                vacuumActions.Push(lastNode._action);
                lastNode = lastNode._parentNode;
            }
            return vacuumActions;
        }

        private static AStarNode RunAlgo(Problem problem)
        {
            // Liste des états déjà visités pour optimisation
            List<CustomEnvState> closed = new List<CustomEnvState>();
            // Frontière des noeuds
            PriorityQueue<AStarNode> fringe = new PriorityQueue<AStarNode>();
            fringe.Enqueue(new AStarNode(problem._initialState));

            while (true)
            {
                if (fringe.Count == 0) return null;
                AStarNode currentNode = fringe.Dequeue();
                if (problem.HasBeenSolved(currentNode._state)) return currentNode;
                // Vérification de l'absence de l'état dans les états visités
                if (!closed.Contains(currentNode._state))
                {
                    closed.Add(currentNode._state);
                    // Ajout des noeuds suivants
                    List<VacuumAgent.VacuumAction> vacuumActions = VacuumAgent.PossibleActionFromThere(currentNode._state);
                    foreach (var action in vacuumActions)
                    {
                        AStarNode newNode = new AStarNode(currentNode, action);
                        newNode._parentNode = currentNode;
                        fringe.Enqueue(newNode);
                    }
                }
            }
        }
    }
}
