using ReedSolomon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjetInfo
{
    /// <summary>
    /// Interaction logic for QRcode.xaml
    /// </summary>
    public partial class QRcode : Page
    {
        public QRcode()
        {
            InitializeComponent();
        }

        public int StringToNum(char val)
        {
            int resultat = 0;

            switch (val)
            {
                case ' ':
                    return 36;
                case '$':
                    return 37;
                case '%':
                    return 38;
                case '*':
                    return 39;
                case '+':
                    return 40;
                case '-':
                    return 41;
                case '/':
                    return 42;
                case ':':
                    return 43;
                default:
                    resultat = (((int)val % 32) + 9);
                    return resultat;
            }
        }

        public string DecimalToBytes(int valdeciaml, int bytesNecessaire) //Pour decimal -> binaire, en ajoutant 0 avant selon les necessités
        {
            string bytes = Convert.ToString(valdeciaml, 2);

            //ajouter les 0 à gauche pour faire zeroAvant bytes
            string zeros = "";
            for (int i = 0; i < bytesNecessaire - bytes.Length; i++)
            {
                zeros += "0";
            }
            bytes = zeros + bytes;

            return bytes;
        }

        private void CreerQR_Click(object sender, RoutedEventArgs e)
        {
            // Alphanumerique donc 0010
            //Convertir nombre de caracteres à 9 bytes binaire
            //Determiner Version selon n° de carac. : V1 - max 25cars  ET V2 - max 47cars
            //Convertir texte à 9 bytes binaire
            //

            int nombreCaracs = qrText.Text.Length;

            string val = DecimalToBytes(nombreCaracs, 9);

            test.Text = val + " ";


            //Convertir Text en 11 bits//
            //Couper Texte en couples de 2
            string textqr = qrText.Text; 
            textqr = textqr.ToUpper();
            var coupleLettres = new List<string>();  //contient {"He", "ll", "o ", "wo", "rl", "d"}
            int compt = 0;
            int len = textqr.Length/2;

            bool finUnique = false; // pour savoir si le dernier "couple" a qu'une seule valeur

            if(textqr.Length%2 != 0)
            {
                len = len + 1;
                textqr += "0"; // caractere temporaire
                finUnique = true;
            }
            for(int i = 0; i < len; i++)
            {
                coupleLettres.Add(textqr[compt] + "" + textqr[compt + 1]);
                compt = compt + 2;
            }
            coupleLettres[coupleLettres.Count - 1] = Convert.ToString(coupleLettres[coupleLettres.Count - 1][0]);

            //Calculs LETTRE COUPLE ==> DECIMAL NUMERIQUE (fin page 18) // ex : HE = 45^1*17 + 45^0 * 14 = 779
            var resultatDecimal = new List<double>(); //va conenir 779,966... par exemple

            foreach (string txt in coupleLettres)
            {
                if(txt.Length == 2)
                {
                    resultatDecimal.Add(Math.Pow(45, 1)* StringToNum(txt[0]) + Math.Pow(45, 0) * StringToNum(txt[1]));
                }
                else
                {
                    resultatDecimal.Add(Math.Pow(45, 0) * StringToNum(txt[0]));
                    
                }
            }
            //test.Text = resultatDecimal[2] + " ";

            //Calculs DECIMAL NUM. ==> BINAIRE 11 Bites (fin page 18) // ex :  779 -> 01100001011 

            var resultatBytes = new List<string>(); //va conenir 779,966... par exemple
            for(int i = 0; i < resultatDecimal.Count; i++)
            {
                int valdecimal = Convert.ToInt32(resultatDecimal[i]);

                string valbytes = DecimalToBytes(valdecimal, 11); //11 bytes necessaires 

                if(finUnique && i == (resultatDecimal.Count - 1)) //car dernier valeur UNIQUE (si existe) prends que 6 bytes
                {
                    valbytes = DecimalToBytes(valdecimal, 6);
                }

                resultatBytes.Add(valbytes);
            }
            
            test.Text = resultatBytes[2] + " ";


            // Maintenant on a tous les binaires de notre phrase dans resultatBytes


            //Maintenant, commencons à traiter notre donnes binaire

            // Nombre bits souhaité : 152 bits

            //avec he ll o  wo rl d    on a : 11 + 11 + 11 + 11 + 11 + 6 = 61

            //int longeurBinaire = 0;
            List<char> bytesContinu = new List<char>(); //Contient 0110000101101111000110 ...

            string bytesDonnees = "";

            foreach (string bin in resultatBytes) //on prend 01000100
            {
                bytesDonnees += bin;
            }

            //ajouter 0 à la fin (4 maximum) si < 152
            for(int i = 0; i < 4; i++)
            {
                if(bytesDonnees.Length < 152)
                {
                    bytesDonnees += "0";
                }
            }

            //nombre de bits est un MULTIPLE DE 8 ?
            //Sinon ajoute encore des 0

            while (bytesDonnees.Length % 8 != 0)
            {
                bytesDonnees += "0";
            }

            // Toujours pas 152 ?
            // Ajouter 11101100 00010001 (236 et 17) des specifications QRcode pour remplir vide

            while (bytesDonnees.Length < 152) 
            {
                bytesDonnees += "1110110000010001"; 
            }


            // Maintenant REED SOLOMONS

            // Le message final est de forme : bytedonnees + ERREUR CORREC de REEDS
            // Donc faisons Reed Solomons

            Encoding u8 = Encoding.UTF8;
            string a = "HELLO WORLD";
            byte[] bytesa = u8.GetBytes(a);
            //byte[] result = ReedSolomonAlgorithm.Encode(bytesa, 7);
            //Privilégiez l'écriture suivante car par défaut le type choisi est DataMatrix 
            byte[] result = ReedSolomonAlgorithm.Encode(bytesa, 7, ErrorCorrectionCodeType.QRCode);



            string message = bytesDonnees; // pas necessaire mais on creer un nouveau string pour bytesdonnes + corrections 
            foreach (byte i in result)
            {
                message += DecimalToBytes(i, 8); //8 car on veut un octet pour chaque mot de corrections
            }


            //Test
            string teststring = "";
            foreach(char i in bytesContinu)
            {
                teststring += i + " ";
            }
            test.Text = message.Length + "";





            //Changement de Version selon n° caracs.
            if (nombreCaracs <= 25)
            {
                //version 1
            }
            else
            {
                //version 2
            }

            //char c= Convert.ToChar(qrText.Text);
            //test.Text = StringToNum('H') + " ";
        }
    }
}
