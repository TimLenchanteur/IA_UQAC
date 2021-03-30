using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MagicWoodWPF.Facts;

namespace MagicWoodWPF
{
    /// <summary>
    /// Agent base sur les buts
    /// Son but est de sortir de la foret sans tomber dans une crevasse et en recontrant le moins d'obstacle possible
    /// </summary>
    class WoodTravelerAgent
    {
        // Environement dans lequel evolue l'agent
        MagicWood _environment;

        // Position Actuel dans l'environment;
        Vector2 _currentPosition;

        // Classe permetant d'afficher l'environement dans l'application
        MainWindow _appDisplayer;

        // Croyances actuel de l'agent
        WoodSquare[,] _beliefs;
        // Ensemble de regles qui definisse les croyance de l'agent
        List<Rule> _rules;

        private Queue<Effector> _queuedActions = new Queue<Effector>();

        public WoodTravelerAgent(MainWindow appDisplayer, MagicWood environment) {
            _appDisplayer = appDisplayer;
            _environment = environment;
            _currentPosition = _environment.PlaceAgent();
            _rules = RulesGenerator.Instance.GeneratedRules;

            _beliefs = new WoodSquare[environment.SqrtSize, environment.SqrtSize];
            for (int x = 0; x<environment.SqrtSize; x++) {
                for (int y = 0; y < environment.SqrtSize; y++) {
                    _beliefs[x, y] = new WoodSquare(new Vector2(x,y));
                    if (x == _currentPosition.X && y == _currentPosition.Y) {
                        _beliefs[x,y].MarkedAsExplored(false);
                    }
                    if (((x == _currentPosition.X - 1 || x == _currentPosition.X + 1) && y== _currentPosition.Y)
                        || ((y == _currentPosition.Y + 1 || y == _currentPosition.Y - 1) && x==_currentPosition.X)) {
                        _beliefs[x, y].Unblock();
                    }
                }
            }
        }

        #region Capteurs
        /// <summary>
        /// Applique les capteurs de l'agent et modifie ses croyances a partir de ceux-ci
        /// </summary>
        void CaptureSignals()
        {
            if(FeelWind())
            {
                _beliefs[_currentPosition.X, _currentPosition.Y].PerceiveNewClue(ClueType.Wind); 
            }
            if (SmellSomething())
            {
                _beliefs[_currentPosition.X, _currentPosition.Y].PerceiveNewClue(ClueType.Smell);
            }
            if (SeeLight())
            {
                _beliefs[_currentPosition.X, _currentPosition.Y].PerceiveNewClue(ClueType.Light);
            }
        }

        /// <summary>
        /// Capteur permettant de reperer le vent
        /// </summary>
        /// <returns>vrai si l'agent a reperer du vent sur la case ou il se trouve, faux sinon</returns>
        bool FeelWind()
        {
            return (_environment.Grid[_currentPosition.X, _currentPosition.Y] & MagicWood.WIND) == MagicWood.WIND;
        }

        /// <summary>
        /// Capteur permettant de sentir une odeur
        /// </summary>
        /// <returns>vrai si l'agent sent une odeur sur la case ou il se trouve, faux sinon</returns>
        bool SmellSomething()
        {
            return (_environment.Grid[_currentPosition.X, _currentPosition.Y] & MagicWood.SMELL) == MagicWood.SMELL;
        }

        /// <summary>
        /// Capteur permettant de reperer de la lumiere
        /// </summary>
        /// <returns>vrai si l'agent voit de la lumiere sur la case ou il se trouve, faux sinon</returns>
        bool SeeLight()
        {
            return (_environment.Grid[_currentPosition.X, _currentPosition.Y] & MagicWood.PORTAL) == MagicWood.PORTAL;
        }
        #endregion

        #region Effecteurs
        // Signature d'un effecteur
        abstract class Effector {

            // Position a laquelle on veut effectuer l'action
            public Vector2 _position;

            /// <summary>
            /// Position a laquelle on veut effectuer l'action
            /// </summary>
            /// <param name="position"></param>
            public Effector(Vector2 position){
                _position = position;
            }

            /// <summary>
            /// Execute l'action
            /// </summary>
            public abstract void Execute(WoodTravelerAgent agent);
        }

        // Action de bouger sur une case
        class Move : Effector {

            public Move(Vector2 position) :base(position) { }

            /// <summary>
            /// Ajoute les nouvelle position que le jour peut explorer a partir de celle-ci
            /// Ne doit etre appele que lors de la premiere exploration
            /// </summary>
            /// <param name="agent"></param>
            protected void AddExplorablePosition(WoodTravelerAgent agent) {
                int sqrSize = (int)Math.Sqrt(agent._beliefs.Length);
                if (_position.X + 1 < sqrSize) agent._beliefs[_position.X + 1, _position.Y].Unblock();
                if (_position.X - 1 >= 0) agent._beliefs[_position.X - 1, _position.Y].Unblock();
                if (_position.Y + 1 < sqrSize)  agent._beliefs[_position.X, _position.Y + 1].Unblock();
                if (_position.Y - 1 >= 0) agent._beliefs[_position.X, _position.Y - 1].Unblock();
            }

            public override void Execute(WoodTravelerAgent agent)
            {
                bool died = !agent._environment.MoveAgent(_position);

                if (!died) {
                    agent._currentPosition = _position;
                    AddExplorablePosition(agent);
                }

                agent._beliefs[_position.X, _position.Y].MarkedAsExplored(died);
            }
        }

        // Action de lancer un rocher sur une case
        class Throw : Effector {
            public Throw(Vector2 position) : base(position) { }

            public override void Execute(WoodTravelerAgent agent)
            {
                agent._environment.AgentThrowRock(_position);

                agent._beliefs[_position.X, _position.Y].ThrowRock();
            }
        }


        // Action de quitter la foret dans laquelle se trouve l'agent
        class Leave:Effector
        {
            public Leave(Vector2 position) : base(position) { }

            public override void Execute(WoodTravelerAgent agent)
            {
                agent._environment.AgentLeave(_position);
            }
        }
        #endregion

        /// <summary>
        /// Execute l'effecteur le plus approprie selon l'agent
        /// </summary>
        public void ExecuteMoves() {
            // Observe l’environnement
            CaptureSignals();

            // Met A jour les croyance a partir des nouveaux fait observe et des croyances deja etablie
            InferenceEngine.InferenceCycle(ref _beliefs, _rules);

            if(_queuedActions.Count == 0)
            {
                PlanNextMove();
            }
            Effector action = _queuedActions.Dequeue();
            Debug.WriteLine("Action effectuée : " + action.GetType() + " Position : "+action._position.X+" "+action._position.Y);
            action.Execute(this);
        }

        /// <summary>
        /// Planifie le prochain effecteur a execute 
        /// </summary>
        void PlanNextMove() {
            if (_beliefs[_currentPosition.X, _currentPosition.Y].IsAnExit)
            {
                _queuedActions.Enqueue(new Leave(_currentPosition));
                return;
            }

            // Etablit une liste des actions possibles a partir des croyances 
            List<WoodSquare> explorableTiles = new List<WoodSquare>();
            List<WoodSquare> exploredTiles = new List<WoodSquare>();

            for(int i = 0; i<_environment.SqrtSize; i++)
            {
                for (int j = 0; j < _environment.SqrtSize; j++)
                {
                    if (_beliefs[i, j].Explored)
                    {
                        exploredTiles.Add(_beliefs[i, j]);
                    }
                    else if (_beliefs[i, j].CanExplore)
                    {
                        explorableTiles.Add(_beliefs[i, j]);
                    }
                }
            }

            //will be used if no perfectly safe tile can be found
            List<WoodSquare> canOnlyHaveMonster = new List<WoodSquare>();

            //find safe explorable tiles
            foreach(WoodSquare tile in explorableTiles)
            {
                if(tile.ShouldBeSafe)
                {
                    _queuedActions.Enqueue(new Move(tile.Position));
                    return;
                }
                else if(tile.MayHaveAMonster && !tile.MayBeARift)
                {
                    canOnlyHaveMonster.Add(tile);
                }
            }

            //no safe tiles, throw rock on one that can only have a monster
            if (canOnlyHaveMonster.Count != 0)
            {
                _queuedActions.Enqueue(new Throw(canOnlyHaveMonster[0].Position));
                _queuedActions.Enqueue(new Move(canOnlyHaveMonster[0].Position));
                return;
            }

            List<WoodSquare> windyTiles = new List<WoodSquare>();
            foreach(WoodSquare tile in exploredTiles)
            {
                if (tile.HasWind)
                {
                    windyTiles.Add(tile);
                }
            }

            
            Debug.WriteLine("probability computation");
            List<List<WoodSquare>> coherentCombinations = GetAllCoherentCombinations(explorableTiles, windyTiles);

            float minRiftProb = 1;
            WoodSquare toExplore = new WoodSquare(new Vector2(-1,-1));
            foreach(WoodSquare tile in explorableTiles)
            {
                float riftProb = RiftProbability(tile, coherentCombinations, explorableTiles.Count);
                if(riftProb < minRiftProb && !tile.Deadly)
                {
                    minRiftProb = riftProb;
                    toExplore = tile;
                }
            }

            if (toExplore.MayHaveAMonster)
            {
                _queuedActions.Enqueue(new Throw(toExplore.Position));
            }
            _queuedActions.Enqueue(new Move(toExplore.Position));
            return;
        }

        private List<WoodSquare> GetNeighbours(WoodSquare tile)
        {
            List<WoodSquare> neighbours = new List<WoodSquare>();
            Vector2 pos = tile.Position;
            if (pos.X > 0)
            {
                neighbours.Add(_beliefs[pos.X - 1, pos.Y]);
            }
            if (pos.X < _environment.SqrtSize - 1)
            {
                neighbours.Add(_beliefs[pos.X + 1, pos.Y]);
            }
            if (pos.Y > 0)
            {
                neighbours.Add(_beliefs[pos.X, pos.Y - 1]);
            }
            if (pos.Y < _environment.SqrtSize - 1)
            {
                neighbours.Add(_beliefs[pos.X, pos.Y + 1]);
            }
            return neighbours;
        }

        private float RiftProbability(WoodSquare toCompute, List<List<WoodSquare>> coherentCombinations, int explorableTilesCount)
        {
            float sumIsRift = 0;
            float sumIsNotRift = 0;
            foreach(List<WoodSquare> combination in coherentCombinations)
            {
                if (combination.Contains(toCompute))
                {
                    sumIsRift += (float)(Math.Pow(0.1, combination.Count) * Math.Pow(0.9, explorableTilesCount - combination.Count));
                }
                else
                {
                    sumIsNotRift += (float)(Math.Pow(0.1, combination.Count) * Math.Pow(0.9, explorableTilesCount - combination.Count));
                }
            }

            sumIsRift *= 0.1f;
            sumIsNotRift *= 0.9f;

            return sumIsRift / (sumIsRift + sumIsNotRift);
        }

        private List<List<WoodSquare>> GetAllCoherentCombinations(List<WoodSquare> explorableTiles, List<WoodSquare> windyTiles)
        {
            List<List<WoodSquare>> combinations = new List<List<WoodSquare>>();
            for (int i = 1; i < (int)Math.Pow(2, explorableTiles.Count); i++)
            {
                List<WoodSquare> combination = new List<WoodSquare>();
                for (uint j = 0; j < explorableTiles.Count; j++)
                {
                    if ((i & (1u << (int)j)) > 0)
                    {
                        combination.Add(explorableTiles[(int)j]);
                    }
                }
                if (IsWindCoherent(windyTiles, combination))
                {
                    combinations.Add(combination);
                }
            }
            return combinations;
        }


        private bool IsWindCoherent(List<WoodSquare> windyTiles, List<WoodSquare> supposedRifts)
        {
            List<WoodSquare> supposedWindyTiles = new List<WoodSquare>();
            foreach(WoodSquare supposedRift in supposedRifts)
            {
                foreach(WoodSquare neighbour in GetNeighbours(supposedRift))
                {
                    if(neighbour.Explored && !supposedWindyTiles.Contains(neighbour))
                    {
                        supposedWindyTiles.Add(neighbour);
                        if (supposedWindyTiles.Count > windyTiles.Count)
                        {
                            return false;
                        }
                    }
                }
            }
            if(supposedWindyTiles.Count != windyTiles.Count)
            {
                return false;
            }
            else
            {
                foreach(WoodSquare tile in windyTiles)
                {
                    if (!supposedWindyTiles.Contains(tile))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    
    }
}
