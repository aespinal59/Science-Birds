using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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

    class WSelect
    {
        public string Select(List<string> choices, List<double> weights)
        {
            double total = 100.0;
            double count = 0.0;
            double winner = UnityEngine.Random.Range(0, 100);

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
}
