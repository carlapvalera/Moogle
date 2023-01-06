using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    class VQuery
    {
        public Dictionary<string,double> VectorPalabra = new Dictionary<string,double>();
        double SumaRaiz = 0;
        public VQuery(List<Tuple<string,double>> vect)//constructor de la clase
        {
            foreach(var palabra in vect)
            {
                this.VectorPalabra.Add(palabra.Item1, palabra.Item2);
                this.SumaRaiz += palabra.Item2 * palabra.Item2;
            }
        }

        public double SimilitudCoseno(VQuery vec, Operadores Op)
        // es una formula para determinar que tan parecidos(similares) son dos vectores entre si 
        {
            double sum = 0;

            foreach(var element in vec.VectorPalabra)
            {
                if(this.VectorPalabra.ContainsKey(element.Key))
                {
                    sum += element.Value * this.VectorPalabra[element.Key] * Op.ImportanciaPalabra[element.Key];
                                                                             //operador de "importancia"(multiplica por 2la cant de veces q este aparece)
                }
            }

            sum /= Math.Sqrt(vec.SumaRaiz * this.SumaRaiz);
            
            return sum;
        }
    }
}