using System;
using System.IO;

namespace CatsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //on redimentionne toutes les image
            if(true)
            {
                Pixelisator pixelisator;
                //pixelisator = new Pixelisator(128, 128);
                //pixelisator.PixelizeImagesFromDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "raw"), Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_128"));

                pixelisator = new Pixelisator(64, 64);
                pixelisator.PixelizeImagesFromDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_128"), Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_64"));

                pixelisator = new Pixelisator(32, 32);
                pixelisator.PixelizeImagesFromDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_64"), Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_32"));

                pixelisator = new Pixelisator(16, 16);
                pixelisator.PixelizeImagesFromDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_32"), Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_16"));
            }

            //on crée un rep avec les image bruité
            if(false)
            {
                float[] noisePercentages = new float[] { 0.99f, 0.97f, 0.94f, 0.9f, 0.85f, 0.79f, 0.72f, 0.64f, 0.65f, 0.45f, 0.34f, 0.12f };
                string[] noisyImagesDirectoriesTmp = new string[] { "1%", "3%", "6%", "10%", "15%", "21%", "28%", "36%", "45%", "55%", "66%", "88%" };
                string[] noisyImagesDirectories_16 = new string[noisyImagesDirectoriesTmp.Length];
                string[] noisyImagesDirectories_32 = new string[noisyImagesDirectoriesTmp.Length];
                string[] noisyImagesDirectories_64 = new string[noisyImagesDirectoriesTmp.Length];
                string[] noisyImagesDirectories_128 = new string[noisyImagesDirectoriesTmp.Length];
                for (int i = 0; i < noisyImagesDirectoriesTmp.Length; i++)
                {
                    noisyImagesDirectories_16[i] = Path.Combine(Directory.GetCurrentDirectory(), "Cats", "noisy_16", noisyImagesDirectoriesTmp[i]);
                    noisyImagesDirectories_32[i] = Path.Combine(Directory.GetCurrentDirectory(), "Cats", "noisy_32", noisyImagesDirectoriesTmp[i]);
                    noisyImagesDirectories_64[i] = Path.Combine(Directory.GetCurrentDirectory(), "Cats", "noisy_64", noisyImagesDirectoriesTmp[i]);
                    noisyImagesDirectories_128[i] = Path.Combine(Directory.GetCurrentDirectory(), "Cats", "noisy_128", noisyImagesDirectoriesTmp[i]);
                }

                NoisyTexturesGenerator.GenerateNoisyImages(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_16"), noisyImagesDirectories_16, noisePercentages);
                Console.WriteLine("Finish generate 16*16 noisy images");
                NoisyTexturesGenerator.GenerateNoisyImages(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_32"), noisyImagesDirectories_32, noisePercentages);
                Console.WriteLine("Finish generate 32*32 noisy images");
                NoisyTexturesGenerator.GenerateNoisyImages(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_64"), noisyImagesDirectories_64, noisePercentages);
                Console.WriteLine("Finish generate 64*64 noisy images");
                NoisyTexturesGenerator.GenerateNoisyImages(Path.Combine(Directory.GetCurrentDirectory(), "Cats", "pixellized_128"), noisyImagesDirectories_128, noisePercentages);
            }

            Console.WriteLine("Finish!");
            Console.ReadLine();
        }
    }
}
