using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "Rocher")]
    public class RockThrown:Fact
    {
        [XmlAttribute(AttributeName = "Present", DataType = "boolean")]
        public bool _atThisLocation = true;

        protected RockThrown() : base()
        {
            _id = FactID.FACTID_ROCK;
        }

        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public RockThrown(Vector2 position, bool wasThrown)
        {
            _id = FactID.FACTID_ROCK;
            _atThisLocation = wasThrown;
            _isAbstract = false;
            _position = position;
        }

        public override bool InConflictWith(WoodSquare otherFact)
        {
            return otherFact.HasRock != _atThisLocation;
        }

        // Ne sera probablement pas utilise car on ne peut pas inferer son existence avec les regles actuel
        public override void Apply(WoodSquare square)
        {
            if (_atThisLocation) square.ThrowRock();
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            return square.HasRock == _atThisLocation;
        }

        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            RockThrown otherRockFact = obj as RockThrown;
            return _atThisLocation == otherRockFact._atThisLocation;
        }

    }
}
