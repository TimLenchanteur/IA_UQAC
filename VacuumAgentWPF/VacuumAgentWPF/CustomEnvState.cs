using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class CustomEnvState :EnvState
    {
        public const int GRID_STATE_ATTRIBUTE = 1;
        public const int NUMBER_DIRTY_ROOM_ATTRIBUTE = 2;
        public const int AGENT_POSTION_ATTRIBUTE = 3;

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

        public CustomEnvState(int[,] envGridState, Vector2 agentPos)
        {
            InitState(envGridState, agentPos);
        }

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

        public override bool IsEqualRelativeToTestedAttribute(EnvState otherState) 
        {
            CustomEnvState otherCustomState = otherState as CustomEnvState;
            if (otherCustomState != null)
            {
                switch (_markedState) {
                    case GRID_STATE_ATTRIBUTE:
                        break;
                    case NUMBER_DIRTY_ROOM_ATTRIBUTE:
                        // This is the only one we will use, don't need any other
                        return otherCustomState._nbOfDirtyRoom == _nbOfDirtyRoom;
                    case AGENT_POSTION_ATTRIBUTE:
                        break;
                    default:
                        break;
                }
            }
            return base.IsEqualRelativeToTestedAttribute(otherState);
        }

        public override bool Equals(Object obj)
        {
            CustomEnvState otherState = obj as CustomEnvState;
            if (otherState == null)
            {
                return false;
            }
            else
            {
                return _agentPos.Equals(otherState._agentPos) && _nbOfDirtyRoom == otherState._nbOfDirtyRoom;
            }
        }

        public void DefineWishedGridStateAs(List<Vector2> dirtyRoom, List<Vector2> dirtyRoomWithJewel) {
            _dirtyRoom = dirtyRoom;
            _dirtyRoomWithJewel = dirtyRoomWithJewel;
        }

        public void DefineWishedRoomDirtyAs(int wishedNumber) {
            _nbOfDirtyRoom = wishedNumber;
        }

        public void DefineWishedAgentPosition(Vector2 wishedPos)
        {
            _agentPos = wishedPos;
        }

        public bool IsDirty() {
            return _dirtyRoom.Contains(_agentPos) || _dirtyRoomWithJewel.Contains(_agentPos);
        }

        public bool ContainJewel() {
            return _dirtyRoomWithJewel.Contains(_agentPos);
        }

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
                    // This imply that the agent execute this action only when he is on a dirty room
                    _nbOfDirtyRoom -= 1;
                    break;
                case VacuumAgent.VacuumAction.GrabClean:
                    if (_dirtyRoomWithJewel.Contains(_agentPos)) _dirtyRoomWithJewel.Remove(_agentPos);
                    // This imply that the agent execute this action only when he is on a dirty room
                    _nbOfDirtyRoom -= 1;
                    break;
                default:
                    break;
            }
            return;
        }

        public float EuclidianActionHeuristic() {
            float minDistance = float.MaxValue;

            float tempCost = 0;
            foreach (Vector2 dirtyRoom in _dirtyRoom) {
                tempCost = (dirtyRoom - _agentPos).Magnitude();
                if (minDistance >= tempCost)  minDistance = tempCost + 1;
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
