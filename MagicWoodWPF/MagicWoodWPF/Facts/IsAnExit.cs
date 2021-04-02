using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{

    /// <summary>
    /// Fait indiquant si la case est une sortie
    /// </summary>
    [XmlType(TypeName = "EstUneSortie")]
    public class IsAnExit:Fact
    {
        [XmlAttribute(AttributeName = "Actif", DataType = "boolean")]
        public bool _activated = true;

        protected IsAnExit() : base()
        {
            _id = FactID.FACTID_EXIT;
        }

        public IsAnExit(Vector2 position, bool activated)
        {
            _id = FactID.FACTID_EXIT;
            _activated = activated;
            _isAbstract = false;
            _position = position;
        }

        public override bool InConflictWith(WoodSquare otherFact)
        {
            return otherFact.IsAnExit != _activated;
        }

        public override void Apply(WoodSquare square)
        {
           if(_activated) square.FoundPortalHere();
        }

        public override bool IsContainedIn(WoodSquare square)
        {
           return square.IsAnExit == _activated;
        }

        public override bool Equals(Object obj)
        {
            if(!base.Equals(obj))return false;
            IsAnExit otherExitFact = obj as IsAnExit;
            return otherExitFact._activated == _activated;
        }
    }
}
