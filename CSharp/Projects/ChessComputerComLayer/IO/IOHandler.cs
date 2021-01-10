using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Huo_Chess_0._93_cs
{
    class IOHandler
    {
        // De nodige velden komen hierin
        private string poortNaam;
        private int baudRate;
        private string pariteit;
        private int dataBits;
        private string stopBits;
        private bool zetControle = false;   // false = zet 1 nog niet geregistreerd, true = wel + wachten op zet 2
        private GameLogic logica;
        private Punt oorsprong;
        private Punt doel;
        private SerialPort comPoort = new SerialPort();
        private IODelegate ioDel;
        private ObserverDelegate obsDel;

        #region Constructoren
        // Geen nood aan een standaardconstructor in deze klasse

        // Niet-standaardconstructor om de IODelegate mee te geven
        public IOHandler(IODelegate ioDel)
        {
            this.poortNaam = "COM3";
            this.baudRate = 57600;
            this.pariteit = "None";
            this.dataBits = 8;
            this.stopBits = "One";
            this.ioDel = ioDel;

            if (!verbindMicroController())
            {
                throw new Exception("De verbinding met de microcontroller kon niet worden opgesteld.");
            }

            this.comPoort.DataReceived += new SerialDataReceivedEventHandler(dataOntvangen);

            this.obsDel = new ObserverDelegate(ontvangFeedback);
        }

        // Niet-standaardconstructor
        public IOHandler(string poortNaam, int baudRate, string pariteit, int dataBits, string stopBits, IODelegate ioDel)
        {
            this.poortNaam = poortNaam;
            this.baudRate = baudRate;
            this.pariteit = pariteit;
            this.dataBits = dataBits;
            this.stopBits = stopBits;
            this.ioDel = ioDel;

            if (!verbindMicroController())
            {
                throw new Exception("De verbinding met de microcontroller kon niet worden opgesteld.");
            }

            this.comPoort.DataReceived += new SerialDataReceivedEventHandler(dataOntvangen);

            this.obsDel = new ObserverDelegate(ontvangFeedback);
        }

        // Copy-constructor
        public IOHandler(IOHandler ioHandler)
        {
            this.poortNaam = ioHandler.poortNaam;
            this.baudRate = ioHandler.baudRate;
            this.pariteit = ioHandler.pariteit;
            this.dataBits = ioHandler.dataBits;
            this.stopBits = ioHandler.stopBits;
            this.ioDel = ioHandler.ioDel;
            this.obsDel = ioHandler.obsDel;

            if (!verbindMicroController())
            {
                throw new Exception("De verbinding met de microcontroller kon niet worden opgesteld.");
            }

            this.comPoort.DataReceived += new SerialDataReceivedEventHandler(dataOntvangen);
        }
        #endregion

        // Initialisatie-methode om de verbinding met de microcontroller in te stellen
        public bool verbindMicroController()
        {
            try
            {
                // als de communicatiepoort al open is, sluit ze dan eerst
                if (this.comPoort.IsOpen)
                {
                    this.comPoort.Close();
                }

                // vanaf hier tot aan de volgende commentaar dient om de parity en stopbits te casten naar de benodigde types
                Parity parity = Parity.Even;
                StopBits stopBits = System.IO.Ports.StopBits.None;

                switch (this.pariteit)
                {
                    case "Even": parity = Parity.Even;
                        break;
                    case "Mark": parity = Parity.Mark;
                        break;
                    case "None": parity = Parity.None;
                        break;
                    case "Odd": parity = Parity.Odd;
                        break;
                    case "Space": parity = Parity.Space;
                        break;
                    default: parity = Parity.None;
                        break;
                }

                switch (this.stopBits)
                {
                    case "None": stopBits = System.IO.Ports.StopBits.None;
                        break;
                    case "One": stopBits = System.IO.Ports.StopBits.One;
                        break;
                    case "OnePointFive": stopBits = System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case "Two": stopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default: stopBits = System.IO.Ports.StopBits.None;
                        break;
                }

                // laadt alle variabelen in om de communicatie met de microcontroller op te starten
                this.comPoort.PortName = this.poortNaam;
                this.comPoort.BaudRate = this.baudRate;
                this.comPoort.Parity = parity;
                this.comPoort.DataBits = this.dataBits;
                this.comPoort.StopBits = stopBits;

                // open de communicatie
                this.comPoort.Open();
            }
            catch (Exception ex)
            {
                // indien het programma nog verder doet, wat wellicht niet gebeurt, geef weer dat er een fail is opgetreden
                return false;
            }

            // de fouten gebeuren binnen in de try-catch dus zal deze wel meestal true weergeven
            return true;
        }

        // Schrijf data weg naar de microcontroller aan de hand van een meegegeven string
        public void schrijfData(string data)
        {
            if (!this.comPoort.IsOpen)
            {
                this.comPoort.Open();
            }
            string[] temp = data.Split(';');
            Punt oorsprong = new Punt(int.Parse(temp[0]), int.Parse(temp[1]));
            Punt doel = new Punt(int.Parse(temp[2]), int.Parse(temp[3]));

            string info = Coordinaat.vertaalCoordinaatNumeriek(oorsprong) + ";" + Coordinaat.vertaalCoordinaatNumeriek(doel);
            this.comPoort.Write(info);
        }

        // Event handler die constant luistert naar input van de microcontroller uit
        private void dataOntvangen(object sender, SerialDataReceivedEventArgs e)
        {
            if (!this.comPoort.IsOpen)
            {
                this.comPoort.Open();
            }

            // string-leesmethode
            string input = comPoort.ReadExisting();

            // byte-leesmethode
            /*
             * int bytes = comPoort.BytesToRead;
             * byte[] comBuffer = new byte[bytes];
             * this.comPoort.Read(comBuffer, 0, bytes);
             */

            if (!this.zetControle)
            {
                // registreer de oorsprong van de pion
                this.oorsprong = Coordinaat.vertaalCoordinaat(int.Parse(input));
                this.zetControle = true;
            }
            else
            {
                // registreer het doel van de pion
                this.doel = Coordinaat.vertaalCoordinaat(int.Parse(input));

                // als het doel en de oorsprong hetzelfde zijn, geef dan weer dat de zet ongedaan gemaakt is op het bord
                if (this.oorsprong.X == this.doel.X && this.oorsprong.Y == this.doel.Y)
                {
                    schrijfData("INVALID MOVE!");
                }
                else
                {
                    IoDel(this.oorsprong, this.doel);
                }

                this.zetControle = false;
            }
        }

        public void ontvangFeedback(string info)
        {
            // compOorsprong.X + "," + compOorsprong.Y + ";" + compDoel.X + "," + compDoel.Y of "INVALID MOVE!"
            schrijfData(info);
        }

        #region Eigenschappen
        // Get/set-eigenschap voor het veld oorsprong
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

        // Get/set-eigenschap voor het veld doel
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

        // Get/set-eigenschap voor het veld poortNaam
        public string PoortNaam
        {
            get
            {
                return this.poortNaam;
            }
            set
            {
                this.poortNaam = value;
            }
        }

        // Get/set-eigenschap voor het veld baudRate
        public int BaudRate
        {
            get
            {
                return this.baudRate;
            }
            set
            {
                this.baudRate = value;
            }
        }

        // Get/set-eigenschap voor het veld parity
        public string Pariteit
        {
            get
            {
                return this.pariteit;
            }
            set
            {
                this.pariteit = value;
            }
        }

        // Get/set-eigenschap voor het veld dataBits
        public int DataBits
        {
            get
            {
                return this.dataBits;
            }
            set
            {
                this.dataBits = value;
            }
        }

        // Get/set-eigenschap voor het veld stopBits
        public string StopBits
        {
            get
            {
                return this.stopBits;
            }
            set
            {
                this.stopBits = value;
            }
        }

        // Get/set-eigenschap voor het veld ioDel
        public Huo_Chess_0._93_cs.IODelegate IoDel
        {
            get
            {
                return this.ioDel;
            }
            set
            {
                this.ioDel = value;
            }
        }

        // Get/set-eigenschap voor het veld 
        public Huo_Chess_0._93_cs.ObserverDelegate ObsDel
        {
            get
            {
                return this.obsDel;
            }
            set
            {
                this.obsDel = value;
            }
        }
        #endregion
    }
}
