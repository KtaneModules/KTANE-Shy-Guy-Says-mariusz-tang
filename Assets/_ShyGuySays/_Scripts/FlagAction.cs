using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlagAction {
    public FlagAction(float speed, params FlagRaise[] raises) {
        if (speed < 0) {
            throw new ArgumentException("Time must be positive.");
        }

        if (raises.Select(raise => raise.Position).Distinct().Count() != raises.Length) {
            throw new ArgumentException("Cannot flip the same flag more than once at the same time.");
        }

        Raises = raises;
        Speed = speed;
    }

    public FlagRaise[] Raises { get; private set; }
    public float Speed { get; private set; }
}

public class FlagRaise {

    public FlagRaise(int position, Color colour, char letter) {
        Position = position;
        Colour = colour;
        Letter = letter;
    }

    public int Position { get; private set; }
    public Color Colour { get; private set; }
    public char Letter { get; private set; }
}
