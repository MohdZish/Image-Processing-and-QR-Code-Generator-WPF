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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Windows.Media.Animation;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Windows.Media;

namespace ProjetInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.OpenFileDialog dlg; // une variable dialogue pour ouvrir fichier

        public string cheminOriginal = "coco.bmp"; //chemin du fichier image qui est actuellement ouvert
        public MainWindow()
        {
            InitializeComponent();
            dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Image files (*.bmp)|*.bmp";  //type de fichier bmp 
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) // Fonction non nécessaire mais fait pour aesthétique
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Buttonclose(object sender, RoutedEventArgs e) // Methode aesthetique WPF
        {
            Close();
        }

        private void Buttonminimise(object sender, RoutedEventArgs e)// Methode aesthetique WPF
        {
            MainPage.WindowState = WindowState.Minimized;
        }


        private void Import_Click(object sender, RoutedEventArgs e) // Methode pour ouvrir fichier WPF
        {
            imgTraite.Source = null; // enlever tous les images deja ouverts sur WPF
            string filename = null;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.FileName;
            }

            BitmapImage bitmap = new BitmapImage(); // bitmap n'est pas un VRAI bitmap ! Ici utilisé pour WPF

            bitmap.BeginInit(); 

            if (filename != null)
            {
                bitmap.UriSource = new Uri(filename); // Definir source du fichier ouvert
                bitmap.EndInit();
                imgOriginel.Source = bitmap; // image sur WPF prends comme source le BitmapImage

                //Pour trouver nom du fichier
                string originallocation = Convert.ToString(imgOriginel.Source);
                string result = originallocation.Substring(originallocation.LastIndexOf('/') + 1);

                System.IO.File.Copy(filename, "./Resource/originalimg.bmp", true); // non nécessaire mais facilite WPF

                cheminOriginal = "./Resource/originalimg.bmp"; // chemin original des fichiers ouverts
                MyImage imageOriginal = new MyImage(cheminOriginal);

                imgnomtxt.Text = "Nom Image : " + result; // Texte en bas du WPF
                imgdimtxt.Text = "Dimensions Image : " + imageOriginal.hauteur + "x" + imageOriginal.largeur;
            }


            //MyImage imageclass = new MyImage(originallocation);
            //imgdimtxt.Text = Convert.ToString(imageclass.largeur + " x " + imageclass.hauteur);

        }

        public void SupprimerAncien() // enlever l'image existante
        {
            imgTraite.Source = null;
        }

        
        /// <summary>
        ///  Methode pour afficher une image apres la touche des boutons Filtres
        /// </summary>
        /// <param name="message"></param>
        public void AfficherImage(string message) 
        {
            imgTraite.Source = null;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            
            image.UriSource = new Uri("pack://application:,,,/Resource/tempimg.bmp");
            // Codes en dessous completement optionnel, sert que pour aesthetique optionnel
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.EndInit();
            imgTraite.Source = image; // Image traitée prends l'image mis en jour

            MenuPopUp(message); // Texte pour afficher en animations !!!
        }


        /// <summary>
        /// Methode pour créer une Fractale apres toucher bouton Fractal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fractal_Click(object sender, RoutedEventArgs e)
        {
            imgTraite.Source = null;
            imgOriginel.Source = null;

            MyImage imageOriginal = new MyImage("./Resource/FractalBase.bmp"); // On prend une image de base - pas nécessaire mais facilite la lecture et relecture

            int imageHauteur = imageOriginal.hauteur;
            int imageLargeur = imageOriginal.largeur;

            byte[,] monImage = imageOriginal.partieImage;

            Pixel[,] imagePixel = new Pixel[imageHauteur, imageLargeur]; // Une matrice Pixel avec dimensions hauteur largeur

            int compte = 0;
            for (int i = 0; i < imagePixel.GetLength(0); i++)
            {
                for (int j = 0; j < imagePixel.GetLength(1); j++)
                {
                    imagePixel[i, j] = new Pixel(monImage[i, compte], monImage[i, compte + 1], monImage[i, compte + 2]);
                    compte += 3;
                }
                compte = 0;
            }

            for (int i = 0; i < imageHauteur; i++)
            {
                for (int j = 0; j < imageLargeur; j++)
                {
                    double a = (double)(i - (imageLargeur / 2)) / (double)(imageLargeur / 4);
                    double b = (double)(j - (imageHauteur / 2)) / (double)(imageHauteur / 4);

                    Complexe c = new Complexe(a, b); // Utilisation de classe Complexe 
                    Complexe z = new Complexe(0, 0); //on peut modifier ces parametres !!!!

                    int iterations = 0;
                    do
                    {
                        iterations++;

                        // f(z) = z2 + c ;   Mandelbrot Fractale
                        z.CarreComplexe();
                        z.AjouterComplexe(c);

                        if (z.Module() > 2.0)
                        {
                            break;
                        }

                    } while (iterations < 100);


                    if(iterations < 100)
                    {
                        imagePixel[i, j] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        imagePixel[i, j] = new Pixel(255, 255, 255);
                    }
                }
            }


            // Pixel -> image
            int index = 0;
            for (int i = 0; i < imageOriginal.partieImage.GetLength(0); i++)
            {
                for (int j = 0; j < imageOriginal.partieImage.GetLength(1); j = j + 3)
                {
                    imageOriginal.partieImage[i, j] = imagePixel[i, index].Red;
                    imageOriginal.partieImage[i, j + 1] = imagePixel[i, index].Green;
                    imageOriginal.partieImage[i, j + 2] = imagePixel[i, index].Blue;
                    index++;
                }

                index = 0;
            }

            imageOriginal.Image_to_File(); //Methode de classe Image pour enregistrer la nouvelle image afin d'afficher

            //imgOriginel.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
            AfficherImage("C'est une fractale!");
            //Process.Start("Result.bmp");
        }


        /// <summary>
        /// Methode pour creer une image noir et blanc à partir d'une image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoirBlanc_Click(object sender, RoutedEventArgs e)
        {
            SupprimerAncien(); // pas necessaire mais on supprime les images deja ouvertes si ouverts sur WPF
            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;

            for (int i = 0; i < monImage.GetLength(0); i++)
            {
                for (int j = 0; j < monImage.GetLength(1); j=j+3)
                {
                    // Faire la moyenne des couleurs
                    byte somme = Convert.ToByte((monImage[i, j] + monImage[i, j + 1]  + monImage[i, j + 2])/3);
                    monImage[i, j] = somme;
                    monImage[i, j+1] = somme;
                    monImage[i, j+2] = somme;
                }
            }

            imageOriginal.partieImage = monImage;
            imageOriginal.Image_to_File();

            AfficherImage("Converti en noir et blanc");
        }


        /// <summary>
        /// Methode fait juste pour amuser, pas affiché dans WPF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectsPlay(object sender, RoutedEventArgs e)
        {
            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originel

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;

            for (int i = 0; i < monImage.GetLength(0); i++)
            {
                for (int j = 0; j < monImage.GetLength(1); j = j + 3)
                {
                    // Changer 5 pour voir les effets
                    byte somme = Convert.ToByte((monImage[i, j] / 5) + (monImage[i, j + 1] / 5) + (monImage[i, j + 2] / 5));
                    monImage[i, j] = somme;
                }
            }

            imageOriginal.partieImage = monImage;
            imageOriginal.Image_to_File();

            AfficherImage("Effects");
        }

        /// <summary>
        /// Methode pour creer une image mirroir verticale en partant d'une image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mirroirBtn_Click(object sender, RoutedEventArgs e)
        {
           
            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originale

            MyImage imageOriginal= new MyImage(cheminOriginal);
            byte[,] monImage = imageOriginal.partieImage; //interesting : monImage ====== partieImage !!!

            //Traitement d'Image
            int compt = 0;
            for (int i = 0; i < monImage.GetLength(0); i++)
            {
                for (int j = Convert.ToInt32(monImage.GetLength(1) / 2); j < monImage.GetLength(1); j= j + 3)
                {
                    monImage[i, j] = monImage[i, Convert.ToInt32(monImage.GetLength(1) / 2) - compt - 3];
                    monImage[i, j+1] = monImage[i, Convert.ToInt32(monImage.GetLength(1) / 2) - compt - 2];
                    monImage[i, j + 2] = monImage[i, Convert.ToInt32(monImage.GetLength(1) / 2) - compt -  1];

                    compt=compt+3;
                }
                compt = 0;
            }

            //imageOriginal.partieImage = monImage; 

            imageOriginal.Image_to_File();

            AfficherImage("Une image miroir!");
        }


        /// <summary>
        /// Methode pour creer une image luminosité augmentéé de 1.5 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void luminositeBtn_Click(object sender, RoutedEventArgs e)
        {
            double intensite = 1.5;

            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originel

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;


            for (int i = 0; i < monImage.GetLength(0); i++)
            {
                for (int j = 0; j < monImage.GetLength(1); j++)
                {
                    double a = monImage[i, j] * intensite;
                    if(a > 255)
                    {
                        a = 255;
                    }
                    if (a < 0)
                    {
                        a = 0;
                    }

                    monImage[i, j] = Convert.ToByte(a);
                }
            }

            imageOriginal.partieImage = monImage;
            imageOriginal.Image_to_File();

            //imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));

            AfficherImage("La luminosité a augmenté!");
        }


        /// <summary>
        /// Methode pour creer une image de contour en partant d'une image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contourBtn_Click(object sender, RoutedEventArgs e)
        {
            int limite = 100; // IMPORTANT : c'est la seuil de detection de contours, varient selon images.

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;

            int hauteur = imageOriginal.hauteur;
            int longeur = imageOriginal.largeur;

            int[,] couleurmoyenne = new int[hauteur, longeur];
            //Console.WriteLine("Main mat: " + couleurmoyenne.GetLength(0) + "x" + couleurmoyenne.GetLength(1));


            //On crée une matrice couleurmoyenne qui que les pixels de l'image (sans header ou metadonnées)
            int compteur = 0;
            for (int i = 0; i < couleurmoyenne.GetLength(0); i++)
            {
                for (int j = 0; j < couleurmoyenne.GetLength(1); j++)
                {
                    couleurmoyenne[i, j] = (monImage[i, compteur] + monImage[i, compteur + 1] + monImage[i, compteur + 2]) / 3;
                    compteur = compteur + 3;
                    //Console.Write(couleurmoyenne[i, j] + " ");
                }
                compteur = 0;
                //Console.WriteLine();
            }

            //laptmat est LA MATRICE DE CONVOLUTION
            //Pour ce detection, nous avons utilisé l'algorithme de LAPLACIEN
            int[,] lapmat = new int[3, 3] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };


            // Quand on fait une convolution entre deux matrices, avec cette methode on crée une matrice
            // Cette matrice prends tous les elements de premier matrice, sauf les bords
            //Donc la matrice nouveauimag prends tous sauf les bords de couleurmatrice
            int[,] nouveauimg = new int[couleurmoyenne.GetLength(0) - 2, couleurmoyenne.GetLength(1) - 2];


            // On fait ici la Convolution de Matrice Pixel de l'image avec la Matrice de convolution crée avant.
            for (int i = 1; i < nouveauimg.GetLength(0); i++)
            {
                for (int j = 1; j < nouveauimg.GetLength(1); j++)
                {
                    int somme = 0;

                    int a = lapmat[0, 0] * couleurmoyenne[i - 1, j - 1];
                    int b = lapmat[0, 1] * couleurmoyenne[i - 1, j];
                    int c = lapmat[0, 2] * couleurmoyenne[i - 1, j + 1];
                    int d = lapmat[1, 0] * couleurmoyenne[i, j - 1];
                    int m = lapmat[1, 1] * couleurmoyenne[i, j];
                    int f = lapmat[1, 2] * couleurmoyenne[i, j + 1];
                    int g = lapmat[2, 0] * couleurmoyenne[i + 1, j - 1];
                    int h = lapmat[2, 1] * couleurmoyenne[i + 1, j];
                    int k = lapmat[2, 2] * couleurmoyenne[i + 1, j + 1];

                    somme += a + b + c + d + m + f + g + h + k;

                    nouveauimg[i - 1, j - 1] = somme;
                }
            }



            //On met ici les deux couleurs: noir et blanc.
            for (int i = 0; i < nouveauimg.GetLength(0); i++)
            {
                for (int j = 0; j < nouveauimg.GetLength(1); j++)
                {
                    if (Math.Abs(nouveauimg[i, j]) > limite)
                    {
                        nouveauimg[i, j] = 255;
                    }
                    else
                    {
                        nouveauimg[i, j] = 0;
                    }
                }
            }


            // pixelsfinal est une matrice pour remettre les nouveaux pixels pour le bitmap final
            // Cette matrice n'est pas OBLIGé mais j'ai crée quand meme pour mieux comprendre
            int[,] pixelsfinal = new int[couleurmoyenne.GetLength(0), couleurmoyenne.GetLength(1)];


            // nouveauimage EST L'IMAGE REDUIT MAIS AVEC LES BONNES COULEURS AVEC CONTOURS
            for (int i = 1; i < pixelsfinal.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < pixelsfinal.GetLength(1) - 1; j++)
                {
                    pixelsfinal[i, j] = nouveauimg[i - 1, j - 1];
                }
            }

            //Enfin on transfere les matrices pixels pour version final de l'image
            byte[,] matbyte = new byte[hauteur, longeur * 3];
            int lig = 0;
            int col = 0;

            for (int i = 0; i < matbyte.GetLength(0); i++)
            {
                for (int j = 0; j < matbyte.GetLength(1); j = j + 3)
                {
                    int test = col;
                    matbyte[i, j] = Convert.ToByte(pixelsfinal[lig, col]);
                    matbyte[i, j + 1] = Convert.ToByte(pixelsfinal[lig, col]);
                    matbyte[i, j + 2] = Convert.ToByte(pixelsfinal[lig, col]);

                    col++;

                }
                col = 0;
                lig++;
            }

            imageOriginal.partieImage = matbyte;

            imageOriginal.Image_to_File();

            //imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));

            AfficherImage("Contour dessiné!");
        }


        /// <summary>
        /// Creer une image flou à partir d'une image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flouBtn_Click(object sender, RoutedEventArgs e)
        {
            int limite = 100; // IMPORTANT : c'est la seuil de detection de contours, varient selon images.

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;

            //Pour ce detection, nous avons utilisé l'algorithme de LAPLACIEN
            double[,] matriceFlou = new double[3, 3] { { 1/5, 1/5, 1/5 }, { 1/5, 1/5, 1/5 }, { 1/5, 1/5, 1/5} };

            byte[,] img = monImage;

            // On fait ici la Convolution de Matrice Pixel de l'image avec la Matrice de convolution crée avant.
            for (int i = 3; i < img.GetLength(0)-3; i++)
            {
                for (int j = 3; j < img.GetLength(1)-3; j++)
                {
                    double somme = 0;
                    double valflou = 0.11111;
                    double a = valflou * monImage[i - 1, j - 3];
                    double b = valflou * monImage[i - 1, j];
                    double c = valflou * monImage[i - 1, j + 3];
                    double d = valflou * monImage[i, j - 3];
                    double m = valflou * monImage[i, j];
                    double f = valflou * monImage[i, j + 3];
                    double g = valflou * monImage[i + 1, j - 3];
                    double h = valflou * monImage[i + 1, j];
                    double k = valflou * monImage[i + 1, j + 3];

                    somme += (double)(a + b + c + d + m + f + g + h + k);
                    img[i, j] = Convert.ToByte(somme);
                }
            }

            imageOriginal.partieImage = img;


            imageOriginal.Image_to_File();

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));

            AfficherImage("Flou ? :(");
        }


        /// <summary>
        /// Methode pour afficher le MENU QR CODE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void qrcodeBtn_Click(object sender, RoutedEventArgs e)
        {
            if(qrframe.Content == null)
            {
                QRcode pageQR = new QRcode(); // creer une instance de page QR Code 
                qrframe.Content = pageQR; // Remplr le frame créé pour afficher le menu
                qrcodeBtn.Content = "Accueil";
            }
            else
            {
                qrframe.Content = null; // si menu deja existe, alors ferme le !
                qrcodeBtn.Content = "QR Code";
            }
            
        }


        /// <summary>
        /// Methode pour creer une image avec rotation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void rotationBtn_Click(object sender, RoutedEventArgs e)
        {
            double angle = 50;

            double cosinusAngle = Math.Cos(angle);
            double sinusAngle = Math.Sin(angle);

            //coordonnées point départ
            int XA = 0;
            int YA = 0;

            //coordonnées point d'arrivé 
            int XB = 0;
            int YB = 0;

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] photo = imageOriginal.partieImage;


            //image RGB -> image Pixel
            Pixel[,] imagePixel = new Pixel[photo.GetLength(0), photo.GetLength(1)/3];

            int compte = 0;
            for (int i = 0; i < imagePixel.GetLength(0); i++)
            {
                for (int j = 0; j < imagePixel.GetLength(1); j++)
                {
                    imagePixel[i, j] = new Pixel(photo[i, j+compte], photo[i, j + compte], photo[i, j + compte]);
                    if(j%3 == 0) { compte++; }
                }
                compte = 0;
            }

            //on veut que la rotation se fasse à partir du centre de l'image , on doit donc changer les coordonnées de l'origine,
            //on trasnslate l'origine avec le vecteur u(photo.GetLength(0) / 2);(photo.GetLength(1) / 2))
            int centreXA = ((imagePixel.GetLength(1) + 1) / 2) - 1;
            int centreYA = ((imagePixel.GetLength(0) + 1) / 2) - 1;


            // pour avoir la hauteur et la largeur maximale , on doit appliquer notre fonction rotation 
            int largeurBis = Convert.ToInt32(Math.Abs(cosinusAngle * photo.GetLength(1)) + Math.Abs(sinusAngle * photo.GetLength(0))) + 1;

            int hauteurBis = Convert.ToInt32(Math.Abs(sinusAngle * photo.GetLength(1)) + Math.Abs(cosinusAngle * photo.GetLength(0))) + 1;

            int hauteurMatAgrandie2 = hauteurBis;
            int largeurMatAgrandie2 = largeurBis;

            if (hauteurBis % 4 == 1)
            {
                hauteurMatAgrandie2 = hauteurBis + 3;
            }
            if (hauteurBis % 4 == 2)
            {
                hauteurMatAgrandie2 = hauteurBis + 2;
            }
            if (hauteurBis % 4 == 3)
            {
                hauteurMatAgrandie2 = hauteurBis + 1;
            }

            if (largeurBis % 4 == 1)
            {
                largeurMatAgrandie2 = largeurBis + 3;
            }
            if (largeurBis % 4 == 2)
            {
                largeurMatAgrandie2 = largeurBis + 2;
            }
            if (largeurBis % 4 == 3)
            {
                largeurMatAgrandie2 = largeurBis + 1;
            }

            Pixel[,] matAgrandie2 = new Pixel[hauteurMatAgrandie2, largeurMatAgrandie2];

            for (int i = 0; i < matAgrandie2.GetLength(0); i++)
            {
                for (int j = 0; j < matAgrandie2.GetLength(1); j++)
                {
                    Pixel nulle = new Pixel(0, 255, 0);
                    matAgrandie2[i, j] = nulle;
                }
            }


            //on effectue une trasnlation de l'origine dans le nouveau repere de vecteur (largeurBis/2 , hauteurBis/2) 
            int centreXB = Convert.ToInt32(((largeurBis + 1) / 2) - 1);
            int centreYB = Convert.ToInt32(((hauteurBis + 1) / 2) - 1);


            Pixel[,] matAgrandie = new Pixel[hauteurBis, largeurBis]; //Sécrurité plus 1

            for (int i = 0; i < matAgrandie.GetLength(0); i++)
            {
                for (int j = 0; j < matAgrandie.GetLength(1); j++)
                {
                    Pixel nulle = new Pixel(0, 0, 255);
                    matAgrandie[i, j] = nulle;
                }
            }



            for (int indexl = 0; indexl <= imagePixel.GetLength(0) - 1; indexl++)
            {
                for (int indexc = 0; indexc <= imagePixel.GetLength(1) - 1; indexc++)
                {
                    // on trannlate les coorodnnées XA et YA dans le  repère

                    XA = imagePixel.GetLength(1) - indexc - 1 - centreXA;
                    YA = imagePixel.GetLength(0) - indexl - 1 - centreYA;
                    //on realise la rotation dans le nouveau repere
                    XB = Convert.ToInt32((XA * cosinusAngle) + (YA * sinusAngle));
                    YB = Convert.ToInt32((XA * (-sinusAngle)) + (cosinusAngle * YA));

                    // on fait la translation sens inverse dans le repère
                    XB = centreXB - XB;
                    YB = centreYB - YB;
                    if ((0 <= XB && XB <= (matAgrandie.GetLength(1) - 1)) && (0 <= YB && YB <= (matAgrandie.GetLength(0) - 1)))
                    {
                        matAgrandie[YB, XB] = imagePixel[indexl, indexc];
                        //indexl
                        // indexc
                    }


                }
            }

            for (int i = 0; i < matAgrandie.GetLength(0); i++)
            {
                for (int j = 0; j < matAgrandie.GetLength(1); j++)
                {
                    matAgrandie2[i, j] = matAgrandie[i, j];
                }
            }

            // on retourne la nouvelle photo

            // this.photoRotation = matAgrandie2;
            int largeurRotation = matAgrandie2.GetLength(1);

            int hauteurRotation = matAgrandie2.GetLength(0);

            int tailleFichierRotation = 54 + (hauteurRotation * largeurRotation * 3);

            byte[] imageRotation = new byte[tailleFichierRotation];


            int compteur = 54;
            for (int i = 0; i < hauteurRotation; i++)
            {
                for (int j = 0; j < largeurRotation; j++)
                {

                    imageRotation[compteur] = matAgrandie2[i, j].Red;
                    imageRotation[compteur + 1] = matAgrandie2[i, j].Green;
                    imageRotation[compteur + 2] = matAgrandie2[i, j].Blue;
                    compteur = compteur + 3;
                }
            }

            byte[] tailleRotationByte = imageOriginal.Convertir_Int_To_Endian(tailleFichierRotation, 4);
            for (int k = 0; k < 54; k++)
            {
                imageRotation[k] = imageOriginal.imgdonnees[k];

            }

            for (int i = 2; i < 6; i++)
            {
                imageRotation[i] = tailleRotationByte[i - 2];
            }

            byte[] conversionHauteur = imageOriginal.Convertir_Int_To_Endian(hauteurRotation, 4);
            byte[] conversionLargeur = imageOriginal.Convertir_Int_To_Endian(largeurRotation, 4);

            for (int i = 0; i < 4; i++) //largeur
            {
                //largeurByte[i] = image[i + 18]; // hauteur
                imageRotation[i + 18] = conversionLargeur[i];
                //Console.WriteLine(imageRotation[i + 18]);
            }


            for (int j = 0; j < 4; j++)//hauteur
            {
                //hauteurByte[j] = image[j + 22];
                imageRotation[j + 22] = conversionHauteur[j];
            }


            //imageOriginal.partieImage = imageRotation;

            //imageOriginal.Image_to_File();

            File.WriteAllBytes("./Resource/rotSortie.bmp", imageRotation);

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/rotSortie.bmp"));
           
        }

        /// <summary>
        /// Methode pour cacher une image (Coco) dans une autre image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cacherImage_Click(object sender, RoutedEventArgs e)
        {
            // Image original 

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;

            int hauteur = imageOriginal.hauteur;
            int largeur = imageOriginal.largeur;

            Pixel[,] tempImg = new Pixel[hauteur, largeur];

            // Pixel pour image original
            Pixel[,] imagePixel = new Pixel[hauteur, largeur];

            // Image RGB -> PIXEL (pas necessaire de faire ici)
            int compte = 0;
            for (int i = 0; i < imagePixel.GetLength(0); i++)
            {
                for (int j = 0; j < imagePixel.GetLength(1); j++)
                {
                    imagePixel[i, j] = new Pixel(monImage[i, j + compte], monImage[i, j + compte+1], monImage[i, j + compte+2]);
                    compte += 2;
                }
                compte = 0;
            }

            // Image à cacher :

            MyImage imageCacher = new MyImage("coco.bmp");

            byte[,] cachcerPImage = imageCacher.partieImage; //RGB de image à cacher

            Pixel[,] cacherPixel = new Pixel[imageCacher.hauteur, imageCacher.largeur];

            compte = 0;
            for (int i = 0; i < cacherPixel.GetLength(0); i++)
            {
                for (int j = 0; j < cacherPixel.GetLength(1); j++)
                {
                    cacherPixel[i, j] = new Pixel(cachcerPImage[i, j + compte], cachcerPImage[i, j + compte], cachcerPImage[i, j + compte]);
                    if (j % 3 == 0) { compte++; }
                }
                compte = 0;
            }

            // HAUTEUR ET LARGEUR SONT MOITIE de l'original
            int modifhauteur = (hauteur - imageCacher.hauteur) / 2;

            int modiflargeur = (largeur - imageCacher.largeur) / 2;

            tempImg = imagePixel;  // tempImg prends l'image original maintenant, et ensuite on remplace l'image à cacher

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (i >= modifhauteur && j >= modiflargeur && j < (largeur - modiflargeur) && i < (hauteur - modifhauteur))
                    {
                        // Partie Bleu

                        int[] binaire1_Blue = int_To_Bye(imagePixel[i, j].Blue); 
                        int[] binaire2_Blue = int_To_Bye(cacherPixel[i - modifhauteur, j - modiflargeur].Blue); 
                        int[] nouveauBinaire_Blue = new int[8];
                        for (int k = 0; k < 8; k++)
                        {
                            if (k <= 3)
                            {
                                nouveauBinaire_Blue[k] = binaire2_Blue[4 + k]; // Les 4 premiers bits (premier moitié)
                            }
                            else
                            {
                                nouveauBinaire_Blue[k] = binaire1_Blue[k]; 
                            }
                        }
                        int valueBlue = Convert.ToInt32(Byte_To_int(nouveauBinaire_Blue));


                        // Traitment de Partie Verte
                        int[] binaire1_Green = int_To_Bye(imagePixel[i, j].Green);
                        int[] binaire2_Green = int_To_Bye(cacherPixel[i - modifhauteur, j - modiflargeur].Green);
                        int[] nouveauBinaire_Green = new int[8];
                        for (int k = 0; k < 8; k++)
                        {
                            if (k <= 3)
                            {
                                nouveauBinaire_Green[k] = binaire2_Green[4 + k];
                            }
                            else
                            {
                                nouveauBinaire_Green[k] = binaire1_Green[k];
                            }
                        }
                        int valueGreen = Convert.ToInt32(Byte_To_int(nouveauBinaire_Green));

                        int[] binaire1_Red = int_To_Bye(imagePixel[i, j].Red);
                        int[] binaire2_Red = int_To_Bye(cacherPixel[i - modifhauteur, j - modiflargeur].Red);
                        int[] nouveauBinaire_Red = new int[8];
                        for (int k = 0; k < 8; k++)
                        {
                            if (k <= 3)
                            {
                                nouveauBinaire_Red[k] = binaire2_Red[4 + k];
                            }
                            else
                            {
                                nouveauBinaire_Red[k] = binaire1_Red[k];
                            }
                        }
                        int valueRed = Byte_To_int(nouveauBinaire_Red);

                        tempImg[i, j] = new Pixel(Convert.ToByte(valueBlue), Convert.ToByte(valueGreen), Convert.ToByte(valueRed));
                    }
                    else 
                    {
                        tempImg[i, j] = imagePixel[i, j];
                    }
                }
            }


            // Image Temp Pixels ------> image original RGB
            int index = 0;
            for(int i = 0; i < imageOriginal.partieImage.GetLength(0); i++)
            {
                for (int j = 0; j < imageOriginal.partieImage.GetLength(1); j=j+3)
                {
                    imageOriginal.partieImage[i, j] = tempImg[i, index].Red;
                    imageOriginal.partieImage[i, j+1] = tempImg[i, index].Green;
                    imageOriginal.partieImage[i, j+2] = tempImg[i, index].Blue;
                    index++;
                }

                index = 0;
            }

            imageOriginal.Image_to_File();

            //imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
            AfficherImage("Coco est caché !");
        }


        // Convertir un Int to Byte, utile pour la methode de Cacher une image
        public static int[] int_To_Bye(int val)
        {
            int reste;

            int[] oct = new int[8];

            int i = 0;
            while (val != 0 && i < 8) //Changer un int à un byte en evaluant le valeur donnée en parametre
            {
                if (val == 0)
                {
                    oct[i] = 0;
                }
                reste = val % 2;
                oct[i] = reste;
                val = (val - reste) / 2;
                i++;
            }
            return oct;
        }

        public int Byte_To_int(int[] oct)
        {
            int val = 0;
            for (int i = 0; i < 8; i++)
            {
                val += Convert.ToInt32(oct[i] * Math.Pow(2, i));
            }
            return val;
        }


        /// <summary>
        /// Methode pour enlever toutes les images ouverts sur WPF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttondelete_Click(object sender, RoutedEventArgs e)
        {
            imgOriginel.Source = null;
            imgTraite.Source = null;
        }


        /// <summary>
        /// Methode pour l'ANIMATION du message en bas 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task MenuPopUp(string text)
        {
            await MenuPopUp1(text);
            await MenuPopUp2(text);
        }

        /// <summary>
        /// Premiere phase se l'animation
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task MenuPopUp1(string text)  //Animation premiere partie
        {
            popuptext.Content = text;
            TranslateTransform trans = new TranslateTransform(); // un objet WPF pour l'animation
            popupbar.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(0, -50, TimeSpan.FromSeconds(0.7)); // animation durant 0?7 secondes, avec l'objet qui monte de 50 en ordonnes
            trans.BeginAnimation(TranslateTransform.YProperty, anim1); // commencer l'animation WPF de l'objet popupbar

            //The delay between the two animations
            await Task.Delay(2 * 1000);
        }

        /// <summary>
        /// Deuxieme phase de l'animation
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task MenuPopUp2(string text)  //Animation Deuxieme partie
        {
            popuptext.Content = text;
            TranslateTransform trans = new TranslateTransform();
            popupbar.RenderTransform = trans;
            DoubleAnimation anim2 = new DoubleAnimation(-50, 0, TimeSpan.FromSeconds(0.7));
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }



    }
}
