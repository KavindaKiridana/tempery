using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.DynamicData;
using System.Xml.Linq;
// Avoid using System.Drawing directly if not needed
using DrawingFont = System.Drawing.Font;
using DrawingRectangle = System.Drawing.Rectangle;
using iTextFont = iTextSharp.text.Font;
using iTextRectangle = iTextSharp.text.Rectangle;

namespace WebApplication1
{
    public class GeneratePDF
    {
        SqlConnection sqlconn = new SqlConnection(ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString);

        public void GetPDF(int DocumentId)
        {
            try
            {
                // Get document data
                var documentData = GetDocumentData(DocumentId);
                if (documentData == null)
                {
                    throw new Exception("Document not found");
                }

                // Generate PDF
                var pdfBytes = CreatePDF(documentData);

                // Generate filename with serial number
                string serialNo = GenerateSerialNumber(documentData);
                string fileName = $"IT_Requisition_{serialNo.Replace("/", "_")}.pdf";

                // Send PDF to browser for download
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                HttpContext.Current.Response.BinaryWrite(pdfBytes);
                //HttpContext.Current.Response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating PDF: {ex.Message}", ex);
            }
        }

        private DocumentModel GetDocumentData(int documentId)
        {
            DocumentModel doc = null;

            try
            {
                sqlconn.Open();

                string query = @"
SELECT 
    d.*,
    c.CName AS CompanyName,
    c.Flag AS CompanyFlag,
    r.RName AS ReasonName,
    dept.DName AS DepartmentName,
    usedDept.DName AS UsedByDepartmentName,
    u.FullName AS RequestedByName,
    deptHead.FullName AS DepartmentHeadName,
    confirmedByUser.FullName AS ConfirmedByUserName
FROM Document d
INNER JOIN Company c ON d.CompanyId = c.CompanyId
INNER JOIN Reason r ON d.ReasonId = r.ReasonId
INNER JOIN Department dept ON d.DepartmentId = dept.DepartmentId
INNER JOIN Department usedDept ON d.UsedByToWhom = usedDept.DepartmentId
INNER JOIN [Users] u ON d.UsersId = u.UsersId
INNER JOIN [Users] deptHead ON d.DepartmentHead = deptHead.UsersId
INNER JOIN [Users] confirmedByUser ON d.ConfirmedBy = confirmedByUser.UsersId
WHERE d.DocumentId = @DocumentId";

                int templateIdFromDoc = 0;

                using (SqlCommand cmd = new SqlCommand(query, sqlconn))
                {
                    cmd.Parameters.AddWithValue("@DocumentId", documentId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Keep TemplateId for the 2nd query (authorizers)
                            templateIdFromDoc = Convert.ToInt32(reader["TemplateId"]);

                            doc = new DocumentModel
                            {
                                DocumentId = documentId,
                                SavedTime = reader["SavedTime"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(reader["SavedTime"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                CompanyFlag = reader["CompanyFlag"].ToString(),
                                Reason = reader["ReasonName"].ToString(),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                UsedByDepartmentName = reader["UsedByDepartmentName"].ToString(),
                                RequestedByName = reader["RequestedByName"].ToString(),
                                DepartmentHeadName = reader["DepartmentHeadName"].ToString(),
                                Budgeted = reader["Budgeted"] != DBNull.Value && Convert.ToBoolean(reader["Budgeted"]),
                                TotalCost = reader["TotalCost"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalCost"]),
                                ITDivisionComment = reader["ITDivisionComment"] == DBNull.Value ? null : reader["ITDivisionComment"].ToString(),
                                ITDivisionRecommendation = reader["ITDivisionRecommendation"] == DBNull.Value ? null : reader["ITDivisionRecommendation"].ToString(),
                                Remarks = reader["Remarks"] == DBNull.Value ? null : reader["Remarks"].ToString(),
                                EIDDateOfPurchase = reader["EIDDateOfPurchase"] == DBNull.Value ? "N/A" : Convert.ToDateTime(reader["EIDDateOfPurchase"]).ToString("yyyy-MM-dd"),
                                EIDMake = reader["EIDMake"] == DBNull.Value ? null : reader["EIDMake"].ToString(),
                                EIDSerialNo = reader["EIDSerialNo"] == DBNull.Value ? null : reader["EIDSerialNo"].ToString(),
                                EIDWarranty = reader["EIDWarranty"] == DBNull.Value ? null : reader["EIDWarranty"].ToString(),
                                EIDModel = reader["EIDModel"] == DBNull.Value ? null : reader["EIDModel"].ToString(),
                                Quotation = reader["Quotation"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["Quotation"]),
                                Configuration = reader["Configuration"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["Configuration"]),
                                CostBreakdown = reader["CostBeakdown"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["CostBeakdown"]),
                                ConfirmedBy = reader["ConfirmedByUserName"].ToString()
                            };
                        }
                    }
                }

                // 2) Get authorizers for the document's template from PersonPosition + Users
                if (doc != null && templateIdFromDoc > 0)
                {
                    doc.Authorizers = GetAuthorizersForTemplate(templateIdFromDoc);
                    doc.NoOfAuthorizers = doc.Authorizers?.Count ?? 0;

                    // Keep your existing calls
                    doc.RequestedItems = GetRequestedItems(documentId);
                    var currencySummary = CalculateCurrencySummary(doc.RequestedItems);
                    doc.IsSameCurrency = currencySummary.IsSameCurrency;
                    doc.Currency = currencySummary.Currency;
                    doc.TotalCost = currencySummary.TotalCost;
                    doc.TotalLKR = currencySummary.TotalLKR;
                    doc.TotalUSD = currencySummary.TotalUSD;
                    doc.TotalINR = currencySummary.TotalINR;
                }
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return doc;
        }


        private List<AuthorizerInfo> GetAuthorizersForTemplate(int flexibleTemplateId)
        {
            var list = new List<AuthorizerInfo>();

            // You can tweak the ORDER BY to whatever display order you want in your PDF
            string sql = @"
SELECT 
    u.FullName,
    pp.Position
FROM PersonPosition pp
INNER JOIN [Users] u ON u.UsersId = pp.PersonId
WHERE pp.FlexibleTemplateId = @FlexibleTemplateId
ORDER BY pp.PersonPositionId";

            using (var cmd = new SqlCommand(sql, sqlconn))
            {
                cmd.Parameters.AddWithValue("@FlexibleTemplateId", flexibleTemplateId);

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new AuthorizerInfo
                        {
                            Name = rdr["FullName"].ToString(),
                            Position = rdr["Position"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private CurrencySummary CalculateCurrencySummary(List<RequestedItemModel> requestedItems)
        {
            var summary = new CurrencySummary();

            if (requestedItems == null || requestedItems.Count == 0)
                return summary;

            // Get distinct currencies
            var distinctCurrencies = requestedItems
                .Where(item => !string.IsNullOrEmpty(item.Currency))
                .Select(item => item.Currency.ToUpper())
                .Distinct()
                .ToList();

            // Check if all items have the same currency
            summary.IsSameCurrency = distinctCurrencies.Count == 1;

            if (summary.IsSameCurrency)
            {
                summary.Currency = distinctCurrencies.First();
                summary.TotalCost = requestedItems.Sum(item => item.Qty * item.UnitPrice);
            }
            else
            {
                // Calculate totals for each currency
                summary.TotalLKR = requestedItems
                    .Where(item => item.Currency?.ToUpper() == "LKR")
                    .Sum(item => item.Qty * item.UnitPrice);

                summary.TotalUSD = requestedItems
                    .Where(item => item.Currency?.ToUpper() == "USD")
                    .Sum(item => item.Qty * item.UnitPrice);

                summary.TotalINR = requestedItems
                    .Where(item => item.Currency?.ToUpper() == "INR")
                    .Sum(item => item.Qty * item.UnitPrice);
            }

            return summary;
        }

        private List<RequestedItemModel> GetRequestedItems(int documentId)
        {
            var items = new List<RequestedItemModel>();

            string query = @"
                SELECT 
                    rip.Description,
                    rip.Qty,
                    rip.UnitPrice,
                    rip.Currency,
                    s.SName as SupplierName
                FROM RequestedItemPayments rip
                INNER JOIN Supplier s ON rip.SupplierId = s.SupplierId
                WHERE rip.DocumentID = @DocumentId
                ORDER BY rip.RequestedItemPaymentsId";

            using (SqlCommand cmd = new SqlCommand(query, sqlconn))
            {
                cmd.Parameters.AddWithValue("@DocumentId", documentId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new RequestedItemModel
                        {
                            Description = reader["Description"].ToString(),
                            Qty = Convert.ToInt32(reader["Qty"]),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            Currency= reader["Currency"].ToString(),
                            SupplierName = reader["SupplierName"].ToString()
                        });
                    }
                }
            }

            return items;
        }


        private string GenerateSerialNumber(DocumentModel doc)
        {
            string year = doc.SavedTime.Year.ToString();
            string documentIdFormatted = doc.DocumentId.ToString("D4");
            return $"{doc.CompanyFlag}/{year}/{documentIdFormatted}";
        }

        private byte[] CreatePDF(DocumentModel doc)
        {
            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 18, 18, 18, 18);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                FooterPageEventHelper footerEvent = new FooterPageEventHelper(doc);
                writer.PageEvent = footerEvent;

                document.Open();

                // Fonts
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                // Header
                CreateHeader(document, doc, titleFont, headerFont, normalFont);
                // Requisition Details
                CreateRequisitionDetails(document, doc, headerFont, normalFont);
                // Existing Item Details
                CreateExistingItemDetails(document, doc, headerFont, normalFont);
                // Cost Summary Table
                CreateCostSummaryTable(document, doc, headerFont, normalFont);
                // Comments and Recommendations
                CreateCommentsSection(document, doc, headerFont, normalFont);
                

                var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
               CreateFooter(document, writer, doc, footerFont, headerFont, normalFont);

                document.Close();
                return memoryStream.ToArray();
            }
        }

        private void CreateHeader(Document document, DocumentModel doc, Font titleFont, Font headerFont, Font normalFont)
        {
            // Create main header table with 3 columns
            var headerTable = new PdfPTable(3) { WidthPercentage = 100 };
            headerTable.SetWidths(new float[] { 30f, 40f, 30f }); // Left, Center, Right proportions

            // Left cell - Form number
            var leftCell = new PdfPCell();
            leftCell.Border = Rectangle.NO_BORDER;
            leftCell.HorizontalAlignment = Element.ALIGN_LEFT;
            leftCell.VerticalAlignment = Element.ALIGN_TOP;
            leftCell.AddElement(new Paragraph("Form:IT-PD-01.1", normalFont));

            // Center cell - Title and Company
            var centerCell = new PdfPCell();
            centerCell.Border = Rectangle.NO_BORDER;
            centerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            centerCell.VerticalAlignment = Element.ALIGN_TOP;

            // Create center content with proper alignment
            var titleParagraph = new Paragraph("IT Approval Requisition form", titleFont);
            titleParagraph.Alignment = Element.ALIGN_CENTER;

            var companyParagraph = new Paragraph("Renuka Group", headerFont);
            companyParagraph.Alignment = Element.ALIGN_CENTER;

            centerCell.AddElement(titleParagraph);
            centerCell.AddElement(companyParagraph);

            // Right cell - Serial Number
            var rightCell = new PdfPCell();
            rightCell.Border = Rectangle.NO_BORDER;
            rightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            rightCell.VerticalAlignment = Element.ALIGN_TOP;
            rightCell.AddElement(new Paragraph($"Serial No:" + GenerateSerialNumber(doc), normalFont));

            // Add cells to table
            headerTable.AddCell(leftCell);
            headerTable.AddCell(centerCell);
            headerTable.AddCell(rightCell);

            // Add table to document
            document.Add(headerTable);

            // Add some space after header
            //document.Add(new Paragraph(" ", normalFont));
        }

        private void CreateRequisitionDetails(Document document, DocumentModel doc, Font headerFont, Font normalFont)
        {
            // Create the first table - 2x2 grid (Date, Requested By, Invoice Company, Allocation Department)
            var topTable = new PdfPTable(4) { WidthPercentage = 100 };
            topTable.SetWidths(new float[] { 25f, 25f, 25f, 25f }); // Equal width columns

            // Row 1: Date and Requested By
            AddCell(topTable, "Date", normalFont, true);
            AddCell(topTable, doc.SavedTime.ToString("dd/MM/yyyy"), normalFont, true);
            AddCell(topTable, "Requested By", normalFont, true);
            AddCell(topTable, doc.RequestedByName, normalFont, true);

            document.Add(topTable);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));// Add minimal spacing

            var topTable2 = new PdfPTable(4) { WidthPercentage = 100 };
            topTable.SetWidths(new float[] { 25f, 25f, 25f, 25f });

            // Row 2: Invoice Company and Allocation Department  
            AddCell(topTable2, "Invoice Company", normalFont, true);
            AddCell(topTable2, doc.CompanyName, normalFont, true);
            AddCell(topTable2, "Allocation Department", normalFont, true);
            AddCell(topTable2, doc.DepartmentName, normalFont, true);

            document.Add(topTable2);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));// Add minimal spacing

            // Create the second table - Reason and Division Head row
            var middleTable = new PdfPTable(4) { WidthPercentage = 100 };
            middleTable.SetWidths(new float[] { 25f, 25f, 25f, 25f });

            AddCell(middleTable, "Reason", normalFont, true);
            AddCell(middleTable, doc.Reason, normalFont, true);
            AddCell(middleTable, "Division Head", normalFont, true);
            AddCell(middleTable, doc.DepartmentHeadName, normalFont, false);

            document.Add(middleTable);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));

            // Create the main requisition details table
            var detailsTable = new PdfPTable(1) { WidthPercentage = 100 };

            // Title row spanning full width
            var titleCell = new PdfPCell(new Phrase("Requisition Details", headerFont));
            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //titleCell.Padding = 5f;
            detailsTable.AddCell(titleCell);

            document.Add(detailsTable);

            // Requirement Items and Suppliers
            var itemsText = string.Join(", ", doc.RequestedItems.ConvertAll(x => x.Description));
            var suppliersText = string.Join(", ", doc.RequestedItems.ConvertAll(x => x.SupplierName).Distinct());

            var bottomTable = new PdfPTable(4) { WidthPercentage = 100 };
            bottomTable.SetWidths(new float[] { 20f, 40f, 20f, 20f });
            AddCell(bottomTable, "Requirement", normalFont, true);
            AddCell(bottomTable, itemsText, normalFont, true);
            AddCell(bottomTable, "Used by/To whom", normalFont, true);
            AddCell(bottomTable, doc.UsedByDepartmentName, normalFont, true);

            AddCell(bottomTable, "Supplier", normalFont, true);
            AddCell(bottomTable, suppliersText, normalFont, false);
            AddCell(bottomTable, "Budgeted", normalFont, true);
            AddCell(bottomTable, doc.Budgeted ? "Yes" : "No", normalFont, false);

            document.Add(bottomTable);

            // Add minimal spacing
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));
        }

        private void CreateExistingItemDetails(Document document, DocumentModel doc, Font headerFont, Font normalFont)
        {
            // Create the main requisition details table
            var detailsTable = new PdfPTable(1) { WidthPercentage = 100 };
            // Title row spanning full width
            var titleCell = new PdfPCell(new Phrase("Existing Item Details (If the item is not a new/ new project)", headerFont));
            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //titleCell.Padding = 5f;
            detailsTable.AddCell(titleCell);
            document.Add(detailsTable);

            // Check each field individually and assign "N/A" if null
            if (string.IsNullOrWhiteSpace(doc.EIDWarranty)) doc.EIDWarranty = "N/A";
            if (string.IsNullOrWhiteSpace(doc.EIDMake)) doc.EIDMake = "N/A";
            if (string.IsNullOrWhiteSpace(doc.EIDModel)) doc.EIDModel = "N/A";
            if (string.IsNullOrWhiteSpace(doc.EIDSerialNo)) doc.EIDSerialNo = "N/A";

            var topTable = new PdfPTable(4) { WidthPercentage = 100 };
            topTable.SetWidths(new float[] { 25f, 25f, 25f, 25f });

            AddCell(topTable, "Date of Purchase", normalFont, true);
            AddCell(topTable, doc.EIDDateOfPurchase, normalFont, true);
            AddCell(topTable, "Warranty", normalFont, true);
            AddCell(topTable, doc.EIDWarranty, normalFont, true);

            AddCell(topTable, "Make", normalFont, true);
            AddCell(topTable, doc.EIDMake, normalFont, true);
            AddCell(topTable, "Model", normalFont, true);
            AddCell(topTable, doc.EIDModel, normalFont, true);

            AddCell(topTable, "Serial Number", normalFont, true);
            AddCell(topTable, doc.EIDSerialNo, normalFont, true);
            AddCell(topTable, " ", normalFont, true);
            AddCell(topTable, " ", normalFont, true);

            document.Add(topTable);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));
        }

        private void CreateCostSummaryTable(Document document, DocumentModel doc, Font headerFont, Font normalFont)
        {
            // Create main table with 7 columns
            var mainTable = new PdfPTable(7) { WidthPercentage = 100 };
            mainTable.SetWidths(new float[] { 24f, 10f, 25f, 23f, 5f, 12f, 15f });

            // First row - Main headers
            var smallerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            var configHeaderCell = new PdfPCell(new Phrase("Costing & Configuration (If repair only quotation will be attached)", smallerFont));
            configHeaderCell.Colspan = 2; // Columns 1-2
            configHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            configHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            configHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(configHeaderCell);

            var costHeaderCell = new PdfPCell(new Phrase("Cost Summary & Recommended Supplier", headerFont));
            costHeaderCell.Colspan = 5; // Columns 3-7 (changed from 6 to 5)
            costHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            costHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            costHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(costHeaderCell);

            // Second row - Sub headers
            var descriptionHeaderCell = new PdfPCell(new Phrase("Description", smallerFont));
            descriptionHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            descriptionHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            descriptionHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(descriptionHeaderCell);

            var attachedHeaderCell = new PdfPCell(new Phrase("Attached", smallerFont));
            attachedHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            attachedHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            attachedHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(attachedHeaderCell);

            var supplierHeaderCell = new PdfPCell(new Phrase("Supplier", headerFont));
            supplierHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            supplierHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            supplierHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(supplierHeaderCell);

            var descriptionHeaderCell2 = new PdfPCell(new Phrase("Description", headerFont));
            descriptionHeaderCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            descriptionHeaderCell2.VerticalAlignment = Element.ALIGN_MIDDLE;
            descriptionHeaderCell2.Border = Rectangle.BOX;
            mainTable.AddCell(descriptionHeaderCell2);

            var qtyHeaderCell = new PdfPCell(new Phrase("Qty", headerFont));
            qtyHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            qtyHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            qtyHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(qtyHeaderCell);

            var unitPriceHeaderCell = new PdfPCell(new Phrase("Unit Price", headerFont));
            unitPriceHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            unitPriceHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            unitPriceHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(unitPriceHeaderCell);

            //   var totalHeaderCell = new PdfPCell(new Phrase($"Total - {doc.Currency}", headerFont));
            var totalHeaderCell = new PdfPCell(new Phrase("Total", headerFont));
            totalHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            totalHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            totalHeaderCell.Border = Rectangle.BOX;
            mainTable.AddCell(totalHeaderCell);

            // Configuration rows
            string[] configItems = { "Quotation", "Configuration Evaluation", "Cost Breakdown" };
            bool?[] configValues = { doc.Quotation, doc.Configuration, doc.CostBreakdown };

            int maxRows = Math.Max(configItems.Length, doc.RequestedItems?.Count ?? 0);

            for (int i = 0; i < maxRows; i++)
            {
                // Column 1: Configuration items (Description)
                if (i < configItems.Length)
                {
                    AddCell(mainTable, configItems[i], smallerFont, false);
                }
                else
                {
                    AddCell(mainTable, "", smallerFont, false);
                }

                // Column 2: Attached status
                if (i < configItems.Length)
                {
                    string attachedText = "";
                    if (configValues[i] == true)
                    {
                        attachedText = "YES";
                    }
                    else if (configValues[i] == false)
                    {
                        attachedText = "NO";
                    }
                    // If configValues[i] is null, attachedText remains empty (blank)

                    var attachedCell = new PdfPCell(new Phrase(attachedText, normalFont));
                    attachedCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    attachedCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    attachedCell.Border = Rectangle.BOX;
                    mainTable.AddCell(attachedCell);
                }
                else
                {
                    AddCell(mainTable, "", normalFont, false);
                }

                // Column 3: Supplier
                if (i < (doc.RequestedItems?.Count ?? 0))
                {
                    AddCell(mainTable, doc.RequestedItems[i].SupplierName ?? "", normalFont, false);
                }
                else
                {
                    AddCell(mainTable, "", normalFont, false);
                }

                // Columns 4-7: Cost summary items
                if (i < (doc.RequestedItems?.Count ?? 0))
                {
                    var item = doc.RequestedItems[i];
                    var total = item.Qty * item.UnitPrice;
                    AddCell(mainTable, item.Description ?? "", normalFont, false);
                    AddCellLeft(mainTable, item.Qty.ToString(), normalFont, false);
                    AddCellLeft(mainTable, item.UnitPrice.ToString("N2"), normalFont, false);
                    AddCellLeft(mainTable, total.ToString("N2"), normalFont, false);

                }
                else
                {
                    // Empty cells for cost summary section (4 cells)
                    for (int j = 0; j < 4; j++)
                    {
                        AddCell(mainTable, "", normalFont, false);
                    }
                }
            }

            document.Add(mainTable);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4))); // small line break

            //var secondTable = new PdfPTable(4) { WidthPercentage = 100 };
            //secondTable.SetWidths(new float[] { 36f,14f,35f,15f });
            //var smallerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            //AddCell(secondTable, "Costing, Configuration & recommendation confirmed by", smallerFont, true);
            //AddCell(secondTable, doc.ConfirmedBy, normalFont, true);
            //AddCell(secondTable, $"Total Cost - {doc.Currency}", normalFont, true);
            //PdfPCell cell = new PdfPCell(new Phrase(doc.TotalCost.ToString("N2"), normalFont));
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //secondTable.AddCell(cell);
            //document.Add(secondTable);

            var confirmTable = new PdfPTable(3) { WidthPercentage = 100 };
            confirmTable.SetWidths(new float[] { 50, 25,25 });
            
            AddCell(confirmTable, "Costing, Configuration & recommendation confirmed by", normalFont, true);
            AddCell(confirmTable, doc.ConfirmedBy, normalFont, true);
            AddCell(confirmTable, "", normalFont, true); // Add empty third cell
            document.Add(confirmTable);

            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));
        }

        private void AddCellLeft(PdfPTable table, string text, Font font, bool isHeader)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);
        }

        private void CreateCommentsSection(Document document, DocumentModel doc, Font headerFont, Font normalFont)
        {
            // IT Division Comments Section
            PdfPTable commentsTable = new PdfPTable(1);
            commentsTable.WidthPercentage = 100;

            // Header cell for IT Division Comments
            PdfPCell headerCell = new PdfPCell(new Phrase("IT Division Comments", headerFont));
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            headerCell.Border = Rectangle.BOX;
            // headerCell.Padding = 5f;
            commentsTable.AddCell(headerCell);

            // Content cell for IT Division Comments
            PdfPCell contentCell = new PdfPCell(new Phrase(doc.ITDivisionComment ?? "", normalFont));
            contentCell.Border = Rectangle.BOX;
            //  contentCell.Padding = 8f;
            contentCell.MinimumHeight = 50f;
            commentsTable.AddCell(contentCell);

            document.Add(commentsTable);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));//small line break


            
            PdfPTable recommendationTable = new PdfPTable(1);
            recommendationTable.WidthPercentage = 100;

            // Header cell for IT Division Recommendation
            PdfPCell recHeaderCell = new PdfPCell(new Phrase("IT Division Recommendation (with justification)", headerFont));
            recHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            recHeaderCell.Border = Rectangle.BOX;
            //  recHeaderCell.Padding = 5f;
            recommendationTable.AddCell(recHeaderCell);

            // Content cell for IT Division Recommendation
            PdfPCell recContentCell = new PdfPCell(new Phrase(doc.ITDivisionRecommendation, normalFont));
            recContentCell.Border = Rectangle.BOX;
     
            recContentCell.MinimumHeight = 40f;
            recommendationTable.AddCell(recContentCell);

            document.Add(recommendationTable);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));//small line break
              
            PdfPTable remarksTable = new PdfPTable(1);
            remarksTable.WidthPercentage = 100;

            // Header cell for Remarks
            PdfPCell remarksHeaderCell = new PdfPCell(new Phrase("Remarks", headerFont));
            remarksHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
            remarksHeaderCell.Border = Rectangle.BOX;
            // remarksHeaderCell.Padding = 5f;
            remarksTable.AddCell(remarksHeaderCell);

            // Content cell for Remarks
            PdfPCell remarksContentCell = new PdfPCell(new Phrase(doc.Remarks, normalFont));
            remarksContentCell.Border = Rectangle.BOX;
            // remarksContentCell.Padding = 8f;
            remarksContentCell.MinimumHeight = 40f;
            remarksTable.AddCell(remarksContentCell);

            document.Add(remarksTable);
            document.Add(new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 4)));//small line break
        }


        private void CreateSignatureSection(Document document, DocumentModel doc, Font headerFont, Font normalFont)
        {
            // Create signature table with dynamic number of columns based on NoOfAuthorizers
            int columnCount = doc.NoOfAuthorizers;
            if (columnCount < 1) return; // Safety check

            var sigTable = new PdfPTable(columnCount) { WidthPercentage = 100 };

            // Set equal column widths
            float[] columnWidths = new float[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                columnWidths[i] = 100f / columnCount;
            }
            sigTable.SetWidths(columnWidths);

            // Add the first row: Names and Positions
            foreach (var authorizer in doc.Authorizers)
            {
                var cell = new PdfPCell(new Phrase(authorizer.Name, normalFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOX;
                cell.Padding = 5f;
                sigTable.AddCell(cell);
            }

            // Add the second row: Signature lines (empty cells)
            foreach (var authorizer in doc.Authorizers)
            {
                var cell = new PdfPCell();
                cell.Border = Rectangle.BOX;
                cell.Padding = 5f;
                cell.MinimumHeight = 40f; // Space for signature
                sigTable.AddCell(cell);
            }

            // Add the third row: Position labels
            foreach (var authorizer in doc.Authorizers)
            {
                var cell = new PdfPCell(new Phrase(authorizer.Position, normalFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOX;
                cell.Padding = 5f;
                sigTable.AddCell(cell);
            }

            document.Add(sigTable);
            // Footer company name - properly formatted as paragraph
            var companyFooterParagraph = new Paragraph("Renuka Group - IT", normalFont);
            companyFooterParagraph.Alignment = Element.ALIGN_CENTER;
            document.Add(companyFooterParagraph);
        }

        private void CreateFooter(Document document, PdfWriter writer, DocumentModel doc, Font footerFontFont, Font headerFont, Font normalFont)
        {
            // This approach manually positions footer but may not stick to bottom if page is empty

            // Calculate remaining space and add filler if needed
            float currentY = writer.GetVerticalPosition(false);
            float pageHeight = document.PageSize.Height;
            float bottomMargin = document.BottomMargin;
            float footerHeight = 120f; // Adjust based on your footer content

            // Calculate space needed to push footer to bottom
            float spaceNeeded = currentY - (bottomMargin + footerHeight);

            if (spaceNeeded > 0)
            {
                // Add invisible spacer to push footer to bottom
                Paragraph spacer = new Paragraph(" ");
                spacer.SpacingAfter = spaceNeeded;
                document.Add(spacer);
            }

            CreateSignatureSection(document, doc, headerFont, normalFont);
        }

        private void AddCell(PdfPTable table, string text, Font font, bool isHeader)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
        }

    }

    // Data Models
    public class DocumentModel
    {
        public int DocumentId { get; set; }
        public DateTime SavedTime { get; set; }
        public string CompanyName { get; set; }
        public string CompanyFlag { get; set; }
        public string DepartmentName { get; set; }
        public string UsedByDepartmentName { get; set; }
        public string RequestedByName { get; set; }
        public string DepartmentHeadName { get; set; }
        public bool Budgeted { get; set; }
        public string ITDivisionComment { get; set; }
        public string ITDivisionRecommendation { get; set; }
        public string Remarks { get; set; }
        public string EIDDateOfPurchase { get; set; }
        public string EIDMake { get; set; }
        public string EIDSerialNo { get; set; }
        public string EIDWarranty { get; set; }
        public string EIDModel { get; set; }
        public bool? Quotation { get; set; }
        public bool? Configuration { get; set; }
        public bool? CostBreakdown { get; set; } 
        public List<RequestedItemModel> RequestedItems { get; set; } = new List<RequestedItemModel>();
        public string Reason { get; set; }
        public string ConfirmedBy { get; set; }
        public int NoOfAuthorizers { get; set; }
        public List<AuthorizerInfo> Authorizers { get; set; } = new List<AuthorizerInfo>();
        public string Currency { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalLKR { get; set; }
        public decimal TotalUSD { get; set; }
        public decimal TotalINR { get; set; }
        public bool IsSameCurrency { get; set; }
    }

    public class AuthorizerInfo
    {
        public string Name { get; set; }
        public string Position { get; set; }
    }

    public class RequestedItemModel
    {
        public string Description { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; }
        public string SupplierName { get; set; }
    }

    public class CurrencySummary
    {
        public bool IsSameCurrency { get; set; }
        public string Currency { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalLKR { get; set; }
        public decimal TotalUSD { get; set; }
        public decimal TotalINR { get; set; }
    }

    public class FooterPageEventHelper : PdfPageEventHelper
    {
        private Font footerFont;
        private DocumentModel doc;

        public FooterPageEventHelper(DocumentModel document)
        {
            this.doc = document;
            this.footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            CreateFooter(writer, document);
        }

        private void CreateFooter(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;
        }
    }

}
