using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/*
 * Mini WGet. Utilidad para obtener archivos similar a Wget de Linux
 * Uso:
 *      MniGWet URL [-agent:<some-agent]
 *      
 *      
 * Parámetros:
 *      --agent: Indica un nombre de agente
 *      --cookie: Indica el encabezado Cookie
 *      --referer: Indica el referer
 *      --help: muestra la ayuda
 *      --verbose: Muestra lo que va haciendo la app
 */

namespace MiniWGet
{
    class Program
    {
        static void Main(string[] args)
        {
            const string SEPARATOR = "--"; // Separador de args

            // Código de salida para ERRORLEVEL (por si se llama a MiniWGet desde un archivo BAT, este pueda determinar si MiniWGet termina con normalidad)
            // ver el archivo de ejemplo mwg.bat
            int ex = 0;

            // D' Colores
            ConsoleColor curColor = Console.ForegroundColor;

            // Argumentros que se pasan a la solicitud
            string passUserAgent = null;
            string passCookie = null;
            string passReferer = null;
            string URLGet = null;
            bool verbose = false;
            try
            {
                // Step 1: Parse Args
                if (args.Length == 0) // No arguments
                {
                    ex = 1;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[ ! ] No arguments found, try MiniWGet --help");
                    Console.ForegroundColor = curColor;
                }
                else
                {
                    // Para C# los parámetros se separan por espacios, para esta App cada parámetro se separa por 2 guiones
                    // la forma más fácil de empezar a hacer el parse de los params, es crear una sola cadena y parsearla a nuestro gusto.
                    string allParams;
                    StringBuilder sb = new StringBuilder();
                    for (int p = 0; p < args.Length; p++)
                    {
                        sb.Append(args[p]);
                        sb.Append(" ");
                    }
                    // aquí ya tenemos todos los params en un StringBuilder, lo convertimos a String
                    allParams = sb.ToString().Trim();
                    // Parseamos los params con nuestro --
                    string[] realParams = allParams.Split(new string[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                    for (int p = 0; p < realParams.Length; p++)
                    {
                        // Console.WriteLine(realParams[p]);
                        // Determinar que parámetro es y obtener su value:
                        // ver si es la URL
                        if (realParams[p].StartsWith("http:") || realParams[p].StartsWith("https:"))
                        {
                            URLGet = realParams[p];
                        }
                        else
                        {
                            string[] thisParam = realParams[p].Split(new char[] { ':' },2);
                            string lvalue, rvalue;
                            lvalue = thisParam[0];
                            // Para params sin valor, por ejemplo verbose
                            rvalue = thisParam.Length > 1 ? thisParam[1] : "";
                            switch (lvalue)
                            {
                                // acá cada param
                                case "agent":
                                    {
                                        passUserAgent = rvalue;
                                        break;
                                    }
                                case "cookie":
                                    {
                                        passCookie = rvalue;
                                        break;
                                    }
                                case "referer":
                                    {
                                        passReferer = rvalue;
                                        break;
                                    }
                                case "help":
                                    {
                                        Console.Write("================================================================================\n");
                                        Console.Write("                       Mini WGet Development Version                            \n");
                                        Console.Write("Description: A small console app similar to WGet, this version only works with\n");
                                        Console.Write("plain files. I'm coding this while I learn C# jejeje.\n");
                                        Console.Write("How to use:\n");
                                        Console.Write("Just type MiniWGet http://uri/path --param1:value1 --param2:value2\n");
                                        Console.Write("Parameters:\n");
                                        Console.Write("\t--agent: Set user-agent, useful to spoof browsers.\n");
                                        Console.Write("\t--cookie: Send specified cookies.\n");
                                        Console.Write("\t          Cookies should be sent in this format: COOKIE=VALUE;COOKIE2=VALUE2\n");
                                        Console.Write("\t          Useful to test hijacked sessions :)\n");
                                        Console.Write("\t--referer: Set http-referer header.\n");
                                        Console.Write("\t--help: Shows this help.\n");
                                        Console.Write("\t--verbose: Turns on verbose mode\n");
                                        Console.Write("To save the file instead of display it, use > filename.ext (don't use this with verbose!!!)\n");
                                        Console.Write("Example: Get Google's homepage:\n");
                                        Console.Write("\tMiniWget https://google.com\n");
                                        Console.Write("Example2: Get Yandex's homepage and save it to a file:\n");
                                        Console.Write("\tMiniWget https://yandex.ru > yandex.htm\n");
                                        Console.Write("Example3: Get Yandex's homepage sending a Cookie named PHPSESSID\n");
                                        Console.Write("\tMiniWget https://yandex.ru --cookie:PHPSESSID=AAAAAAAAA\n");
                                        Console.Write("Folow me on Twitter: @underdog1987\n");
                                        Console.Write("================================================================================");
                                        Environment.Exit(0);
                                        break;
                                    }
                                case "verbose":
                                    {
                                        verbose = true;
                                        break;
                                    }
                                default:
                                    {
                                        throw new ArgumentException("Illegal parameter: " + lvalue);
                                        break;
                                    }
                            }
                        }
                    }
                    // ¿sí pasó una URL?
                    if (URLGet == null) { throw new NullReferenceException("Expected: URL"); }

                    // Llenar params que no pasó el user con valores default
                    if (passUserAgent == null)
                    {
                        if (verbose) { Console.WriteLine("\t[INFO] No user-agent passed, using default..."); }
                        passUserAgent = "Mini WGet V0.1 Alfa, by @underdog1987";
                    }
                    if (passCookie == null && verbose)
                    {
                        Console.WriteLine("\t[INFO] No cookie passed, using empty String...");
                        
                    }
                    if (passReferer == null && verbose)
                    {
                        Console.WriteLine("\t[INFO] No referer passed, using empty String..."); 
                    }
                    if (verbose)
                    {
                        Console.WriteLine("\t[INFO] URL Found: " + URLGet);
                    }

                    WebClient w = new WebClient();
                    w.Headers.Add("User-Agent", passUserAgent);
                    if (passReferer != null) { w.Headers.Add("Referer", passReferer); }
                    if (passCookie != null) { w.Headers.Add("Cookie", passCookie); }

                    Stream d = w.OpenRead(URLGet);
                    StreamReader r = new StreamReader(d);
                    //BinaryReader r = new BinaryReader(d);
                    if (verbose) { Console.WriteLine("\t[INFO] Opening...: "); }
                    string s = r.ReadToEnd();
                    if (verbose) { Console.WriteLine("\t[INFO] Reading...: "); }
                    d.Close();
                    r.Close();
                    if (verbose) { Console.WriteLine("\t[INFO] Closing...: "); }
                    // s tiene el contenido obtenido
                    // Sacamos el encabezado Content Type y Lenght
                    for(int rr = 0; rr < w.ResponseHeaders.Count; rr++)
                    {
                        // Console.WriteLine("\t" + w.ResponseHeaders.GetKey(rr) + " = " + w.ResponseHeaders.Get(rr));
                    }
                    Console.Write(s);
                }
                
            }
            catch (Exception e)
            {
                ex = 1;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] " + e.Message);
                
            }
            finally
            {
                Console.ForegroundColor = curColor; // Dejamos el color del texto de la consola como estaba
            }
            Environment.Exit(ex);
        }
    }
}
