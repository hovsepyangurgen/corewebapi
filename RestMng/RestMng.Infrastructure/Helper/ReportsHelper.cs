using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml.Core.ExcelPackage;
using RestMng.Domain;

namespace RestMng.Infrastructure
{
    public static class ReportsHelper
    {
        public static byte[] GenerateCheckPdf(Customers customer, Orders ord, List<MenuItems> menuItems)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A5))
                {
                    PdfWriter.GetInstance(document, ms);
                    document.Open();


                    // Add customer information
                    document.Add(new Paragraph($"Customer Name: {customer.Name}"));
                    document.Add(new Paragraph($"Contact Info: {customer.ContactInfo}"));
                    Paragraph spacePrg = new Paragraph("");
                    float paddingBefore = 10f;
                    float paddingAfter = 10f;
                    spacePrg.SpacingBefore = paddingBefore;
                    spacePrg.SpacingAfter = paddingAfter;
                    document.Add(spacePrg);

                    // Add order items
                    int cellPadding = 3;
                    PdfPTable table = new PdfPTable(3);
                    table.AddCell(new PdfPCell(new Phrase("Name")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Quantity")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Subtotal")) { Padding = cellPadding });
                    foreach (OrderItems item in ord.OrderItems)
                    {
                        table.AddCell(new PdfPCell(new Phrase(menuItems.First(m => m.ItemID == item.ItemID).Name)) { Padding = cellPadding });
                        table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString())) { Padding = cellPadding });
                        table.AddCell(new PdfPCell(new Phrase(item.Subtotal.ToString())) { Padding = cellPadding });
                    }

                    document.Add(table);
                    document.Add(new Paragraph(""));

                    // Add order total amount
                    document.Add(new Paragraph($"Order Total: {ord.TotalAmount}"));


                }
                return ms.ToArray();
            }
        }

        public static byte[] ProductList(List<Inventory> products, List<MenuItems> menuItems)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A5))
                {
                    PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // Products
                    document.Add(new Paragraph("Products"));
                    Paragraph spacePrg = new Paragraph("");
                    float paddingBefore = 10f;
                    float paddingAfter = 10f;
                    spacePrg.SpacingBefore = paddingBefore;
                    spacePrg.SpacingAfter = paddingAfter;
                    document.Add(spacePrg);

                    int cellPadding = 2;
                    PdfPTable table = new PdfPTable(4);
                    table.AddCell(new PdfPCell(new Phrase("Name")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Description")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Price")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Quantity")) { Padding = cellPadding });

                    foreach (Inventory prod in products)
                    {
                        var menuItem = menuItems.First(m => m.ItemID == prod.ItemID);
                        table.AddCell(new PdfPCell(new Phrase(menuItem.Name)) { Padding = cellPadding });
                        table.AddCell(new PdfPCell(new Phrase(menuItem.Description)) { Padding = cellPadding });
                        table.AddCell(new PdfPCell(new Phrase(menuItem.Price.ToString())) { Padding = cellPadding });
                        table.AddCell(new PdfPCell(new Phrase(prod.Quantity.ToString())) { Padding = cellPadding });
                    }
                    document.Add(table);
                    document.Add(new Paragraph(""));
                }
                return ms.ToArray();
            }
        }
        public static byte[] ProductsGenerateExcel(List<Inventory> products, List<MenuItems> menuItems)
        {
            byte[] result;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    // Create the worksheet
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Products");

                    // Add headers to the worksheet
                    worksheet.Cell(1, 1).Value = "Name";
                    worksheet.Cell(1, 2).Value = "Description";
                    worksheet.Cell(1, 3).Value = "Price";
                    worksheet.Cell(1, 4).Value = "Quantity";

                    // Add product items
                    int row = 2;
                    foreach (Inventory prod in products)
                    {
                        var menuItem = menuItems.First(m => m.ItemID == prod.ItemID);
                        worksheet.Cell(row, 1).Value = menuItem.Name;
                        worksheet.Cell(row, 2).Value = menuItem.Description;
                        worksheet.Cell(row, 3).Value = menuItem.Price;
                        worksheet.Cell(row, 4).Value = prod.Quantity;

                        row++;
                    }

                    workbook.SaveAs(memoryStream);
                }

                result = memoryStream.ToArray();
            }

            return result;
        }

        public static byte[] MenuList(List<MenuItems> menuItems)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A5))
                {
                    PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // Products
                    document.Add(new Paragraph("Products"));
                    Paragraph spacePrg = new Paragraph("");
                    float paddingBefore = 10f;
                    float paddingAfter = 10f;
                    spacePrg.SpacingBefore = paddingBefore;
                    spacePrg.SpacingAfter = paddingAfter;
                    document.Add(spacePrg);

                    int cellPadding = 2;
                    PdfPTable table = new PdfPTable(3);
                    table.AddCell(new PdfPCell(new Phrase("Name")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Description")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Price")) { Padding = cellPadding });

                    foreach (MenuItems menuItem in menuItems)
                    {
                        table.AddCell(new PdfPCell(new Phrase(menuItem.Name)) { Padding = cellPadding });
                        table.AddCell(new PdfPCell(new Phrase(menuItem.Description)) { Padding = cellPadding });
                        table.AddCell(new PdfPCell(new Phrase(menuItem.Price.ToString())) { Padding = cellPadding });
                    }
                    document.Add(table);
                    document.Add(new Paragraph(""));
                }
                return ms.ToArray();
            }
        }
        public static byte[] MenuGenerateExcel(List<MenuItems> menuItems)
        {
            byte[] result;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    // Create the worksheet
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Menu");

                    // Add headers to the worksheet
                    worksheet.Cell(1, 1).Value = "Name";
                    worksheet.Cell(1, 2).Value = "Description";
                    worksheet.Cell(1, 3).Value = "Price";

                    // Add product items
                    int row = 2;
                    foreach (MenuItems menuItem in menuItems)
                    {
                        worksheet.Cell(row, 1).Value = menuItem.Name;
                        worksheet.Cell(row, 2).Value = menuItem.Description;
                        worksheet.Cell(row, 3).Value = menuItem.Price;

                        row++;
                    }

                    workbook.SaveAs(memoryStream);
                }

                result = memoryStream.ToArray();
            }

            return result;
        }

        public static byte[] ClientsList(List<Clients> clients, List<Orders> orders, List<Customers> customers)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4))
                {
                    PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // Products
                    document.Add(new Paragraph("Clients"));
                    Paragraph spacePrg = new Paragraph("");
                    float paddingBefore = 10f;
                    float paddingAfter = 10f;
                    spacePrg.SpacingBefore = paddingBefore;
                    spacePrg.SpacingAfter = paddingAfter;
                    document.Add(spacePrg);

                    int cellPadding = 2;
                    PdfPTable table = new PdfPTable(7);
                    table.AddCell(new PdfPCell(new Phrase("Name")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Role")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("OrderID")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Customer")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Status")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("Created")) { Padding = cellPadding });
                    table.AddCell(new PdfPCell(new Phrase("TotalAmount")) { Padding = cellPadding });

                    foreach (Clients client in clients)
                    {
                        foreach (var order in orders.Where(m => m.ClientID == client.ClientID))
                        {
                            var customer = customers.FirstOrDefault(m => m.CustomerID == order.CustomerID);

                            table.AddCell(new PdfPCell(new Phrase(client.Name, new Font(Font.FontFamily.HELVETICA, 10))) { Padding = cellPadding });
                            table.AddCell(new PdfPCell(new Phrase(client.Role.ToString(), new Font(Font.FontFamily.HELVETICA, 10))) { Padding = cellPadding });
                            table.AddCell(new PdfPCell(new Phrase(order.OrderID.ToString(), new Font(Font.FontFamily.HELVETICA, 10))) { Padding = cellPadding });
                            table.AddCell(new PdfPCell(new Phrase(customer?.Name, new Font(Font.FontFamily.HELVETICA, 10))) { Padding = cellPadding });
                            table.AddCell(new PdfPCell(new Phrase(order.Status.ToString(), new Font(Font.FontFamily.HELVETICA, 10))) { Padding = cellPadding });
                            table.AddCell(new PdfPCell(new Phrase(order.Created?.ToString("dd/MM/yyyy HH:mm"), new Font(Font.FontFamily.HELVETICA, 10))) { Padding = cellPadding });
                            table.AddCell(new PdfPCell(new Phrase(order.TotalAmount.ToString(), new Font(Font.FontFamily.HELVETICA, 10))) { Padding = cellPadding });
                        }
                    }
                    document.Add(table);
                    document.Add(new Paragraph(""));
                }
                return ms.ToArray();
            }
        }


        public static byte[] ClientsGenerateExcel(List<Clients> clients, List<Orders> orders, List<Customers> customers)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Clients");

                    // Headers
                    worksheet.Cell(1, 1).Value = "Name";
                    worksheet.Cell(1, 2).Value = "Role";
                    worksheet.Cell(1, 3).Value = "OrderID";
                    worksheet.Cell(1, 4).Value = "Customer";
                    worksheet.Cell(1, 5).Value = "Status";
                    worksheet.Cell(1, 6).Value = "Created";
                    worksheet.Cell(1, 7).Value = "TotalAmount";

                    IXLRange headerRange = worksheet.Range("A1:G1");
                    headerRange.Style.Font.FontSize = 12;
                    headerRange.Style.Font.Bold = true;

                    // Data
                    int row = 2;
                    foreach (Clients client in clients)
                    {
                        foreach (var order in orders.Where(m => m.ClientID == client.ClientID))
                        {
                            var customer = customers.FirstOrDefault(m => m.CustomerID == order.CustomerID);

                            worksheet.Cell(row, 1).Value = client.Name;
                            worksheet.Cell(row, 2).Value = client.Role.ToString();
                            worksheet.Cell(row, 3).Value = order.OrderID.ToString();
                            worksheet.Cell(row, 4).Value = customer?.Name;
                            worksheet.Cell(row, 5).Value = order.Status.ToString();
                            worksheet.Cell(row, 6).Value = order.Created?.ToString("dd/MM/yyyy HH:mm");
                            worksheet.Cell(row, 7).Value = order.TotalAmount.ToString();

                            IXLRange dataRange = worksheet.Range($"A{row}:G{row}");
                            dataRange.Style.Font.FontSize = 10;

                            row++;
                        }
                    }

                    workbook.SaveAs(ms);
                }

                return ms.ToArray();
            }
        }
    }
}
