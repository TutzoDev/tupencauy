using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace tupencauy.Models
{
    public class PencaSitio
    {
        /*
          PARA MANTENER LA INTEGRIDAD DE DATOS:
            ESTA ENTIDAD ANEXA:
                - LAS PENCAS INSTANCIADAS EN UN SITIO CON LOS USUARIOS QUE SE REGISTREN EN LA PENCA
                - TIENE UNA TABLA DE POSICIONES PROPIA DE LA PENCA DENTRO DEL SITIO
                - TIENE PREMIOS CONFIGURADOS PARA ESTA PENCA EN PARTICULAR
                - TIENE NOTIFICACIONES ANEXADAS A LA PENCA EN PARTICULAR           
         */
        public string Id { get; set; }
        public string SitioTenantId { get; set; }
        public string PencaId { get; set; }
        public string Nombre { get; set; }
        public double Costo { get; set; }
        [Range(0, 100, ErrorMessage = "El premio es un %, debe estar entre 0 y 100.")] 
        public double Premio { get; set; }

        //FALTA: AUTOINCREMENTO CUANDO UN USUARIO SE REGISTRA A LA PENCA (Cantidad en la tabla: PencaSitioUsuario)
        public int Inscriptos { get; set; }
        public double Comision { get; set; }
        public double Recaudacion { get; set; }
        //Usuarios registrados + tabla de posiciones
        public ICollection<PencaSitioUsuario> Usuarios { get; set; }
    }
}