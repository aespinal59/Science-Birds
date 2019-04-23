using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


//  Block coordinates (x, y) indicate the center of the block.
//  Floor is at y = -3.50 . 

class LSystem
{
    private Dictionary<string, Tuple<List<string>, List<double>>> rules;

    public List<string> iterations;

    public List<double> rowStartCoordinates;

    public List<List<List<double>>> blockCoordinates;

    public Dictionary<string, List<double>> blocks = new Dictionary<string, List<double>>
        {
            {"1", new List<double> {0.84, 0.84}},
            {"2", new List<double> {0.85, 0.43}},
            {"3", new List<double> {0.43, 0.85}},
            {"4", new List<double> {0.43, 0.43}},
            {"5", new List<double> {0.22, 0.22}},
            {"6", new List<double> {0.43, 0.22}},
            {"7", new List<double> {0.22, 0.43}},
            {"8", new List<double> {0.85, 0.22}},
            {"9", new List<double> {0.22, 0.85}},
            {"A", new List<double> {1.68, 0.22}},
            {"B", new List<double> {0.22, 1.68}},
            {"C", new List<double> {2.06, 0.22}},
            {"D", new List<double> {0.22, 2.06}}
        };


    //  TODO: fix issue with air blocks and height checks
    public Dictionary<string, string> block_names = new Dictionary<string, string>
    {
        {"0", "Air"},
        {"1", "SquareHole"},
        {"2", "RectFat"},
        {"3","RectFat"},
        {"4","SquareSmall"},
        {"5", "SquareTiny"},
        {"6", "RectTiny"},
        {"7", "RectTiny"},
        {"8", "RectSmall"},
        {"9", "RectSmall"},
        {"A", "RectMedium"},
        {"B", "RectMedium"},
        {"C", "RectBig"},
        {"D", "RectBig"}
    };

    public LSystem(string axiom, Dictionary<string, Tuple<List<string>, List<double>>> r)
    {
        rules = r;
        iterations = new List<string> { axiom };
        rowStartCoordinates = new List<double> { };

    }

    //  TODO
    public void GenerateRandomRules()
    {


    }

    //  TODO
    public void Crossover()
    {

    }

    //  TODO
    public void Mutation()
    {

    }

    // Change to iterate certain number of times so that GetStartCoordinates can be used maybe?
    public void Iterate(int numIter)
    {
        for (; numIter > 0; numIter--)
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

        blockCoordinates = new List<List<List<double>>>();
        for (int i = 0; i < iterations.Count; i++)
        {
            blockCoordinates.Add(new List<List<double>>());
            for (int j = 0; j < iterations[i].Length; j++)
            {
                blockCoordinates[i].Add(new List<double>());
            }
        }

        GetStartCoordinates(0.0);
        GetBlockCoordinates();

    }


    //  Update the list of start coordinates
    private void GetStartCoordinates(double structureCenterX)
    {

        double rowCenterX = structureCenterX;

        //  Find the start of each row (X-coordinate) by subtracting half of width from
        //  structure center.
        //  Goes from top row to bottom row.
        for (int rowIndex = 0; rowIndex < iterations.Count; rowIndex++)
        {

            double rowWidth = 0;


            foreach (char symbol in iterations[rowIndex])
            {
                rowWidth += blocks[symbol.ToString()][0];
            }

            double rowStart = rowCenterX - (rowWidth / 2);

            rowStartCoordinates.Add(rowStart);

        }


    }

    private void GetBlockCoordinates()
    {

        for (int rowIndex = iterations.Count - 1; rowIndex > -1; rowIndex--)
        {

            //  Get x coordinates
            double x = rowStartCoordinates[rowIndex];
            for (int colIndex = 0; colIndex < iterations[rowIndex].Length; colIndex++)
            {
                x += blocks[iterations[rowIndex][colIndex].ToString()][0] / 2;
                blockCoordinates[rowIndex][colIndex].Add(x);
                x += blocks[iterations[rowIndex][colIndex].ToString()][0] / 2;
            }

            //  Get y coordinates
            if (rowIndex == iterations.Count - 1)
            {
                for (int colIndex = 0; colIndex < iterations[rowIndex].Length; colIndex++)
                {
                    blockCoordinates[rowIndex][colIndex].Add(-3.5 + blocks[iterations[rowIndex][colIndex].ToString()][1] / 2);
                }
            }
            else
            {

                string bottomRow = iterations[rowIndex + 1], currRow = iterations[rowIndex];
                for (int colIndex = 0; colIndex < iterations[rowIndex].Length; colIndex++)
                {
                    List<double> checkInterval = new List<double> {
                        blockCoordinates[rowIndex][colIndex][0] - blocks[currRow[colIndex].ToString()][0],
                        blockCoordinates[rowIndex][colIndex][0] + blocks[currRow[colIndex].ToString()][0]
                    };

                    double maxHeight = -3.5;
                    for (int axIndex = 0; axIndex < bottomRow.Length; axIndex++)
                    {
                        string symbol = bottomRow[axIndex].ToString();
                        double blockWidth = blocks[symbol][0];
                        double blockCenterX = blockCoordinates[rowIndex + 1][axIndex][0];
                        double leftEdge = blockCenterX - (blockWidth / 2);
                        double rightEdge = blockCenterX + (blockWidth / 2);

                        if (
                        (leftEdge > checkInterval[0] && leftEdge < checkInterval[1]) ||
                        (rightEdge > checkInterval[0] && rightEdge < checkInterval[1]) ||
                        (leftEdge < checkInterval[0] && rightEdge > checkInterval[1])
                        )
                        {
                            double blockHeight = blocks[symbol][1];
                            double blockTopEdgeHeight = blockCoordinates[rowIndex + 1][axIndex][1] + blockHeight / 2;
                            if (blockTopEdgeHeight > maxHeight)
                            {
                                maxHeight = blockTopEdgeHeight;
                            }
                        }
                    }

                    blockCoordinates[rowIndex][colIndex].Add(maxHeight + blocks[currRow[colIndex].ToString()][1]);
                }
            }


        }

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
