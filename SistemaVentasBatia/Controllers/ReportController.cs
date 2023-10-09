using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        //private readonly IHttpClientFactory _httpClientFactory;

        //public ReportController(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClientFactory = httpClientFactory;
        //}

        [HttpPost("[action]/{tipo}")]
        public IActionResult DescargarReporteCotizacion([FromBody] int idCotizacion, int tipo = 0)
        {
            if (tipo == 1)
            {
                try
                {
                    var url = ("http://192.168.2.4/Reporte?%2freportecotizacion&rs:Format=PDF&idCotizacion=" + idCotizacion.ToString());
                    WebClient wc = new WebClient
                    {
                        Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                    };
                    byte[] myDataBuffer = wc.DownloadData(url.ToString());

                    return new FileContentResult(myDataBuffer, "application/pdf")
                    {
                        FileDownloadName = "PropuestaTecnica.pdf"
                    };

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                    return StatusCode(500, "Error al obtener el archivo PDF");
                }
            }
            else
            {
                try
                {
                    var url = ("http://192.168.2.4/Reporte?%2freportecotizacion2&rs:Format=PDF&idCotizacion=" + idCotizacion.ToString());
                    WebClient wc = new WebClient
                    {
                        Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                    };
                    byte[] myDataBuffer = wc.DownloadData(url.ToString());

                    return new FileContentResult(myDataBuffer, "application/pdf")
                    {
                        FileDownloadName = "PropuestaEconomica.pdf"
                    };

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                    return StatusCode(500, "Error al obtener el archivo PDF");
                }
            }

        }

    }
}
