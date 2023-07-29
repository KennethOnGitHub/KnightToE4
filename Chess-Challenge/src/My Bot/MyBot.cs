using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public class MyBot : IChessBot
{
    int baseMaxDepth = 4;
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };


    public Move Think(Board board, Timer timer)
    {
        bool botIsWhite = board.IsWhiteToMove;
        Move[] allmoves = board.GetLegalMoves();

        Move bestmove = allmoves[0];
        int bestMoveAdvantage = int.MinValue;

        PawnLineMaintained(board, botIsWhite);

        foreach (Move move in allmoves) //I hate this, this smells, refactor tmrw
        {
            board.MakeMove(move);
            int moveAdvantage = Evaluate(board, 0, int.MaxValue, int.MinValue, true);
            board.UndoMove(move);
            if (moveAdvantage > bestMoveAdvantage)
            {
                bestMoveAdvantage = moveAdvantage;
                bestmove = move;
            }
        }
        return bestmove;
    }

    private int Evaluate(Board board, int currentDepth, int alpha, int beta, bool ourTurn)
    {
        //ourturn can be calculated a number of ways, I just this one because I wanted to. But you could also look at if currentDepth even or odd?
        Move[] moves = board.GetLegalMoves();

        if (board.IsInCheckmate())
        {
            return ourTurn ? int.MinValue : int.MaxValue; //Returns the lowest value if we are checkmated and highest value if the enemy is mated
        }
        if (board.IsDraw())
        {
            return 0;
        }

        if (currentDepth == baseMaxDepth)
        {
            return CalculateAdvantage(board);
        }

        if (ourTurn)
        {
            int maxEval = int.MinValue;
            foreach (Move move in moves)
            {
                board.MakeMove(move);
                int evaluation = Evaluate(board, currentDepth + 1, alpha, beta, false);
                maxEval = Math.Max(maxEval, evaluation);
                alpha = Math.Max(alpha, evaluation); //Minor optimisation by using maxEval as the alpha?, same with minEval. Look into this further
                board.UndoMove(move);
                if (beta <= alpha)
                {
                    break;
                }
            }
            
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (Move move in moves)
            {
                board.MakeMove(move);
                int evaluation = Evaluate(board, currentDepth + 1, alpha, beta, true);
                minEval = Math.Min(minEval, evaluation);
                beta = Math.Min(beta, evaluation);
                board.UndoMove(move);
                if (beta <= alpha)
                {
                    break;
                }
            }
                        
            return minEval;
        }

        //This is alpha beta pruning, I can't explain it better than what's online anyways

    }
    private int CalculateAdvantage(Board board)
    {
        int materialAdvantage = CalculateMaterialAdvantage(board);
        int positionalAdvantage = CalculatePositionalAdvantage(board);
        int boardValue = materialAdvantage + positionalAdvantage;

        return boardValue;
    }

    private int CalculateMaterialAdvantage(Board board)
    {
        bool botIsWhite = board.IsWhiteToMove;
        PieceList[] pieceListList = board.GetAllPieceLists();
        int whiteMaterialValue = pieceListList.Take(6).Sum(list => list.Count * pieceValues[(int)list.TypeOfPieceInList]); //Sums values of first 6 lists (white pieces)
        int blackMaterialValue = pieceListList.Skip(6).Take(6).Sum(list => list.Count * pieceValues[(int)list.TypeOfPieceInList]); //Sums up next 6 lists (black pieces)

        int whiteMaterialAdvantage = whiteMaterialValue - blackMaterialValue;
        int blackMaterialAdvantage = blackMaterialValue - whiteMaterialValue;
        int materialAdvantage = botIsWhite ? whiteMaterialAdvantage: blackMaterialAdvantage;

        return materialAdvantage;
    }

    private int CalculatePositionalAdvantage(Board board) //Refactor this and material advantage due to reused code
    {

        return 0;
        
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


    private bool PawnLineMaintained(Board board, bool isWhite)
    {
        //if enough pawns are ahead(idk, say like 5), then dont bother checking PawnLineMaintained, and assume True.
        //calculate this when the whole board is checked for where pawns are.


        //draw a circle around each pawn and check if squares on either side contain a pawn.
        //if it does, then you are good, the pawn line here is maintained :D
        //if not, check the other squares.

        //after u get the basic shit going, you should check the whole board for pawns, evaluate if more pawns are ahead or behind, and move pieces based off of that.
        //if the pawn is ahead, move current pawn up to meet this.
        //if it is behind, then move that one up to this one.

        //right im fucking exhausted but like ill do this bit I SWEAR OKAY ILL DO IT TMMR >:(
        //id say that you could convert the bitBoard to a binary value but idk how to do this and my connections down =D





        //DONT FUCKING NEED THIS BECAUSE PIECElISTS EXIST AAAAARHHHR
         /*
        
        ulong pawnBitboard = board.GetPieceBitboard(PieceType.Pawn, isWhite);
        Console.WriteLine(pawnBitboard);
        //CAST HERE BUT LIKE IT WONT FUKCING WORK
        long pawnBitboardLong = unchecked((long)pawnBitboard);
        string pawnBitboardLongString = Convert.ToString((long)pawnBitboardLong, 2);
        pawnBitboardLongString = Regex.Replace(pawnBitboardLongString, ".{8}", "$0 \n"); //stolen regex from here: https://stackoverflow.com/questions/9932096/add-separator-to-string-at-every-n-characters
        Console.WriteLine(pawnBitboardLongString);
        
         */

        int pawnLeft = 0;
        int pawnRight = 0;

        //def a more efficient way to do this but... uwu
        foreach (Piece pawn in board.GetPieceList(PieceType.Pawn, isWhite)) //iterate through all the pawns 
        {
            if (pawn.Square.File > 5) //if pawns are developed then we guicci bbg -> ALSO IDK IF THE COLOUR THAT THE BOT IS PLAYING WILL AFFECT WHAT "SIDE" IT SPAWNS ON, SO MAY HAVE TO CHECK FOR THAT ALSO
            {
                return true;
            }

            //checks if the pawn is on the edge of the board 
            if (pawn.Square.Rank != 0)
            {
                pawnLeft = pawn.Square.Rank - 1;
            }

            if (pawn.Square.Rank != 7)
            {
                pawnRight = pawn.Square.Rank + 1;
            }
            else
                pawnLeft = 0;
                pawnRight = 7;
        
            if ((pawnLeft == pawn.Square.File - 1) || (pawnRight == pawn.Square.File + 1)) //currently always returns true, no me gusta nada ;-;
            {
                Console.WriteLine("YIP DE FUCKING YE"); 
                bool pawnLineMaintained = true;
            }

        }

        
        return true; //just for testing stuff

    }


    /*       //IGNORE THIS SHIT :D -> this will be used if pawns arent really developed by the bot (not even finished lmaooo)
    private int pawnDevelopment(Board board) 
    {
        int movesPlayed = board.PlyCount;


        if (movesPlayed > 10) //arbitary ass number for now, keep in mind that movesPlayed is total, not just ones played by the bot
        {
            PieceList[] listOfPawns = board.GetPieceList(PieceType.Pawn, botIsWhite);
            int pawnDevelopment = listOfPawns.Length() * pieceValues[1] * (-movesPlayed + 11); //the less moves played, the higher the need for pawns to develop forwards. Will need to change formula if movesPlayed is changed (y = -x+11)

            return pawnDevelopment;
        }
    }
    */


}