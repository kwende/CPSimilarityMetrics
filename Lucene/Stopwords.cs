using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene
{
    public class Stopwords
    {
        List<string> _stopWords = new List<string>();
        public Stopwords(string sourceFile)
        {
            _stopWords.AddRange(File.ReadAllLines(sourceFile));
        }

        public bool IsStopWord(string input, out string output)
        {
            output = null; 

            // remove characters that can be attached to words, making them "different"
            StringBuilder trimmedBit = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                // if there is a digit in the string, it's bad. 
                // so reject immediately. 
                if (char.IsDigit(c))
                {
                    return true;
                }

                if(char.IsLetter(c) || c =='\'')
                {
                    trimmedBit.Append(c);
                }
            }

            output = trimmedBit.ToString();
            double parsedValue = 0.0;
            if (!double.TryParse(output, out parsedValue) && !_stopWords.Contains(output))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string TrimStopwords(string input)
        {
            StringBuilder bs = new StringBuilder(102400);
            string[] bits = input.Split(' ');
            foreach (string bit in bits)
            {
                string processed = null;
                if (!IsStopWord(bit, out processed))
                {
                    bs.AppendFormat("{0} ", processed);
                }
            }
            return bs.ToString();
        }
    }
}
