using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    public class Moogle
    {
        static Diccionario Dic = new Diccionario();
        static VEspacio Vspace = new VEspacio();
        static DocsInfo DInfo = new DocsInfo();

        public static void Init()
        /*metodo encargado de ejecutar todos los demás necesarios para crear la base de datos del programa
         el cual se corre cuando se carga el servidor(en Programs)*/

        {
            List<string> nombres = Lectura.ObtenerDirectoriosArchivos("../Content/");
            
            List<Tuple<string,List<string>>> Docs = new List<Tuple<string,List<string>>>();

            foreach(var nombre in nombres)
            {
                List<string> vect = Lectura.ObtenerPalabrasString(Lectura.ObtenerStringArchivo("../Content/" + nombre));

                if(vect.Count == 0)
                {
                    continue;
                }

                foreach(var palabra in vect)
                {
                    Dic.Insertar(palabra);
                }

                Docs.Add(new Tuple<string, List<string>>(nombre, vect));
            }

            DInfo = new DocsInfo(Docs);

            Vspace = new VEspacio(DInfo.ObtenerTodosTFIDF());
        }

        public static Tuple<string,string,double> EncontrarTexto (string doc, double val, List<string> vect, Operadores Op)
        //SNIPET
        {
            string DocText = Lectura.ObtenerStringArchivo("../Content/" + doc);//obtener el texto del documento

            List<Tuple<string,int>> PalabrasDocs = Lectura.ObtenerPalabrasyPosicionesString(DocText);
            //palabras documento

            if(PalabrasDocs.Count == 0)
            {
                return new Tuple<string, string, double>(doc, "", val);
            }

            VQuery Qu = new VQuery(DInfo.ObtenerTFIDF(vect));

            int PieceSize = Math.Max(vect.Count*3, 15);// dividir el documentos en pedazos

            double MejorVal = -1;
            int MejorComienzo = -1;
            int MejorFinal = -1;

            int NumberOfPairWords = 0;

            for(int i = 0 ; i < PalabrasDocs.Count ; i += PieceSize/6)
            {
                List<string> Piece = new List<string>();

                int Start = PalabrasDocs[i].Item2;
                int End = -1;

                for(int j = i ; j < Math.Min(i+PieceSize, PalabrasDocs.Count) ; j++)
                {
                    Piece.Add(PalabrasDocs[j].Item1);
                    End = PalabrasDocs[j].Item2 + PalabrasDocs[j].Item1.Length - 1;
                }

                VQuery VPiece = new VQuery(DInfo.ObtenerTFIDF(Piece));

                foreach(var P in Op.ParejaPalabra)
                {
                    if(VPiece.VectorPalabra.ContainsKey(P.Item1) && VPiece.VectorPalabra.ContainsKey(P.Item2))
                    {
                        NumberOfPairWords++;
                    }
                }

                double TempVal = VPiece.SimilitudCoseno(Qu, Op);
                // obtener la similitud de cosenos entre la query i el snnipet para obtener el mejor snnipet posible

                if(TempVal > MejorVal)
                {
                    MejorVal = TempVal;
                    MejorComienzo = Start;
                    MejorFinal = End;
                }

                if(Math.Min(i+PieceSize, PalabrasDocs.Count) == PalabrasDocs.Count)
                {
                    break;
                }
            }

            return new Tuple<string, string, double>(doc, DocText.Substring(MejorComienzo, MejorFinal-MejorComienzo+1), val * MejorVal * (Math.Log(NumberOfPairWords+1) + 1));
        }

        public static SearchResult Query(string query)
        //metodo que nos pidieron modificar para mandar los resultados de la busqueda
        {
            List<string> vect = Lectura.ObtenerPalabrasString(query);
            //convertir la query en una lista

            if(vect.Count == 0)
            {
                return new SearchResult();
            }

            for(int i = 0 ; i < vect.Count ; i++)
            {
                vect[i] = Dic.EncontrarPalabra(vect[i])[0].Item1;
            }

            query = Lectura.ArreglarQueryPalabras(query, vect);
            //la sugerencia de busqueda

            Operadores Op = new Operadores(query);//operadores de la query

            VQuery Qu = new VQuery(DInfo.ObtenerTFIDF(vect));//vector query

            List<Tuple<string,double>> arr = Vspace.EncontarVectoresSimilares(Qu, DInfo, Op);
            //los mejores documentos que responder a es busqueda

            List<Tuple<string,string,double>> ans = new List<Tuple<string, string, double>>();

            foreach(var doc in arr)
            {
                ans.Add(EncontrarTexto(doc.Item1, doc.Item2, vect, Op));
            }

            ans.Sort((x,y) => y.Item3.CompareTo(x.Item3));

            SearchItem[] items = new SearchItem[arr.Count];

            for(int i = 0 ; i < ans.Count ; i++)//resultados de busqueda
            {
                items[i] = new SearchItem(ans[i].Item1 + "\n" + "Score: " + ans[i].Item3, ans[i].Item2, (float)ans[i].Item3);
            }

            return new SearchResult(items, query);
        }
    }
}
