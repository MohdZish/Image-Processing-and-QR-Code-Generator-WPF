using System;
using System.IO;

namespace ProjetInfo
{
    class MyImage
    {
        public int hauteur;
        public int largeur;
        public int taille;
        public byte[] imgdonnees; 
        public byte[,] partieImage;  //matrice de bytes


        //Rotation
        //rotation
        int tailleFichierRotation;
        int hauteurRotation;
        int largeurRotation;
        Pixel[,] photo;
        Pixel[,] photoRotation;
        byte[] imageRotation;
        byte[] photoPure;

        public MyImage(string myfile)
        {
            imgdonnees = File.ReadAllBytes(myfile); //byte[] avec tous les donnees du bmp (metadonnee + image)


            byte[] taille_image = new byte[4];
            int compt = 0;
            for (int i = 2; i < 6; i++)
            {
                taille_image[compt] = imgdonnees[i];
                //taille = imgdonnees[i];
                compt++;
            }
            taille = Convertir_Endian_To_Int(taille_image);


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


            partieImage = new byte[hauteur, largeur * 3];
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
            int a = 54;
            for(int i = 0; i < partieImage.GetLength(0); i++)
            {
                for(int j = 0; j < partieImage.GetLength(1); j++)
                {
                    imgdonnees[a] = partieImage[i, j];
                    a++;
                }
            }


            File.WriteAllBytes("./Resource/tempimg.bmp", imgdonnees);
        }


        public Pixel[,] RecupererImagePixel()
        {

            photo = new Pixel[hauteur, largeur];
            //(nbOctetCouleur * largeur) , on a 3 octets pour chaque pixel , et sur une ligne avec un certaine lageur
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 56; j < 56 + (3 * largeur); j = j + 3) // le premier bit annonce couleur est 54  (nbOcterCouleur
                {
                    Pixel pixel = new Pixel(imgdonnees[j - 2 + (i * largeur * 3)], imgdonnees[j - 1 + (i * largeur * 3)], imgdonnees[j + (i * largeur * 3)]); // pour chaque triplet d'octet on en fait 1 pixel
                    photo[i, (j - 56) / 3] = pixel;//(j/3) + (56/3) //j - 56 - 2*((j - 56) / 3)
                }
            }

            return photo;
        }


        public byte[] Convertir_Int_To_Endian(int a)
        {
            byte[] tab = new byte[4];
            int quotient;

            for (int i = tab.Length - 1; i > -1; i--)
            {
                quotient = a / Convert.ToInt32(Math.Pow(256, i));

                tab[i] = Convert.ToByte(quotient);
            }

            return tab;
        }


        public byte[] Convertir_Int_To_Endian(int val, int byteAlloues)
        {

            byte[] tab = new byte[byteAlloues];

            int puissanceMaximale = 0;
            while (val > Math.Pow(256, puissanceMaximale))//savoir jusqu'a valeur maximale de puissance de 256 on peut decomposer notre nombre valeur 
            {
                puissanceMaximale++;
            }

            int reste;

            // 54 68 42 35 26 0
            // taille = 6
            // puissancemax = 4

            if (byteAlloues >= puissanceMaximale) // si on a un tableau de byte suffisant allloué , on peut demarer la conversion , sinon non 
            {
                for (int j = puissanceMaximale + 1; j < byteAlloues; j++)
                {
                    tab[j] = 0;
                }

                for (int i = puissanceMaximale; i >= 0; i--)
                {
                    tab[i] = Convert.ToByte(val / Convert.ToInt32(Math.Pow(256, i)));
                    Console.WriteLine(" conversion chiffre " + tab[i]);
                    reste = Convert.ToInt32(val % (Math.Pow(256, i)));
                    val = reste;
                }
            }
            else
            {
                Console.WriteLine("Vous n'allouez pas un tableau assez suffisant");
                tab = null;
            }

            return tab;

        }
    }
}
