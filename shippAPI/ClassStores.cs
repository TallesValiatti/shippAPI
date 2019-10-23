using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace shippAPI
{
    public class ClassStores
    {

        #region private variables

        private double maxDistance = 6.5;

        #endregion

        #region properties

        public string County { get; set; }
        public string LicenseNumber { get; set; }
        public string OperationType { get; set; }
        public string EstablishmentType { get; set; }
        public string EntityName { get; set; }
        public string DBAName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string SquareFootage { get; set; }
        public string Location { get; set; }
        public string Distance { get; set; }
        public double rawDistance { get; set; }
        #endregion

        #region methods
        public ClassStores()
        {
        }

        public bool ListStores(ref List<ClassStores> lstStores, double latitudeAPI, double longitudeAPI)
        {
            try
            {
                var connectionString = "data source=database/database;";
                //var connectionString = "";
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var selectCommand = connection.CreateCommand();

                    selectCommand.CommandText = "SELECT * FROM tblStores";

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        ClassStores objTemp = null;
                        double distanceTemp = 0;
                        while (reader.Read())
                        {
                            objTemp = new ClassStores();

                            objTemp.Location = reader.GetString(14).Trim();
                            if (!calculateDistance(objTemp.Location, ref distanceTemp, latitudeAPI, longitudeAPI))
                                continue;

                            objTemp.Distance = distanceTemp.ToString("N2") + " KM";
                            objTemp.rawDistance = distanceTemp;

                            objTemp.County = reader.GetString(0).Trim();
                            objTemp.LicenseNumber = reader.GetString(1).Trim();
                            objTemp.OperationType = reader.GetString(2).Trim();
                            objTemp.EstablishmentType = reader.GetString(3).Trim();
                            objTemp.EntityName = reader.GetString(4).Trim();
                            objTemp.DBAName = reader.GetString(5).Trim();
                            objTemp.StreetNumber = reader.GetString(6).Trim();
                            objTemp.StreetName = reader.GetString(7).Trim();
                            objTemp.AddressLine2 = reader.GetString(8).Trim();
                            objTemp.AddressLine3 = reader.GetString(9).Trim();
                            objTemp.City = reader.GetString(10).Trim();
                            objTemp.State = reader.GetString(11).Trim();
                            objTemp.ZipCode = reader.GetString(12).Trim();
                            objTemp.SquareFootage = reader.GetString(13).Trim();
                            
                            lstStores.Add(objTemp);
                            
                        }
                    }
                    connection.Close();
                }
                //OK
                return true;
            }
            catch(SqliteException)
            {
                //Exception on open and process the query at sqlite database
                return false;
            }
            catch (Exception ex)
            {
                //Generic Exception
                var a = ex.Message;
                return false;
            }

        }

        private bool calculateDistance(string location, ref double distance, double latitudeAPI, double longitudeAPI)
        {
            var arrayStr = location.Split(",");

            var latitudeStr = "";
            var longitudeStr = "";

            foreach (var obj in arrayStr)
            {
                if (obj.Contains("'latitude'"))
                    latitudeStr = obj.Replace("'latitude'", string.Empty).Replace(":", string.Empty).Replace("}", string.Empty).Replace("{", string.Empty).Replace("'", string.Empty).Trim();
                else if (obj.Contains("'longitude'"))
                    longitudeStr = obj.Replace("'longitude'", string.Empty).Replace(":", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty).Replace("'", string.Empty).Trim();
            }

            if (string.IsNullOrEmpty(longitudeStr) || string.IsNullOrEmpty(longitudeStr))
                return false;

            var latitudeStore = (double)Convert.ToDecimal(latitudeStr, CultureInfo.InvariantCulture.NumberFormat);
            var longitudeStore = (double)Convert.ToDecimal(longitudeStr, CultureInfo.InvariantCulture.NumberFormat);

            distance = DistanceTwoCoordinates((double)latitudeAPI, (double)longitudeAPI, latitudeStore, longitudeStore);

            if (distance > this.maxDistance)
                return false;

            return true;
        }

        private double DistanceTwoCoordinates(double Lat1, double Long1, double Lat2, double Long2)
        {
            double dDistance = Double.MinValue;
            double dLat1InRad = Lat1 * (Math.PI / 180.0);
            double dLong1InRad = Long1 * (Math.PI / 180.0);
            double dLat2InRad = Lat2 * (Math.PI / 180.0);
            double dLong2InRad = Long2 * (Math.PI / 180.0);

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Asin(Math.Sqrt(a));

            // Distance.
            // const Double kEarthRadiusMiles = 3956.0;
            const Double kEarthRadiusKms = 6376.5;
            dDistance = kEarthRadiusKms * c;

            return dDistance;
        }
        #endregion

    }
}
