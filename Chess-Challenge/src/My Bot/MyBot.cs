using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    bool BotIsWhite = board.IsWhiteToMove;

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

    private Move CentreObjetive(Move[] moves)
    {

        Dictionary<Move,int> allPieceValues = new Dictionary<Move, int>(); //creates a dictionary where the key is of type Move and value is of type 

        //look at this fucking idiot -> USE 2D ARRAY SO THAT U CAN THEN USE ITERATOR VARIABLE TO SORT IT BY POINT VALUE, THEN DO THE LARGEST POINT MOVE :3

        //j is used first because i implemented this after and it kinda makes sense (it doesn't)
        for (int j = 0; j < moves.Length; j++) //iterats and appends each move to the array, with null for each value (is added later -> DO THIS ALL AT ONCE 
        {
            allPieceValues.Add(moves[j], 0);
        }

        int i = 0;
        foreach (Move move in moves)
        {
            
            Move mostDevelopedMove = moves[0];

            double developmentVal = 3.5 - Math.Abs(move.TargetSquare.Rank - 3.5); //formula that gives "centre" ranks (rows) a higher value, and outside "ranks" a lower one (from 0 - 3)
            double PieceVal = developmentVal * pieceValues[(int)move.MovePieceType];

            allPieceValues(i) = PieceVal; //adds value to the dict, at corresponding key 
            
            Console.WriteLine(PieceVal); //DEBUG -> REMOVE LATER

            i += 1;
        }

        foreach(Move move in moves) //must be a seperate foreach, because the "allPieceValues" dict must have all its values before this logic is run
        {
            //use bubblesort dumbass (will add later if this fucking works :£)
            if (allPieceValues[i + 1] > allPieceValues[move]) //FIGURE OUT HOW TO GET "move + 1" then ur done :£
            {
                //swapping values logic - we are sorting the array here
                int temp1 = allPieceValues[i + 1];
                allPieceValues(i + 1) = allPieceValues[move];
                allPieceValues(move) = temp1;
            }
        }

        mostDevelopedMove = allPieceValues[-1]; //gives the highest scoring move here
        return mostDevelopedMove;
    }

}