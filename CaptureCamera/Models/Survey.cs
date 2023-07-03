using System;
using System.Collections.Generic;

namespace CaptureCamera.Models;

public class Survey
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public FileDto? Recording { get; set; }

    public Dictionary<string, byte[]> Frames { get; set; } = new();

    public List<Step> Steps { get; set; } = new()
    {
        new (){Id = 101, Question = "Would you rather never be stuck in traffic again or never get another cold?"},
        new (){Id = 102, Question = "Would you rather live on the beach or in a cabin in the woods?"},
        new (){Id = 103, Question = "Would you rather travel the world for a year, all expenses paid, or have $40,000 to spend on whatever you want?"},
        new (){Id = 104, Question = "Would you rather lose all your money and valuables or lose all the pictures you have ever taken?"},
        new (){Id = 105, Question = "Would you rather speak to animals or speak 10 foreign languages?"},
        new (){Id = 106, Question = "Would you rather be the hero that saved the girl or the villain that took over the world?"},
        new (){Id = 107, Question = "Would you rather lick the bottom of your shoe or eat your boogers?"},
        new (){Id = 108, Question = "Would you rather eat a dead bug or a live worm?"},
        new (){Id = 109, Question = "Would you rather brush your teeth with soap or drink sour milk?"},
        new (){Id = 110, Question = "Would you rather only be able to walk on all fours or only be able to walk sideways like a crab?"},
        new (){Id = 111, Question = "Would you rather surf in the ocean with a bunch of sharks or surf with a bunch of jellyfish?"},
        new (){Id = 112, Question = "Would you rather climb the highest mountains or swim in the deepest seas?"},
        new (){Id = 113, Question = "Would you rather talk like Darth Vader or speak in the language of the Middle Ages?"},
        new (){Id = 114, Question = "Would you rather be good-looking but stupid or ugly but intelligent?"}
    };

    public class Step
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string? Answer { get; set; }
    }

    public void AddFrame(int? currentStepId, byte[] frameAsBase64String)
    {
        Frames.Add($"step-{currentStepId}-{DateTime.Now:O}", frameAsBase64String);
    }
}