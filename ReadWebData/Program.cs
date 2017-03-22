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
        private static string file_name = "sportinformation.xml";
        private static string event_file_name = "eventinformation.xml";
        private static int year = 0;
        private static int month = 0;
        //private static int start = 165000;
        private static int start = 170950;
        //private static int end = 205000;
        private static int end = 185000;
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
            /*
            readPrintLocations();
            readDiningLocations();
            readFitnessLocations();
            readLibraryLocations();
            readComputerLocations();
                       
            for (int i = start; i < end; i++)
            {
                url = "https://events.pitt.edu/MasterCalendar/EventDetails.aspx?EventDetailId=" + i;
                try
                {
                    client.DownloadFile(url, event_file_name);
                    readEventFile();
                }
                catch(Exception e)
                {

                }
            }
            
    
            CampusEventCollection col = CampusEventAPI.GetAllEvents();
            foreach (CampusEvent Event in col.Events)
            {
                updateEventDate(Event);
            }
            */
        }

        private static void updateEventDate(CampusEvent Event)
        {
            if (Event.Date.Hour == 0)
            {
                string time = Event.Time.Split('-')[0].Trim();
                if (time.Contains("AM"))
                {
                    int hour = Int32.Parse(time.Split('A')[0].Trim().Split(':')[0]);
                    int minute = Int32.Parse(time.Split('A')[0].Trim().Split(':')[1]);
                    if (hour == 12)
                    {
                        hour = 0;
                    }
                    Event.Date = Event.Date.AddHours(hour);
                    Event.Date = Event.Date.AddMinutes(minute);
                    CampusEventAPI.UpdateEventDate(Event);
                }
                else if (time.Contains("PM"))
                {
                    int hour = Int32.Parse(time.Split('P')[0].Trim().Split(':')[0]);
                    int minute = Int32.Parse(time.Split('P')[0].Trim().Split(':')[1]);
                    if (hour != 12)
                    {
                        hour = hour + 12;
                    }
                    Event.Date = Event.Date.AddHours(hour);
                    Event.Date = Event.Date.AddMinutes(minute);
                    CampusEventAPI.UpdateEventDate(Event);
                }
            }
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

        private static void readEventFile()
        {
            //List<CampusEvent> events = new List<CampusEvent>();

            if (File.Exists(event_file_name))
            {
                string[] lines = File.ReadAllLines(event_file_name);
                CampusEvent Event = null;
                for (int i = 0; i < lines.Length; i++)
                {
                    
                    if(lines[i].Contains("id=\"EventTitle"))
                    {
                        Event = new CampusEvent();
                        Event.Title = lines[i].Split('>')[1].Split('<')[0].Trim();
                    }

                    if(lines[i+2].Contains("class=\"info-date\""))
                    {
                        int month = 0;
                        int day;
                        int year;
                        string temp = lines[i+2].Split('>')[1].Split('<')[0];
                        string strmonth = temp.Split(' ')[1];
                        switch (strmonth)
                        {
                            case "January":
                                month = 1;
                                break;
                            case "February":
                                month = 2;
                                break;
                            case "March":
                                month = 3;
                                break;
                            case "April":
                                month = 4;
                                break;
                            case "May":
                                month = 5;
                                break;
                            case "June":
                                month = 6;
                                break;
                            case "July":
                                month = 7;
                                break;
                            case "August":
                                month = 8;
                                break;
                            case "September":
                                month = 9;
                                break;
                            case "October":
                                month = 10;
                                break;
                            case "November":
                                month = 11;
                                break;
                            case "December":
                                month = 12;
                                break;
                        }
                        Int32.TryParse(lines[i+2].Split('>')[1].Split(' ')[2].Split(',')[0],out day);
                        Int32.TryParse(lines[i+2].Split('>')[1].Split('<')[0].Split(' ')[3],out year);
                        DateTime date = new DateTime(year, month, day);
                        if(date < DateTime.Now)
                        {
                            break;
                        }
                        Event.Date = date;
                    }
                    if (lines[i+2].Contains("class=\"info-time fl\""))
                    {
                        Event.Time = lines[i + 2].Split('>')[3].Split('<')[0];
                    }
                    if (lines[i + 3].Contains("class=\"info-location\""))
                    {
                        Event.Location = lines[i + 3].Split('>')[2].Split('<')[0];
                        if(Event.Location.Trim() == "")
                        {
                            Event.Location = lines[i + 3].Split('>')[1].Split('<')[0];
                        }
                    }
                    if (lines[i + 5].Contains("EventType"))
                    {
                        if (Event != null)
                        {
                            Event.Type = new CampusEventType(lines[i + 5].Split('>')[4].Split('<')[0]);
                        }
                    }
                    if (lines[i + 7].Contains("id=\"Department") && !lines[i + 7].Contains("id=\"DepartmentLabel\""))
                    {
                        if (Event != null)
                        {
                            Event.Organization = lines[i + 7].Split('>')[2].Split('<')[0];
                        }
                    }
                    else if(lines[i + 9].Contains("id=\"Department"))
                    {
                        if (Event != null)
                        {
                            Event.Organization = lines[i + 9].Split('>')[2].Split('<')[0];
                        }
                    }
                    if (Event != null) 
                    {
                        if (Event.Date == null)
                        {
                            Event.Date = new DateTime();
                        }
                        if (Event.Location != null)
                        {
                            Event.Location = Event.Location.Replace("&amp;", "&");
                        }
                        if (Event.Title != null)
                        {
                            Event.Title = Event.Title.Replace("&amp;", "&");
                        }
                        if (Event.Organization != null)
                        {
                            Event.Organization = Event.Organization.Replace("&amp;", "&");
                        }
                        if (Event.Date >= DateTime.Now)
                        {
                            CampusEventAPI.AddEvent(Event);
                            break;
                        }
                    }
                }
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
