using System.Diagnostics;
using _5_Calculator.Models;
using _5_Calculator.Data;
using _5_Calculator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using System.Text.Json;


namespace _5_Calculator.Controllers
{
    public class HomeController : Controller
    {
       
        private CalculatorContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly KafkaProducerService<Null, string> _producer;
        public HomeController(CalculatorContext context, ILogger<HomeController> logger, KafkaProducerService<Null, string> producer)
        {
            _context = context;
            _logger = logger;
           
            _producer = producer;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculator(double Number1, double Number2, Operation operation)
        {
            var dataInputVariant = new DataInputVariant
            {
                Operand_1 = Number1,
                Operand_2 = Number2,
                Type_operation = operation,
            };
            await SendDataToKafka(dataInputVariant);

            return RedirectToAction(nameof(Calculator));

        }
        public IActionResult Callback([FromBody] DataInputVariant inputData)
        {
            SaveDataAndResult(inputData);
            return Ok();
        }
        private DataInputVariant SaveDataAndResult(DataInputVariant inputData)
        {
            _context.DataInputVariants.Add(inputData);
            _context.SaveChanges();
            return inputData;
        }
        private async Task SendDataToKafka(DataInputVariant dataInputVariant)

        {

            var json = JsonSerializer.Serialize(dataInputVariant);

            await _producer.ProduceAsync("5_Calculator", new Message<Null, string>

            { Value = json });

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
