using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Security.AccessControl;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 50 }; //these are arbitary because like... the king shouldnt really be pushing and developing :D


    public Move Think(Board board, Timer timer)
    {

        Move[] moves = board.GetLegalMoves();

        //Move nextMove = RngMove(moves);
        //Move nextMove = ForwardestMove(board, moves);
        Move nextMove = CentreObjetive(moves);

        return nextMove;
    }

    private Move RngMove(Move[] moves)
    {
        Random r = new();
        Move nextMove = moves[r.Next(moves.Length)];

        return nextMove;
    }

    private Move ForwardestMove(Board board, Move[] moves)
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

    //sorry for yoinking yer code tree 
    private Move CentreObjetive(Move[] moves)
    {
        //previously i tried to store these as 'linked' variables, where they were explicitly tied, using a dict... This is bloat, and doing this reduces error
        Move mostDevMove = moves[0]; //this will store the move 
        double mostDevInc = 0;       //this will store the increase

        foreach (Move move in moves)
        {
            double startCentreVal = Centreness(move.StartSquare);
            double targetCentreVal = Centreness(move.TargetSquare);

            int pieceVal = pieceValues[(int)(move.MovePieceType)]; //get the value of a piece from the defined pieceValues array 
            
            //pieceVal is not actually nessesary here currently, but is placeholder for a better way to get devIncrease
            double pieceCurrentDev = pieceVal + startCentreVal;    //gets the pieces current development score
            double pieceTargetDev = pieceVal + targetCentreVal;    //gets the projected development score

            double devIncrease = pieceTargetDev - pieceCurrentDev;

            if (devIncrease > mostDevInc)
            {
                mostDevInc = devIncrease;
                mostDevMove = move;
            }
        }
        return mostDevMove;
    }

    private double Centreness(Square square) //formulas are subject to change
    {
        //possible quadratic = x^2  + 7x (gives a similar curve to other formula)
        //double rankVal = 3.5 - Math.Abs(square.Rank - 3.5);
        //double fileVal = 3.5 - Math.Abs(square.File - 3.5);
        double rankVal = (square.Rank)^2 + (7 * square.Rank);
        double fileVal = (square.File) ^ 2 + (7 * square.File);

        return rankVal * fileVal;
    }
}