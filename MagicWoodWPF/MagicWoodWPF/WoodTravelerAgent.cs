using System;
using System.Collections.Generic;
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

        public WoodTravelerAgent(MainWindow appDisplayer, MagicWood environment) {
            _appDisplayer = appDisplayer;
            _environment = environment;
            _currentPosition = _environment.PlaceAgent();
            _rules = RulesGenerator.Instance.GeneratedRules;

            _beliefs = new WoodSquare[environment.SqrtSize, environment.SqrtSize];
            for (int x = 0; x<environment.SqrtSize; x++) {
                for (int y = 0; y < environment.SqrtSize; y++) {
                    _beliefs[x, y] = new WoodSquare();
                    if (x == _currentPosition.X && y == _currentPosition.Y) {
                        _beliefs[x,y].MarkedAsExplored(false);
                    }
                    if (x == _currentPosition.X - 1 || x == _currentPosition.X + 1 || y == _currentPosition.Y + 1 || y == _currentPosition.Y - 1) {
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
            protected Vector2 _position;

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

                agent._beliefs[_position.X + 1, _position.Y].Unblock();
                agent._beliefs[_position.X - 1, _position.Y].Unblock();
                agent._beliefs[_position.X, _position.Y + 1].Unblock();
                agent._beliefs[_position.X, _position.Y - 1].Unblock();
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
        public void ExecuteMove() {
            // Observe l’environnement
            CaptureSignals();

            // Met A jour les croyance a partir des nouveaux fait observe et des croyances deja etablie
            InferenceEngine.InferenceCycle(ref _beliefs, _rules);

            // Choisit une action
            Effector nextAction = PlanNextMove();

            // Exécute l’action
            if (nextAction != null) {
                nextAction.Execute(this);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Planifie le prochain effecteur a execute 
        /// </summary>
        Effector PlanNextMove() {
            // Etablis une liste des actions possible a partir des croyances 
            List<Effector> _actionsDoable;

            // Defini la meilleur actions a partir du but et des performances et des proba

            // Return la meilleur actions
            throw new NotImplementedException();
        }


    
    }
}
