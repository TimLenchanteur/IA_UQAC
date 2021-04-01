using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "DangerEstSur")]
    public class HazardIsOn:Fact
    {
        [XmlAttribute(AttributeName = "Type")]
        public DangerType _type;

        protected HazardIsOn() : base()
        {
            _id = FactID.FACTID_ELEMENTS;
        }


        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public HazardIsOn(Vector2 position, DangerType type)
        {
            _id = FactID.FACTID_ELEMENTS;
            _isAbstract = false;
            _position = position;
            _type = type;
        }


        /// <summary>
        /// Indique si ce fait est en conflit avec un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public override bool InConflictWith(WoodSquare otherFact)
        {
            if (otherFact.IsAnExit) return true;
            if ((otherFact.NoMonster && _type == DangerType.Monster) || (otherFact.NoRift && _type == DangerType.Rift)) return true;
            if (otherFact.HasRock && _type == DangerType.Monster) return true;
            if (otherFact.Explored && !otherFact.Deadly || !otherFact.CanExplore) return true;
            return false;
        }

        public override void Apply(WoodSquare square)
        {
            square.AddElementToPossibility(_type);
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            return (square.MayHaveAMonster && _type == DangerType.Monster || square.MayBeARift && _type == DangerType.Rift); 
        }

        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            HazardIsOn otherHazardIsOnFact = obj as HazardIsOn;
            return _type == otherHazardIsOnFact._type;
        }


    }
}
