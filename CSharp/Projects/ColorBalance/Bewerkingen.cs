using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ColorBalance
{
    class Bewerkingen
    {
        private Bitmap origineel;
        private Bitmap bewerkt;
        private Boolean geladen = false;
        private const int MAXFACTOR = 5;

        //Maak een nieuw object aan aan de hand van een reeds voorziene Pagoda.bmp-afbeelding
        public Bewerkingen()
        {
            origineel = new Bitmap("Pagoda.bmp");
            bewerkt = new Bitmap("Pagoda.bmp");
            geladen = true;
        }

        //Maak een nieuw object aan aan de hand van een pad
        public Bewerkingen(string path)
        {
            origineel = new Bitmap(path);
            bewerkt = new Bitmap(path);
            geladen = true;
        }

        //Maak een nieuw object uit een reeds bestaand Bitmapobject
        public Bewerkingen(Bitmap origineel)
        {
            this.origineel = origineel;
            bewerkt = origineel;
            geladen = true;
        }

        //Deze methode verandert de kleur waarbij de waarde van alle kleurfactoren in acht genomen wordt
        public void veranderKleur(int roodFactor, int roodMax, int groenFactor, int groenMax, int blauwFactor, int blauwMax)
        {
            //Deze 3 if-structuren zorgen ervoor dat de juiste maximumfactor gebruikt wordt
            if (roodMax != MAXFACTOR)
            {
                double rFactor = roodFactor / roodMax * MAXFACTOR;
                roodFactor = (int)Math.Round(rFactor);
            }

            if (groenMax != MAXFACTOR)
            {
                double gFactor = groenFactor / groenMax * MAXFACTOR;
                groenFactor = (int)Math.Round(gFactor);
            }

            if (blauwMax != MAXFACTOR)
            {
                double bFactor = blauwFactor / blauwMax * MAXFACTOR;
                blauwFactor = (int)Math.Round(bFactor);
            }

            //Overloop de pixels
            for (int x = 0; x < origineel.Height; x++)
            {
                for (int y = 0; y < origineel.Width; y++)
                {
                    //Neem de kleur van de pixel en stel de waarden in voor de berekening
                    Color pixelKleur = origineel.GetPixel(x, y);
                    int[] nieuweKleur = {0, 0, 0}; //rij 0: R; rij 1: G; rij 2: B
                    int roodFormule = (int)(pixelKleur.R * roodFactor);
                    int groenFormule = (int)(pixelKleur.G * groenFactor);
                    int blauwFormule = (int)(pixelKleur.B * blauwFactor);

                    //Deze if-structuren zorgen ervoor dat de kleuren de maximumwaarde niet overschrijden
                    if (roodFormule >= 255)
                    {
                        nieuweKleur[0] = 255;
                    }
                    else
                    {
                        nieuweKleur[0] = roodFormule;
                    }

                    if (groenFormule >= 255)
                    {
                        nieuweKleur[1] = 255;
                    }
                    else
                    {
                        nieuweKleur[1] = groenFormule;
                    }

                    if (blauwFormule >= 255)
                    {
                        nieuweKleur[2] = 255;
                    }
                    else
                    {
                        nieuweKleur[2] = blauwFormule;
                    }

                    bewerkt.SetPixel(x, y, Color.FromArgb(nieuweKleur[0], nieuweKleur[1], nieuweKleur[2]));
                }
            }
        }

        //Deze methode neemt de kleurfactoren in acht en zet de afbeelding om naar grijswaarden
        public void grijswaarde(int roodFactor, int roodMax, int groenFactor, int groenMax, int blauwFactor, int blauwMax)
        {
            //Deze 3 if-structuren zorgen ervoor dat de juiste maximumfactor gebruikt wordt
            if (roodMax != MAXFACTOR)
            {
                double rFactor = roodFactor / roodMax * MAXFACTOR;
                roodFactor = (int)Math.Round(rFactor);
            }

            if (groenMax != MAXFACTOR)
            {
                double gFactor = groenFactor / groenMax * MAXFACTOR;
                groenFactor = (int)Math.Round(gFactor);
            }

            if (blauwMax != MAXFACTOR)
            {
                double bFactor = blauwFactor / blauwMax * MAXFACTOR;
                blauwFactor = (int)Math.Round(bFactor);
            }

            //Overloop de pixels
            for (int x = 0; x < origineel.Height; x++)
            {
                for (int y = 0; y < origineel.Width; y++)
                {
                    //Neem de kleur van de pixel en stel de waarden in voor de berekening
                    Color pixelKleur = origineel.GetPixel(x, y);
                    int[] nieuweKleur = { 0, 0, 0 }; //rij 0: R; rij 1: G; rij 2: B
                    int roodFormule = (int)(pixelKleur.R * roodFactor);
                    int groenFormule = (int)(pixelKleur.G * groenFactor);
                    int blauwFormule = (int)(pixelKleur.B * blauwFactor);

                    //Deze if-structuren zorgen ervoor dat de kleuren de maximumwaarde niet overschrijden
                    if (roodFormule >= 255)
                    {
                        nieuweKleur[0] = 255;
                    }
                    else
                    {
                        nieuweKleur[0] = roodFormule;
                    }

                    if (groenFormule >= 255)
                    {
                        nieuweKleur[1] = 255;
                    }
                    else
                    {
                        nieuweKleur[1] = groenFormule;
                    }

                    if (blauwFormule >= 255)
                    {
                        nieuweKleur[2] = 255;
                    }
                    else
                    {
                        nieuweKleur[2] = blauwFormule;
                    }

                    /**
                     * Deze formule zorgt ervoor dat, in samenwerking met de voorgaande if-structuren,
                     * de maximumwaarde van 255 niet overschreden wordt
                     */
                    int grijsTint = (int)(nieuweKleur[0] + nieuweKleur[1] + nieuweKleur[2]) / 3;
                    
                    //Ieder kleurkanaal krijgt dezelfde waarde om zijn grijswaarde te verkrijgen
                    bewerkt.SetPixel(x, y, Color.FromArgb(grijsTint, grijsTint, grijsTint));
                }
            }
        }

        //Deze methode blurt de afbeelding met behulp van alle omringende pixels
        public void blurAfbeelding()
        {
            Color[] kleurenRij = new Color[9];
            int[] nieuweKleur = {0, 0, 0}; //rij 0: R; rij 1: G; rij 2: B

            //Controle om zeker te zijn dat bewerkt geset is
            if (bewerkt == null) {
                bewerkt = new Bitmap(origineel);
            }

            //Overloop de pixels
            for (int x = 0; x < bewerkt.Height; x++)
            {
                for (int y = 0; y < bewerkt.Width; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[5] = bewerkt.GetPixel((x + 1), (y));
                        kleurenRij[7] = bewerkt.GetPixel(x, (y + 1));
                        kleurenRij[8] = bewerkt.GetPixel((x + 1), (y + 1));

                        nieuweKleur[0] = (kleurenRij[4].R + kleurenRij[5].R + kleurenRij[7].R + kleurenRij[8].R) / 4;
                        nieuweKleur[1] = (kleurenRij[4].G + kleurenRij[5].G + kleurenRij[7].G + kleurenRij[8].G) / 4;
                        nieuweKleur[2] = (kleurenRij[4].B + kleurenRij[5].B + kleurenRij[7].B + kleurenRij[8].B) / 4;
                    }
                    else if (x == 0 && (y > 0 && y < (bewerkt.Height - 1)))
                    {
                        kleurenRij[1] = bewerkt.GetPixel(x, (y - 1));
                        kleurenRij[2] = bewerkt.GetPixel((x + 1), (y - 1));
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[5] = bewerkt.GetPixel((x + 1), (y));
                        kleurenRij[7] = bewerkt.GetPixel(x, (y + 1));
                        kleurenRij[8] = bewerkt.GetPixel((x + 1), (y + 1));

                        nieuweKleur[0] = (kleurenRij[1].R + kleurenRij[2].R + kleurenRij[4].R + kleurenRij[5].R + kleurenRij[7].R + kleurenRij[8].R) / 6;
                        nieuweKleur[1] = (kleurenRij[1].G + kleurenRij[2].G + kleurenRij[4].G + kleurenRij[5].G + kleurenRij[7].G + kleurenRij[8].G) / 6;
                        nieuweKleur[2] = (kleurenRij[1].B + kleurenRij[2].B + kleurenRij[4].B + kleurenRij[5].B + kleurenRij[7].B + kleurenRij[8].B) / 6;
                    }
                    else if (x == bewerkt.Width && y == 0)
                    {
                        kleurenRij[3] = bewerkt.GetPixel((x - 1), (y));
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[6] = bewerkt.GetPixel((x - 1), (y + 1));
                        kleurenRij[7] = bewerkt.GetPixel(x, (y + 1));

                        nieuweKleur[0] = (kleurenRij[3].R + kleurenRij[4].R + kleurenRij[6].R + kleurenRij[7].R) / 4;
                        nieuweKleur[1] = (kleurenRij[3].G + kleurenRij[4].G + kleurenRij[6].G + kleurenRij[7].G) / 4;
                        nieuweKleur[2] = (kleurenRij[3].B + kleurenRij[4].B + kleurenRij[6].B + kleurenRij[7].B) / 4;
                    }
                    else if (x == bewerkt.Width && (y > 0 && y < bewerkt.Height))
                    {
                        kleurenRij[0] = bewerkt.GetPixel((x - 1), (y - 1));
                        kleurenRij[1] = bewerkt.GetPixel(x, (y - 1));
                        kleurenRij[3] = bewerkt.GetPixel((x - 1), (y));
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[6] = bewerkt.GetPixel((x - 1), (y + 1));
                        kleurenRij[7] = bewerkt.GetPixel(x, (y + 1));

                        nieuweKleur[0] = (kleurenRij[0].R + kleurenRij[1].R + kleurenRij[3].R + kleurenRij[4].R + kleurenRij[6].R + kleurenRij[7].R) / 6;
                        nieuweKleur[1] = (kleurenRij[0].G + kleurenRij[1].G + kleurenRij[3].G + kleurenRij[4].G + kleurenRij[6].G + kleurenRij[7].G) / 6;
                        nieuweKleur[2] = (kleurenRij[0].B + kleurenRij[1].B + kleurenRij[3].B + kleurenRij[4].B + kleurenRij[6].B + kleurenRij[7].B) / 6;
                    }
                    else if (y == 0 && (x > 0 && x < (bewerkt.Width - 1)))
                    {
                        kleurenRij[3] = bewerkt.GetPixel((x - 1), (y));
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[5] = bewerkt.GetPixel((x + 1), (y));
                        kleurenRij[6] = bewerkt.GetPixel((x - 1), (y + 1));
                        kleurenRij[7] = bewerkt.GetPixel(x, (y + 1));
                        kleurenRij[8] = bewerkt.GetPixel((x + 1), (y + 1));

                        nieuweKleur[0] = (kleurenRij[3].R + kleurenRij[4].R + kleurenRij[5].R + kleurenRij[6].R + kleurenRij[7].R + kleurenRij[8].R) / 6;
                        nieuweKleur[1] = (kleurenRij[3].G + kleurenRij[4].G + kleurenRij[5].G + kleurenRij[6].G + kleurenRij[7].G + kleurenRij[8].G) / 6;
                        nieuweKleur[2] = (kleurenRij[3].B + kleurenRij[4].B + kleurenRij[5].B + kleurenRij[6].B + kleurenRij[7].B + kleurenRij[8].B) / 6;
                    }
                    else if (y == bewerkt.Height && x == 0)
                    {
                        kleurenRij[1] = bewerkt.GetPixel(x, (y - 1));
                        kleurenRij[2] = bewerkt.GetPixel((x + 1), (y - 1));
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[5] = bewerkt.GetPixel((x + 1), (y));

                        nieuweKleur[0] = (kleurenRij[1].R + kleurenRij[2].R + kleurenRij[4].R + kleurenRij[5].R) / 4;
                        nieuweKleur[1] = (kleurenRij[1].G + kleurenRij[2].G + kleurenRij[4].G + kleurenRij[5].G) / 4;
                        nieuweKleur[2] = (kleurenRij[1].B + kleurenRij[2].B + kleurenRij[4].B + kleurenRij[5].B) / 4;
                    }
                    else if (y == bewerkt.Height && (x > 0 && x < (bewerkt.Width - 1)))
                    {
                        kleurenRij[0] = bewerkt.GetPixel((x - 1), (y - 1));
                        kleurenRij[1] = bewerkt.GetPixel(x, (y - 1));
                        kleurenRij[2] = bewerkt.GetPixel((x + 1), (y - 1));
                        kleurenRij[3] = bewerkt.GetPixel((x - 1), (y));
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[5] = bewerkt.GetPixel((x + 1), (y));

                        nieuweKleur[0] = (kleurenRij[0].R + kleurenRij[1].R + kleurenRij[2].R + kleurenRij[3].R + kleurenRij[4].R + kleurenRij[5].R) / 6;
                        nieuweKleur[1] = (kleurenRij[0].G + kleurenRij[1].G + kleurenRij[2].G + kleurenRij[3].G + kleurenRij[4].G + kleurenRij[5].G) / 6;
                        nieuweKleur[2] = (kleurenRij[0].B + kleurenRij[1].B + kleurenRij[2].B + kleurenRij[3].B + kleurenRij[4].B + kleurenRij[5].B) / 6;
                    }
                    else if ((x > 0 && x < (bewerkt.Width - 1)) && (y > 0 && y < (bewerkt.Height - 1)))
                    {
                        kleurenRij[0] = bewerkt.GetPixel((x - 1), (y - 1));
                        kleurenRij[1] = bewerkt.GetPixel(x, (y - 1));
                        kleurenRij[2] = bewerkt.GetPixel((x + 1), (y - 1));
                        kleurenRij[3] = bewerkt.GetPixel((x - 1), (y));
                        kleurenRij[4] = bewerkt.GetPixel(x, y);
                        kleurenRij[5] = bewerkt.GetPixel((x + 1), (y));
                        kleurenRij[6] = bewerkt.GetPixel((x - 1), (y + 1));
                        kleurenRij[7] = bewerkt.GetPixel(x, (y + 1));
                        kleurenRij[8] = bewerkt.GetPixel((x + 1), (y + 1));

                        nieuweKleur[0] = (kleurenRij[0].R + kleurenRij[1].R + kleurenRij[2].R + kleurenRij[3].R + kleurenRij[4].R + kleurenRij[5].R + kleurenRij[6].R + kleurenRij[7].R + kleurenRij[8].R) / 9;
                        nieuweKleur[1] = (kleurenRij[0].G + kleurenRij[1].G + kleurenRij[2].G + kleurenRij[3].G + kleurenRij[4].G + kleurenRij[5].G + kleurenRij[6].G + kleurenRij[7].G + kleurenRij[8].G) / 9;
                        nieuweKleur[2] = (kleurenRij[0].B + kleurenRij[1].B + kleurenRij[2].B + kleurenRij[3].B + kleurenRij[4].B + kleurenRij[5].B + kleurenRij[6].B + kleurenRij[7].B + kleurenRij[8].B) / 9;
                    }

                    bewerkt.SetPixel(x, y, Color.FromArgb(nieuweKleur[0], nieuweKleur[1], nieuweKleur[2]));
                }
            }
        }

        //Controleer of er een afbeelding is ingeladen
        public Boolean isGeladen()
        {
            return geladen;
        }

        //Geeft de originele bitmap-afbeelding terug
        public Bitmap geefOrigineel()
        {
            return origineel;
        }

        //Geeft de bewerkte bitmap-afbeelding terug
        public Bitmap geefBewerkt()
        {
            return bewerkt;
        }

        //Herstelt de bewerkte bitmap-afbeelding om met een verse lei te beginnen
        public void resetDefault()
        {
            bewerkt = new Bitmap("Pagoda.bmp");
        }

        //Herstelt de bewerkte bitmap-afbeelding via een opgegeven pad
        public void resetDefault(string path)
        {
            bewerkt = new Bitmap(path);
        }

        //Herstelt de bewerkte bitmap-afbeelding uit een Bitmapobject
        public void resetDefault(Bitmap bmp)
        {
            bewerkt = bmp;
        }
    }
}
