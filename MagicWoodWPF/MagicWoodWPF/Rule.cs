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
        [XmlArrayItem(typeof(Explored))]
        [XmlArrayItem(typeof(RockThrown))]
        public Fact[] _triggers;

        // Faits concluant la regle
        [XmlArray(ElementName = "Corps")]
        [XmlArrayItem(typeof(Fact))]
        [XmlArrayItem(typeof(ElementIsOn))]
        [XmlArrayItem(typeof(ClueIsOn))]
        [XmlArrayItem(typeof(Explored))]
        [XmlArrayItem(typeof(RockThrown))]
        public Fact[] _body;

        // Defini si une regle est abstraite
        // Une regle est abstraite si elle contient au moins un fait abstrait
        [XmlIgnore]
        protected bool _isAbstract;

        // Valeur concrete associe a la valeur X d'une position pour une regle concrete
        [XmlIgnore]
        protected Vector2 _positionXValue;

        /// <summary>
        /// Constructeur
        /// </summary>
        protected Rule() {
            _isAbstract = true;
        }

        /// <summary>
        /// Constructeur de regle concrete
        /// </summary>
        /// <param name="triggers">Une liste de declencheur contenant uniquement des fait abstrait a traduire</param>
        /// <param name="body">Une liste modelisant le corps de la regle contenant uniquement des faits abstrait a traduire</param>
        protected Rule(Vector2 positionXValue, Fact[] triggers, Fact[] body) {
            _isAbstract = false;
            _positionXValue = positionXValue;

            _triggers = BuildReal(triggers);
            _body = BuildReal(body);
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
            foreach (Fact trigger in _triggers){
                foreach (Fact belief in currentBeliefs) {
                    if (trigger.InConflictWith(belief)) return true;
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
                List<Rule> tempRelevantRule = new List<Rule>();
                // Pour chaque fait dans les croyances on creer autant de regle concrete que de declencheur equivalent a une croyance dans la regle
                foreach (Fact fact in currentBeliefs) {
                    foreach (Fact trigger in _triggers) {
                        if (fact.IsEquivalent(trigger)) {
                            // Traduit les faits abstraits
                            // Le but est de retrouver la valeur de X et non de XDessus ou autre
                            Vector2 xPosition;
                            if (trigger.isAbstract())
                            {
                                xPosition = fact.GetPosition();
                                switch (trigger._abstractPos) {
                                    case AbstractVector.Up:
                                        xPosition.Y -= 1;
                                        break;
                                    case AbstractVector.Down:
                                        xPosition.Y += 1;
                                        break;
                                    case AbstractVector.Right:
                                        xPosition.X -= 1;
                                        break;
                                    case AbstractVector.Left:
                                        xPosition.X += 1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else {
                                xPosition = trigger.GetPosition();
                            }

                            // Creer une regle a partir de la traduction
                            Rule newRealRule = new Rule(xPosition, _triggers, _body);
                            // Ajoute cette regle a la liste des regles a tester
                            tempRelevantRule.Add(newRealRule);
                        } 
                    }
                }
     
                // Pour toute les regles qui viennent d'etre creer verifier que la regle peut etre declencher
                foreach (Rule rule in tempRelevantRule) {
                    bool isRelevant = true;
                    float triggerCertaintyFactor = float.MaxValue;
                    bool triggerAreUncertain = false;
                    foreach (Fact trigger in rule._triggers) {
                        //Si le trigger n'existe pas dans les croyances actuelle on peut oublier la regle
                        if (!currentBeliefs.Contains(trigger)) {
                            isRelevant = false;
                            break;
                        }
                        // On ajoute si necessaire le facteur de certitude au calcul
                        Fact realTrigger = currentBeliefs[currentBeliefs.IndexOf(trigger)];
                        if (realTrigger.isUncertain()) {
                            triggerAreUncertain = true;
                            triggerCertaintyFactor = MathF.Min(triggerCertaintyFactor, realTrigger.GetCertaintyFactor());
                        }
                    }
                    // Si la regle peut etre ajouter on l'ajoute au regles pertinente et on calcul le facteur de certitude des faits composant son corp
                    if (isRelevant) {
                        if(triggerAreUncertain) rule.ComputeBodyCertaintyFactor(triggerCertaintyFactor);
                        relevantRules.Add(rule);
                    }
                }
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
        /// Construit des faits concret pour une regle concrete a partir de leurs equivalent abstraits
        /// </summary>
        /// <param name="abstractFacts">Faits a traduire</param>
        /// <returns>Un tableau de fait concret equivalent au fait proposer</returns>
        public Fact[] BuildReal(Fact[] abstractFacts) {
            Fact[] realFacts = new Fact[abstractFacts.Length];

            for (int i = 0; i < abstractFacts.Length; i++) {
                Fact abstractFact = abstractFacts[i];
                realFacts[i] = BuildReal(abstractFact);
            }

            return realFacts;
        }

        /// <summary>
        /// Construit un fait concret pour une regle concrete a partir son equivalent abstrait
        /// </summary>
        /// <param name="abstractFact">Fait a traduire</param>
        /// <returns>Le fait concret equivalent</returns>
        public Fact BuildReal(Fact abstractFact)
        {
            Fact newRealFact = null;

            // Traduit la position
            Vector2 position = abstractFact.TranslatePosition(_positionXValue);

            switch (abstractFact.GetID()) {
                case FactID.FACTID_ELEMENTS:
                    ElementIsOn elementIsOn = (ElementIsOn)abstractFact;
                    if(elementIsOn.isUncertain()) newRealFact = new ElementIsOn(position, elementIsOn._object, elementIsOn.TranslateProbability());
                    else newRealFact = new ElementIsOn(position, elementIsOn._object);
                    break;
                case FactID.FACTID_CLUE:
                    ClueIsOn clueIsOn = (ClueIsOn)abstractFact;
                    if(clueIsOn.isUncertain()) newRealFact = new ClueIsOn(position, clueIsOn._clue, clueIsOn.TranslateProbability());
                    else newRealFact = new ClueIsOn(position, clueIsOn._clue);
                    break;
                case FactID.FACTID_EXPLORE:
                    Explored explored = (Explored)abstractFact;
                    // Fait toujours certain
                    newRealFact = new Explored(position, explored._deathCount);
                    break;
                case FactID.FACTID_ROCK:
                    // Fait toujours certain
                    newRealFact = new RockThrown(position);
                    break;
                default:
                    break;
            }
            return newRealFact;
        }

        /// <summary>
        /// Met a jour le facteur de certitude des faits du corps a partir de celui calculer des declencheurs
        /// </summary>
        /// <param name="triggerCertaintyFactor">Facteur de certitude calculer sur les declencheurs</param>
        void ComputeBodyCertaintyFactor(float triggerCertaintyFactor) {
            foreach (Fact body in _body) {
                body.UpdateCertaintyFactorBy(triggerCertaintyFactor);
            }
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
