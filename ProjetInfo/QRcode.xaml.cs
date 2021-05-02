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


        /// <summary>
        /// Methode qui change un valeur ALPHANUMERIQUE en Int pour QR Code
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int StringToNum(char val)
        {
            int resultat = 0;

            switch (val)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
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


        /// <summary>
        /// Methode pour convertir un valeur int à des Bytes
        /// la parametre bytesNecessaires signifie la longeur de bytes souhaité
        /// </summary>
        /// <param name="valdeciaml"></param>
        /// <param name="bytesNecessaire"></param>
        /// <returns></returns>
        public string DecimalToBytes(int valdeciaml, int bytesNecessaire) //Pour decimal -> binaire, en ajoutant 0 avant selon les necessités
        {
            string bytes = Convert.ToString(valdeciaml, 2);

            //ajouter les 0 à gauche pour faire zeroAvant bytes
            string zeros = "";
            for (int i = 0; i < bytesNecessaire - bytes.Length; i++) // jusqu'au que la condition de bytesNecessaires est rempli
            {
                zeros += "0";
            }
            bytes = zeros + bytes;

            return bytes;
        }

      
        /// <summary>
        /// METHODE POUR CREER LE QR CODE !!!!!!
        /// Appeler apres avoir clicker sur le bouton Generer un QR Code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreerQR_Click(object sender, RoutedEventArgs e) 
        {
            // Etapes intiale :
            // Alphanumerique donc 0010
            //Convertir nombre de caracteres à 9 bytes binaire
            //Determiner Version selon n° de carac. : V1 - max 25cars  ET V2 - max 47cars
            //Convertir texte à 9 bytes binaire


            int nombreCaracs = qrText.Text.Length; // qrText ====> le text ecrit par l'utilisateur

            string nombreCaracBYTES = DecimalToBytes(nombreCaracs, 9); // convertir la longeur en 9 bytes

            //Convertir Text en 11 bits//
            //Couper Texte en couples de 2
            string textqr = qrText.Text; // phrase ecrit
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
            if (finUnique)
            {
                coupleLettres[coupleLettres.Count - 1] = Convert.ToString(coupleLettres[coupleLettres.Count - 1][0]);

            }

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
            string bytesDonnees = "";

            // Ajouter l'Indicateur
            bytesDonnees += "0010"; // car AlphaNumerique

            // Ajouter Nombre de caracteres 9 bites
            bytesDonnees += nombreCaracBYTES;


            foreach (string bin in resultatBytes) //on prend 01000100
            {
                bytesDonnees += bin;
            }
           
            //ajouter 0 à la fin (4 maximum) si < 152
            for (int i = 0; i < 4; i++)
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
            int calcul = (152 - bytesDonnees.Length) / 8;
                

            // Ajouter les bytes necessaire pour remplir le QR code à 208 bytes
            bool change = true;
            for (int i = 0; i < calcul; i++)
            {
                if (change)
                {
                    bytesDonnees += "11101100";
                    change = false;
                }
                else
                {
                    bytesDonnees += "00010001";
                    change = true;
                }
            }
            

            // Maintenant REED SOLOMONS

            // Le message final est de forme : bytedonnees + ERREUR CORREC de REEDS
            // Donc faisons Reed Solomons pour trouver les bytes de corrections
            Encoding u8 = Encoding.UTF8;
            string a = qrText.Text; // on prend le text input
            a = a.ToUpper();
            byte[] bytesa = u8.GetBytes(a); // on le converti en bytes

            string[] stringDonnee = new string[bytesDonnees.Length / 8];


            int tempcount = 0;
            int index = -1;
            foreach(char c in bytesDonnees)
            {
                if(tempcount % 8 == 0)
                {
                    index++;
                }
                stringDonnee[index] += c;
                tempcount++;
            }

            byte[] bytesPourRS = new byte[bytesDonnees.Length / 8];

            for(int i = 0; i < stringDonnee.Length; i++)
            {
                bytesPourRS[i] = Convert.ToByte(stringDonnee[i], 2);
            }

            //string res = Encoding.UTF8.GetString(bytes); //interessant ! 10 -> 1

            byte[] result = ReedSolomonAlgorithm.Encode(bytesPourRS, 7, ErrorCorrectionCodeType.QRCode); // reed solomons qui nous donne par exemple 296 76 25 ...

            // CORRECTIONS 7 donc minimum 8 caracteres necessaires dans INPUT

            string message = bytesDonnees; // pas necessaire mais on creer un nouveau string pour bytesdonnes + corrections 

            string erreurcorrection = "";
            
            foreach (byte i in result)
            {
                erreurcorrection += DecimalToBytes(i, 8); //8 car on veut un octet pour chaque mot de corrections
            }

            message += erreurcorrection;

            // Reed Solomon Terminé !!!!

            // Dernier etape : Marquage
            // 0 donne BLANC et 1 donne NOIR
            // Modeles d'Alignement dans VERSION 2 ! PAS V1 !

            string QRImageLocation = "./Resource/QRbase.bmp"; // Fichier base de QRCode
            MyImage imageOriginal = new MyImage(QRImageLocation);
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
            tab21 = Carree(0, 0, tab21, 8, 250); //en bas à gauche
            tab21 = Carree(13, 0, tab21, 8, 250); //en haut à gauche
            tab21 = Carree(13, 13, tab21, 8, 250); //en haut à droite

            // Separateurs fini

            // MOTIFS DE RECHERCHES 

            // motif en haut à gauche
            tab21 = CarreeCentre(3, 3, tab21, 3, 1);
            tab21 = CarreeCentre(3, 3, tab21, 2, 254);
            tab21 = CarreeCentre(3, 3, tab21, 1, 1);

            // motif en haut à gauche
            tab21 = CarreeCentre(17, 3, tab21, 3, 1);
            tab21 = CarreeCentre(17, 3, tab21, 2, 254);
            tab21 = CarreeCentre(17, 3, tab21, 1, 1);

            // motif en haut à droite
            tab21 = CarreeCentre(17, 17, tab21, 3, 1);
            tab21 = CarreeCentre(17, 17, tab21, 2, 254);
            tab21 = CarreeCentre(17, 17, tab21, 1, 1);

            //Module des Recherches FINI


            // MOTIFS DES SYNCHRONISATIONS
            // Juste des lignes simple

            bool noir = true;
            for (int i = 8; i < 13; i++)  // Ligne Synchro TOP
            {
                if (noir)
                {
                    tab21[14, i] = 1;
                    noir = false;
                }
                else
                {
                    tab21[14, i] = 254;
                    noir = true;
                }
            }

            noir = true;
            for (int i = 8; i < 13; i++)  // Ligne Synchro TOP
            {
                if (noir)
                {
                    tab21[i, 6] = 1;
                    noir = false;
                }
                else
                {
                    tab21[i, 6] = 254;
                    noir = true;
                }
            }

            // Motifs de Synchro fini

            // MOTIFS DE SOMBRE

            tab21[7, 8] = 0;

            // Motifs de sombre fini

            // MESSAGE est le donne avec tous les BYTES + CORRECTIONS en string !
            // BYTES -------------------------> QR Code !!!!

            // ZIGZAG /\/\


            // Methode SUPER IMPORTANT pour faire les testes quand on rempli le QR Code
            int x1 = 0;
            int y1 = 20;
            bool cotefait = false; // est-ce que nous sommes à gauche ou à droite de ZigZag actuellement ?
            bool descente = false; // Actuellement on monte ou on descent?

            for (int i = 0; i < message.Length; i++) // aller jusqu' à remplir tous les bits de message sur le qr code
            {
                byte couleur = 255; // couleur de base 
                if (message[i] == '1') // si 1 alors couleur Noir 0
                {
                    couleur = 0;
                }
                tab21[x1, y1] = couleur;

                // les testes pour choisir prochain bloc

                if (descente == false) // Si on est en train de monter dans le qr code
                {
                    if (cotefait == false) // verifie si actuellement on est en gauche ou froite de zigzag
                    {
                        y1--;
                        cotefait = true; // alors cote fait donc on modifie
                    }
                    else  //aller en haut
                    {
                        // TESTS pour eviter regions interdits pendant MONTE !
                        if (x1 == 20 || tab21[x1 + 1, y1] == Convert.ToByte(60))
                        {
                            if(tab21[x1, y1 - 1] == Convert.ToByte(254)) // partie gauche haut EXCEPTION BLANC
                            {
                                y1 = 5;
                                cotefait = false;
                                descente = true;
                            }
                            else
                            {
                                y1--;
                                cotefait = false;
                                descente = true;
                            }
                        }
                        else
                        {
                            if (tab21[x1 + 1, y1] == Convert.ToByte(254) || tab21[x1 + 1, y1] == Convert.ToByte(1))
                            {
                                x1 = x1 + 2;
                                y1++;
                                cotefait = false;
                            }

                            else
                            {
                                x1++;
                                y1++;
                                cotefait = false;
                            }
                        }
                    }
                }

                else
                {
                    if (cotefait == false)
                    {
                        y1--;
                        cotefait = true;
                    }
                    else  //aller en haut
                    {
                        // TESTS pour eviter regions interdits pendant DESCENTE !
                        if (x1 == 0 || tab21[x1 - 1, y1] == Convert.ToByte(60))
                        {
                            if(tab21[x1, y1-1] == Convert.ToByte(60)) // partie gauche en bas EXCEPTION
                            {
                                x1 = 8;
                                y1--;
                                cotefait = false;
                                descente = false;
                            }
                            else
                            {
                                y1--;
                                cotefait = false;
                                descente = false;
                            }
                        }
                        else
                        {
                            if (tab21[x1 - 1, y1] == Convert.ToByte(254) || tab21[x1 - 1, y1] == Convert.ToByte(1) || tab21[x1 - 1, y1] == Convert.ToByte(250))
                            {
                                if (tab21[x1 - 1, y1] == Convert.ToByte(250))// 250 -> separateur EXCEPTION Gauche
                                {
                                    y1--;
                                    cotefait = false;
                                    descente = false;
                                }
                                else
                                {
                                    x1 = x1 - 2;
                                    y1++;
                                    cotefait = false;
                                }
                                
                            }
                            else
                            {
                                x1--;
                                y1++;
                                cotefait = false;
                            }

                        }
                    }
                }
            }


            // MASQUAGE !!!! Type : 000 
            byte[,] mask0 = new byte[21,21];

            for(int i = 0; i< mask0.GetLength(0); i++)
            {
                for (int j = 0; j < mask0.GetLength(1); j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        mask0[i, j] = 0;
                    }
                    else
                    {
                        mask0[i, j] = 255;
                    }
                }
            }


            // masquage ici de XOR
            bool masqer = true;
            if (masqer)
            {
                for (int i = 0; i < tab21.GetLength(0); i++)
                {
                    for (int j = 0; j < tab21.GetLength(1); j++)
                    {
                        if ((tab21[i, j] == Convert.ToByte(255) && mask0[i, j] == Convert.ToByte(0)) || (tab21[i, j] == Convert.ToByte(0) && mask0[i, j] == Convert.ToByte(255)))
                        {
                            tab21[i, j] = Convert.ToByte(0);
                        }

                        else if((tab21[i, j] == Convert.ToByte(255) && mask0[i, j] == Convert.ToByte(255)) || (tab21[i, j] == Convert.ToByte(0) && mask0[i, j] == Convert.ToByte(0)))
                        {
                            tab21[i, j] = Convert.ToByte(255);
                        }
                    }
                }
            }
            

            // Masquage Fini

            // VERSIONS MASQUE + CORRECTIONS INFO
            // https://www.thonky.com/qr-code-tutorial/format-version-information

            string infoVERSIONS = "111011111000100"; // donnée page 21 --- Bits de L et masque 0

            byte[] couleurscorrect = new byte[15];

            for(int i = 0; i < couleurscorrect.Length; i++)
            {
                byte newval = 0;
                if (infoVERSIONS[i] == '1')
                {
                    newval = Convert.ToByte(0);
                }
                else
                {
                    newval = Convert.ToByte(255);
                }
                couleurscorrect[i] = newval;
            }

            //page 25
            // 5 premiers
            for(int i = 0; i < 6; i++)
            {
                tab21[12, i] = couleurscorrect[i];
            }
            for (int i = 0; i < 7; i++)
            {
                tab21[i, 8] = couleurscorrect[i];
            }

            tab21[12, 7] = couleurscorrect[6];
            tab21[12, 8] = couleurscorrect[7];
            tab21[13, 8] = couleurscorrect[8];

            //9-14
            for (int i = 15; i < 21; i++)
            {
                tab21[i, 8] = couleurscorrect[i-15+9];
            }

            //7-14
            for (int i = 13; i < 21; i++)
            {
                tab21[12, i] = couleurscorrect[i - 13 + 7];
            }

            // Donner les bonnes couleurs
            for (int i = 0; i < tab21.GetLength(0); i++)
            {
                for (int j = 0; j < tab21.GetLength(0); j++)
                {
                    if(tab21[i,j] == Convert.ToByte(60))
                    {
                        tab21[i, j] = Convert.ToByte(255);
                    }

                    if (tab21[i, j] == Convert.ToByte(250))
                    {
                        tab21[i, j] = Convert.ToByte(255);
                    }

                    if (tab21[i, j] == Convert.ToByte(1))
                    {
                        tab21[i, j] = Convert.ToByte(0);
                    }
                    if (tab21[i, j] == Convert.ToByte(254))
                    {
                        tab21[i, j] = Convert.ToByte(255);
                    }
                }
            }



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

            imageQR.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp")); // WPF pour ouvrir le fichier enregistré


            //Changement de Version selon n° caracs. PAS FAIT :(
            if (nombreCaracs <= 25)
            {
                //version 1
            }
            else
            {
                //version 2
            }
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


        /// <summary>
        /// Methode pour creer une carrée, en partant sur en haut à gacuhe en coordonnées
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="monimage"></param>
        /// <param name="longeur"></param>
        /// <param name="couleur"></param>
        /// <returns></returns>
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
