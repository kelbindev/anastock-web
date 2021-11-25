using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IQuoteRepository
    {
        Quote GetQuote(Guid id);
        IEnumerable<QuoteDetails> GetQuoteDetails(Guid id);
        CustomerAddress getCustomerAddress(int id);
        IEnumerable<Quote> GetAllQuotes();
        IEnumerable<Quote> GetQuotesByCompanyId(int id);
        IEnumerable<Quote> GetWonQuotesByCompanyId(int id);
        bool Create(Quote model, int companyId);
        bool Update(Quote model, int companyId);
        bool Delete(Guid id, int companyId);
        bool Convert(Guid id, int companyId);
    }
}
