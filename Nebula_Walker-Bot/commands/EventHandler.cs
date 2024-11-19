using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Nebula_Walker_Bot.database;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Nebula_Walker_Bot.commands
{
    public class EventHandler
    {
        //  Event message triggered task
        public async Task MessageTrigger(DiscordClient sender, MessageCreateEventArgs args)
        {
            // Verifies if the message is not made by a bot
            if (!args.Author.IsBot)
            {
                DiscordRole role = args.Guild.GetRole(1255943018272395365);
                DBengine db = new DBengine();

                ulong discordID = args.Author.Id;
                int userID = await db.GetUserID(discordID);

                // Verifies if the message is not a command for the bot
                if (!args.Message.Content.StartsWith("!"))
                {
                    // Verifies if the user is new
                    if (userID == 0)
                    {
                        // Creates a new user
                        DBuser user = new DBuser
                        {
                            ID = userID,
                            discordID = Convert.ToString(discordID),
                            quantMensagem = 0,
                            tempoCall = 0
                        };

                        // Uses the new user and log in the console
                        await db.StoreNewUserAsync(user);
                        Console.WriteLine($"{discordID} foi registrado no numero {userID}");
                    }
                    else
                    {
                        // Increment the user message total
                        bool incrementTest = await db.IncrementMensages(userID);
                        bool quotaCompleted = await isQuotaCompleted(userID, db);

                        // Log in the console
                        Console.Write("Mensagem de membro com Discord ID: " + discordID + ".");
                        Console.WriteLine("BD User ID: " + userID + ".");

                        // Verifies the user quota status.
                        if (quotaCompleted)
                        {
                            Console.WriteLine("Quota completed");
                            // Gives the role if the quota is completed
                            await args.Guild.GetMemberAsync(discordID).Result.GrantRoleAsync(role);
                        }
                        else
                        {
                            // Remove the role if the quota is not completed
                            await args.Guild.GetMemberAsync(discordID).Result.RevokeRoleAsync(role);
                        }

                        // Test if the increment has gone bad
                        if (!incrementTest)
                        {
                            throw new Exception("Problema no increment");
                        }
                    }
                }
            }
        }

        // Verifies the Quota Status
        public async Task<bool> isQuotaCompleted(int userID, DBengine db)
        {
            int totalMensages = await db.GetUserMensagesByID(userID);
            Console.WriteLine("Total mensages: " + totalMensages + ".");
            if ((Convert.ToDecimal(totalMensages) / 3000) >= 1)
            {
                return true;
            }

            return false;
        }
    }
}
