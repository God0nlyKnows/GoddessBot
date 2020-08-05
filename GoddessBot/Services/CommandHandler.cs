#pragma warning disable 1998
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yande.re;

namespace GoddessBot.Services
{
    public class CommandHandler
    {
        // setup fields to be set later in the constructor
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;


        public CommandHandler(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;

            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;

            _client.MessageUpdated += MessageUpdated;

            _client.UserJoined += _client_UserJoinedAsync;




        }





        /*  private Task _client_UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3) //buggy as f 
          {
              try
              { if(follow)
                  if (arg1.Id == 243299631180546048)
                      arg3.VoiceChannel.Users.FirstOrDefault(x => x.Id == 243299631180546048).VoiceChannel.ConnectAsync();
              }
              catch { }
              return null;
          }*/



        public CommandHandler()
        {
        }

        private async Task _client_UserJoinedAsync(SocketGuildUser arg)
        {
            var role = arg.Guild.Roles.FirstOrDefault(x => x.Name == "Zwykłe żółte lunty");
            await (arg as IGuildUser).AddRoleAsync(role);
            var builder = new EmbedBuilder();
            builder.WithTitle("Warning !!!");
            builder.WithDescription(arg.Mention + " a new Big Black NiBBa's approaching\n\n" + arg.Guild.Owner.Mention + " Prepare him correctly!\n\nIts OUR #" + arg.Guild.Users.Count + " nibba ♿");
            builder.Color = Color.Purple;

            await arg.Guild.TextChannels.FirstOrDefault(x => x.Name == "no-game-no-life").SendMessageAsync("", false, builder.Build());
            await arg.Guild.TextChannels.FirstOrDefault(x => x.Name == "no-game-no-life").SendFileAsync("Pic\\original.gif", null);
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {before.Value}");
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // this class is where the magic starts, and takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            // sets the argument position away from the prefix we set
            var argPos = 0;

            // get prefix from the configuration file
            char prefix = Char.Parse(_config["Prefix"]);

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            // execute command if one is found that matches
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            var date = DateTime.UtcNow;
            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                System.Console.WriteLine($"{date} Command failed for [{context.User}], Serv[{context.Guild + "/" + context.Channel}], Msg[{context.Message}]!");
                return;
            }


            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                System.Console.WriteLine($"{date} Command [{context.Message}] executed for -> [{context.User}]");
                return;
            }


            // failure scenario, let's let the user know
            await context.Channel.SendMessageAsync($"{date} Sorry, ... something went wrong -> [{context.Message}]!");
        }


        public async Task Write()
        {
            await _client.StartAsync();
        }
        public async Task GrantAdminAsync(ICommandContext usr, IGuild guild)
        {
            //var role = guild.Roles.FirstOrDefault(x => x.Name == "MaBot");

            //usr.Permissions.Modify(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true);
            //Console.WriteLine(role.Id);

            // await (role as IRole).ModifyAsync(a => a.Permissions = GuildPermissions.All);

        }

        public async Task Send(IGuild guild, ulong id, string msg)
        {
            var channel = await guild.GetTextChannelAsync(id);
            await channel.SendMessageAsync(msg);


        }

        public async Task DeleteMgs(IMessage mgs)
        {
            await mgs.DeleteAsync();
        }

        public async Task Delete(ITextChannel channel, int n)
        {
            for (int i = 0; i < n; i++)
            {
                var msg = await channel.GetMessageAsync(channel.Id);
                await msg.DeleteAsync();
            }
        }

        public async Task<Task> SaveYande(Uri url, string filename, ICommandContext context)
        {

            using (System.Net.WebClient web = new System.Net.WebClient())
            {
                web.DownloadFileCompleted += (sender, e) => DownloadCompleted(sender, e, context, filename);
                web.DownloadFileAsync(url, @"Yande\" + filename);




            }

            return Task.CompletedTask;

        }
        public async static void DownloadCompleted(object sender, AsyncCompletedEventArgs e, ICommandContext context, string filename)
        {
            if (e.Error != null && e.Error.ToString() != "")
            {
                await context.Channel.SendMessageAsync("SENPAIII!!! Something goes wrong while downloading another lewd pic!");
            }
            else
                try
                {
                    await context.Channel.SendFileAsync(@"Yande\" + filename);
                }
                catch (Discord.Net.HttpException ex)
                {
                    Console.WriteLine(ex);
                    await context.Channel.SendMessageAsync("YAMETE! >///< file is toooo big onii-chan!");
                }

        }


        #region getImages


        public async Task<BooruSharp.Search.Post.SearchResult> Yande(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            BooruSharp.Booru.Yandere Yandere = new BooruSharp.Booru.Yandere();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Yandere.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> GelBooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            BooruSharp.Booru.Gelbooru Booru = new BooruSharp.Booru.Gelbooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> Furrybooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            BooruSharp.Booru.Furrybooru Booru = new BooruSharp.Booru.Furrybooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> Atfbooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Atfbooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }
        public async Task<BooruSharp.Search.Post.SearchResult> DanbooruDonmai(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.DanbooruDonmai();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> E621(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.E621();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> E926(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.E926();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }


        public async Task<BooruSharp.Search.Post.SearchResult> Konachan(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Konachan();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }
        public async Task<BooruSharp.Search.Post.SearchResult> Lolibooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Lolibooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> Realbooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Realbooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> Rule34(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Rule34();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> Safebooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Safebooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> Sakugabooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Sakugabooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }

        public async Task<BooruSharp.Search.Post.SearchResult> Xbooru(IUser usr, IGuild guild, ICommandContext context, string tag = "thighhighs")
        {

            var Booru = new BooruSharp.Booru.Xbooru();
            BooruSharp.Search.Post.SearchResult result = new BooruSharp.Search.Post.SearchResult();
            try
            {
                result = await Booru.GetRandomPostAsync(tag.Split(','));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await SearchTag(context, tag);
            }

            return result;

        }
        #endregion
        public async Task<Task> GetTagInfo(ICommandContext context, string tag)
        {
            BooruSharp.Booru.Konachan konachan = new BooruSharp.Booru.Konachan();
            BooruSharp.Search.Wiki.SearchResult result = await konachan.GetWikiAsync(tag);
            EmbedBuilder builder = new EmbedBuilder();
            builder.Color = Color.DarkPurple;
            builder.Title = result.title;
            builder.Description = result.body;
            await context.Channel.SendMessageAsync("", false, builder.Build());

            return Task.CompletedTask;
        }

        public async Task SearchTag(ICommandContext context, string tag)
        {
            BooruSharp.Booru.Konachan Booru = new BooruSharp.Booru.Konachan();
            BooruSharp.Search.Tag.SearchResult[] results = await Booru.GetTagsAsync(tag);
            string msg = "I can't find this tag! >///< \n" + String.Join(Environment.NewLine, results.Where(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.type == BooruSharp.Search.Tag.TagType.Character); }).Select(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.name); }));
            await context.Channel.SendMessageAsync(msg);
        }
        public async Task Retard(ICommandContext message, string mess)
        {
            string tmp = "";
            int i = 0;
            foreach (char c in mess)
            {
                if (++i % 2 == 0)
                {
                    tmp += c.ToString().ToLower();
                }
                else
                {
                    tmp += c.ToString().ToUpper();
                }
            }

            await message.Channel.DeleteMessageAsync(message.Message as IMessage);
            await message.Channel.SendMessageAsync(tmp);


        }

        //public Task Follow() 
        // {
        //     follow = (follow)? false : true;
        //     return null;

        // }

    }
}

//YandereClient Client = new YandereClient();
//YandereTag Config = new YandereTag();
//Config.Rating = YandereRating.Explicit;
//Config.Tags.Add(tag.Replace(" ","_"));

//   var _builder = new ConfigurationBuilder()
//   .SetBasePath(AppContext.BaseDirectory)
//    .AddJsonFile(path: "config.json");

// build the configuration and assign to _config          
// IConfiguration _config = _builder.Build();
// var page = Convert.ToInt32(_config["YandePage"]);
// Config.Page = 1;
// try
// {
//Console.WriteLine($"page = {page}");
//  List<YandereImage> Images = await Client.GetImagesAsync(Config);
// page++;
// _config["YandePage"] = page.ToString();
//   return Images;
//   }
// catch  { await context.Channel.SendMessageAsync("nothing found");  }
//using (var client = new WebClient())
//{
//    string filename = "Yande\\jpg.jpg";
//    try
//    {
//        Uri uri = new Uri(Images[0].ImageUrl);

//        client.DownloadFileAsync(uri, filename);
//    }
//    catch (Exception a) {
//        Console.WriteLine(a.ToString());
//    }
//}
