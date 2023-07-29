using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        return moves[0];
    }


    private bool PawnLineMaintained(Board board, bool isWhite)
    {

        //NOT FINISHED BUT THE IDEA IS THERE, I WOULD SUGGEST FINDING A BETTER WAY TO STORE AND FIND THE "PAWN CIRCLE", OVER INTS AND IF STATEMENTS (NOT EFFCICIENT AT ALL)

        //if enough pawns are ahead(idk, say like 5), then dont bother checking PawnLineMaintained, and assume True.                                                              ✓ (kinda)
        //calculate this when the whole board is checked for where pawns are.                                                                                                     


        //draw a circle around each pawn and check if squares on either side contain a pawn.                                                                                      ✓
        //if it does, then you are good, the pawn line here is maintained :D                                                                                                      ✓

        //HERE  NOW:

        //after u get the basic shit going, you should check the whole board for pawns, evaluate if more pawns are ahead or behind, and move pieces based off of that.
        //if the pawn is ahead, move current pawn up to meet this.
        //if it is behind, then move that one up to this one.



        //DIAGRAM TIMEEE:
        // aight so middle is pawn right. So far, the left and right value are calculated.
        // to get diagonals you can +/-1 to both file and rank. TopLeft = -1Rank +1file, TopRight = +1R +1 F, BottomLeft = -1R, -1F, BottomRight = +1R, -1F
        // to get the direct aboves of the pawn, just do the same as left and right, but with File instead of Rank
        //
        //       [][][]
        //       []()[]
        //       [][][]
        //binky bonk bonky bink, 
        //i officially declare thee "twink"

        //bonki bink binky bonk
        //over 25? then declared "twonk"

        int pawnLeft = 0;
        int pawnRight = 0;

        /*
        int pawnUp = 0;
        int pawnDown = 0;

        int pawnTopLeft = 0;
        int pawnTopRight = 0;

        int pawnBottomLeft = 0;
        int pawnBottomRight = 0;
        */

        int pawnDevelopment = 0;
        int pawnLineMaintain = 0; //stinky variable name but ill make do

        //def a more efficient way to do this but... uwu
        foreach (Piece pawn in board.GetPieceList(PieceType.Pawn, isWhite)) //iterate through all the pawns 
        {
            if (pawn.Square.File > 5) //if pawns are developed then we guicci bbg -> ALSO IDK IF THE COLOUR THAT THE BOT IS PLAYING WILL AFFECT WHAT "SIDE" IT SPAWNS ON, SO MAY HAVE TO CHECK FOR THAT ALSO
            {
                pawnDevelopment++;
            }

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

            if ((pawnLeft == pawn.Square.File - 1) || (pawnRight == pawn.Square.File + 1))
            {
                pawnLineMaintain++;
            }
        }

        //quick debugging stuff for now
        if (pawnDevelopment >= 5)
        {
            Console.WriteLine("pawnLine Developed!");
        }

        if (pawnLineMaintain >= 8)
        {
            Console.WriteLine("YIP DE FUCKING YE");
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