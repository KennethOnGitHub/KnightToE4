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
    //could calculate a way that for each piece, their importance of development is increased / decreased over time => pawn has higher value that shrinks rapidly etc.


    //bool BotIsWhite = board.IsWhiteToMove; -> you need board to call this but thats not defined here - it would b rlly useful to have the colour of the player but eh i cant figure it out :3    

    public Move Think(Board board, Timer timer)
    {

        Move[] moves = board.GetLegalMoves();


        //take each piece and assign it an arbitary value to get to, based on colour, piece type, and position.
        //first check colour of piece (only have to check 1 for simplicity)
        //then, check type of piece, and give it the "line" to get to
        //add a value if closer to the middle, otherwise keep same
        //when it gets here, default to taking pieces with "non-centre" pieces


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
        double mostDevInc = 0; //this will store the increase

        foreach (Move move in moves)
        {
            double startCentreVal = Centreness(move.StartSquare);
            double targetCentreVal = Centreness(move.TargetSquare);

            int pieceVal = pieceValues[(int)(move.MovePieceType)]; //get the value of a piece from the defined pieceValues array 
            double pieceCurrentDev = pieceVal * startCentreVal; //gets the pieces current development score
            double pieceTargetDev = pieceVal * targetCentreVal; //gets the projected development score

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

    //im rewiritng this function and removing the weird bullshit bloat - legacy code
    /*
    private Move CentreObjetive(Move[] moves)
    {
        Move mostDevelopedMove = moves[0];

        Dictionary<Move,double> allPieceValues = new Dictionary<Move, double>(); //creates a dictionary where the key is of type Move and value is of type 

        //look at this fucking idiot -> USE 2D ARRAY SO THAT U CAN THEN USE ITERATOR VARIABLE TO SORT IT BY POINT VALUE, THEN DO THE LARGEST POINT MOVE :3

        //j is used first because i implemented this after and it kinda makes sense (it doesn't)
        for (int j = 0; j < moves.Length; j++) //iterats and appends each move to the array, with null for each value (is added later -> DO THIS ALL AT ONCE 
        {
            allPieceValues.Add(moves[j], 0);
        }

        foreach (Move move in moves)
        {
            //legacy formula: (3.5 - Math.Abs(move.TargetSquare.Rank - 3.5))
            double rankDevVal = -(move.TargetSquare.Rank)^2 + 7 * move.TargetSquare.Rank; //formula that gives "centre" ranks (rows) a higher value, and outside "ranks" a lower one (from 0 - 3)
            double fileDevVal = -(move.TargetSquare.File)^2 + 7 * move.TargetSquare.File; 
            //yea i dont think this worky

            double pieceVal = (rankDevVal * fileDevVal) * pieceValues[(int)move.MovePieceType];

            allPieceValues[move] = pieceVal; //adds value to the dict, at corresponding key 
            
            Console.WriteLine(pieceVal); //DEBUG -> REMOVE LATER

        }

        int k = 0;
        foreach(Move move in moves) //must be a seperate foreach, because the "allPieceValues" dict must have all its values before this logic is run
        {
            BubbleSort(moves, move, allPieceValues, k);
            k += 1;
        }

        mostDevelopedMove = allPieceValues[k]; //gives the highest scoring move here
        //"error this" "error that" SHUT THE FUCK UP VISUAL STUDIO IT WORKYY

        return mostDevelopedMove;
    }

    private Move BubbleSort(Move[] moves, Move move, Dictionary<Move, double> dict, int iterator)
    {
        Move nextMove = moves[iterator + 1];
        if (dict[nextMove] > dict[move])
        {
            //swapping values logic - we are sorting the array here
            double temp1 = dict[nextMove];
            dict[nextMove] = dict[move];
            dict[move] = temp1;
        }

        return move;
    }

}
    */