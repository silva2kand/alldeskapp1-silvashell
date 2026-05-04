using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;

namespace SilvaShell.App.Core;

public class TtsVoice
{
    public string Name { get; set; } = "";
    public string Culture { get; set; } = "";
    public string DisplayName { get; set; } = "";
}

public static class SpeechService
{
    private static readonly SpeechSynthesizer Synth = new();

    public static IReadOnlyList<TtsVoice> GetVoices()
    {
        var raw = Synth.GetInstalledVoices()
            .Select(v => new TtsVoice
            {
                Name = v.VoiceInfo.Name,
                Culture = v.VoiceInfo.Culture.Name,
                DisplayName = v.VoiceInfo.Name
            })
            .ToList();

        foreach (var v in raw)
        {
            if (v.Culture == "ta-IN")
                v.DisplayName = "Tamil (Jaffna / India)";
            else if (v.Culture == "en-GB")
                v.DisplayName = "English (UK)";
            else if (v.Culture == "en-US")
                v.DisplayName = "English (US)";
        }

        return raw;
    }

    public static void SetVoice(string voiceName)
    {
        if (!string.IsNullOrWhiteSpace(voiceName))
            Synth.SelectVoice(voiceName);
    }

    public static void SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        Synth.SpeakAsyncCancelAll();
        Synth.SpeakAsync(text);
    }
}
