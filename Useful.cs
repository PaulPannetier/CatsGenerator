#region using

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

#endregion

#region Random

public static class Random
{
    private static System.Random random = new System.Random();
    private static readonly float twoPi = 2f * 3.14159274f;

    #region Seed

    public static void SetSeed(in int seed)
    {
        random = new System.Random(seed);
    }
    /// <summary>
    /// randomize de seed of the random, allow to have diffenrent random number at each lauch of the game
    /// </summary>
    public static void SetRandomSeed()
    {
        SetSeed((int)DateTime.Now.Ticks);
    }

    #endregion

    #region Random Value and vector

    /// <returns> A random integer between a and b, [|a, b|]</returns>
    public static int Rand(in int a, in int b) => random.Next(a, b + 1);
    /// <returns> A random float between 0 and 1, [0, 1]</returns>
    public static float Rand() => (float)random.Next(int.MaxValue) / (int.MaxValue - 1);
    /// <returns> A random float between a and b, [a, b]</returns>
    public static float Rand(in float a, in float b) => Rand() * Math.Abs(b - a) + a;
    /// <returns> A random int between a and b, [|a, b|[</returns>
    public static int RandExclude(in int a, in int b) => random.Next(a, b);
    /// <returns> A random double between a and b, [a, b[</returns>
    public static float RandExclude(in float a, in float b) => (float)random.NextDouble() * (Math.Abs(b - a)) + a;
    public static float RandExclude() => (float)random.NextDouble();
    public static Color Color() => System.Drawing.Color.FromArgb(Rand(0, 255), Rand(0, 255), Rand(0, 255));
    /// <returns> A random color with a random opacity</returns>
    public static Color ColorTransparent() => System.Drawing.Color.FromArgb(Rand(0, 255), Rand(0, 255), Rand(0, 255), Rand(0, 255));

    #endregion

    #region Proba laws

    //Generer les lois de probas ! fonction non tester
    public static int Bernoulli(in float p) => Rand() <= p ? 1 : 0;
    public static int Binomial(in int n, in int p)
    {
        int count = 0;
        for (int i = 0; i < n; i++)
            count += Bernoulli(p);
        return count;
    }
    public static float Expodential(in float lambda) => (-1f / lambda) * (float)Math.Log(Rand());
    public static int Poisson(in float lambda)
    {
        float x = Rand();
        int n = 0;
        while (x > Math.Exp(-lambda))
        {
            x *= Rand();
            n++;
        }
        return n;
    }
    public static int Geometric(in float p)
    {
        int count = 0;
        do
        {
            count++;
        } while (Bernoulli(p) == 0);
        return count;
    }
    private static float N01() => (float)(Math.Sqrt(-2f * Math.Log(Rand())) * Math.Cos(twoPi * Rand()));
    public static float Normal(in float esp, in float sigma) => N01() * sigma + esp;

    #endregion

}

#endregion

#region 2DArray

[Serializable]
public class Array2D<T>
{
    public T[] array;
    public int width, height;

    public Array2D(in int width, in int height)
    {
        this.width = width; this.height = height;
        array = new T[width * height];
    }

    public T this[int line, int column]
    {
        get
        {
            return array[column + line * width];
        }
        set
        {
            array[column + line * width] = value;
        }
    }

    public T[,] ToArray()
    {
        T[,] res = new T[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                res[i, j] = array[j + i * width];
            }
        }
        return res;
    }

    public List<List<T>> ToList()
    {
        List<List<T>> res = new List<List<T>>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                res[i][j] = array[j + i * width];
            }
        }
        return res;
    }
}

#endregion

#region Useful

public static class Useful
{
    #region Color

    /// <summary>
    /// Return a Color deeper than the color in argument
    /// </summary>
    /// <param name="c">The color to change</param>
    /// <param name="percent">le coeff €[0, 1] d'assombrissement</param>
    /// <returns></returns>
    public static Color ColorDeeper(in Color c, float coeff) => Color.FromArgb(c.A, (int)(c.R * (1f - coeff)), (int)(c.G * (1f - coeff)), (int)(c.B * (1f - coeff)));

    #endregion

    #region Vector and Maths

    /// <summary>
    /// t € [0, 1]
    /// </summary>
    public static int Lerp(in int a, in int b, float t) => (int)(a + (b - a) * t);
    public static float Lerp(in float a, in float b, float t) => a + (b - a) * t;

    public static decimal Sqrt(in decimal x)
    {
        if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

        decimal current = (decimal)Math.Sqrt((double)x), previous;
        do
        {
            previous = current;
            if (previous == 0.0M) return 0;
            current = (previous + x / previous) * 0.5m;
        }
        while (Math.Abs(previous - current) > 0.0M);
        return current;
    }

    public static decimal Abs(in decimal x) => x >= 0m ? x : -x;

    #endregion

    #region Array

    public static T GetRandom<T>(this T[] array) => array[Random.RandExclude(0, array.Length)];

    public static T[,] CloneArray<T>(this T[,] Array)
    {
        T[,] a = new T[Array.GetLength(0), Array.GetLength(1)];
        for (int l = 0; l < a.GetLength(0); l++)
        {
            for (int c = 0; c < a.GetLength(1); c++)
            {
                a[l, c] = Array[l, c];
            }
        }
        return a;
    }

    /// <summary>
    /// Retourne le sous tableau de Array, cad Array[IndexStart]
    /// </summary>
    /// <param name="indexStart">l'index de la première dimension de Array</param>
    public static T[,,] GetSubArray<T>(this T[,,,] Array, in int indexStart = 0)
    {
        T[,,] a = new T[Array.GetLength(1), Array.GetLength(2), Array.GetLength(3)];
        for (int l = 0; l < a.GetLength(0); l++)
        {
            for (int c = 0; c < a.GetLength(1); c++)
            {
                for (int i = 0; i < a.GetLength(2); i++)
                {
                    a[l, c, i] = Array[indexStart, l, c, i];
                }
            }
        }
        return a;
    }

    public static int GetIndexOf<T>(this T[] arr, T value)  where T : IComparable
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if(arr[i].CompareTo(value) == 0)
                return i;
        }
        return -1;
    }

    public static bool Exist<T>(this T[,] tab, int l, int c) => l >= 0 && c >= 0 && l < tab.GetLength(0) && c < tab.GetLength(1);

    public static T[] Merge<T>(this T[] arr, in T[] other)
    {
        T[] res = new T[arr.Length + other.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            res[i] = arr[i];
        }
        for (int i = arr.Length; i < res.Length; i++)
        {
            res[i] = other[i - arr.Length];
        }
        return res;
    }

    public static List<T> Merge<T>(this List<T> lst, List<T> other)
    {
        List<T> res = new List<T>();
        for (int i = 0; i < lst.Count; i++)
        {
            res.Add(lst[i]);
        }
        for (int i = 0; i < other.Count; i++)
        {
            res.Add(other[i]);
        }
        return res;
    }

    #region Show

    public static void ShowArray<T>(this T[] tab, string begMessage = "", string endMessage = "")
    {
        Console.WriteLine(begMessage + tab.ToString<T>() + endMessage);
    }

    public static string ToString<T>(this T[] arr)
    {
        StringBuilder sb = new StringBuilder("[ ");
        for (int l = 0; l < arr.Length; l++)
        {
            sb.Append(arr[l].ToString());
            sb.Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(" ]");
        return sb.ToString();
    }

    public static void ShowArray<T>(this T[,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text = "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += tab[l, c].ToString() + ", ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ],";
            Console.WriteLine(text);
        }
    }
    public static void ShowArray<T>(this T[,,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text += "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += "[ ";
                for (int i = 0; i < tab.GetLength(2); i++)
                {
                    text += tab[l, c, i].ToString() + ", ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ], ";
        }
        text = text.Remove(text.Length - 3, 3);
        text += "]";
        Console.WriteLine(text);
    }
    public static void ShowArray<T>(this T[,,,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text += "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += "[ ";
                for (int i = 0; i < tab.GetLength(2); i++)
                {
                    text += "[ ";
                    for (int j = 0; j < tab.GetLength(3); j++)
                    {
                        text += tab[l, c, i, j].ToString() + ", ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ], ";
        }
        text = text.Remove(text.Length - 3, 3);
        text += "]";
        Console.WriteLine(text);
    }
    public static void ShowArray<T>(this T[,,,,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text += "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += "[ ";
                for (int i = 0; i < tab.GetLength(2); i++)
                {
                    text += "[ ";
                    for (int j = 0; j < tab.GetLength(3); j++)
                    {
                        text += "[ ";
                        for (int k = 0; k < tab.GetLength(4); k++)
                        {
                            text += tab[l, c, i, j, k].ToString() + ", ";
                        }
                        text = text.Remove(text.Length - 2, 2);
                        text += " ], ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ], ";
        }
        text = text.Remove(text.Length - 3, 3);
        text += "]";
        Console.WriteLine(text);
    }

    #endregion

    #endregion

    #region Normalise Array

    /// <summary>
    /// Normalise tout les éléments de l'array pour obtenir des valeur entre 0f et 1f, ainse le min de array sera 0f, et le max 1f.
    /// </summary>
    /// <param name="array">The array to normalised</param>
    public static void NormaliseArray(this float[] array)
    {
        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < min)
                min = array[i];
            if (array[i] > max)
                max = array[i];
        }
        float maxMinusMin = max - min;
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (array[i] - min) / maxMinusMin;
        }
    }

    /// <summary>
    /// Change array like the sum of each element make 1f
    /// </summary>
    /// <param name="array">the array to change, each element must to be positive</param>
    public static void GetProbabilityArray(this float[] array)
    {
        float sum = 0f;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < 0f)
            {
                return;
            }
            sum += array[i];
        }
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= sum;
        }
    }
    #endregion

    #region Shuffle

    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count; i >= 0; i--)
        {
            int j = Random.Rand(0, i);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
    /// <summary>
    /// Shuffle a little bit the list, reproduce approximately the real life
    /// </summary>
    /// <param name="percentage">The percentage to shuffle between 0 and 1</param>
    public static void ShufflePartialy<T>(this List<T> list, in float percentage)
    {
        int nbPermut = (int)(list.Count * percentage);
        for (int i = 0; i < nbPermut; i++)
        {
            int randIndex1 = Random.RandExclude(0, list.Count);
            int randIndex2 = Random.RandExclude(0, list.Count);
            T temp = list[randIndex1];
            list[randIndex1] = list[randIndex2];
            list[randIndex2] = temp;
        }
    }

    #endregion

    #region Parse

    private static string[] letter = new string[36] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    private static string Troncate(string mot)
    {
        string newMot = mot;
        for (int i = 0; i < mot.Length; i++)
        {
            string s = mot.Substring(i, 1);
            if (s == "," || s == ".")
            {
                newMot = newMot.Remove(i, mot.Length - i);
                break;
            }
        }
        return newMot;
    }

    public static int ConvertStringToInt(string number)
    {
        int nb = 0;
        number = Troncate(number);
        for (int i = number.Length - 1; i >= 0; i--)
        {
            string carac = number.Substring(i, 1);
            for (int j = 26; j <= 35; j++)
            {
                if (carac == letter[j])
                {
                    int n = j - 26;
                    nb += n * (int)Math.Pow(10, number.Length - 1 - i);
                    break;
                }
            }
        }
        if (number.Substring(0, 1) == "-")
            nb *= -1;

        return nb;
    }

    public static float ConvertStringToFloat(string number)
    {
        float result = 0f;
        string partieEntiere = number;
        string partieDecimal = "";

        int indexComa = 0;
        for (int i = 0; i < number.Length; i++)
        {
            string s = number.Substring(i, 1);
            if (s == "," || s == ".")
            {
                partieDecimal = partieEntiere.Substring(i + 1, partieEntiere.Length - (i + 1));
                partieEntiere = partieEntiere.Remove(i, partieEntiere.Length - i);
                indexComa = i;
                break;
            }
        }
        //part entière
        result = ConvertStringToInt(partieEntiere);
        //part decimal
        for (int i = 0; i < partieDecimal.Length; i++)
        {
            string carac = partieDecimal.Substring(i, 1);
            for (int j = 26; j <= 35; j++)
            {
                if (carac == letter[j])
                {
                    int n = j - 26; //n € {0,1,2,3,4,5,6,7,8,9}
                    result += n * (float)Math.Pow(10, -(i + 1));
                    break;
                }
            }
        }
        return result;
    }

    #endregion

    #region List

    public static T GetRandom<T>(this List<T> lst) => lst[Random.RandExclude(0, lst.Count)];

    public static T Last<T>(this List<T> lst) => lst[lst.Count - 1];

    /// <summary>
    /// Retourne lst1 union lst2
    /// </summary>
    /// <param name="lst1">La première liste</param>
    /// <param name="lst2">La seconde liste</param>
    /// <param name="doublon">SI on autorise ou pas les doublons</param>
    /// <returns></returns>     
    public static List<T> SumList<T>(this List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
    {
        return SumList(lst1, lst2);
    }

    public static List<T> SumList<T>(in List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
    {
        List<T> result = new List<T>();
        foreach (T nb in lst1)
        {
            if (doublon || !result.Contains(nb))
                result.Add(nb);
        }
        foreach (T nb in lst2)
        {
            if (doublon || !result.Contains(nb))
                result.Add(nb);
        }
        return result;
    }

    public static void Add<T>(this List<T> lst1, in List<T> lstToAdd, bool doublon = false)//pas de doublon par defaut
    {
        if (doublon)
        {
            foreach (T element in lstToAdd)
            {
                lst1.Add(element);
            }
        }
        else
        {
            foreach (T element in lstToAdd)
            {
                if (lst1.Contains(element))
                {
                    continue;
                }
                lst1.Add(element);
            }
        }

    }

    public static void Show<T>(this List<T> lst)
    {
        StringBuilder sb = new StringBuilder("[");
        for (int i = 0; i < lst.Count; i++)
        {
            sb.Append((lst[i] == null ? "null" : lst[i].ToString()) + ", ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        Console.WriteLine(sb.ToString());
    }

    #endregion
}

#endregion