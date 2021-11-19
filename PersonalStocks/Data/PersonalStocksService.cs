using Microsoft.EntityFrameworkCore;

namespace PersonalStocks.Data
{
    public class PersonalStocksService
    {
        private readonly PersonalStocksContext _dBContext;

        public PersonalStocksService(PersonalStocksContext dBContext) { _dBContext = dBContext; }

        #region Stocks CRUD
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
            catch (Exception) { return false; }
        }

        public async Task<Stock?> GetStockById(int Id)
        {
            try { return await _dBContext.Stocks.FirstOrDefaultAsync(c => c.Id.Equals(Id)); }
            catch (Exception) { return null; }
        }

        public async Task<bool> UpdateStock(Stock Stock)
        {
            try
            {
                _dBContext.Stocks.Update(Stock);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> DeleteStock(Stock Stock)
        {
            try
            {
                _dBContext.Remove(Stock);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region Movements CRUD
        public async Task<List<Movement>> GetMovements(Stock stock = null)
        {
            if(stock != null)
                return await _dBContext.Movements.Where(x => x.Stock == stock).OrderBy(x => x.Date).ToListAsync();
            else
                return await _dBContext.Movements.OrderBy(x => x.Date).ToListAsync();
        }

        public async Task<bool> AddMovement(Movement movement)
        {
            try
            {
                await _dBContext.Movements.AddAsync(movement);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> UpdateMovement(Movement movement)
        {
            try
            {
                _dBContext.Movements.Update(movement);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> DeleteMovement(Movement movement)
        {
            try
            {
                _dBContext.Remove(movement);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        //
        // Summary:
        //      confronta tutte le entry nel database,
        //      se alcuni stock hanno entry per date non presenti negli altri stock
        //      aggiunge negli altri stock un valore vuoto per la data corrispondente
        //      (necessario altrimenti il grafico sballa)
        //
        public async Task AlignMovementsOfStock(Stock stock)
        {
            try
            {
                List<DateTime> alreadyPresentDates = await _dBContext.Movements
                                                    .Where(x => x.Stock == stock)                                
                                                    .Select(x => x.Date)
                                                    .ToListAsync();

                List<DateTime> missingMovementDates = await _dBContext.Movements
                                                .Where(x => x.Stock != stock && !alreadyPresentDates.Contains(x.Date))
                                                .Select(x => x.Date)
                                                .ToListAsync();

                foreach (DateTime missingDate in missingMovementDates)
                    await AddMovement(new(stock, 0, "%", missingDate));
            }
            catch (Exception) { }
        }

        public async Task UpdateStockCurrentValue(Stock stock)
        {
            stock.CurrentValue = stock.StartingValue;

            foreach (Movement movement in await GetMovements(stock))
                stock.ApplyMovement(movement);

            await UpdateStock(stock);
        }
    }
}