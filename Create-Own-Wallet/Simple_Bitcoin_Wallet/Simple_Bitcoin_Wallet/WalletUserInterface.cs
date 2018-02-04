using System;

namespace Simple_Bitcoin_Wallet
{
    public static class WalletUserInterface
    {
        public static void ProcessUserCommands(string userCommand)
        {
            switch (WalletUIHelper.WalletCommands[userCommand])
            {
                case WalletCommandsEnum.Create:
                    ProcessCreate();
                    break;
                case WalletCommandsEnum.Recover:
                    ProcessRecover();
                    break;
                case WalletCommandsEnum.Receive:
                    ProcessReceive();
                    break;
                case WalletCommandsEnum.Balance:
                    ProcessBalance();
                    break;
                case WalletCommandsEnum.History:
                    ProcessHistory();
                    break;
                case WalletCommandsEnum.Send:
                    ProcessSend();
                    break;
            }
        }

        private static void ProcessCreate()
        {
            WalletLogic.CreateWallet();
        }

        private static void ProcessReceive()
        {
            Console.Write("Enter wallet's name: ");
            var walletName = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();
            WalletLogic.Receive(password, walletName);
        }

        private static void ProcessSend()
        {
            Console.Write("Enter wallet's name: ");
            var walletName = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();
            Console.Write("Enter wallet address: ");
            var wallet = Console.ReadLine();
            Console.Write("Select outpoint (transaction ID): ");
            var outpoint = Console.ReadLine();
            WalletLogic.Send(password, walletName, wallet, outpoint);
        }

        private static void ProcessHistory()
        {
            Console.Write("Enter wallet's name: ");
            var walletName = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();
            Console.Write("Enter wallet address: ");
            var wallet = Console.ReadLine();
            WalletLogic.ShowHistory(password, walletName, wallet);
        }

        private static void ProcessRecover()
        {
            Console.WriteLine("Please note the wallet cannot check if your password is correct or not. " +
                              "If you provde a wrong password a wallet will be recovered with your " +
                              "provided mnemonic AND password pair: ");
            Console.Write("Enter password: ");
            var password = Console.ReadLine();
            Console.Write("Enter mnemonic phrase: ");
            var mnemonicStr = Console.ReadLine();
            Console.WriteLine("Enter date (yyyy-MM-dd): ");
            var date = Console.ReadLine();
            WalletLogic.RecoverWallet(password, mnemonicStr, date);
        }

        private static void ProcessBalance()
        {
            Console.Write("Enter wallet's name: ");
            var walletName = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();
            Console.Write("Enter wallet address: ");
            var wallet = Console.ReadLine();
            WalletLogic.ShowBalance(password, walletName, wallet);
        }
    }
}
