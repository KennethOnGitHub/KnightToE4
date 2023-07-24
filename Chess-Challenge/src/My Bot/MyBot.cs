using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10000 };
    public Move Think(Board board, Timer timer)
    {
        bool BotIsWhite = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();

        Move bestmove = moves[0];

        double bestDevelopmentIncrease = 0;
        foreach (Move move in moves)
        {
            int startCentreness = CalculateCentredness(move.StartSquare);
            int targetCentreness = CalculateCentredness(move.TargetSquare);

            int pieceVal = pieceValues[(int)(move.MovePieceType)];
            //int pieceDevelopment = pieceVal * startCentreness;
            //int newDevelopment = pieceVal * targetCentreness;
            int initialDevelopment = startCentreness;
            int newDevelopment = targetCentreness;

            int developmentIncrease = newDevelopment - initialDevelopment;

            if (developmentIncrease > bestDevelopmentIncrease)
            {
                bestmove = move;
                bestDevelopmentIncrease = newDevelopment;
            }

        }
        Console.WriteLine(bestDevelopmentIncrease);
        return bestmove;
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