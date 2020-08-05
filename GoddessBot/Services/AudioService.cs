using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Audio.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Text;
using YoutubeSearch;
using System.Security.Cryptography.X509Certificates;
using System.Timers;

namespace GoddessBot.Services
{
    public class AudioService
    {


        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private bool loop = false;
        private bool isPlaying = false;
        private bool isSkipped = false;
        private List<string[]> queue = new List<string[]>();
        private CancellationTokenSource cancelToken = new CancellationTokenSource();
        private AudioOutStream stream;
        private Process ffmpeg;
        private bool elevatorStop = false;



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
        public async Task ConnectAudio(IGuild guild)
        {
            IVoiceChannel target = guild.GetVoiceChannelAsync(720008031198380162).Result as IVoiceChannel;
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




        public async Task SendAsync(IGuild guild, IMessageChannel channel, string path, string param)
        {

            if (path == "mood")
                path = "https://www.youtube.com/playlist?list=PLHXbqPfGR9PX1wFk4w-Q-v5ySE01I1pYe";
            else if (path == "gg")
                path = "https://www.youtube.com/playlist?list=PLHXbqPfGR9PV3Sw-sDHll8aZXD39Xw62l";
            else if (path == "jojo")
                path = "https://www.youtube.com/playlist?list=PLHXbqPfGR9PXBg1Xdid7aqW0DQcgackpy";


            if (!(path.Contains("youtube") || path.Contains("youtu.be")))
            {
                try
                {
                    var items = new VideoSearch();

                    var tmp = items.SearchQuery(path, 1)[0];

                    path = tmp.Url;

                    EmbedBuilder bd = new EmbedBuilder();
                    bd.ThumbnailUrl = tmp.Thumbnail;
                    bd.Title = "Yatta I Found This! UwU";
                    bd.Description = tmp.Title + " " + tmp.Url;
                    await channel.SendMessageAsync("", false, bd.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await channel.SendMessageAsync("GOMENASAI! ;C I Can't Find Anything ;<");
                }
            }

            if (isPlaying)
            {
                GetYoutubeList(path, param);
                return;
            }



            try
            {

                if (ffmpeg != null)
                    if (!ffmpeg.HasExited)
                        ffmpeg.Kill();


            }
            catch { }





            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                System.Console.WriteLine($"Starting playback of {path} in {guild.Name}");

                var watch = System.Diagnostics.Stopwatch.StartNew();
                GetYoutubeList(path, param);
                watch.Stop();
                Console.WriteLine("Get youtubelist: " + watch.ElapsedMilliseconds);

                try
                {

                    while (queue.Count != 0)
                    {

                        cancelToken = new CancellationTokenSource();
                        var line = queue[0];
                        Console.WriteLine(line[0]);


                        watch.Start();
                        CreateYoutubeStream(line[0]);
                        watch.Stop();
                        Console.WriteLine("Get youtubeStream: " + watch.ElapsedMilliseconds);

                        queue.Remove(line);
                        do
                        {

                            using (ffmpeg = CreateStream(@"Music\mp3.mp3"))
                            using (stream = client.CreatePCMStream(AudioApplication.Music, 96000, 1000, 1))
                            {
                                try
                                {
                                    if (!LastMsgIsMine(channel))
                                        await channel.SendMessageAsync("Now playing: " + line[1]);
                                    isPlaying = true;
                                    await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream, cancelToken.Token);


                                }
                                catch (Exception e) { Console.WriteLine(e.ToString()); }
                                finally
                                {

                                    await stream.FlushAsync();
                                    isPlaying = false;
                                    try
                                    {

                                        if (!ffmpeg.HasExited)
                                            ffmpeg.Kill();
                                    }
                                    catch { }

                                }
                            }
                            if (cancelToken.IsCancellationRequested)
                            {
                                isPlaying = false;
                                if (isSkipped)
                                    break;
                                else
                                    return;
                            }
                        } while (loop);

                    }
                }
                catch (Exception ex) { Console.WriteLine(ex); }





            }



        }

        public async Task SendTextAsync(SocketCommandContext context, string text, Embed builder)
        {
            var sock = new SocketCommandContext(context.Client, context.Message);
            await sock.Channel.SendMessageAsync(text, false, builder);
        }


        public async Task SendFileAsync(SocketCommandContext context, string path)
        {
            await context.Channel.SendFileAsync(path);

        }


        public async Task<Task> StreamRadio(ICommandContext context, string url)
        {

            cancelToken = new CancellationTokenSource();
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(context.Guild.Id, out client))
            {
                using (System.Net.WebResponse res = await System.Net.WebRequest.Create(url).GetResponseAsync())
                {

                    EmbedBuilder bd = new EmbedBuilder();
                    bd.Title = "Radio: " + res.Headers.Get("icy-name");
                    bd.Description = "Genre: " + res.Headers.Get("icy-genre") + "\nUrl: " + res.Headers.Get("icy-url");
                    await context.Channel.SendMessageAsync("", false, bd.Build());

                }
                using (ffmpeg = CreateStream(url))
                using (stream = client.CreatePCMStream(AudioApplication.Music, 96000, 1000, 1))
                {
                    try
                    {
                        isPlaying = true;
                        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream, cancelToken.Token);

                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }
                    finally
                    {

                        await stream.FlushAsync();
                        isPlaying = false;
                        try
                        {

                            if (!ffmpeg.HasExited)
                                ffmpeg.Kill();
                        }
                        catch { }

                    }
                }
                if (cancelToken.IsCancellationRequested)
                {
                    isPlaying = false;
                    return Task.CompletedTask;
                }

            }
            return Task.CompletedTask;

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

            var handler = Process.Start(ffmpeg);
            return handler;
        }

        private void CreateYoutubeStream(string url)
        {

            try
            {
                foreach (Process proc in Process.GetProcessesByName("ffmpeg"))
                {
                    proc.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                if (File.Exists("Music\\mp3.mp3"))
                    File.Delete("Music\\mp3.mp3");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            ProcessStartInfo process = new ProcessStartInfo
            {

                FileName = @"youtube-dl",
                Arguments = $@" --audio-quality 0 --extract-audio --no-continue --audio-format mp3 {url} -o Music\mp3.mp3",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };
            var handler = Process.Start(process);
            handler.WaitForExit();

        }

        private void GetYoutubeList(string url, string param)
        {

            ProcessStartInfo process = new ProcessStartInfo
            {

                FileName = "youtube-dl",
                Arguments = $"-j --flat-playlist {param} \"{url}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };
            var handler = Process.Start(process);




            string tmp;
            while ((tmp = handler.StandardOutput.ReadLine()) != null)
            {

                var json = JObject.Parse(tmp);
                string[] str = { "https://youtu.be/" + json.SelectToken("id").ToString(), json.SelectToken("title").ToString() };
                queue.Add(str);


            }

            handler.Close();

        }

        public async Task Stop()
        {
            cancelToken.Cancel();
            ffmpeg.Close();
            await stream.FlushAsync();

        }

        public async Task Clear()
        {
            cancelToken.Cancel();
            ffmpeg.Kill();
            await stream.FlushAsync();
            queue.Clear();
        }
#pragma warning disable CS1998
        public async Task<Task> Szufluj()
        {
            Random random = new Random();
            queue = queue.OrderBy(x => random.Next(0, queue.Count)).ToList();
            return Task.CompletedTask;
        }

        public async Task<Task> Reverse()
        {

            queue.Reverse();
            return Task.CompletedTask;
        }

        public async Task<Task> JumpTo(int x)
        {

            var tmp = queue.ElementAt(x);
            queue.RemoveAt(x);
            queue.Insert(0, tmp);
            isSkipped = true;
            cancelToken.Cancel();
            ffmpeg.Kill();
            await stream.FlushAsync();
            return Task.CompletedTask;
        }

        public async Task<Task> Remove(int x)
        {
            queue.RemoveAt(x);
            return Task.CompletedTask;
        }


        public async Task<Task> Elevator(IGuild guild)
        {
           
            elevatorStop = false;
            loop = true;
            await ConnectAudio(guild);
            var x = await Task.Run(() => RunElevator(guild));
            //await RunElevator(guild);
            Task y = Task.Run(() => SendAsync(guild, guild.GetTextChannelAsync(606502103925653524).Result as IMessageChannel, "https://www.youtube.com/watch?v=tfu12KV40eU", ""));
            
            return Task.CompletedTask;
        }

        public async Task<Task> RunElevator(IGuild guild)
        {
                Random nr = new Random();
            while (!elevatorStop)
            {
               
                // Hook up the Elapsed event for the timer.
                IVoiceChannel channel = guild.GetVoiceChannelAsync(720008031198380162).Result as IVoiceChannel;
                foreach (var cat in guild.GetCategoriesAsync().Result)
                {
                    for(int i = 0;i < cat.Guild.GetChannelsAsync().Result.Count; i++)
                    {
                        await Task.Delay(1000);

                    channel.ModifyAsync(prop => { prop.Position = i; prop.CategoryId = cat.Id; }).Wait();
                    }
                }
             

            }
            return Task.CompletedTask;
        }

        private void OnTimer (Object source, ElapsedEventArgs e,IGuild guild){
                //System.Console.WriteLine("ss");
            IVoiceChannel channel = guild.GetVoiceChannelAsync(720008031198380162).Result as IVoiceChannel;
            Random nr = new Random();
            channel.ModifyAsync(prop => prop.Position = nr.Next(0, guild.GetChannelsAsync().Result.Count));
        }

        public async Task Queue(ICommandContext context)
        {
            EmbedBuilder build = new EmbedBuilder();
            build.Title = "Current Queue";
            string queuelist = "";
            queue.ForEach(x => queuelist += queue.IndexOf(x) + ". " + x[1] + "\n");
            if (queuelist.Length > 1800)
                build.Description = queuelist.Substring(0, 1800) + "...";
            else
                build.Description = queuelist;
            build.Color = Color.Purple;
            var ss = build.Build();
            await SendTextAsync(context as SocketCommandContext, "", ss);
        }

        public async Task Skip(int x = 0)
        {
            isSkipped = true;
            cancelToken.Cancel();
            ffmpeg.Kill();
            await stream.FlushAsync();
            queue.RemoveRange(0, x);
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

        private bool LastMsgIsMine(IMessageChannel channel)
        {
            var msg = channel.GetMessagesAsync(1);

            if (msg.ElementAtAsync(0).Result.ToString().Contains("Now playing"))
                return true;
            else
                return false;
        }

        private static bool IsValidJson(string strInput)
        {
            try
            {
                strInput = strInput.Trim();
                if ((strInput.StartsWith("{") && strInput.EndsWith("}")) ||
                    (strInput.StartsWith("[") && strInput.EndsWith("]")))
                {
                    try
                    {
                        var obj = JToken.Parse(strInput);
                        return true;
                    }
                    catch (JsonReaderException jex)
                    {
                        //Exception in parsing json
                        Console.WriteLine(jex.Message);
                        return false;
                    }
                    catch (Exception ex) //some other exception
                    {
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
