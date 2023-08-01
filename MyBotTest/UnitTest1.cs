using ChessChallenge;
using ChessChallenge.API;
using ChessChallenge.Application;
using ChessChallenge.Chess;

namespace MyBotTest
{
    [TestClass]
    public class EvaluateTestss
    {
        [TestMethod]
        public void CalculateMaterialAdvantage_Position1_0()
        {
            string position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            var bot = new MyBot();
            var board = ChessChallenge.API.Board.CreateBoardFromFEN(position);

            int advantage = bot.CalculateMaterialAdvantageOfCurrentPlayer(board);

            Assert.AreEqual(0, advantage);

        }

        [TestMethod]
        public void CalculateMaterialAdvantage_Position2_neg2100()
        {
            string position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RNB1K3 w Qkq - 0 1";
            var bot = new MyBot();
            var board = ChessChallenge.API.Board.CreateBoardFromFEN(position);

            int advantage = bot.CalculateMaterialAdvantageOfCurrentPlayer(board);

            Assert.AreEqual(-2100, advantage);//botiswhite messes with this I think?
        }
    }
}