using System;
using System.Collections.Generic;
using System.Text;
using MagicWoodWPF.Facts;

namespace MagicWoodWPF
{
    public class WoodSquare
    {
        Vector2 _position;
        public Vector2 Position
        {
            get => _position;
        }

        bool _canExplore;
        public bool CanExplore {
            get => _canExplore;
        }

        bool _deadly;
        public bool Deadly {
            get => _deadly;
        }

        bool _hasRock;
        public bool HasRock {
            get => _hasRock;
        }

        bool _explored;
        public bool Explored {
            get => _explored;
        }

        List<DangerType> _hazardThatCouldBeThere;
        public bool ShouldBeSafe {
            get => _hazardThatCouldBeThere.Count == 0 || _hazardThatCouldBeThere.Contains(DangerType.Impossible);
        }
        public bool MayHaveAMonster {
            get => _hazardThatCouldBeThere.Contains(DangerType.Monster);
        }
        public bool MayBeARift {
            get => _hazardThatCouldBeThere.Contains(DangerType.Rift);
        }

        bool _isAnExit;
        public bool IsAnExit {
            get => _isAnExit;
        }

        List<ClueType> _clues;
        public bool NoClue {
            get => _clues.Count == 0;
        }
        public bool HasSmell {
            get => _clues.Contains(ClueType.Smell);
        }
        public bool HasWind
        {
            get => _clues.Contains(ClueType.Wind);
        }
        public bool IsBright
        {
            get => _clues.Contains(ClueType.Light);
        }


        float _monsterProb;
        public float MonsterProb { 
            get => _monsterProb;
            set => _monsterProb = value;
        }

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
            _clues = new List<ClueType>();

            _monsterProb = 0;
            _riftProb = 0;
        }

        public void Unblock(){
            if(!_explored) _canExplore = true;
        }

        public void MarkedAsExplored(bool deadly) {
            _explored = !deadly;
            _canExplore = deadly;
            _deadly = deadly;
        }

        public void ThrowRock() {
            _hasRock = true;
            if (_hazardThatCouldBeThere.Contains(DangerType.Monster)) _hazardThatCouldBeThere.Remove(DangerType.Monster);
        }

        public void AddElementToPossibility(DangerType hazard) {
            if (!_hazardThatCouldBeThere.Contains(hazard)) {
                _hazardThatCouldBeThere.Add(hazard);
            }
        }

        public void FoundPortalHere() {
            _isAnExit = true;
        }

        public void PerceiveNewClue(ClueType clue) {
            if (!_clues.Contains(clue)) {
                _clues.Add(clue);
            }
        }
    }
}
