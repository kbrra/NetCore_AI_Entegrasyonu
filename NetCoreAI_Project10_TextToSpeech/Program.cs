using System.Speech.Synthesis;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Seslendirilecek Metni Giriniz: ");
        string text = Console.ReadLine();

        using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
        {
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.Rate = 0;  
            synthesizer.Volume = 100; 
            synthesizer.Speak(text);
        }

        Console.WriteLine("Seslendirme tamamlandı!");
    }
}
