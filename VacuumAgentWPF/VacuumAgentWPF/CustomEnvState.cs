using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    /// <summary>
    /// Representation d'un etat de l'environement pour l'application VacuumAgentWPF
    /// </summary> 
    class CustomEnvState :EnvState
    {
        /// <summary>
        /// Marqueur d'attribut a testé
        /// </summary>
        public const int GRID_STATE_ATTRIBUTE = 1;
        public const int NUMBER_DIRTY_ROOM_ATTRIBUTE = 2;
        public const int AGENT_POSTION_ATTRIBUTE = 3;

        /// <summary>
        /// Attribut representant l'etat
        /// _dirtyRoom : Liste des salles sales
        /// _dirtyRoomWithJewel : Liste des salles sales contenant un bijou
        /// _agentPos :  Position de l'agent dans l'environement
        /// </summary>
        List<Vector2> _dirtyRoom;
        List<Vector2> _dirtyRoomWithJewel;
        int _nbOfDirtyRoom;
        public int NbOfDirtyRoom
        {
            get => _nbOfDirtyRoom;
        }
        Vector2 _agentPos;
        public Vector2 Agent_Pos
        {
            get => _agentPos;
        }


        /// <summary>
        /// Fonction qui calcul l'etat d'un environement a partir des informations reccueilli dans cette environement
        /// </summary>
        /// <param name="envGridState">Grille representant l'etat de chaque salle</param>
        /// <param name="agentPos">Position de l'agent sur la grille</param>
        private void InitState(int[,] envGridState, Vector2 agentPos) {
            _dirtyRoom = new List<Vector2>();
            _dirtyRoomWithJewel = new List<Vector2>();
            for (int x = 0; x < Environment._gridDim.X; x++)
            {
                for (int y = 0; y < Environment._gridDim.Y; y++)
                {
                    if ((envGridState[x, y] & Environment.DIRT) == Environment.DIRT)
                    {
                        Vector2 currentPos = new Vector2(x, y);
                        int state = envGridState[x, y];
                        if ((state & Environment.JEWEL) == Environment.JEWEL)
                        {
                            _dirtyRoomWithJewel.Add(currentPos);
                        }
                        else {
                            _dirtyRoom.Add(currentPos);
                        } 
                        
                    }
                }
            }
            _nbOfDirtyRoom = _dirtyRoom.Count + _dirtyRoomWithJewel.Count;
            _agentPos = new Vector2(agentPos.X, agentPos.Y);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="envGridState">Grille representant l'etat de chaque salle</param>
        /// <param name="agentPos">Position de l'agent sur la grille</param>
        public CustomEnvState(int[,] envGridState, Vector2 agentPos)
        {
            InitState(envGridState, agentPos);
        }

        /// <summary>
        /// Constructeur d'un etat a partir d'un autre etat et d'une action effectue
        /// </summary>
        /// <param name="previousState">Etat a partir duquel cette etat va etre construit</param>
        /// <param name="newAction">Action effectue sur l'etat donne pour arriver a cette etat</param>
        public CustomEnvState(CustomEnvState previousState, VacuumAgent.VacuumAction newAction)
        {
            _dirtyRoom = new List<Vector2>();
            _dirtyRoomWithJewel = new List<Vector2>();
            foreach (Vector2 dirtyRoom in previousState._dirtyRoom) {
                _dirtyRoom.Add(dirtyRoom);
            }
            foreach (Vector2 dirtyJewelRoom in previousState._dirtyRoomWithJewel) {
                _dirtyRoomWithJewel.Add(dirtyJewelRoom);
            }
            _nbOfDirtyRoom = previousState._nbOfDirtyRoom;
            _agentPos = new Vector2(previousState._agentPos.X, previousState._agentPos.Y);
            _markedState = previousState._markedState;

            ExecuteNewAction(newAction);
        }

        /// <summary>
        /// Test si les etats sont egals mais seulement relativement a l'attribut marqué
        /// </summary>
        /// <param name="otherState">L'etat dont l'on test l'egalite</param>
        /// <returns></returns>
        public override bool IsEqualRelativeToTestedAttribute(EnvState otherState) 
        {
            CustomEnvState otherCustomState = otherState as CustomEnvState;
            if (otherCustomState != null)
            {
                switch (_markedState) {
                    case GRID_STATE_ATTRIBUTE:
                        break;
                    case NUMBER_DIRTY_ROOM_ATTRIBUTE:
                        // Seul attributs que l'on utilise pour cette application actuellement
                        return otherCustomState._nbOfDirtyRoom == _nbOfDirtyRoom;
                    case AGENT_POSTION_ATTRIBUTE:
                        break;
                    default:
                        break;
                }
            }
            return base.IsEqualRelativeToTestedAttribute(otherState);
        }

        /// <summary>
        /// Test d'egalite 
        /// </summary>
        /// <param name="obj">L'objet dont on test l'egalite avec l'etat</param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            CustomEnvState otherState = obj as CustomEnvState;
            if (otherState == null)
            {
                return false;
            }
            else
            {
                if (_agentPos.Equals(otherState._agentPos)) 
                {
                    if(_dirtyRoom.Count == otherState._dirtyRoom.Count && _dirtyRoomWithJewel.Count == otherState._dirtyRoomWithJewel.Count)
                    {
                        foreach(Vector2 room in _dirtyRoom)
                        {
                            if (!otherState._dirtyRoom.Contains(room))
                            {
                                return false;
                            }
                        }
                        foreach (Vector2 room in _dirtyRoomWithJewel)
                        {
                            if (!otherState._dirtyRoomWithJewel.Contains(room))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// Defini l'etat de la grille souhaité
        /// </summary>
        /// <param name="dirtyRoom">Liste de salle sales souhaités</param>
        /// <param name="dirtyRoomWithJewel">Liste des salles sales avec bijou souhaités</param>
        public void DefineWishedGridStateAs(List<Vector2> dirtyRoom, List<Vector2> dirtyRoomWithJewel) {
            _dirtyRoom = dirtyRoom;
            _dirtyRoomWithJewel = dirtyRoomWithJewel;
        }

        /// <summary>
        /// Defini le nombre de salles sales souhaitées
        /// </summary>
        /// <param name="wishedNumber">Nombre de salles sales souhaités</param>
        public void DefineWishedRoomDirtyAs(int wishedNumber) {
            _nbOfDirtyRoom = wishedNumber;
        }

        /// <summary>
        /// Defini la position souhaité de l'agent
        /// </summary>
        /// <param name="wishedPos">Position souhaité</param>
        public void DefineWishedAgentPosition(Vector2 wishedPos)
        {
            _agentPos = wishedPos;
        }

        /// <summary>
        /// L'agent se trouve t'il sur une salle sale ?
        /// </summary>
        /// <returns>true si c'est le cas, false sinon</returns>
        public bool IsDirty() {
            return _dirtyRoom.Contains(_agentPos) || _dirtyRoomWithJewel.Contains(_agentPos);
        }

        /// <summary>
        /// La sale sur laquelle se trouve l'agent contient t'elle un bijou ?
        /// </summary>
        /// <returns>true si c'est le cas, false sinon</returns>
        public bool ContainJewel() {
            return _dirtyRoomWithJewel.Contains(_agentPos);
        }

        /// <summary>
        /// Simule l'execution d'une action
        /// </summary>
        /// <param name="action">Action a simulé</param>
        public void ExecuteNewAction(VacuumAgent.VacuumAction action) {
            switch (action) {
                case VacuumAgent.VacuumAction.GoUp:
                    _agentPos.Y += 1;
                    break;
                case VacuumAgent.VacuumAction.GoDown:
                    _agentPos.Y -= 1;
                    break;
                case VacuumAgent.VacuumAction.GoRight:
                    _agentPos.X += 1;
                    break;
                case VacuumAgent.VacuumAction.GoLeft:
                    _agentPos.X -= 1;
                    break;
                case VacuumAgent.VacuumAction.Clean:
                    if (_dirtyRoom.Contains(_agentPos)) _dirtyRoom.Remove(_agentPos);
                    else if (_dirtyRoomWithJewel.Contains(_agentPos)) _dirtyRoomWithJewel.Remove(_agentPos);
                    // Implique que l'on a verifie que la salle est bien sale
                    _nbOfDirtyRoom -= 1;
                    break;
                case VacuumAgent.VacuumAction.GrabClean:
                    if (_dirtyRoomWithJewel.Contains(_agentPos)) _dirtyRoomWithJewel.Remove(_agentPos);
                    // Implique que l'on a verifie que la salle est bien sale
                    _nbOfDirtyRoom -= 1;
                    break;
                default:
                    break;
            }
            return;
        }

        /// <summary>
        /// Heuristique donnant le coup pour acceder a la salles sales la plus proche + le coup des actions pour la vider
        /// </summary>
        /// <returns>Le coup pour vider la salle sale la plus proche</returns>
        public float EuclidianActionHeuristic() {
            float minDistance = float.MaxValue;

            float tempCost = 0;
            foreach (Vector2 dirtyRoom in _dirtyRoom) {
                tempCost = (dirtyRoom - _agentPos).Magnitude() + 1;
                if (minDistance >= tempCost)  minDistance = tempCost;
            }
            foreach (Vector2 dirtyJewelRoom in _dirtyRoomWithJewel)
            {
                tempCost = (dirtyJewelRoom - _agentPos).Magnitude() + 2;
                if (minDistance >= tempCost) minDistance = tempCost;
            }
            return minDistance;
        }
    }
}
