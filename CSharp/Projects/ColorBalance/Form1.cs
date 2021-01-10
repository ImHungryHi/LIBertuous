using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ColorBalance
{
    public partial class Form1 : Form
    {
        private Bewerkingen afbeelding;

        public Form1()
        {
            InitializeComponent();
            
            //Standaardwaarde per kleur zetten op een waarde waar niets verandert aan de afbeelding
            trkRood.Value = 1;
            trkGroen.Value = 1;
            trkBlauw.Value = 1;
        }

        private void btnBladeren_Click(object sender, EventArgs e)
        {
            try
            {
                //File dialog aanmaken om een afbeelding te kunnen kiezen van een zelf te kiezen plaats
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Title = "Kies een afbeelding...";
                openFile.Multiselect = false;
                openFile.ShowDialog();

                //Fileinfo variabele om de extensie te controleren
                FileInfo fileInfo = new FileInfo(openFile.FileName);

                if (fileInfo.Extension.Equals(".bmp"))
                {
                    //Schrijf het pad en de afbeeldingsnaam weg naar het textveld
                    txtBladeren.Text = openFile.FileName;
                    afbeelding = new Bewerkingen(openFile.FileName);

                    if (afbeelding.isGeladen())
                    {
                        //Kan ook met: picBox.Image = Image.FromFile(openFile.FileName);
                        picBox.Image = afbeelding.geefOrigineel();

                        lblFeedback.Text = "Afbeelding succesvol ingeladen";
                    }
                }
                else
                {
                    lblFeedback.Text = "Ongeldige extensie, gelieve een .bmp te laden";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het openen van het bestand");
            }
        }

        private void btnVernieuw_Click(object sender, EventArgs e)
        {
            try
            {
                //Controleren of er een afbeelding is ingeladen en herladen
                if (afbeelding != null && afbeelding.isGeladen())
                {
                    picBox.Image = afbeelding.geefBewerkt();
                }
                //Indien de afbeelding niet is ingeladen, deze opvullen en de origineelwaarde ervan laden
                else
                {
                    afbeelding = new Bewerkingen();
                    picBox.Image = afbeelding.geefOrigineel();
                }

                lblFeedback.Text = "De afbeelding is vernieuwd";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het vernieuwen van de afbeelding");
            }
        }

        private void trkRood_Scroll(object sender, EventArgs e)
        {
            try
            {
                //Indien afbeelding opgevuld is, bereken de waarde van de kleurkanalen als rood verandert
                if (afbeelding != null)
                {
                    afbeelding.veranderKleur(trkRood.Value, trkRood.Maximum, trkGroen.Value, trkGroen.Maximum, trkBlauw.Value, trkBlauw.Maximum);
                    picBox.Image = afbeelding.geefBewerkt();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het berekenen van de nieuwe kleurwaarden met de rode slider");
            }
        }

        private void trkGroen_Scroll(object sender, EventArgs e)
        {
            try
            {
                //Indien afbeelding opgevuld is, bereken de waarde van de kleurkanalen als groen verandert
                if (afbeelding != null)
                {
                    afbeelding.veranderKleur(trkRood.Value, trkRood.Maximum, trkGroen.Value, trkGroen.Maximum, trkBlauw.Value, trkBlauw.Maximum);
                    picBox.Image = afbeelding.geefBewerkt();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het berekenen van de nieuwe kleurwaarden met de groene slider");
            }
        }

        private void trkBlauw_Scroll(object sender, EventArgs e)
        {
            try
            {
                //Indien afbeelding opgevuld is, bereken de waarde van de kleurkanalen als blauw verandert
                if (afbeelding != null)
                {
                    afbeelding.veranderKleur(trkRood.Value, trkRood.Maximum, trkGroen.Value, trkGroen.Maximum, trkBlauw.Value, trkBlauw.Maximum);
                    picBox.Image = afbeelding.geefBewerkt();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het berekenen van de nieuwe kleurwaarden met de blauwe slider");
            }
        }

        private void btnGrijswaarde_Click(object sender, EventArgs e)
        {
            try
            {
                //Indien afbeelding opgevuld is, bereken de grijswaarde ervan
                if (afbeelding != null)
                {
                    afbeelding.grijswaarde(trkRood.Value, trkRood.Maximum, trkGroen.Value, trkGroen.Maximum, trkBlauw.Value, trkBlauw.Maximum);
                    picBox.Image = afbeelding.geefBewerkt();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het berekenen van de grijswaarde");
            }
        }

        private void btnBlur_Click(object sender, EventArgs e)
        {
            try
            {
                //Indien afbeelding is opgevuld, bereken de geblurde inhoud en geef deze weer
                if (afbeelding != null)
                {
                    afbeelding.blurAfbeelding();
                    picBox.Image = afbeelding.geefBewerkt();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgetreden bij het blurren");
            }
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            try
            {
                //Controleren of de afbeelding is ingeladen en dan de initiele toestand herstellen
                if (afbeelding.isGeladen() && afbeelding != null)
                {
                    trkRood.Value = 1;
                    trkGroen.Value = 1;
                    trkBlauw.Value = 1;
                    picBox.Image = afbeelding.geefOrigineel();

                    lblFeedback.Text = "Alles werd naar de oorspronkelijke waarde teruggezet";
                }
                else
                {
                    lblFeedback.Text = "Er is geen afbeelding ingeladen om te resetten";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een fout opgelopen bij het resetten van de afbeelding");
            }
        }
    }
}
