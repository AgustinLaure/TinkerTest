using UnityEngine;
using System.Collections.Generic;

public abstract class Origami : MonoBehaviour
{
    public enum Moves
    {
        Up,
        Down,
        Left,
        Right
    }

    protected List<Moves> craftMoves;

    public bool isValid(List<Moves> moves)
    {
        bool isValid = true;

        if (moves.Count <= craftMoves.Count)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                if (moves[i] != craftMoves[i])
                {
                    isValid = false;
                }
            }
        }
        else
        {
            Debug.Log("Tried to input a larger move amount than the origami");
            return false;
        }

        return isValid;
    }

    public bool isDone(List<Moves> moves)
    {
        bool isDone = true;

        if (moves.Count != craftMoves.Count)
        {
            return false;
        }

        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i] != craftMoves[i])
            {
                isDone = false;
            }
        }

        return isDone;
    }
}
