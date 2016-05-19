using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ChocAn.Models;

// the purpose of this class is to randomly initialize the database with info for testing

namespace ChocAn.EfData
{
    public class ChocAnContentInitializer : DropCreateDatabaseIfModelChanges<ChocAnDb>
    {
        protected override void Seed(ChocAnDb context)
        {
            // create a list of new services to add to the db
            List<Service> services = new List<Service> {
                new Service { ServiceName = "Swedish Chocolate Massage", Enabled = true, Fee = 60 },
                new Service { ServiceName = "ChocoRush Massage", Enabled = true, Fee = 40 },
                new Service { ServiceName = "Hot Cocoa Massage", Enabled = true, Fee = 90 },
                new Service { ServiceName = "Chocolate Wrap", Enabled = true, Fee = 85 },
                new Service { ServiceName = "Chocolate Salted Caramel Body Scrub", Enabled = true, Fee = 85 },
                new Service { ServiceName = "Chocolate Bath", Enabled = true, Fee = 120 },
                new Service { ServiceName = "Aroma Therapy", Enabled = true, Fee = 35 },
                new Service { ServiceName = "Acupuncture", Enabled = true, Fee = 80 },
                new Service { ServiceName = "Meditation Room", Enabled = true, Fee = 20 },
                new Service { ServiceName = "Addiction Counseling", Enabled = true, Fee = 75 },
                new Service { ServiceName = "Withdrawal Assistance", Enabled = true, Fee = 30 },
                new Service { ServiceName = "Solitary Confinement", Enabled = true, Fee = 98 },
                new Service { ServiceName = "Room of Silence", Enabled = true, Fee = 74 },
                new Service { ServiceName = "Zero Calorie Enforcement", Enabled = true, Fee = 50 }
            };
            // for each one in the list, use the Context and add them to the db and save the changes
            services.ForEach(service => context.Services.Add(service));
            context.SaveChanges();

            // set up arrays of first names, last names, cities, and states for the generator to randomly 
            // pick from when initializing the db
            string[] firstNames = {
                "Adam", "Jenna", "Betty", "Carl", "Diane", "Erik", "Frank", "Gin", "Hilda", "July", "Kourtney",
                "Lindsay", "Mary", "Nick", "Oscar", "Alexis", "Asa", "Tory", "Bree", "Stephanie" };

            string[] lastNames = {
                "Grib", "Hilster", "Haze", "Jetson", "Simpson", "Smith", "Groupon", "Westchester",
                "Lee", "Miltner", "Lee", "Kenya", "Texas", "Akira", "Novak", "Reed", "Lane", "Olson" };

            string[] cities = {
                "Atlanta", "Chicago", "New York", "Kansas City", "Jasper", "Indianapolis",
                "Austin", "San Antonio", "Dallas", "Sandpoint", "Gresham", "Portland", "Medford",
                "Eugine", "Tillamook", "Huntingburg", "Boston", "Jacksonville", "Memphis",
                "Petopia", "Springfield", "South Park", "Columbus", "Santa Claus", "London"};

            string[] states = {
                "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA", "HI", "IA", "ID",
                "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC",
                "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD",
                "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY" };

            Random rand = new Random();

            // calculate the max index of each array
            int firstNameIndex = firstNames.Length - 1;
            int lastNameIndex = lastNames.Length - 1;
            int cityIndex = cities.Length - 1;

            List<UserProfile> profiles = new List<UserProfile>();

            // in each iteration, pick some random content and make a new profile name with it
            for (int i = 0; i < 250; ++i)
            {
                // initialize the first four profiles to dedicated user roles for testing 
                // purposes, otherwise randomize it
                int role = 1;
                if (i < 4) role += i;
                else role = rand.Next(1, 5);

                UserProfile u = new UserProfile
                {
                    FirstName = firstNames[rand.Next(0, firstNameIndex)],
                    LastName = lastNames[rand.Next(0, lastNameIndex)],
                    Address = new Address
                    {
                        AddressLineOne = rand.Next(1000, 9999) + " Street",
                        City = cities[rand.Next(0, cityIndex)],
                        State = states[rand.Next(0, 49)],
                        Zip = rand.Next(10000, 99999)
                    },                         
                    RoleId = role
                };
                profiles.Add(u);
            }
            // add each profile from the list to the db and save the changes
            profiles.ForEach(profile => context.UserProfiles.Add(profile));
            context.SaveChanges();

            // clear the services and profiles lists
            services.Clear();
            profiles.Clear();
            GC.Collect();

            // create an array of all the members, providers, and services from the db. Then 
            // calculate the proper index size of each
            UserProfile[] members = context.UserProfiles.Where(m => m.RoleId == 1).ToArray();
            int memberIndex = members.Length - 1;

            UserProfile[] providers = context.UserProfiles.Where(m => m.RoleId == 2).ToArray();
            int providerIndex = providers.Length - 1;

            Service[] serviceArray = context.Services.ToArray();
            int serviceIndex = serviceArray.Length - 1;

            // setup a list of treatment records to push to the db
            List<TreatmentRecord> records = new List<TreatmentRecord>();
            for (int i = 0; i < 500; ++i)
            {
                TreatmentRecord record = new TreatmentRecord()
                {
                    EntryDate = DateTime.Now.AddDays(rand.Next(1, 45) * (-1)),
                    Member = members[rand.Next(0, memberIndex)],
                    Provider = providers[rand.Next(0, providerIndex)],
                    Service = serviceArray[rand.Next(0, serviceIndex)],
                    TreatmentDate = DateTime.Now.AddDays(rand.Next(1, 45) * (-1)),
                    Comments = "Some random notes and stuff"
                };
                records.Add(record);
            }
            records.ForEach(record => context.TreatmentRecords.Add(record));
            context.SaveChanges();
        }
    }
}