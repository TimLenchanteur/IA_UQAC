using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class CustomEnvState :EnvState
    {
        public const int GRID_STATE = 1;
        public const int ROOM_STATE = 2;
        public const int AGENT_STATE = 3;

        int[,] _gridState;
        public int[,] Grid_State
        {
            get => _gridState;
        }
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
            _gridState = new int[Environment._gridDim.X, Environment._gridDim.Y];
            _nbOfDirtyRoom = 0;
            for (int x = 0; x < Environment._gridDim.X; x++)
            {
                for (int y = 0; y < Environment._gridDim.Y; y++)
                {
                    _gridState[x, y] = envGridState[x, y];
                    if ((_gridState[x, y] & Environment.DIRT) == 1)
                    {
                        _nbOfDirtyRoom++;
                    }
                }
            }
            _agentPos = new Vector2(agentPos.X, agentPos.Y);
        }

        public CustomEnvState(int[,] envGridState, Vector2 agentPos)
        {
            InitState(envGridState, agentPos);
        }

        public CustomEnvState(CustomEnvState previousState, VacuumAgent.VacuumAction newAction)
        {
            InitState(previousState._gridState, previousState._agentPos);
            _markedState = previousState._markedState;

            ExecuteNewAction(newAction);
        }

        public override bool IsEqual(EnvState otherState) 
        {
            CustomEnvState otherCustomState = otherState as CustomEnvState;
            if (otherCustomState != null)
            {
                switch (_markedState) {
                    case GRID_STATE:
                        break;
                    case ROOM_STATE:
                        // This is the only one we will use, don't need any other
                        return otherCustomState._nbOfDirtyRoom == _nbOfDirtyRoom;
                    case AGENT_STATE:
                        break;
                    default:
                        break;
                }
            }
            return base.IsEqual(otherState);
        }

        public void DefineWishedGridStateAs(int[,] wishedGrid) {
            _gridState = wishedGrid;
        }

        public void DefineWishedRoomDirtyAs(int wishedNumber) {
            _nbOfDirtyRoom = wishedNumber;
        }

        public void DefineWishedAgentPosition(Vector2 wishedPos)
        {
            _agentPos = wishedPos;
        }

        public void ExecuteNewAction(VacuumAgent.VacuumAction action) {
            switch (action) {
                case VacuumAgent.VacuumAction.GoUp:
                    _agentPos.Y += 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAgent.VacuumAction.GoDown:
                    _agentPos.Y -= 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAgent.VacuumAction.GoRight:
                    _agentPos.X += 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAgent.VacuumAction.GoLeft:
                    _agentPos.Y -= 1;
                    Environment.MoveAgent();
                    break;
                case VacuumAgent.VacuumAction.Clean:
                    _gridState[_agentPos.X, _agentPos.Y] = Environment.NONE;
                    // This imply that the agent execute this action only when he is on a dirty room
                    _nbOfDirtyRoom -= 1;
                    break;
                case VacuumAgent.VacuumAction.Grab:
                    _gridState[_agentPos.X, _agentPos.Y] -= Environment.JEWEL;
                    break;
                default:
                    break;
            }
        }
    }
}
