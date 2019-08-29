using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace GarminTcxToCsvTimeCoord
{
    class Program
    {
        static void Main(string[] args)
        {
            var setOfTrackPoints = new List<Trackpoint>();
            var orignalPath = args[0];
            var outputPath = args[1];
            Console.WriteLine($"Attempting to parse file {orignalPath}");
            using (var stream = new FileStream(path: orignalPath, FileMode.Open, FileAccess.Read)) 
            {
                using (var streamReader = new StreamReader(stream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var currentLine = streamReader.ReadLine();

                        if (currentLine.Contains(@"<Trackpoint>"))
                        {
                            var freshTrackPoint = new Trackpoint();
                            var nextLine = streamReader.ReadLine();
                            while (!nextLine.Contains(@"</Trackpoint>"))
                            {
                                if (nextLine.Contains("<Time>"))
                                {
                                    freshTrackPoint.Time = DateTime.Parse(nextLine.Split('<', '>')[2]);
                                } else if (nextLine.Contains("<Position>"))
                                {
                                    var pos = new Position();
                                    var latLine = streamReader.ReadLine();
                                    pos.Latitude = float.Parse(latLine.Split('<', '>')[2]);
                                    var longLine = streamReader.ReadLine();
                                    pos.Longitude = float.Parse(longLine.Split('<', '>')[2]);
                                    freshTrackPoint.Position = pos;
                                } else if (nextLine.Contains("<AltitudeMeters>"))
                                {
                                    freshTrackPoint.Altitude = float.Parse(nextLine.Split('<', '>')[2]);
                                }
                                setOfTrackPoints.Add(freshTrackPoint);
                                nextLine = streamReader.ReadLine();
                            }
                        }
                    }
                }
            }
            using (var writer = new StreamWriter(outputPath,false))
            {
                foreach(var item in setOfTrackPoints)
                {
                    writer.WriteLine(item.ToCsvRow());
                }
            }
        }
    }

    class Trackpoint
    {
        public DateTime Time { get; set; }
        public Position Position { get; set; }
        public float Altitude { get; set; }
        public String ToCsvRow()
        {
            return $"{Time},{Position.Latitude},{Position.Longitude},{Altitude}";
        }
    }
    class Position
    {
        public float Latitude { get; set; } 
        public float Longitude { get; set; }
    }
}
