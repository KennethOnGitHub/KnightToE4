using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        bool BotIsWhite = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();
        Move[] captures = board.GetLegalMoves(true);
        Random rnd = new Random();

        if (captures.Length > 0)
        {
            return captures[0];
        }

        Move forwardestmove = moves[0];
        int bestforwardness = 0;
        foreach (Move move in moves)
        {
            int forwardness = move.TargetSquare.Rank;
            if (!BotIsWhite)
            {
                forwardness = 7 - forwardness;
            }

            if (forwardness > bestforwardness)
            {
                bestforwardness = forwardness;
                forwardestmove = move;

            }
        }
        return forwardestmove;
    }
}