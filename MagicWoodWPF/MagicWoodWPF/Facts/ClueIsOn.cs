using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    /// <summary>
    /// Fait indiquant que la case contient un indice 
    /// </summary>
    [XmlType(TypeName = "IndiceEstSur")]
    public class ClueIsOn:Fact
    {
        [XmlAttribute(AttributeName = "Indice")]
        public ClueType _clue;

        protected ClueIsOn() : base()
        {
            _id = FactID.FACTID_CLUE;
        }

        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public ClueIsOn(Vector2 position, ClueType clueType)
        {
            _id = FactID.FACTID_CLUE;
            _isAbstract = false;
            _position = position;
            _clue = clueType;
        }

        // Ne sera probablement pas utilise car on ne peut pas inferer l'existence d'une preuve avec les regles actuel
        public override void Apply(WoodSquare square)
        {
            square.PerceiveNewClue(_clue);
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            switch (_clue) {
                case ClueType.Smell:
                    return square.HasSmell;
                case ClueType.Wind:
                    return square.HasWind;
                case ClueType.Light:
                    return square.IsBright;
                default:
                    break;
            }
            return false;
        }

        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            ClueIsOn otherClueIsOnFact = obj as ClueIsOn;
            return _clue == otherClueIsOnFact._clue;
        }
    }
}
