using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene
{
    class HelloWorldExample
    {
        private List<string> PullAbstracts(string source)
        {
            //return new List<string>(new string[] { "the quick brown fox jumps over the lazy dog", "the quick brown fox jumps over the lazy dog" }); 

            string[] lines = File.ReadAllLines(source);

            bool readingAbstract = false;
            StringBuilder sb = null;
            List<string> abstracts = new List<string>();
            foreach (string line in lines)
            {
                if (line.StartsWith("AB  - "))
                {
                    readingAbstract = true;
                    sb = new StringBuilder(1024);
                    sb.Append(line.Substring("AB  - ".Length));
                }

                if (readingAbstract)
                {
                    if (line.StartsWith("ER  -") || line.Length == 0)
                    {
                        readingAbstract = false;
                        abstracts.Add(sb.ToString().ToLower());
                    }
                    else
                    {
                        sb.Append(line);
                    }
                }
            }
            return abstracts;
        }
        public void Main()
        {
            List<string> abstracts = PullAbstracts(@"ExampleData\references.txt");

            Stopwords sw = new Stopwords(@"ExampleData\Stopwords.txt");

            StandardAnalyzer analyzer = new StandardAnalyzer(Net.Util.Version.LUCENE_30);
            using (IndexWriter writer = new IndexWriter(FSDirectory.Open(@"ExampleData\Index"),
                analyzer, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (string ab in abstracts)
                {
                    string trimmedAb = sw.TrimStopwords(ab);
                    if (trimmedAb.Contains("here "))
                        return; 
                    Document document = new Document();
                    Field field = new Field("content", trimmedAb, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    document.Add(field);

                    writer.AddDocument(document);
                }
                writer.Optimize();
            }

            List<Dictionary<string, int>> vectors = new List<Dictionary<string, int>>();
            using (IndexReader reader = IndexReader.Open(FSDirectory.Open(@"ExampleData\Index"), true))
            {
                // get all terms. 
                TermEnum termsEnum = reader.Terms();
                List<string> termStrings = new List<string>();
                while (termsEnum.Next())
                {
                    string term = termsEnum.Term.Text;
                    string processed = null; 
                    if (!sw.IsStopWord(term, out processed))
                    {
                        termStrings.Add(processed);
                    }
                }

                // create vectors
                for (int c = 0; c < reader.NumDocs(); c++)
                {
                    Dictionary<string, int> vector = new Dictionary<string, int>(); 
                    foreach (string term in termStrings)
                    {
                        vector.Add(term, 0); 
                    }

                    ITermFreqVector freqVector = reader.GetTermFreqVector(c, "content");
                    string[] terms = freqVector.GetTerms();
                    foreach (string term in terms)
                    {
                        int count = vector[term];
                        vector[term] = count + 1;
                    }
                    vectors.Add(vector); 
                }
            }

            StringBuilder sb = new StringBuilder(1024); 
            foreach (Dictionary<string, int> vector in vectors)
            {
                double similarity = CosineSimilarity.Compute(vectors[1], vector);
                sb.AppendLine(similarity.ToString()); 
            }
        }
    }
}
