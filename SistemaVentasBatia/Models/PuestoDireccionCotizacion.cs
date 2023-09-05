﻿using System;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class PuestoDireccionCotizacion
    {
        public int IdPuestoDireccionCotizacion { get; set; }
        public int IdPuesto { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public int Cantidad { get; set; }
        public decimal Jornada { get; set; }
        public int IdTurno { get; set; }
        public int IdSalario { get; set; }
        public TimeSpan HrInicio { get; set; }
        public TimeSpan HrFin { get; set; }
        public DiaSemana DiaInicio { get; set; }
        public DiaSemana DiaFin { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }
        public decimal Sueldo { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal PrimaVacacional { get; set; }
        public decimal IMSS { get; set; }
        public decimal ISN { get; set; }
        public decimal Aguinaldo { get; set; }
        public decimal Total { get; set; }
        // custom
        public string Turno { get; set; }
        public string Puesto { get; set; }
        public int IdCotizacion { get; set; }
    }
}
