using System;
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
        [XmlArray(ElementName = "Declencheurs")]
        [XmlArrayItem(typeof(Fact))]
        [XmlArrayItem(typeof(ElementIsOn))]
        [XmlArrayItem(typeof(ClueIsOn))]
        public Fact[] _triggers;
        
        // Faits concluant la regle
        [XmlArray(ElementName = "Corps")]
        [XmlArrayItem(typeof(Fact))]
        [XmlArrayItem(typeof(ElementIsOn))]
        [XmlArrayItem(typeof(ClueIsOn))]
        public Fact[] _body;

        [XmlIgnore]
        protected bool _isAbstract;

        [XmlIgnore]
        protected Vector2 _XValue;

        protected Rule() {
            _isAbstract = true;
        }

        protected Rule(Vector2 xValue, Fact[] triggers, Fact[] body) {
            _XValue = xValue;
            _isAbstract = false; 
            _triggers = triggers;
            _body = body;
        }

        /// <summary>
        /// A partir d'une liste de fait, verifie que si les faits necessaire pour activer cette regle existe
        /// Si c'est le cas construit une nouvelle regle equivalente avec les faits non abstrait
        /// Seule facon de creer une regle pendant le runtime
        /// </summary>
        /// <returns>Une regle equivalente avec des fait non abstrait</returns>
        public Rule RuntimeRule(List<Fact> currentBeliefs) {
            Fact[] triggers =new Fact[_triggers.Length];
            int countTrigger = 0;
            Vector2 xValue = new Vector2(0,0);
            foreach (Fact fact in currentBeliefs) {
                for (int i = 0; i < _triggers.Length; i++) {
                    if (fact.Equals(_triggers[i]) && triggers[i] == null) {
                        triggers[i] = fact;
                        countTrigger += 1;

                        //Define x value
                        throw new NotImplementedException();

                        break;
                    }
                }
            }
            if (countTrigger == _triggers.Length) {
                return new Rule(xValue, triggers, CreateBody(triggers));
            }
            return null;
        }

        Vector2 defineXValue(Fact abstractfact, Fact definedFact) {
            /*switch () { 
                
            }*/
            throw new NotImplementedException();
        }

        Fact[] CreateBody(Fact[] triggers) {
            Fact[] body = new Fact[_body.Length];

            throw new NotImplementedException();
        }

        /// <summary>
        /// Defini si le fait est equivalent a un autre fait 
        /// </summary>
        /// <param name="otherFact">L'autre fait propose</param>
        /// <returns>Vrai si les faits sont equivalent, faux sinon</returns>
        public override bool Equals(Object obj)
        {
            Rule otherRule = obj as Rule;
            if (otherRule == null) return false;

            for (int i = 0; i < _triggers.Length; i++) {
                if (!_triggers[i].Equals(otherRule._triggers[i])) return false;
            }
            for (int i = 0; i < _body.Length; i++)
            {
                if (!_body[i].Equals(otherRule._body[i])) return false;
            }
            return true;
        }
    }
}
