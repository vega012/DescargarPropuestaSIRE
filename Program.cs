using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO;
using System.IO.Compression;
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
                //descargar propuesta SIRE
                string NomArchivoReporte = "650de50512af3d2822cfe47e__LE2010139236920230900014040001EXP2.zip";
                string nomArchivoContenido = "LE201013923692023090014040001EXP2.txt";

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://api-sire.sunat.gob.pe/v1/contribuyente/migeigv/libros/rvierce/gestionprocesosmasivos/web/masivo/archivoreporte?nomArchivoReporte=" + NomArchivoReporte + "&codTipoArchivoReporte=01&codLibro=140000");
                request.Method = "GET";
                request.Headers["Authorization"] = "Bearer " + access_token;
                request.ContentType = "application/json";
                string rutaTemporal = Path.GetTempPath();
                string rutaCompleta = Path.Combine(rutaTemporal, NomArchivoReporte);

                // Realiza la solicitud y obtén la respuesta
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Abre un flujo de lectura desde la respuesta
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    responseStream.CopyTo(memoryStream);
                                    byte[] datosBinarios = memoryStream.ToArray();
                                    File.WriteAllBytes(rutaCompleta, datosBinarios);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("La solicitud al API no fue exitosa.Código de estado: " + response.StatusCode);
                        Console.ReadLine();
                    }
                }

                string rutaCompletaCSV = Path.Combine(rutaTemporal, nomArchivoContenido);
                string contenidoCSV = "";
                if (!File.Exists(rutaCompletaCSV))
                {
                    ZipFile.ExtractToDirectory(rutaCompleta, rutaTemporal);
                }
                if (File.Exists(rutaCompletaCSV))
                {
                    // Leer el contenido del archivo CSV
                    contenidoCSV = File.ReadAllText(rutaCompletaCSV);
                    File.Delete(rutaCompletaCSV);
                    Console.WriteLine(contenidoCSV);
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("El archivo  " + rutaCompletaCSV + " no se encuentra en el zip");
                    Console.ReadLine();
                }
            }
        }//fin void main        
    }
}


