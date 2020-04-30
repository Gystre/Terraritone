using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

public struct Move
{
    public Move(string name, Vector2 cost)
    {
        this.name = name;
        this.cost = cost;
    }

    public string name;
    public Vector2 cost;
}

public static class Moves
{
    //probably overthinking this...
    public static Move[] List =
    {
        new Move("TRAVERSE_LEFT", new Vector2(-1, 0)),
        new Move("TRAVERSE_RIGHT", new Vector2(+1, 0)),
        new Move("PILLAR_UP", new Vector2(0, -1)),
        new Move("DOWNWARDS", new Vector2(0, +1)),

        new Move("JUMP_LEFT", new Vector2(-6, 0)),
        new Move("JUMP_RIGHT", new Vector2(+6, 0))
    };

    //might be better as dictionary?
    //public static Dictionary<string, Vector2> List = new Dictionary<string, Vector2>()
    //{
    //    { "TRAVERSE_LEFT", new Vector2(-1, 0)},
    //    {"TRAVERSE_RIGHT",  new Vector2(+1, 0)},
    //    {"PILLAR_UP", new Vector2(0, +1)},
    //    {"DOWNWARDS", new Vector2(0, -1) },

    //    {"JUMP_LEFT", new Vector2(-6, 0) },
    //    {"JUMP_RIGHT", new Vector2(+6, 0) }
    //};
}
