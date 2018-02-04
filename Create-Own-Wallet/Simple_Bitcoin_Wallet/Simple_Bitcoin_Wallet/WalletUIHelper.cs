using System.Collections.Generic;

namespace Simple_Bitcoin_Wallet
{
    public static class WalletUIHelper
    {
        public static IDictionary<string, WalletCommandsEnum> WalletCommands =
            new Dictionary<string, WalletCommandsEnum>()
            {
                { "recover", WalletCommandsEnum.Recover },
                { "create", WalletCommandsEnum.Create },
                { "balance", WalletCommandsEnum.Balance },
                { "history", WalletCommandsEnum.History },
                { "receive", WalletCommandsEnum.Receive },
                { "send", WalletCommandsEnum.Send },
                { "exit", WalletCommandsEnum.Exit }
            };
    }
}
