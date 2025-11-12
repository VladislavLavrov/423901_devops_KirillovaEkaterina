using System.Diagnostics;
using _5_Calculator.Models;
using _5_Calculator.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _5_Calculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CalculatorContext _context;

        public HomeController(ILogger<HomeController> logger, CalculatorContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Calculator(double Number1, double Number2, string Operation)
        {
            var model = new CalculatorModel
            {
                Number1 = Number1,
                Number2 = Number2,
                Operation = Operation
            };

            _logger.LogInformation($"Received: {Number1} {Operation} {Number2}");

            try
            {
                model.Result = Operation switch
                {
                    "+" => Number1 + Number2,
                    "-" => Number1 - Number2,
                    "*" => Number1 * Number2,
                    "/" when Number2 != 0 => Number1 / Number2,
                    "/" when Number2 == 0 => throw new DivideByZeroException(),
                    _ => throw new ArgumentException("Неверная операция")
                };

                _logger.LogInformation($"Result: {model.Result}");
            }
            catch (DivideByZeroException)
            {
                model.ErrorMessage = "Невозможно";
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Ошибка: {ex.Message}";
            }

            var dataInputVariant = new DataInputVariant
            {
                Operand_1 = Number1.ToString(),
                Operand_2 = Number2.ToString(),
                Type_operation = Operation
            };

            _context.DataInputVariants.Add(dataInputVariant);
            _context.SaveChanges();

            // Если это AJAX-запрос, возвращаем JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { 
                    result = model.Result, 
                    errorMessage = model.ErrorMessage 
                });
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
