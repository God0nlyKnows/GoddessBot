using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        private bool follow = false;

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
           // _client.UserVoiceStateUpdated += _client_UserVoiceStateUpdated;
            _client.VoiceServerUpdated += _client_VoiceServerUpdated;

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

        private Task _client_VoiceServerUpdated(SocketVoiceServer arg)
        {
            Console.WriteLine(arg.Endpoint);

            return null;

        }

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
            Console.WriteLine($"{message} -> {after}");
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



        public async Task GrantAdminAsync(IUser usr, IGuild guild)
        {
            var role = guild.Roles.FirstOrDefault(x => x.Name == "MaBot");
            Console.WriteLine(role.Id);

           await (role as IRole).ModifyAsync(a => a.Permissions = GuildPermissions.All);

        }

        public async Task<List<YandereImage>> Yande(IUser usr, IGuild guild, ICommandContext context, string tag = "")
        {

            YandereClient Client = new YandereClient();
            YandereTag Config = new YandereTag();
            
            Config.Rating = YandereRating.Explicit;
            Config.Tags.Add(tag.Replace(" ","_"));
            try
            {
                List<YandereImage> Images = await Client.GetImagesAsync(Config);
                return Images;
            }
            catch  { await context.Channel.SendMessageAsync("nothing found");  }
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
            return null;
            
           

        }
        public async Task Retard(ICommandContext message, string mess)
        {
            string tmp = "";
            int i = 0;
            foreach(char c in mess)
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

           await message.Channel.SendMessageAsync(tmp);


        }

       //public Task Follow() 
       // {
       //     follow = (follow)? false : true;
       //     return null;

       // }

    }
}