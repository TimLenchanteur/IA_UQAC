﻿using System;
using System.Collections.Generic;
using System.Text;

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

        // Plan d'action ou intention de l'effecteur 
        // (Peut etre que la stack n'est pas adapte ici vu qu'il va falloir ajouter des elements entre certaines actions) 
        Stack<Effector> _actionPlan;

        WoodTravelerAgent(MainWindow appDisplayer, MagicWood environment) {
            _appDisplayer = appDisplayer;
            _environment = environment;
            _currentPosition = _environment.PlaceAgent();

            // Initialise le plan d'action
            // Le but etant de quitter la foret la derniere action du plan sera donc l'effecteur qui permet de quitter la foret
            _actionPlan = new Stack<Effector>();
            Effector leave = new Effector(Leave);
            _actionPlan.Push(leave);
        }

        #region Capteurs
        /// <summary>
        /// Capteur permettant de reperer le vent
        /// </summary>
        /// <returns>vrai si l'agent a reperer du vent sur la case ou il se trouve, faux sinon</returns>
        bool FeelWind() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Capteur permettant de sentir une odeur
        /// </summary>
        /// <returns>vrai si l'agent sent une odeur sur la case ou il se trouve, faux sinon</returns>
        bool SmellSomething() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Capteur permettant de reperer de la lumiere
        /// </summary>
        /// <returns>vrai si l'agent voit de la lumiere sur la case ou il se trouve, faux sinon</returns>
        bool SeeLight() {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deplace l'agent vers le bas
        /// </summary>
        void MoveDown()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Deplace l'agent vers la droite
        /// </summary>
        void MoveRight()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Deplace l'agent vers la gauche
        /// </summary>
        void MoveLeft()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Planifie le prochain effecteur a execute 
        /// </summary>
        void PlanNextMove() {
            throw new NotImplementedException();
        }
    
    }
}
