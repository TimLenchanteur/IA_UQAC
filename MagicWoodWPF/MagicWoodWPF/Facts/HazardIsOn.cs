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
            throw new NotImplementedException();
            /*if (otherFact.GetID() != FactID.FACTID_ELEMENTS) return false;
            ElementIsOn otherElementIsOn = otherFact as ElementIsOn;

            return 
                (((otherElementIsOn._object == ObjectType.Portail && (_object == ObjectType.Monster || _object == ObjectType.Rift)) || (_object == ObjectType.None && otherElementIsOn._object != ObjectType.None))
                && ((_certaintyFactor == 1 && _isUncertain)||!_isUncertain))
                ||
                (((_object == ObjectType.Portail && (otherElementIsOn._object == ObjectType.Monster || otherElementIsOn._object == ObjectType.Rift)) || (otherElementIsOn._object == ObjectType.None && _object != ObjectType.None))
                && ((otherElementIsOn._certaintyFactor == 1 && otherElementIsOn._isUncertain) || !otherElementIsOn._isUncertain));*/
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
            HazardIsOn otherHazardIsOnFact = obj as HazardIsOn;
            return _type == otherHazardIsOnFact._type;
        }


    }
}
