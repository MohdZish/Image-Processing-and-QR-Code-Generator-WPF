﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetInfo
{
    class Pixel
    {
        byte red;
        byte green;
        byte blue;


        public Pixel(byte red, byte green, byte blue)// constructeur qui prend en entrée 3 byte associé à chaque couleur
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public Pixel() //constructeur de pixel nul
        {
            this.red = 0;
            this.blue = 0;
            this.green = 0;
        }
        public int RedInt
        {
            get { return Convert.ToInt32(this.red); }
        }

        public int BlueInt
        {
            get { return Convert.ToInt32(this.blue); }
        }

        public int GreenInt
        {
            get { return Convert.ToInt32(this.green); }
        }

        public double RedDouble
        {
            get { return Convert.ToDouble(this.red); }
        }

        public double BlueDouble
        {
            get { return Convert.ToDouble(this.blue); }
        }

        public double GreenDouble
        {
            get { return Convert.ToDouble(this.green); }
        }
        public byte Red
        {
            get { return this.red; }
            set { red = value; }
        }

        public byte Blue
        {
            get { return this.blue; }
            set { blue = value; }
        }

        public byte Green
        {
            get { return this.green; }
            set { green = value; }
        }

        public string toString()
        {
            return red + " " + green + " " + blue;
        }
    }
}
