namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class PaginadoResultado<T>
    {
        public List<T> Items { get; set; } = new();
        public int PaginaActual { get; set; } = 1;
        public int TamanioPagina { get; set; } = 10;
        public int TotalRegistros { get; set; }
        public string? Busqueda { get; set; }

        public int TotalPaginas => TamanioPagina > 0
            ? (int)Math.Ceiling(TotalRegistros / (double)TamanioPagina)
            : 0;
    }
}