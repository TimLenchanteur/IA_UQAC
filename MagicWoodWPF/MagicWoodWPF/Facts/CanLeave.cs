using System;
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
    }
}
