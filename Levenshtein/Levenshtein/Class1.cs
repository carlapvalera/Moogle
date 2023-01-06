namespace Levenshtein
{
    public class Class1
    {
        public int DistanciaL ( string s, string t)
        /* La Distancia de Levenshtein es el número mínimo de operaciones requeridas para transformaruna cadena de
         caracteres en otra. Se entiende por operación; insercion, eliminación o la sustitución de un carácter*/
        {
            int ls = s.Length;  
            int lt = t.Length;
            int[,] distancia = new int[ls + 1, lt + 1];
            int peso = 0;
            int costo = 0;
            //tabla de de la longitud de s+1 folas y de la longitud de t +1 columnas

            //verificar que estamos comparando dos palabras o q hay algo con lo  para comparar
            if (lt == 0) return ls;
            if (ls == 0) return lt;

            // rellenar la primera columna y la primera fila de la tabla
            for (int i = 0; i < ls; i++)
            {
                distancia[i, 0] = i++;
            }
            for (int j = 0; j < lt; j++)
            {
                distancia[0, j] = j++;
            }
            
            //llenar los demas espacios de la matriz, es decir las i columnas restantes y los j filas
            for (int i = 1; i < ls; i++)
            {
                for (int j = 1;j < lt; j++)
                {
                    // si son iguales en posiciones equidistantes el peso es 0 de lo contrario el peso suma a 1
                    if(s[i-1] ==t[j-1])
                        peso = distancia[i-1, j-1];  
                    else
                        peso = distancia[i-1,j-1] +1; 

                    distancia[i,j]= Math.Min(Math.Min(distancia[i-1,j]+1, distancia[i,j-1]+1), peso);  
                                                      //Eliminacion         Insercion          Sustitucion
                } 

            }

            return distancia[ls, lt];
        }

    }
}