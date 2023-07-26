using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ExceptionServices;

public class MyBot : IChessBot
{
    int baseMaxDepth = 4;
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    bool botIsWhite;

    public Move Think(Board board, Timer timer)
    {
        botIsWhite = board.IsWhiteToMove;

          
        /*
        Move[] moves = board.GetLegalMoves();
        int initialAdvantage = CalculateAdvantage(board);

        Move bestmove = moves[0];
        int bestMoveValue = 0;
        foreach (Move move in moves)
        {
            //The captured value is calculated here as that   
            Piece capturedPiece = board.GetPiece(move.TargetSquare); 
            int materialIncrease = pieceValues[(int)capturedPiece.PieceType];

            int newAdvantage = CalculateAdvantage(board);  

            int moveValue = newAdvantage - initialAdvantage;

            if (moveValue > bestMoveValue)
            {
                bestMoveValue = moveValue;
                bestmove = move;
            }

        }
        Console.WriteLine(bestMoveValue);
        return bestmove;
        */
    }

    private int Evaluate(Board board, int currentDepth)
    {
        bool ourTurn = botIsWhite == board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();
        int[] boardValues = new int[moves.Length];

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

        return ourTurn ? boardValues.Max() : boardValues.Min();
        //return highest value in boardvalues if it is our turn, return lowest if it is enemy turn

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