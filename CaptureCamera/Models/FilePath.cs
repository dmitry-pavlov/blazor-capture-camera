using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CaptureCamera.Models;

public class FilePath
{
    public static FilePath From(params string[] parts)
    {
        try
        {
            return new FilePath
            {
                Parents = parts[..^1].ToList(),
                Last = parts.Last()
            };
        }
        catch (Exception exception)
        {
            throw new FormatException($"Failed to parse the path from parts: {string.Join(", ", parts)}", exception);
        }
    }

    public List<string> Parents { get; init; } = new();

    public string Last { get; private set; } = string.Empty;
}