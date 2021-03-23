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
        /// <summary>
        /// Signature d'un effecteur
        /// </summary>
        /// <param name="direction">Parametre passe a tout les effecteurs</param>
        delegate void Effector();

        /// <summary>
        /// Deplace l'agent vers le haut
        /// </summary>
        void MoveUp()
        {
            _currentPosition.Y -= 1;
        }

        /// <summary>
        /// Deplace l'agent vers le bas
        /// </summary>
        void MoveDown()
        {
            _currentPosition.Y += 1;
        }

        /// <summary>
        ///  Deplace l'agent vers la droite
        /// </summary>
        void MoveRight()
        {
            _currentPosition.X += 1;
        }

        /// <summary>
        ///  Deplace l'agent vers la gauche
        /// </summary>
        void MoveLeft()
        {
            _currentPosition.X -= 1;
        }

        /// <summary>
        /// Lance un rocher sur une case
        /// </summary>
        void ThrowRock()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Quitte la foret dans laquelle se trouve l'agent
        /// </summary>
        void Leave()
        {
            throw new NotImplementedException();
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
                nextAction();
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
