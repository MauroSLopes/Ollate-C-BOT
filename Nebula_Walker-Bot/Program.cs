using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Nebula_Walker_Bot.commands;
using System;
using System.Threading.Tasks;

namespace Nebula_Walker_Bot
{
    public class Program
    {
        public static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands {  get; set; }

        static async Task Main(string[] args) 
        {
            commands.EventHandler eventHandler = new commands.EventHandler();

            JSONReader JsonData = new JSONReader();
            await JsonData.ReadJSON();

            DiscordConfiguration discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = JsonData.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(discordConfig);


            Client.MessageCreated += eventHandler.MessageTrigger;
            Client.Ready += isReady;

            CommandsNextConfiguration CommandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[]
                {
                    JsonData.prefix
                },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(CommandsConfig);

            Commands.RegisterCommands<TestCmd>();
            Commands.RegisterCommands<Profile>();

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromMinutes(2)
            });

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
                
        private static Task isReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
