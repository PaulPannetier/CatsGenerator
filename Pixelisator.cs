using System;
using System.IO;
using System.Drawing;

namespace CatsGenerator
{
    public class Pixelisator
    {
        public int targetWidth = 128, targetHeight = 128;

        public Pixelisator()
        {

        }

        public Pixelisator(int targetWidth, int targetHeight)
        {
            this.targetWidth = targetWidth;
            this.targetHeight= targetHeight;
        }

        private void PixelizedImage(string path, string fileName)
        {
            try
            {
                using (Image original = Image.FromFile(path))
                {
                    using (Bitmap bitmap = new Bitmap(original, new Size(targetWidth, targetHeight)))
                    {
                        bitmap.Save(fileName);
                        bitmap.Dispose();
                    }
                    original.Dispose();
                }
                GC.Collect();
            }
            catch
            {
                Console.WriteLine("Can't pixelized this image : " + path);
            }
        }

        public void PixelizeImagesFromDirectory(string directoryName, string pathToSavePixelizedImages)
        {
            if(!Directory.Exists(directoryName))
            {
                Console.WriteLine("Directory " + directoryName + " doesn't exist!");
                return;
            }

            int i = 1;
            PixellizedRecur(directoryName, pathToSavePixelizedImages, ref i);

            void PixellizedRecur(string dir, string pathToSave, ref int index)
            {
                string[] imagesPath = Directory.GetFiles(directoryName);
                foreach (string imagePath in imagesPath)
                {
                    if (imagePath.EndsWith(".jpg") || imagePath.EndsWith(".png"))
                    {
                        PixelizedImage(Path.Combine(dir, imagePath), Path.Combine(pathToSavePixelizedImages, i.ToString() + ".jpg"));
                        i++;
                    }
                }

                string[] directories = Directory.GetDirectories(dir);
                foreach (string directory in directories)
                {
                    PixellizedRecur(directory, pathToSave, ref index);
                }
            }
        }
    }

    public class RawTexture
    {
        public int width, height;
        public byte[] data;

        public RawTexture(int width, int height)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height * 3];
        }

        public RawTexture(Bitmap bitmap)
        {
            width = bitmap.Width;
            height = bitmap.Height;
            data = new byte[width * height * 3];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    this[i, j] = bitmap.GetPixel(i, j);
                }
            }
        }

        public Color this[int line, int column]
        {
            get
            {
                byte r = data[(column * 3) + line * width * 3];
                byte g = data[(column * 3) + 1 + line * width * 3];
                byte b = data[(column * 3) + 2 + line * width * 3];
                return Color.FromArgb(r, g, b);
            }
            set
            {
                data[(column * 3) + line * width * 3] = value.R;
                data[(column * 3) + 1 + line * width * 3] = value.G;
                data[(column * 3)  + 2 + line * width * 3] = value.B;
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap res = new Bitmap(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    res.SetPixel(i, j, this[i, j]);
                }
            }
            return res;
        }

        public void Save(string filename)
        {
            using(Bitmap b = ToBitmap())
            {
                b.Save(filename);
            }
        }
    }

    public static class NoisyTexturesGenerator
    {
        private static RawTexture GenerateNoise(int width, int height)
        {
            RawTexture res = new RawTexture(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    res[i, j] = Random.Color();
                }
            }

            return res;
        }

        private static RawTexture GenerateNoisyTexture(RawTexture texture, RawTexture noiseTexture, float noisePercentage)
        {
            noisePercentage = Math.Max(Math.Min(1f, noisePercentage), 0f);
            RawTexture res = new RawTexture(texture.width, texture.height);

            int R,G, B;
            float tmp = 1f - noisePercentage;
            for (int i = 0; i < texture.height; i++)
            {
                for (int j = 0; j < texture.width; j++)
                {
                    R = (int)(texture[i, j].R * tmp + noiseTexture[i, j].R * noisePercentage);
                    G = (int)(texture[i, j].G * tmp + noiseTexture[i, j].G * noisePercentage);
                    B = (int)(texture[i, j].B * tmp + noiseTexture[i, j].B * noisePercentage);
                    res[i, j] = Color.FromArgb(R, G, B);
                }
            }

            return res;
        }

        public static void GenerateNoisyImages(string imagesDirectory, string[] targetDirectories, float[] noisePercentages)
        {
            if(!Directory.Exists(imagesDirectory))
            {
                Console.WriteLine("Directory : " + imagesDirectory + " does not exist!");
                return;
            }

            if(targetDirectories.Length != noisePercentages.Length)
            {
                Console.WriteLine("targetDirectories and noisePercentages must have the same length! : " + targetDirectories.Length + "!=" + noisePercentages.Length);
                return;
            }

            string[] imagesName = Directory.GetFiles(imagesDirectory);

            foreach (string imageName in imagesName)
            {
                try
                {
                    using (Image original = Image.FromFile(Path.Combine(imagesDirectory, imageName)))
                    {
                        using (Bitmap bitmap = new Bitmap(original))
                        {
                            RawTexture rawTexture = new RawTexture(bitmap);
                            RawTexture noiseTexture = GenerateNoise(rawTexture.width, rawTexture.height);

                            for (int i = 0; i < targetDirectories.Length; i++)
                            {
                                RawTexture noisyTexture = GenerateNoisyTexture(rawTexture, noiseTexture, noisePercentages[i]);
                                noisyTexture.Save(Path.Combine(targetDirectories[i], Path.GetFileName(imageName)));
                            }
                        }
                    }
                    GC.Collect();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Image : " + imageName + " can't be load");
                }

            }
        }
    }
}
