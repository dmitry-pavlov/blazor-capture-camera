using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimatedGif;
using CaptureCamera.Extensions;
using CaptureCamera.Interfaces;
using CaptureCamera.Models;
using Microsoft.Extensions.Logging;

namespace CaptureCamera.Services;

public class VideoRecorder
{
    private readonly IFileStorage _storage;
    private readonly ILogger<VideoEncoder> _logger;

    public VideoRecorder(IFileStorage storage, ILogger<VideoEncoder> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    public async Task<FileDto> HandleCapturedFramesFrom(Survey survey, CancellationToken token = default)
    {
        using var output = new MemoryStream();
        using var gif = new AnimatedGifCreator(output, 200);
        var video = new FileDto { Directory = "Surveys", FileName = $"{DateTime.UtcNow:s}-survey-{survey.Id}.gif", ContentType = "image/gif" };

        _logger.LogInformation("Service creating video file '{FileName}' from {FramesCount} captured frames...", video.FileName, survey.Frames.Count);
        foreach (var frame in survey.Frames.OrderBy(pair => pair.Key))
        {
            _logger.LogInformation("- adding frame '{FrameName}'...", frame.Key);
            using var input = new MemoryStream(frame.Value);
            var image = Image.FromStream(input);
            await gif.AddFrameAsync(image, delay: 200, quality: GifQuality.Bit8, cancellationToken: token);
        }

        _logger.LogInformation("Uploading video file '{FileName}' to storage directory '{FileDirectory}'...", video.FileName, video.Directory);
        survey.Recording = await _storage.Upload(video, output.ToArray(), token);

        _logger.LogInformation("{FileSize} has been uploaded to '{FilePath}'.", survey.Recording.GetFileSize(), $"{survey.Recording.Directory}/{survey.Recording.FileName}");

        return survey.Recording;
    }

}