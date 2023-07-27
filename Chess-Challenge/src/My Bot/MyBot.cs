﻿using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

public class MyBot : IChessBot
{
    int baseMaxDepth = 4;
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    bool botIsWhite;

    public Move Think(Board board, Timer timer)
    {
        botIsWhite = board.IsWhiteToMove;
        Move[] allmoves = board.GetLegalMoves();

        Move bestmove = allmoves[0];
        int bestMoveAdvantage = -int.MaxValue;
        foreach (Move move in allmoves) //I hate this, this smells, refactor tmrw
        {
            board.MakeMove(move);
            int moveAdvantage = Evaluate(board, 0);
            board.UndoMove(move);
            if (moveAdvantage > bestMoveAdvantage)
            {
                bestMoveAdvantage = moveAdvantage;
                bestmove = move;
            }
        }
        return bestmove;
    }

    private int Evaluate(Board board, int currentDepth)
    {
        bool ourTurn = botIsWhite == board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();
        int[] boardValues = new int[moves.Length];

        if (board.IsInCheckmate())
        {
            return ourTurn ? -int.MaxValue : int.MinValue; //Returns the lowest value if we are checkmated and highest value if the enemy is mated
        }

        if (currentDepth == baseMaxDepth)
        {
            return CalculateAdvantage(board);
        }

        for (int i = 0; i < boardValues.Length; i++)
        {
            board.MakeMove(moves[i]);
            boardValues[i] = Evaluate(board, currentDepth + 1);
            board.UndoMove(moves[i]);
        }
        Console.WriteLine(boardValues.Length);
        return ourTurn ? boardValues.Max() : boardValues.Min(); //this results in a crash if there were no legal moves
        //This should be reworked so that the "tree" stores the best moves as well so that we can save on calculations.
        //Maybe make node objects??

        //personal notes: could possibly be optimised as Max and Min might iterate through the whole list again?

        /*Explanation:
        This is a recursive function, what happens is that it calculates the boardvalues at the very end on a board, then, works backwards.

        Look at the diagram below, this function calculates the advantage of the top most board, then the onee below top board. The "parent" board's value is equal to either
        the highest value child's value if it is our turn, as we would pick the best move, or the lowest value child if it is the enemy turn, as the enemy would pick the best move 
        for them. This then repeats
              []
             /
           []       
          /  \_[]
        []
          \  /-[]
           []
             \_[]
        */

    }
    private int CalculateAdvantage(Board board)
    {
        int materialAdvantage = CalculateMaterialAdvantage(board);
        int positionalAdvantage = CalculatePositionalAdvantage(board);
        int boardValue = materialAdvantage + positionalAdvantage;

        return boardValue;
    }

    private int CalculateMaterialAdvantage(Board board)
    {
        PieceList[] pieceListList = board.GetAllPieceLists();
        int whiteMaterialValue = pieceListList.Take(6).Sum(list => list.Count * pieceValues[(int)list.TypeOfPieceInList]); //Sums values of first 6 lists (white pieces)
        int blackMaterialValue = pieceListList.Skip(6).Take(6).Sum(list => list.Count * pieceValues[(int)list.TypeOfPieceInList]); //Sums up next 6 lists (black pieces)

        int whiteMaterialAdvantage = whiteMaterialValue - blackMaterialValue;
        int blackMaterialAdvantage = blackMaterialValue - whiteMaterialValue;
        int materialAdvantage = botIsWhite ? whiteMaterialAdvantage: blackMaterialAdvantage;

        return materialAdvantage;
    }

    private int CalculatePositionalAdvantage(Board board) //Refactor this and material advantage due to reused code
    {
        return 0;
    }

    private int CalculateDevelopmentIncrease(Move move)
    {
        int startCentreness = CalculateCentredness(move.StartSquare);
        int targetCentreness = CalculateCentredness(move.TargetSquare);

        int initialDevelopment = startCentreness;
        int newDevelopment = targetCentreness;
        //Currently, development is only calculated based on how "central" the pieces are, it does not take the piece into account

        return newDevelopment - initialDevelopment;
    }

    private int CalculateCentredness(Square square)
    {
        int rankMiddleness = (int)(3.5 - Math.Abs(square.Rank - 3.5)); //Closeness to the middle in terms of ranks
        int fileMiddleness = (int)(3.5 - Math.Abs(square.File- 3.5)); //Closeness to middle in terms of files

        int Centreness = rankMiddleness * fileMiddleness; 
        /*Things closest to the centre have the highest centreness. Multiplying the values gives the highest difference in centreness when moving into the centre.
         This makes the calculations for development increase work better as the AI will choose to develop pieces into the centre as that gives the greatest development
         incresase. 
        Previously, using addition lead it to sometimes developing pieces on the edge of the board as moving two spaces forwards increases development by the same amount 
        as moving a piece in the middle to the centre*/
        //Issue? The AI will never develop pieces on the leftmost and rightmost side of the board, as the increase in development score is 0.
        
        return Centreness;
    }


}