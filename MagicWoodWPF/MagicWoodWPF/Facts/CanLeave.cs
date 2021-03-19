﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "PeutPartir")]
    public class CanLeave : Fact
    {
        [XmlAttribute(AttributeName = "Position")]
        public AbstractVector _abstractPos;

        protected Vector2 _position;

        protected CanLeave() :base()
        {
            _id = FactID.FACTID_LEAVE; 
        }

        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public CanLeave(Vector2 position) {
            _position = position;
        }


        /// <summary>
        /// Defini si le fait est equivalent a un autre fait 
        /// </summary>
        /// <param name="otherFact">L'autre fait propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool IsEquals(Fact otherFact) 
        {
            if (!base.IsEquals(otherFact)) return false;
            CanLeave otherCanLeaveFact = (CanLeave)otherFact;

            if (_isAbstract || otherCanLeaveFact._isAbstract) return true;

            return _position.Equals(otherCanLeaveFact._position);
        }
    }
}
