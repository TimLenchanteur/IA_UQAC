using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "DangerNestPasSur")]
    public class HazardCantBeOn:Fact
    {
        [XmlAttribute(AttributeName = "Valeur", DataType = "boolean")]
        public bool _value;

        [XmlAttribute(AttributeName = "Type")]
        public DangerType _type;


        protected HazardCantBeOn() : base()
        {
            _id = FactID.FACTID_NOHAZARD;
        }


        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public HazardCantBeOn(Vector2 position, DangerType type, bool value)
        {
            _id = FactID.FACTID_NOHAZARD;
            _isAbstract = false;
            _position = position;
            _type = type;
            _value = value;
        }


        /// <summary>
        /// Indique si ce fait est en conflit avec un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public override bool InConflictWith(WoodSquare otherFact)
        {
            switch (_type) {
                case DangerType.Monster:
                    return _value != otherFact.NoMonster;
                case DangerType.Rift:
                    return _value != otherFact.NoRift;
                default:
                    break;
            }
            return false;
        }

        public override void Apply(WoodSquare square)
        {
            square.RemoveHazard(_type);
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            switch (_type)
            {
                case DangerType.Monster:
                    return _value == square.NoMonster;
                case DangerType.Rift:
                    return _value == square.NoRift;
                default:
                    break;
            }
            return false;
        }

        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            HazardCantBeOn otherNoHazardFact = obj as HazardCantBeOn;
            return _type == otherNoHazardFact._type && _value == otherNoHazardFact._value;
        }

    }
}
