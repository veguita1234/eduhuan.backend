﻿namespace EduHuan.backend.DTOs
{
    public class ProductoDTO
    {
        public Guid ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public string Descripcion { get; set; }
        public string Imagen {  get; set; }
        public string Categoria { get; set; }
    }
}
