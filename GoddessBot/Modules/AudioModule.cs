using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
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
            //await _service.SendAsync(Context.Guild,Context.Channel, @"D:\Music\[Drumstep] - Pegboard Nerds - Try This [Monstercat Release].mp3");
        }

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            await _service.Stop();
        }

        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        public async Task PlayCmd([Remainder] string song)
        {
            //song = song.Replace("\"", "");
            await _service.SendAsync(Context.Guild, Context.Channel, song);
        }

        [Command("loop")]
        public async Task Loop()
        {

            await _service.Loop(Context.Message);
        }
    }
}
