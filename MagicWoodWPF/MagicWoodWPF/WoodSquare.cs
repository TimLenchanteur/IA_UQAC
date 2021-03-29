﻿using System;
using System.Collections.Generic;
using System.Text;
using MagicWoodWPF.Facts;

namespace MagicWoodWPF
{
    class WoodSquare
    {
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

        List<ObjectType> _elementThatCouldBeThere;
        public bool ShouldBeSafe {
            get => _elementThatCouldBeThere.Count == 0;
        }
        public bool MayHaveAMonster {
            get => _elementThatCouldBeThere.Contains(ObjectType.Monster);
        }
        public bool MayBeARift {
            get => _elementThatCouldBeThere.Contains(ObjectType.Rift);
        }
        public bool IsAnExit {
            get => _elementThatCouldBeThere.Contains(ObjectType.Portal);
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

        public WoodSquare() {
            _canExplore = false;
            _hasRock = false;
            _explored = false;

            _elementThatCouldBeThere = new List<ObjectType>();
            _clues = new List<ClueType>();
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
            if (_elementThatCouldBeThere.Contains(ObjectType.Monster)) _elementThatCouldBeThere.Remove(ObjectType.Monster);
        }

        public void AddElementToPossibility(ObjectType element) {
            if (!_elementThatCouldBeThere.Contains(element)) {
                _elementThatCouldBeThere.Add(element);
            }
        }


        public void PerceiveNewClue(ClueType clue) {
            if (!_clues.Contains(clue)) {
                _clues.Add(clue);
            }
        }
    }
}