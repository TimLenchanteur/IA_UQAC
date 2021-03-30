using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
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


        public override bool InConflictWith(WoodSquare otherFact)
        {
            throw new NotImplementedException();
        }


        public override void Apply(WoodSquare square)
        {
            throw new NotImplementedException();
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defini si le fait est egal a un autre objet 
        /// </summary>
        /// <param name="obj">L'autre objet propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            ClueIsOn otherClueIsOnFact = obj as ClueIsOn;
            return _clue == otherClueIsOnFact._clue;
        }
    }
}
