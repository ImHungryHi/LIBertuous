using System;
using System.Collections.Generic;

using System.Text;

namespace Huo_Chess_0._93_cs
{
    public class Punt
    {
        private int x;
        private int y;

        // Standaard constructor
        public Punt()
        {
            this.x = 0;
            this.y = 0;
        }

        // Niet-standaard constructor
        public Punt(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        // Copy-constructor
        public Punt(Punt punt)
        {
            this.x = punt.X;
            this.y = punt.Y;
        }

        // Get/set-eigenschap voor het x-coordinaat
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        // Get/set-eigenschap voor het y-coordinaat
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
    }
}
