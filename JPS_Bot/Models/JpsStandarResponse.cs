using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JPS_Bot.Models
{
    public partial class JpsStandarResponse
    {
        public long Code { get; set; }
        public string Message { get; set; }
        public List<Result> Result { get; set; }
    }

    public partial class Result
    {
        public string TipoLoteria { get; set; }
        public long NumeroSorteo { get; set; }
        public long Serie { get; set; }
        public long Numero { get; set; }
        public long TipoPremio { get; set; }
        public long SubPremio { get; set; }
        public string Descripcion { get; set; }
        public double MontoUnitario { get; set; }
    }
}
