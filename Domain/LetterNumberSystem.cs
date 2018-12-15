using System;
using System.Collections.Generic;

namespace BLL
{
    public class LetterNumberSystem
    {
        public Dictionary<char, int> LetterValues = new Dictionary<char, int>();
        public Dictionary<int, string> NumValues = new Dictionary<int, string>();

        public LetterNumberSystem()
        {
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (var i = 0; i < alphabet.Length; i++)
            {
                var c = alphabet[i];
                LetterValues.Add(c, i + 1);
                NumValues.Add(i + 1 , c.ToString());
            }

            foreach (char c2 in alphabet)
            {
                foreach (char c1 in alphabet)
                {
                    NumValues.Add(GetNumberFromLetters(c2.ToString() + c1.ToString()), c2.ToString() + c1.ToString());
                }
            }
            
            
        }

        public string GetLetter(int number)
        {
            if (number > 702)
            {
                throw new ArgumentException("Number too big");
            }
            return NumValues[number];

        }
        
        /**
         * returns a number from the letter
         * i.e - A returns 1, B returns 2, Z returns 26
         */
        public int GetNumberFromLetters(string letters)
        {
            if (string.IsNullOrEmpty(letters))
            {
                throw new ArgumentException("Empty or null string");
            }
            double resultDouble = 0;
            int power = 0, result = 0; 
            for (int i = letters.Length - 1; i >= 0; i--)
            {
                resultDouble += Math.Pow(26, power++) * LetterValues[letters[i]];
            }

            if (int.TryParse(resultDouble.ToString(), out result)) return result;
            throw new ArgumentException("Number too big for int");
        }
    }
}