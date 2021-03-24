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

        // Defini si une regle est abstraite
        // Une regle est abstraite si elle contient au moins un fait abstrait
        [XmlIgnore]
        protected bool _isAbstract;

        /// <summary>
        /// Constructeur
        /// </summary>
        protected Rule() {
            _isAbstract = true;
        }

        /// <summary>
        /// Constructeur de regle concrete
        /// </summary>
        /// <param name="triggers">Une liste de declencheur contenant uniquement des fait concret</param>
        /// <param name="body">Une liste modelisant le corps de la regle contenant uniquement des faits concrets</param>
        protected Rule(Fact[] triggers, Fact[] body) {
            _isAbstract = false; 
            _triggers = triggers;
            _body = body;
        }

        /// <summary>
        /// Indique si la regle est abstraite ou non 
        /// </summary>
        /// <returns>Vrai si la regle est abstraite, faux sinon</returns>
        public bool IsAbstract()
        {
            return _isAbstract;
        }

        /// <summary>
        /// Applique la regles sur la base de fait renseigner
        /// </summary>
        /// <param name="currentFact">Une base de fait, apres application elle pourra contenir des faits differents</param>
        public void Apply(ref List<Fact> currentFacts)
        {
            // Si la regle doit consommer les declencheur elle les enleve de la base de fait 
            if (_useTriggers)
            {
                foreach (Fact trigger in _triggers)
                {
                    currentFacts.Remove(trigger);
                }
            }

            foreach (Fact newFact in _body)
            {
                // Si le fait est deja contenu dans la base de donnees le fusionner (Utile pour le facteur de certitude)
                if (currentFacts.Contains(newFact))
                {
                    int index = currentFacts.IndexOf(newFact);
                    currentFacts[index].Merge(newFact);
                }
                else
                {
                    currentFacts.Add(newFact);
                }
            }
        }

        /// <summary>
        /// Indique qu'une regle n'est pas utilisable du fait d'un fait contradictoire ou de fait non present
        /// </summary>
        /// <param name="currentBeliefs"></param>
        /// <returns></returns>
        public bool BecameIrrelevant(List<Fact> currentBeliefs, ref bool isInConflict) {
            if (IsInConflict(currentBeliefs)) {
                isInConflict = true;
                return true;
            }
            foreach(Fact trigger in _triggers){
                if (!currentBeliefs.Contains(trigger)) return true;
            }
            return false;
        }

        /// <summary>
        /// Indique qu'une regle est entree en conflit avec un plusieurs fait d'une base de fait
        /// </summary>
        /// <param name="currentBeliefs">La base de fait a verifie</param>
        /// <returns></returns>
        protected bool IsInConflict(List<Fact> currentBeliefs) {
            foreach (Fact trigger in _triggers)
            {
                foreach (Fact belief in currentBeliefs) {
                    if (!trigger.InConflictWith(belief)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Renvoie une liste de toute les regles qui on pu etre construite a partir de cette regle et d'une base de fait
        /// </summary>
        /// <param name="currentBeliefs">Une base de fait</param>
        /// <returns>La liste de toute les regles cree a partir de la regle abstraite</returns>
        public List<Rule> RelevantsRules(List<Fact> currentBeliefs) {
            List<Rule> relevantRules = new List<Rule>();
            if (_isAbstract) {
                // On cherche assez de fait equivalent avec les declencheurs pour que l'on puisse traduire tout les elements abstrait de la regle
                // On cree une base de traduction d'element abstrait

                // Une fois que l'on a traduit la regle on verifie si elle peut etre declenche et si c'est le cas on l'ajoute a la liste

                // On recommence pour voir si il n'est pas possible d'interpreter la regle differement avec les autres fait 

                throw new NotImplementedException();
            }
            else {
                //Verifie que tout les fait necessaire pour valider cette regle sont present dans la base
                bool relevant = true;
                foreach (Fact trigger in _triggers) {
                    if (!currentBeliefs.Contains(trigger)) {
                        relevant = false;
                        break;
                    }
                }
                //Si c'est le cas renvoye uniquement cette regle
                if (relevant) relevantRules.Add(this);
            }
            return relevantRules;
        }

        /// <summary>
        /// Defini si un objet est egale a cette regle
        /// </summary>
        /// <param name="obj">L'autre objet propose</param>
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
