namespace DocumentProcessor.Services
{
    using DocumentProcessor.Data.Commands;
    using DocumentProcessor.Data.Models;
    using System.Globalization;

    public class DocumentProcessorService : IDocumentProcessorService
    {
        public DocumentProcessResponse ProcessDocument(DocumentProcessCommand command)
        {
            var documents = new List<Document>();
            decimal maxNetValue = 0;
            var productsWithMaxNetValue = new List<string>();
            Document currentDocument = null;
            DocumentItem currentDocumentItem = null;

            var lines = command.RequestBody.Split('\n');

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length == 0) continue;

                var recordType = parts[0];

                if (recordType == "H")
                {
                    currentDocument = new Document
                    {
                        Header = ParseHeader(parts),
                        Items = new List<DocumentItem>()
                    };
                    documents.Add(currentDocument);
                }
                else if (recordType == "C")
                {
                    if (currentDocument is null)
                    {
                        documents.Add(new Document
                        {
                            Description = string.Join(" ", parts.Skip(1))
                        });
                    }
                    else
                    {
                        currentDocument.Description = string.Join(" ", parts.Skip(1));
                    }
                }
                else if (recordType == "B" && currentDocument != null)
                {
                    currentDocumentItem = ParseItem(parts);
                    currentDocument.Items.Add(currentDocumentItem);

                    if (currentDocumentItem.NetValue > maxNetValue)
                    {
                        maxNetValue = currentDocumentItem.NetValue;
                        productsWithMaxNetValue.Clear();
                        productsWithMaxNetValue.Add(currentDocumentItem.ProductName);
                    }
                    else if (currentDocumentItem.NetValue == maxNetValue)
                    {
                        productsWithMaxNetValue.Add(currentDocumentItem.ProductName);
                    }
                }
            }

            var response = new DocumentProcessResponse
            {
                Documents = documents,
                LineCount = lines.Length,
                CharCount = command.RequestBody.Length,
                Sum = documents.Sum(x => x.Items.Count),
                XCount = documents.Count(x => x.Items.Count > command.NumberOfItems),
                ProductsWithMaxNetValue = string.Join(", ", productsWithMaxNetValue.Distinct())
            };

            return response;
        }

        private DocumentHeader ParseHeader(string[] parts)
        {
            if (parts.Length != 17)
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
            if (parts.Length != 13)
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