using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MagicWoodWPF.Facts
{
    public struct FactID {
        public const int FACTID_NONE = 0;
        public const int FACTID_LEAVE = 1;
    }

    public enum AbstractVector
    {
        [XmlEnum(Name = "X")]
        originalPos,
        [XmlEnum(Name = "XUp")]
        Up,
        [XmlEnum(Name = "XDown")]
        Down,
        [XmlEnum(Name = "XRight")]
        Right,
        [XmlEnum(Name = "XLeft")]
        Left
    }

    [XmlType(TypeName = "Fait")]
    public class Fact
    {
        // identifiant permettant de connaitre le type de la variable pour upcast
        [XmlIgnore]
        protected int _id;

        // Defini si le fait est abstrait ou non
        // Les faits abstraits ne devrait etre que ceux serialiser, les non abstrait sont ceux creer pendant le runtime
        // Cette variable permet d'utiliser une notation abstraite comme X pour definir une variable qui ne sera connu que pendant le runtime
        [XmlIgnore]
        protected bool _isAbstract;

        protected Fact() {
            _id = FactID.FACTID_NONE;
            _isAbstract = true;
        }

        public virtual bool IsEquals() {
            return _id == 0;
        }


    }
}
