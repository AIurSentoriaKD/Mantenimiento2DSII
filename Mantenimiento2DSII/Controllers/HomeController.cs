using Mantenimiento2DSII.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Security.Claims;

namespace Mantenimiento2DSII.Controllers
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
            //List<User> users = new List<User>();
            //// Mysql connection strings
            //using (MySqlConnection con = new MySqlConnection("Server=bl98mfelycxs4dzk0klf-mysql.services.clever-cloud.com; Port=3306; Database=bl98mfelycxs4dzk0klf; Uid=u6vdxycbnlct7yqo; Pwd=lfNWLoFJjgEPxoAz3Wtb"))
            //{
            //    con.Open();
            //    MySqlCommand cmd = new MySqlCommand("Select * from tusuario", con);
            //    MySqlDataReader reader = cmd.ExecuteReader();


            //    while (reader.Read())
            //    {
            //        User user = new User();
            //        user.Usuario = Convert.ToString(reader["Usuario"]);
            //        user.Contrasena = Convert.ToString(reader["constrasena"]);

            //        users.Add(user);
            //    }
            //    reader.Close();

            //    con.Close();
            //}

            return View();
        }

        [HttpGet]
        public IActionResult Docente(string updatestatus = "default")
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.Name).Value;
                Console.WriteLine(currentUserID);

                List<AlumnoNotas> alnotas = new List<AlumnoNotas>();
                using (MySqlConnection con = new MySqlConnection("Server=bl98mfelycxs4dzk0klf-mysql.services.clever-cloud.com; Port=3306; Database=bl98mfelycxs4dzk0klf; Uid=u6vdxycbnlct7yqo; Pwd=lfNWLoFJjgEPxoAz3Wtb"))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from tdocente where usuario= '" + currentUserID + "'", con);
                    MySqlDataReader reader1 = cmd.ExecuteReader();
                    String CodDocente = "";
                    while (reader1.Read())
                    {
                        CodDocente = Convert.ToString(reader1["CodDocente"]);
                    }
                    reader1.Close();
                    // obtencion de datos en base al codigo de docente obtenido en la consulta anterior
                    MySqlCommand alumnonotas_data = new MySqlCommand("call getnotasdecursos_dedocente('" + CodDocente + "')", con);
                    MySqlDataReader alumnonotas_reader = alumnonotas_data.ExecuteReader();
                    while (alumnonotas_reader.Read())
                    {
                        AlumnoNotas alumnonota = new AlumnoNotas();
                        alumnonota.CodAsignatura = Convert.ToString(alumnonotas_reader["CodAsignatura"]);
                        alumnonota.Asignatura = Convert.ToString(alumnonotas_reader["Asignatura"]);
                        alumnonota.CodAlumno = Convert.ToString(alumnonotas_reader["CodAlumno"]);
                        alumnonota.CodCarrera = Convert.ToString(alumnonotas_reader["codtnotas"]);
                        alumnonota.NombreAlumno = Convert.ToString(alumnonotas_reader["NombreAlumno"]);
                        alumnonota.Semestre = Convert.ToString(alumnonotas_reader["Semestre"]);
                        alumnonota.Parcial1 = Convert.ToString(alumnonotas_reader["Parcial1"]);
                        alumnonota.Parcial2 = Convert.ToString(alumnonotas_reader["Parcial2"]);
                        alumnonota.NotaFinal = Convert.ToString(alumnonotas_reader["NotaFinal"]);
                        alnotas.Add(alumnonota);
                    }
                    alumnonotas_reader.Close();

               
                    con.Close();
                }
                ViewBag.message = updatestatus;
                return View(alnotas);
            }
            catch (Exception)
            {
                ViewBag.message = updatestatus;
                return View();
            }

        }
      
        [HttpPost]
        public IActionResult Docente(string codalumno, string nota1, string nota2)
        {
            Console.WriteLine("Actualizada nota de " + codalumno + "." + " Con notas: "+ nota1 + " "+ nota2);
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.Name).Value;
                Console.WriteLine(currentUserID);

                float nota1fl;
                float nota2fl;
                if(float.TryParse(nota1, out nota1fl) && float.TryParse(nota2, out nota2fl))
                {
                    float promed = (nota1fl + nota2fl) / 2;
                    // ambos numeros pudieron convertirse a float actualizando ambas notas
                    Console.WriteLine(promed);
                    using (MySqlConnection con = new MySqlConnection("Server=bl98mfelycxs4dzk0klf-mysql.services.clever-cloud.com; Port=3306; Database=bl98mfelycxs4dzk0klf; Uid=u6vdxycbnlct7yqo; Pwd=lfNWLoFJjgEPxoAz3Wtb"))
                    {
                        con.Open();
                        // actualizando la nota del alumno con id con las indicadas

                        MySqlCommand update2notas = new MySqlCommand("update tnotas set parcial1 = @parcial1, parcial2 = @parcial2, notafinal = @notafinal where codtnotas = @codalumno", con);
                        update2notas.Parameters.AddWithValue("@parcial1", nota1fl);
                        update2notas.Parameters.AddWithValue("@parcial2", nota2fl);
                        update2notas.Parameters.AddWithValue("@notafinal", promed);
                        update2notas.Parameters.AddWithValue("@codalumno", codalumno.ToString());
                        update2notas.ExecuteNonQuery();
                        update2notas.Dispose();

                        con.Close();
                    }
                }   


                
                // si salió bien retorno la view con el mensaje correcto
                return Docente("correcto");
            }
            catch (Exception)
            {
                // si algo salió mal envio el mensaje fallo
                return Docente("error");
            }
            
            
        }
        
        public IActionResult Alumno()
        {
            try
            {
                // Si es alumno
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.Name).Value;
                Console.WriteLine(currentUserID);
                List<Alumno> alumnos = new List<Alumno>();
                List<Notas> notas = new List<Notas>();
                // Mysql connection strings
                using (MySqlConnection con = new MySqlConnection("Server=bl98mfelycxs4dzk0klf-mysql.services.clever-cloud.com; Port=3306; Database=bl98mfelycxs4dzk0klf; Uid=u6vdxycbnlct7yqo; Pwd=lfNWLoFJjgEPxoAz3Wtb"))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("Select * from talumno where Usuario= '"+ currentUserID+"'", con);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    String Codalumno = "";

                    while (reader.Read())
                    {
                        Alumno user = new Alumno();
                        
                        user.CodAlumno = Convert.ToString(reader["CodAlumno"]);
                        Codalumno = Convert.ToString(reader["CodAlumno"]);
                        user.APaterno = Convert.ToString(reader["APaterno"]);
                        user.AMaterno = Convert.ToString(reader["AMaterno"]);
                        user.Usuario = Convert.ToString(reader["Usuario"]);
                        user.Nombres = Convert.ToString(reader["Nombres"]);
                        user.CodCarrera = Convert.ToString(reader["CodCarrera"]);

                        alumnos.Add(user);
                    }
                    reader.Close();
                    MySqlCommand cmd2 = new MySqlCommand("call getnotasforalumno ('"+ Codalumno + "')", con);
                    MySqlDataReader readernotas = cmd2.ExecuteReader();

                    while (readernotas.Read())
                    {
                        Notas nota = new Notas();
                        nota.CodTNotas = Convert.ToString(readernotas["CodTNotas"]);
                        nota.CodAsignatura = Convert.ToString(readernotas["CodAsignatura"]);
                        nota.Asignatura = Convert.ToString(readernotas["Asignatura"]);
                        nota.Semestre = Convert.ToString(readernotas["semestre"]);
                        nota.Parcial1 = Convert.ToString(readernotas["parcial1"]);
                        nota.Parcial2 = Convert.ToString(readernotas["parcial2"]);
                        nota.NotaFinal = Convert.ToString(readernotas["notafinal"]);
                        notas.Add(nota);
                    }

                    readernotas.Close();
                }
                return View(notas);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View();
            }
            
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
