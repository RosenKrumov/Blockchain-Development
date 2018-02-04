using System;
using System.Collections;

namespace Simple_Bitcoin_Wallet
{
    class Wallet
    {
        static void Main()
        {
            string input = string.Empty;

            while (input != null && !input.ToLower().Equals("exit"))
            {
                do
                {
                    Console.Write("Enter operation [\"Create\", \"Recover\", \"Balance\", \"History\", \"Receive\", \"Send\", \"Exit\"]: ");
                    input = Console.ReadLine()?.ToLower().Trim();
                } while (!WalletUIHelper.WalletCommands.Keys.Contains(input));

                WalletUserInterface.ProcessUserCommands(input);
            }
        }
    }
}
