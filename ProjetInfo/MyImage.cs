using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetInfo
{
    class MyImage
    {
        public int hauteur;
        public int largeur;
        public string taille;
        public byte[] imgdonnees;
        public byte[,] partieImage;

        public MyImage(string myfile)
        {
            imgdonnees = File.ReadAllBytes(myfile); //byte[] avec tous les donnees du bmp (metadonnee + image)
          

            byte[] taille_image = new byte[4];
            int compt = 0;
            for (int i = 14; i < 18; i++)
            {
                taille_image[compt] = imgdonnees[i];
                taille+= imgdonnees[i] + " ";
                compt++;
            }
            taille += Convertir_Endian_To_Int(taille_image);


            byte[] largeur_image = new byte[4];
            compt = 0;
            for (int i = 18; i < 22; i++)
            {
                largeur_image[compt] = imgdonnees[i];
                compt++;
            }
            largeur = Convertir_Endian_To_Int(largeur_image);

            byte[] hauteur_image = new byte[4];
            compt = 0;
            for (int i = 22; i < 26; i++)
            {
                hauteur_image[compt] = imgdonnees[i];
                //taille += imgdonnees[i] + " ";
                compt++;
            }
            hauteur = Convertir_Endian_To_Int(hauteur_image);


            partieImage = new byte[hauteur,largeur];
            /*int a = 0;
            int b = 0;
            for (int i = 54; i < myfile.Length-54; i = i++)
            {
                partieImage[a, b] = imgdonnees[i];
                b++;
                if (b == largeur)
                {
                    b = 0;
                    a++;
                }
            }
            */

            int compte = 54;
            for (int i = 0; i < partieImage.GetLength(0); i++)
            {
                for (int j = 0; j < partieImage.GetLength(1); j++)
                {
                    partieImage[i, j] = Convert.ToByte(imgdonnees[compte]);

                    compte++;
                }
            }

        }

        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int a = 0;

            for (int i = 0; i < tab.Length; i++)
            {
                a += tab[i] * Convert.ToInt32(Math.Pow(256, i));
            }

            return a;
        }

        public void Image_to_File()
        {
            
            int compte = 54;
            for (int i = 0; i < partieImage.GetLength(0); i++)
            {
                for (int j = 0; j < partieImage.GetLength(1); j++)
                {
                    imgdonnees[compte] = Convert.ToByte(partieImage[i, j]);

                    compte++;
                }
            }
            hauteur = imgdonnees[100];
            File.WriteAllBytes("./Resource/tempimg.bmp", imgdonnees);
        }

    }
}
