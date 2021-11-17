using Microsoft.EntityFrameworkCore;

namespace PersonalStocks.Data
{
    public class PersonalStocksService
    {
        private readonly PersonalStocksContext _dBContext;

        public PersonalStocksService(PersonalStocksContext dBContext) { _dBContext = dBContext; }

        public async Task<List<Stock>> GetStocks(string filter = "")
        {
            return await _dBContext.Stocks.ToListAsync();
        }

        public async Task<bool> AddStock(Stock Stock)
        {
            try
            {
                await _dBContext.Stocks.AddAsync(Stock);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<Stock> GetStockById(int Id)
        {
            return await _dBContext.Stocks.FirstOrDefaultAsync(c => c.Id.Equals(Id));
        }

        public async Task<bool> UpdateStock(Stock Stock)
        {
            try
            {
                _dBContext.Stocks.Update(Stock);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex) { return false; }
        }

        public async Task<bool> DeleteStock(Stock Stock)
        {
            try
            {
                _dBContext.Remove(Stock);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex) { return false; }
        }
    }
}