﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MagicWoodWPF.Facts;

namespace MagicWoodWPF
{
    /// <summary>
    /// Regles generiques qui devront etre suivis par l'agent
    /// </summary>
    [XmlRoot(ElementName = "Regle")]
    public class Rule
    {
        // Indique si les declencheurs doivent disparaitre appres utilisation de la regles
        [XmlAttribute(DataType = "boolean", AttributeName = "ConsommeDeclencheurs")]
        public bool _useTriggers;

        // Faits declencheurs de la regle
        [XmlArray(ElementName = "Declencheurs"), 
        XmlArrayItem(Type = typeof(Fact)),
        XmlArrayItem(Type = typeof(CanLeave))]
        public Fact[] _triggers;
        
        // Faits concluant la regle
        [XmlArray(ElementName = "Corps"),
        XmlArrayItem(Type = typeof(Fact)),
        XmlArrayItem(Type = typeof(CanLeave))]
        public Fact[] _body;

        public Rule() {}
    }
}
