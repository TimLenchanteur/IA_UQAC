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
        public const int FACTID_EXPLORE = 3;
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

    public enum Death {
        [XmlEnum(Name = "0")]
        Zero,
        [XmlEnum(Name = "1")]
        Once,
        [XmlEnum(Name = "2+")]
        MoreThanOnce
    }

    public enum Probability
    {
        [XmlEnum(Name = "Aucune")]
        None,
        [XmlEnum(Name = "Impossible")]
        DefinitelyNot,
        [XmlEnum(Name = "Surement pas")]
        AlmostCertainlyNot,
        [XmlEnum(Name = "Probablement pas")]
        ProbablyNot,
        [XmlEnum(Name = "Peut etre pas")]
        MaybeNot,
        [XmlEnum(Name = "Inconnu")]
        Unknown,
        [XmlEnum(Name = "Peut etre")]
        Maybe,
        [XmlEnum(Name = "Probablenent")]
        Probably,
        [XmlEnum(Name = "Presque sure")]
        AlmostCertainly,
        [XmlEnum(Name = "Certain")]
        Definitely
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

        // Attribut definissant une probabilite pour un fait abstrait possedant un facteur d'incertitude
        // Par defaut la probabilite est mise a aucune ce qui signifie que le fait n'est pas lie a un facteur d'incertitude
        [XmlAttribute(AttributeName = "Probabilite")]
        public Probability _probability;

        // Defini si un fait concret est uncertain
        protected bool _isUncertain;
        // Facteur d'incertitude associe au fait
        protected float _certaintyFactor;

        protected Fact() {
            _id = FactID.FACTID_NONE;
            _isAbstract = true;
        }

        public Fact(float certaintyFactor) {
            _isAbstract = false;
            _isUncertain = true;
            _certaintyFactor = certaintyFactor;
        }

        public int GetID() {
            return _id;
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public float GetCertaintyFactor() {
            return _certaintyFactor;
        }

        public bool isAbstract() {
            return _isAbstract;
        }

        /// <summary>
        /// Indique si le fait est soumis a un facteur d'incertitude
        /// </summary>
        /// <returns>Vrai si le fait est lie a un facteur d'incertitude, faux sinon</returns>
        public bool isUncertain(){ 
            if(_isAbstract) return _probability != Probability.None;
            return _isUncertain;
        }

        /// <summary>
        /// Fusionne ce fait avec un autre fait concret equivalent a l'interieur de ce fait
        /// </summary>
        /// <param name="otherFact">L'autre fait propose</param>
        public virtual void Merge(Fact otherFact)
        {
            // Rien ne sert de combiner des fait qui ne sont pas concret ou soumis a un facteur de certitude
            if (_isAbstract || otherFact._isAbstract) return;
            if (!_isUncertain || otherFact._isUncertain) {
                _isUncertain = false;
                return;
            }

            if (_certaintyFactor > 0 && otherFact._certaintyFactor > 0) {
                _certaintyFactor = _certaintyFactor + otherFact._certaintyFactor * (1 - _certaintyFactor);
            }
            else if (_certaintyFactor < 0 && otherFact._certaintyFactor < 0) {
                _certaintyFactor = _certaintyFactor + otherFact._certaintyFactor * (1 + _certaintyFactor);
            }
            else if (_certaintyFactor < 0 || otherFact._certaintyFactor < 0) {
                _certaintyFactor = (_certaintyFactor + otherFact._certaintyFactor) / (1 - MathF.Min(MathF.Abs(_certaintyFactor), MathF.Abs(otherFact._certaintyFactor)));
            }
        }

        /// <summary>
        /// Indique si ce fait est en conflit avec un autre fait
        /// </summary>
        /// <param name="otherFact">L'autre fait</param>
        /// <returns></returns>
        public virtual bool InConflictWith(Fact otherFact) {
            return false;
        }


        /// <summary>
        /// Defini si le fait est equivalent a un autre fait
        /// </summary>
        /// <param name="fact">L'autre fait</param>
        /// <returns></returns>
        public virtual bool IsEquivalent(Fact otherFact)
        {
            bool res = true;
            if (!_isAbstract && !otherFact._isAbstract) res &= _position.Equals(otherFact._position);
            res &= _id == otherFact._id;
            return res;
        }

        /// <summary>
        /// Traduie la position abstraite de ce fait a partir d'une valeur X
        /// Cette fonction ne doit etre utilise que sur des fait abstrait
        /// </summary>
        /// <param name="xValue">La valeur concrete de X</param>
        /// <returns></returns>
        public Vector2 TranslatePosition(Vector2 xValue) {
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
        /// Met a jour le facteur de certitude 
        /// Ne doit etre utilise que sur des faits non abstrait et incertain
        /// </summary>
        public void UpdateCertaintyFactorBy(float factor) {
            _certaintyFactor *= factor;
        }

        /// <summary>
        /// Traduie la probabilite d'un fait abstrait en un facteur de certitude
        /// Ne doit etre utilise que sur des fait abstrait incertain
        /// </summary>
        /// <returns></returns>
        public float TranslateProbability() {
            float certaintyFactor = 1;
            switch (_probability) {
                case Probability.DefinitelyNot:
                    certaintyFactor = -1;
                    break;
                case Probability.AlmostCertainlyNot:
                    certaintyFactor = -0.8f;
                    break;
                case Probability.ProbablyNot:
                    certaintyFactor = -0.6f;
                    break;
                case Probability.MaybeNot:
                    certaintyFactor = -0.4f;
                    break;
                case Probability.Unknown:
                    certaintyFactor = 0.2f;
                    break;
                case Probability.Maybe:
                    certaintyFactor = 0.4f;
                    break;
                case Probability.Probably:
                    certaintyFactor = 0.6f;
                    break;
                case Probability.AlmostCertainly:
                    certaintyFactor = 0.8f;
                    break;
                default:
                    break;
            }
            return certaintyFactor;
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
