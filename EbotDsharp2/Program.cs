using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using DSharpPlus.Interactivity;

namespace EbotDSharp2
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;
        static VoiceNextClient voice;


        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
                
            });

            discord.UseInteractivity(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = TimeoutBehaviour.Ignore,

                // default pagination timeout to 5 minutes
                PaginationTimeout = TimeSpan.FromMinutes(5),

                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!",
                EnableDms = false
            });

            commands.RegisterCommands<EbotDsharp2.Commands>();


            voice = discord.UseVoiceNext(new VoiceNextConfiguration
            {

            });



            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
