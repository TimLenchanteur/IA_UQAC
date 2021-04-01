using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace MagicWoodWPF.Facts
{

    [XmlType(TypeName = "Explorable")]
    public class CanExplore:Fact
    {
        [XmlAttribute(AttributeName = "Actif", DataType = "boolean")]
        public bool _activated = true;

        protected CanExplore() : base()
        {
            _id = FactID.FACTID_CANEXPLORE;
        }

        public CanExplore(Vector2 position, bool activate) {
            _id = FactID.FACTID_CANEXPLORE;
            _activated = activate;
            _isAbstract = false;
            _position = position;
        }

        public override bool InConflictWith(WoodSquare otherFact)
        {
            return otherFact.CanExplore != _activated;
        }

        // Ne sera probablement pas utilise car on ne peut pas inferer son existence avec les regles actuel
        public override void Apply(WoodSquare square)
        {
            if (_activated) square.Unblock();
        }

        public override bool IsContainedIn(WoodSquare square)
        {
            return square.CanExplore == _activated;
        }

        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj)) return false;
            CanExplore otherCanexploreFact = obj as CanExplore;
            return _activated == otherCanexploreFact._activated;
        }
    }
}
