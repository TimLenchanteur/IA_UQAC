using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    /// <summary>
    /// Fait indiquant si la case ne peut pas contenir un indice
    /// </summary>
    [XmlType(TypeName = "IndiceNePeutPasEtreSur")]
    public class NoClue:Fact
    {
        [XmlAttribute(AttributeName = "Actif", DataType = "boolean")]
        public bool _activated;

        [XmlAttribute(AttributeName = "Indice")]
        public ClueType _clue;

        NoClue() : base() {
            _id = FactID.FACTID_NOCLUE;
        }

        public NoClue(Vector2 position, ClueType clueType, bool activated)
        {
            _id = FactID.FACTID_NOCLUE;
            _isAbstract = false;
            _position = position;
            _clue = clueType;
            _activated = activated;
        }

        public override bool InConflictWith(WoodSquare otherFact)
        {
            bool containsClue = false;
            switch (_clue)
            {
                case ClueType.Smell:
                    containsClue = otherFact.HasSmell;
                    break;
                case ClueType.Wind:
                    containsClue = otherFact.HasWind;
                    break;
                case ClueType.Light:
                    containsClue = otherFact.IsBright;
                    break;
                default:
                    break;
            }

            return (_activated && (!otherFact.Explored || (otherFact.Explored && containsClue))) ||
                    (!_activated && otherFact.Explored && !containsClue);
           
        }

        // Ne sera probablement pas utilise car on ne peut pas inferer l'existence d'une preuve avec les regles actuel
        public override void Apply(WoodSquare square)
        {
            if (_activated) square.RemoveClue(_clue);
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            bool containsClue = false;
            switch (_clue)
            {
                case ClueType.Smell:
                    containsClue = square.HasSmell;
                    break;
                case ClueType.Wind:
                    containsClue = square.HasWind;
                    break;
                case ClueType.Light:
                    containsClue = square.IsBright;
                    break;
                default:
                    break;
            }

            return (_activated && square.Explored && !containsClue) ||
               (!_activated && (!square.Explored || (square.Explored && containsClue)));
        }

        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            NoClue otherNoClueFact = obj as NoClue;
            return _clue == otherNoClueFact._clue && _activated == otherNoClueFact._activated;
        }
    }
}
