using System;
using System.Linq;
using System.Threading.Tasks;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Tests.Helpers;
using Xunit;

namespace RavendbStalenessConsistency
{
    public class TestIndexStaleness : RavenTestBase
    {
        private readonly IDocumentStore _store;
        private string _targetInvoiceId;
        private readonly Random _randomDude = new Random();

        public TestIndexStaleness()
        {
            _store = NewDocumentStore();
        }

        [Fact]
        public void ProjectedQueryHasTheSameStalenessLevel()
        {
            SetupTestInvoiceAndIndex();

            StartChangingTheTestInvoiceLikeCrazy();

            while (true)
            {
                using (var session = _store.OpenSession())
                {
                    var invoices = session.Query<Invoice, InvoiceIndex>()
                                        .Select(x => new
                        {
                            x.Price,
                            x.Fees,
                            x.TotalValue
                        })
                        .ToArray();

                    var targetInvoice = invoices.Single();

                    var expected = targetInvoice.Fees + targetInvoice.Price;
                    var actual = targetInvoice.TotalValue;

                    Assert.Equal(expected, actual);
                }
            }
        }

        private void SetupTestInvoiceAndIndex()
        {
            _targetInvoiceId = StoreTestInvoice();
            CreateIndexes();
            WaitForIndexing(_store);
        }

        private void StartChangingTheTestInvoiceLikeCrazy()
        {
            Enumerable.Range(0, Environment.ProcessorCount).ForEach(_ => Task.Run(() =>
            {
                while (true)
                {
                    using (var session = _store.OpenSession())
                    {
                        var invoice = session.Load<Invoice>(_targetInvoiceId);

                        var nextPrice = _randomDude.Next();
                        var nextFees = _randomDude.Next();

                        invoice.SetPrices(nextPrice, nextFees);
                        session.SaveChanges();
                    }
                }
            }));
        }

        private void CreateIndexes()
        {
            _store.ExecuteIndex(new InvoiceIndex());
        }

        private string StoreTestInvoice()
        {
            using (var session = _store.OpenSession())
            {
                var invoice = new Invoice();
                invoice.SetPrices(1, 2);

                session.Store(invoice);
                session.SaveChanges();

                return invoice.Id;
            }
        }
    }
}
