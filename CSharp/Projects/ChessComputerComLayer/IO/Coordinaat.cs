using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huo_Chess_0._93_cs
{
    public static class Coordinaat
    {
        #region Vertalingen
        // Vertaal de coordinaten aan de hand van een string van het systeem op het bord naar een x-y systeem
        public static Punt vertaalCoordinaat(int coordinaat)
        {
            // definitie van de te gebruiken integers
            double rij = coordinaat / 8;
            int x = 0;
            int y = 0;

            // controleer of het getal binnen de grenzen van het aantal speelvakken ligt
            if (coordinaat >= 0 && coordinaat < 64)
            {
                y = (int)Math.Round(rij);
                x = coordinaat - (y * 8);
            }

            return new Punt(x, y);
        }

        // Vertaal de coordinaten van een x-y systeem naar het systeem op het bord
        public static int vertaalCoordinaatNumeriek(Punt vertaalMij)
        {
            return vertaalMij.X + (vertaalMij.Y * 8);
        }

        // Vertaal de coordinaten van een alfabet-cijfer combinatie naar een x-y systeem
        public static Punt vertaalCoordinaat(string coordinaat)
        {
            int x = 0;
            int y = 0;

            if (coordinaat.Length == 2)
            {
                coordinaat = coordinaat.ToUpper();
                char[] coordinaten = coordinaat.ToCharArray();

                switch (coordinaten[0])
                {
                    case 'A': x = 1;
                        break;
                    case 'B': x = 2;
                        break;
                    case 'C': x = 3;
                        break;
                    case 'D': x = 4;
                        break;
                    case 'E': x = 5;
                        break;
                    case 'F': x = 6;
                        break;
                    case 'G': x = 7;
                        break;
                    case 'H': x = 8;
                        break;
                    default: throw new Exception("No valid x-coordinate found; Must be between A and H");
                }

                y = int.Parse(coordinaten[1].ToString());
            }

            return new Punt(x, y);
        }

        public static int vertaalXCoordinaat(string coordinaat)
        {
            int x = 0;

            if (coordinaat.Length == 2)
            {
                coordinaat = coordinaat.ToUpper();
                char[] coordinaten = coordinaat.ToCharArray();

                switch (coordinaten[0])
                {
                    case 'A': x = 1;
                        break;
                    case 'B': x = 2;
                        break;
                    case 'C': x = 3;
                        break;
                    case 'D': x = 4;
                        break;
                    case 'E': x = 5;
                        break;
                    case 'F': x = 6;
                        break;
                    case 'G': x = 7;
                        break;
                    case 'H': x = 8;
                        break;
                    default: throw new Exception("No valid x-coordinate found; Must be between A and H");
                }
            }

            return x;
        }

        // Vertaal de coordinaten van een x-y naar alfa
        public static string vertaalCoordinaatSchaakbordgelijk(Punt vertaalMij)
        {
            char xChar = 'A';   // default waarde
            int x = vertaalMij.X;
            int y = vertaalMij.Y;

            switch (x)
            {
                case 1: xChar = 'A';
                    break;
                case 2: xChar = 'B';
                    break;
                case 3: xChar = 'C';
                    break;
                case 4: xChar = 'D';
                    break;
                case 5: xChar = 'E';
                    break;
                case 6: xChar = 'F';
                    break;
                case 7: xChar = 'G';
                    break;
                case 8: xChar = 'H';
                    break;
                default: throw new Exception("No valid x-coordinate found; Must be between 1 and 8");
            }

            return "" + xChar + y;
        }

        // Vertaal de coordinaten van een x-y naar alfa2 voor schaakprogramma
        public static string vertaalCoordinaatSchaak(Punt vertaalMij)
        {
            char xChar = 'A';   // default waarde
            int x = vertaalMij.X;
            int y = vertaalMij.Y;

            switch (x)
            {
                case 1: xChar = 'A';
                    break;
                case 2: xChar = 'B';
                    break;
                case 3: xChar = 'C';
                    break;
                case 4: xChar = 'D';
                    break;
                case 5: xChar = 'E';
                    break;
                case 6: xChar = 'F';
                    break;
                case 7: xChar = 'G';
                    break;
                case 8: xChar = 'H';
                    break;
                default: throw new Exception("No valid x-coordinate found; Must be between 1 and 8");
            }

            return "" + xChar +";"+ y;
        }
        #endregion
    }
}
