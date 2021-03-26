using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    [XmlType(TypeName = "Rocher")]
    public class RockThrown:Fact
    {
        protected RockThrown() : base()
        {
            _id = FactID.FACTID_ROCK;
        }

        /// <summary>
        /// Constructeur pendant le runtime
        /// </summary>
        /// <param name="position">Position a partir duquel le joueur peut partir</param>
        public RockThrown(Vector2 position)
        {
            _id = FactID.FACTID_ROCK;
            _isAbstract = false;
            _position = position;
        }


    }
}
