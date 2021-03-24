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

        [XmlAttribute(AttributeName = "Probabilite")]
        public Probability _probability;

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
