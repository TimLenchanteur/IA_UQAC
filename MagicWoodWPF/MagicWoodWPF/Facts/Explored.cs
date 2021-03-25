using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "Explorer")]
    public class Explored:Fact
    {
        [XmlAttribute(AttributeName = "Mort")]
        public Death _deathCount;

        protected Explored() : base()
        {
            _id = FactID.FACTID_EXPLORE;
        }

        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public Explored(Vector2 position, Death deathCount)
        {
            _id = FactID.FACTID_EXPLORE;
            _isAbstract = false;
            _position = position;
            _deathCount = deathCount;
        }

        /// <summary>
        /// Defini si le fait est equivalent a un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public override bool IsEquivalent(Fact otherFact)
        {
            if (!base.IsEquivalent(otherFact)) return false;
            Explored otherExploredFact = otherFact as Explored;
            return _deathCount == otherExploredFact._deathCount;
        }

        /// <summary>
        /// Defini si le fait est egal a un autre objet 
        /// </summary>
        /// <param name="obj">L'autre objet propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            Explored otherExploredFact = obj as Explored;
            return _deathCount == otherExploredFact._deathCount;
        }
    }
}
