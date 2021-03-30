using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{

    [XmlType(TypeName = "EstUneSortie")]
    public class IsAnExit:Fact
    {
        [XmlAttribute(AttributeName = "Actif", DataType = "boolean")]
        public bool _activated = true;


        protected IsAnExit() : base()
        {
            _id = FactID.FACTID_CANEXPLORE;
        }

        public IsAnExit(Vector2 position, bool activated)
        {
            _id = FactID.FACTID_CANEXPLORE;
            _activated = activated;
            _isAbstract = false;
            _position = position;
        }


        public override bool InConflictWith(WoodSquare otherFact)
        {
            throw new NotImplementedException();
        }


        public override void Apply(WoodSquare square)
        {
            throw new NotImplementedException();
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Object obj)
        {
            throw new NotImplementedException();
        }
    }
}
