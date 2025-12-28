using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Dapper;
using ExcelDataReader;
using System.Data;
using System.Text;
using System.Linq;

namespace ForecastApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImportController : ControllerBase
    {
        private readonly Func<SqliteConnection> _connectionFactory;

        public ImportController(Func<SqliteConnection> connectionFactory)
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

                // Получаем первую таблицу из Excel
                if (dataSet.Tables.Count == 0)
                {
                    return BadRequest("Файл не содержит данных");
                }

                var table = dataSet.Tables[0];
                
                // Преобразуем DataTable в список словарей для передачи на фронтенд
                var result = new List<Dictionary<string, object>>();
                
                foreach (DataRow row in table.Rows)
                {
                    var rowDict = new Dictionary<string, object>();
                    foreach (DataColumn column in table.Columns)
                    {
                        var value = row[column];
                        rowDict[column.ColumnName] = value == DBNull.Value ? null : value;
                    }
                    result.Add(rowDict);
                }

                // Получаем названия колонок
                var columns = table.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();

                using var conn = _connectionFactory();
                conn.Open();
                
                // Обработка данных из Excel
                // Здесь должна быть логика парсинга и вставки данных в БД
                // В зависимости от структуры Excel файла

                return Ok(new
                {
                    message = "Импорт успешно завершён",
                    columns = columns,
                    data = result,
                    totalRows = result.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при импорте: {ex.Message}");
            }
        }
    }
}

