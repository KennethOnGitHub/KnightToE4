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

        if (captures.Length > 0) //if piece can be captured then it is (capture has highest importance)
        {
            return captures[0];
        }

        Move bestmove = moves[0];
        int bestforwardness = 0;
        int bestmiddleness = 0;
        foreach (Move move in moves)
        {
            int forwardness = move.TargetSquare.Rank; //Sets forwardness to Rank of piece. Rank is the row (from 0 to 7, starting from the bottom of the board)
            if (!BotIsWhite)
            {
                forwardness = 7 - forwardness; //reverse funtion if the bot is playing as black
            }
            int middleness = Math.Abs(7 - 2 * move.TargetSquare.File);

            if (forwardness > bestforwardness)
            {
                bestforwardness = forwardness;
                bestmiddleness = middleness;
                bestmove = move;
            }
            else if (middleness > bestmiddleness & forwardness == bestforwardness) {
                bestforwardness = forwardness;
                bestmiddleness = middleness;
                bestmove = move;
            }
            //above is doo doo fucking shit, terrible code but it is near bed time and I am gonna hon snoo snoo snoo

        }
        return bestmove;
    }
}