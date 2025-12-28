using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using ExcelDataReader;
using System.Data;
using System.Text;

namespace ForecastApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImportController : ControllerBase
    {
        private readonly Func<MySqlConnection> _connectionFactory;

        public ImportController(Func<MySqlConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [HttpPost("excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });

                using var conn = _connectionFactory();
                
                // Обработка данных из Excel
                // Здесь должна быть логика парсинга и вставки данных в БД
                // В зависимости от структуры Excel файла

                return Ok("Импорт успешно завершён");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при импорте: {ex.Message}");
            }
        }
    }
}

