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
        List<Fact> _beliefs;
        // Ensemble de regles qui definisse les croyance de l'agent
        List<Rule> _rules;

        public WoodTravelerAgent(MainWindow appDisplayer, MagicWood environment) {
            _appDisplayer = appDisplayer;
            _environment = environment;
            _currentPosition = _environment.PlaceAgent();
            _beliefs = new List<Fact>();
            _rules = RulesGenerator.Instance.GeneratedRules;
        }

        #region Capteurs
        /// <summary>
        /// Applique les capteurs de l'agent et modifie ses croyances a partir de ceux-ci
        /// </summary>
        void CaptureSignals()
        {
            if(FeelWind())
            {
                _beliefs.Add(new ClueIsOn(_currentPosition, ClueType.Wind));
            }
            if (SmellSomething())
            {
                _beliefs.Add(new ClueIsOn(_currentPosition, ClueType.Smell));
            }
            if (SeeLight())
            {
                _beliefs.Add(new ClueIsOn(_currentPosition, ClueType.Light));
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
                bool exploredUp = false;
                bool exploredDown = false;
                bool exploredRight = false;
                bool exploredLeft = false;

                Vector2 up = new Vector2(_position.X, _position.Y + 1);
                Vector2 down = new Vector2(_position.X, _position.Y - 1);
                Vector2 right = new Vector2(_position.X + 1, _position.Y);
                Vector2 left = new Vector2(_position.X - 1, _position.Y);
                    
                foreach (Fact fact in agent._beliefs) {
                    if (fact.GetID() == FactID.FACTID_EXPLORE && fact.GetPosition().Equals(up)) exploredUp = true;
                    if (fact.GetID() == FactID.FACTID_EXPLORE && fact.GetPosition().Equals(down)) exploredDown = true;
                    if (fact.GetID() == FactID.FACTID_EXPLORE && fact.GetPosition().Equals(right)) exploredRight = true;
                    if (fact.GetID() == FactID.FACTID_EXPLORE && fact.GetPosition().Equals(left)) exploredLeft = true;
                }
                if (!exploredUp) agent._beliefs.Add(new ElementIsOn(up, ObjectType.None, 0.2f));
                if (!exploredDown) agent._beliefs.Add(new ElementIsOn(down, ObjectType.None, 0.2f));
                if (!exploredRight) agent._beliefs.Add(new ElementIsOn(right, ObjectType.None, 0.2f));
                if (!exploredLeft) agent._beliefs.Add(new ElementIsOn(left, ObjectType.None, 0.2f));
            }

            public override void Execute(WoodTravelerAgent agent)
            {
                bool died = agent._environment.MoveAgent(_position);
                Explored alreadyExplored = null;
                foreach(Fact fact in agent._beliefs)
                {
                    if (fact.GetID() == FactID.FACTID_EXPLORE && fact.GetPosition().Equals(_position)) {
                        alreadyExplored = (Explored)fact;
                        break;
                    }
                }

                Death deathCount = Death.Zero;
                if (alreadyExplored != null){
                    agent._beliefs.Remove(alreadyExplored);
                    // Si on l'a refait c'est forcement qu'il ya deja eu une mort
                    deathCount = Death.MoreThanOnce;
                }
                else {
                    if (died) deathCount = Death.Once;
                    AddExplorablePosition(agent);
                }
                Explored newFact = new Explored(_position, deathCount);
                agent._beliefs.Add(newFact);
            }
        }

        // Action de lancer un rocher sur une case
        class Throw : Effector {
            public Throw(Vector2 position) : base(position) { }

            public override void Execute(WoodTravelerAgent agent)
            {
                agent._environment.AgentThrowRock(_position);

                RockThrown newMarkedRock = new RockThrown(_position);
                agent._beliefs.Add(newMarkedRock);

                // On ne pense plus qu'il y a un monstre a cette endroit
                ElementIsOn canHaveMonster = null;
                foreach (Fact fact in agent._beliefs)
                {
                    if (fact.GetID() == FactID.FACTID_ELEMENTS && ((ElementIsOn)fact)._object == ObjectType.Monster)
                    {
                        canHaveMonster = (ElementIsOn)fact;
                        break;
                    }
                }
                if (canHaveMonster != null) agent._beliefs.Remove(canHaveMonster);
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
            _beliefs = InferenceEngine.InferenceCycle(_beliefs, _rules);

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
