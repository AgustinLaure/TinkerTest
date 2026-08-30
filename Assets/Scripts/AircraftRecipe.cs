using System.Collections.Generic;
using UnityEngine;

public class AircraftRecipe : Origami
{
    private void Start()
    {
        craftMoves = new List<Moves>();

        craftMoves.Add(Moves.Left);
        craftMoves.Add(Moves.Right);
        craftMoves.Add(Moves.Left);
        craftMoves.Add(Moves.Right);
    }
}
