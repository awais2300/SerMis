using Microsoft.AspNetCore.Mvc;
using SerMis.Data;
using SerMis.Models;
using System.Diagnostics;

namespace SerMis.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

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
            ViewBag.Url = "/Home/Age";
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //// Example Controller Action method
        //[HttpPost]
        //public IActionResult SubmitForm(PeopleModel model)
        //{
        //    // Process the submitted data
        //    string textValue = model.Name;

        //    // Redirect to another action or return a view
        //    return RedirectToAction("ActionName", "ControllerName");
        //}

        [HttpPost]
        public IActionResult Index(PeopleModel model)
        {
            string name = model.Name;

            if(name != null)
            {
                var valuesFromDatabase = _context.People.FirstOrDefault(x => x.Name == name);
                if (valuesFromDatabase != null)
                {
                    HttpContext.Session.SetString("Name", name);
                    HttpContext.Session.SetString("Age", valuesFromDatabase.Age.ToString());
                    if (valuesFromDatabase.City == null)
                        HttpContext.Session.SetString("City", "");
                    else
                        HttpContext.Session.SetString("City", valuesFromDatabase.City.ToString());

                    if (valuesFromDatabase.Flag != 2 || valuesFromDatabase.Flag == null)
                    {
                        valuesFromDatabase.Flag = 2;
                        _context.SaveChanges();

                        ViewBag.Url = "/Home/City";
                    }
                }
                else
                {
                    HttpContext.Session.SetString("Name", name);
                    HttpContext.Session.SetString("Age", "");
                    HttpContext.Session.SetString("City", "");

                    #region Insert Into DB
                    var entity = new PeopleModel { Name = name, Flag = 2 };
                    _context.People.Add(entity);
                    _context.SaveChanges();
                    #endregion

                    ViewBag.Url = "/Home/City";
                }
            }
            
           
            return View();
        }

        [HttpPost]
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
    }
}