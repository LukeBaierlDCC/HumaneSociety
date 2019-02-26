﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        private  static HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();
        internal static List<USState> GetStates()
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            HumaneSocietyDCCDataContext  db = new HumaneSocietyDCCDataContext();

            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            HumaneSocietyDCCDataContext  db = new HumaneSocietyDCCDataContext();

            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = zipCode;
                newAddress.USStateId = stateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            

            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = clientAddress.Zipcode;
                newAddress.USStateId = clientAddress.USStateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }

        public static List<Adoption> GetPendingAdoptions(string pendingAdoption)
        {
            var adoptionPending = db.Adoptions.Where(pd => pd.ApprovalStatus == pendingAdoption).ToList();
           return adoptionPending;
            //throw new NotImplementedException();
        }

        public static Employee RunEmployeeQueries(Employee employee, string update)
        {
            Employee employeeQueries = db.Employees.Where(eq => eq.EmployeeId == update).Single();
            return employeeQueries;
            //throw new NotImplementedException();
        }
                
        public static List<Animal> SearchForAnimalByMultipleTraits(Animal animal)
        {
            var multipleTraits = db.Animals.Where(mt => mt.Name == animal.Name && mt.Demeanor == animal.Demeanor && mt.Gender == animal.Gender && mt.Category == animal.Category).ToList() ;
            //List<Animal> animalsFound = new List<Animal>();

            return multipleTraits;
        }

        public static Adoption UpdateAdoption(bool isAdopting, Adoption adoption)
        {
            Adoption adoptionUpdate = db.Adoptions.Where(a => a.AdoptionId == adoption.AdoptionId).Single();
            //Adoption adoptionUpdate = db.Adoptions.Where(a => a.AdoptionId == adoption.AdoptionId).SingleOrDefault();
            //Adoption adoptionUpdate = db.Adoptions.Where(a => a.AdoptionId == adoption.AdoptionId).First();
            //Adoption adoptionUpdate = db.Adoptions.Where(a => a.AdoptionId == adoption.AdoptionId).FirstOrDefault();

            if (isAdopting == true)
            {
                adoptionUpdate.ApprovalStatus = "adopt";

            }
            else
            {
                adoptionUpdate.ApprovalStatus = "NO";

            }

            return adoptionUpdate;
            //throw new NotImplementedException();
        }

        public static Animal GetAnimalByID(int id)
        {
            Animal myAnimal = db.Animals.Where(s => s.AnimalId == id).Single();
            return myAnimal;
        }

        //static void RunEmployeeQueries(Employee employee, string v)
        //{
        //    throw new NotImplementedException();
        //}

        public static void Adopt(Animal animal, Client client)
        {
            Adoption adoptNew = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId).Single();
            adoptNew.ClientId = client.ClientId;
            //throw new NotImplementedException();
        }

        public static Shot GetShots(Shot shot)
        {
            var newShots = db.Shots.Where(s => s.ShotId == shot.ShotId).Single();
            return newShots;

            //throw new NotImplementedException();
        }
        public static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(r=> r.AnimalId == animalId).Single();
            return room;
        }

        public static Shot UpdateShot(Shot shot)
        {
            Shot shotUpdate = db.Shots.Where(s => s.ShotId == shot.ShotId).Single();
            return shotUpdate;
            //throw new NotImplementedException();
        }

        public static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)
        {
            var animalUpdate = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
            

            //throw new NotImplementedException();
        }
 
        public static Animal RemoveAnimal(Animal animalId)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId != animalId.AnimalId).Single();
            return animalId;
            //throw new NotImplementedException();
        }

        public static int GetCategoryId(Animal animalName)
        {
            Category category = db.Categories.Where(c => c.Name == animalName.Name).Single();
            return category.CategoryId;
        }

        public static int GetDietPlanId(Animal animalDietPlan)
        {
            DietPlan diet = db.DietPlans.Where(d => d.DietPlanId == animalDietPlan.DietPlanId).Single();
            //table, variable, = -----------------------------------our own name...of type----number of items
            return diet.DietPlanId;
        }

        public static int AddAnimal(Animal animalId)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == animalId.AnimalId).Single();
            return animal.AnimalId;
        }
        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDCCDataContext  db = new HumaneSocietyDCCDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if(employeeFromDb == null)
            {
                throw new NullReferenceException();            
            }
            else
            {
                return employeeFromDb;
            }            
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            HumaneSocietyDCCDataContext  db = new HumaneSocietyDCCDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDCCDataContext  db = new HumaneSocietyDCCDataContext();

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDCCDataContext  db = new HumaneSocietyDCCDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }
    }
}