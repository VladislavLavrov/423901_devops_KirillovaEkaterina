using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication5_Calculator.Models;

namespace WebApplication5_Calculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new 5_CalculatorModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Calculate(5_CalculatorModel model)
        {
            try
            {
                switch (model.Operation)
                {
                    case "+":
                        model.Result = model.FirstNumber + model.SecondNumber;
                        break;
                    case "-":
                        model.Result = model.FirstNumber - model.SecondNumber;
                        break;
                    case "*":
                        model.Result = model.FirstNumber * model.SecondNumber;
                        break;
                    case "/":
                        if (model.SecondNumber == 0)
                        {
                            model.ErrorMessage = "Деление на ноль невозможно!";
                            model.Result = 0;
                        }
                        else
                        {
                            model.Result = model.FirstNumber / model.SecondNumber;
                        }
                        break;
                    default:
                        model.ErrorMessage = "Неизвестная операция!";
                        model.Result = 0;
                        break;
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Ошибка: {ex.Message}";
                model.Result = 0;
            }

            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
