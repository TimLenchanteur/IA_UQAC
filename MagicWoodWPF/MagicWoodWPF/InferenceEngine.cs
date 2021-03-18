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
        static public List<Fact> InferenceCycle(List<Fact> beliefs, List<Rule> rules, Fact goal) {

            // Pas sur que la boucle soit applicable ?
            List<Fact> newBeliefs = new List<Fact>(beliefs);

            // Si le but a ete atteint pas besoin de mettre a jour les fait
            while (GoalReached()) {
                // Filtre les regles applicable
                List<Rule> relevantRules = FilterRules(newBeliefs, rules);

                // Choose Rule 
                Rule choosenRule = ChooseRuleBasedOnGoal(relevantRules, goal);

                // Apply rule
                ApplyRule(choosenRule, out newBeliefs);
            }
            return newBeliefs;
        }

        /// <summary>
        /// Defini si la croyance a atteindre l'a ete
        /// </summary>
        /// <returns>Vrai si c'est le cas, faux sinon</returns>
        static bool GoalReached() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Filtre les regles possible a execute a partir des croyances actuelles
        /// </summary>
        /// <param name="beliefs">Croyances actuel</param>
        /// <param name="rules">Ensemble des regles possible sur les croyances</param>
        /// <returns>Les regles qu'il est possible d'execute</returns>
        static List<Rule> FilterRules(List<Fact> beliefs, List<Rule> rules) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Choisi une regle parmis celle propose a partir du but de l'agent
        /// </summary>
        /// <param name="rules">Ensemble de regles sur les croyances</param>
        /// <param name="goal">But a atteindre</param>
        /// <returns>La regle choisi</returns>
        static Rule ChooseRuleBasedOnGoal(List<Rule> rules, Fact goal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applique une regle sur les croyances
        /// </summary>
        /// <param name="ruleToApply">Regle a appliquer</param>
        /// <param name="currentBeliefs">En entree, les croyance actuelle. En les croyance apres appliquations de la regle</param>
        static void ApplyRule(Rule ruleToApply, out List<Fact> currentBeliefs)
        {
            throw new NotImplementedException();
        }
    }
}
