using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace KalaplengetoDB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ConnectionString = "SERVER=localhost;DATABASE=kalaplengeto;UID=root;PASSWORD=;";
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            List<Versenyzo> versenyzok = new List<Versenyzo>();

            #region Versenyzők listázása

            connection.Open();
            string sql = "SELECT * FROM versenyzok ORDER BY nev ASC";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Versenyzo v = new Versenyzo(
                    id: reader.GetInt32("id"),
                    nev: reader.GetString("nev"),
                    lengetes1: reader.GetInt32("lengetes1"),
                    ido1: reader.GetDouble("ido1"),
                    lengetes2: reader.GetInt32("lengetes2"),
                    ido2: reader.GetDouble("ido2"),
                    lengetes3: reader.GetInt32("lengetes3"),
                    ido3: reader.GetDouble("ido3"),
                    legjobb_pont: reader.GetInt32("legjobb_pont"),
                    legjobb_ido: reader.GetDouble("legjobb_ido"),
                    pilla_hely: reader.GetInt32("pilla_hely")
                    );
                versenyzok.Add(v);
            }
            connection.Close();
            #endregion

            Console.Clear();
            Console.WriteLine("*** Új felhasználó rögzítése ***");
            Console.Write("A versenyző neve: ");
            string nev = Console.ReadLine();

            Console.Write("Az első idő: ");
            double ido1 = double.Parse(Console.ReadLine());
            Console.Write("A második idő: ");
            double ido2 = double.Parse(Console.ReadLine());
            Console.Write("A harmadik idő: ");
            double ido3 = double.Parse(Console.ReadLine());

            var legjobbpontröv = legjobbpont(szamolpont(ido1), szamolpont(ido2), szamolpont(ido3));

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

            connection.Open();
            string sql1 = @"INSERT INTO versenyzok (id, nev, lengetes1, ido1, lengetes2, ido2, lengetes3, ido3, legjobb_pont, legjobb_ido, pilla_hely) VALUES (@id, @nev, @lengetes1, @ido1, @lengetes2, @ido2, @lengetes3, @ido3, @legjobb_pont, @legjobb_ido, @pilla_hely);";
            string visszaalit = "TRUNCATE TABLE versenyzok;";
            MySqlCommand vissza = new MySqlCommand(visszaalit, connection);
            vissza.ExecuteNonQuery();
            foreach (var v in versenyzok)
            {
                MySqlCommand feltöltes = new MySqlCommand(sql1, connection);
                feltöltes.Parameters.AddWithValue("@id", v.Id);
                feltöltes.Parameters.AddWithValue("@nev", v.Nev);
                feltöltes.Parameters.AddWithValue("@lengetes1", v.Lengetes1);
                feltöltes.Parameters.AddWithValue("@ido1", v.Ido1);
                feltöltes.Parameters.AddWithValue("@lengetes2", v.Lengetes2);
                feltöltes.Parameters.AddWithValue("@ido2", v.Ido2);
                feltöltes.Parameters.AddWithValue("@lengetes3", v.Lengetes3);
                feltöltes.Parameters.AddWithValue("@ido3", v.Ido3);
                feltöltes.Parameters.AddWithValue("@legjobb_pont", v.Legjobb_pont);
                feltöltes.Parameters.AddWithValue("@legjobb_ido", v.Legjobb_ido);
                feltöltes.Parameters.AddWithValue("@pilla_hely", v.Pilla_hely);
                feltöltes.ExecuteNonQuery();
            }
            connection.Close();

            foreach (var v in versenyzok)
            {
                Console.WriteLine(
                    $"ID: {v.Id}, Név: {v.Nev}, " +
                    $"Lengetés1: {v.Lengetes1} pont, {v.Ido1} mp, " +
                    $"Lengetés2: {v.Lengetes2} pont, {v.Ido2} mp, " +
                    $"Lengetés3: {v.Lengetes3} pont, {v.Ido3} mp, " +
                    $"Legjobb pont: {v.Legjobb_pont}, Legjobb idő: {v.Legjobb_ido}, " +
                    $"Pillanatnyi hely: {v.Pilla_hely}"
                );
            }
        }

        /// <summary>
        /// Számolja az egyéni lengetés pontját az idő alapján. 
        /// 10 mp felett 0 pont, különben 10 - idő floor értéke.
        /// </summary>
        static int szamolpont(double ido)
        {
            if (ido >= 10)
            {
                return 0;
            }
            return (int)Math.Floor(10 - ido);
        }

        /// <summary>
        /// Visszaadja a három pontszám közül a legnagyobbat.
        /// </summary>
        static int legjobbpont(int pont1, int pont2, int pont3)
        {
            return Math.Max(pont1, Math.Max(pont2, pont3));
        }

        /// <summary>
        /// Visszaadja a három idő közül a legkisebbet (legjobb időt).
        /// </summary>
        static double legjobbido(double ido1, double ido2, double ido3)
        {
            return Math.Min(ido1, Math.Min(ido2, ido3));
        }

        /// <summary>
        /// Meghatározza az új versenyző helyezését a pontok és idők alapján.
        /// </summary>
        static int helyezes(Versenyzo ujversenyzo, List<Versenyzo> versenyzok)
        {
            int helyezese = 1;
            foreach (var v in versenyzok)
            {
                if (v.Id != ujversenyzo.Id)
                {
                    if (v.Legjobb_pont > ujversenyzo.Legjobb_pont)
                    {
                        helyezese++;
                    }
                    else if (v.Legjobb_pont == ujversenyzo.Legjobb_pont && v.Legjobb_ido < ujversenyzo.Legjobb_ido)
                    {
                        helyezese++;
                    }
                    else if (v.Legjobb_pont == ujversenyzo.Legjobb_pont &&
                             v.Legjobb_ido == ujversenyzo.Legjobb_ido &&
                             v.Id < ujversenyzo.Id) // <- Ha a régi ID kisebb, akkor a régi a jobb
                    {
                        helyezese++;
                    }
                }
            }
            return helyezese;
        }

        /// <summary>
        /// Visszaadja az újonnan beszúrt versenyző következő Id-ját.
        /// </summary>
        static int idje(List<Versenyzo> versenyzok)
        {
            return versenyzok.Count + 1;
        }

        /// <summary>
        /// Frissíti az összes versenyző pillanatnyi helyét a teljes listában.
        /// </summary>
        static void pillahelymegváltoztatas(List<Versenyzo> versenyzok)
        {
            foreach (var v in versenyzok)
            {
                v.Pilla_hely = helyezes(v, versenyzok);
            }
        }

        /// <summary>
        /// Frissíti az összes versenyző lengetés pontjait és legjobb pont/idő értékeit.
        /// </summary>
        static void lengetespontfix(List<Versenyzo> versenyzok)
        {
            foreach (var v in versenyzok)
            {
                v.Lengetes1 = szamolpont(v.Ido1);
                v.Lengetes2 = szamolpont(v.Ido2);
                v.Lengetes3 = szamolpont(v.Ido3);
                v.Legjobb_pont = legjobbpont(szamolpont(v.Ido1), szamolpont(v.Ido2), szamolpont(v.Ido3));
                v.Legjobb_ido = legjobbido(v.Ido1, v.Ido2, v.Ido3);
            }
        }
    }

    public class Versenyzo
    {
        private int id;
        private string nev;
        private int lengetes1;
        private double ido1;
        private int lengetes2;
        private double ido2;
        private int lengetes3;
        private double ido3;
        private int legjobb_pont;
        private double legjobb_ido;
        private int pilla_hely;

        public int Id { get => id; set => id = value; }
        public string Nev { get => nev; set => nev = value; }
        public int Lengetes1 { get => lengetes1; set => lengetes1 = value; }
        public double Ido1 { get => ido1; set => ido1 = value; }
        public int Lengetes2 { get => lengetes2; set => lengetes2 = value; }
        public double Ido2 { get => ido2; set => ido2 = value; }
        public int Lengetes3 { get => lengetes3; set => lengetes3 = value; }
        public double Ido3 { get => ido3; set => ido3 = value; }
        public int Legjobb_pont { get => legjobb_pont; set => legjobb_pont = value; }
        public double Legjobb_ido { get => legjobb_ido; set => legjobb_ido = value; }
        public int Pilla_hely { get => pilla_hely; set => pilla_hely = value; }

        public Versenyzo() { }

        public Versenyzo(int id, string nev, int lengetes1, double ido1, int lengetes2, double ido2, int lengetes3, double ido3, int legjobb_pont, double legjobb_ido, int pilla_hely)
        {
            this.id = id;
            this.nev = nev;
            this.lengetes1 = lengetes1;
            this.ido1 = ido1;
            this.lengetes2 = lengetes2;
            this.ido2 = ido2;
            this.lengetes3 = lengetes3;
            this.ido3 = ido3;
            this.legjobb_pont = legjobb_pont;
            this.legjobb_ido = legjobb_ido;
            this.pilla_hely = pilla_hely;
        }
    }
}
