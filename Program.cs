using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ConexionSIRE
{
    class Program
    {
        static void Main(string[] args)
        {

            string idclient = "", client_secret = "", username = "", password = "";

            
            idclient = "ID_CLIENTE_SUNAT";
            client_secret = "SECRET_CLIENT";
            username = "RUCUSUARIO";
            password = "CLAVE_USUARIO";
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://api-seguridad.sunat.gob.pe/v1/clientessol/" + idclient + "/oauth2/token/");
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            string postData = "grant_type=password&scope=https://api-cpe.sunat.gob.pe&client_id=" + idclient + "&client_secret=" + client_secret + "&username=" + username + "&password=" + password;
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(postData);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null)
            {
                Console.WriteLine("Problemas al iniciar sesion SUNAT");
                Console.ReadLine();
            }
            else
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string Result = sr.ReadToEnd().Trim();
                if (Result.Trim() == "")
                {

                    Console.WriteLine("Problemas al iniciar sesion SUNAT");
                    Console.ReadLine();
                }
                Newtonsoft.Json.Linq.JObject ResponseData = Newtonsoft.Json.Linq.JObject.Parse(Result);
                var access_token = ResponseData["access_token"];
                //consultar estado del ticket
                string periodo = "202305";
                string ticket = "20230300000011";
                req = System.Net.WebRequest.Create("https://api-sire.sunat.gob.pe/v1/contribuyente/migeigv/libros/rvierce/gestionprocesosmasivos/web/masivo/consultaestadotickets?perIni=" + periodo + "&perFin=" + periodo + "&page=1&perPage=20&numTicket=" + ticket);
                req.Headers["Authorization"] = "Bearer " + access_token;
                req.ContentType = "application/json";
                req.Method = "GET";
                resp = req.GetResponse();
                if (resp == null)
                {
                    Console.WriteLine("No existe informacion del ticket");
                    Console.ReadLine();
                }
                else
                {
                    sr = new System.IO.StreamReader(resp.GetResponseStream());
                    Result = sr.ReadToEnd().Trim();
                    ApiResponseTicket apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponseTicket>(Result);


                    foreach (var registro in apiResponse.registros)
                    {
                        Console.WriteLine("estado de envio: " + registro.detalleTicket.desEstadoEnvio);
                        foreach (var item in registro.archivoReporte)
                        {
                            Console.WriteLine("nombre del reporte: " + item.nomArchivoReporte);
                            Console.WriteLine("nombre del archivo: " + item.nomArchivoContenido);
                        }
                    }
                    Console.ReadLine();
                }

            }
        }//fin void main
        public class ApiResponseTicket
        {
            public Paginacion paginacion { get; set; }
            public List<Registro> registros { get; set; }
        }
        public class Paginacion
        {
            public int page { get; set; }
            public int perPage { get; set; }
            public int totalRegistros { get; set; }
        }
        public class Registro
        {
            public string showReporteDescarga { get; set; }
            public string perTributario { get; set; }
            public string numTicket { get; set; }
            public object fecCargaImportacion { get; set; }
            public string fecInicioProceso { get; set; }
            public string codProceso { get; set; }
            public string desProceso { get; set; }
            public string codEstadoProceso { get; set; }
            public string desEstadoProceso { get; set; }
            public object nomArchivoImportacion { get; set; }
            public DetalleTicket detalleTicket { get; set; }
            public List<ArchivoReporte> archivoReporte { get; set; }
        }
        public class DetalleTicket
        {
            public string numTicket { get; set; }
            public string fecCargaImportacion { get; set; }
            public string horaCargaImportacion { get; set; }
            public string codEstadoEnvio { get; set; }
            public string desEstadoEnvio { get; set; }
            public string nomArchivoReporte { get; set; }
            public int cntFilasvalidada { get; set; }
            public int cntCPError { get; set; }
            public int cntCPInformados { get; set; }
        }

        public class ArchivoReporte
        {
            public object codTipoAchivoReporte { get; set; }
            public string nomArchivoReporte { get; set; }
            public string nomArchivoContenido { get; set; }
        }
    }
}


