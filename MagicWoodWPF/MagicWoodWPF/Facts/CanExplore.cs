﻿using System;
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
