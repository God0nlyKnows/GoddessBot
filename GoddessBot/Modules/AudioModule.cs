using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using GoddessBot.Services;
namespace GoddessBot.Modules
{
    public class AudioModule : ModuleBase
    {
        // Scroll down further for the AudioService.
        // Like, way down
        private readonly AudioService _service;

        // Remember to add an instance of the AudioService
        // to your IServiceCollection when you initialize your bot


        public AudioModule(AudioService service)
        {
            _service = service;
        }

        // You *MUST* mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {

            var service = new AudioService();
            await _service.ConnectAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            //await _service.SendAsync(Context.Guild,Context.Channel,"https://www.youtube.com/playlist?list=PLHXbqPfGR9PV3Sw-sDHll8aZXD39Xw62l","");
        }

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leave", RunMode = RunMode.Sync)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("stop", RunMode = RunMode.Sync)]
        public async Task Stop()
        {
            await _service.Stop();
        }

        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        public async Task PlayCmd([Remainder]string song)
        {
            await _service.ConnectAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            //song = song.Replace("\"", "");
            await _service.SendAsync(Context.Guild, Context.Channel, song, "");
        }

        [Command("skip", RunMode = RunMode.Sync)]
        public async Task Skip(int x = 0)
        {
            await _service.Skip(x);
        }

        [Command("szufluj", RunMode = RunMode.Sync)]
        public async Task Szufluj()
        {
            await _service.Szufluj();
        }

        [Command("reverse", RunMode = RunMode.Sync)]
        public async Task Reverse()
        {
            await _service.Reverse();
        }

        [Command("jump", RunMode = RunMode.Sync)]
        public async Task JumpTo(int x)
        {
            await _service.JumpTo(x);
        }
        [Command("remove", RunMode = RunMode.Sync)]
        public async Task Remove(int x)
        {
            await _service.Remove(x);
        }
        [Command("loop")]
        public async Task Loop()
        {

            await _service.Loop(Context.Message);
        }

        [Command("radio", RunMode = RunMode.Async)]
        public async Task Radio(string url = @"http://5.135.154.69:11590/")
        {

            await _service.StreamRadio(Context, url);
        }

        [Command("clear")]
        public async Task Clear()
        {

            await _service.Clear();
        }
        [Command("queue")]
        public async Task Queue()
        {

            await _service.Queue(Context);
        }

    }
}
