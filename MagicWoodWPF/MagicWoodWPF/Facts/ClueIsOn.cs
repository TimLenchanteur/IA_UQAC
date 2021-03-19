using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "IndiceEstSur")]
    public class ClueIsOn:Fact
    {
        [XmlAttribute(AttributeName = "Position")]
        public AbstractVector _abstractPos;

        [XmlAttribute(AttributeName = "Indice")]
        public ClueType _clue;

        protected Vector2 _position;

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
            _position = position;
            //_clue= clueType;
        }

        /// <summary>
        /// Defini si le fait est equivalent a un autre fait 
        /// </summary>
        /// <param name="otherFact">L'autre fait propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool IsEquals(Fact otherFact)
        {
            if (!base.IsEquals(otherFact)) return false;
            ClueIsOn otherClueIsOnFact = (ClueIsOn)otherFact;

            bool res = true;
            if (!_isAbstract && !otherClueIsOnFact._isAbstract) res &= _position.Equals(otherClueIsOnFact._position);
           // res &= _clue == otherClueIsOnFact._clue;

            return res;
        }
    }
}
