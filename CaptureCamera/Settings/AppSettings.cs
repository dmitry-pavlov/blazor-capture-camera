using System.Collections.Generic;
using CaptureCamera.Interfaces;

namespace CaptureCamera.Settings;

public class AppSettings : ISelfCheck
{
    public BlobSettings Storage { get; set; } = new();

    void ISelfCheck.Check() => new List<ISelfCheck> { Storage }.ForEach(self => self.Check());
}