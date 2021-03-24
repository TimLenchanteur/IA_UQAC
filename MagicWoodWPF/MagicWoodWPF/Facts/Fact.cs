using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    public struct FactID {
        public const int FACTID_NONE = 0;
        public const int FACTID_ELEMENTS = 1;
        public const int FACTID_CLUE = 2;
    }

    public enum AbstractVector
    {
        [XmlEnum(Name = "X")]
        originalPos,
        [XmlEnum(Name = "XDessus")]
        Up,
        [XmlEnum(Name = "XSous")]
        Down,
        [XmlEnum(Name = "XDroite")]
        Right,
        [XmlEnum(Name = "XGauche")]
        Left
    }

    public enum ObjectType
    {
        [XmlEnum(Name = "Aucun")]
        None,
        [XmlEnum(Name = "Monstre")]
        Monster,
        [XmlEnum(Name = "Crevasse")]
        Rift,
        [XmlEnum(Name = "Portail")]
        Portail
    }

    public enum ClueType
    {
        [XmlEnum(Name = "Vent")]
        Wind,
        [XmlEnum(Name = "Odeur")]
        Smell,
        [XmlEnum(Name = "Lumiere")]
        Light
    }

    public enum Probability
    {
        [XmlEnum(Name = "Possible")]
        Possible,
        [XmlEnum(Name = "Probable")]
        Likely,
        [XmlEnum(Name = "Tres Probable")]
        VeryLikely,
        [XmlEnum(Name = "Presque Sure")]
        AlmostCertain
    }

    [XmlType(TypeName = "Fait")]
    public class Fact
    {
        // identifiant permettant de connaitre le type de la variable pour upcast
        [XmlIgnore]
        protected int _id;

        [XmlAttribute(AttributeName = "Position")]
        public AbstractVector _abstractPos;

        protected Vector2 _position;

        // Defini si le fait est abstrait ou non
        // Les faits abstraits ne devrait etre que ceux serialiser, les non abstrait sont ceux creer pendant le runtime
        // Cette variable permet d'utiliser une notation abstraite comme X pour definir une variable qui ne sera connu que pendant le runtime
        [XmlIgnore]
        protected bool _isAbstract;

        [XmlIgnore]
        protected bool _isUncertain = false;
        protected float _certaintyFactor;

        protected Fact() {
            _id = FactID.FACTID_NONE;
            _isAbstract = true;
        }

        public int GetID() {
            return _id;
        }

        public Vector2 GetPosition() {
            return _position;
        }

        /// <summary>
        /// Fusionne deux fait equivalent
        /// </summary>
        /// <param name="otherFact">L'autre fait propose</param>
        public virtual void Merge(Fact otherFact)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indique si ce fait est en conflit avec un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public virtual bool InConflictWith(Fact otherFact) {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Defini si le fait est equivalent a un autre fait
        /// </summary>
        /// <param name="fact">L'autre fait</param>
        /// <returns></returns>
        public virtual bool IsEquivalent(Fact fact)
        {

            /*if (otherFact == null)
            {
                return false;
            }

            bool res = true;
            if (!_isAbstract && !otherFact._isAbstract) res &= _position.Equals(otherFact._position);
            res &= _id == otherFact._id;
            return res;*/
            throw new NotFiniteNumberException();
        }

        /// <summary>
        /// Defini si le fait est egal a un autre objet 
        /// </summary>
        /// <param name="obj">L'autre objet propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool Equals(Object obj)
        {
            Fact otherFact = obj as Fact;
            if (otherFact == null)
            {
                return false;
            }

            bool res = true;
            if (!_isAbstract && !otherFact._isAbstract) res &= _position.Equals(otherFact._position);
            res &= _id == otherFact._id;
            return res;
            throw new NotFiniteNumberException();
        }
    }
}
