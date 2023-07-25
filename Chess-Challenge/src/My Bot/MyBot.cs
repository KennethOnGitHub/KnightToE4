using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Security.AccessControl;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 }; //these are arbitary because like... the king shouldnt really be pushing and developing :D


    public Move Think(Board board, Timer timer)
    {

        Move[] moves = board.GetLegalMoves();

        //Move nextMove = RngMove(moves);
        //Move nextMove = ForwardestMove(board, moves);
        //Move nextMove = CentreObjetive(moves);

        //figure out the move using CentreObjective
        //evauluate if this will place the piece in "danger", by seeing if it taking a piece will also get it taken (it is okay to do this if it "gains" value by taking)

        Move nextMove = CentreObjetive(moves); //here is the "bottleneck", dont focus on MoveValueCheck, but make a better inital moveCheck
        bool moveIsValuable = MoveValueCheck(nextMove, board); //Check above comment! only problem is that itll still hang pieces that are like... more than 1 move ahead. 

        if (moveIsValuable) 
        {
            return nextMove;
        }
        
        nextMove = RngMove(moves); //placeholder for a function that will be more conservative in checking for a "neutral" move (or just a better one?)
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

            //do an evaluation of the current piece, minus the other pieces value, if the target square is being attacked
            //int pieceVal = pieceValues[(int)(move.MovePieceType)];
            int pieceVal = pieceValues[(int)(move.CapturePieceType)];

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

    private bool MoveValueCheck(Move move, Board board) //checks if target square is being attacked, and weighs up value of move
    {

        Square targetSquare = move.TargetSquare;
        bool isAttacked = board.SquareIsAttackedByOpponent(targetSquare);
        int pieceVal = pieceValues[(int)(move.MovePieceType)]; //"players" piece
        int targetPieceVal = pieceValues[(int)(move.CapturePieceType)]; //targeted piece

        if ((isAttacked) && (targetPieceVal >= pieceVal)) //might want to change the ">=" depending on the agression of the bot, or get it to factor in the state of the board (move many pieces etc.)
        {
            return true;
        }
        return false;

    }

    private double Centreness(Square square) //formulas are subject to change
    {
        //formula found by tree that is good (never ever trust me to make those things jesus)
        double rankVal = 3.5 - Math.Abs(square.Rank - 3.5);
        double fileVal = 3.5 - Math.Abs(square.File - 3.5);

        return rankVal * fileVal;
    }
}