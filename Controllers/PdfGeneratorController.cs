using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using backend_amatista.Models;

namespace BackendAmatista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> createReport()
        {
            // Crea una instancia del conversor de HTML a PDF
            HtmlToPdf converter = new HtmlToPdf();

            // Obtener el contenido HTML (puede ser dinámico con datos de un modelo)
            string htmlContent = GenerateHtmlContent();

            // Convertir el HTML en PDF
            PdfDocument doc = converter.ConvertHtmlString(htmlContent);

            using (MemoryStream pdfStream = new MemoryStream())
            {
                doc.Save(pdfStream);
                doc.Close();

                // Convertir el stream en un arreglo de bytes
                byte[] pdfBytes = pdfStream.ToArray();

                // Devolver el archivo PDF
                return File(pdfBytes, "application/pdf", "report.pdf");
            }
        }

        // Método para generar contenido HTML
        private string GenerateHtmlContent()
        {
          
            string html = $@"
                <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            h1 {{ color: navy; }}
                            table {{ width: 100%; border-collapse: collapse; }}
                            th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
                            th {{ background-color: #f2f2f2; }}
                        </style>
                    </head>
                    <body>
                        <h1>hola</h1>
                        <p>asd</p>
                        <table>
                            <tr>
                                <th>Producto</th>
                                <th>Cantidad</th>
                                <th>Precio</th>
                                <th>Total</th>
                            </tr>";
            html += @"
                        </table>
                    </body>
                </html>";

            return html;
        }
    }
}
