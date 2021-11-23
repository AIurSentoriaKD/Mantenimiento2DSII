namespace Mantenimiento2DSII.Models
{
    // esta clase contendra todos los datos de los alumnos que un docente
    // tenga en un curso
    public class AlumnoNotas
    {
        public string CodAsignatura { get; set; }
        public string Asignatura { get; set;}
        public string CodAlumno { get; set; }  
        public string CodCarrera { get; set; }
        public string NombreAlumno { get; set; }
        public string Semestre { get; set; }
        public string Parcial1 { get; set; }
        public string Parcial2 { get; set; }
        public string NotaFinal { get; set; }
    }
}
