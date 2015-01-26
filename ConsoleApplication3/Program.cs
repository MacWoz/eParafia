using System;
using Npgsql;
    
namespace eParafia
{
    class MainClass
    {
        static NpgsqlConnection connection = null;
        static bool logged = false;
        public static void LogIn()
        {
            Console.WriteLine("Podaj login:");
            string username = Console.ReadLine();
            Console.WriteLine("Podaj hasło:");
            string password = Console.ReadLine();
            string connstring = "Server=127.0.0.1;Port=5432;Database=" + username + ";User Id=" + username +";";
            connstring += "Password=" + password + ";";
            connection = new NpgsqlConnection(connstring);
            try
            {
                connection.Open();
            }
            catch(NpgsqlException exc)
            {
                Console.WriteLine("BŁĄD: " + exc.BaseMessage);
                return;
            }
            logged = true;
        }
        public static void ShowManual()
        {
            Console.WriteLine("Instrukcja");
            Console.WriteLine("Aby dodać odpowiednie informacje wpisz:");
            Console.WriteLine("\t" + "\"chrzest\"" + " - Aby dodać informacje o nowym członku parafii i udzielonym chrzcie.");
            Console.WriteLine("\t" + "\"pierwsza komunia\"" + " - Aby dodać informacje o udzielonej pierwszej Komunii.");
            Console.WriteLine("\t" + "\"bierzmowanie\"" + " - Aby dodać informacje o udzielonym bierzmowaniu.");
            Console.WriteLine("\t" + "\"slub\"" + " - Aby dodać informacje o udzielonym ślubie.");
            Console.WriteLine("\t" + "\"pogrzeb\"" + " - Aby dodać informacje o pogrzebie.");
            Console.WriteLine("Następnie postępuj według dalszych instrukcji.");
            Console.WriteLine();

            Console.WriteLine("Aby usunąć odpowiednie informacje, wpisz:");
            Console.WriteLine("\t" + "\"usun osobe\"" + " - Aby usunąć informacje o parafianinie i wszystkie informacje z nim związane.");
            Console.WriteLine("\t" + "\"usun komunie\"" + " - Aby usunąć informacje o udzielonej pierwszej Komunii.");
            Console.WriteLine("\t" + "\"usun bierzmowanie\"" + " - Aby usunąć informacje o udzielonym bierzmowaniu.");
            Console.WriteLine("\t" + "\"usun slub\"" + " - Aby usunąć informacje o udzielonym ślubie.");
            Console.WriteLine("\t" + "\"usun pogrzeb\"" + " - Aby usunąć informacje o pogrzebie.");
            Console.WriteLine("Aby unieważnić ślub, wpisz \"uniewaznij slub\".");
            Console.WriteLine("Następnie postępuj według dalszych instrukcji.");
            Console.WriteLine();

            Console.WriteLine("Aby wyświetlić odpowiednie informacje wpisz \"wyswietl\".\n");
            Console.WriteLine("Następnie postępuj według dalszych instrukcji.");

            Console.WriteLine("Aby poprawić błędne dane wpisz \"popraw\".\n");
            Console.WriteLine("Następnie postępuj według dalszych instrukcji.");
            Console.WriteLine();

            Console.WriteLine("Inne polecenia nie będą rozpoznane przez aplikację.");
        }
        public static void ExecuteCommand(string command)
        {
            switch (command)
            {
                case "manual":
                    ShowManual();
                    break;
                case "chrzest":
                    AddParafianie();
                    break;
                case "pierwsza komunia":
                    AddKomunie();
                    break;
                case "bierzmowanie":
                    AddBierzmowania();
                    break;
                case "slub":
                    AddSluby();
                    break;
                case "pogrzeb":
                    AddPogrzeby();
                    break;
                case "usun osobe":
                    DeleteParafianie();
                    break;
                case "usun komunie":
                    DeleteKomunie();
                    break;
                case "usun bierzmowanie":
                    DeleteBierzmowania();
                    break;
                case "usun slub":
                    DeleteSluby();
                    break;
                case "usun pogrzeb":
                    DeletePogrzeby();
                    break;
                case "uniewaznij slub":
                    UniewaznijSlub();
                    break;
                case "wyswietl":
                    ShowInfo();
                    break;
                case "popraw":
                    Correct();
                    break;
                default:
                    Console.WriteLine("Nie odnaleziono polecenia.");
                    break;
            }
        }

        public static void AddParafianie()
        {
            Console.WriteLine("Podaj imię:");
            string imie = "'" + Console.ReadLine() + "'";
            imie = "'" + imie + "'";
            Console.WriteLine("Podaj nazwisko:");
            string nazwisko = "'" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj PESEL:");
            string pesel = "'" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj datę urodzenia (w formacie YYYY-MM-DD):");
            string data_urodzenia = "date '" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj płeć (dozwolone K lub M):");
            string plec = "'" + Console.ReadLine()[0] + "'";
            Console.WriteLine("Podaj PESEL ojca (wpisz \"nieznany\" jeśli ojciec nie należy do parafii):");
            string ojciec_pesel = Console.ReadLine();
            if (ojciec_pesel == "nieznany")
                ojciec_pesel = "null";
            else 
                ojciec_pesel = "'" + ojciec_pesel + "'";
            Console.WriteLine("Podaj PESEL matki (wpisz \"nieznany\" jeśli matka nie należy do parafii):");
            string matka_pesel = Console.ReadLine();
            if (matka_pesel == "nieznany")
                matka_pesel = "null";
            else
                matka_pesel = "'" + matka_pesel + "'";
            Console.WriteLine("Podaj datę chrztu (w formacie YYYY-MM-DD):");
            string data = "date '" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj imię (imiona) nadane na chrzcie:");
            string imiona = "'" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj imię i nazwisko ojca chrzestnego:");
            string dane_ojca = "'" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj imię i nazwisko matki chrzestnej:");
            string dane_matki = "'" + Console.ReadLine() + "'";
            string query = "SELECT insert_chrzty(" + pesel + ", " + nazwisko +  ", " + imie + ", " + data_urodzenia + ", " + plec + ", ";
            query += ojciec_pesel + ", " + matka_pesel + ", " + data + ", " + imiona + ", " + dane_ojca + ", " + dane_matki + ")";
            System.Console.WriteLine(query);
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                while (dr.Read()) ;
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null) 
                    dr.Close();
            }
        }
        public static void AddKomunie()
        {
            Console.WriteLine("Podaj PESEL osoby przyjmującej I Komunię:");
            string pesel = "'" +  Console.ReadLine() + "'";
            Console.WriteLine("Podaj datę I Komunii (w formacie YYYY-MM-DD):");
            string data = "date '" + Console.ReadLine() + "'";
            string query = "SELECT insert_komunie (" + data + ", " + pesel + ")";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query " + query);
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                while (dr.Read()) ;
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
        public static void AddBierzmowania()
        {
            Console.WriteLine("Podaj PESEL osoby bierzmowanej:");
            string pesel = "'" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj datę bierzmowania (w formacie YYYY-MM-DD):");
            string data = "date '" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj imię nadane przy bierzmowaniu:");
            string imie = "'" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj imię i nazwisko świadka bierzmowania:");
            string swiadek = "'" + Console.ReadLine() + "'";
            string query = "SELECT insert_bierzmowania(" + data + ", " + pesel + ", " + imie + ", " + swiadek + ")";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query " + query);
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                while (dr.Read()) ;
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
        public static void AddSluby()
        {
            Console.WriteLine("Czy mężczyzna jest z parafii? (T/N)");
            string resp = Console.ReadLine();
            string pesel_mezczyzny = null;
            string dane_mezczyzny = null;
            string pesel_kobiety = null;
            string dane_kobiety = null;
            if (resp == "T")
            {
                Console.WriteLine("Podaj PESEL mężczyzny:");
                pesel_mezczyzny = "'" + Console.ReadLine() + "'";
                dane_mezczyzny = "null";
            }
            else if (resp == "N")
            {
                Console.WriteLine("Podaj dane mężczyzny:");
                pesel_mezczyzny = "null";
                dane_mezczyzny = "'" + Console.ReadLine() + "'";
            }
            Console.WriteLine("Czy kobieta jest z parafii? (T/N)");
            resp = Console.ReadLine();
            if (resp == "T")
            {
                Console.WriteLine("Podaj PESEL kobiety:");
                pesel_kobiety = "'" + Console.ReadLine() + "'";
                dane_kobiety = "null";
            }
            else if (resp == "N")
            {
                Console.WriteLine("Podaj dane kobiety:");
                pesel_kobiety = "null";
                dane_kobiety = "'" + Console.ReadLine() + "'";
            }
            Console.WriteLine("Podaj datę ślubu (w formacie YYYY-MM-DD)");
            string data = "date '" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj imię i nazwisko pierwszego świadka:");
            string swiadek1 = "'" + Console.ReadLine() + "'";
            Console.WriteLine("Podaj imię i nazwisko drugiego świadka:");
            string swiadek2 = "'" + Console.ReadLine() +"'";
            string query = "SELECT insert_sluby(" + data + ", " + pesel_mezczyzny + ", " + dane_mezczyzny + ", " + pesel_kobiety + ", ";
            query += dane_kobiety + ", " + swiadek1 + ", " + swiadek2 + ")";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query " + query);
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                while (dr.Read()) ;
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
        public static void AddPogrzeby()
        {
            Console.WriteLine("Podaj PESEL osoby zmarłej:");
            string pesel = Console.ReadLine();
            Console.WriteLine("Podaj datę śmierci (w formacie YYYY-MM-DD) albo wpisz \"nieznana\":");
            string data_smierci = Console.ReadLine();
            if (data_smierci == "nieznana")
                data_smierci = "null";
            else
                data_smierci = "date '" + data_smierci + "'";
            Console.WriteLine("Podaj datę pogrzebu:");
            string data_pogrzebu = "date '" + Console.ReadLine() + "'";
            
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO pogrzeby values ('" + pesel + "', " + data_smierci  + ", " + data_pogrzebu + ")", connection);
            Console.WriteLine("query " + "INSERT INTO pogrzeby values ('" + pesel + "', " + data_smierci + ", " + data_pogrzebu + ")");
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                while (dr.Read()) ;
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        public static void DeleteParafianie()
        {
            Console.WriteLine("Podaj PESEL osoby do usunięcia:");
            string pesel = "'" + Console.ReadLine() + "'";
            string query = "DELETE FROM parafianie WHERE pesel=" + pesel;
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query " + query);
            try
            {
                int count = command.ExecuteNonQuery();
                if (count == 0)
                {
                    Console.WriteLine("Niepoprawny PESEL, nic nie usunięto.");
                }

            }
            catch (NpgsqlException exc)
            {
                System.Console.WriteLine(exc.BaseMessage);
            }
        }
        public static void DeleteKomunie()
        {
            Console.WriteLine("Podaj PESEL osoby przyjmującej I Komunię:");
            string pesel = "'" + Console.ReadLine() + "'";
            string query = "SELECT id_sakramentu FROM pierwsze_komunie WHERE osoba = " + pesel + " LIMIT 1";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query " + query);
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                dr.Read();
                int id = (int)dr[0];
                query = "DELETE FROM pierwsze_komunie WHERE id_sakramentu = " + id;
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
                return;
            }
            finally
            {
                if (dr != null) 
                    dr.Close();
                dr = null;
            }
            command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query2 " + query);
            try
            {
                dr = command.ExecuteReader();
                dr.Read();
                for (int i = 0; i < dr.FieldCount; ++i)
                    Console.WriteLine("{0} {1}\n", i, dr[i]);
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
                return;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
        public static void DeleteBierzmowania()
        {
            Console.WriteLine("Podaj PESEL osoby przyjmującej bierzmowanie:");
            string pesel = "'" + Console.ReadLine() + "'";
            string query = "SELECT id_sakramentu FROM bierzmowania WHERE osoba = " + pesel + " LIMIT 1";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query " + query);
            NpgsqlDataReader dr = null;
            int id = 0;
            try
            {
                dr = command.ExecuteReader();
                dr.Read();
                id = (int)dr[0];
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
                return;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                dr = null;
            }
            query = "DELETE FROM bierzmowania WHERE id_sakramentu = " + id;
            command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query2 " + query);
            try
            {
                dr = command.ExecuteReader();
                dr.Read();
                for (int i = 0; i < dr.FieldCount; ++i)
                    Console.WriteLine("{0} -> {1}\n", i, dr[i]);
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
        public static void DeleteSluby()
        {
            Console.WriteLine("Czy mężczyzna jest z parafii? (T/N)");
            string resp = Console.ReadLine();
            string pesel_mezczyzny = null;
            string dane_mezczyzny = null;
            string pesel_kobiety = null;
            string dane_kobiety = null;
            if (resp == "T")
            {
                Console.WriteLine("Podaj PESEL mężczyzny:");
                pesel_mezczyzny = " = '" + Console.ReadLine() + "'";
                dane_mezczyzny = " is NULL";
            }
            else if (resp == "N")
            {
                Console.WriteLine("Podaj dane mężczyzny:");
                pesel_mezczyzny = " is NULL";
                dane_mezczyzny = " = '" + Console.ReadLine() + "'";
            }
            Console.WriteLine("Czy kobieta jest z parafii? (T/N)");
            resp = Console.ReadLine();
            if (resp == "T")
            {
                Console.WriteLine("Podaj PESEL kobiety:");
                pesel_kobiety = " = '" + Console.ReadLine() + "'";
                dane_kobiety = " is NULL";
            }
            else if (resp == "N")
            {
                Console.WriteLine("Podaj dane kobiety:");
                pesel_kobiety = " is NULL";
                dane_kobiety = " = '" + Console.ReadLine() + "'";
            }
            Console.WriteLine("Podaj datę ślubu (w formacie YYYY-MM-DD)");
            string data = "date '" + Console.ReadLine() + "'";

            string join = "SELECT sa.id FROM sluby sl JOIN sakramenty sa ON sl.id_sakramentu = sa.id WHERE sa.data_udzielenia = "+ data + " AND ";
            join += "sl.maz" + pesel_mezczyzny + " AND sl.dane_meza" + dane_mezczyzny + " AND sl.zona" + pesel_kobiety;
            join += " AND sl.dane_zony" + dane_kobiety; 
            Console.WriteLine("joinQuery " + join);
            NpgsqlCommand command = new NpgsqlCommand(join, connection);
            NpgsqlDataReader dr = null;
            int id = 0;
            try
            {
                dr = command.ExecuteReader();
                bool read = dr.Read();
                if (!read)
                {
                    System.Console.WriteLine("Błąd, nie ma ślubu o podanych parametrach!");
                    return;
                }
                for (int i = 0; i < dr.FieldCount; ++i)
                    System.Console.WriteLine(i + " -> " + dr[i]);
                id = (int)dr[0];
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
                return;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                dr = null;
            }
            string query = "DELETE FROM sluby WHERE id_sakramentu = " + id;
            Console.WriteLine("query " + query);
            command = new NpgsqlCommand(query, connection);
            try
            {
                dr = command.ExecuteReader();
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
        public static void DeletePogrzeby()
        {
            Console.WriteLine("Podaj PESEL osoby zmarłej:");
            string pesel = "'" + Console.ReadLine() + "'";
            string query = "DELETE FROM pogrzeby WHERE osoba = " + pesel;
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("query " + query);
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                dr.Read();
                for (int i = 0; i < dr.FieldCount; ++i)
                    Console.WriteLine("{0} {1}\n", i, dr[i]);
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        public static void UniewaznijSlub()
        {
            Console.WriteLine("Czy mężczyzna jest z parafii? (T/N)");
            string resp = Console.ReadLine();
            string pesel_mezczyzny = null;
            string dane_mezczyzny = null;
            string pesel_kobiety = null;
            string dane_kobiety = null;
            if (resp == "T")
            {
                Console.WriteLine("Podaj PESEL mężczyzny:");
                pesel_mezczyzny = " = '" + Console.ReadLine() + "'";
                dane_mezczyzny = " is NULL";
            }
            else if (resp == "N")
            {
                Console.WriteLine("Podaj dane mężczyzny:");
                pesel_mezczyzny = " is NULL";
                dane_mezczyzny = " = '" + Console.ReadLine() + "'";
            }
            Console.WriteLine("Czy kobieta jest z parafii? (T/N)");
            resp = Console.ReadLine();
            if (resp == "T")
            {
                Console.WriteLine("Podaj PESEL kobiety:");
                pesel_kobiety = " = '" + Console.ReadLine() + "'";
                dane_kobiety = " is NULL";
            }
            else if (resp == "N")
            {
                Console.WriteLine("Podaj dane kobiety:");
                pesel_kobiety = " is NULL";
                dane_kobiety = " = '" + Console.ReadLine() + "'";
            }
            Console.WriteLine("Podaj datę ślubu (w formacie YYYY-MM-DD)");
            string data = "date '" + Console.ReadLine() + "'";

            string join = "SELECT sa.id FROM sluby sl JOIN sakramenty sa ON sl.id_sakramentu = sa.id WHERE sa.data_udzielenia = " + data + " AND ";
            join += "sl.maz" + pesel_mezczyzny + " AND sl.dane_meza" + dane_mezczyzny + " AND sl.zona" + pesel_kobiety;
            join += " AND sl.dane_zony" + dane_kobiety;
            Console.WriteLine("joinQuery " + join);
            NpgsqlCommand command = new NpgsqlCommand(join, connection);
            int id = 0;
            NpgsqlDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                bool read = dr.Read();
                if (!read)
                {
                    System.Console.WriteLine("Błąd, nie ma ślubu o podanych parametrach!");
                    return;
                }
                for (int i = 0; i < dr.FieldCount; ++i)
                    System.Console.WriteLine(i + " -> " + dr[i]);
                id = (int)dr[0];
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                dr = null;
            }
            string query = "UPDATE sluby SET uniewazniony = TRUE where id_sakramentu = " + id;
            command = new NpgsqlCommand(query, connection);
            try
            {
                dr = command.ExecuteReader();
                while (dr.Read()) ;
            }
            catch (NpgsqlException exc)
            {
                Console.WriteLine(exc.BaseMessage);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        public static void ShowInfo()
        {
            Console.Write("Czy chcesz wyświetlić dla każdego parafianina wszystkie informacje o przyjętych przez niego sakramentach");
            Console.Write(" i o ewentualnym pogrzebie ?\n");
            Console.Write("Albo czy chcesz wyświetlić informacje o wszystkich udzielonych w parafii sakramentach ");
            Console.Write("i pogrzebach, bądź tylko na temat jednego z sakramentów (tylko pogrzebów)?\n");
            Console.WriteLine("a może interesują Cię statystyki na temat udzielonych sakramentów?");
            Console.WriteLine("Wybierz opcję: wpisz 0, 1, 2 lub 3.");
            string answer = Console.ReadLine();
            if (answer == "0")
            {
                answer = "parafianie";
                GetInfo(0, 0, answer);
                return;
            }
            else if (answer == "1")
                answer = "all";
            else if (answer == "2")
            {
                Console.WriteLine("Jakie informacje chcesz uzyskać ?");
                Console.WriteLine("Wpisz odpowiednio : \"chrzty\", \"pierwsze komunie\", \"bierzmowania\", \"sluby\" lub \"pogrzeby\".");
                answer = Console.ReadLine();
            }

            else if (answer == "3")
            {
                Console.WriteLine("Czy chcesz uzyskać statystyki na temat tylko jednego sakramentu/tylko pogrzebów, czy na temat wszystkich sakramentów i pogrzebów?");
                Console.WriteLine("Wpisz odpowiednio 0 albo 1.");
                string caseString = Console.ReadLine();
                if (caseString == "0")
                {
                    Console.WriteLine("Jakie informacje chcesz uzyskać ?");
                    Console.WriteLine("Wpisz odpowiednio : \"chrzty\", \"pierwsze komunie\", \"bierzmowania\", \"sluby\" lub \"pogrzeby\".");
                    caseString = Console.ReadLine();
                    if (caseString == "chrzty")
                    {
                        Console.WriteLine("Jeśli chcesz wyswietlić informacje od pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane od początku, wpisz 0.");
                        int startyear = int.Parse(Console.ReadLine());

                        Console.WriteLine("Jeśli chcesz wyswietlić informacje do pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane do chwili obecnej, wpisz 0.");
                        int endyear = int.Parse(Console.ReadLine());
                        Count(startyear, endyear, caseString);
                        return;
                    }

                    else if (caseString == "pierwsze komunie")
                    {
                        Console.WriteLine("Jeśli chcesz wyswietlić informacje od pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane od początku, wpisz 0.");
                        int startyear = int.Parse(Console.ReadLine());

                        Console.WriteLine("Jeśli chcesz wyswietlić informacje do pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane do chwili obecnej, wpisz 0.");
                        int endyear = int.Parse(Console.ReadLine());
                        Count(startyear, endyear, caseString);
                        return;
                    }

                    else if (caseString == "bierzmowania")
                    {
                        Console.WriteLine("Jeśli chcesz wyswietlić informacje od pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane od początku, wpisz 0.");
                        int startyear = int.Parse(Console.ReadLine());

                        Console.WriteLine("Jeśli chcesz wyswietlić informacje do pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane do chwili obecnej, wpisz 0.");
                        int endyear = int.Parse(Console.ReadLine());
                        Count(startyear, endyear, caseString);
                        return;
                    }

                    else if (caseString == "sluby")
                    {
                        Console.WriteLine("Jeśli chcesz wyswietlić informacje od pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane od początku, wpisz 0.");
                        int startyear = int.Parse(Console.ReadLine());

                        Console.WriteLine("Jeśli chcesz wyswietlić informacje do pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane do chwili obecnej, wpisz 0.");
                        int endyear = int.Parse(Console.ReadLine());
                        Count(startyear, endyear, caseString);
                        return;
                    }

                    else if (caseString == "pogrzeby")
                    {
                        Console.WriteLine("Jeśli chcesz wyswietlić informacje od pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane od początku, wpisz 0.");
                        int startyear = int.Parse(Console.ReadLine());

                        Console.WriteLine("Jeśli chcesz wyswietlić informacje do pewnego roku, wpisz go teraz.");
                        Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane do chwili obecnej, wpisz 0.");
                        int endyear = int.Parse(Console.ReadLine());
                        Count(startyear, endyear, caseString);
                        return;
                    }
                }

                else if (caseString == "1")
                {
                    Console.WriteLine("Jeśli chcesz wyswietlić informacje od pewnego roku, wpisz go teraz.");
                    Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane od początku, wpisz 0.");
                    int startyear = int.Parse(Console.ReadLine());

                    Console.WriteLine("Jeśli chcesz wyswietlić informacje do pewnego roku, wpisz go teraz.");
                    Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane do chwili obecnej, wpisz 0.");
                    int endyear = int.Parse(Console.ReadLine());
                    Count(startyear, endyear, "all");
                }
                return;
            }

            Console.WriteLine("Jeśli chcesz wyswietlić informacje od pewnego roku, wpisz go teraz.");
            Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane od początku, wpisz 0.");
            int start = int.Parse(Console.ReadLine());

            Console.WriteLine("Jeśli chcesz wyswietlić informacje do pewnego roku, wpisz go teraz.");
            Console.WriteLine("Jeśli chcesz wyświetlić informacje zebrane do chwili obecnej, wpisz 0.");
            int end = int.Parse(Console.ReadLine());

            GetInfo(start, end, answer);
        }

        public static void GetInfo(int start, int end, string type)
        {
            if (type == "parafianie")
            {
                string query = "SELECT * FROM all_parafianie ORDER BY chrzest";
                NpgsqlDataReader dr = null;
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        Console.WriteLine(dr[0] + " (PESEL: " + dr[1] + ")");
                        Console.WriteLine("\t" + dr[2]);
                        Console.WriteLine("\t" + dr[3]);
                        Console.WriteLine("\tChrzest dnia: " + dr[4].ToString().Substring(0, 10));
                        if (!dr[5].GetType().Equals(typeof(System.DBNull)))
                            Console.WriteLine("\tPierwsza Komunia dnia: " + dr[5].ToString().Substring(0, 10));
                        if (!dr[6].GetType().Equals(typeof(System.DBNull)))
                            Console.WriteLine("\tBierzmowanie dnia: " + dr[6].ToString().Substring(0, 10));
                        if (!dr[7].GetType().Equals(typeof(System.DBNull)))
                        {
                            Console.Write("\tŚlub dnia: " + dr[7].ToString().Substring(0, 10));
                            Console.WriteLine(", Małżonek: " + dr[8].ToString());
                        }
                        if (!dr[10].GetType().Equals(typeof(System.DBNull)))
                        {
                            if (dr[9].GetType().Equals(typeof(System.DBNull)))
                                Console.Write("\tData śmierci nieznana");
                            else
                                Console.Write("\tData śmierci: " + dr[9].ToString().Substring(0, 10));
                            Console.WriteLine(", Data pogrzebu: " + dr[10].ToString().Substring(0, 10));
                        }
                    }
                }
                finally
                {
                    if(dr != null)
                        dr.Close();
                }
            }
            else if (type == "all")
            {
                Console.WriteLine("Chrzty:");
                GetInfo(start, end, "chrzty");
                Console.WriteLine("Pierwsze Komunie:");
                GetInfo(start, end, "pierwsze komunie");
                Console.WriteLine("Bierzmowania:");
                GetInfo(start, end, "bierzmowania");
                Console.WriteLine("Śluby:");
                GetInfo(start, end, "sluby");
                Console.WriteLine("Pogrzeby:");
                GetInfo(start, end, "pogrzeby");
            }

            else if (type == "chrzty")
            {
                string query = "SELECT p.imie||' '||p.nazwisko||' (pesel: '||p.pesel||')', 'ochrzczony dnia '||s.data_udzielenia,";
                query += "'ojciec chrzestny: '||c.ojciec_chrzestny, 'matka chrzestna: '||c.matka_chrzestna";
                query += " FROM ((parafianie p JOIN chrzty c ON p.pesel = c.osoba) JOIN sakramenty s ON s.id = c.id_sakramentu)";
                query += " WHERE s.powiazany = TRUE";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; ++i)
                            Console.Write(dr[i] + " ");
                        Console.Write("\n");
                    }
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
            else if (type == "pierwsze komunie")
            {
                string query = "SELECT p.imie||' '||p.nazwisko||' (pesel: '||p.pesel||')', 'przyjął I Komunię dnia '||s.data_udzielenia";
                query += " FROM parafianie p JOIN pierwsze_komunie pk JOIN sakramenty s ON s.id = pk.id_sakramentu";
                query += " ON pk.osoba = p.pesel WHERE s.powiazany = TRUE";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; ++i)
                            Console.Write(dr[i] + " ");
                        Console.Write("\n");
                    }
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
            else if (type == "bierzmowania")
            {
                string query = "SELECT p.imie||' '||p.nazwisko||' (pesel: '||p.pesel||')', 'przyjął/przyjęła bierzmowanie";
                query += " dnia '||s.data_udzielenia, 'swiadek: '||b.swiadek";
                query += " FROM (parafianie p JOIN bierzmowania b ON p.pesel = b.osoba) JOIN sakramenty s ON s.id = b.id_sakramentu";
                query += " WHERE s.powiazany = TRUE";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; ++i)
                            Console.Write(dr[i] + " ");
                        Console.Write("\n");
                    }
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
            else if (type == "sluby")
            {
                string query = "SELECT sl.maz, sl.dane_meza, (SELECT CASE WHEN sl.maz IS NOT NULL THEN imie||' '||nazwisko END FROM parafianie WHERE pesel = sl.maz), sl.zona, sl.dane_zony, ";
                query += " (SELECT CASE WHEN sl.zona IS NOT NULL THEN imie||' '||nazwisko END FROM parafianie WHERE pesel = sl.zona), sl.swiadek1, sl.swiadek2, sl.uniewazniony, s.data_udzielenia";
                query += " FROM sluby sl JOIN sakramenty s ON sl.id_sakramentu = s.id";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        string maz_dane = null;
                        string zona_dane = null;
                        if (dr[2].GetType().Equals(typeof(System.DBNull)))
                            maz_dane = (string)dr[1] + " (osoba spoza parafii) ";
                        else
                            maz_dane = dr[2] + " (PESEL: " + dr[0] + ") ";
                        if (dr[5].GetType().Equals(typeof(System.DBNull)))
                            zona_dane = (string)dr[4] + " (osoba spoza parafii) ";
                        else
                            zona_dane = dr[5] + " (PESEL: " + dr[3] + ") ";
                        string res = maz_dane + " i " + zona_dane + "zawarli ślub dnia " + dr[9].ToString().Substring(0, 10) + ", świadkowie: " + dr[6] + ", " + dr[7];
                        if ((bool)dr[8] == true)
                            res += " (unieważniony)";
                        Console.WriteLine(res);
                    }
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
            else if (type == "pogrzeby")
            {
                string query = "SELECT p.imie||' '||p.nazwisko||' (pesel: '||p.pesel||')', 'zmarł dnia: '||pog.data_smierci, ";
                query += "'pogrzeb odbyl sie dnia '||pog.data_pogrzebu, pog.data_smierci IS NULL";
                query += " FROM parafianie p JOIN pogrzeby pog ON p.pesel = pog.osoba";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', pog.data_smierci) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', pog.data_smierci) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount - 1; ++i)
                            Console.Write(dr[i] + " ");
                        if ((bool)dr[dr.FieldCount - 1] == true)
                            Console.Write("(data śmierci nieznana)");
                        Console.Write("\n");
                    }
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
        }

        public static long Count(int start, int end, string type)
        {
            if (type == "all")
            {
                long count = Count(start, end, "chrzty") + Count(start, end, "pierwsze komunie") + Count(start, end, "bierzmowania") + Count(start, end, "sluby");
                long pog = Count(start, end, "pogrzeby");
                if ((start == end) && (start != 0))
                    Console.WriteLine("W roku " + start + " udzielono razem " + count + " sakramentów i " + pog + " pogrzebów.");
                else if ((start == end) && (start == 0))
                    Console.WriteLine("W parafii udzielono razem " + count + " sakramentów i " + pog + " pogrzebów.");
                else
                    Console.WriteLine("Od roku " + start + " do roku " + end + " udzielono razem " + count + " sakramentów i " + pog + " pogrzebów.");
                return 0;
            }

            else if (type == "chrzty")
            {
                string query = "SELECT count(*) FROM sakramenty s JOIN chrzty c ON s.id = c.id_sakramentu WHERE s.powiazany = TRUE";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                long count = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    count = (long)dr[0];
                    if ((start == end) && (start != 0))
                        Console.WriteLine("W roku " + start + " udzielono " + count + " chrztów.");
                    else if ((start == end) && (start == 0))
                        Console.WriteLine("W parafii udzielono razem " + count + " chrztów.");
                    else
                        Console.WriteLine("Od roku " + start + " do roku " + end + " udzielono " + count + " chrztów.");
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                return count;
            }

            else if (type == "pierwsze komunie")
            {
                string query = "SELECT count(*) FROM sakramenty s JOIN pierwsze_komunie pk ON s.id = pk.id_sakramentu WHERE s.powiazany = TRUE";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                long count = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    count = (long)dr[0];
                    if ((start == end) && (start != 0))
                        Console.WriteLine("W roku " + start + " udzielono " + count + " Pierwszych Komunii.");
                    else if ((start == end) && (start == 0))
                        Console.WriteLine("W parafii udzielono razem " + count + " Pierwszych Komunii.");
                    else
                        Console.WriteLine("Od roku " + start + " do roku " + end + " udzielono " + count + " Pierwszych Komunii.");
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                return count;
            }

            else if (type == "bierzmowania")
            {
                string query = "SELECT count(*) FROM sakramenty s JOIN bierzmowania b ON s.id = b.id_sakramentu WHERE s.powiazany = TRUE";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                long count = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    count = (long)dr[0];
                    if ((start == end) && (start != 0))
                        Console.WriteLine("W roku " + start + " udzielono " + count + " bierzmowań.");
                    else if ((start == end) && (start == 0))
                        Console.WriteLine("W parafii udzielono razem " + count + " bierzmowań.");
                    else
                        Console.WriteLine("Od roku " + start + " do roku " + end + " udzielono " + count + " bierzmowań.");
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                return count;
            }

            else if (type == "sluby")
            {
                string query = "SELECT count(*) FROM sakramenty s JOIN sluby sl ON s.id = sl.id_sakramentu WHERE s.powiazany = TRUE";
                if (start != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) >= " + start + ")";
                }
                if (end != 0)
                {
                    query += " AND (SELECT date_part('year', s.data_udzielenia) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                long count = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    count = (long)dr[0];
                    if ((start == end) && (start != 0))
                        Console.WriteLine("W roku " + start + " udzielono " + count + " ślubów.");
                    else if ((start == end) && (start == 0))
                        Console.WriteLine("W parafii udzielono razem " + count + " ślubów.");
                    else
                        Console.WriteLine("Od roku " + start + " do roku " + end + " udzielono " + count + " ślubów.");
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                return count;
            }

            else if (type == "pogrzeby")
            {
                string query = "SELECT count(*) FROM pogrzeby p";
                if (start != 0)
                {
                    query += " WHERE (SELECT date_part('year', p.data_pogrzebu) >= " + start + ")";
                }
                if (end != 0 && start != 0)
                {
                    query += " AND (SELECT date_part('year', p.data_pogrzebu) <= " + end + ")";
                }
                else if (end != 0)
                {
                    query += " WHERE (SELECT date_part('year', p.data_pogrzebu) <= " + end + ")";
                }
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                long count = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    count = (long)dr[0];
                    if ((start == end) && (start != 0))
                        Console.WriteLine("W roku " + start + " udzielono " + count + " pogrzebów.");
                    else if ((start == end) && (start == 0))
                        Console.WriteLine("W parafii udzielono razem " + count + " pogrzebów.");
                    else
                        Console.WriteLine("Od roku " + start + " do roku " + end + " udzielono " + count + " pogrzebów.");
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                return count;
            }
            return 0;
        }

        public static void Correct()
        {
            Console.WriteLine("Jakie informacje chcesz poprawić?");
            Console.WriteLine("Wpisz odpowiednio: \"dane\", \"chrzest\", \"pierwsza komunia\", \"bierzmowanie\", \"slub\" lub \"pogrzeb\")");
            string answer = Console.ReadLine();
            if (answer == "dane")
            {
                Console.WriteLine("Podaj PESEL osoby, której informacje chcesz poprawić.");
                string pesel = "'" +  Console.ReadLine() + "'";
                Console.WriteLine("Podaj imię:");
                string imie = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj nazwisko:");
                string nazwisko = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj nowy PESEL (jeśli nie chceszz zmienić, wpisz ten sam)");
                string new_pesel = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj datę urodzenia (w formacie YYYY-MM-DD):");
                string data_urodzenia = "date '" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj płeć (K lub M):");
                string plec = "'" + Console.ReadLine()[0] + "'";
                Console.WriteLine("Podaj PESEL ojca (wpisz \"nieznany\" jeśli ojciec nie należy do parafii):");
                string ojciec_pesel = Console.ReadLine();
                if (ojciec_pesel == "nieznany")
                    ojciec_pesel = "null";
                else
                    ojciec_pesel = "'" + ojciec_pesel + "'";
                Console.WriteLine("Podaj PESEL matki (wpisz \"nieznany\" jeśli matka nie należy do parafii):");
                string matka_pesel = Console.ReadLine();
                if (matka_pesel == "nieznany")
                    matka_pesel = "null";
                else
                    matka_pesel = "'" + matka_pesel + "'";
                
                string query = "UPDATE parafianie SET pesel = " + new_pesel + ", nazwisko = " + nazwisko + ", imie = " + imie + ", data_urodzenia = " + data_urodzenia + ", plec = " + plec + ", ";
                query += "ojciec = " + ojciec_pesel + ", matka = " + matka_pesel + ", data_urodzenia = " + data_urodzenia + " WHERE pesel = " + pesel;
                Console.WriteLine(query);
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read()) ;
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
            else if (answer == "chrzest")
            {
                Console.WriteLine("Podaj PESEL osoby, której informacje chcesz poprawić.");
                string pesel = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj imię (imiona) nadane na chrzcie:");
                string imiona = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj imię i nazwisko ojca chrzestnego:");
                string dane_ojca = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj imię i nazwisko matki chrzestnej:");
                string dane_matki = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj datę chrztu (w formacie YYYY-MM-DD):");
                string data = "date '" + Console.ReadLine() + "'";
                string query = "UPDATE chrzty SET imie_chrztu = " + imiona + ", ojciec_chrzestny = " + dane_ojca + ", matka_chrzestna = " + dane_matki + " WHERE osoba = " + pesel;
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read()) ;
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                query = "SELECT id_sakramentu FROM chrzty WHERE osoba = " + pesel;
                command = new NpgsqlCommand(query, connection);
                dr = null;
                int id = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    id = (int)dr[0];
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                query = "UPDATE sakramenty SET data_udzielenia = " + data + " WHERE id = " + id;
                command = new NpgsqlCommand(query, connection);
                dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read()) ;
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
            else if (answer == "pierwsza komunia")
            {
                Console.WriteLine("Podaj PESEL osoby, której informacje chcesz poprawić.");
                string pesel = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj datę I Komunii (w formacie YYYY-MM-DD):");
                string data = "date '" + Console.ReadLine() + "'";
                string query = "SELECT id_sakramentu FROM pierwsze_komunie WHERE osoba = " + pesel;
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                int id = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    id = (int)dr[0];
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                query = "UPDATE sakramenty SET data_udzielenia = " + data + " WHERE id = " + id;
                command = new NpgsqlCommand(query, connection);
                dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read()) ;
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }

            else if (answer == "bierzmowanie")
            {
                Console.WriteLine("Podaj PESEL osoby, której informacje chcesz poprawić.");
                string pesel = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj imię nadane na bierzmowaniu:");
                string imie = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj imię i nazwisko świadka:");
                string swiadek = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj datę bierzmowania (w formacie YYYY-MM-DD):");
                string data = "date '" + Console.ReadLine() + "'";
                string query = "UPDATE bierzmowania SET imie_bierzmowania = " + imie + ", swiadek = " + swiadek + " WHERE osoba = " + pesel;
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read()) ;
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                query = "SELECT id_sakramentu FROM bierzmowania WHERE osoba = " + pesel;
                command = new NpgsqlCommand(query, connection);
                dr = null;
                int id = 0;
                try
                {
                    dr = command.ExecuteReader();
                    dr.Read();
                    id = (int)dr[0];
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                query = "UPDATE sakramenty SET data_udzielenia = " + data + " WHERE id = " + id;
                command = new NpgsqlCommand(query, connection);
                dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read()) ;
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }

            else if (answer == "slub")
            {
                Console.WriteLine("Czy mężczyzna jest z parafii? (T/N)");
                string resp = Console.ReadLine();
                string pesel_mezczyzny = null;
                string dane_mezczyzny = null;
                string pesel_kobiety = null;
                string dane_kobiety = null;
                if (resp == "T")
                {
                    Console.WriteLine("Podaj PESEL mężczyzny:");
                    pesel_mezczyzny = " = '" + Console.ReadLine() + "'";
                    dane_mezczyzny = " is NULL";
                }
                else if (resp == "N")
                {
                    Console.WriteLine("Podaj dane mężczyzny:");
                    pesel_mezczyzny = " is NULL";
                    dane_mezczyzny = " = '" + Console.ReadLine() + "'";
                }
                Console.WriteLine("Czy kobieta jest z parafii? (T/N)");
                resp = Console.ReadLine();
                if (resp == "T")
                {
                    Console.WriteLine("Podaj PESEL kobiety:");
                    pesel_kobiety = " = '" + Console.ReadLine() + "'";
                    dane_kobiety = " is NULL";
                }
                else if (resp == "N")
                {
                    Console.WriteLine("Podaj dane kobiety:");
                    pesel_kobiety = " is NULL";
                    dane_kobiety = " = '" + Console.ReadLine() + "'";
                }
                Console.WriteLine("Podaj starą datę ślubu (w formacie YYYY-MM-DD)");
                string data = "date '" + Console.ReadLine() + "'";

                Console.WriteLine("Podaj nową datę ślubu (w formacie YYYY-MM-DD)");
                string newdata = "date '" + Console.ReadLine() + "'";

                Console.WriteLine("Podaj dane I świadka:");
                string s1 = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj dane II świadka:");
                string s2 = "'" + Console.ReadLine() + "'";

                string join = "SELECT sa.id FROM sluby sl JOIN sakramenty sa ON sl.id_sakramentu = sa.id WHERE sa.data_udzielenia = " + data + " AND ";
                join += "sl.maz" + pesel_mezczyzny + " AND sl.dane_meza" + dane_mezczyzny + " AND sl.zona" + pesel_kobiety;
                join += " AND sl.dane_zony" + dane_kobiety;
                Console.WriteLine("joinQuery " + join);
                NpgsqlCommand command = new NpgsqlCommand(join, connection);
                NpgsqlDataReader dr = null;
                int id = 0;
                try
                {
                    dr = command.ExecuteReader();
                    bool read = dr.Read();
                    if (!read)
                    {
                        System.Console.WriteLine("Błąd, nie ma ślubu o podanych parametrach!");
                        return;
                    }
                    for (int i = 0; i < dr.FieldCount; ++i)
                        System.Console.WriteLine(i + " -> " + dr[i]);
                    id = (int)dr[0];
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                    return;
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                    dr = null;
                }
                string query = "UPDATE sluby SET ";                
                query += "swiadek1 = " + s1 + ", swiadek2 = " + s2 + " WHERE id_sakramentu = " + id;
                Console.WriteLine("query 2 : " + query);

                command = new NpgsqlCommand(query, connection);
                try
                {
                    dr = command.ExecuteReader();
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine("BŁĄD: " + exc.BaseMessage);
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
                query = "UPDATE sakramenty SET data_udzielenia = " + newdata + " WHERE id = " + id;
                command = new NpgsqlCommand(query, connection);
                try
                {
                    dr = command.ExecuteReader();
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.BaseMessage);
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }

            else if (answer == "pogrzeb")
            {
                Console.WriteLine("Podaj PESEL osoby zmarłej:");
                string pesel = "'" + Console.ReadLine() + "'";
                Console.WriteLine("Podaj datę śmierci (w formacie YYYY-MM-DD) albo wpisz \"nieznana\":");
                string data_smierci = Console.ReadLine();
                if (data_smierci == "nieznana")
                    data_smierci = "null";
                else
                    data_smierci = "date '" + data_smierci + "'";
                Console.WriteLine("Podaj datę pogrzebu:");
                string data_pogrzebu = "date '" + Console.ReadLine() + "'";

                NpgsqlCommand command = new NpgsqlCommand("UPDATE pogrzeby SET data_smierci = " + data_smierci + ", data_pogrzebu = " + data_pogrzebu + " WHERE osoba = " + pesel, connection);
                NpgsqlDataReader dr = null;
                try
                {
                    dr = command.ExecuteReader();
                    while (dr.Read()) ;
                }
                catch (NpgsqlException exc)
                {
                    Console.WriteLine(exc.Message);
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Witaj w aplikacji eParafia!");
            while (logged == false)
                LogIn();
            Console.WriteLine("Pomyślnie zalogowano.");
            Console.WriteLine("Aby wyświetlić instrukcję, wpisz \"manual\".");
            Console.WriteLine("Aby zakończyć, wpisz \"koniec\".");
            try
            {
                while (true)
                {
                    Console.Write(">>> ");
                    string command = Console.ReadLine();
                    if (command == "koniec")
                        break;
                    else
                        ExecuteCommand(command);
                }
            }
            finally
            {
                if(connection != null)
                    connection.Close();
            }
        }
    }   
}
