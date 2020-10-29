using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WizzifyWeb
{
    class YoutubeDownloader
    {
        public string videoid { get; set; }
        public string APIKey { get; set; }
        public List<string> videos { get; set; }

        public YoutubeDownloader(string APIKey) {
            this.APIKey = APIKey;
        }

        public async Task Coverter() {
            // Our test youtube link
            string link = "https://www.youtube.com/watch?v=" + this.videoid;
            Console.WriteLine("searching" + link);

            /*
             * Downloading youtube file using
             * youtube-dl exe file, by executing via command-line, the code is wrapped in 
             * try-catch scope for eventual System.Exception Exception type handling
             * Check the developer page for requirements, license and documentation;
             */

            try {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                string exeFile = "youtube-dl";
                string strCmdText;
                strCmdText = exeFile + "-f 'bestvideo[ext=mp4]' -o \"\\Music\\'%(title)s.%(ext)s'\" " + link;
                Console.WriteLine(strCmdText); //Check for the command line arguments
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = strCmdText;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit(); //Wait for CMD to finish download process

                /*
                 * Converting downloaded video from .mp4 format to .mp3 format
                 * using NRCore.Converter Package.
                 * Check Package page for license, usage and docuemntation;
                 */
                string[] files = Directory.GetFiles(".\\Music");
                
                if (!(files.Length == 0))
                {
                    foreach (string file in files)
                    {
                        file.Replace(".mp4", ".mp3");
                    }
                }
                else
                {
                    return ;
                }
            } catch(Exception ex){
                throw ex;
            }
            
        }


        public async Task<List<string>> Search(string videoName)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = this.APIKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = videoName; // Replace with your search term.
            searchListRequest.MaxResults = 10;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        this.videoid = searchResult.Id.VideoId;
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
                break;
            }
            
            Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
            return videos;
        }
    }
}
