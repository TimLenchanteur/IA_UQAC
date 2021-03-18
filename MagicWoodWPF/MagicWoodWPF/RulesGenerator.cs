using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MagicWoodWPF
{
    /// <summary>
    /// Singleton qui recupere les regles creer par l'utilisateur
    /// </summary>
    class RulesGenerator
    {
        // Repertoire a partir duquel les regles sont generes
        // Si necessaire, doit etre modifier avant que les regles ne soit generer 
        // (Premier appel de l'instance ou appel a GenerateNewRules())
        static public string RulesDirectory = ""; 

        // Instance du singleton
        static RulesGenerator _instance;
        static public RulesGenerator Instance {
            get {
                if (_instance == null) {
                    _instance = new RulesGenerator();
                }
                return _instance;
            }     
        }

        // List des regles generer a partir du repertoire fourni
        List<Rule> _generatedRules;
        public List<Rule> GeneratedRules {
            get => _generatedRules;
        }
       
        RulesGenerator() {
            _generatedRules = new List<Rule>();
            GenerateNewRules();
        }

        public void GenerateNewRules() {
            _generatedRules.Clear();
            try
            {
                if (RulesDirectory == "") RulesDirectory = Directory.GetCurrentDirectory();
                var rulesFiles = Directory.EnumerateFiles(RulesDirectory, "*.xml", SearchOption.AllDirectories);
                XmlSerializer serializer = new XmlSerializer(typeof(Rule));

                foreach (string currentRule in rulesFiles)
                {
                    string fileName = currentRule.Substring(RulesDirectory.Length + 1);
                    Debug.WriteLine("File found : " + fileName);
                    // To read the file, create a FileStream.
                    using var myFileStream = new FileStream(fileName, FileMode.Open);
                    // Call the Deserialize method and cast to the object type.
                    Rule newRule = (Rule)serializer.Deserialize(myFileStream);
                    _generatedRules.Add(newRule);
                }
                return;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }
}
