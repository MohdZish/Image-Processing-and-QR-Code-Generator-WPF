using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetInfo
{
    public class Complexe
    {
        public double a; //nombre reel
        public double b; //nombre complexe

        public Complexe(double a, double b)
        {
            this.a = a;
            this.b = b;
        }
        public void CarreComplexe() //Carre d'un complexe (a+ib)^2 = (a2 - b2) + 2ab
        {
            double temp = (a * a) - (b * b);
            b = 2.0 * a * b;
            a = temp;
        }

        public double Module() // Module d'un complexe: |a+ib| = Racine(a2 + b2)
        {
            return Math.Sqrt((a * a) + (b * b));
        } 

        public void AjouterComplexe(Complexe c)
        {
            a += c.a;
            b += c.b;
        }
    }
}
