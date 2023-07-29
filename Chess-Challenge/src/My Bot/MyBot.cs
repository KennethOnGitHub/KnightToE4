using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

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
        int bestMoveAdvantage = int.MinValue;


        foreach (Move move in allmoves) //I hate this, this smells, refactor tmrw
        {
            board.MakeMove(move);
            int moveAdvantage = Evaluate(board, 0, int.MinValue, int.MaxValue, false);
            board.UndoMove(move);
            if (moveAdvantage > bestMoveAdvantage)
            {
                bestMoveAdvantage = moveAdvantage;
                bestmove = move;
            }
        }
        return bestmove;
    }

    private int Evaluate(Board board, int currentDepth, int alpha, int beta, bool ourTurn)
    {
        //ourturn can be calculated a number of ways, I just this one because I wanted to. But you could also look at if currentDepth even or odd?
        Move[] moves = board.GetLegalMoves();

        if (board.IsInCheckmate())
        {
            return ourTurn ? int.MinValue : int.MaxValue; //Returns the lowest value if we are checkmated and highest value if the enemy is mated
        }
        if (board.IsDraw())
        {
            return 0;
        }

        if (currentDepth == baseMaxDepth)
        {
            return CalculateAdvantage(board);
        }

        if (ourTurn)
        {
            int maxEval = int.MinValue;
            foreach (Move move in moves)
            {
                board.MakeMove(move);
                int evaluation = Evaluate(board, currentDepth + 1, alpha, beta, false);
                maxEval = Math.Max(maxEval, evaluation);
                alpha = Math.Max(alpha, evaluation); //Minor optimisation by using maxEval as the alpha?, same with minEval. Look into this further
                board.UndoMove(move);
                if (beta <= alpha)
                {
                    break;
                }
            }
            
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (Move move in moves)
            {
                board.MakeMove(move);
                int evaluation = Evaluate(board, currentDepth + 1, alpha, beta, true);
                minEval = Math.Min(minEval, evaluation);
                beta = Math.Min(beta, evaluation);
                board.UndoMove(move);
                if (beta <= alpha)
                {
                    break;
                }
            }
                        
            return minEval;
        }

        //This is alpha beta pruning, I can't explain it better than what's online anyways

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