namespace DocumentProcessor.Services
{
    using DocumentProcessor.Data.Commands;
    using DocumentProcessor.Data.Models;
    using System.Globalization;

    public class DocumentProcessorService : IDocumentProcessorService
    {
        public async Task<DocumentProcessResponse> ProcessDocument(DocumentProcessCommand command)
        {
            var documents = new List<Document>();
            decimal maxFileValueProduct = 0;
            string productsWithMaxNetValue = string.Empty;

            string[] lines = command.RequestBody.Split('\n');

            Document? currentDocument = null;

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length == 0)
                {
                    continue;
                }

                string recordType = parts[0];

                if (recordType == "H")
                {
                    currentDocument = new Document
                    {
                        Header = ParseHeader(parts),
                        Items = new List<DocumentItem>()
                    };

                    documents.Add(currentDocument);

                    var maxNetValueProduct = currentDocument.Items.OrderByDescending(item => item.NetValue).FirstOrDefault();
                    if (maxNetValueProduct != null)
                    {
                        if (maxFileValueProduct == maxNetValueProduct.NetValue)
                        {
                            productsWithMaxNetValue = String.Join(", ", productsWithMaxNetValue, maxNetValueProduct.ProductName);
                        }
                        else if (maxNetValueProduct.NetValue > maxFileValueProduct)
                        {
                            productsWithMaxNetValue = maxNetValueProduct.ProductName;
                        }
                    }
                }

                else if (recordType == "B" && currentDocument != null)
                {
                    documents.Last().Items.Add(ParseItem(parts));
                }
            }

            var maxNetValueProducts = new List<string>();

            if (documents.Count > 0)
            {
                maxNetValueProducts = documents
                    .SelectMany(doc => doc.Items)
                    .GroupBy(item => item.NetValue)
                    .OrderByDescending(group => group.Key)
                    .FirstOrDefault()
                    .Select(item => item.ProductName)
                    .ToList();
            }

            DocumentProcessResponse response = new DocumentProcessResponse
            {
                Documents = documents,
                LineCount = command.RequestBody.Split('\n').Length,
                CharCount = command.RequestBody.Length,
                Sum = documents.Sum(x => x.Items.Count),
                XCount = documents.Count(x => x.Items.Count > command.NumberOfItems),
                ProductsWithMaxNetValue = String.Join(", ", maxNetValueProducts)
            };

            return response;
        }

        private DocumentHeader ParseHeader(string[] parts)
        {
            if (parts.Length < 16)
            {
                throw new ArgumentException("Invalid Document header format. Header does not contain 15 parts.");
            }

            return new DocumentHeader
            {
                CodeBA = parts[1],
                Type = parts[2],
                DocumentNumber = parts[3],
                OperationDate = DateTime.ParseExact(parts[4], "dd-MM-yyyy", CultureInfo.InvariantCulture),
                DocumentDayNumber = int.Parse(parts[5]),
                ContractorCode = parts[6],
                ContractorName = parts[7],
                ExternalDocumentNumber = parts[8],
                ExternalDocumentDate = DateTime.ParseExact(parts[9], "dd-MM-yyyy", CultureInfo.InvariantCulture),
                NetAmount = decimal.Parse(parts[10].Replace('.', ',')),
                VatAmount = decimal.Parse(parts[11].Replace('.', ',')),
                GrossAmount = decimal.Parse(parts[12].Replace('.', ',')),
                Field1 = parts[13],
                Field2 = parts[14],
                Field3 = parts[15]
            };
        }

        private DocumentItem ParseItem(string[] parts)
        {
            if (parts.Length < 12)
            {
                throw new ArgumentException("Invalid DocumentItem format. Item does not contain 11 parts.");
            }

            return new DocumentItem
            {
                ProductCode = parts[1],
                ProductName = parts[2],
                Quantity = decimal.Parse(parts[3].Replace('.', ',')),
                NetPrice = decimal.Parse(parts[4].Replace('.', ',')),
                NetValue = decimal.Parse(parts[5].Replace('.', ',')),
                VatValue = decimal.Parse(parts[6].Replace('.', ',')),
                QuantityBefore = decimal.Parse(parts[7].Replace('.', ',')),
                AvgBefore = decimal.Parse(parts[8].Replace('.', ',')),
                QuantityAfter = decimal.Parse(parts[9].Replace('.', ',')),
                AvgAfter = decimal.Parse(parts[10].Replace('.', ',')),
                Group = parts[11]
            };
        }
    }
}