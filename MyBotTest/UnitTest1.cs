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
            string position = "nbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            var bot = new MyBot();
            var board = ChessChallenge.API.Board.CreateBoardFromFEN(position);

            int advantage = bot.CalculateMaterialAdvantage(board);

            Assert.AreEqual(0, advantage);

        }
    }
}