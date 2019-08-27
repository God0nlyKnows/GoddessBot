using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using GoddessBot.Modules;

namespace GoddessBot.Services
{
    public class AudioService
    {

        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private bool loop = false;

        class AudioHandler
        {
            public AudioOutStream stream;
            public Process ffmpeg;
        }



        public async Task ConnectAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }

     

        
        public async Task SendAsync(IGuild guild, IMessageChannel channel, string path)
        {

            var handler = new AudioHandler();

            try
            {

                if (handler.ffmpeg != null)
                    handler.ffmpeg.Kill();
                

            }
            catch { }

            
            

            if (path.Contains("you"))
            {
                IAudioClient client;
                if (ConnectedChannels.TryGetValue(guild.Id, out client))
                {
                    System.Console.WriteLine($"Starting playback of {path} in {guild.Name}");
                    
                    
                    
                    CreateYoutubeStream(path);
                    do
                    {
                        using (handler.ffmpeg = CreateStream("Music\\mp3.mp3"))
                        using (handler.stream = client.CreatePCMStream(AudioApplication.Music, 96000, 500, 80))
                        {

                            try
                            {

                                handler.ffmpeg.StandardOutput.BaseStream.CopyTo(handler.stream);

                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                            finally { await handler.stream.FlushAsync(); }
                        }
                    } while (loop);
                }


            }
            else
            {
                if (!File.Exists(path))
                {
                    await channel.SendMessageAsync("File does not exist.");
                    return;
                }
                IAudioClient client;
                if (ConnectedChannels.TryGetValue(guild.Id, out client))
                {
                    System.Console.WriteLine($"Starting file {path} in {guild.Name}");
                    using (handler.ffmpeg = CreateStream(path))
                    using (handler.stream = client.CreatePCMStream(AudioApplication.Music))
                    {
                        
                        try {
                        await handler.ffmpeg.StandardOutput.BaseStream.CopyToAsync(handler.stream);
                         
                        }
                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                        finally { await handler.stream.FlushAsync(); }
                    }
                }
            }
        }

        public async Task SendTextAsync(SocketCommandContext context, string text)
        {
            await context.Channel.SendMessageAsync(text);
        }


        public async Task SendFileAsync(SocketCommandContext context, string path)
        {
            await context.Channel.SendFileAsync(path);
      
        }
      

        public async Task StreamRadio(IAudioClient client, string url)
        {
            /*
            WebResponse res = await WebRequest.Create(@"http://uk5.internet-radio.com:8278/live").GetResponseAsync();
            Console.WriteLine(res.ContentLength);
            Stream web = res.GetResponseStream();
            var ffmpeg = CreateRadioStream();
            var input = ffmpeg.StandardInput.BaseStream;
            var output = ffmpeg.StandardOutput.BaseStream;
            var discord = client.CreatePCMStream(AudioApplication.Mixed, 1920);

            web.CopyTo(input);
            await output.CopyToAsync(discord);
            await discord.FlushAsync();
            if (DependencyMap.Get<VoiceService>().inUse())
                DependencyMap.Get<VoiceService>().stopContext();
            */
        }

        private Process CreateStream(string path)
        {
            ProcessStartInfo ffmpeg = new ProcessStartInfo
            {
                FileName = @"ffmpeg",
                Arguments = $"-i \"{path}\" -ac 2 -f s16le -ar 48000 -b:a 96k pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            return Process.Start(ffmpeg);
        }

        private void CreateYoutubeStream(string url)
        {
            if (File.Exists("Music\\mp3.mp3"))
                File.Delete("Music\\mp3.mp3");
            ProcessStartInfo process = new ProcessStartInfo
            {
                
                FileName = @"youtube-dl",
                Arguments = $@" --audio-quality 0 --extract-audio --audio-format mp3 {url} -o Music\mp3.mp3",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };
            var handler = Process.Start(process);
            handler.WaitForExit();
            
        }

        public async Task Stop()
        {
            var handler = new AudioHandler();
            handler.ffmpeg.Kill();
            await handler.stream.FlushAsync();
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }

        public async Task Loop(IMessage message)
        {
            loop = (loop) ? false : true;
            string reply = (loop) ? "loop enabled" : "loop disabled";
            await message.Channel.SendMessageAsync(reply);
           
        }
    }
}
