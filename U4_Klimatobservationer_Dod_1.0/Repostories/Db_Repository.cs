using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using U4_Klimatobservationer_Dod_1._0.Models;



namespace U4_Klimatobservationer_Dod_1._0.Repostories
{
    public class Db_Repository
    {
        public List<Observer> observers = new List<Observer>();
        public List<Observation> observations = new List<Observation>();
        Polymorph_Tools po = new Polymorph_Tools();

        private string _connectionString;
        public Db_Repository()
        {
            
            var config = new ConfigurationBuilder().AddUserSecrets<Db_Repository>()
                        .Build();
            _connectionString = config.GetConnectionString("dbConn");
        }

        public bool AddObserver(string firstname, string lastname)
        {
            //metod för att lägga till observatör
            string CheckIfLetters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvXxYyZzÅåÄäÖö";
            foreach (char c in firstname)
            {
                if (!CheckIfLetters.Contains(c))
                {
                    return false;
                }
            }

            foreach (char c in lastname)
            {
                if (!CheckIfLetters.Contains(c))
                {
                    return false;
                }
            }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("insert into observer");
                sql.AppendLine("(firstname ,lastname) ");
                sql.AppendLine("values(@firstname , @lastname) ");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                command.Parameters.AddWithValue("@firstname",firstname );
                command.Parameters.AddWithValue("@lastname", lastname );
                var result = command.ExecuteScalar();
                return true;
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23503")
                {
                    if (ex.ConstraintName == "person_sex_id_fkey")
                    {
                        throw new Exception("fel kön", ex);
                    }
                    throw new Exception("Du försöker ge en rank till en pirat som inte existerar", ex);
                }
                throw new Exception("Allvarligt fel, får inte kontakt med databasen", ex);

            }

            return false;
        
        }

        public bool CheckIfObserverHasDoneObservation(int id)
        {
            try
            {
                // metod för att kontrollera om observatör gjort observation
                Observation? observation = null;
                List<Observation> observations = new List<Observation>();
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("select ");
                sql.AppendLine("observer_id ");
                sql.AppendLine("from observation");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        observation = new Observation();
                        {

                            observation.observer_id = reader.GetInt32(0);
                        }
                        observations.Add(observation);
                    }
                    foreach (var item in observations)
                    {
                        if (item.observer_id == id)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        public bool CheckIfInput_ID_MatchesObserverID(int id)
        {
            try
            {
                // metod för att kontrollera om observatör gjort observation
                Observation? observation = null;
                List<Observation> observations = new List<Observation>();
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("select ");
                sql.AppendLine("observer_id ");
                sql.AppendLine("from observation");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        observation = new Observation();
                        {

                            observation.observer_id = reader.GetInt32(0);
                        }
                        observations.Add(observation);
                    }
                    foreach (var item in observations)
                    {
                        if (item.observer_id == id)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }












        public void RemoveObserver(int id)
        {
            // metod för att radera
            Observation? observation = null;
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("delete from observer ");
                sql.AppendLine("where id =@id");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                command.Parameters.AddWithValue("@id", id);
                var result = command.ExecuteScalar();
            }

            catch (PostgresException ex)
            {
                throw new Exception("Allvarligt fel, får inte kontakt med databasen", ex);
            }
        }

        public void RegisterMeasurement(double Value_Input,int? Observer_Id_Input, int Category_Id_Input)
        {
           
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("insert into measurement ");
                sql.AppendLine("(value ,observation_id, category_id) ");
                sql.AppendLine("values(@value ,@observation_id,@category_id)");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                command.Parameters.AddWithValue("@value", Value_Input);
                command.Parameters.AddWithValue("@observation_id", Observer_Id_Input);
                command.Parameters.AddWithValue("@category_id", Category_Id_Input);
                var result = command.ExecuteScalar();
            }
            catch (PostgresException ex)
            {
                throw new Exception("Allvarligt fel, får inte kontakt med databasen", ex);
            }

        }

        public double? SelectGeolocation()
        {
            // Slumpar geolocation. Tas någon av dessa bort i postgres kraschar hela systemet.
            Random random = new Random();
            random.Next(101);
      
            if (random.Next(101) <= 33 && random.Next(101) >= 0)
            {
                return 2;
            }
            if (random.Next(101) <= 66 && random.Next(101) > 33)
            {
                return 3;
            }
            if (random.Next(101) <= 100 && random.Next(101) > 66)
            {
                return 4;
            }
            return 0;
        }

        public List<Measurement> GetObservationReportedMeasurements(int? id_input)
        {
            // Hämtar alla measurements från en observation
            try
            {
                var Measurements = new List<Measurement>();
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var cmd = new NpgsqlCommand();
                cmd.CommandText = "Select * FROM measurement WHERE observation_id =@observation_id ";
                cmd.Parameters.AddWithValue("@observation_id", id_input);
                cmd.Connection = conn;
                Measurement? measurement = null;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        measurement = new Measurement();
                        {
                            measurement.id = reader.GetInt32(0);
                            measurement.value = reader.GetDouble(1);
                            measurement.observation_id = reader["observation_id"] == DBNull.Value ? null : (int?)reader["observation_id"];
                            measurement.category_id = reader["category_id"] == DBNull.Value ? null : (int?)reader["category_id"];
                        }
                        Measurements.Add(measurement);
                    }

                }
                return Measurements;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Observation> GetObserverReportedObservation(int? id_input)
        {
            // Hämtar alla observationer som en observatör rapporterat
            try
            {
                var Observations = new List<Observation>();
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var cmd = new NpgsqlCommand();
                cmd.CommandText = "Select * FROM observation WHERE observer_id =@observer_id ";
                cmd.Parameters.AddWithValue("@observer_id", id_input);
                cmd.Connection = conn;
                Observation? observation = null;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        observation = new Observation();
                        {
                            observation.id = reader.GetInt32(0);
                            observation.date = reader["date"] == DBNull.Value ? null : (DateTime?)reader["date"];
                            observation.observer_id = reader["observer_id"] == DBNull.Value ? null : (int?)reader["observer_id"];
                            observation.geolocation_id = reader["geolocation_id"] == DBNull.Value ? null : (int?)reader["geolocation_id"];
                        }
                        Observations.Add(observation);
                    }
                }
                return Observations;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RegisterObservation(int? Observer_ID_input,double? Geolocation_ID_input)
        {

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("insert into observation");
                sql.AppendLine("(observer_id ,geolocation_id) ");
                sql.AppendLine("values(@Observer_ID , @Geolocation_ID)");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
              command.Parameters.AddWithValue("@Observer_ID", Observer_ID_input);
              command.Parameters.AddWithValue("@Geolocation_ID", Geolocation_ID_input);
                var result = command.ExecuteScalar();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23503")
                {
                    if (ex.ConstraintName == "person_sex_id_fkey")
                    {
                        throw new Exception("fel kön", ex);

                    }
                    throw new Exception("Du försöker ge en rank till en pirat som inte existerar", ex);

                }
                throw new Exception("Allvarligt fel, får inte kontakt med databasen", ex);
            }

        }

        public int? CatchCreatedObservationID()
        {
            // Fångar det sista Observation.ID och retunerar det. 
            int? CatchCreatedID = 0;
            Observation? observation = null;
            List<Observation> observations = new List<Observation>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("select ");
                sql.AppendLine("id");
                sql.AppendLine("from observation");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        observation = new Observation();
                        {
                            observation.id = reader.GetInt32(0);
                            for (int i = 0; i < observation.id; i++)
                            {
                                CatchCreatedID = observation.id;
                            }
                        }
                        observations.Add(observation);
                    }
                    return CatchCreatedID;
                }
            }
            catch (PostgresException ex)
            {
                throw new Exception("Allvarligt fel får inte kontakt", ex);
            }
        }

        public void UpdateObservationMeasurement(double? input_value, int? id)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("UPDATE measurement ");
                sql.AppendLine("SET value=@value ");
                sql.AppendLine("WHERE id=@id");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                command.Parameters.AddWithValue("@value", input_value);
                command.Parameters.AddWithValue("@id", id);
                var result = command.ExecuteScalar();
            }
            catch (Exception)
            {

                throw;
            }
        }   

        public List<Measurement> GetMeasurementObservation_id()
        {
            // Hämtar Observation_id i measurement
            int? CatchCreatedID = 0;
            Measurement? measurement = null;
            List<Measurement> measurements = new List<Measurement>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                StringBuilder sql = new StringBuilder("select ");
                sql.AppendLine("observation_id");
                sql.AppendLine("from measurement");
                using var command = new NpgsqlCommand(sql.ToString(), conn);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        measurement = new Measurement();
                        {
                            measurement.observation_id = reader.GetInt32(0);
                        }
                        measurements.Add(measurement);
                    }
                    return measurements;
                }
            }
            catch (PostgresException ex)
            {
                throw new Exception("Allvarligt fel får inte kontakt", ex);
            }
        }

        public List<Observer> GetObservers()
        {
            
            try
            {
                var Observers = new List<Observer>();
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var cmd = new NpgsqlCommand();
                cmd.CommandText = "Select id, firstname,lastname FROM observer ORDER BY lastname";
                cmd.Connection = conn;
                Observer? observer = null;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        observer = new Observer();
                        {
                            observer.id = reader.GetInt32(0);
                            observer.firstname = reader["firstname"] == DBNull.Value ? null : (string?)reader["firstname"];
                            observer.lastname = reader["lastname"] == DBNull.Value ? null : (string?)reader["lastname"];
                        }
                        Observers.Add(observer);
                    }
                }
                return Observers;
            }
            catch (PostgresException ex)
            {
                throw new Exception("Någonting i databasen stämmer inte överens med koden i programmet. Specifikt i metoden GetObservers()", ex);
            }
        }

        public bool CheckIfObserverDBEmpty()
        {
            try
            {
                var Observers = new List<Observer>();
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var cmd = new NpgsqlCommand();
                cmd.CommandText = "Select id, firstname,lastname FROM observer ORDER BY lastname";
                cmd.Connection = conn;
                Observer? observer = null;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        observer = new Observer();
                        {
                            observer.id = reader.GetInt32(0);
                            observer.firstname = reader["firstname"] == DBNull.Value ? null : (string?)reader["firstname"];
                            observer.lastname = reader["lastname"] == DBNull.Value ? null : (string?)reader["lastname"];
                        }
                        Observers.Add(observer);
                    }
                    if (Observers.Count == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (PostgresException ex)
            {
                throw new Exception("Någonting i databasen stämmer inte överens med koden i programmet. Specifikt i metoden GetObservers()", ex);
            }
        }



        public bool BlockLetters(string input)
        {
            bool IsLetter = false;
            bool EmptyTextBox = false;


            foreach (char c in input)
            {
                if (c == ' ')
                {
                    EmptyTextBox = true;
                    return false;
                }


                if (char.IsLetter(c))
                {
                    IsLetter = true;
                }
            }
            IsLetter = IsLetter == true ? false : true;
            return true;
        }









    }
}
