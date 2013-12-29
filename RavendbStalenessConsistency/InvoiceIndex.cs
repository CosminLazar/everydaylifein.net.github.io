using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace RavendbStalenessConsistency
{
    public class InvoiceIndex : AbstractIndexCreationTask<Invoice>
    {
        public InvoiceIndex()
        {
            Map = invoices => from invoice in invoices
                select new
                {
                    invoice.Price,
                    invoice.Fees,
                    invoice.TotalValue
                };

            Store(x => x.TotalValue, FieldStorage.Yes);
        }
    }
}