using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BoolMap
{
    public bool[,] colors;
    public BoolMap parent;
    public Coord turnCoord;
    public int turns;

    public BoolMap(Node[,] nodes, int mainColor)
    {
        this.colors = ArrayRetyper.RetypeArray<Node, bool>(nodes, n => n != null && n.color == mainColor);
        this.parent = null;
    }

    public BoolMap(bool[,] colors)
    {
        this.colors = colors;
        this.parent = null;
    }
    
    private BoolMap(bool[,] colors, BoolMap parent)
    {
        this.colors = colors;
        this.parent = parent;
    }
    
    public IEnumerable<BoolMap> PossibleSteps()
    {
        List<BoolMap> boolMapList = new List<BoolMap>();
        foreach (var c in TrueCoordGroupNeighbours())
        {
            if (CoordIsRotable(c))
            {
                for (int i = 1; i < 6; i++)
                {
                    BoolMap boolMap = Copy();
                    boolMap.Rotate(c, i);
                    boolMapList.Add(boolMap);
                }
            }
        }

        return boolMapList;
    }

    private void Rotate(Coord c, int turns)
    {
        if (!CoordIsInMap(c))
            return;
        if (turns == 0)
            return;
        
        int x = c.x;
        int y = c.y;

        bool t;

        for (int i = 0; i < turns; i ++) {
            if (y % 2 == 1) {
                t = colors[x-1, y];

                colors[x - 1, y] = colors[x - 1, y - 1];
                colors[x - 1, y - 1] = colors[x, y - 1];
                colors[x, y - 1] = colors[x + 1, y];
                colors[x + 1, y] = colors[x, y + 1];
                colors[x, y + 1] = colors[x - 1, y + 1];
                colors[x - 1, y + 1] = t;

            } else {
                t = colors[x-1, y];

                colors[x - 1, y] = colors[x, y - 1];
                colors[x, y - 1] = colors[x + 1, y - 1];
                colors[x + 1, y - 1] = colors[x + 1, y];
                colors[x + 1, y] = colors[x + 1, y + 1];
                colors[x + 1, y + 1] = colors[x, y + 1];
                colors[x, y + 1] = t;
            }
        }

        turnCoord = c;
        this.turns = turns;
    }

    private List<Coord> TrueCoordGroupNeighbours()
    {
        List<Coord> allCoords = new List<Coord>();

        for (int i = 0; i < colors.GetLength(0); i++)
        {
            for (int j = 0; j < colors.GetLength(1); j++)
            {
                Coord c = new Coord(i, j);
                if (CoordIsTrue(c) && !allCoords.Contains(c))
                {
                    allCoords.AddRange(TrueCoordGroup(c));
                }
            }   
        }
        
        List<Coord> allCoordsWithNeighbours = new List<Coord>();
        allCoordsWithNeighbours.AddRange(allCoords);
        
        foreach (var coord in allCoords)
        {
            foreach (var c in NearCoords(coord))
            {
                if (!allCoordsWithNeighbours.Contains(c) && CoordIsInMap(c))
                    allCoordsWithNeighbours.Add(c);   
            }
        }

        return allCoordsWithNeighbours;
    }
    
    public int GroupsCount ()
    {
        List<Coord> allCoords = new List<Coord>();
        int groupsCount = 0;

        for (int i = 0; i < colors.GetLength(0); i++)
        {
            for (int j = 0; j < colors.GetLength(1); j++)
            {
                Coord c = new Coord(i, j);
                if (CoordIsTrue(c) && !allCoords.Contains(c))
                {
                    allCoords.AddRange(TrueCoordGroup(c));
                    groupsCount++;
                }
            }   
        }

        return groupsCount;
    }

    private List<Coord> TrueCoordGroup(Coord coord)
    {
        List<Coord> coordGroup = new List<Coord>();
        
        Queue<Coord> queue = new Queue<Coord> ();
        queue.Enqueue (coord);
        coordGroup.Add (coord);
        Coord particularCoord;
		
        while (queue.Count != 0) {
            particularCoord = queue.Dequeue();
            foreach (Coord c in NearCoords(particularCoord)) {
                if (CoordIsTrue(c) && !coordGroup.Contains(c))
                {
                    coordGroup.Add(c);
                    queue.Enqueue(c);
                }
            }
        }

        return coordGroup;
    }
    
    private static List <Coord> NearCoords (Coord coord) {
        int x = coord.x;
        int y = coord.y;
        List <Coord> nearCoords = new List<Coord> ();

        nearCoords.Add (new Coord(x, y));
        
        nearCoords.Add (new Coord(x + 1, y));
        nearCoords.Add (new Coord(x - 1, y));
        nearCoords.Add (new Coord(x, y + 1));
        nearCoords.Add (new Coord(x, y - 1));

        if (y % 2 == 1) {
            nearCoords.Add (new Coord(x - 1, y + 1));
            nearCoords.Add (new Coord(x - 1, y - 1));
        } else {
            nearCoords.Add (new Coord(x + 1, y + 1));
            nearCoords.Add (new Coord(x + 1, y - 1));
        }

        return nearCoords;
    }

    private bool CoordIsTrue(Coord c)
    {
        if (!CoordIsInMap(c))
            return false;

        return colors[c.x, c.y];
    }

    private bool CoordIsInMap(Coord c)
    {
        if (c.x < 0 || c.x >= colors.GetLength(0))
            return false;
        if (c.y < 0 || c.y >= colors.GetLength(1))
            return false;
        if (c.y % 2 == 0 && c.x >= colors.GetLength(0) - 1)
            return false;

        return true;
    }

    private bool CoordIsRotable(Coord c)
    {
        if (c.x < 1 || c.x >= colors.GetLength(0) - 1)
            return false;
        if (c.y < 1 || c.y >= colors.GetLength(1) - 1)
            return false;
        if (c.y % 2 == 0 && c.x >= colors.GetLength(0) - 2)
            return false;

        return true;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < colors.GetLength(0); i++)
        {
            for (int j = 0; j < colors.GetLength(1); j++)
            {
                sb.Append(colors[i, j]).Append(", ");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public BoolMap Copy()
    {
        bool[,] newColors = new bool[colors.GetLength(0), colors.GetLength(1)];
        for (int i = 0; i < colors.GetLength(0); i++)
        {
            for (int j = 0; j < colors.GetLength(1); j++)
            {
                newColors[i, j] = colors[i, j];
            }
        }
        return new BoolMap(newColors, this);
    }

    private bool this[int i, int j]
    {
        get { return colors[i, j]; }
    } 
}
