using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Nebula_Walker_Bot.database;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Nebula_Walker_Bot.commands
{
    public class TestCmd : BaseCommandModule
    {
        DBengine db = new DBengine();

        // Template command
        [Command("ping")]
        public async Task PingPong(CommandContext commandCtx)
        {
            await commandCtx.Message.RespondAsync("Pong");
        }

        // Changes a user total menssages using the Discord ID
        [Command("changeDB")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task ChangeUserMensages(CommandContext cmdCtx, int totalMensagens, ulong discordID)
        {
            await db.changeTotalMensages(totalMensagens, discordID);
            await cmdCtx.Channel.SendMessageAsync("Done.");
        }

        // Resets a user using the Discord ID
        [Command("resetUser")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task ResetUser(CommandContext cmdCtx, ulong discordID)
        {
            await db.ResetUser(discordID);

            await cmdCtx.Channel.SendMessageAsync("Updated User");
        }

        // Resets all message and call time in the database
        [Command("resetAll")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task ResetAll(CommandContext cmdCtx)
        {
            await db.ResetAll();

            await cmdCtx.Channel.SendMessageAsync("Erased all");
        }


        // Embed command WIP
        [Command("relatar")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task EmbedMessage(CommandContext cmdCtx, string title, [RemainingText] string enteredDescription)
        {
            await cmdCtx.Message.DeleteAsync();

            enteredDescription = enteredDescription.Replace("\n", "\n### ");

            DiscordEmbedBuilder embeds = new DiscordEmbedBuilder()
            {
                Description = $"# {title}\n### {enteredDescription}",
                Color = cmdCtx.Member.Color,
                Timestamp = DateTime.Now,
            };
            embeds.WithAuthor(cmdCtx.User.Username, null ,cmdCtx.User.AvatarUrl);
            embeds.WithThumbnail(cmdCtx.Guild.IconUrl);


            await cmdCtx.Channel.SendMessageAsync("", embeds.Build());
        }
    }
}
