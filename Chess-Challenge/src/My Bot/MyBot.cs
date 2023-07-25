using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;


public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

    public Move Think(Board board, Timer timer)
    {
        bool isWhite = board.IsWhiteToMove;
        Move[] moves = board.GetLegalMoves();
        Move nextMove = CentreDevelopemntMove(moves);

        CalculateBoardAdvantage(board, isWhite);

        bool moveIsValuable = MoveValueCheck(board, nextMove);
        bool isMoveDraw = board.IsDraw();

        return nextMove;

        /* //need to add this in later, but is good for now ->MAY NEED TO REMOVE THIS LATER AS IT IS REDUNDANT (possibly, ill ask tree :3)
        if ((moveIsValuable && !isMoveDraw))
        {
            return nextMove;
        }
        */
    }

    private int CalculateBoardAdvantage(Board board, bool isWhite)
    {
        int boardAdvantage = CalculateMaterialAdvantage(board, isWhite);
        Console.WriteLine(boardAdvantage);
        //am adding more checks such as the "future checker" here in the future :>
        return boardAdvantage;
    }

    private int CalculateMaterialAdvantage(Board board, bool isWhite)
    {
        PieceList[] pieceListList = board.GetAllPieceLists();

        int whiteMaterialValue = pieceListList.Take(6).Sum(list => list.Count * pieceValues[(int)list.TypeOfPieceInList]); //Sums values of first 6 lists (white pieces)
        int blackMaterialValue = pieceListList.Skip(6).Take(6).Sum(list => list.Count * pieceValues[(int)list.TypeOfPieceInList]); //Sums up next 6 lists (black pieces)

        int whiteMaterialAdvantage = whiteMaterialValue - blackMaterialValue;
        int blakMaterialAdvantage = blackMaterialValue - whiteMaterialValue;
        int materialAdvantage = isWhite ? whiteMaterialAdvantage : blakMaterialAdvantage; //shorthand of if-else statement

        return materialAdvantage;
    }


    private Move CentreDevelopemntMove(Move[] moves)
    {
        Move mostDevelopedMove = moves[0];
        double mostDevelopedIncrease = 0;

        foreach (Move move in moves)
        {
            double startCentreVal = CalculateCentreness(move.StartSquare);
            double targetCentreVal = CalculateCentreness(move.TargetSquare);

            int pieceValue = pieceValues[(int)move.CapturePieceType];

            double pieceCurrentDevelopment = DevelopmentIncrease(pieceValue, startCentreVal);
            double pieceTargetDevelopment = DevelopmentIncrease(pieceValue, targetCentreVal);

            double devIncrease = pieceTargetDevelopment - pieceCurrentDevelopment;

            if (devIncrease > mostDevelopedIncrease)
            {
                mostDevelopedIncrease = devIncrease;
                mostDevelopedMove = move;
            }
        }
        return mostDevelopedMove;
    }

    private double DevelopmentIncrease(int pieceVal, double centreVal) //refactor to take in more variables at a later point 
    {
        return pieceVal + centreVal;
    }


    private bool MoveValueCheck(Board board, Move move)
    {
        Square targetSquare = move.TargetSquare;
        bool isAttacked = board.SquareIsAttackedByOpponent(targetSquare);
        
        int pieceVal = pieceValues[(int)move.MovePieceType];
        int targetPieceVal = pieceValues[(int)move.CapturePieceType];

        if (!isAttacked)
        {
            return true; //if player isnt attacked, then the move is most likely "worth it" (for now)
        }
        else if (targetPieceVal > pieceVal)
        {
            return true;
        }
        return false;
    }
    
    private double CalculateCentreness(Square square)
    {
        double rankVal = 3.5 - Math.Abs(square.Rank - 3.5);
        double fileVal = 3.5 - Math.Abs(square.File - 3.5);

        return rankVal * fileVal;
    }





}