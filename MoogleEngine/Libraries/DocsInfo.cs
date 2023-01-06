using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    class DocsInfo
    {
        static int DocsTotal = 0;
        static HashSet<string> DocsNombre = new HashSet<string>();
        static Dictionary<string,int> InDocs = new Dictionary<string,int>();
        static Dictionary<string,Dictionary<string,double>> TF = new Dictionary<string,Dictionary<string,double>>();
        static Dictionary<string,double> IDF = new Dictionary<string,double>();
        static Dictionary<string,List<string>> DocsPalabras = new Dictionary<string,List<string>>();
        static Dictionary<string,List<string>> DocsPalabrasDiff = new Dictionary<string,List<string>>();
        static Dictionary<string,HashSet<string>> DocsPalabrasArr = new Dictionary<string,HashSet<string>>();

        public bool ExistenPalabras(string doc, string palabra)
        //devuelve un operador booleano al echo de que hayan o no palabras en el documento
        {
            return DocsPalabrasArr[doc].Contains(palabra);
        }

        public List<Tuple<string,double>> ObtenerTFIDF(string docname)
        //crea un vector donde a cada palabra le corresponde su TF*IDF
        {
            List<Tuple<string,double>> vect = new List<Tuple<string, double>>();
        
            foreach(var palabra in DocsPalabrasDiff[docname])
            {
                vect.Add(new Tuple<string,double>(palabra, TF[docname][palabra] * IDF[palabra]));
            }
            
            return vect;
        }

        public List<Tuple<string,List<Tuple<string,double>>>> ObtenerTodosTFIDF()
        //obtener el vector general todos los documentos con todas las palabras y sus TF*IDF( de las palabras)
        {
            List<Tuple<string,List<Tuple<string,double>>>> vect = new List<Tuple<string,List<Tuple<string,double>>>>();
        
            foreach(var doc in DocsNombre)
            {
                vect.Add(new Tuple<string, List<Tuple<string, double>>>(doc, ObtenerTFIDF(doc)));
            }
            
            return vect;
        }

        public List<Tuple<string,double>> ObtenerTFIDF(List<string> vect)
        //calcular el TF y el IDF segun sus fórmulas
        {
            Dictionary<string,int> Freq = new Dictionary<string,int>();

            foreach(var palabra in vect)
            {
                if(!Freq.ContainsKey(palabra))
                {
                    Freq.Add(palabra,0);
                }

                Freq[palabra]++;
            }

            List<Tuple<string,double>> arr = new List<Tuple<string,double>>();

            foreach(var word in Freq)
            {
                double tf = (double)Freq[word.Key]/(double)vect.Count;
                //la frecuencia de la palabra entre la cant de palabras del documento
                double idf = IDF[word.Key];//calcula en el constructor
                arr.Add(new Tuple<string, double>(word.Key, tf * idf));
            }

            return arr;
        }

        public DocsInfo(List<Tuple<string,List<string>>> arr)//Constructor de la clase( se calcula el idf de las palabras)
        {
            DocsTotal = arr.Count;

            foreach(var doc in arr)
            {
                Dictionary<string,int> Freq = new Dictionary<string,int>();
                HashSet<string> Diff = new HashSet<string>();

                foreach(var word in doc.Item2)
                {
                    if(!Diff.Contains(word))
                    {
                        Diff.Add(word);
                        Freq.Add(word,0);
                    }

                    Freq[word]++;
                }

                DocsPalabrasArr.Add(doc.Item1, Diff);

                foreach(var word in Diff)
                {
                    if(!InDocs.ContainsKey(word))
                    {
                        InDocs.Add(word, 0);
                    }

                    InDocs[word]++;
                }

                TF.Add(doc.Item1,new Dictionary<string,double>());

                foreach(var word in Diff)
                {
                    TF[doc.Item1].Add(word, (double)Freq[word]/(double)doc.Item2.Count);
                    //logaritmo de base 2 (de la cantidad de documentos / la cantidad de documentos en los que aparece la palabra)
                }

                DocsNombre.Add(doc.Item1);
                DocsPalabras.Add(doc.Item1, doc.Item2);
                DocsPalabrasDiff.Add(doc.Item1, new List<string>(Diff));
            }

            foreach(var word in InDocs)
            {
                IDF.Add(word.Key, Math.Log((double)DocsTotal/(double)InDocs[word.Key]) + 0.001);
            }
        }

        public DocsInfo()
        {
        }
    }
}