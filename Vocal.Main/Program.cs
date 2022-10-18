using System.Runtime.InteropServices;
using System.Text.Json;
using PortAudioSharp;
using Vosk;
using Vocal.Intents;
using Vocal.Tts;
using Newtonsoft.Json.Linq;

namespace Vocal.Main
{
    class Program
    {
        static StreamParameters oParams;
        static Model model = new Model("model");
        static VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
        static IntentClassifier ic = new IntentClassifier(null);
        static Speaker speaker = new Speaker("/usr/bin/espeak");

        static void Main(string[] args)
        {
            PortAudio.LoadNativeLibrary();
            PortAudio.Initialize();

            oParams.device = PortAudio.DefaultInputDevice;
            if (oParams.device == PortAudio.NoDevice)
                throw new Exception("No default audio input device available");

            oParams.channelCount = 1;
            oParams.sampleFormat = SampleFormat.Int16;
            oParams.hostApiSpecificStreamInfo = IntPtr.Zero;

            var callbackData = new VoskCallbackData()
                {
                    textResult=String.Empty
                };

            var stream = new PortAudioSharp.Stream(
                oParams,
                null,
                16000,
                4096,
                StreamFlags.ClipOff,
                playCallback,
                callbackData
            );

            stream.Start();
            Console.WriteLine("Vocal assistant is ready");
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
            stream.Stop();
        }

        class VoskCallbackData
        {
            public String textResult { get; set; }
        }

        class VoskResult{
            public String text { get; set; }
        }

        private static StreamCallbackResult playCallback(
            IntPtr input, IntPtr output,
            System.UInt32 frameCount,
            ref StreamCallbackTimeInfo timeInfo,
            StreamCallbackFlags statusFlags,
            IntPtr dataPtr
        )
        {
            byte[] buffer = new byte[frameCount];
            Marshal.Copy(input , buffer, 0, buffer.Length);
            System.IO.Stream streamInput = new MemoryStream(buffer);
            using(System.IO.Stream source = streamInput) {
                byte[] bufferRead = new byte[frameCount];
                int bytesRead;
                while((bytesRead = source.Read(bufferRead, 0, bufferRead.Length)) > 0) {
                    if (rec.AcceptWaveform(bufferRead, bytesRead)) {
                        VoskResult result = JsonSerializer.Deserialize<VoskResult>(rec.Result());
                        if(result.text.Trim() != "") {
                            JObject intents = ic.FindIntents(result.text);
                            string mainIntent = (string)intents["intents"][0]["intent"];

                            //Console.WriteLine(mainIntent);

                            switch (mainIntent) {
                                case "heure":
                                    speaker.SpeakText("Il est " + DateTime.Now.ToString("HH") + "heures et " + DateTime.Now.ToString("mm") + "minutes");
                                    break;
                                default:
                                    speaker.SpeakText("Je n'ai pas bien compris votre demande, pouvez-vous répéter?");
                                    break;
                            }
                        }
                    }
                }
            }

            return StreamCallbackResult.Continue;
            
        }
    }
}