using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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
        }

        // =========================
        // MENÜ
        // =========================
        static void MenuFuttatas(List<Versenyzo> versenyzok)
        {
            int currentPoint = 0;
            bool running = true;

            string[] menu = {
                "Versenyzők listázása",
                "Új versenyző felvétele",
                "Kilépés"
            };

            while (running)
            {
                Console.Clear();

                for (int i = 0; i < menu.Length; i++)
                {
                    Console.WriteLine(i == currentPoint ? $"> {menu[i]}": $"  {menu[i]}");
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
                                running = false;
                                break;
                        }
                        break;
                }
            }
        }

        // =========================
        // ADATBETÖLTÉS
        // =========================
        static void BetoltVersenyzok(List<Versenyzo> versenyzok)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM versenyzok ORDER BY nev ASC";
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


        // =========================
        // LISTÁZÁS
        // =========================
        static void VersenyzokKiirasa(List<Versenyzo> versenyzok)
        {
            Console.Clear();

            foreach (var v in versenyzok)
            {
                Console.WriteLine(
                    $"ID: {v.Id}, Név: {v.Nev}, " +
                    $"Legjobb pont: {v.Legjobb_pont}, " +
                    $"Legjobb idő: {v.Legjobb_ido}, " +
                    $"Hely: {v.Pilla_hely}"
                );
            }

            Console.WriteLine("\nEnter a visszalépéshez...");
            Console.ReadLine();
        }

        // =========================
        // ÚJ VERSENYZŐ
        // =========================
        static void UjVersenyzoFelvetele(List<Versenyzo> versenyzok)
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

            int legjobbpontröv =
                legjobbpont(szamolpont(ido1), szamolpont(ido2), szamolpont(ido3));

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
        }

        // =========================
        // ADATBÁZIS FRISSÍTÉS
        // =========================
        static void AdatbazisFrissites(List<Versenyzo> versenyzok)
        {
            using (MySqlConnection connection =
                new MySqlConnection(ConnectionString))
            {
                connection.Open();

                MySqlCommand truncate =
                    new MySqlCommand("TRUNCATE TABLE versenyzok;", connection);
                truncate.ExecuteNonQuery();

                string sql = @"INSERT INTO versenyzok
        (id, nev, lengetes1, ido1, lengetes2, ido2, lengetes3, ido3,
         legjobb_pont, legjobb_ido, pilla_hely)
         VALUES (@id,@nev,@l1,@i1,@l2,@i2,@l3,@i3,@lp,@li,@ph);";

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
        }


        // =========================
        // AZ EREDETI FÜGGVÉNYEID
        // (NEM VÁLTOZTATVA)
        // =========================

        static int szamolpont(double ido)
        {
            if (ido >= 10) return 0;
            return (int)Math.Floor(10 - ido);
        }

        static int legjobbpont(int pont1, int pont2, int pont3)
        {
            return Math.Max(pont1, Math.Max(pont2, pont3));
        }

        static double legjobbido(double ido1, double ido2, double ido3)
        {
            return Math.Min(ido1, Math.Min(ido2, ido3));
        }

        static int helyezes(Versenyzo helyezesversenyzo, List<Versenyzo> versenyzok)
        {
            int helyezese = 1;
            foreach (var v in versenyzok)
            {
                if (v.Id != helyezesversenyzo.Id)
                {
                    if (v.Legjobb_pont > helyezesversenyzo.Legjobb_pont)
                        helyezese++;
                    else if (v.Legjobb_pont == helyezesversenyzo.Legjobb_pont &&
                             v.Legjobb_ido < helyezesversenyzo.Legjobb_ido)
                        helyezese++;
                    else if (v.Legjobb_pont == helyezesversenyzo.Legjobb_pont &&
                             v.Legjobb_ido == helyezesversenyzo.Legjobb_ido &&
                             v.Id < helyezesversenyzo.Id)
                        helyezese++;
                }
            }
            return helyezese;
        }

        static int idje(List<Versenyzo> versenyzok)
        {
            if (versenyzok.Count == 0) return 1;
            return versenyzok.Count + 1;
        }

        static void pillahelymegváltoztatas(List<Versenyzo> versenyzok)
        {
            foreach (var v in versenyzok)
                v.Pilla_hely = helyezes(v, versenyzok);
        }

        static void lengetespontfix(List<Versenyzo> versenyzok)
        {
            foreach (var v in versenyzok)
            {
                v.Lengetes1 = szamolpont(v.Ido1);
                v.Lengetes2 = szamolpont(v.Ido2);
                v.Lengetes3 = szamolpont(v.Ido3);
                v.Legjobb_pont =
                    legjobbpont(v.Lengetes1, v.Lengetes2, v.Lengetes3);
                v.Legjobb_ido =
                    legjobbido(v.Ido1, v.Ido2, v.Ido3);
            }
        }
    }

    // =========================
    // VERSENYZŐ OSZTÁLY
    // =========================
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

        public Versenyzo(int id, string nev, int l1, double i1,
                         int l2, double i2, int l3, double i3,
                         int lp, double li, int ph)
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
