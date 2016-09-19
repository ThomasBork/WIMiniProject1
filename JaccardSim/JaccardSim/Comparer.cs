using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebCrawler
{
    public static class Comparer
    {
        private const int SHINGLE_LENGTH = 3;
        private const int SUPER_SHINGLE_LENGTH = 4;

        /// <summary>
        /// Compares two texts, by comparing all possible shingles using jaccard similarity.
        /// </summary>
        /// <param name="text1">The first text.</param>
        /// <param name="text2">The second text.</param>
        /// <returns>A value between 0 and 1, with 1 being completely similar</returns>
        public static double FullShingleCompare(string text1, string text2)
        {
            var shingles1 = Shinglify(text1);
            var shingles2 = Shinglify(text2);

            var overlap = shingles1.Intersect(shingles2);
            var union = shingles1.Union(shingles2);

            return (double)overlap.Count() / (double)union.Count();
        }


        /// <summary>
        /// Compares two texts, by their minimum result of a hash function for all shingles. Multiple hash functions are used for better approximation. 
        /// </summary>
        /// <param name="text1">The first text.</param>
        /// <param name="text2">The second text.</param>
        /// <returns>A value between 0 and 1, with 1 being completely similar</returns>
        public static double MinHashCompare(string text1, string text2)
        {
            var shingles1 = Shinglify(text1);
            var shingles2 = Shinglify(text2);

            var foundMatches = 0;
            var sketchLength = 5;

            for (int i = 0; i < sketchLength; i++)
            {
                var min1 = shingles1.Select(x => Hash(x, i)).Min();
                var min2 = shingles2.Select(x => Hash(x, i)).Min();

                foundMatches += (min1 == min2) ? 1 : 0;
            }

            return (double)foundMatches / (double)sketchLength;
        }

        /// <summary>
        /// Compares two texts, by finding their supershingles and finding the number of matches.
        /// </summary>
        /// <param name="text1">The first text.</param>
        /// <param name="text2">The second text.</param>
        /// <returns>A equal to, or higher than, 0, where it represents the number of supershingles of the texts that had a match</returns>
        public static double SuperShingleCompare(string text1, string text2)
        {
            var shingles1 = Shinglify(text1);
            var shingles2 = Shinglify(text2);

            var superShingles1 = SuperShinglify(shingles1);
            var superShingles2 = SuperShinglify(shingles2);

            var superHashes1 = superShingles1.Select(x => Hash(x, 1));
            var superHashes2 = superShingles2.Select(x => Hash(x, 1));

            return superHashes1.Intersect(superHashes2).Count();
        }

        /// <summary>
        /// Construct supershingles 
        /// </summary>
        /// <param name="shingles"></param>
        /// <returns></returns>
        private static List<List<int>> SuperShinglify(string[] shingles)
        {
            var ret = new List<List<int>>();

            var runs = 64;

            for (int i = 0; i < runs; i++)
            {
                if (i % SUPER_SHINGLE_LENGTH == 0)
                {
                    ret.Add(new List<int>());
                }

                ret[i / SUPER_SHINGLE_LENGTH].Add(shingles.Select(x => Hash(x, i)).Min());
            }

            return ret;
        }

        private static string[] Shinglify(string text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 ]");
            text = rgx.Replace(text, "");
            text = text.ToLower();
            List<string> temp = text.Split(' ').ToList();
            List<string> ret = new List<string>();

            if (temp.Count < SHINGLE_LENGTH)
            {
                throw new ArgumentOutOfRangeException("Must be more than " + SHINGLE_LENGTH + " words!");
            }

            for (int i = 0; i <= temp.Count-SHINGLE_LENGTH; i++)
            {
                string shingleString ="";
                for (int j = i; j < i + SHINGLE_LENGTH; j++)
                {
                    shingleString += temp[j];
                }
                ret.Add(shingleString);
            }

            return ret.ToArray();
        }

        #region HASH
        private static int Hash(string input, int seed)
        {
            Random rng = new Random(seed);
            return input.GetHashCode() * rng.Next();
        }

        private static int Hash(List<int> input, int seed)
        {
            Random rng = new Random(seed);
            List<long> longList = input.Select(x => (long)x).ToList();
            return (int)(longList.Sum() * (long)rng.Next());
        }
        #endregion
    }
}
