using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;
using Mono.Data.Sqlite;
using System.IO;
using System.Linq;

public class DatabaseManager : MonoBehaviour {

	private string path;
    public ManagersContainer managers;
    public UIShopManager uiManager_shop;

    // Use this for initialization
    void Start () {
		checkPath ();
    }
    

    ////////////////////////////////////////////////////////////////////////////
    //////////////////////      PlantsOnField       ////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addPlant(float x, float z, int[] dna)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("INSERT INTO Plant(stalk, cup, leaves, creepers, spikes, teeth, fruits) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "SELECT last_insert_rowid()";
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                query = String.Format("INSERT INTO PlantsOnField(posX, posY, posZ, plant_id, born_time, fruit_time) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\")", x, 0, z, plant_id, Utilities.getCurrentTime(), Utilities.getCurrentTime());
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();
                
                dbConnection.Close();
            }
        }
    }
    public void removePlant(float x, float z)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT plant_id FROM PlantsOnField WHERE posX=\"{0}\" and posZ=\"{1}\"", x, z);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                query = String.Format("DELETE FROM PlantsOnField WHERE posX=\"{0}\" and posZ=\"{1}\"", x, z);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "DELETE FROM Plant WHERE id =" + plant_id;
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                dbConnection.Close();
            }
        }
    }
    public List<PlantOnField> getPlantsOnField()
    {
        List<PlantOnField> plants = new List<PlantOnField>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT posX, posY, posZ, life_bonus, fruit_time, stalk, cup, leaves, creepers, spikes, teeth, fruits FROM Plant, PlantsOnField WHERE Plant.id = PlantsOnField.plant_id";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PlantOnField plant = new PlantOnField(new Vector3(reader.GetFloat(0), reader.GetFloat(1), reader.GetFloat(2)),
                            reader.GetInt32(3),
                            DateTime.Parse(reader.GetString(4)),
                            new int[] { reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10), reader.GetInt32(11) }
                            );
                        plants.Add(plant);
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return plants;
    }
    public int[] getDnaOfPlantOnField(float x, float z)
    {
        int[] dna;

        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits FROM Plant, PlantsOnField WHERE Plant.id = PlantsOnField.plant_id AND PlantsOnField.posX =\"{0}\" AND PlantsOnField.posZ =\"{1}\"", x, z);
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    reader.Read();
                    dna = new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) };
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return dna;
    }
    public DateTime getBornTimeOfPlantOnField(float x, float z)
    {
        DateTime bornTime;

        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT born_time FROM PlantsOnField WHERE posX =\"{0}\" AND posZ =\"{1}\"", x, z);
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    reader.Read();
                    bornTime = DateTime.Parse(reader.GetString(0));
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return bornTime;
    }
    public void setBornTimeOfPlantOnFieldToActual(GameObject plant)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                DateTime dt = Utilities.getCurrentTime();
                string query = String.Format("UPDATE PlantsOnField SET born_time=\"{0}\" WHERE posX =\"{1}\" AND posZ = \"{2}\"", dt, plant.transform.position.x, plant.transform.position.z);
                plant.GetComponent<PlantCreating>().bornTime = dt;
                dbCommand.CommandText = query;
                dbCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }
    }
    public void updateLifeBonusForPlant(float x, float z, int bonus)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                DateTime dt = Utilities.getCurrentTime();
                string query = String.Format("UPDATE PlantsOnField SET life_bonus=\"{0}\" WHERE posX =\"{1}\" AND posZ = \"{2}\"", bonus, x, z);
                dbCommand.CommandText = query;
                dbCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }
    }
    public void updateFruitTimeForCurrent(float x, float z)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                DateTime dt = Utilities.getCurrentTime();
                string query = String.Format("UPDATE PlantsOnField SET fruit_time=\"{0}\" WHERE posX =\"{1}\" AND posZ = \"{2}\"", Utilities.getCurrentTime(), x, z);
                dbCommand.CommandText = query;
                dbCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////       PlantsInBag        ////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addPlantToBag(int[] dna, int countToAdd)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT COUNT(PlantsInBag.id) from Plant, PlantsInBag WHERE Plant.id = plant_id AND stalk =\"{0}\" AND cup =\"{1}\" AND leaves = \"{2}\" AND creepers =\"{3}\" AND spikes =\"{4}\" AND teeth =\"{5}\" AND fruits=\"{6}\"", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int count = Convert.ToInt32(dbCommand.ExecuteScalar());

                if (count != 0)
                {
                    query = String.Format("UPDATE PlantsInBag SET count=count+\"{0}\" WHERE plant_id=(SELECT id from Plant where Plant.id = plant_id AND stalk =\"{1}\" AND cup =\"{2}\" AND leaves = \"{3}\" AND creepers =\"{4}\" AND spikes =\"{5}\" AND teeth =\"{6}\" AND fruits=\"{7}\")", countToAdd, dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteNonQuery();
                }
                else
                {
                    query = String.Format("INSERT INTO Plant(stalk, cup, leaves, creepers, spikes, teeth, fruits) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteScalar();

                    query = "SELECT last_insert_rowid()";
                    dbCommand.CommandText = query;
                    dbCommand.CommandType = CommandType.Text;
                    int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                    query = String.Format("INSERT INTO PlantsInBag(count, plant_id) values (\"{0}\",\"{1}\")", countToAdd, plant_id);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteScalar();
                }

                dbConnection.Close();
            }
        }
    }
    public List<PlantInBag> getPlantsInBag()
    {
        List<PlantInBag> plants = new List<PlantInBag>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits, count FROM Plant, PlantsInBag WHERE Plant.id = PlantsInBag.plant_id";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PlantInBag plant = new PlantInBag(new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) }, reader.GetInt32(7));
                        plants.Add(plant);
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return plants;
    }
    public int getCountOfPlantInBag(int[] dna)
    {
        int count = 0;
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT count FROM PlantsInBag, Plant WHERE Plant.id = PlantsInBag.plant_id AND stalk=\"{0}\" AND cup=\"{1}\" AND leaves=\"{2}\" AND creepers=\"{3}\" AND spikes=\"{4}\" AND teeth=\"{5}\" AND fruits=\"{6}\"", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                count = Convert.ToInt32(dbCommand.ExecuteScalar());

                dbConnection.Close();
            }
        }
        return count;
    }
    public void subtractOnePlantFromBag(int[] dna)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT count from Plant, PlantsInBag WHERE Plant.id = plant_id AND stalk =\"{0}\" AND cup =\"{1}\" AND leaves = \"{2}\" AND creepers =\"{3}\" AND spikes =\"{4}\" AND teeth =\"{5}\" AND fruits=\"{6}\"", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int count = Convert.ToInt32(dbCommand.ExecuteScalar());

                if (count > 1)
                {
                    query = String.Format("UPDATE PlantsInBag SET count=count-1 WHERE plant_id=(SELECT id from Plant where Plant.id = plant_id AND stalk =\"{0}\" AND cup =\"{1}\" AND leaves = \"{2}\" AND creepers =\"{3}\" AND spikes =\"{4}\" AND teeth =\"{5}\" AND fruits=\"{6}\")", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteNonQuery();
                }
                else
                {
                    query = String.Format("SELECT plant_id FROM PlantsInBag, Plant WHERE Plant.id = plant_id AND stalk =\"{0}\" AND cup =\"{1}\" AND leaves = \"{2}\" AND creepers =\"{3}\" AND spikes =\"{4}\" AND teeth =\"{5}\" AND fruits=\"{6}\"", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                    dbCommand.CommandText = query;
                    dbCommand.CommandType = CommandType.Text;
                    int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                    query = String.Format("DELETE FROM PlantsInBag WHERE plant_id=\"{0}\"", plant_id);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteScalar();

                    query = "DELETE FROM Plant WHERE id =" + plant_id;
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteScalar();
                }

                dbConnection.Close();
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////     PlantsDiscovered     ////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addPlantToDiscovered(int[] dna)
    {
        List<int[]> dnaDiscovered = getDiscoveredDnas();
        if (!Utilities.ListContainsDna(dnaDiscovered, dna))
        {
            using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
            {
                dbConnection.Open();
                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    string query = String.Format("SELECT COUNT(id) from PlantsDiscovered WHERE stalk =\"{0}\" AND cup =\"{1}\" AND leaves = \"{2}\" AND creepers =\"{3}\" AND spikes =\"{4}\" AND teeth =\"{5}\" AND fruits=\"{6}\"", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                    dbCommand.CommandText = query;
                    dbCommand.CommandType = CommandType.Text;
                    int count = Convert.ToInt32(dbCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        query = String.Format("INSERT INTO PlantsDiscovered(stalk, cup, leaves, creepers, spikes, teeth, fruits) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                        dbCommand.CommandText = query;
                        dbCommand.ExecuteScalar();
                    }

                    dbConnection.Close();
                }
            }
        }
    }
    public List<Plant> getDiscoveredPlants()
    {
        List<Plant> plants = new List<Plant>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits FROM PlantsDiscovered";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Plant plant = new Plant(new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) });
                        plants.Add(plant);
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }

        return plants;
    }
    public List<int[]> getDiscoveredDnas()
    {
        List<int[]> dnas = new List<int[]>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits FROM PlantsDiscovered";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dnas.Add(new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) });
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }

        return dnas;
    }
    public int getCountOfDiscoveredPlants()
    {
        int count = 0;
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT COUNT(id) from PlantsDiscovered";
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                count = Convert.ToInt32(dbCommand.ExecuteScalar());
                dbConnection.Close();
            }
        }
        return count;
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////      PlantBuilders       ////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addPlantBuilder(ActionPlant actionPlant)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("INSERT INTO Plant(stalk, cup, leaves, creepers, spikes, teeth, fruits) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", actionPlant.Dna[0], actionPlant.Dna[1], actionPlant.Dna[2], actionPlant.Dna[3], actionPlant.Dna[4], actionPlant.Dna[5], actionPlant.Dna[6]);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "SELECT last_insert_rowid()";
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                query = String.Format("INSERT INTO PlantBuilders(posX, posZ, plant_id, start_time, plant_increase) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\")", actionPlant.Position.x, actionPlant.Position.z, plant_id, actionPlant.StartTime, actionPlant.PlantIncreaseBonus);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                dbConnection.Close();
            }
        }
    }
    public void removePlantBuilder(float x, float z)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT plant_id FROM PlantBuilders WHERE posX=\"{0}\" and posZ=\"{1}\"", x, z);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                query = String.Format("DELETE FROM PlantBuilders WHERE posX=\"{0}\" and posZ=\"{1}\"", x, z);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "DELETE FROM Plant WHERE id =" + plant_id;
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                dbConnection.Close();
            }
        }
    }
    public List<ActionPlant> getPlantBuilders()
    {
        List<ActionPlant> plantBuilders = new List<ActionPlant>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits, posX, posZ, start_time, plant_increase FROM Plant, PlantBuilders WHERE Plant.id = PlantBuilders.plant_id";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ActionPlant actionPlant = new ActionPlant(
                            new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) },
                            new Vector3(reader.GetFloat(7), 0, reader.GetFloat(8)),
                            DateTime.Parse(reader.GetString(9)),
                            reader.GetInt32(10));
                        
                        plantBuilders.Add(actionPlant);
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return plantBuilders;
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////       ActionCross        ////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addActionCross(ActionCross actionCross)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("INSERT INTO Plant(stalk, cup, leaves, creepers, spikes, teeth, fruits) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", actionCross.DnaOne[0], actionCross.DnaOne[1], actionCross.DnaOne[2], actionCross.DnaOne[3], actionCross.DnaOne[4], actionCross.DnaOne[5], actionCross.DnaOne[6]);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "SELECT last_insert_rowid()";
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id_one = Convert.ToInt32(dbCommand.ExecuteScalar());

                Debug.Log("TestTab: " + actionCross.DnaTwo[0]);
                query = String.Format("INSERT INTO Plant(stalk, cup, leaves, creepers, spikes, teeth, fruits) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", actionCross.DnaTwo[0], actionCross.DnaTwo[1], actionCross.DnaTwo[2], actionCross.DnaTwo[3], actionCross.DnaTwo[4], actionCross.DnaTwo[5], actionCross.DnaTwo[6]);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "SELECT last_insert_rowid()";
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id_two = Convert.ToInt32(dbCommand.ExecuteScalar());

                query = String.Format("INSERT INTO ActionsCross(plant_id_one, plant_id_two, start_time, cross_chance, crossing_time) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\")", plant_id_one, plant_id_two, actionCross.StartTime, actionCross.CrossChance, actionCross.LifeTimeInMinutes);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                dbConnection.Close();
            }
        }
    }
    public void removeActionCross(ActionCross actionCross)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT plant_id_one, plant_id_two FROM ActionsCross WHERE start_time=\"{0}\"", actionCross.StartTime);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id_one = 0;
                int plant_id_two = 0;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        plant_id_one = reader.GetInt32(0);
                        plant_id_two = reader.GetInt32(1);
                    }
                    reader.Close();
                }

                query = String.Format("DELETE FROM ActionsCross WHERE plant_id_one=\"{0}\" and plant_id_two=\"{1}\"", plant_id_one, plant_id_two);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "DELETE FROM Plant WHERE id =" + plant_id_one;
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "DELETE FROM Plant WHERE id =" + plant_id_two;
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                dbConnection.Close();
            }
        }
    }

    private struct StructActionCross
    {
        public int idOne { get; set; }
        public int idTwo { get; set; }
        public DateTime startTime { get; set; }
        public float crossChance { get; set; }
        public float crossingTime { get; set; }
        public StructActionCross(int idOne, int idTwo, DateTime startTime, float crossChance, float crossingTime)
        {
            this.idOne = idOne;
            this.idTwo = idTwo;
            this.startTime = startTime;
            this.crossChance = crossChance;
            this.crossingTime = crossingTime;
        }
    }

    public List<ActionCross> getActionCrosses()
    {
        List<ActionCross> actionCrosses = new List<ActionCross>();
        List<StructActionCross> structs = new List<StructActionCross>();
        //Dictionary<int[], DateTime> dataDictionary = new Dictionary<int[], DateTime>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT plant_id_one, plant_id_two, start_time, cross_chance, crossing_time FROM ActionsCross";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StructActionCross structAction = new StructActionCross(
                            reader.GetInt32(0), 
                            reader.GetInt32(1),
                            DateTime.Parse(reader.GetString(2)), 
                            reader.GetFloat(3),
                            reader.GetFloat(4));
                        structs.Add(structAction);
                    }
                    reader.Close();
                }
                foreach (StructActionCross s in structs)
                {
                    int[] dnaOne = null;
                    int[] dnaTwo = null;
                    //Debug.Log("id: " + s.idOne);
                    query = "SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits FROM Plant WHERE Plant.id = " + s.idOne;
                    dbCommand.CommandText = query;
                    using (IDataReader reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dnaOne = new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) };
                        }
                        reader.Close();
                    }
                    query = "SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits FROM Plant WHERE Plant.id = " + s.idTwo;
                    dbCommand.CommandText = query;
                    using (IDataReader reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dnaTwo = new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) };
                        }
                        reader.Close();
                    }
                    ActionCross actionCross = new ActionCross(dnaOne, dnaTwo, s.startTime, s.crossChance, s.crossingTime);
                    actionCrosses.Add(actionCross);
                }
                dbConnection.Close();
            }
        }
        return actionCrosses;
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////          Garbage           //////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addGarbage(float x, float z)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("INSERT INTO Garbages(posX, posZ) values (\"{0}\",\"{1}\")", x, z);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();
                
                dbConnection.Close();
            }
        }
    }
    public void removeGarbage(float x, float z)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("DELETE FROM Garbages WHERE posX=\"{0}\" and posZ=\"{1}\"", x, z);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();
                dbConnection.Close();
            }
        }
    }
    public List<Vector2> getGarbagesPositions()
    {
        List<Vector2> garbages = new List<Vector2>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT posX, posZ FROM Garbages";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        garbages.Add(new Vector2(reader.GetFloat(0), reader.GetFloat(1)));
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return garbages;
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////           Items            //////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addItem(Item item)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT COUNT(Items.id) from Items WHERE name =\"{0}\"", item.Name);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int count = Convert.ToInt32(dbCommand.ExecuteScalar());
                
                if (count != 0)
                {                    
                    query = String.Format("UPDATE Items SET count=count+1 WHERE name =\"{0}\"", item.Name);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteNonQuery();
                }
                else
                {
                    query = String.Format("INSERT INTO Items(count, name, type) values (\"{0}\",\"{1}\",\"{2}\")", 1, item.Name, item.GetType());
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteScalar();                    
                }

                dbConnection.Close();
            }
        }
    }
    public List<Item> getItemsByType(string type)
    {
        List<Item> items = new List<Item>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT name, type FROM Items";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(reader.GetString(1).Equals(type))
                        {
                            switch (type)
                            {
                                case "Shovel":
                                    Shovel shovel = uiManager_shop.shovels.Find(el => (el.Name.Equals(reader.GetString(0))));
                                    items.Add(shovel);
                                    break;
                                case "Fertilizer":
                                    Fertilizer fertilizer = uiManager_shop.fertilizers.Find(el => (el.Name.Equals(reader.GetString(0))));
                                    items.Add(fertilizer);
                                    break;
                                case "Conditioner":
                                    Conditioner conditioner = uiManager_shop.conditioners.Find(el => (el.Name.Equals(reader.GetString(0))));
                                    items.Add(conditioner);
                                    break;
                            }
                        }
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return items;
    }
    public int getCountOfItem(Item item)
    {
        int count = 0;
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT count FROM Items WHERE name=\"{0}\" AND type=\"{1}\"", item.Name, item.GetType().ToString());
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                count = Convert.ToInt32(dbCommand.ExecuteScalar());

                dbConnection.Close();
            }
        }
        return count;
    }
    public void subtractOneItem(string name)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT count FROM Items WHERE name =\"{0}\"", name);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int count = Convert.ToInt32(dbCommand.ExecuteScalar());

                if (count > 1)
                {
                    query = String.Format("UPDATE Items SET count=count-1 WHERE name =\"{0}\"", name);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteNonQuery();
                }
                else
                {
                    query = String.Format("DELETE FROM Items WHERE name =\"{0}\"", name);
                    dbCommand.CommandText = query;
                    dbCommand.ExecuteScalar();
                }

                dbConnection.Close();
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////         MISSIONS           //////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    public void addMission(int[] dna, int award)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("INSERT INTO Plant(stalk, cup, leaves, creepers, spikes, teeth, fruits) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "SELECT last_insert_rowid()";
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                query = String.Format("INSERT INTO Missions(plant_id, award) values (\"{0}\",\"{1}\")", plant_id, award);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                dbConnection.Close();
            }
        }
    }
    public int getCountOfMissions()
    {
        int count = 0;
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT COUNT(id) from Missions";
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                count = Convert.ToInt32(dbCommand.ExecuteScalar());
                dbConnection.Close();
            }
        }
        return count;
    }
    public List<int[]> getMissionsDnas()
    {
        List<int[]> dnas = new List<int[]>();
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = "SELECT stalk, cup, leaves, creepers, spikes, teeth, fruits FROM Plant, Missions WHERE Plant.id = Missions.plant_id";
                dbCommand.CommandText = query;
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dnas.Add(new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) });
                    }
                    reader.Close();
                }
                dbConnection.Close();
            }
        }
        return dnas;
    }
    public void removeMission(int[] dna)
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT plant_id FROM Missions, Plant WHERE Plant.id = Missions.plant_id and stalk=\"{0}\" and cup=\"{1}\" and leaves=\"{2}\" and creepers=\"{3}\" and spikes=\"{4}\" and teeth=\"{5}\" and fruits=\"{6}\"", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                int plant_id = Convert.ToInt32(dbCommand.ExecuteScalar());

                query = String.Format("DELETE FROM Missions WHERE plant_id=\"{0}\"", plant_id);
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                query = "DELETE FROM Plant WHERE id =" + plant_id;
                dbCommand.CommandText = query;
                dbCommand.ExecuteScalar();

                dbConnection.Close();
            }
        }
    }
    public int getAwardValue(int[] dna)
    {
        int award = 0;
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + path))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string query = String.Format("SELECT award FROM Missions, Plant WHERE Plant.id = Missions.plant_id and stalk=\"{0}\" AND cup=\"{1}\" AND leaves=\"{2}\" AND creepers=\"{3}\" AND spikes=\"{4}\" AND teeth=\"{5}\" AND fruits=\"{6}\"", dna[0], dna[1], dna[2], dna[3], dna[4], dna[5], dna[6]);
                dbCommand.CommandText = query;
                dbCommand.CommandType = CommandType.Text;
                award = Convert.ToInt32(dbCommand.ExecuteScalar());

                dbConnection.Close();
            }
        }
        return award;
    }
    ////////////////////////////////////////////////////////////////////////////
    //////////////////////           PATH             //////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    private void checkPath() {
		if (Application.platform != RuntimePlatform.Android) {
			path = Application.dataPath + "/PolyGainDB.sqlite";
		} else {
			path = Application.persistentDataPath + "/PolyGainDB.sqlite";
			if(!File.Exists(path)){
				WWW load = new WWW ("jar:file://" + Application.dataPath + "!/assets/" + "PolyGainDB.sqlite"); 
				while (!load.isDone){}
				File.WriteAllBytes (path, load.bytes);
			}    
		}
	}

}
