using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "ObjetEstSur")]
    public class ElementIsOn:Fact
    {
        [XmlAttribute(AttributeName = "Position")]
        public AbstractVector _abstractPos;

        [XmlAttribute(AttributeName = "ObjetIndefini", DataType = "boolean")]
        public bool _abstractObject = false;

        [XmlAttribute(AttributeName = "Objet")]
        public ObjectType _object;

        [XmlAttribute(AttributeName = "Probabilite")]
        public Probability _probability;

        protected Vector2 _position;

        protected ElementIsOn() : base()
        {
            _id = FactID.FACTID_ELEMENTS;
        }

        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public ElementIsOn(Vector2 position, ObjectType objectType, Probability probability)
        {
            _position = position;
            _object = objectType;
            _probability = probability;
            _abstractObject = false;
        }


        /// <summary>
        /// Defini si le fait est equivalent a un autre fait 
        /// </summary>
        /// <param name="otherFact">L'autre fait propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool IsEquals(Fact otherFact)
        {
            if (!base.IsEquals(otherFact)) return false;
            ElementIsOn otherElementIsOnFact = (ElementIsOn)otherFact;

            bool res = true;
            if (!_isAbstract && !otherElementIsOnFact._isAbstract) res &= _position.Equals(otherElementIsOnFact._position);
            if (!_abstractObject && !otherElementIsOnFact._abstractObject) res &= _object == otherElementIsOnFact._object;
            res &= _probability == otherElementIsOnFact._probability;

            return res;
        }
    }
}
