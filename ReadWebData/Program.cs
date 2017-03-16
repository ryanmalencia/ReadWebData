using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using DataTypes;
using WebAPIClient.APICalls;

namespace ReadWebData
{
    class Program
    {
        private static string file_name = "information.xml";
        private static int year = 0;
        private static int month = 0;
        static void Main(string[] args)
        {
            month = Int32.Parse(args[0]);
            year = Int32.Parse(args[1]);
            string type = args[2];

            WebClient client = new WebClient();

            string url = "";

            switch (type)
            {
                case "sports":
                    url = "http://www.pittsburghpanthers.com/calendar/events/" + (year - 1) + "_" + year + "0" + month + ".html";
                    client.DownloadFile(url, file_name);
                    readSportsFile(file_name);
                    break;
                case "cityconcerts":
                    url = "http://pittsburgh.eventful.com/events/categories/music#!category=music&sort=date&venue=106517%2C151745%2C492043%2C10696061&page_number=" + month;
                    client.DownloadFile(url, file_name);
                    break;
            }

            readPrintLocations();
            readDiningLocations();
            readFitnessLocations();
            readLibraryLocations();
            readComputerLocations();
        }

        private static void readPrintLocations()
        {
            string[] locations = File.ReadAllLines(@"C:\AppFiles\printcoord.txt");

            for (int i = 0; i < locations.Length; i++)
            {
                PrintLocation loc = new PrintLocation();
                string[] temp = locations[i].Split(':');
                loc.Name = temp[0];
                loc.Latitude = double.Parse(temp[1]);
                loc.Longitude = double.Parse(temp[2]);
                LocationAPI.AddPrintLocation(loc);
            }
        }

        private static void readDiningLocations()
        {
            string[] locations = File.ReadAllLines(@"C:\AppFiles\foodcoords.txt");

            for (int i = 0; i < locations.Length; i++)
            {
                DiningLocation loc = new DiningLocation();
                string[] temp = locations[i].Split(':');
                loc.Name = temp[0];
                loc.SubTitle = temp[1];
                loc.Latitude = double.Parse(temp[2]);
                loc.Longitude = double.Parse(temp[3]);
                LocationAPI.AddDiningLocation(loc);
            }
        }

        private static void readFitnessLocations()
        {
            string[] locations = File.ReadAllLines(@"C:\AppFiles\fitnesscoords.txt");

            for (int i = 0; i < locations.Length; i++)
            {
                FitnessLocation loc = new FitnessLocation();
                string[] temp = locations[i].Split(';');
                loc.Name = temp[0];
                loc.SubTitle = temp[1];
                loc.Latitude = double.Parse(temp[2]);
                loc.Longitude = double.Parse(temp[3]);
                LocationAPI.AddFitnessLocation(loc);
            }
        }

        private static void readLibraryLocations()
        {
            string[] locations = File.ReadAllLines(@"C:\AppFiles\librarycoords.txt");

            for (int i = 0; i < locations.Length; i++)
            {
                LibraryLocation loc = new LibraryLocation();
                string[] temp = locations[i].Split(';');
                loc.Name = temp[0];
                loc.SubTitle = temp[1];
                loc.Latitude = double.Parse(temp[2]);
                loc.Longitude = double.Parse(temp[3]);
                LocationAPI.AddLibraryLocation(loc);
            }
        }

        private static void readComputerLocations()
        {
            string[] locations = File.ReadAllLines(@"C:\AppFiles\computercoords.txt");

            for (int i = 0; i < locations.Length; i++)
            {
                ComputerLocation loc = new ComputerLocation();
                string[] temp = locations[i].Split(';');
                loc.Name = temp[0];
                loc.SubTitle = temp[1];
                loc.Latitude = double.Parse(temp[2]);
                loc.Longitude = double.Parse(temp[3]);
                LocationAPI.AddComputerLocation(loc);
            }
        }

        private static void readSportsFile(string filename)
        {
            int day = 1;
            List<string> events = new List<string>();
            List<SportEvent> sportevents = new List<SportEvent>();
            if (File.Exists(filename))
            {

                string[] lines = File.ReadAllLines(filename);
                for (int i = 0; i < lines.Length; i++)
                {
                    string eventtype = "";
                    string school = "";
                    string result = "";
                    string location = "";
                    string tvhosting = "";
                    string imageloc = "";
                    bool home = false;
                    int hour = 0;
                    int minute = 0;
                    int second = 0;

                    if (lines[i].Contains("class=\"daynumber\""))
                    {
                        bool valid = false;
                        valid = Int32.TryParse(lines[i].Split('>')[1].Split('<')[0], out day);
                        if (!valid)
                        {
                            day = 1;
                        }
                    }

                    if (lines[i].Contains("=\"LiveEvent"))
                    {
                        eventtype = lines[i].Split('>')[1].Split('<')[0];
                        if (lines[i + 1].Contains("calopplogo"))
                        {
                            school = lines[i + 1].Split('>')[1].Split('<')[0];
                            if (lines[i + 1].Contains("img"))
                            {
                                imageloc = lines[i + 1].Split('=')[1].Split('\"')[1];
                            }
                            if (lines[i + 2].Contains("href="))
                            {
                                result = lines[i + 2].Split('>')[2].Split('<')[0];
                                if (lines[i + 3].Contains("<br>"))
                                {
                                    location = lines[i + 3].Split('>')[1];
                                    if (lines[i + 4].Contains("<br>"))
                                    {
                                        tvhosting = lines[i + 4].Split('>')[1];
                                    }
                                }
                            }
                            else if (lines[i + 2].Contains("<br>") && !lines[i + 2].Contains("href"))
                            {
                                result = lines[i + 2].Split('>')[1];
                                if (lines[i + 3].Contains("<br>"))
                                {
                                    location = lines[i + 3].Split('>')[1];
                                    if (lines[i + 4].Contains("<br>"))
                                    {
                                        tvhosting = lines[i + 4].Split('>')[1];
                                    }
                                }
                            }
                        }
                    }
                    eventtype = eventtype.Replace("&amp;", "&");
                    result = result.Replace("&amp;", "&");
                    school = school.Replace("&amp;", "&");
                    location = location.Replace("&amp;", "&");
                    string homeloc = location.ToLower().Trim();
                    if(homeloc == "pittsburgh, pa" || homeloc == "petersen events center" || homeloc == "pittsburgh, pa." || homeloc == "petersen event center" || homeloc == "charles l. cost field" || homeloc == "vartabedian field")
                    {
                        home = true;
                    }
                    tvhosting = tvhosting.Replace("&amp;", "&");
                    string newevent = eventtype + "!" + school + "!" + result + "!" + location + "!" + tvhosting;
                    newevent = newevent.Replace("&amp;", "&");
                    if(result.Trim() != "")
                    {
                        if(result.Contains(":"))
                        {
                            if (result.Contains("AM"))
                            {
                                hour = Int32.Parse(result.Split(':')[0]);
                            }
                            else
                            {
                                hour = Int32.Parse(result.Split(':')[0]);
                                if(hour != 12)
                                {
                                    hour = hour + 12;
                                }
                            }
                        }
                    }
                    if (newevent.Trim().Length > 4)
                    {
                        SportEvent current = new SportEvent(new DateTime(year, month, day, hour, minute, second), new Sport(eventtype.Trim()), school, result, location, tvhosting,imageloc, home);
                        if (!sportevents.Contains(current) && tvhosting.Trim().Length > 0)
                        {
                            SportEventAPI.AddSportEvent(current);
                        }
                        //SportEventAPI.AddSportEvent(current);
                        events.Add(newevent);
                        sportevents.Add(current);
                    }

                }
            }
            File.WriteAllLines("data.txt", events.ToArray()); 
        }
    }
}
