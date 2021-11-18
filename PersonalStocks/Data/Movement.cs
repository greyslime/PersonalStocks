using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalStocks.Data
{
    public class Movement
    {
        [Key]
        public int Id { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; } = "%";
        public DateTime Date { get; set; }

        [ForeignKey("Stock")]
        public int StockId { get; set; }
        public Stock Stock { get; set; }

    }
}
