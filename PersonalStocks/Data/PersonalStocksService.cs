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
            catch (Exception ex) { return false; }
        }

        public async Task<bool> UpdateMovement(Movement movement)
        {
            try
            {
                _dBContext.Movements.Update(movement);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> DeleteMovement(Movement movement)
        {
            try
            {
                _dBContext.Remove(movement);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<List<Movement>> AlignMovements(List<Movement> movementsList)
        {
            try
            {
                if (movementsList.Count > 0)
                {
                    Stock stock = movementsList.FirstOrDefault().Stock;
                    List<DateTime> alreadyPresentDates = movementsList.Select(x => x.Date).ToList();

                    List<DateTime> missingMovementDates = await _dBContext.Movements
                                                    .Where(x => x.Stock != stock && !alreadyPresentDates.Contains(x.Date))
                                                    .Select(x => x.Date)
                                                    .ToListAsync();

                    foreach (DateTime missingDate in missingMovementDates)
                    {
                        Movement movement = new Movement();
                        movement.Date = missingDate;
                        movement.Stock = stock;
                        movement.Value = 0;
                        movement.Unit = "%";
                        await AddMovement(movement);
                        movementsList.Add(movement);
                    }

                    return movementsList;
                }
                else throw new Exception("Empty movementList");
            }
            catch(Exception ex) { return new List<Movement>(); }
        }
    }
}