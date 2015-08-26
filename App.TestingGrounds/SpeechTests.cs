using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
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

        private bool Continue = true;

        private void RunTests()
        {
            using (var engine =
                new SpeechRecognitionEngine(
                    new CultureInfo("en-US")))
            {
                //plain listener
                var grammar = new DictationGrammar();
                engine.LoadGrammar(grammar);

                engine.SpeechHypothesized += engine_SpeechHypothesized;
                engine.SpeechRecognized += engine_SpeechRecognized;
                engine.SpeechRecognitionRejected += engine_SpeechRecognitionRejected;

                engine.SetInputToDefaultAudioDevice();

                engine.RecognizeAsync(RecognizeMode.Multiple);

                while(Continue)
                {
                    Thread.Sleep(800);
                }
            }
        }

        void grammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("G: " + e.Result.Text + " - " + e.Result.Confidence);
        }

        void engine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("X: " + e.Result.Text + " - " + e.Result.Confidence);
        }

        void engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("0: " + e.Result.Text + " - " + e.Result.Confidence);
            if (e.Result.Text.ToLower().Contains("exit"))
            {
                Continue = false;
            }
        }

        void engine_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.WriteLine("H: " + e.Result.Text + " - " + e.Result.Confidence);
        }
    }
}
