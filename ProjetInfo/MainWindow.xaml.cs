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
                imgnomtxt.Text = "Nom Image : " + result;

                System.IO.File.Copy(filename, "./Resource/originalimg.bmp", true);


                cheminOriginal = "./Resource/originalimg.bmp";

            }


            //MyImage imageclass = new MyImage(originallocation);
            //imgdimtxt.Text = Convert.ToString(imageclass.largeur + " x " + imageclass.hauteur);

        }


        private void FractalePlace(object sender, RoutedEventArgs e)
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
                    Complexe z = new Complexe(0, 0);

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

                    imageFractale.SetPixel(i, j, iterations < 100 ? Color.Black : Color.White);
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

        public static void LaPlacienDet(string myfile) //myfile est l'emplacement de l'image
        {
            int limite = 100; // IMPORTANT : c'est la seuil de detection de contours, varient selon images.

            Bitmap originel = new Bitmap(myfile); ;

            byte[] imagebyte = File.ReadAllBytes(myfile);

            int hauteur = originel.Height;
            int longeur = originel.Width;

            int[,] couleurmoyenne = new int[hauteur, longeur];
            //Console.WriteLine("Main mat: " + couleurmoyenne.GetLength(0) + "x" + couleurmoyenne.GetLength(1));


            //On crée une matrice couleurmoyenne qui que les pixels de l'image (sans header ou metadonnées)
            int compteur = 54;
            for (int i = 0; i < couleurmoyenne.GetLength(0); i++)
            {
                for (int j = 0; j < couleurmoyenne.GetLength(1); j++)
                {
                    couleurmoyenne[i, j] = (imagebyte[compteur] + imagebyte[compteur + 1] + imagebyte[compteur + 2]) / 3;
                    compteur = compteur + 3;
                    //Console.Write(couleurmoyenne[i, j] + " ");
                }
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
                    int e = lapmat[1, 1] * couleurmoyenne[i, j];
                    int f = lapmat[1, 2] * couleurmoyenne[i, j + 1];
                    int g = lapmat[2, 0] * couleurmoyenne[i + 1, j - 1];
                    int h = lapmat[2, 1] * couleurmoyenne[i + 1, j];
                    int k = lapmat[2, 2] * couleurmoyenne[i + 1, j + 1];


                    somme += a + b + c + d + e + f + g + h + k;

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
            int[,] matbyte = new int[hauteur, longeur * 3];
            int lig = 0;
            int col = 0;

            for (int i = 0; i < matbyte.GetLength(0); i++)
            {
                for (int j = 0; j < matbyte.GetLength(1); j = j + 3)
                {
                    int test = col;
                    matbyte[i, j] = pixelsfinal[lig, col];
                    matbyte[i, j + 1] = pixelsfinal[lig, col];
                    matbyte[i, j + 2] = pixelsfinal[lig, col];

                    col++;

                }
                col = 0;
                lig++;
            }

            int compte = 54;
            for (int i = 0; i < matbyte.GetLength(0); i++)
            {
                for (int j = 0; j < matbyte.GetLength(1); j++)
                {
                    imagebyte[compte] = Convert.ToByte(matbyte[i, j]);

                    compte++;
                }
            }

            File.WriteAllBytes("Sortie.bmp", imagebyte);
        }


        private void NoirBlanc_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originel
            Bitmap imagebmp = BitmapImage2Bitmap(originel);

            imagebmp.RotateFlip(RotateFlipType.Rotate180FlipX);

            imagebmp.Save("./Resource/tempimg.bmp");

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }

        private void contourBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void mirroirBtn_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage originel = (BitmapImage)imgOriginel.Source; //on prend l'image originale
            //Bitmap imagebmp = BitmapImage2Bitmap(originel);

            MyImage imageOriginal= new MyImage(cheminOriginal);


            byte[,] imageTab = imageOriginal.partieImage;
            
            for (int i = 0; i < imageTab.GetLength(0); i++)
            {
                for (int j = 0; j < Convert.ToInt32(imageTab.GetLength(1)); j++)
                {
                    imageTab[i, j] = 255;
                }
            }

            imageOriginal.partieImage = imageTab;
            title.Text = Convert.ToString(imageOriginal.partieImage[0,0]);
            
            imageOriginal.Image_to_File();
            title.Text = Convert.ToString(imageOriginal.hauteur);

            imgTraite.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/tempimg.bmp"));
        }
    }
    }
