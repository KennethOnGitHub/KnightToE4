using ChessChallenge.API;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

public class MyBot : IChessBot
{
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    public Move Think(Board board, Timer timer)
    {
        bool BotIsWhite = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();

        Move bestmove = moves[0];
        int bestMoveValue = 0;
        foreach (Move move in moves)
        {
            Piece capturedPiece = board.GetPiece(move.TargetSquare);
            int captureValue = pieceValues[(int)capturedPiece.PieceType];
            int moveValue = captureValue + CalculateDevelopmentIncrease(move);
            if (moveValue > bestMoveValue)
            {
                bestMoveValue = moveValue;
                bestmove = move;
            }

        }
        Console.WriteLine(bestMoveValue);
        return bestmove;
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