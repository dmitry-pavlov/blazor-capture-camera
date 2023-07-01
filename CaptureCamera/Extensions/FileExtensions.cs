using System;
using System.Globalization;
using CaptureCamera.Models;

namespace CaptureCamera.Extensions;

public static class FileExtensions
{
    public static string GetFileSize(this FileDto file)
    {
        var byteSize = file.Length.GetValueOrDefault();

        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        if (byteSize == 0) return "0" + suf[0];

        var bytes = Math.Abs(byteSize);

        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

        var num = Math.Round(bytes / Math.Pow(1024, place), 1);

        return $"{(Math.Sign(byteSize) * num).ToString(CultureInfo.InvariantCulture)} {suf[place]}";
    }
}