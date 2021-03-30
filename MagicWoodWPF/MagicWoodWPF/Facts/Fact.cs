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
        public const int FACTID_CANEXPLORE = 3;
        public const int FACTID_ROCK = 4;
        public const int FACTID_EXIT= 5;
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

    public enum DangerType
    {
        [XmlEnum(Name = "Impossible")]
        Impossible,
        [XmlEnum(Name = "Monstre")]
        Monster,
        [XmlEnum(Name = "Crevasse")]
        Rift
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

        protected Fact() {
            _id = FactID.FACTID_NONE;
            _isAbstract = true;
        }

        public int GetID() {
            return _id;
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public bool isAbstract() {
            return _isAbstract;
        }

        /// <summary>
        /// Traduie la position abstraite de ce fait a partir d'une valeur X
        /// Cette fonction ne doit etre utilise que sur des fait abstrait
        /// </summary>
        /// <param name="xValue">La valeur concrete de X</param>
        /// <returns></returns>
        public Vector2 TranslatePosition(Vector2 xValue)
        {
            Vector2 translation = new Vector2(xValue.X, xValue.Y);
            switch (_abstractPos)
            {
                case AbstractVector.Up:
                    translation.Y += 1;
                    break;
                case AbstractVector.Down:
                    translation.Y -= 1;
                    break;
                case AbstractVector.Right:
                    translation.X += 1;
                    break;
                case AbstractVector.Left:
                    translation.X -= 1;
                    break;
                default:
                    break;
            }

            return translation;
        }


        /// <summary>
        /// Indique si ce fait est en conflit avec un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public virtual bool InConflictWith(WoodSquare otherFact) {
            throw new NotImplementedException();
        }


        public virtual void Apply(WoodSquare square) {
            throw new NotImplementedException();
        }

        public virtual bool IsContainedIn(WoodSquare square) {
            throw new NotImplementedException();
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
            else res &= _isAbstract && otherFact._isAbstract;
            res &= _id == otherFact._id;
            return res;
        }
    }
}
