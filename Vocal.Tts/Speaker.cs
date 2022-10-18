using System.Diagnostics;

namespace Vocal.Tts
{

    public class Speaker
    {
        public Speaker(string eSpeakPath)
        {
            this.eSpeakPath = eSpeakPath;
        }
        
        string eSpeakPath;
        public string VoiceLanguage { get; set; } = "fr";
        public int Speed { get; set; } = 175;
        public int Pitch { get; set; } = 50;

        string GetVoice(string voiceLanguage)
        {
            return "-v" + voiceLanguage;
        }

        string GetSpeed()
        {
            return "-s " + Speed;
        }

        string GetPitch()
        {
            return "-p " + Pitch;
        }

        public void SpeakText(string text)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = eSpeakPath,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"{GetVoice(VoiceLanguage)} {GetSpeed()} {GetPitch()} \"{text}\""
            };
            Process.Start(startInfo).WaitForExit();
        }

    }
}