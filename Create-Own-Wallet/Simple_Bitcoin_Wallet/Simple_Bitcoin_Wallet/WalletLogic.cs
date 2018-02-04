using System;
using System.Globalization;
using System.Text;
using HBitcoin.KeyManagement;
using NBitcoin;
using QBitNinja.Client;

namespace Simple_Bitcoin_Wallet
{
    public class WalletLogic
    {
        private static readonly string WalletFilePath = @"Wallets\";
        private static readonly Network CurrentNetwork = Network.TestNet;

        public static void Send(string password, string walletName, string wallet, string outpoint)
        {
            BitcoinExtKey privateKey = null;
            var loadedSafe = TryLoadWallet(password, walletName);

            if (loadedSafe != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (loadedSafe.GetAddress(i).ToString() == wallet)
                    {
                        Console.Write("Enter private key: ");
                        privateKey = new BitcoinExtKey(Console.ReadLine());
                        if (!privateKey.Equals(loadedSafe.FindPrivateKey(loadedSafe.GetAddress(i))))
                        {
                            Console.WriteLine("Wrong private key!");
                            return;
                        }

                        break;
                    }
                }

                var client = new QBitNinjaClient(Network.TestNet);
                var balance = client.GetBalance(BitcoinAddress.Create(wallet), false).Result;
                OutPoint outpointToSpend = null;
                foreach (var operation in balance.Operations)
                {
                    foreach (var coins in operation.ReceivedCoins)
                    {
                        if (coins.Outpoint.ToString().Substring(0, coins.Outpoint.ToString().Length - 2) == outpoint)
                        {
                            outpointToSpend = coins.Outpoint;
                            break;
                        }
                    }
                }

                var transaction = new Transaction();
                transaction.Inputs.Add(new TxIn()
                {
                    PrevOut = outpointToSpend
                });

                Console.Write("Enter address to send to: ");
                var addressToSendto = Console.ReadLine();
                var hallOfTheMakersAddress = BitcoinAddress.Create(addressToSendto);

                Console.Write("Enter amount to send: ");
                var amountToSend = decimal.Parse(Console.ReadLine());
                var hallOfTheMakersTxOut = new TxOut()
                {
                    Value = new Money(amountToSend, MoneyUnit.BTC),
                    ScriptPubKey = hallOfTheMakersAddress.ScriptPubKey
                };

                Console.Write("Enter amount to get back: ");
                var amountToGetBack = decimal.Parse(Console.ReadLine());
                var changeBackTxOut = new TxOut()
                {
                    Value = new Money(amountToGetBack, MoneyUnit.BTC),
                    ScriptPubKey = privateKey.ScriptPubKey
                };

                transaction.Outputs.Add(hallOfTheMakersTxOut);
                transaction.Outputs.Add(changeBackTxOut);

                Console.Write("Enter message: ");
                var message = Console.ReadLine();
                var bytes = Encoding.UTF8.GetBytes(message);
                transaction.Outputs.Add(new TxOut()
                {
                    Value = Money.Zero,
                    ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
                });

                transaction.Inputs[0].ScriptSig = privateKey.ScriptPubKey;
                transaction.Sign(privateKey, false);

                var broadcastResponse = client.Broadcast(transaction).Result;
                Console.WriteLine(broadcastResponse.Success ? "Transaction sent!" : "Something went wrong!");
            }
        }

        public static void ShowHistory(string password, string walletName, string wallet)
        {
            var loadedSafe = TryLoadWallet(password, walletName);

            if (loadedSafe != null)
            {
                var client = new QBitNinjaClient(Network.TestNet);
                var coinsReceived = client.GetBalance(BitcoinAddress.Create(wallet), true).Result;
                var header = "------- COINS RECEIVED -------";
                Console.WriteLine(header);
                foreach (var operation in coinsReceived.Operations)
                {
                    foreach (var coins in operation.ReceivedCoins)
                    {
                        var amount = (Money)coins.Amount;
                        Console.WriteLine($"Transaction ID: {coins.Outpoint}; Received coins: {amount.ToDecimal(MoneyUnit.BTC)}");
                    }
                }

                Console.WriteLine(new string('-', header.Length));

                var coinsSpent = client.GetBalance(BitcoinAddress.Create(wallet), false).Result;
                string footer = "------- COINS SPENT --------";
                Console.WriteLine(footer);
                foreach (var operation in coinsSpent.Operations)
                {
                    foreach (var coins in operation.SpentCoins)
                    {
                        var amount = (Money)coins.Amount;
                        Console.WriteLine($"Transaction ID: {coins.Outpoint}; Spent coins: {amount.ToDecimal(MoneyUnit.BTC)}");
                    }
                }

                Console.WriteLine(new string('-', footer.Length));
            }
        }

        public static void ShowBalance(string password, string walletName, string wallet)
        {
            var loadedSafe = TryLoadWallet(password, walletName);

            if (loadedSafe != null)
            {
                var client = new QBitNinjaClient(Network.TestNet);
                decimal totalBalance = 0;
                var balance = client.GetBalance(BitcoinAddress.Create(wallet), true).Result;
                foreach (var operation in balance.Operations)
                {
                    foreach (var coins in operation.ReceivedCoins)
                    {
                        var amount = (Money)coins.Amount;
                        decimal currentAmount = amount.ToDecimal(MoneyUnit.BTC);
                        totalBalance += currentAmount;
                    }
                }

                Console.WriteLine($"Balance of wallet: {totalBalance}");
            }
        }

        public static void Receive(string password, string walletName)
        {
            var loadedSafe = TryLoadWallet(password, walletName);

            if (loadedSafe != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(loadedSafe.GetAddress(i));
                }
            }
        }

        public static void RecoverWallet(string password, string mnemonicStr, string date)
        {
            var mnemonic = new Mnemonic(mnemonicStr);
            var random = new Random();
            Safe.Recover(mnemonic, password,
                WalletFilePath + "RecoveredWalletNum" + random.Next() + ".json",
                CurrentNetwork,
                creationTime: DateTimeOffset.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture));

            Console.WriteLine("Wallet successfully recovered");
        }

        public static void CreateWallet()
        {
            string password;
            string passwordConfirmed;

            do
            {
                Console.Write("Enter password: ");
                password = Console.ReadLine();
                Console.Write("Confirm password: ");
                passwordConfirmed = Console.ReadLine();

                if (password != passwordConfirmed)
                {
                    Console.WriteLine("Passwords did not match!");
                    Console.WriteLine("Try again.");
                }
            } while (password != passwordConfirmed);

            var failure = true;
            while (failure)
            {
                try
                {
                    Console.Write("Enter wallet name: ");
                    var walletName = Console.ReadLine();
                    var safe = Safe.Create(out var mnemonic, password, WalletFilePath + walletName + ".json",
                        CurrentNetwork);
                    Console.WriteLine("Wallet created successfully");
                    Console.WriteLine("Write down the following mnemonic words.");
                    Console.WriteLine("----------------");
                    Console.WriteLine(mnemonic);
                    Console.WriteLine("----------------");
                    Console.WriteLine("With the mnemonic words AND the password you can recover this wallet.");
                    Console.WriteLine("Write down and keep in SECURE place your private keys. Only through them you can access your coins!");
                    for (var i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"Address: {safe.GetAddress(i)} -> Private key: {safe.FindPrivateKey(safe.GetAddress(i))}");
                    }

                    failure = false;
                }
                catch
                {
                    Console.WriteLine("Wallet already exists");
                }
            }
        }

        private static Safe TryLoadWallet(string password, string walletName)
        {
            Safe loadedSafe = null;
            try
            {
                loadedSafe = Safe.Load(password, WalletFilePath + walletName + ".json");
            }
            catch (Exception)
            {
                Console.WriteLine("Wallet with such name does not exist!");
            }

            return loadedSafe;
        }
    }
}
