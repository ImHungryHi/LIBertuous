using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huo_Chess_0._93_cs
{
    class GameLogic
    {
        // Hier komt de spellogica

        //klein voorbeeld:
        private ObserverDelegate obsDel;
        private Punt oorsprong;
        private Punt doel;

        public Punt Oorsprong
        {
            get
            {
                return this.oorsprong;
            }
            set
            {
                this.oorsprong = value;
            }
        }
        public Punt Doel
        {
            get
            {
                return this.doel;
            }
            set
            {
                this.doel = value;
            }
        }
        public GameLogic(ObserverDelegate obsDel)
        {
            this.obsDel = obsDel;

            // juiste input gekregen
            schrijfFeedback("true");

            // foute input gekregen
            schrijfFeedback("false");
        }

        public GameLogic()
        {
            IOHandler ioHandelaar = new IOHandler(new IODelegate(Update));
            this.obsDel = ioHandelaar.ObsDel;
            obsDel("blabla");
        }

        public void Update(Punt oorsprong, Punt doel)
        {
            this.oorsprong = oorsprong;
            this.doel = doel;
        }

        private void schrijfFeedback(string info)
        {
            this.obsDel(info);
        }
    }
}
