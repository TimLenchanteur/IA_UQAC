using System;
using System.Collections.Generic;
using System.Text;
using MagicWoodWPF.Facts;

namespace MagicWoodWPF
{
    static class InferenceEngine
    {
        /// <summary>
        /// Execute un cycle d'inference a partir des croyances, regles et but indiquer
        /// </summary>
        /// <param name="beliefs">Croyances initial</param>
        /// <param name="rules">Regles sur les croyances</param>
        /// <param name="goal">Croyance a atteindre</param>
        /// <returns>Les nouvelles croyances de l'agent</returns>
        static public List<Fact> InferenceCycle(List<Fact> beliefs, List<Rule> rules) {

            List<Fact> newBeliefs = new List<Fact>(beliefs);
            List<Rule> markedRules = new List<Rule>();

            // Chainage avant en largeur etant donne qu'on ne peut pas vraiment atteindre le but a partir des regles
            // Si le but a ete atteint pas besoin de mettre a jour les fait

            // Filtre les regles applicable
            List<Rule> relevantRules = FilterRules(newBeliefs,rules, ref markedRules);
            while (relevantRules.Count != 0) {
                // Choisi la regle
                Rule choosenRule = ChooseNextRule(relevantRules);

                // Applique la regle
                ApplyRule(choosenRule, ref newBeliefs);

                //Marque la regle
                markedRules.Add(choosenRule);

                // Filtre les nouvelles regles applicable
                relevantRules = FilterRules(newBeliefs, rules, ref markedRules);
            }
            return newBeliefs;
        }

        /// <summary>
        /// Filtre les regles possible a execute a partir des croyances actuelles
        /// </summary>
        /// <param name="beliefs">Croyances actuel</param>
        /// <param name="rules">Ensemble des regles possible sur les croyances</param>
        /// <returns>Les regles qu'il est possible d'execute</returns>
        static List<Rule> FilterRules(List<Fact> beliefs, List<Rule> rules, ref List<Rule> markedRules) {
            List<Rule> filteredRules = new List<Rule>();
            foreach(Rule rule in rules){
                if (markedRules.Contains(rule)) continue;
                // Si la regle contient des fait contradictoire avec les croyance, la marque
                throw new NotImplementedException();
                // Si des fait permettant de declancher cette regle existent,
                // on recupere une regles equivalente cree a partir des faits declencheur pour avoir une regle qui n'est plus abstraite
                Rule runtimeRule = rule.RuntimeRule(beliefs);
                if (runtimeRule != null) filteredRules.Add(runtimeRule);
            }
            return filteredRules;
        }

        /// <summary>
        /// Choisi une regle parmis celle propose 
        /// </summary>
        /// <param name="rules">Ensemble de regles sur les croyances</param>
        /// <returns>La regle choisi</returns>
        static Rule ChooseNextRule(List<Rule> rules)
        {
           
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applique une regle sur les croyances
        /// </summary>
        /// <param name="ruleToApply">Regle a appliquer</param>
        /// <param name="currentBeliefs">En entree, les croyance actuelle. En les croyance apres appliquations de la regle</param>
        static void ApplyRule(Rule ruleToApply, ref List<Fact> currentBeliefs)
        {
            // Applique la regle
            // Ajoute ou fusionne les faits resultants
            throw new NotImplementedException();
        }
    }
}
