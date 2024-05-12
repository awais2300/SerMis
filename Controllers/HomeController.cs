using Microsoft.AspNetCore.Mvc;
using SerMis.Data;
using SerMis.Models;
using System.Diagnostics;

namespace SerMis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new PeopleModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult City()
        {
            var City = HttpContext.Session.GetString("City");
            var Name = HttpContext.Session.GetString("Name");
            ViewBag.City = City; 
            ViewBag.Name = Name;
            //ViewBag.Url = "/Home/Age";
            return View();
        }

        public IActionResult Age()
        {
            var Age = HttpContext.Session.GetString("Age");
            var Name = HttpContext.Session.GetString("Name");
            ViewBag.Age = Age;
            ViewBag.Name = Name;
            return View();
        }

        public IActionResult PeopleDetail()
        {
            var Name = HttpContext.Session.GetString("Name"); //Getting name value from session 
            var City = HttpContext.Session.GetString("City"); //Getting City value from session 
            var Age = HttpContext.Session.GetString("Age"); //Getting Age value from session 
            ViewBag.Name = Name; //Setting Name value to pass to the view
            ViewBag.City = City; //Setting City value to pass to the view
            ViewBag.Age = Age; //Setting Age value to pass to the view
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult Index(PeopleModel model)
        {
            string name = model.Name; //Getting Name from form submit textbox field

            if(name != null)
            {
                var valuesFromDatabase = _context.People.FirstOrDefault(x => x.Name == name);
                if (valuesFromDatabase != null) //If Name is available in the database
                {
                    HttpContext.Session.SetString("Name", name); //Setting the name to a session variable 
                    HttpContext.Session.SetString("Age", valuesFromDatabase.Age.ToString()); //Setting the Age to a session variable 
                    if (valuesFromDatabase.City == null)
                        HttpContext.Session.SetString("City", ""); 
                    else
                        HttpContext.Session.SetString("City", valuesFromDatabase.City.ToString()); //Setting the City to a session variable

                    if (valuesFromDatabase.Flag != 1 || valuesFromDatabase.Flag == null) //If the tab is already open, then do not open the tab again
                    {
                        valuesFromDatabase.Flag = 1; //Setting the flag to one 
                        _context.SaveChanges();

                        ViewBag.Url = "/Home/PeopleDetail"; //Url to open the new tab
                    }
                }
                else //If the Name doesnot exist in the Database
                {
                    HttpContext.Session.SetString("Name", name); //Setting the Name to a session variable
                    HttpContext.Session.SetString("Age", ""); //Setting the Age to blank
                    HttpContext.Session.SetString("City", ""); //Setting the City to blank

                    #region Insert Into DB
                    var entity = new PeopleModel { Name = name, Flag = 1 }; //If the name doesnot exisit in the DB, insert it
                    _context.People.Add(entity);
                    _context.SaveChanges();
                    #endregion

                    ViewBag.Url = "/Home/PeopleDetail";
                }
            }
            
           
            return View();
        }
        #region DataBase Reading and Righting Region
        [HttpPost]
        //Funtion to update city value in the database
        public IActionResult SaveCityData(string Name, string City, DateTime Updt_dte) 
        {
            var NameFromDataBase = _context.People.FirstOrDefault(x => x.Name == Name);
            if (NameFromDataBase != null) //Update existing value
            {
                NameFromDataBase.City = City;
                _context.SaveChanges();
            }
            else //Insert New Value
            {
                var entity = new PeopleModel { Name = Name, City = City };
                _context.People.Add(entity);
                _context.SaveChanges();
            }
            
            return Json(new { success = true });
        }

        [HttpPost]
        //Funtion to update Age value in the database
        public IActionResult SaveAgeData(string Name, string Age, DateTime Updt_dte)
        {
            var NameFromDataBase = _context.People.FirstOrDefault(x => x.Name == Name);
            if (NameFromDataBase != null) //Update existing value
            {
                NameFromDataBase.Age = Convert.ToInt32(Age);
                _context.SaveChanges();
            }
            else //Insert New Value
            {
                var entity = new PeopleModel { Name = Name, Age = Convert.ToInt32(Age) };
                _context.People.Add(entity);
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        //Funtion to update Flag value in the database
        public IActionResult UpdateDatabaseOnClosingTab(string Name, string TabName)
        {
            var NameFromDataBase = _context.People.FirstOrDefault(x => x.Name == Name);
            if (NameFromDataBase != null) //Update existing value
            {
                NameFromDataBase.Flag = NameFromDataBase.Flag - 1;
                _context.SaveChanges();
            }

            return Ok();
        }
        #endregion
    }
}