using System;
using System.Collections.Generic;
using System.Text;
using MagicWoodWPF.Facts;

namespace MagicWoodWPF
{
    public class WoodSquare
    {
        // Position de la case
        Vector2 _position;
        public Vector2 Position
        {
            get => _position;
        }

        // Peut-on explorer cette case ?
        bool _canExplore;
        public bool CanExplore {
            get => _canExplore;
        }

        // Cette case a t'elle fait mourir le joueur ?
        bool _deadly;
        public bool Deadly {
            get => _deadly;
        }

        // L'agent a t'il jete un rocher sur cette case ?
        bool _hasRock;
        public bool HasRock {
            get => _hasRock;
        }

        // La case a t'elle ete exploree ? 
        bool _explored;
        public bool Explored {
            get => _explored;
        }

        
        List<DangerType> _hazardThatCouldBeThere;
        List<DangerType> _hazardImpossible;
        // Indique si la case ne devrait pas contenir de danger
        public bool ShouldBeSafe {
            get => _hazardThatCouldBeThere.Count == 0;
        }
        // Indique si la case contient peut etre un monstre
        public bool MayHaveAMonster {
            get => _hazardThatCouldBeThere.Contains(DangerType.Monster);
        }
        // Indique si la case contient peut etre une crevasee
        public bool MayBeARift {
            get => _hazardThatCouldBeThere.Contains(DangerType.Rift);
        }

        // Indique si la case ne peut pas contenir de monstre
        public bool NoMonster {
            get => _hazardImpossible.Contains(DangerType.Monster);
        }
        // Indique si la case ne peut pas contenir de crevasse
        public bool NoRift {
            get => _hazardImpossible.Contains(DangerType.Rift);
        }

        // Indique si la case est une sortie
        bool _isAnExit;
        public bool IsAnExit {
            get => _isAnExit;
        }

        List<ClueType> _clues;
        // Indique si la case ne contient aucun indices
        public bool NoClue {
            get => _clues.Count == 0;
        }
        // Indique si la case ne contient pas d'odeur
        public bool HasSmell {
            get => _clues.Contains(ClueType.Smell);
        }
        // Indique si la case ne contient pas de vent
        public bool HasWind
        {
            get => _clues.Contains(ClueType.Wind);
        }
        // Indique si la case contient de la lumiere
        public bool IsBright
        {
            get => _clues.Contains(ClueType.Light);
        }

        // Probabilite d'un monstre present sur cette case
        float _monsterProb;
        public float MonsterProb { 
            get => _monsterProb;
            set => _monsterProb = value;
        }

        // Probabilite d'une crevasse presente sur cette case
        float _riftProb;
        public float RiftProb {
            get => _riftProb;
            set => _riftProb = value;
        }

        public WoodSquare(Vector2 position) {
            _position = position;

            _canExplore = false;
            _hasRock = false;
            _explored = false;
            _isAnExit = false;

            _hazardThatCouldBeThere = new List<DangerType>();
            _hazardImpossible = new List<DangerType>();
            _clues = new List<ClueType>();

            _monsterProb = 0;
            _riftProb = 0;
        }

        /// <summary>
        /// Debloque une case adjacente a une case exploree par l'agent
        /// </summary>
        public void Unblock(){
            if(!_explored) _canExplore = true;
        }

        /// <summary>
        /// Indique qu'une case a ete exploree
        /// Si le joueur est mort sur la case c'elle si restera explorable et ne sera pas marquee comme exploree
        /// </summary>
        /// <param name="deadly">La case a t'elle tue le joueur ? </param>
        public void MarkedAsExplored(bool deadly) {
            _explored = !deadly;
            _canExplore = deadly;
            _deadly = deadly;
        }

        /// <summary>
        /// Lance un rocher sur cette case
        /// </summary>
        public void ThrowRock() {
            _hasRock = true;
            if (_hazardThatCouldBeThere.Contains(DangerType.Monster)) _hazardThatCouldBeThere.Remove(DangerType.Monster);
        }

        /// <summary>
        /// Ajoute un danger a ceux possible sur cette case
        /// </summary>
        /// <param name="hazard"> Le danger a ajouter</param>
        public void AddElementToPossibility(DangerType hazard) {
            if (!_hazardThatCouldBeThere.Contains(hazard) && !_hazardImpossible.Contains(hazard)) {
                _hazardThatCouldBeThere.Add(hazard);
            }
        }

        /// <summary>
        /// Enleve tout les danger possible sur cette case
        /// </summary>
        public void CleanPossibility() {
            _hazardThatCouldBeThere.Clear();
        }

        /// <summary>
        /// Indique qu'un portail a ete trouve sur cette case
        /// </summary>
        public void FoundPortalHere() {
            _isAnExit = true;
        }

        /// <summary>
        /// Indique qu'un indice a ete trouve sur cette case
        /// </summary>
        /// <param name="clue">L'indice en question</param>
        public void PerceiveNewClue(ClueType clue) {
            if (!_clues.Contains(clue)) {
                _clues.Add(clue);
            }
        }

        /// <summary>
        /// Enleve un indice de la case
        /// </summary>
        /// <param name="clue">L'indice en question</param>
        public void RemoveClue(ClueType clue) {
            _clues.Remove(clue);
        }

        /// <summary>
        /// Enleve une possibilite de danger sur la case
        /// </summary>
        /// <param name="hazard">Le danger que l'on souhaite enleve</param>
        public void RemoveHazard(DangerType hazard) {
            if (!_hazardImpossible.Contains(hazard)) {
                _hazardImpossible.Add(hazard);
                _hazardThatCouldBeThere.Remove(hazard);
            }
        }
    }
}
