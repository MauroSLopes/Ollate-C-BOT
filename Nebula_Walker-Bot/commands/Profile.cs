using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Nebula_Walker_Bot.database;
using System;
using System.Threading.Tasks;

namespace Nebula_Walker_Bot.commands
{
    public class Profile : BaseCommandModule
    {
        DBengine db = new DBengine();

        // Creates a embed message with the user information
        [Command("profile")]
        public async Task ProfileCommand(CommandContext context)
        {
            DBuser user = await db.GetProfileInfo(context.Member.Id);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Description = $"# Perfil de {context.Member.Username}." +
                $"\n### Membro ativo informações." +
                $"\n Mensagens: {user.quantMensagem} / 3000 mensagens." +
                $"\n Tempo de Call: {user.tempoCall} / 20 horas",
                Color = context.Member.Color,
                Timestamp = DateTime.Now,
            };

            embed.WithAuthor(context.User.Username, null, context.User.AvatarUrl);
            embed.WithThumbnail(context.Guild.IconUrl);

            await context.RespondAsync(embed.Build());
        }
    }
}
