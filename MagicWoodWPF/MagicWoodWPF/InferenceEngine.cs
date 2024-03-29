﻿using System;
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
        /// <returns>Les nouvelles croyances de l'agent</returns>
        static public void InferenceCycle(ref WoodSquare[,] beliefs, List<Rule> rules) {

            List<Rule> markedRules = new List<Rule>();
            List<Rule> relevantRules = new List<Rule>();

           // Chainage avant en largeur etant donne qu'on ne peut pas vraiment atteindre le but de l'agent a partir des regles
           // Strategie de recherche en profondeur

           // Filtre les regles applicable
           FilterRules(beliefs, rules, ref markedRules, ref relevantRules);

            while (relevantRules.Count > 0) {
                // Choisi la regle
                // On choisis la derniere regle ajouter car ce sera celle la plus en profondeur dans l'arbre 
                // (Elle aura ete debloquer par la regle precedente)
                Rule choosenRule = relevantRules[relevantRules.Count - 1];

                // Applique la regle
                choosenRule.Apply(ref beliefs);

                //Marque la regle
                markedRules.Add(choosenRule);
                relevantRules.Remove(choosenRule);

                // Filtre les nouvelles regles applicable
                FilterRules(beliefs, rules, ref markedRules, ref relevantRules);
            }
        }

        /// <summary>
        /// Filtre les regles possible a execute a partir des croyances actuelles
        /// </summary>
        /// <param name="beliefs">Croyances actuel</param>
        /// <param name="everyAbstractRules">Ensemble des regles abstraite possible sur les croyances</param>
        /// <param name="markedRules">Regle marque ne pouvant plus etre utilise</param>
        /// <param name="currentRelevantRules">Regles que l'on pourrait utilise mais qui n'on pas encore ete traite</param>
        /// <returns>Les regles qu'il est possible d'execute</returns>
        static void FilterRules(WoodSquare[,] beliefs, List<Rule> everyAbstractRules,ref List<Rule> markedRules, ref List<Rule> currentRelevantRules) {

            // Dans un premier temps on verifie que les regles qui avais ete choisi precedement sont toujours valable
            List<Rule> rulesToRemove = new List<Rule>();
            foreach (Rule rule in currentRelevantRules) {
                bool conflict = false;
                if (rule.BecameIrrelevant(beliefs, ref conflict)){
                    rulesToRemove.Add(rule);
                    // Si l'invalidite est cause par un conflit on marque la regle
                    if(conflict) markedRules.Add(rule);
                }
            }
            currentRelevantRules.RemoveAll(rule => rulesToRemove.Contains(rule));

            // Genere toute les regles concrete applicable a partir de la base de fait courante
            foreach (Rule rule in everyAbstractRules)
            {
                List<Rule> realRules = rule.RelevantsRules(beliefs);
                // Verifie que les nouvelles regles n'existe pas deja ou ne sont pas deja invalide
                // Un peu plus long mais seul moyen de s'assurer que les nouvelles regles sont les dernieres ajoute (Pour la recherche en profondeur)
                List<Rule> toRemove = new List<Rule>();
                foreach (Rule newRule in realRules){
                    if (markedRules.Contains(newRule) || currentRelevantRules.Contains(newRule)) toRemove.Add(newRule);
                    if (newRule.IsInConflict(beliefs)){
                        toRemove.Add(newRule);
                        markedRules.Add(rule);
                    }
                   
                }
                foreach (Rule ruleToRemove in toRemove) {
                    realRules.Remove(ruleToRemove);
                }
                currentRelevantRules.AddRange(realRules);
            }
            return;
        }
    }
}
