using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    class VEspacio
    {
        static int MejoresDocsLimite = 5;
        Dictionary<string,VQuery> DocVector = new Dictionary<string,VQuery>();

        public VEspacio(List<Tuple<string,List<Tuple<string,double>>>> Vspace)//constructor de la clase
        {
            foreach(var doc in Vspace)
            {
                DocVector.Add(doc.Item1, new VQuery(doc.Item2));
                // guarda el vector asociado a cada documento
            }
        }

        public VEspacio()
        {
        }

        public List<Tuple<string,double>> EncontarVectoresSimilares(VQuery vec, DocsInfo Dinfo, Operadores Op)
        //calcular la similitud del coseno pero teniendo en cuenta los operadores de existencia 
        {
            List<Tuple<string,double>> MejorDocs = new List<Tuple<string,double>>();

            foreach(var doc in DocVector)
            {   
                bool flag = true;

                foreach(var palabra in Op.TieneExistirPalabra)//operador "tiene que aparecer"
                {
                    if(Dinfo.ExistenPalabras(doc.Key, palabra) == false)
                    {
                        flag = false;
                    }
                }

                foreach(var palabra in Op.NopuedeExistirPalabra)//operador "no debe aparecer"
                {
                    if(Dinfo.ExistenPalabras(doc.Key, palabra) == true)
                    {
                        flag = false;
                    }
                }

                if(flag == false)
                {
                    continue;
                }

                double sc = doc.Value.SimilitudCoseno(vec, Op);//score(se mada a calcular la similitud del coseno)

                int pos = 0;

                for(int i = 0 ; i < MejorDocs.Count ; i++)
                {
                    if(sc < MejorDocs[i].Item2)
                    {
                        pos++;
                    }
                    else break;
                }

                MejorDocs.Insert(pos, new Tuple<string,double>(doc.Key, sc));
                //la lista de los documentos ordenados por score

                if (MejorDocs.Count > MejoresDocsLimite)
                {
                    MejorDocs.RemoveAt(MejorDocs.Count-1);
                    // escoge en este caso solo los 5 mejores primeros
                }
            }

            return MejorDocs;
        } 
    }
}