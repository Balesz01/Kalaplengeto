using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Xml.Linq;

namespace KalaplengetoDB
{
    internal class Program
    {
        static string ConnectionString = "SERVER=localhost;DATABASE=kalaplengeto;UID=root;PASSWORD=;";

        static void Main(string[] args)
        {
            List<Versenyzo> versenyzok = new List<Versenyzo>();

            BetoltVersenyzok(versenyzok);
            MenuFuttatas(versenyzok);
            HTMLGeneralas(versenyzok);
        }

        /// <summary>
        /// Megjeleníti a főmenüt és kezeli a felhasználói navigációt a nyilak és az Enter segítségével.
        /// </summary>
        static void MenuFuttatas(List<Versenyzo> versenyzok)
        {
            int currentPoint = 0;
            bool running = true;

            string[] menu = {
                "Versenyzők listázása",
                "Új versenyző felvétele",
                "Adatok módosítása",
                "Adatok törlése",
                "Kilépés"
            };

            while (running)
            {
                Console.Clear();

                for (int i = 0; i < menu.Length; i++)
                {
                    Console.WriteLine(i == currentPoint ? $"> {menu[i]}" : $"  {menu[i]}");
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentPoint > 0) currentPoint--;
                        break;

                    case ConsoleKey.DownArrow:
                        if (currentPoint < menu.Length - 1) currentPoint++;
                        break;

                    case ConsoleKey.Enter:
                        switch (currentPoint)
                        {
                            case 0:
                                VersenyzokKiirasa(versenyzok);
                                break;

                            case 1:
                                UjVersenyzoFelvetele(versenyzok);
                                break;

                            case 2:
                                Modositas(versenyzok);
                                break;

                            case 3:
                                Torles(versenyzok);
                                break;

                            case 4:
                                running = false;
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Feltölti a megadott listát az adatbázisból lekérdezett versenyzőkkel, pilla_hely szerint rendezve.
        /// </summary>
        static void BetoltVersenyzok(List<Versenyzo> versenyzok)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM versenyzok ORDER BY pilla_hely ASC";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    versenyzok.Add(new Versenyzo(
                        reader.GetInt32("id"),
                        reader.GetString("nev"),
                        reader.GetInt32("lengetes1"),
                        reader.GetDouble("ido1"),
                        reader.GetInt32("lengetes2"),
                        reader.GetDouble("ido2"),
                        reader.GetInt32("lengetes3"),
                        reader.GetDouble("ido3"),
                        reader.GetInt32("legjobb_pont"),
                        reader.GetDouble("legjobb_ido"),
                        reader.GetInt32("pilla_hely")
                    ));
                }
            }
        }

        /// <summary>
        /// Kilistázza a konzolra az összes tárolt versenyző legfontosabb eredményeit.
        /// </summary>
        static void VersenyzokKiirasa(List<Versenyzo> versenyzok)
        {
            Console.Clear();
            foreach (var v in versenyzok)
            {
                Console.WriteLine(
                    $"ID: {v.Id}, Név: {v.Nev}; " +
                    $"Legjobb pont: {v.Legjobb_pont}; " +
                    $"Legjobb idő: {v.Legjobb_ido}; " +
                    $"Hely: {v.Pilla_hely}"
                );
            }

            Console.WriteLine("\nEnter a visszalépéshez...");
            Console.ReadLine();
        }

        /// <summary>
        /// Bekéri az új versenyző adatait, kiszámítja az eredményeit és elmenti a listába, valamint az adatbázisba.
        /// </summary>
        static void UjVersenyzoFelvetele(List<Versenyzo> versenyzok)
        {
         
            try
            {
                Console.Clear();
                Console.Write("A versenyző neve: ");
                string nev = Console.ReadLine();

                Console.Write("Az első idő: ");
                double ido1 = double.Parse(Console.ReadLine());
                Console.Write("A második idő: ");
                double ido2 = double.Parse(Console.ReadLine());
                Console.Write("A harmadik idő: ");
                double ido3 = double.Parse(Console.ReadLine());

                int legjobbpontröv = legjobbpont(szamolpont(ido1), szamolpont(ido2), szamolpont(ido3));

                Versenyzo ujversenyzo = new Versenyzo
                {
                    Id = idje(versenyzok),
                    Nev = nev,
                    Lengetes1 = szamolpont(ido1),
                    Ido1 = ido1,
                    Lengetes2 = szamolpont(ido2),
                    Ido2 = ido2,
                    Lengetes3 = szamolpont(ido3),
                    Ido3 = ido3,
                    Legjobb_pont = legjobbpontröv,
                    Legjobb_ido = legjobbido(ido1, ido2, ido3),
                };

                versenyzok.Add(ujversenyzo);

                lengetespontfix(versenyzok);
                pillahelymegváltoztatas(versenyzok);
                AdatbazisFrissites(versenyzok);
                HTMLGeneralas(versenyzok);
            }
            catch (Exception)
            {
                Console.WriteLine("Hibás input! Próbáld újra!");
                Console.ReadLine();
            }
        }

        #region módosítás
        /// <summary>
        /// Bekéri egy meglévő versenyző nevét és új adatait, majd frissíti a rekordot a listában és az adatbázisban is.
        /// </summary>
        static void Modositas(List<Versenyzo> versenyzok)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);

                connection.Open();
                Console.Clear();
                Console.WriteLine("***Felhasználó Módosítása ***\n");
                foreach (var v in versenyzok)
                {
                    Console.WriteLine(
                        $"ID: {v.Id}, Név: {v.Nev}; " +
                        $"Legjobb pont: {v.Legjobb_pont}; " +
                        $"Legjobb idő: {v.Legjobb_ido}; " +
                        $"Hely: {v.Pilla_hely}"
                    );
                }

                Console.Write("\nA versenyző neve: ");
                string nev = Console.ReadLine();
                Console.Write("Az első idő: ");
                double ido1 = double.Parse(Console.ReadLine());
                Console.Write("A második idő: ");
                double ido2 = double.Parse(Console.ReadLine());
                Console.Write("A harmadik idő: ");
                double ido3 = double.Parse(Console.ReadLine());

                int legjobbpontröv = legjobbpont(szamolpont(ido1), szamolpont(ido2), szamolpont(ido3));

                int darab = 0;
                foreach (var v in versenyzok)
                {
                    if (v.Nev == nev)
                    {
                        break;
                    }
                    else
                    {
                        darab++;
                    }
                }

                var Id = versenyzok[darab].Id;
                var Nev = nev;
                var Lengetes1 = szamolpont(ido1);
                var Ido1 = ido1;
                var Lengetes2 = szamolpont(ido2);
                var Ido2 = ido2;
                var Lengetes3 = szamolpont(ido3);
                var Ido3 = ido3;
                var Legjobb_pont = legjobbpontröv;
                var Legjobb_ido = legjobbido(ido1, ido2, ido3);

                var Pilla_hely = versenyzok[darab].Pilla_hely;

                Versenyzo beillesztversenyzo = new Versenyzo(Id, Nev, Lengetes1, ido1, Lengetes2, ido2, Lengetes3, Ido3, Legjobb_pont, Legjobb_ido, Pilla_hely);
                versenyzok.RemoveAt(darab);
                versenyzok.Insert(darab, beillesztversenyzo);
                pillahelymegváltoztatas(versenyzok);
                AdatbazisFrissites(versenyzok);

                string modositSQL = "UPDATE `versenyzok` SET nev = @nev, lengetes1 = @l1, ido1 = @i1, lengetes2 = @l2, ido2 = @i2, lengetes3 = @l3, ido3 = @i3, legjobb_pont = @lp, legjobb_ido = @li, pilla_hely = @ph WHERE id = @id";
                MySqlCommand modositascommand = new MySqlCommand(modositSQL, connection);

                modositascommand.Parameters.AddWithValue("@id", Id);
                modositascommand.Parameters.AddWithValue("@nev", Nev);
                modositascommand.Parameters.AddWithValue("@l1", Lengetes1);
                modositascommand.Parameters.AddWithValue("@i1", Ido1);
                modositascommand.Parameters.AddWithValue("@l2", Lengetes2);
                modositascommand.Parameters.AddWithValue("@i2", Ido2);
                modositascommand.Parameters.AddWithValue("@l3", Lengetes3);
                modositascommand.Parameters.AddWithValue("@i3", Ido3);
                modositascommand.Parameters.AddWithValue("@lp", Legjobb_pont);
                modositascommand.Parameters.AddWithValue("@li", Legjobb_ido);
                modositascommand.Parameters.AddWithValue("@ph", versenyzok[darab].Pilla_hely);
                modositascommand.ExecuteNonQuery();
                connection.Close();
                HTMLGeneralas(versenyzok);
            }
            catch (Exception)
            {
                Console.WriteLine("Hibás input! Próbáld újra!");
                Console.ReadLine();
            }
        }
        #endregion

        #region törlés
        /// <summary>
        /// Név alapján megkeresi és törli a versenyzőt a memóriában tárolt listából és az adatbázisból is.
        /// </summary>
        static void Torles(List<Versenyzo> versenyzok)
        {
            try { 
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();
                Console.Clear();
                Console.WriteLine("***Felhasználó törlése ***\n");
                foreach (var v in versenyzok)
                {
                    Console.WriteLine(
                        $"ID: {v.Id}, Név: {v.Nev}; " +
                        $"Legjobb pont: {v.Legjobb_pont}; " +
                        $"Legjobb idő: {v.Legjobb_ido}; " +
                        $"Hely: {v.Pilla_hely}"
                    );
                }

                Console.Write("\nA felhasználó neve: ");
                string nev1 = Console.ReadLine();
                string delete = "DELETE FROM `versenyzok` WHERE Nev = @nev";
                MySqlCommand deletecommand = new MySqlCommand(delete, connection);
                deletecommand.Parameters.AddWithValue("@nev", nev1);
                int darab = 0;
                foreach (var v in versenyzok)
                {
                    if (v.Nev == nev1)
                    {
                        break;
                    }
                    else
                    {
                        darab++;
                    }
                }
                versenyzok.RemoveAt(darab);
                deletecommand.ExecuteNonQuery();

                pillahelymegváltoztatas(versenyzok);
                AdatbazisFrissites(versenyzok);
                connection.Close();
                HTMLGeneralas(versenyzok);
            }
            catch (Exception)
            {
                Console.WriteLine("Hibás input! Próbáld újra!");
                Console.ReadLine();
            }
        }
        #endregion

        /// <summary>
        /// Törli az adatbázis tábla teljes tartalmát, majd a lista aktuális elemeit egyesével újra beszúrja.
        /// </summary>
        static void AdatbazisFrissites(List<Versenyzo> versenyzok)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                MySqlCommand truncate = new MySqlCommand("TRUNCATE TABLE versenyzok;", connection);
                truncate.ExecuteNonQuery();

                string sql = @"INSERT INTO versenyzok (id, nev, lengetes1, ido1, lengetes2, ido2, lengetes3, ido3, legjobb_pont, legjobb_ido, pilla_hely) VALUES (@id,@nev,@l1,@i1,@l2,@i2,@l3,@i3,@lp,@li,@ph);";

                foreach (var v in versenyzok)
                {
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", v.Id);
                    cmd.Parameters.AddWithValue("@nev", v.Nev);
                    cmd.Parameters.AddWithValue("@l1", v.Lengetes1);
                    cmd.Parameters.AddWithValue("@i1", v.Ido1);
                    cmd.Parameters.AddWithValue("@l2", v.Lengetes2);
                    cmd.Parameters.AddWithValue("@i2", v.Ido2);
                    cmd.Parameters.AddWithValue("@l3", v.Lengetes3);
                    cmd.Parameters.AddWithValue("@i3", v.Ido3);
                    cmd.Parameters.AddWithValue("@lp", v.Legjobb_pont);
                    cmd.Parameters.AddWithValue("@li", v.Legjobb_ido);
                    cmd.Parameters.AddWithValue("@ph", v.Pilla_hely);
                    cmd.ExecuteNonQuery();
                }
            }
            HTMLGeneralas(versenyzok);
        }

        /// <summary>
        /// Generálja a Szélesbálási Kalaplengető Verseny aktuális állását HTML formátumban.
        /// </summary>
        static void HTMLGeneralas(List<Versenyzo> versenyzok)
        {
            // Rendezzük a versenyzőket helyezés szerint
            var rendezettLista = versenyzok.OrderBy(v => v.Pilla_hely).ToList();

            // HTML fejléc + CSS + header
            string htmlEleje = @"<!DOCTYPE html>
            <html lang=""hu"">
            <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Szélesbálási Kalaplengető Verseny</title>
            <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"" rel=""stylesheet"">
            <link href=""https://fonts.googleapis.com/css2?family=Merriweather:ital,wght@0,400;0,900;1,400&family=Rye&display=swap"" rel=""stylesheet"">
            <style>
            body { font-family: 'Merriweather', serif; background-color: #f4e1d2; color: #5d4037; margin:0; padding:0; }
            header { display:flex; justify-content:space-around; align-items:center; background-color:#a0522d; padding:10px 0; border-bottom:5px solid #5d4037; }
            .title-box { text-align:center; font-family:'Rye', cursive; color:#ffebcd; }
            h1 { font-size:3rem; line-height:1; margin-bottom:5px; }
            .sub-title { font-size:1.2rem; font-style:italic; }
            .center-img { width:100px; margin-top:10px; border:3px solid #5d4037; }
            .side-img { width:120px; height:120px; object-fit:cover; border:5px solid #5d4037; border-radius:50%; }

            .table-responsive-scroll { max-height:500px; overflow-y:hidden; border:5px solid #5d4037; max-width:100%; margin:0 auto; }
            table { width:100%; table-layout:fixed; }
            .table-responsive-scroll thead th { position:sticky; top:0; z-index:10; background-color:#a0522d !important; color:#ffebcd; font-family:'Rye', cursive; }
            .table td { vertical-align:middle; white-space:normal; padding:0.5rem; }
            .rank-badge { display:inline-block; width:30px; height:30px; line-height:30px; border-radius:50%; text-align:center; font-weight:bold; color:#5d4037; }
            footer { text-align:center; padding:10px; background-color:#a0522d; color:#ffebcd; font-size:0.8rem; margin-top:20px; }
            </style>
            </head>
            <body>

            <header>
            <img src=""traktor.jpg"" class=""side-img"">
            <div class=""title-box"">
            <h1>Szélesbálási<br>Kalaplengető Viadal</h1>
            <div class=""sub-title"">~ A Faluház Dísztermében ~</div>
            <img src=""hurka.jpg"" class=""center-img"">
            </div>
            <img src=""sör.jpg"" class=""side-img"">
            </header>

            <main class=""container-fluid mt-4 px-2"">
            <div class=""text-center fst-italic mb-3"">
            ""Aki a gyertyát a leggyorsabban eloltja, annak torka nem marad szárazon!""
            </div>

            <div id=""scroll-container"" class=""table-responsive-scroll"">
            <table class=""table table-striped table-bordered mb-0"">
            <thead>
            <tr>
            <th width=""8%"">Hely</th>
            <th>A Vitéz Neve</th>
            <th>I. Kör</th>
            <th>II. Kör</th>
            <th>III. Kör</th>
            <th>Legjobb</th>
            </tr>
            </thead>
            <tbody>";

            string htmlVege = @"
            </tbody>
            </table>
            </div>
            </main>

            <footer>
            <p>A hiteles időmérést a <strong>Falu Jegyzőjének Okosórája</strong> szavatolja.</p>
            <p style=""opacity:0.7"">2025 &copy; Szélesbálás</p>
            </footer>

            <script>
            const container = document.getElementById('scroll-container');
            let scrollSpeed = 1; // px/iteráció
            let delay = 50; // ms

            function autoScroll() {
                container.scrollTop += scrollSpeed;
                if (container.scrollTop + container.clientHeight >= container.scrollHeight) {
                    container.scrollTop = 0;
                }
            }

            setInterval(autoScroll, delay);
            </script>

            </body>
            </html>";

            // Táblázat sorok generálása
            string sorok = "";
            foreach (var v in rendezettLista)
            {
                string badgeStyle = "";
                string sorOsztaly = "table-light";

                if (v.Pilla_hely == 1)
                {
                    badgeStyle = "background:#ffd700;border:2px solid #b8860b;";
                    sorOsztaly = "table-warning";
                }
                else if (v.Pilla_hely == 2)
                {
                    badgeStyle = "background:#c0c0c0;border:2px solid #7f8c8d;";
                    sorOsztaly = "table-secondary";
                }
                else if (v.Pilla_hely == 3)
                {
                    badgeStyle = "background:#cd7f32;border:2px solid #a0522d;";
                }

                string badge = v.Pilla_hely <= 3
                    ? $"<div class=\"rank-badge\" style=\"{badgeStyle}\">{v.Pilla_hely}.</div>"
                    : v.Pilla_hely + ".";

                sorok += $@"
                <tr class=""{sorOsztaly}"">
                <td class=""text-center"">{badge}</td>
                <td>{v.Nev}</td>
                <td>{v.Lengetes1} p <small>({v.Ido1} mp)</small></td>
                <td>{v.Lengetes2} p <small>({v.Ido2} mp)</small></td>
                <td>{v.Lengetes3} p <small>({v.Ido3} mp)</small></td>
                <td><strong>{v.Legjobb_pont}</strong> p <small>({v.Legjobb_ido} mp)</small></td>
                </tr>";
            }

            string projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..");
            string fullPath = Path.Combine(projectRoot, "kalaplengeto.html");

            File.WriteAllText(fullPath, htmlEleje + sorok + htmlVege, Encoding.UTF8);
        }

        /// <summary>
        /// Kiszámítja a kapott pontot: 10-ből levonja az időt és lefelé kerekíti. Minimum 0 pont.
        /// </summary>
        static int szamolpont(double ido)
        {
            if (ido >= 10)
                return 0;
            return (int)Math.Floor(10 - ido);
        }

        /// <summary>
        /// Visszaadja a három megadott pontszám közül a legnagyobbat.
        /// </summary>
        static int legjobbpont(int pont1, int pont2, int pont3)
        {
            return Math.Max(pont1, Math.Max(pont2, pont3));
        }

        /// <summary>
        /// Visszaadja a három megadott időeredmény közül a legkisebbet (legjobbat).
        /// </summary>
        static double legjobbido(double ido1, double ido2, double ido3)
        {
            return Math.Min(ido1, Math.Min(ido2, ido3));
        }

        /// <summary>
        /// Kiszámítja egy konkrét versenyző helyezését a többiekhez képest pontszám, majd idő alapján.
        /// </summary>
        static int helyezes(Versenyzo helyezesversenyzo, List<Versenyzo> versenyzok)
        {
            int helyezese = 1;
            string[] nevek = new string[versenyzok.Count]; 
            for (int i = 0; i < versenyzok.Count; i++)
            {
                nevek[i] = versenyzok[i].Nev; 
            }
            Array.Sort(nevek);

            foreach (var v in versenyzok)
            {
                if (v.Id != helyezesversenyzo.Id)
                {
                    if (v.Legjobb_pont > helyezesversenyzo.Legjobb_pont)
                        helyezese++;
                    else if (v.Legjobb_pont == helyezesversenyzo.Legjobb_pont && v.Legjobb_ido < helyezesversenyzo.Legjobb_ido)
                        helyezese++;
                    else if (v.Legjobb_pont == helyezesversenyzo.Legjobb_pont && v.Legjobb_ido == helyezesversenyzo.Legjobb_ido && Array.IndexOf(nevek, helyezesversenyzo.Nev) < Array.IndexOf(nevek, v.Nev))
                        helyezese++;
                }
            }
            return helyezese;
        }

        /// <summary>
        /// Új azonosítót generál a lista hossza alapján.
        /// </summary>
        static int idje(List<Versenyzo> versenyzok)
        {
            if (versenyzok.Count == 0)
                return 1;
            return versenyzok.Count + 1;
        }

        /// <summary>
        /// Végigfrissíti a teljes lista minden tagjának a pilla_hely (helyezés) értékét.
        /// </summary>
        static void pillahelymegváltoztatas(List<Versenyzo> versenyzok)
        {
            foreach (var v in versenyzok)
                v.Pilla_hely = helyezes(v, versenyzok);
        }

        /// <summary>
        /// Újraszámolja a lengetési pontokat és a legjobb eredményeket a lista minden versenyzőjénél.
        /// </summary>
        static void lengetespontfix(List<Versenyzo> versenyzok)
        {
            foreach (var v in versenyzok)
            {
                v.Lengetes1 = szamolpont(v.Ido1);
                v.Lengetes2 = szamolpont(v.Ido2);
                v.Lengetes3 = szamolpont(v.Ido3);
                v.Legjobb_pont = legjobbpont(v.Lengetes1, v.Lengetes2, v.Lengetes3);
                v.Legjobb_ido = legjobbido(v.Ido1, v.Ido2, v.Ido3);
            }
        }
    }

    /// <summary>
    /// Egy versenyző adatait és eredményeit reprezentáló osztály.
    /// </summary>
    public class Versenyzo
    {
        public int Id { get; set; }
        public string Nev { get; set; }
        public int Lengetes1 { get; set; }
        public double Ido1 { get; set; }
        public int Lengetes2 { get; set; }
        public double Ido2 { get; set; }
        public int Lengetes3 { get; set; }
        public double Ido3 { get; set; }
        public int Legjobb_pont { get; set; }
        public double Legjobb_ido { get; set; }
        public int Pilla_hely { get; set; }

        public Versenyzo() { }

        public Versenyzo(int id, string nev, int l1, double i1, int l2, double i2, int l3, double i3, int lp, double li, int ph)
        {
            Id = id;
            Nev = nev;
            Lengetes1 = l1;
            Ido1 = i1;
            Lengetes2 = l2;
            Ido2 = i2;
            Lengetes3 = l3;
            Ido3 = i3;
            Legjobb_pont = lp;
            Legjobb_ido = li;
            Pilla_hely = ph;
        }
    }
}