using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();


        //rng 


        //take each piece and assign it an arbitary value to get to, based on colour, piece type, and position.
        //first check colour of piece (only have to check 1 for simplicity)
        //then, check type of piece, and give it the "line" to get to
        //add a value if closer to the middle, otherwise keep same
        //when it gets here, default to taking pieces with "non-centre" pieces

        //Move nextMove = RngMove(moves);
        Move nextMove = ForwardestMove(board, moves);

        return nextMove;

    }

    public Move RngMove(Move[] moves)
    {
        Random r = new();
        Move nextMove = moves[r.Next(moves.Length)];

        return nextMove;
    }
    public Move ForwardestMove(Board board, Move[] moves)
    {
        bool BotIsWhite = board.IsWhiteToMove;
        int bestforwardness = 0;
        Move forwardestMove = moves[0];

        foreach (Move move in moves)
        {
            int forwardness = move.TargetSquare.Rank;
            if (!BotIsWhite)
            {
                forwardness = 7 - forwardness; //if bot is playing White, then this is inverted so pieces move to the opposing side of the board
            }

            if (forwardness > bestforwardness)
            {
                bestforwardness = forwardness;
                forwardestMove = move;

            }
        }
        return forwardestMove;

    }
}