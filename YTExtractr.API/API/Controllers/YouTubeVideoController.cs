using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace YTExtractr.API.API.Controllers
{
    [Route("api/YouTube/Video/{id?}")]
    [ApiController]
    public class YouTubeVideoController : ControllerBase
    {
        private YoutubeClient youtubeClient;

        const string testUrl = "H7E3G1PEOAY";

        public YouTubeVideoController(YoutubeClient youtubeClient)
        {
            this.youtubeClient = youtubeClient;
        }

        [HttpGet]
        public async Task<ActionResult<Video>> Get([FromRoute] string? id = testUrl)
        {
            var video = await youtubeClient.Videos.GetAsync(VideoId.Parse(id != null ? id : testUrl));

            return video;
        }

        [HttpGet("Streams")]
        public async Task<IActionResult> GetStreams([FromRoute] string? id = testUrl)
        {
            var streams = await youtubeClient.Videos.Streams.GetManifestAsync(VideoId.Parse(id != null ? id : testUrl));

            var test = new
            {
                videoOnly = streams.GetVideoOnlyStreams(),
                audioOnly = streams.GetAudioOnlyStreams(),
                muxed = streams.GetMuxedStreams()
            };

            var test2 = streams.GetVideoOnlyStreams().GroupBy(s => s.VideoQuality.MaxHeight, e => new
            {
                Resolution = e.VideoQuality.MaxHeight,
                Framerate = e.VideoQuality.Framerate,
                QualityLabel = e.VideoQuality.Label,
                VideoCodec = e.VideoCodec.Substring(0, e.VideoCodec.Length > 3 ? 4 : 3),
                Container = e.Container.Name,
                Size = e.Size.MegaBytes,
                Bitrate = e.Bitrate.KiloBitsPerSecond,
                Url = e.Url
            }, (k, o) => new
            {
                Key = k,
                Objs = o
            });

            return Ok(test2);
        }

    }
}
