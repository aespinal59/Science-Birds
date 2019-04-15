using System;
using System.Collections.Generic;
using System.IO;

namespace StringToStructure
{
    class LSystem
    {
        private Dictionary<string, Tuple<List<string>, List<double>>> rules;
        public List<string> iterations;

        public LSystem(string axiom, Dictionary<string, Tuple<List<string>, List<double>>> r)
        {
            rules = r;
            iterations = new List<string> { axiom };
        }

        public void Iterate()
        {
            string newAxiom = "";
            foreach (char symbol in iterations[iterations.Count - 1])
            {
                if (rules.ContainsKey(symbol.ToString()))
                {
                    WSelect wselect = new WSelect();
                    newAxiom += wselect.Select(rules[symbol.ToString()].Item1, rules[symbol.ToString()].Item2);
                }
                else
                {

                    newAxiom += symbol;
                }
            }
            iterations.Add(newAxiom);
        }

        public string this[int key]
        {
            get
            {
                return iterations[key];
            }
            set
            {
                iterations[key] = value;
            }
        }


    }

    class WSelect
    {
        private static Random random = new Random();

        public string Select(List<string> choices, List<double> weights)
        {
            double total = 100.0;
            double count = 0.0;
            double winner = random.Next(0, 100);

            for (int i = 0; i < choices.Count; i++)
            {
                count += weights[i] * total;
                if (winner < count)
                {
                    return choices[i];
                }
            }

            return "Error: choice was not picked.";
        }
    }

    class XML
    {
        public string Xmlify(string blockType, string material, double x, double y, double rot)
        {
            return String.Format(
                "<Block type=\"{0}\" material=\"{1}\" x=\"{2}\" y=\"{3}\" rotation=\"{4}\" />\n",
                blockType,
                material,
                x.ToString(),
                y.ToString(),
                rot.ToString());
        }

        public void StartFile(string path) 
        {
            string text = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" +
                "<Level width =\"2\">\n" +
                "<Camera x=\"0\" y=\"2\" minWidth=\"20\" maxWidth=\"30\">\n" +
                "<Birds>\n" +
                "<Bird type=\"BirdRed\"/>\n" +
                "<Bird type=\"BirdRed\"/>\n" +
                "<Bird type=\"BirdRed\"/>\n" +
                "</Birds>\n" +
                "<Slingshot x=\"-8\" y=\"-2.5\">\n" +
                "<GameObjects>\n";
            File.WriteAllText(path, text);
        }

        public void EndFile(string path) {
            string text = "</GameObjects>\n" +
                "</Level>\n";
            File.WriteAllText(path, text);
        }
    }

    class Test
    {
        static void Main(string[] args)
        {
            Dictionary<string, Tuple<List<string>, List<double>>> rules = new Dictionary<string, Tuple<List<string>, List<double>>>
                {
                    {"S", new Tuple<List<string>, List<double>>(new List<string> {"S", "ISI"}, new List<double> {0.80, 0.20})},
                    {"W", new Tuple<List<string>, List<double>>(new List<string> {"W", "SWS"}, new List<double> {0.60, 0.40})},
                    {"I", new Tuple<List<string>, List<double>>(new List<string> {"I", "IAIAI"}, new List<double> {0.20, 0.80})}
                };

            LSystem lsystem = new LSystem("W", rules);

            for (int i = 0; i < 5; i++) 
            {
                lsystem.Iterate();
            }

            foreach (string axiom in lsystem.iterations) 
            {
                Console.WriteLine(axiom);
            }
            Console.ReadKey();
        }
    }
}