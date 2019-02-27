using System;
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
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

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
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            var adoptionPending = db.Adoptions.Where(pd => pd.ApprovalStatus == pendingAdoption).ToList();
            return adoptionPending;
            //throw new NotImplementedException();
        }

        public static Employee RunEmployeeQueries(Employee employee, string update)
        {
            Employee employeeQueries = db.Employees.Where(eq => eq.EmployeeId == employee.EmployeeId).Single();
            return employeeQueries;
            //throw new NotImplementedException();
        }
                
        public static List<Animal> SearchForAnimalByMultipleTraits(Animal animal)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Console.WriteLine("What trait would you like to search for your animal by?");
            Console.WriteLine("Enter [1] to search by the name of the animal.");
            Console.WriteLine("Enter [2] to search by the demeanor of the animal.");
            Console.WriteLine("Enter [3] to search by the gender of the animal.");
            Console.WriteLine("Enter [4] to search by the category of the animal.");
            Console.WriteLine("Enter [5] to search for a pet friendly animal.");
            Console.WriteLine("Enter [6] to search for a kid friendly animal.");

            List<string> userChoice = Console.ReadLine().Split(' ').ToList();
            var multipleTraits = db.Animals.Where(mt => mt.Name == animal.Name && mt.Demeanor == animal.Demeanor && mt.Gender == animal.Gender && mt.Category == animal.Category && mt.PetFriendly == animal.PetFriendly && mt.KidFriendly == animal.KidFriendly).ToList();
            //List<Animal> animalsFound = new List<Animal>();
            foreach(string u in userChoice)
            {
                int searchCriteria = int.Parse(u);

                switch (searchCriteria)
                {
                    case 1:
                        Console.WriteLine("Enter the name of the animal you are searching for: ");
                        var animalNameChoice = Console.ReadLine();

                        var refinedNameSearch = from animals in multipleTraits
                                                where animal.Name == animalNameChoice
                                                select animal;
                        multipleTraits = refinedNameSearch.ToList();
                        break;

                    case 2:
                        Console.WriteLine("Enter the demeanor of the animal you are searching for: ");
                        var animalDemeanorChoice = Console.ReadLine();

                        var refinedDemeanorSearch = from animals in multipleTraits
                                                where animal.Name == animalDemeanorChoice
                                                select animal;
                        multipleTraits = refinedDemeanorSearch.ToList();
                        break;

                    case 3:
                        Console.WriteLine("Enter the gender of the animal you are searching for: [M] or [F] ");
                        var animalGenderChoice = Console.ReadLine().ToUpper();

                        var refinedGenderSearch = from animals in multipleTraits
                                                where animal.Name == animalGenderChoice
                                                select animal;
                        multipleTraits = refinedGenderSearch.ToList();
                        break;

                    case 4:
                        Console.WriteLine("Enter the category of the animal you are searching for: ");
                        var animalCategoryChoice = Console.ReadLine();

                        var refinedCategorySearch = from animals in multipleTraits
                                                where animal.Name == animalCategoryChoice
                                                select animal;
                        multipleTraits = refinedCategorySearch.ToList();
                        break;

                    case 5:
                        Console.WriteLine("Are you looking for a pet friendly animal?: [Y] or [N] ");
                        bool? animalPetFriendlyChoice = (Console.ReadLine().ToUpper() == "Y");

                        var refinedPetFriendlySearch = from animals in multipleTraits
                                                       where animal.PetFriendly == animalPetFriendlyChoice
                                                       select animal;
                        multipleTraits = refinedPetFriendlySearch.ToList();
                        break;

                    case 6:
                        Console.WriteLine("Are you looking for a kid friendly animal?: [Y] or [N] ");
                        bool? animalKidFriendlyChoice = (Console.ReadLine().ToUpper() == "Y");

                        var refinedKidFriendlySearch = from animals in multipleTraits
                                                       where animal.KidFriendly == animalKidFriendlyChoice
                                                       select animal;
                        multipleTraits = refinedKidFriendlySearch.ToList();
                        break;

                    default:
                        Console.WriteLine("I'm sorry, that is an invalid response. Please select a valid trait to search for your animal by.");
                        //SearchForAnimalByMultipleTraits();
                        break;
                }
            }
            

            return multipleTraits;
        }

        public static Adoption UpdateAdoption(bool isAdopting, Adoption adoption)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

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
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Animal myAnimal = db.Animals.Where(s => s.AnimalId == id).Single();
            return myAnimal;
        }

        //static void RunEmployeeQueries(Employee employee, string v)
        //{
        //    throw new NotImplementedException();
        //}

        public static void Adopt(Animal animal, Client client)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Adoption adoptNew = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId).Single();
            adoptNew.ClientId = client.ClientId;
            //throw new NotImplementedException();
        }

        public static List<AnimalShot> GetShots(Animal animal)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();
            // getting rows from AnimalShots that pertain to our animal
            var newShots = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).ToList();

            // get the shots from db.Shots that have an ShotId contained in "newShots"
            return newShots;



            // .Contains example
            //List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            //List<int> oddNumbers = new List<int> { 1, 3, 5 };

            //var testQuery = numbers.Where(n => oddNumbers.Contains(n)).ToList();

            //return actualShots;

            //throw new NotImplementedException();
        }
        public static Room GetRoom(int animalId)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Room room = db.Rooms.Where(r=> r.AnimalId == animalId).Single();
            return room;
        }

        public static Shot UpdateShot(string booster, Animal animal)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Shot shotUpdate = db.Shots.Where(s => s.Name == booster).Single();
            return shotUpdate;
            //throw new NotImplementedException();
        }

        public static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            var animalUpdate = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
            

            //throw new NotImplementedException();
        }
 
        public static Animal RemoveAnimal(Animal animalId)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Animal animal = db.Animals.Where(a => a.AnimalId != animalId.AnimalId).Single();
            return animalId;
            //throw new NotImplementedException();
        }

        public static int GetCategoryId(Animal animalName)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            Category category = db.Categories.Where(c => c.Name == animalName.Name).Single();
            return category.CategoryId;
        }

        public static int GetDietPlanId(Animal animalDietPlan)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();

            DietPlan diet = db.DietPlans.Where(d => d.DietPlanId == animalDietPlan.DietPlanId).Single();
            //table, variable, = -----------------------------------our own name...of type----number of items
            return diet.DietPlanId;
        }

        public static int AddAnimal(Animal animalId)
        {
            HumaneSocietyDCCDataContext db = new HumaneSocietyDCCDataContext();
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