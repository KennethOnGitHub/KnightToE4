using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10000 };
    public Move Think(Board board, Timer timer)
    {
        bool BotIsWhite = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();
        Move[] captures = board.GetLegalMoves(true);
        Random rnd = new Random();

        /*
        if (captures.Length > 0) //if piece can be captured then it is (capture has highest importance)
        {
            return captures[0];
        }
        */

        Move bestmove = moves[0];
        double bestDevelopmentIncrease = 0;
        foreach (Move move in moves)
        {
            int startCentreness = CalculateCentredness(board, move.StartSquare);
            int targetCentreness = CalculateCentredness(board, move.TargetSquare);

            int pieceVal = pieceValues[(int)(move.MovePieceType)];
            int pieceDevelopment = pieceVal * startCentreness;
            int newDevelopment = pieceVal * targetCentreness;

            int developmentIncrease = newDevelopment - pieceDevelopment;

            if (developmentIncrease > bestDevelopmentIncrease)
            {
                bestmove = move;
                bestDevelopmentIncrease = newDevelopment;
            }

        }
        return bestmove;
    }

    private int CalculateCentredness(Board board, Square square)
    {
        int rankMiddleness = (int)(3.5 - Math.Abs(square.Rank - 3.5));
        int fileMiddleness = (int)(3.5 - Math.Abs(square.File- 3.5));

        return rankMiddleness + fileMiddleness;
    }


}