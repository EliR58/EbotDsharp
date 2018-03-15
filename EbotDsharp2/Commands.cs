using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Linq;

namespace EbotDsharp2
{
    class Commands
    {
        public Random rng = new Random();

        [Command("user"), Description("Provides info basic info about the user who invokes the command.")]
        public async Task User(CommandContext context)
        {
            await context.TriggerTypingAsync();
            await context.RespondAsync($"{context.Member.DisplayName}, {context.Guild.Name}, {context.Channel.Name}");
        }

        [Command("random"), Aliases("rand"), Description("Generates a random number between the two provided integers.")]
        public async Task Random(CommandContext context, int min, int max)
        {
            int x;
            if (min > max)
            {
                x = min;
                min = max;
                max = x;
            }
            await context.TriggerTypingAsync();
            var rnd = new Random();
            await context.RespondAsync($"Your random number is: {rnd.Next(min, max + 1)}");
        }

        [Command("pepe"), Aliases("feelsbadman"), Description("Posts a pepe meme")]
        public async Task Pepe(CommandContext context)
        {
            await context.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Title = "Pepe",
                ImageUrl = "http://i.imgur.com/44SoSqS.jpg"
            };

            await context.RespondAsync(embed: embed);
        }


        //[Command("join"), Description("Joins same voice channel as user who invoked the command")]
        //public async Task Join(CommandContext context)
        //{
        //    var vnext = context.Client.GetVoiceNextClient();
        //    var vnc = vnext.GetConnection(context.Guild);
        //    if (vnc != null)
        //        throw new InvalidOperationException("Already connected in this guild.");

        //    var vchan = context.Member?.VoiceState?.Channel;
        //    if (vchan == null)
        //        throw new InvalidOperationException("Must be in a voice channel.");
        //    vnc = await vnext.ConnectAsync(vchan);
        //    await context.RespondAsync("Connection successful.");
        //}

        //[Command("leave")]
        //public async Task Leave(CommandContext context)
        //{
        //    var vnext = context.Client.GetVoiceNextClient();

        //    var vnc = vnext.GetConnection(context.Guild);
        //    if (vnc == null)
        //        throw new InvalidOperationException("Not connected in this guild.");
        //    vnc.Disconnect();
        //    await context.RespondAsync("Disconnected.");
        //}

        [Command("trivia")]
        public async Task Trivia(CommandContext context)
        {
            //appear typing
            await context.TriggerTypingAsync();

            //retrieve interactivity module
            var interactivity = context.Client.GetInteractivityModule();

            //get token from triviaDB API
            var httpClient = new HttpClient();
            var tokenJSON = await httpClient.GetStringAsync("https://opentdb.com/api_token.php?command=request");
            dynamic tokenDes = JsonConvert.DeserializeObject(tokenJSON);
            string tokenString = tokenDes.token;

            //get question from triviaDB API
            var questionJSON = await httpClient.GetStringAsync("https://opentdb.com/api.php?amount=1&token=" + tokenString);

            //deserialize json
            Question questionDes = JsonConvert.DeserializeObject<Question>(questionJSON);
            //decode html formatting
            string question = WebUtility.HtmlDecode(questionDes.results[0].question);

            //base embed
            var embedQuestion = new DiscordEmbedBuilder
            {
                Title = $"TRIVIA | {questionDes.results[0].category}",
                Description = $"{question}",
                Color = new DiscordColor(0,255,0)
            };

            //decode answers list
            List<string> answerList = questionDes.results[0].incorrect;
            answerList = answerList.Select(WebUtility.HtmlDecode).ToList();
            answerList.Add(WebUtility.HtmlDecode(questionDes.results[0].correct));

            //add fields to embed
            if (questionDes.results[0].type == "multiple")
            {
                Shuffler.Shuffle(answerList);
                embedQuestion.AddField("A", $"{answerList.ElementAt(0)}");
                embedQuestion.AddField("B", $"{answerList.ElementAt(1)}");
                embedQuestion.AddField("C", $"{answerList.ElementAt(2)}");
                embedQuestion.AddField("D", $"{answerList.ElementAt(3)}");
            } else if (questionDes.results[0].type == "boolean")
            {
                embedQuestion.AddField($"A", $"{answerList.ElementAt(0)}");
                embedQuestion.AddField($"B", $"{answerList.ElementAt(1)}");
            }

            //match correct in results[0].correct to correct answer in answerList
            string answerLetter = null;
            for (int i = 0; i < answerList.Count; i++)
            {
                if (questionDes.results[0].correct.ToString().Equals(answerList.ElementAt(i).ToString()))
                {
                    if (i == 0) { answerLetter = "A"; }
                    if (i == 1) { answerLetter = "B"; }
                    if (i == 2) { answerLetter = "C"; }
                    if (i == 3) { answerLetter = "D"; }
                }
            }

            //push question to discord
            await context.RespondAsync(embed: embedQuestion);

            //check for responses
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Content.ToString().Equals(answerLetter), TimeSpan.FromSeconds(10));
            if (msg != null)
            {
                //push answer and winner to discord
                var embedWinner = new DiscordEmbedBuilder
                {
                    Title = $"Answer: {answerLetter}",
                    Description = $"Winner: {msg.User.Username}"
                };

                await context.RespondAsync(embed: embedWinner);
            } else
            {
                //push no response message
                var embedSilence = new DiscordEmbedBuilder
                {
                    Title = $"Answer: {answerLetter}",
                    Description = $"Winner: Nobody won."
                };
                await context.RespondAsync(embed: embedSilence);
            }
            
            httpClient.Dispose();
        }

    }
}
