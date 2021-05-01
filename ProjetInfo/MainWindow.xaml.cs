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
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Forms;

namespace ProjetInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.OpenFileDialog dlg;

        public string cheminOriginal; //chemin du fichier image qui est actuellement ouvert
        public MainWindow()
        {
            InitializeComponent();
            dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Image files (*.bmp)|*.bmp";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Buttonclose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Buttonminimise(object sender, RoutedEventArgs e)
        {
            MainPage.WindowState = WindowState.Minimized;
        }


        private void Import_Click(object sender, RoutedEventArgs e)
        {
            string filename = null;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.FileName;
            }

            BitmapImage bitmap = new BitmapImage();
            
            bitmap.BeginInit();
            

            if (filename != null)
            {
                bitmap.UriSource = new Uri(filename);
                bitmap.EndInit();
                imgOriginel.Source = bitmap;

                //Pour trouver nom du fichier
                string originallocation = Convert.ToString(imgOriginel.Source);
                string result = originallocation.Substring(originallocation.LastIndexOf('/') + 1);

                System.IO.File.Copy(filename, "./Resource/originalimg.bmp", true);

                cheminOriginal = "./Resource/originalimg.bmp";
                MyImage imageOriginal = new MyImage(cheminOriginal);

                imgnomtxt.Text = "Nom Image : " + result;
                imgdimtxt.Text = "Dimensions Image : " + imageOriginal.hauteur + "x" + imageOriginal.largeur;
            }


            //MyImage imageclass = new MyImage(originallocation);
            //imgdimtxt.Text = Convert.ToString(imageclass.largeur + " x " + imageclass.hauteur);

        }

        public void SupprimerAncien()
        {
            imgTraite.Source = null;
            //File.Delete("./Resource/tempimg.bmp");
        }


        public void AfficherImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri("pack://application:,,,/Resource/tempimg.bmp");
            image.EndInit();
            imgTraite.Source = image;
        }

        private void Fractal_Click(object sender, RoutedEventArgs e)
        {
            imgTraite.Source = null;
            int imageHauteur = 340;
            int imageLargeur = 340;
            Bitmap imageFractale = new Bitmap(imageHauteur, imageLargeur);
            for (int i = 0; i < 340; i++)
            {
                for (int j = 0; j < 340; j++)
                {
                    double a = (double)(i - (imageLargeur / 2)) / (double)(imageLargeur / 4);
                    double b = (double)(j - (imageHauteur / 2)) / (double)(imageHauteur / 4);

                    Complexe c = new Complexe(a, b);
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

                    imageFractale.SetPixel(i, j, iterations < 100 ? Color.White : Color.Black);
                }
            }

            imageFractale.Save("Resource/tempimg.bmp");

            imgOriginel.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
            //Process.Start("Result.bmp");

        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private void NoirBlanc_Click(object sender, RoutedEventArgs e)
        {
            SupprimerAncien();
            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;


            for (int i = 0; i < monImage.GetLength(0); i++)
            {
                for (int j = 0; j < monImage.GetLength(1); j=j+3)
                {
                    // PLAY WITH THE 5 here, INCREASE IT TO SEE DIFFERENT EFFECTS
                    byte somme = Convert.ToByte((monImage[i, j] + monImage[i, j + 1]  + monImage[i, j + 2])/3);
                    monImage[i, j] = somme;
                    monImage[i, j+1] = somme;
                    monImage[i, j+2] = somme;
                }
            }

            imageOriginal.partieImage = monImage;
            imageOriginal.Image_to_File();

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }

        private void EffectsPlay(object sender, RoutedEventArgs e)
        {
            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originel

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;

            for (int i = 0; i < monImage.GetLength(0); i++)
            {
                for (int j = 0; j < monImage.GetLength(1); j = j + 3)
                {
                    // PLAY WITH THE 5 here, INCREASE IT TO SEE DIFFERENT EFFECTS
                    byte somme = Convert.ToByte((monImage[i, j] / 5) + (monImage[i, j + 1] / 5) + (monImage[i, j + 2] / 5));
                    monImage[i, j] = somme;
                }
            }

            imageOriginal.partieImage = monImage;
            imageOriginal.Image_to_File();

            AfficherImage();
        }

        private void FlipRotation(object sender, RoutedEventArgs e)
        {
            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originel
            Bitmap imagebmp = BitmapImage2Bitmap(originel);

            imagebmp.RotateFlip(RotateFlipType.Rotate180FlipX);

            imagebmp.Save("./Resource/tempimg.bmp");

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }

        private void mirroirBtn_Click(object sender, RoutedEventArgs e)
        {
           
            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originale

            MyImage imageOriginal= new MyImage(cheminOriginal);
            title.Text = imageOriginal.taille + " ";
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

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }

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

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }

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

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }

        private void flouBtn_Click(object sender, RoutedEventArgs e)
        {
            int limite = 100; // IMPORTANT : c'est la seuil de detection de contours, varient selon images.

            MyImage imageOriginal = new MyImage(cheminOriginal);

            byte[,] monImage = imageOriginal.partieImage;

            int hauteur = imageOriginal.hauteur;
            int longeur = imageOriginal.largeur;


            //Pour ce detection, nous avons utilisé l'algorithme de LAPLACIEN
            double[,] matriceFlou = new double[3, 3] { { 1/9, 1/9, 1/9 }, { 1/9, 1/9, 1/9 }, { 1/9, 1/9, 1/9} };

            byte[,] nouveauimg = monImage;
            byte[,] img = monImage;

            // On fait ici la Convolution de Matrice Pixel de l'image avec la Matrice de convolution crée avant.
            for (int i = 3; i < nouveauimg.GetLength(0)-3; i++)
            {
                for (int j = 3; j < nouveauimg.GetLength(1)-3; j++)
                {
                    int somme = 0;

                    double a = matriceFlou[0, 0] * monImage[i - 1, j - 3];
                    double b = matriceFlou[0, 1] * monImage[i - 1, j];
                    double c = matriceFlou[0, 2] * monImage[i - 1, j + 3];
                    double d = matriceFlou[1, 0] * monImage[i, j - 3];
                    double m = matriceFlou[1, 1] * monImage[i, j];
                    double f = matriceFlou[1, 2] * monImage[i, j + 3];
                    double g = matriceFlou[2, 0] * monImage[i + 1, j - 3];
                    double h = matriceFlou[2, 1] * monImage[i + 1, j];
                    double k = matriceFlou[2, 2] * monImage[i + 1, j + 3];

                    somme += (int)(a + b + c + d + m + f + g + h + k);

                    img[i, j] = Convert.ToByte(somme);
                }
            }


            imageOriginal.partieImage = img;


            imageOriginal.Image_to_File();

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }

        private void qrcodeBtn_Click(object sender, RoutedEventArgs e)
        {
            if(qrframe.Content == null)
            {
                QRcode pageQR = new QRcode();
                qrframe.Content = pageQR;
            }
            else
            {
                qrframe.Content = null;
            }
            
        }


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


            title.Text = imagePixel[0, 0].Red + "";



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

        /*  public void RotationImage(double angle)
          {
              //A le point de départ
              //B le point d'arrivé

              double cosinusAngle = Math.Cos(angle);
              double sinusAngle = Math.Sin(angle);


              BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originel

              MyImage imageOriginal = new MyImage(cheminOriginal);

              byte[,] monImage = imageOriginal.partieImage;



              //coordonnées point départ
              int XA = 0;
              int YA = 0;

              //coordonnées point d'arrivé 
              int XB = 0;
              int YB = 0;


              //on veut que la rotation se fasse à partir du centre de l'image , on doit donc changer les coordonnées de l'origine,
              //on trasnslate l'origine avec le vecteur u(photo.GetLength(0) / 2);(photo.GetLength(1) / 2))
              int centreXA = ((photo.GetLength(1) + 1) / 2) - 1;
              int centreYA = ((photo.GetLength(0) + 1) / 2) - 1;

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


              for (int indexl = 0; indexl <= photo.GetLength(0) - 1; indexl++)
              {
                  for (int indexc = 0; indexc <= photo.GetLength(1) - 1; indexc++)
                  {
                      // on trannlate les coorodnnées XA et YA dans le  repère

                      XA = photo.GetLength(1) - indexc - 1 - centreXA;
                      YA = photo.GetLength(0) - indexl - 1 - centreYA;
                      //on realise la rotation dans le nouveau repere
                      XB = Convert.ToInt32((XA * cosinusAngle) + (YA * sinusAngle));
                      YB = Convert.ToInt32((XA * (-sinusAngle)) + (cosinusAngle * YA));

                      // on fait la translation sens inverse dans le repère
                      XB = centreXB - XB;
                      YB = centreYB - YB;
                      if ((0 <= XB && XB <= (matAgrandie.GetLength(1) - 1)) && (0 <= YB && YB <= (matAgrandie.GetLength(0) - 1)))
                      {
                          matAgrandie[YB, XB] = photo[indexl, indexc];
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

              //on retourne la nouvelle photo

              this.photoRotation = matAgrandie2;
              this.largeurRotation = matAgrandie2.GetLength(1);
              Console.WriteLine(Largeur);
              this.hauteurRotation = matAgrandie2.GetLength(0);
              Console.WriteLine(Hauteur);
              tailleFichierRotation = 54 + (hauteurRotation * largeurRotation * 3);
              imageRotation = new byte[tailleFichierRotation];
              RemettreImageModifie(photoRotation, hauteurRotation, largeurRotation);

              byte[] tailleRotationByte = Convertir_Int_To_Endian(tailleFichierRotation, 4);
              for (int k = 0; k < 54; k++)
              {
                  imageRotation[k] = image[k];

              }

              for (int i = 2; i < 6; i++)
              {
                  imageRotation[i] = tailleRotationByte[i - 2];
              }


              byte[] conversionHauteur = Convertir_Int_To_Endian(hauteurRotation, 4);
              byte[] conversionLargeur = Convertir_Int_To_Endian(largeurRotation, 4);

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
              Console.WriteLine(largeurRotation);
              Console.WriteLine(hauteurRotation);

              File.WriteAllBytes("./Images/SortieRotation2.bmp", imageRotation);
          }

      */

    }
    }
