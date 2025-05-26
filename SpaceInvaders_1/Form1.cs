using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;
using SpaceInvaders_1.Properties;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static SpaceInvaders_1.Form1;
using Image = System.Drawing.Image;

namespace SpaceInvaders_1
{
    public partial class Form1 : Form
    { 
        /* ***** postavke varijabli/parametara vezanih za početnu formu ***** */
        class FormaStartEnd  
        {
            // Referenca na vanjsku klasu - formu
            private Form1 outer;

            /// <summary>
            /// Slika vanzemaljca u gornjem desnom kutu.
            /// </summary>
            public Vanzemaljac SlikaStart1 { get; set; }
            /// <summary>
            /// Slika vanzemaljca u donjem lijevom kutu.
            /// </summary>
            public Vanzemaljac SlikaStart2 { get; set; }
            /// <summary>
            /// Polje s dvije slike kad je Game Over u donjem desnom kutu. 
            /// Postavlja se tijekom igre na sliku letjelice ili eksplozije.
            /// </summary>
            public Vanzemaljac[] SlikeEnd { get; set; }
            /// <summary>
            /// Oznaka koja je slika za Game Over trenutno odabrana
            /// </summary>
            public int IndeksSlikeEnd { get; set; }
            /// <summary>
            /// Timer za pomicanje slika vanzemaljaca za Start i alterniranje slika za Game Over.
            /// </summary>
            public System.Windows.Forms.Timer Timer { get; set; }
            /// <summary>
            /// Brzina pomicanja slika na početnoj formi
            /// </summary>
            public double Brzina { get; set; } 
            /// <summary>
            /// Smjer kretanja: 1 znaci udesno, -1 znaci ulijevo.
            /// </summary>
            public int Smjer { get; set;  }
            /// <summary>
            /// Broj napravljenih horizontalnih pomaka u jednom smjeru
            /// </summary>
            public int BrojHorizontalnihPomaka { get; set; }
            /// <summary>
            /// Velicina slika vanzemaljaca na početnoj formi
            /// </summary>
            public int VelicinaSlikeStart { get; set; }
            /// <summary>
            /// Velicina slika na formi za Game Over
            /// </summary>
            public int VelicinaSlikeEnd { get; set; }

            public FormaStartEnd(Form1 outer)
            {
                this.outer = outer;
                Timer = new System.Windows.Forms.Timer();
                Timer.Interval = 100;
                Timer.Tick += new EventHandler(outer.FormaStartEndTimer_Tick);

                Brzina = 5.0;

                InicijalizirajSlikeStart();
                InicijalizirajSlikeEnd();
                DefinirajVelicineSlika();
            }

            /// <summary>
            /// Resetiranje položaja slika vanzemaljaca na formi Start.
            /// </summary>
            public void InicijalizirajSlikeStart()
            {
                // Slika vanzemaljca u gornjem desnom kutu
                int X = (int)(0.8 * outer.ClientSize.Width);
                int Y = (int)(0.1 * outer.ClientSize.Height);
                SlikaStart1 = new Vanzemaljac(new Point(X, Y), Properties.Resources.vanzemaljac_2);

                // Početno ulijevo
                Smjer = -1;
                BrojHorizontalnihPomaka = 0;

                // Slika vanzemaljca u donjem lijevom kutu
                X = (int)(0.1 * outer.ClientSize.Width);
                Y = (int)(0.8 * outer.ClientSize.Height);
                SlikaStart2 = new Vanzemaljac(new Point(X, Y), Properties.Resources.vanzemaljac_3);

                // Početno letjelica na sljedećoj formi End
                IndeksSlikeEnd = 0;
            }

            public void InicijalizirajSlikeEnd()
            {
                // Slike kad je Game Over u donjem desnom kutu
                int X = (int)(0.6 * outer.ClientSize.Width);
                int Y = (int)(0.6 * outer.ClientSize.Height);

                SlikeEnd = new Vanzemaljac[2];
                SlikeEnd[0] = new Vanzemaljac(new Point(X, Y), Properties.Resources.letjelica);
                SlikeEnd[1] = new Vanzemaljac(new Point(X, Y), Properties.Resources.eksplozija);
            }

            /// <summary>
            /// Izvojena formula za postavljanje veličine slika na početnoj formi ovisno o veličini forme.
            /// </summary>
            public void DefinirajVelicineSlika()
            {
                VelicinaSlikeStart = (int)(0.1 * outer.ClientSize.Width);
                VelicinaSlikeEnd = 3 * VelicinaSlikeStart;
            }
        }

        /// <summary>
        /// Objekt koji sadrži svojstva za početni izgled forme.
        /// </summary>
        FormaStartEnd formaStartEnd;
        /* ******************************************************************************* */

        /// <summary>
        /// Glavni timer igre
        /// </summary>
        System.Windows.Forms.Timer timer;

        /* ***** varijable vezane za igru ***** */
        int razina;
        int bodovi;
        int brZivotaLetjelice;
        double brzinaLetjelice;
        bool letjelicaIdeLijevo, letjelicaIdeDesno;
        /// <summary>
        /// Lista svih vanzemaljaca u igri.
        /// </summary>
        List<Vanzemaljac> vanzemaljci;
        double brzinaMetkaLetjelice;
        double brzinaMetkaVanzemaljca;
        /* ************************** */

        /* ***** pomoćne varijable ***** */
        bool isFormaStart;
        bool isFormaEnd;
        /* ***************************** */

        /* pamtimo početne dimenzije forme, i svih kontrola i slika, u slučaju Resize-a */
        /// <summary>
        /// Početna širina forme, radi računanja faktora skaliranja kod Resize-a.
        /// </summary>
        readonly int width;
        /// <summary>
        /// Početna visina forme, radi računanja faktora skaliranja kod Resize-a.
        /// </summary>
        readonly int height;
        /// <summary>
        /// Početne lokacije i dimenzije svih kontrola na formi osim metaka, za Resize forme.
        /// </summary>
        Dictionary<object, Rectangle> original;
        /* ********************************************************* */

        public Form1()
        {
            InitializeComponent();

            // Ovo je potrebno zbog Event-a s tipkovnicom
            this.KeyPreview = true;

            /* Za svaki slučaj ponoviti ove postavke i ovdje */
            /* iako smo ih postavili ručno u designer-u */
            foreach(Label l in Controls.OfType<Label>())
            {
                l.Anchor = AnchorStyles.None;
                l.TextAlign = ContentAlignment.MiddleCenter;
                // Važno zbog poravnavanja
                l.AutoSize = false;
                //l.Width = (int)(ClientSize.Width / (double)2.5 );
            }
          
            this.Paint += new PaintEventHandler(Form1_Paint_1);
            this.Resize += new EventHandler(Form1_Resize);

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);   
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Pamtimo početne dimnezije forme, radi Resize-a 
            width = ClientSize.Width;
            height = ClientSize.Height;
                
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Interval od 100 milisekundi
            timer.Tick += Timer_Tick;

            metakTimer.Tick += MetakTimer_Tick;                             /* *** DODANO - TREBALO JE OVDJE I INICIJALIZIRATI SVE TIMERE (kao što smo timer) DA SE IZBJEGNE GRESKA ******* */
            metakVanzemaljacTimer.Tick += MetakVanzemaljacTimer_Tick;       /* *** DODANO - I NAVESTI IH OVDJE NA VRHU KLASE Form1, A NE DALEKO DOLJE ************************************* */

            formaStartEnd = new FormaStartEnd(this);

            razina = 1;
            bodovi = 0;

            // Definiraj veličinu vanzemaljca s obzirom na dimenzije forme
            DefinirajVelicinuVanzemaljaca();
            vanzemaljci = new List<Vanzemaljac>();

            brzinaMetkaLetjelice = 10.0;
            brzinaMetkaVanzemaljca = 10.0;
        }

        /// <summary>
        /// Na Load forme treba se prikazati početna verzija forme.
        /// Pozivaju se redom: Constructor -> Resize -> Load -> Paint -> Shown
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            HorizontalnoCentrirajKontrole();

            ZapamtiOriginal();

            isFormaEnd = false;
            isFormaStart = true;
            // Na početku je prikazana početna forma
            PrikaziFormuStartEnd();

            // Timer za pomicanje slika vanzemaljaca
            formaStartEnd.Timer.Start();      
        }

        /// <summary>
        /// Pozicioniranje svih horizontalno centriranih kontrola na formi.
        /// </summary>
        private void HorizontalnoCentrirajKontrole()
        {
            int height = ClientSize.Height;
            int width = ClientSize.Width;

            /* *** Gdje ovo postaviti? *** */
            labelaRazina.Width = (int)(ClientSize.Width / 3.0);
            labelaNaslov.Width = labelaScore.Width = (int)(2.5 * gumbStart.Width);

            Dictionary<Control, int> tops = new Dictionary<Control, int>
            {
                { labelaRazina, 0 },
                { labelaNaslov, (int)(0.3 * height) },
                { labelaScore, (int)(0.45 * height) },
                { gumbStart, (int)(0.6 * height) },
                { letjelica, height - letjelica.Height - 10 }
            };

            foreach (var entry in tops)
            {
                entry.Key.Top = entry.Value;
                // Centriraj horizontalno na formi
                entry.Key.Left = (int)((width - entry.Key.Width) / 2.0);
            }
        }

        /// <summary>
        /// Spremanje pocetnih lokacija i dimenzija svih kontrola
        /// na formi, zbog Resize-a.
        /// </summary>
        private void ZapamtiOriginal()
        {
            original = new Dictionary<object, Rectangle>();
            foreach (Control c in Controls)
                original[c] = c.Bounds;     
        }

        /// <summary>
        /// Prikaz forme kad je pocetak igre, pocetak sljedece razine i game over.
        /// </summary>
        private void PrikaziFormuStartEnd()
        {
            labelaZivoti.Visible = false;
            labelaRazina.Visible = false;
            labelaBodovi.Visible = false;
            letjelica.Visible = false;

            /* Ostale postavke fonta postavljene ručno u designer-u */
            labelaNaslov.ForeColor = isFormaStart ? Color.Lime : Color.White;

            // isFormaStart/!isFormaEnd && bodovi < 500 <--> pocetak igre
            // isFormaStart && bodovi >= 500 <--> pocetak sljedece razine
            labelaNaslov.Text = isFormaEnd ? "GAME OVER" : (bodovi >= 500 ? "WELL DONE" : "Space Invaders");
            labelaNaslov.Visible = true;

            labelaScore.Text = (isFormaEnd || bodovi >= 500) ? "RAZINA: " + razina + "  BODOVI: " + bodovi : "";
            labelaScore.Visible = (isFormaEnd || bodovi >= 500) ? true : false; 

            gumbStart.Text = isFormaEnd ? "RESTART" : (bodovi >= 500 ? "SLJEDEĆA RAZINA" : "START");
            /* *** Fokus je sada na gumbu, a ne na formi *** */
            gumbStart.Visible = true;
        }

        /// <summary>
        /// Pomicanje slika vanzemaljaca na početnoj formi nakon isteka vremenskog intervala.
        /// Alterniranje slika na formi za Game Over nakon isteka vremenskog intervala.
        /// </summary>
        private void FormaStartEndTimer_Tick(object sender, EventArgs e)
        {
            if (isFormaStart)
                PomakniVanzemaljceNaFormiStart();
            else //sigurno je onda isFormaEnd
                AlternirajSlikeNaFormiEnd();

            // Da se forma ponovno iscrta (isFormaStart je true ili isFormaEnd je true)
            Invalidate();
        }

        /// <summary>
        ///  Pomicanje slika vanzemaljaca na početnoj formi.
        /// </summary>
        private void PomakniVanzemaljceNaFormiStart()
        {
            // Gornja slika se pomiče ulijevo pa udesno ako dotakne rub
            int newX1 = (int)(formaStartEnd.SlikaStart1.Pozicija.X + formaStartEnd.Smjer * formaStartEnd.Brzina);

            // Donja slika se pomiče istom brzinom u smjeru suprotnom od gornje
            int newX2 = (int)(formaStartEnd.SlikaStart2.Pozicija.X - formaStartEnd.Smjer * formaStartEnd.Brzina);

            // Provjera jesu li dotakli ili prešli rubove
            if (newX1 <= 0.1 * ClientSize.Width || newX1 + formaStartEnd.VelicinaSlikeStart >= 0.9 * ClientSize.Width)
            {
                // Ako prelaze rubove, pomaknuti maksimalno koliko preostaje prostora
                if (newX1 < 0.1 * ClientSize.Width)
                {
                    newX1 = (int)(0.1 * ClientSize.Width);
                    newX2 = (int)(0.9 * ClientSize.Width - formaStartEnd.VelicinaSlikeStart);
                }
                else if (newX1 + formaStartEnd.VelicinaSlikeStart > 0.9 * ClientSize.Width)
                {
                    newX1 = (int)(0.9 * ClientSize.Width - formaStartEnd.VelicinaSlikeStart);
                    newX2 = (int)(0.1 * ClientSize.Width);
                }

                // Promjena smjera kretanja
                formaStartEnd.Smjer *= -1;
                formaStartEnd.BrojHorizontalnihPomaka = 0;
            }
            else
                formaStartEnd.BrojHorizontalnihPomaka++;

            /* *** Problem je što je Pozicija struct, pa se ne može mijenjati direktno njezin X *** */
            formaStartEnd.SlikaStart1.Pozicija = new Point(newX1, formaStartEnd.SlikaStart1.Pozicija.Y);
            formaStartEnd.SlikaStart2.Pozicija = new Point(newX2, formaStartEnd.SlikaStart2.Pozicija.Y);
        }

        /// <summary>
        /// Alterniranje slike za Game Over formu između slike letjelice i slike eksplozije.
        /// </summary>
        private void AlternirajSlikeNaFormiEnd()
        {
            // abs(x - 1) ==> x = 1 --> x = 0, x = 0 --> x = 1
            formaStartEnd.IndeksSlikeEnd = Math.Abs(formaStartEnd.IndeksSlikeEnd - 1);
        }

        /// <summary>
        /// Iscrtavanje početne forme
        /// </summary>
        /// <param name="g">e.Graphics iz Form1_Paint</param>
        private void FormaStart_Paint(Graphics g)
        {
            // Crtanje okvira forme
            Pen pen = new Pen(Color.Lime, 10);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
            g.DrawRectangle(pen, 0, 0, ClientSize.Width, ClientSize.Height);   
            pen.Dispose();

            // Crtanje slika vanzemaljaca
            Size velicina = new Size(formaStartEnd.VelicinaSlikeStart, formaStartEnd.VelicinaSlikeStart);

            g.DrawImage(formaStartEnd.SlikaStart1.Slika, new Rectangle(formaStartEnd.SlikaStart1.Pozicija, velicina));  
            g.DrawImage(formaStartEnd.SlikaStart2.Slika, new Rectangle(formaStartEnd.SlikaStart2.Pozicija, velicina));
        }

        /// <summary>
        /// Pritiskom gumba (RE)START/SLJEDEĆA RAZINA započinjemo igru/sljedeću razinu.
        /// </summary>
        private void GumbStart_Click(object sender, EventArgs e)
        {
            PocetnePostavkeIgre();
        }

        /// <summary>
        /// Početne postavke igre i forme za igru, za početak igre i za početak sljedeće razine.
        /// </summary>
        private void PocetnePostavkeIgre()
        {
            formaStartEnd.Timer.Stop();
            formaStartEnd.InicijalizirajSlikeStart();

            // Ako smo prešli na sljedeću razinu 
            if (isFormaStart && bodovi >= 500)
            {
                // Povećavamo razinu za 1
                razina++;
            }
            // Ako je bio Game Over
            if(isFormaEnd)
            {
                // Resetiramo razinu
                razina = 1;
            }
            isFormaStart = false;
            isFormaEnd = false;

            labelaNaslov.Visible = false;
            labelaScore.Visible = false;
            gumbStart.Visible = false;

            labelaZivoti.Visible = true;
            labelaRazina.Visible = true;
            labelaBodovi.Visible = true;

            brZivotaLetjelice = 3;
            labelaZivoti.Text = "Životi: " + brZivotaLetjelice;
            labelaRazina.Text = "Razina: " + razina;
            // Bodovi ce ostati isti ako prelazimo na novu razinu, resetiraju se samo kod restartiranja igre
            if (razina == 1)
            {
                bodovi = 0;
            }
            labelaBodovi.Text = "Bodovi: " + bodovi;

            letjelica.Visible = true;

            letjelicaIdeLijevo = letjelicaIdeDesno = false;
            brzinaLetjelice = 15;

            // Vraćanje fokusa za evente s gumba na formu.
            this.Focus();
            //this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            //this.KeyUp += new KeyEventHandler(Form1_KeyUp);

            InicijalizirajVanzemaljce();

            // Brzina vanzemaljca je funkcija razine
            Vanzemaljac.Brzina = 2.0f + (razina - 1) * 2f; 

            timer.Start();

            // Vezano za ispaljivanje metaka vanzemaljaca
            metakVanzemaljacTimer.Interval = random.Next(2500, 5000);
            //metakVanzemaljacTimer.Tick += MetakVanzemaljacTimer_Tick;   /* ******* PREBACENO U KONSTRUKTOR FORME  ******************* */
            metakVanzemaljacTimer.Start();                                /* MOGLO SE OVDJE DODATI I metakTimer.Start() I ONDA RADIJE NAPRAVITI METODU StartajGlavneTimere() { sva tri .Start() } */

            // Vezano za ispaljivanje metaka letjelice
            metakTimer.Interval = 500;
            //metakTimer.Tick += MetakTimer_Tick;                          /* **** PREBACENO U KONSTRUKTOR FORME ********************* */
            zadnjeIspaljivanjeMetka = DateTime.MinValue;

            Invalidate();
        }

        /// <summary>
        /// Inicijalizacija/resetiranje liste vanzemaljaca za početak igre ili nove razine.
        /// </summary>
        private void InicijalizirajVanzemaljce()
        {
            // Vanzemaljci kreću od gornjeg lijevog kuta forme
            Vanzemaljac.PrviRedak = Vanzemaljac.PrviStupac = 0;
            // Početno udesno
            Vanzemaljac.SmjerKretanja = 1;
           
            vanzemaljci.Clear();

            // Ovo polje inicijaliziramo ponovno pri svakom inicijaliziranju vanzemaljaca
            int[] preostaloSlika = new int[5] { 10, 10, 10, 10, 10 };

            Random random = new Random();
            int indeksSlike;
            Image slika = null;

            int X, Y;
            Point pozicija;
            for (int i = 0; i < 50; i++)
            {
                // 10 vanzemaljaca jedan za drugim u svakom retku
                X = (i % 10) * Vanzemaljac.VelicinaPodrucja;
                // 5 je redaka vanzemaljaca jedan ispod drugog
                Y = (i / 10) * Vanzemaljac.VelicinaPodrucja;
                pozicija = new Point(X, Y);

                while (true)
                {
                    indeksSlike = random.Next(0, slike.Length);
                    if (indeksSlike < preostaloSlika.Length && preostaloSlika[indeksSlike] > 0)
                    {
                        preostaloSlika[indeksSlike]--;
                        break;
                    }
                }

                slika = slike[indeksSlike];

                vanzemaljci.Add(new Vanzemaljac(pozicija, slika)   
                {
                    // Svaki vanzemaljac počinje s jednim životom
                    Zivot = 1
                });
            }
        }

        /// <summary>
        /// Izdvojena formula za određivanje veličine vanzemaljca ovisno o veličini forme.
        /// </summary>
        private void DefinirajVelicinuVanzemaljaca()
        {
            // Zauzimaju ukupno 50% širine forme, a 10 ih je u jednom redu
            Vanzemaljac.VelicinaPodrucja = (int)(0.1 * ClientSize.Width / 2.0);
            Vanzemaljac.VelicinaSlike = (int)(0.8 * Vanzemaljac.VelicinaPodrucja);
        }

        /// <summary>
        /// Pomicanje flote vanzemaljaca i metaka letjelice, i eventualno letjelice, nakon isteka vremenskog intervala tijekom igre.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            PomakniFlotuVanzemaljaca();

            PomakniLetjelicu();

            PomakniMetkeLetjelice();

            // Provjera je li neki živi vanzemaljac došao u redak gdje je letjelica
            if (ProvjeriSudarSLetjelicom()) 
                GameOver();

            // Provjera je li letjelica pogodila nekog/neke žive vanzemaljce
            ProvjeriPogodakVanzemaljcaMetkom(); 

            // Provjera je li neki metak vanzemaljaca pogodio letjelicu
            ProvjeriPogodakLetjeliceMetkom(); 

            // Provjera je li letjelica ubila sve vanzemaljce
            if(ProvjeriPrelazakNaNovuRazinu()) 
                SljedecaRazina();

            // Ponovno iscrtavanje forme
            Invalidate(); 
        }

        /// <summary>
        /// Pomicanje flote vanzemaljaca za jedno mjesto.
        /// </summary>
        private void PomakniFlotuVanzemaljaca()
        {
            // Vanzemaljci se pomiču za jedan stupac ulijevo ili udesno
            Vanzemaljac.PrviStupac += Vanzemaljac.SmjerKretanja;

            // Ažuriranje pozicija vanzemaljaca

            bool promjenaSmjera = false;

            double horizontalniPomak = Vanzemaljac.SmjerKretanja * Vanzemaljac.Brzina * 0.1 * Vanzemaljac.VelicinaPodrucja;

            // Izdvajanje svih živih vanzemaljaca
            List<Vanzemaljac> ziviVanzemaljci = vanzemaljci.FindAll(vanzemaljac => vanzemaljac.Zivot > 0);

            // Vanzemaljci se kreću ulijevo - naći horizontalnu poziciju najljevijeg živog vanzemaljca
            if (Vanzemaljac.SmjerKretanja == -1)
            {
                int minX = ziviVanzemaljci.Min(vanzemaljac => vanzemaljac.Pozicija.X);
                int minNewX = (int)(minX + horizontalniPomak);
                // Provjera je li najljeviji živi vanzemaljac dosegnuo ili bi prešao rub
                if (minNewX <= 0)
                {
                    // Ako bi prešao rub, pomaknuti ulijavo maksimalno koliko preostaje prostora
                    if (minNewX < 0)
                        horizontalniPomak = -minX;

                    // Promjena smjera kretanja i pomak u redak ispod
                    promjenaSmjera = true;
                }
            }
            else // Vanzemaljci se kreću udesno - naći horizontalnu poziciju najdesnijeg živog vanzemaljca
            {
                int maxX = ziviVanzemaljci.Max(vanzemaljac => vanzemaljac.Pozicija.X);
                int maxRight = maxX + Vanzemaljac.VelicinaPodrucja;
                int maxNewRight = (int)(maxRight + horizontalniPomak);
                // Provjera je li najdesniji živi vanzemaljac dosegnuo ili bi prešao rub
                if (maxNewRight >= ClientSize.Width)
                {
                    // Ako bi prešao rub, pomaknuti udesno maksimalno koliko preostaje prostora
                    if (maxNewRight > ClientSize.Width)
                        horizontalniPomak = ClientSize.Width - maxRight;

                    // Promjena smjera kretanja i pomak u redak ispod
                    promjenaSmjera = true;
                }
            }

            // Ažuriranje horizontalnih pozicija
            int newX;
            foreach (Vanzemaljac vanzemaljac in vanzemaljci)
            {
                newX = (int)(vanzemaljac.Pozicija.X + horizontalniPomak);
                vanzemaljac.Pozicija = new Point(newX, vanzemaljac.Pozicija.Y);
            }

            if (promjenaSmjera)
            {
                // Promjena smjera kretanja
                Vanzemaljac.SmjerKretanja *= -1;
                // Vanzemaljci se spuštaju za jedan redak dolje
                Vanzemaljac.PrviRedak++;
                // Ažuriranje vertikalnih pozicija
                int newY;
                foreach (Vanzemaljac vanzemaljac in vanzemaljci)
                {
                    newY = (int)(vanzemaljac.Pozicija.Y + Vanzemaljac.VelicinaPodrucja);
                    vanzemaljac.Pozicija = new Point(vanzemaljac.Pozicija.X, newY);
                }
            }
        }

        /// <summary>
        /// Pomicanje letjelice za jedno mjesto ulijevo ili udesno.
        /// </summary>
        private void PomakniLetjelicu()
        {
            // Letjelica se može kretati ulijevo
            if (letjelicaIdeLijevo && !letjelicaIdeDesno && (int)(letjelica.Left - brzinaLetjelice) >= 0)  
                letjelica.Left = (int)(letjelica.Left - brzinaLetjelice);

            // Letjelica se može kretati udesno
            else if (letjelicaIdeDesno && !letjelicaIdeLijevo && (int)(letjelica.Right + brzinaLetjelice) <= ClientSize.Width)
                letjelica.Left = (int)(letjelica.Left + brzinaLetjelice);
        }

        /// <summary>
        /// Pomicanje svakog prikazanog metka letjleice za jedno mjesto gore.
        /// </summary>
        private void PomakniMetkeLetjelice()
        {
            foreach(Metak metakL in metci.FindAll(metak => (string)metak.MetakPictureBox.Tag == "metak_l"))
            {
                // Provjera za svaki slučaj
                if (!Controls.Contains(metakL.MetakPictureBox)) continue;

                // Dohvaćanje pripadne ekontrole na formi
                PictureBox metakLBox = metakL.MetakPictureBox;

                // Pomicanje pripadne kontrole prema gore, ovisno o brzini tog metka
                metakLBox.Top = (int)(metakLBox.Top - metakL.Brzina);

                // Uklanjanje metka letjelice kada izađe izvan ekrana
                if (metakLBox.Bottom < 0)
                {
                    // Uklanjanje objekta za taj metak iz liste metaka
                    metci.Remove(metakL);

                    // Uklanjanje kontrole za taj metak
                    Controls.Remove(metakLBox);
                    metakLBox.Dispose();

                    metakAktivan = false; // Oslobađanje metka nakon uklanjanja
                }

                metakL.BrojVertikalnihPomaka++;
            }
        }

        /// <summary>
        /// Provjera je li neki živi vanzemaljac došao u redak gdje je letjelica.
        /// </summary>
        /// <returns>Ako jest: true, ako nije: false</returns>
        private bool ProvjeriSudarSLetjelicom()
        {
            return vanzemaljci.FindAll(vanzemaljac => vanzemaljac.Zivot > 0).Exists(vanzemaljac =>
            {
                int vanzemaljacBottom = vanzemaljac.Pozicija.Y + Vanzemaljac.VelicinaPodrucja;
                return vanzemaljacBottom > letjelica.Top;
            });
        }

        /// <summary>
        /// Prikaz forme kad je Game Over i moguć restart igre.
        /// </summary>
        private void GameOver()
        {
            // Stopiramo glavni timer igre
            timer.Stop();

            // Stopiramo timer za stvaranje metaka vanzemaljaca
            metakVanzemaljacTimer.Stop();

            // Stopiramo timer za stvaranje metaka letjelice   /* ************ DODANO; TREBALO JE NAPRAVITI METODU: StopirajGlavneTimere() { SVA TRI .Stop(); } ************* */
            metakTimer.Stop();

            // Uklanjamo sve metke s forme
            ObrisiSveMetke();

            // Prikaz forme kad je Game Over
            isFormaEnd = true;
            PrikaziFormuStartEnd();

            // Pokrećemoo timer Start/End forme
            formaStartEnd.Timer.Start();

            // Namjerni Trigger događaja Paint
            Invalidate();
        }

        /// <summary>
        /// Iscrtavanje forme za Game Over.
        /// </summary>
        /// <param name="g">e.Graphics iz Form1_Paint</param>
        private void FormaEnd_Paint(Graphics g)
        {
            // Crtanje okvira forme
            Pen pen = new Pen(Color.Red, 10);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
            g.DrawRectangle(pen, 0, 0, ClientSize.Width, ClientSize.Height);  
            pen.Dispose();

            // Crtanje slike letjelice/eksplozije
            Size velicina = new Size(formaStartEnd.VelicinaSlikeEnd, formaStartEnd.VelicinaSlikeEnd);  

            Vanzemaljac slikaEnd = formaStartEnd.SlikeEnd[formaStartEnd.IndeksSlikeEnd];

            g.DrawImage(slikaEnd.Slika, new Rectangle(slikaEnd.Pozicija, velicina));  
        }

        private void Form1_Paint_1(object sender, PaintEventArgs e)  
        {
            Console.WriteLine("Form1_Paint_1 pozvan");

            Graphics g = e.Graphics;

            if (isFormaStart)
            {
                FormaStart_Paint(g);
                return;
            }

            if(isFormaEnd)
            {
                FormaEnd_Paint(g);
                return;
            }

            int slikaX, slikaY;
            Point pozicijaSlike;
            Size velicinaSlike;
            foreach (var vanzemaljac in vanzemaljci)
            {
                // Iscrtavanje samo vanzemaljaca s preostalim životom
                if (vanzemaljac.Zivot > 0) 
                {
                    slikaX = (int)(vanzemaljac.Pozicija.X + 0.1 * Vanzemaljac.VelicinaPodrucja);
                    slikaY = (int)(vanzemaljac.Pozicija.Y + 0.1 * Vanzemaljac.VelicinaPodrucja);

                    pozicijaSlike = new Point(slikaX, slikaY);
                    velicinaSlike = new Size(Vanzemaljac.VelicinaSlike, Vanzemaljac.VelicinaSlike);

                    g.DrawImage(vanzemaljac.Slika, new Rectangle(pozicijaSlike, velicinaSlike));
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        { 
            if (e.KeyCode == Keys.Left)
                letjelicaIdeLijevo = true;

            if (e.KeyCode == Keys.Right)
                letjelicaIdeDesno = true;

            if (e.KeyCode == Keys.Space && (DateTime.Now - zadnjeIspaljivanjeMetka).TotalMilliseconds >= 500)
            {
                if (!metakAktivan)
                {
                    StvoriMetakLetjelica();
                    metakTimer.Start();             /* ******************* OVO JE MOGLO IĆI U POcetnePOstavkeIgre() ****** */
                    zadnjeIspaljivanjeMetka = DateTime.Now;
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                letjelicaIdeLijevo = false;

            if (e.KeyCode == Keys.Right)
                letjelicaIdeDesno = false;
        }


        /* ::: METCI LETJELICE I VANZEMALJACA :::::::::::::::::::::::::::::::::::::::::::  */
        /* ****************************************************************************** */
        /// <summary>
        /// Objekt koji uz kontrolu koja je metak, sadrži i svojstva konkretnog metka.
        /// </summary>
        public class Metak
        {
            /// <summary>
            /// Kontrola na formi koja predstavlja metak.
            /// </summary>
            public PictureBox MetakPictureBox { get; set; }
            /// <summary>
            /// Brzina konkretnog metka.
            /// </summary>
            public double Brzina { get; set; }
            /// <summary>
            /// Lokacija na formi s koje je ovaj metak počeo padati - radi Resize-a forme
            /// </summary>
            public Point PocetnaLokacija { get; }
            /// <summary>
            /// Koliko se puta dosad metak pomaknuo za jedno mjesto dolje - radi Resize-a forme
            /// </summary>
            public int BrojVertikalnihPomaka { get; set; }

            public Metak(PictureBox metak, double brzina)
            {
                MetakPictureBox = metak;
                PocetnaLokacija = metak.Location;
                Brzina = brzina;
                BrojVertikalnihPomaka = 0;
            }
        }

        /// <summary>
        /// Lista koja sadrži svojstva svih trenutno aktivnih metaka u igri.
        /// </summary>
        private List<Metak> metci = new List<Metak>();
        /* ************************************************************************ */

        /* varijable za ispaljivanje metaka letjelice ********* */
        /// <summary>
        /// Praćenje stanja metka
        /// </summary>
        private bool metakAktivan = false; 
        /// <summary>
        /// Timer za kontrolu intervala ispaljivanja metaka letjelice
        /// </summary>
        private System.Windows.Forms.Timer metakTimer = new System.Windows.Forms.Timer();
        /// <summary>
        /// Kad je zadnje ispaljen metak letjelice.
        /// </summary>
        private DateTime zadnjeIspaljivanjeMetka;
        /* ******************************************************* */

        /// <summary>
        /// Funkcija za stvaranje metka letjelice.
        /// </summary>
        private void StvoriMetakLetjelica()
        {
            if (!metakAktivan) // Provjera zastavice prije stvaranja metka
            {
                // Stvaranje kontrole za taj metak
                PictureBox metak = new PictureBox();
                metak.Size = new Size(4, 10); // Promijenjena početna veličina metka
                metak.BackColor = Color.Lime; // Fluorescentno zelena boja
                metak.BorderStyle = BorderStyle.FixedSingle;
                // Postavljanje metka na vrh letjelice
                metak.Top = letjelica.Top - metak.Height;
                // Centriranje metka po širini letjelice
                metak.Left = letjelica.Left + (letjelica.Width / 2) - (metak.Width / 2); 
                metak.Tag = "metak_l";

                // Dodavanje kontrole za taj metak na formu
                Controls.Add(metak);
                metak.BringToFront();

                // Stvaranje objekta za taj metak i dodavanje u listu svih metaka
                Metak metakObj = new Metak(metak, brzinaMetkaLetjelice);
                metci.Add(metakObj);

                metakAktivan = true; // Postavljanje zastavice nakon stvaranja metka
            }
        }

        private void MetakTimer_Tick(object sender, EventArgs e)
        {
            // Resetiranje zastavice nakon 0.5 sekundi (tj. nakon isteka intervala)
            metakAktivan = false;
            // Zaustavljanje timera
            metakTimer.Stop();
        }

        // Definicija klase Vanzemaljac izvan klase Form1, ali unutar istog namespace-a
        public class Vanzemaljac
        {
            public Point Pozicija { get; set; }
            public Image Slika { get; set; }
            /* *** Zasad nije bool, jer možda mogu imati više života *** */
            public int Zivot { get; set; }

            // Svojstva ispod zajednička su svim vanzemaljcima 
            public static int VelicinaSlike { get; set; }    
            public static int VelicinaPodrucja { get; set; }  

            /// <summary>
            /// Indeks prvog "retka" na formi u kojem se nalaze vanzemaljci
            /// </summary>
            public static int PrviRedak { get; set; }
            /// <summary>
            /// Indeks prvog "stupca" na formi u kojem se nalaze vanzemaljci
            /// </summary>
            public static int PrviStupac { get; set; }

            /// <summary>
            /// 1 znači udesno, -1 znači ulijevo
            /// </summary>
            public static int SmjerKretanja { get; set; } 

            /// <summary>
            /// Brzina kretanja vanzemaljaca.
            /// </summary>
            public static double Brzina { get; set; }

            public Vanzemaljac(Point pozicija, Image slika)
            {
                Pozicija = pozicija;
                Slika = slika;
                // Svaki vanzemaljac počinje s 1 životom
                Zivot = 1;
            }
        }

        /// <summary>
        /// Dostupne slike vanzemaljaca - ažurirati po potrebi
        /// </summary>
        Image[] slike = new[]
        {
            Properties.Resources.vanzemaljac_1,
            Properties.Resources.vanzemaljac_2,
            Properties.Resources.vanzemaljac_3,
            Properties.Resources.vanzemaljac_4,
            Properties.Resources.vanzemaljac_5
        };

        /* varijable za ispaljivanje metaka vanzemaljaca *********** */
        Random random = new Random();
        /// <summary>
        /// Timer za ispaljivanje metaka od strane vanzemaljaca.
        /// </summary>
        System.Windows.Forms.Timer metakVanzemaljacTimer = new System.Windows.Forms.Timer();
        /* ********************************************************* */

        private async void MetakVanzemaljacTimer_Tick(object sender, EventArgs e)
        {
            StvoriMetakVanzemaljac();
            await Task.Delay(random.Next(100, 500)); // Nasumično čekanje između 200ms i 1000ms
            metakVanzemaljacTimer.Interval = random.Next(2000, 5000); // Interval između 2 i 5 sekundi
        }

        /// <summary>
        /// Stvaranje, tj ispaljivanje jednog metka jednog slučajno odabranog vanzemaljca koji je najdonji u svom stupcu.
        /// </summary>
        private void StvoriMetakVanzemaljac()
        { 
            // Lista u koju ćemo izdvojiti sve vanzemaljce koji su trenutno najdonji u svom stupcu
            List<Vanzemaljac> najdonjiZiviVanzemaljci = new List<Vanzemaljac>();

            // Lista u koju ćemo izdvojiti sve vanzemaljce iz jednog po jednog stupca
            List<Vanzemaljac> stupacVanzemaljaca = new List<Vanzemaljac>();

            Vanzemaljac najdonjiZiviVanzemaljac = null;
            for(int stupac = 0; stupac < 10; stupac++)
            {
                // U određenom stupcu je svaki 10. vanzemaljac iz početne liste, ukupno njih 5
                for (int redak = 0; redak < 5; redak++)
                {
                    stupacVanzemaljaca.Add(vanzemaljci[stupac + redak * 10]);
                }

                // Okrećemo redoslijed vanzemaljaca iz tog stupca, da najdonji u tom stupcu bude prvi u listi
                stupacVanzemaljaca.Reverse();

                // Sada tražimo prvog živog vanzemaljca u toj listi 
                najdonjiZiviVanzemaljac = stupacVanzemaljaca.Find(vanzemaljac => vanzemaljac.Zivot > 0);

                // Ako je takav pronađen, dodajemo ga u listu takvih za sve stupce
                if(najdonjiZiviVanzemaljac != null)
                {
                    najdonjiZiviVanzemaljci.Add(najdonjiZiviVanzemaljac);
                    najdonjiZiviVanzemaljac = null;
                }

                // Praznimo listu vanzemaljaca za novi stupac
                stupacVanzemaljaca.Clear();
            }

            if (najdonjiZiviVanzemaljci.Count == 0)
            {
                Console.WriteLine("Nema živih vanzemaljaca za ispaljivanje metaka.");
                return;
            }

            // Na slučajan način biramo jednog od živih vanzemaljaca koji su trenutno najdonji u svom stupcu
            Vanzemaljac vanzemaljacZaIspalitiMetak = najdonjiZiviVanzemaljci[random.Next(najdonjiZiviVanzemaljci.Count)];

            // Stvaramo kontrolu za njegov metak
            PictureBox metak = new PictureBox
            {
                Size = new Size(4, 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Top = vanzemaljacZaIspalitiMetak.Pozicija.Y + 10,
                Left = vanzemaljacZaIspalitiMetak.Pozicija.X + (Vanzemaljac.VelicinaSlike / 2) - 2,
                Tag = "metak_v"
            };
            
            // Dodajemo kontrolu za taj metak na formu
            Controls.Add(metak);
            metak.BringToFront();

            // Stvaramo objekt za taj metak i dodajemo ga u listu svih metaka
            Metak metakObj = new Metak(metak, brzinaMetkaVanzemaljca);
            metci.Add(metakObj);

            Console.WriteLine($"Metak ispaljen od vanzemaljca na poziciji ({vanzemaljacZaIspalitiMetak.Pozicija.X}, {vanzemaljacZaIspalitiMetak.Pozicija.Y})");

            System.Windows.Forms.Timer metakKretanjeTimer = new System.Windows.Forms.Timer
            {
                Interval = 20
            };
            metakKretanjeTimer.Tick += (s, e) => MetakKretanjeTimer_Tick(s, e, metakObj, metakKretanjeTimer);
            metakKretanjeTimer.Start();
        }

       
        private void MetakKretanjeTimer_Tick(object sender, EventArgs e, Metak metak, System.Windows.Forms.Timer timer)
        {
            // Dohvat kontrole za metak
            PictureBox metakBox = metak.MetakPictureBox;

            // Pomicanje kontrole prema dolje s obzirom na brzinu tog metka
            metakBox.Top = (int)(metakBox.Top + metak.Brzina); 

            // Provjera je li metak ispod granice prozora - ako da brisemo ga
            if (metakBox.Top > ClientSize.Height)
            {
                // Uklanjamo objekt za taj metak iz liste metaka
                metci.Remove(metak);

                // Uklanjamo kontrolu s forme
                Controls.Remove(metakBox);
                metakBox.Dispose();

                timer.Stop();
                timer.Dispose();
            }

            metak.BrojVertikalnihPomaka++;
        }

        /// <summary>
        /// Provjera je li letjelica pogođena metkom vanzemaljaca.
        /// </summary>
        private void ProvjeriPogodakLetjeliceMetkom()
        {
            Rectangle letjelicaRect = letjelica.Bounds;
            // Smanjujemo područje oko letjelice za koje se detektira pogodak
            letjelicaRect.Inflate(-2, -2);

            // Promatramo sve metke vanzemaljaca
            foreach (Metak metakV in metci.FindAll(metak => (string)metak.MetakPictureBox.Tag == "metak_v"))
            {
                // Provjera za svaki slučaj
                if (!Controls.Contains(metakV.MetakPictureBox)) continue;

                // Dohvaćanje pripadne kontrole na formi
                PictureBox metakVBox = metakV.MetakPictureBox;

                // Ako je metak pogodio letjelicu
                if (metakVBox.Bounds.IntersectsWith(letjelicaRect))
                {
                    // Uklanjamo objekt za taj metak iz liste metaka
                    metci.Remove(metakV);

                    // Uklanjamo kontrolu za taj metak s forme
                    Controls.Remove(metakVBox);
                    metakVBox.Dispose();

                    // Smanjujmeo broj života ketjelice za jedan
                    SmanjiBrojZivotaLetjelice();
                }
            }      
        }

        /// <summary>
        /// Oduzimanje jednog života letjelici. Provjera je li broj pao na 0 i trigger Game Over-a.
        /// </summary>
        private void SmanjiBrojZivotaLetjelice()
        {
            brZivotaLetjelice--;
            labelaZivoti.Text = "Životi: " + brZivotaLetjelice;
            if (brZivotaLetjelice <= 0)
            {
                GameOver();
            }
        }

        /// <summary>
        /// Provjera je li letjelica pogodila metkom/metcima nekog/neke žive vanzemaljce.
        /// </summary>
        private void ProvjeriPogodakVanzemaljcaMetkom()
        {
            // Promatramo sve metke letjelice
            foreach (Metak metakL in metci.FindAll(metak => (string)metak.MetakPictureBox.Tag == "metak_l"))
            {
                // Provjera za svaki slučaj
                if (!Controls.Contains(metakL.MetakPictureBox)) continue;

                // Dohvaćanje pripadne kontrole na formi
                PictureBox metakLBox = metakL.MetakPictureBox;

                // Promatramo sve ŽIVE vanzemaljce
                foreach (Vanzemaljac vanzemaljac in vanzemaljci.FindAll(vanzemaljac => vanzemaljac.Zivot > 0))
                {
                    Rectangle vanzemaljacRect = new Rectangle(vanzemaljac.Pozicija, new Size(Vanzemaljac.VelicinaPodrucja, Vanzemaljac.VelicinaPodrucja));

                    // Provjera je li ovaj metak letjelice ušao u područje tog vanzemaljca
                    if(metakLBox.Bounds.IntersectsWith(vanzemaljacRect))
                    {
                        // Ubijamo tog vanzemaljca
                        vanzemaljac.Zivot--;
                        // Dajemo bodove igraču, ovisno o razini
                        bodovi += razina * 100;
                        labelaBodovi.Text = "Bodovi: " + bodovi;

                        // Uklanjamo objekt za taj metak iz liste metaka
                        metci.Remove(metakL);

                        // Uklanjamo kontrolu za taj metak s forme
                        Controls.Remove(metakLBox);
                        metakLBox.Dispose();

                        // završavamo provjeru za ovaj metak letjelice
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Prikaz forme za prelazak na sljedeću razinu.
        /// </summary>
        public void SljedecaRazina()
        {
            // Stopiramo glavni timer igre
            timer.Stop();

            // Stopiramo timer za stvaranje metaka vanzemaljaca
            metakVanzemaljacTimer.Stop();

            // Stopiramo timer za stvaranje metaka letjelice   /* ************ DODANO; TREBALO JE NAPRAVITI METODU: StopirajTimere() { SVA TRI .Stop(); } ************* */
            metakTimer.Stop();

            ObrisiSveMetke();

            /* *** */
            //labelaNaslov.Font = new Font(labelaNaslov.Font.FontFamily, 14);

            isFormaStart = true;
            PrikaziFormuStartEnd();

            formaStartEnd.Timer.Start();

            Invalidate();
        }

        /// <summary>
        /// Provjera je li letjelica ubila sve vanzemaljce.
        /// </summary>
        /// <returns>Ako jest: true, ako nije: false.</returns>
        private bool ProvjeriPrelazakNaNovuRazinu() 
        {
            // Provjeravamo jesu li svi vanzemaljci eliminirani
            bool sviVanzemaljciEliminirani = vanzemaljci.All(v => v.Zivot == 0);

            if (sviVanzemaljciEliminirani && brZivotaLetjelice > 0)
            {
                Console.WriteLine($"Prelazak na razinu {razina + 1}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Brisanje svih metaka vanzemaljaca i letjelice.
        /// </summary>
        private void ObrisiSveMetke()
        {
            // Praznimo listu objekata s metcima
            metci.Clear();

            // Brišemo sve kontrole za metke s forme
            foreach (Control c in Controls.OfType<PictureBox>().Where(p => (string)p.Tag == "metak_l" || (string)p.Tag == "metak_v").ToList())
            {
                Controls.Remove(c);
                c.Dispose();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Računamo faktore za koje su se skalirale širina i visina forme u odnosu na početne vrijednosti
            double postotakWidth = (double) ClientSize.Width / width;
            double postotakHeight = (double) ClientSize.Height / height;

            // Skaliranje lokacija i veličina svih kontrola, čije su originalne dimenzije spremljene u original rječnik.
            foreach(Control c in Controls)
                if((string)c.Tag != "metak_l" && (string)c.Tag != "metak_v")
                {
                    c.Left = (int)(postotakWidth * original[c].Left);
                    c.Top = (int)(postotakHeight * original[c].Top);

                    double faktor = 1;
                    if (c == letjelica)
                        faktor = 0.7;
          
                    c.Width = (int)(faktor * postotakWidth * original[c].Width);
                    c.Height = (int)(faktor * postotakHeight * original[c].Height);     
                }
           
            // Skaliranje veličina vanzemaljaca
            SkalirajVelicinuVanzemaljaca();

            // Skaliranje veličina slika za formu Start i End
            //formaStartEnd.DefinirajVelicineSlika();

            // Skaliranje položaja slika na formi End s obzirom na veličinu forme
            SkalirajSlikeEnd();

            // Prikazana je forma Start
            if (isFormaStart)
            {
                // Skaliranje položaja slika na formi Start s obzirom na veličinu forme
                SkalirajSlikeStart();
            }
            else if(!isFormaStart && !isFormaEnd)// Ako traje igra
            {
                // Imamo flotu vanzemaljaca incijaliziranu za tu igru
                SkalirajFlotuvVanzemaljaca();

                // Imamo nepraznu listu metaka koji su sad u igri
                SkalirajSveMetke();
            }
         
            Invalidate();  
        }

        /// <summary>
        /// Skaliranje flote vanzemaljaca na Resize forme
        /// </summary>
        private void SkalirajFlotuvVanzemaljaca()
        {
            int horizontalniPomak = (int)(Vanzemaljac.Brzina * 0.1 * Vanzemaljac.VelicinaPodrucja);
            int vertikalniPomak = Vanzemaljac.VelicinaPodrucja;

            // Ažuriranje pozicija svih vanzemaljaca
            int X, Y;
            for (int i = 0; i < vanzemaljci.Count; i++)
            {
                X = (i % 10) * Vanzemaljac.VelicinaPodrucja + Vanzemaljac.PrviStupac * horizontalniPomak;
                Y = (i / 10) * Vanzemaljac.VelicinaPodrucja + Vanzemaljac.PrviRedak * vertikalniPomak;

                vanzemaljci[i].Pozicija = new Point(X, Y);
            }
        }

        /// <summary>
        /// Skaliranje položaja slika na formi Start s obzirom na veličinu forme
        /// </summary>
        private void SkalirajSlikeStart()
        {
            // Dosadašnji horizontalni pomak od posljednje promejene smjera
            int horizontalniPomak = (int)(formaStartEnd.BrojHorizontalnihPomaka * formaStartEnd.Brzina);
            // Ako se gornja slika kreće udesno, a donja ulijevo
            int X1, X2;
            if (formaStartEnd.Smjer == 1)
            {
                // Položaj gornje slike vanzemaljca 
                X1 = (int)(0.1 * ClientSize.Width + horizontalniPomak);
                // Položaj donje slike vanzemaljca
                X2 = (int)(0.9 * ClientSize.Width - formaStartEnd.VelicinaSlikeStart - horizontalniPomak);
            }
            // Ako se donja slika kreće ulijevo, a gornja udesno
            else //formaStartEnd.Smjer == -1
            {
                // Položaj gornje slike vanzemaljca 
                X1 = (int)(0.9 * ClientSize.Width - formaStartEnd.VelicinaSlikeStart - horizontalniPomak);
                // Položaj donje slike vanzemaljca
                X2 = (int)(0.1 * ClientSize.Width + horizontalniPomak);
             
            }

            int Y1 = (int)(0.1 * ClientSize.Height);
            formaStartEnd.SlikaStart1.Pozicija = new Point(X1, Y1);
            int Y2 = (int)(0.8 * ClientSize.Height);
            formaStartEnd.SlikaStart2.Pozicija = new Point(X2, Y2);
        }

        /// <summary>
        /// Skaliranje položaja slika na formi End s obzirom na veličinu forme
        /// </summary>
        private void SkalirajSlikeEnd()
        {
            foreach(Vanzemaljac slika in formaStartEnd.SlikeEnd)
            {
                // Slike kad je Game Over u donjem desnom kutu
                int X = (int)(0.8 * ClientSize.Width);
                int Y = (int)(0.8 * ClientSize.Height);
                slika.Pozicija = new Point(X, Y);
            }
        }
        private void SkalirajSveMetke()
        {
            foreach(Metak metak in metci)
            {
                // Dohvaćamo pripadnu kontrolu na formi
                PictureBox mBox = metak.MetakPictureBox;

                // Dosadašnji vertikalni pomak od početne pozicije na kojoj je stvoren metakl
                int vertikalniPomak = (int)(metak.BrojVertikalnihPomaka * metak.Brzina);

                if ((string)mBox.Tag == "metak_l")
                {
                    // Metci letjelice su se pomicali prema gore
                    mBox.Top = (int)(metak.PocetnaLokacija.Y - vertikalniPomak);
                }
                else if ((string)mBox.Tag == "metak_v")
                {
                    // Metci vanzemaljaca su se pomicali prema dolje
                    mBox.Top = (int)(metak.PocetnaLokacija.Y + vertikalniPomak);
                }
            }
        }

        private void SkalirajVelicinuVanzemaljaca()
        {
            if (ClientSize.Width > 3 * width)
            {
                Vanzemaljac.VelicinaPodrucja = (int)(0.7 * 0.1 * ClientSize.Width / 2.0);
                Vanzemaljac.VelicinaSlike = (int)(0.8 * Vanzemaljac.VelicinaPodrucja);
            }
            else
                DefinirajVelicinuVanzemaljaca();
        }
    }
}

