using System;
using System.IO;

namespace CatsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //on redimentionne toutes les image en 128*128
            if(false)
            {
                Pixelisator pixelisator = new Pixelisator(16, 16);
                pixelisator.PixelizeImagesFromDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_128"), Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_16"));
            }

            //on crée un rep avec les image bruité
            if(false)
            {
                float[] noisePercentages = new float[] { 0.99f, 0.97f, 0.94f, 0.9f, 0.85f, 0.79f, 0.72f, 0.64f, 0.65f, 0.45f, 0.34f, 0.12f };
                string[] noisyImagesDirectoriesTmp = new string[] { "1%", "3%", "6%", "10%", "15%", "21%", "28%", "36%", "45%", "55%", "66%", "88%" };
                string[] noisyImagesDirectories = new string[noisyImagesDirectoriesTmp.Length];
                for (int i = 0; i < noisyImagesDirectories.Length; i++)
                {
                    noisyImagesDirectories[i] = Path.Combine(Directory.GetCurrentDirectory(), "Cats", "noisy_16", noisyImagesDirectoriesTmp[i]);
                }

                NoisyTexturesGenerator.GenerateNoisyImages(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_16"), noisyImagesDirectories, noisePercentages);
            }

            Console.WriteLine("Finish!");
            Console.ReadLine();
        }
    }
}
