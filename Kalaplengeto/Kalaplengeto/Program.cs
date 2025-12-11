using MySql.Data.MySqlClient;

using System;

using System.Collections.Generic;

using System.Data.SqlClient;



namespace KalaplengetoDB

{

    internal class Program

    {

        static void Main(string[] args)

        {

            MySqlConnection connection = new MySqlConnection();

            string connectionString = "SERVER=localhost;DATABASE=kalaplengeto;UID=root;PASSWORD=;";

            connection.ConnectionString = connectionString;



            #region Versenyzők listázása

            connection.Open();

            string sql = "SELECT * FROM competitor ORDER BY name ASC";

            MySqlCommand cmd = new MySqlCommand(sql, connection);



            List<Versenyzo> versenyzok = new List<Versenyzo>();

            MySqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())

            {

                Versenyzo v = new Versenyzo(

                    id: reader.GetInt32("id"),

                    name: reader.GetString("name")

                );

                versenyzok.Add(v);

            }

            connection.Close();



            Console.WriteLine("*** Versenyzők listája ***");

            foreach (var v in versenyzok)

            {

                Console.WriteLine($"{v.Id} - {v.Name}");

            }

            #endregion



            #region Új versenyző felvétele

            Console.WriteLine("\n*** Új versenyző rögzítése ***");

            Console.Write("Név: ");

            string nev = Console.ReadLine();



            connection.Open();

            string insertSQL = "INSERT INTO competitor (name) VALUES (@nev)";

            MySqlCommand insertCmd = new MySqlCommand(insertSQL, connection);



            insertCmd.Parameters.AddWithValue("@nev", nev);



            int sorok = insertCmd.ExecuteNonQuery();

            connection.Close();



            Console.WriteLine(sorok > 0 ? "Sikeres rögzítés!" : "Sikertelen rögzítés!");

            #endregion

        }

    }
    public class Versenyzo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Versenyzo(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
