namespace SistemaVentasBatia.Models
{
    public class PuestoTabulador
    {
        public int IdPuestoSalario { get; set; }
        public int IdPuesto { get; set; }
        public decimal SalarioMixto { get; set; }
        public decimal SalarioMixtoFrontera { get; set; }
        public decimal SalarioReal { get; set; }
        public decimal SalarioRealFrontera { get; set; }
    }
}
