using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    public class SpeechTests
    {
        public static void Test()
        {
            (new SpeechTests()).RunTests();
        }

        private SpeechSynthesizer Synth;
        private SpeechRecognitionEngine Engine;

        private bool Continue = true;

        private void RunTests()
        {
            using (Synth = new SpeechSynthesizer())
            using (Engine =
                new SpeechRecognitionEngine(
                    new CultureInfo("en-US")))
            {
                var specialGrammar = new Grammar(new SrgsDocument(@"C:\dev\Current\Tools\App.TestingGrounds\SolitaireGrammar.xml"));
                Engine.LoadGrammar(specialGrammar);

                //plain listener
                //var grammar = new DictationGrammar();
                //Engine.LoadGrammar(grammar);

                Engine.SpeechHypothesized += engine_SpeechHypothesized;
                Engine.SpeechRecognized += engine_SpeechRecognized;
                Engine.SpeechRecognitionRejected += engine_SpeechRecognitionRejected;

                Synth.SetOutputToDefaultAudioDevice();
                Engine.SetInputToDefaultAudioDevice();

                Engine.RecognizeAsync(RecognizeMode.Multiple);

                while (Continue)
                {
                    Thread.Sleep(800);
                }

                Engine.RecognizeAsyncStop();
                Synth.SpeakAsyncCancelAll();
            }
        }

        void grammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("   !" + e.Result.Text + " - " + e.Result.Confidence + "!");
        }

        void engine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("XXX=- " + e.Result.Text + " - " + e.Result.Confidence + " -=XXX");
        }

        void engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.7)
            {
                if(e.Result.Semantics.ContainsKey("CommandName"))
                {
                    switch(e.Result.Semantics["CommandName"].Value.ToString())
                    {
                        case "OpenCommand":
                            break;
                        case "SwitchCommand":
                            if (e.Result.Semantics["Command"]["Monitor"].Value.ToString() == "tv")
                            {
                                SwitchToTv();
                            }
                            else if (e.Result.Semantics["Command"]["Monitor"].Value.ToString() == "desktop")
                            {
                                SwitchToDesktop();
                            }
                            break;
                        case "Shutdown":
                            Continue = false;
                            break;
                    }
                }
            }

            Console.WriteLine(e.Result.Text + " - " + e.Result.Confidence);

            ////Synth.SpeakAsyncCancelAll();
            ////Synth.SpeakAsync(e.Result.Text);
            //PrintSemanticsRecursive(string.Empty, e.Result.Semantics);
        }

        void engine_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.WriteLine("   ?" + e.Result.Text + " - " + e.Result.Confidence + "?");
        }

        void PrintSemanticsRecursive(string prepend, SemanticValue value)
        {
            foreach (var child in value)
            {
                var line = string.Format("{2}{0} = {1}", child.Key, child.Value.Value, prepend);
                Console.WriteLine(line);
                if (child.Value.Any())
                {
                    PrintSemanticsRecursive(prepend + "- ", child.Value);
                }
            }
        }

        private void SwitchToDesktop()
        {
            Synth.SpeakAsync("Switching to Desktop");
            var proc = new Process();
            proc.StartInfo.FileName = "DisplaySwitch.exe";
            proc.StartInfo.Arguments = "/external";
            proc.Start();
        }

        private void SwitchToTv()
        {
            Synth.SpeakAsync("Switching to TV");
            var proc = new Process();
            proc.StartInfo.FileName = "DisplaySwitch.exe";
            proc.StartInfo.Arguments = "/internal";
            proc.Start();
        }
    }
}
