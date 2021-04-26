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
using System.Drawing;
using System.IO;

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
            
            //test.Text = resultatBytes[2] + " ";


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
            // Donc faisons Reed Solomons pour trouver les bytes de corrections
            Encoding u8 = Encoding.UTF8;

            string a = qrText.Text; // on prend le text input
            byte[] bytesa = u8.GetBytes(a); // on le converti en bytes
            byte[] result = ReedSolomonAlgorithm.Encode(bytesa, 7, ErrorCorrectionCodeType.QRCode); // reed solomons qui nous donne par exemple 296 76 25 ...

            // CORRECTIONS 7 donc minimum 8 caracteres necessaires dans INPUT

            string message = bytesDonnees; // pas necessaire mais on creer un nouveau string pour bytesdonnes + corrections 

            foreach (byte i in result)
            {
                message += DecimalToBytes(i, 8); //8 car on veut un octet pour chaque mot de corrections
            }

            // Read Solomon marche maintenant
            // Reed Solomon Terminé !!!!

            // Dernier etape : Marquage
            // 0 donne BLANC et 1 donne NOIR
            // Modeles d'Alignement dans VERSION 2 ! PAS V1 !


            Bitmap bmp = new Bitmap(210, 210);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.White);

            bmp.Save("./Resource/woah.bmp");
            
            string QRImageLocation = "./Resource/QRbase.bmp"; // Fichier base de QRCode
            MyImage imageOriginal = new MyImage(QRImageLocation);
            test.Text = imageOriginal.partieImage.GetLength(1) + "";
            byte[,] imageRGB = imageOriginal.partieImage; //partie image avec RGB separé

            byte[,] monImage = new byte[imageOriginal.hauteur, imageOriginal.largeur];

            for (int i = 0; i < monImage.GetLength(0); i++)
            {
                for (int j = 0; j < monImage.GetLength(1); j++)
                {
                    monImage[i, j] = 255;
                }
            }

            // UNe matrice 21 * 21 parfait pour V1, APRES on converti cette matrice à 2100*2100 pour l'image final
            byte[,] tab21 = new byte[21, 21];

            for (int i = 0; i < tab21.GetLength(0); i++)
            {
                for (int j = 0; j < tab21.GetLength(1); j++)
                {
                    tab21[i, j] = 190;
                }
            }
            //Noir par défaut:


            // Construction Recherche des Motifs :

            //Bords Noir :


            // IMPORTANT : Chaque pixel vertical vaut : 100;   car j'ai pris une image 2100*2100 de base donc fois 100
            //             Chaque pixel horizontal vaut : 300;

            // Lignes Interdits - lignes qu'on utilise pas ----- VALEUR 60
            tab21 = Carree(12, 0, tab21, 9, 60); //en haut à gauche
            tab21 = Carree(12, 13, tab21, 8, 60); //en haut à droite
            tab21 = Carree(0, 1, tab21, 8, 60); //en haut à gauche


            // LES SEPARATEURS  --  certains lignes sont temporaire car apres sera effacé par noir ! 
            tab21 = Carree(0, 0, tab21, 8, 255); //en bas à gauche
            tab21 = Carree(13, 0, tab21, 8, 255); //en haut à gauche
            tab21 = Carree(13, 13, tab21, 8, 255); //en haut à droite

            // Separateurs fini

            // MOTIFS DE RECHERCHES 

            // motif en haut à gauche
            tab21 = CarreeCentre(3, 3, tab21, 3, 0);
            tab21 = CarreeCentre(3, 3, tab21, 2, 255);
            tab21 = CarreeCentre(3, 3, tab21, 1, 0);

            // motif en haut à gauche
            tab21 = CarreeCentre(17, 3, tab21, 3, 0);
            tab21 = CarreeCentre(17, 3, tab21, 2, 255);
            tab21 = CarreeCentre(17, 3, tab21, 1, 0);

            // motif en haut à droite
            tab21 = CarreeCentre(17, 17, tab21, 3, 0);
            tab21 = CarreeCentre(17, 17, tab21, 2, 255);
            tab21 = CarreeCentre(17, 17, tab21, 1, 0);

            //Module des Recherches FINI


            // MOTIFS DES SYNCHRONISATIONS
            // Juste des lignes simple

            bool noir = true;
            for (int i = 8; i < 13; i++)  // Ligne Synchro TOP
            {
                if (noir)
                {
                    tab21[14, i] = 0;
                    noir = false;
                }
                else
                {
                    tab21[14, i] = 255;
                    noir = true;
                }
            }

            noir = true;
            for (int i = 8; i < 13; i++)  // Ligne Synchro TOP
            {
                if (noir)
                {
                    tab21[i, 6] = 0;
                    noir = false;
                }
                else
                {
                    tab21[i, 6] = 255;
                    noir = true;
                }
            }

            // Motifs de Synchro fini

            // MOTIFS DE SOMBRE

            int nombreVerion = 1;
            tab21[7, 8] = 0;

            // Motifs de sombre fini


            





            // MESSAGE est le donne avec tous les BYTES + CORRECTIONS en string !

            test.Text = message.Length + "";

            // BYTES -------------------------> QR Code !!!!

            // ZIGZAG /\/\

            int x1 = 0;
            int y1 = 20;
            bool cotefait = false;
            for(int i = 0; i < 20; i++)
            {
                if(message[i] == '1')
                {
                    tab21[x1, y1] = 0;
                    if(cotefait == false)
                    {
                        y1--;
                        cotefait = true;
                    }
                    else
                    {
                        x1++;
                        y1++;
                        cotefait = false;
                    }
                    
                }
                if (message[i] == '0')
                {
                    tab21[x1, y1] = 255;
                    if (cotefait == false)
                    {
                        y1--;
                        cotefait = true;
                    }
                    else
                    {
                        x1++;
                        y1++;
                        cotefait = false;
                    }
                }
            }
            test.Text = message + "";




            // Converter 21*21 =====> 2100*2100
            int x = 0;
            int y = 0;
            int comptx = 0;
            int compty = 0;
            int testnum = 0;
            for (int i = 0; i < monImage.GetLength(0); i++)
            {

                for (int j = 0; j < monImage.GetLength(1); j++)
                {
                    monImage[i, j] = tab21[x, y];
                    compty++;
                    if (compty == 100)
                    {
                        y++;
                        compty = 0;

                    }
                }
                compty = 0;
                comptx++;
                if (comptx == 100)
                {
                    x++;
                    comptx = 0;
                    testnum++;
                }
                y = 0;
            }


            int compte = 0;
            for (int i = 0; i < imageRGB.GetLength(0); i++)
            {
                for (int j = 0; j < imageRGB.GetLength(1); j = j + 3)
                {
                    imageRGB[i, j] = monImage[i, compte];
                    imageRGB[i, j + 1] = monImage[i, compte];
                    imageRGB[i, j + 2] = monImage[i, compte];
                    compte++;
                }
                compte = 0;
            }



            imageOriginal.partieImage = imageRGB;
            imageOriginal.Image_to_File();

            imageQR.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));


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


        // Cette methode sert à creer une carre 
        // x et y signifie les coordonnees de centre dans la matrice ou on veut creer une Carree
        // NIVEAU est le niveau ou etages autour le centre de caree
        public byte[,] CarreeCentre(int x, int y, byte[,] monimage, int niveau, byte couleur) //pour creer les carrees (motifs) x,y les coordonnees centre
        {
            for (int i = - niveau; i <= niveau; i++)
            {
                for (int j = -niveau; j <= niveau; j++)  
                {
                    monimage[x+i, y+j] = couleur;
                }
            }


            return monimage;
        }

        public byte[,] Carree(int x, int y, byte[,] monimage, int longeur, byte couleur) //pour creer les carrees (motifs) x,y les coordonnees centre
        {
            for (int i = 0; i < longeur; i++)
            {
                for (int j = 0; j < longeur; j++)  
                {
                    monimage[x+i, y+j] = couleur;
                }
            }

            return monimage;
        }

    }
}
