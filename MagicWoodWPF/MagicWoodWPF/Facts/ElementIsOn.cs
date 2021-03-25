using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "ObjetEstSur")]
    public class ElementIsOn:Fact
    {
        [XmlAttribute(AttributeName = "Objet")]
        public ObjectType _object;

        protected ElementIsOn() : base()
        {
            _id = FactID.FACTID_ELEMENTS;
        }


        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public ElementIsOn(Vector2 position, ObjectType objectType)
        {
            _id = FactID.FACTID_ELEMENTS;
            _isAbstract = false;
            _position = position;
            _object = objectType;
        }

        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public ElementIsOn(Vector2 position, ObjectType objectType, float certaintyFactor) : base(certaintyFactor)
        {
            _id = FactID.FACTID_ELEMENTS;
            _position = position;
            _object = objectType;
        }


        /// <summary>
        /// Indique si ce fait est en conflit avec un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public override bool InConflictWith(Fact otherFact)
        {
            if (otherFact.GetID() != FactID.FACTID_ELEMENTS) return false;
            ElementIsOn otherElementIsOn = otherFact as ElementIsOn;

            return 
                (((otherElementIsOn._object == ObjectType.Portail && (_object == ObjectType.Monster || _object == ObjectType.Rift)) || (_object == ObjectType.None && otherElementIsOn._object != ObjectType.None))
                && ((_certaintyFactor == 1 && _isUncertain)||!_isUncertain))
                ||
                (((_object == ObjectType.Portail && (otherElementIsOn._object == ObjectType.Monster || otherElementIsOn._object == ObjectType.Rift)) || (otherElementIsOn._object == ObjectType.None && _object != ObjectType.None))
                && ((otherElementIsOn._certaintyFactor == 1 && otherElementIsOn._isUncertain) || !otherElementIsOn._isUncertain));
        }

        /// <summary>
        /// Defini si le fait est equivalent a un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public override bool IsEquivalent(Fact otherFact)
        {
            if (!base.IsEquivalent(otherFact)) return false;
            ElementIsOn otherElementIsOnFact = otherFact as ElementIsOn;
            return _object == otherElementIsOnFact._object;
        }

        /// <summary>
        /// Defini si le fait est egal a un autre objet 
        /// </summary>
        /// <param name="obj">L'autre objet propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            ElementIsOn otherElementIsOnFact = obj as ElementIsOn;
            return _object == otherElementIsOnFact._object;
        }


    }
}
