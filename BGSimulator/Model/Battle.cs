using System;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Battle
    {
        public int Round { get; set; }
        public Player PlayerA { get; set; }
        public Player PlayerB { get; set; }
        public Board PlayerABoard { get; set; }
        public Board PlayerBBoard { get; set; }

        public void Start()
        {
            var attacker = FirstAttacker();
            var defender = attacker.CurrentMatch;

            //clone because we want to save the state at the start of the fight.
            var attackerBoard = attacker.Board.Clone();
            var defenderBoard = defender.Board.Clone();

            while (!attackerBoard.IsEmpty && !defenderBoard.IsEmpty)
            {
                PrintBoardState(attackerBoard, defenderBoard);

                attackerBoard.Attack(defenderBoard);

                var temp = attackerBoard;
                attackerBoard = defenderBoard;
                defenderBoard = temp;
            }
        }

        private static void PrintBoardState(Board attackerBoard, Board defenderBoard)
        {
            Console.WriteLine();
            Console.WriteLine("{0} Board", attackerBoard.Player.Name);
            Console.WriteLine(string.Format(string.Join((" | "), attackerBoard.PlayedMinions)));
            Console.WriteLine("{0} Board", defenderBoard.Player.Name);
            Console.WriteLine(string.Format(string.Join((" | "), defenderBoard.PlayedMinions)));
            Console.WriteLine();
        }

        private Player FirstAttacker()
        {
            if (PlayerABoard.PlayedMinions.Count > PlayerBBoard.PlayedMinions.Count)
            {
                return PlayerA;
            }
            else if (PlayerABoard.PlayedMinions.Count < PlayerBBoard.PlayedMinions.Count)
            {
                return PlayerB;
            }
            else
            {
                return FlipCoin();
            }
        }

        private Player FlipCoin()
        {
            return RandomNumber(0, 100) < 50 ? PlayerA : PlayerB;
        }
    }
}