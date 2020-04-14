using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        public static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
            
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
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
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

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
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
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
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
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
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    var checkEmployeeNumber = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    if (checkEmployeeNumber == null)
                    {
                        var checkEmployeeEmail = db.Employees.Where(e => e.Email == employee.Email).FirstOrDefault();
                        if (checkEmployeeEmail == null)
                        {
                            db.Employees.InsertOnSubmit(employee);
                            db.SubmitChanges();
                        }
                        else
                        {
                            UserInterface.DisplayUserOptions("An employee already exists with that data.");
                        }
                    }
                    break;
                case "delete":
                    employee = db.Employees.Where(e => e.FirstName == employee.FirstName && e.LastName == employee.LastName && e.EmployeeNumber == employee.EmployeeNumber).SingleOrDefault();
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
            }
            //throw new NotImplementedException();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
                 db.Animals.InsertOnSubmit(animal);
                 db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal result = new Animal();
            result = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return result;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == animalId);
            foreach (KeyValuePair<int, string>update in updates)
            {
                switch (update)
                { 
                    
                
                }



            }

            
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            bool searchForTrait = true;
            var animals = db.Animals.ToList();
            while (searchForTrait == true)
            {
                List<string> options = new List<string>()
                { "Select Update:", "1. Category", "2. Name", "3. Age", "4. Demeanor", "5. Kid friendly", "6. Pet friendly", "7. Weight", "8. Finished", "You will be prompted again for any additional updates." };
                UserInterface.DisplayUserOptions(options);
                var userInput = UserInterface.GetIntegerData();
                switch (userInput)
                {
                    case 1:
                        UserInterface.DisplayUserOptions("Enter an animal category to search by:");
                        string searchCategory = UserInterface.GetUserInput();
                        animals = animals.Where(c => c.Category.Name == searchCategory).Select(c => c).ToList();
                        break;
                    case 2:
                        UserInterface.DisplayUserOptions("Enter an animal name to search by:");
                        string searchName = UserInterface.GetUserInput();
                        animals = animals.Where(n => n.Name == searchName).Select(n => n).ToList();
                        break;
                    case 3:
                        UserInterface.DisplayUserOptions("Enter an animal age to search by:");
                        int searchAge = UserInterface.GetIntegerData();
                        animals = animals.Where(a => a.Age == searchAge).Select(a => a).ToList();
                        break;
                    case 4:
                        UserInterface.DisplayUserOptions("Enter an animal demeanor to search by:");
                        string searchDemeanor = UserInterface.GetUserInput();
                        animals = animals.Where(d => d.Demeanor == searchDemeanor).Select(d => d).ToList();
                        break;
                    case 5:
                        bool? searchKidFriendly = UserInterface.GetBitData("Search if an animal is kid friendly:");
                        animals = animals.Where(k => k.KidFriendly == searchKidFriendly).Select(k => k).ToList();
                        break;
                    case 6:
                        bool? searchPetFriendly = UserInterface.GetBitData("Search if an animal is pet frirndly:");
                        animals = animals.Where(p => p.PetFriendly == searchPetFriendly).Select(p => p).ToList();
                        break;
                    case 7:
                        UserInterface.DisplayUserOptions("Enter an animal weight to search by:");
                        int searchWeight = UserInterface.GetIntegerData();
                        animals = animals.Where(w => w.Weight == searchWeight).Select(w => w).ToList();
                        break;
                    default:
                        Console.WriteLine("Please make a valid selection.");
                        break;
                }

            }
            //throw new NotImplementedException();
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            throw new NotImplementedException();
        }
        
        internal static Room GetRoom(int animalId)
        {
            throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}