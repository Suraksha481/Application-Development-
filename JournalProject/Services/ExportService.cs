using JournalProject.Models;
using System.Text;

namespace JournalProject.Services
{
    public class ExportService
    {
        public async Task<string> ExportToCsvAsync(List<JournalEntry> entries, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                using var writer = new StreamWriter(filePath);
                await writer.WriteLineAsync("Date,Title,Mood,WordCount,Content");

                foreach (var entry in entries)
                {
                    var content = (entry.Content ?? "").Replace(",", ";").Replace("\n", " ");
                    await writer.WriteLineAsync($"{entry.CreatedAt:yyyy-MM-dd},\"{entry.Title}\",\"{entry.PrimaryMood}\",{entry.WordCount},\"{content}\"");
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to CSV: {ex.Message}");
            }
        }

        public async Task<string> ExportToJsonAsync(List<JournalEntry> entries, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                var json = System.Text.Json.JsonSerializer.Serialize(entries, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to JSON: {ex.Message}");
            }
        }

        public async Task<string> ExportToTextAsync(List<JournalEntry> entries, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                using var writer = new StreamWriter(filePath);
                foreach (var entry in entries.OrderByDescending(e => e.CreatedAt))
                {
                    await writer.WriteLineAsync($"Date: {entry.CreatedAt:yyyy-MM-dd}");
                    await writer.WriteLineAsync($"Title: {entry.Title}");
                    await writer.WriteLineAsync($"Mood: {entry.PrimaryMood}");
                    await writer.WriteLineAsync("---");
                    await writer.WriteLineAsync(entry.Content);
                    await writer.WriteLineAsync("\n\n");
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to Text: {ex.Message}");
            }
        }

        public async Task<string> ExportToPdfAsync(List<JournalEntry> entries, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                var sb = new StringBuilder();
                
                // PDF Header
                sb.AppendLine("%PDF-1.4");
                sb.AppendLine("1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj");
                sb.AppendLine("2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj");
                
                // Create content stream with all journal entries
                var contentSb = new StringBuilder();
                contentSb.AppendLine("BT");
                contentSb.AppendLine("/F1 14 Tf");
                contentSb.AppendLine("50 750 Td");
                contentSb.AppendLine("(Journal Export) Tj");
                contentSb.AppendLine("0 -30 Td");
                
                int lineCount = 0;
                foreach (var entry in entries.OrderByDescending(e => e.CreatedAt))
                {
                    contentSb.AppendLine($"({EscapePdfText(entry.CreatedAt.ToString("yyyy-MM-dd"))}) Tj");
                    contentSb.AppendLine("0 -15 Td");
                    lineCount++;
                    
                    contentSb.AppendLine($"(Title: {EscapePdfText(entry.Title ?? "")}) Tj");
                    contentSb.AppendLine("0 -15 Td");
                    lineCount++;
                    
                    contentSb.AppendLine($"(Mood: {EscapePdfText(entry.PrimaryMood ?? "")}) Tj");
                    contentSb.AppendLine("0 -15 Td");
                    lineCount++;
                    
                    contentSb.AppendLine("(---) Tj");
                    contentSb.AppendLine("0 -15 Td");
                    lineCount++;
                    
                    // Split content into lines for better PDF display
                    var contentLines = (entry.Content ?? "").Split(new[] { '\n', '\r' }, StringSplitOptions.None);
                    foreach (var line in contentLines.Take(20))
                    {
                        var escapedLine = EscapePdfText(line);
                        if (!string.IsNullOrEmpty(escapedLine))
                        {
                            contentSb.AppendLine($"({escapedLine}) Tj");
                            contentSb.AppendLine("0 -12 Td");
                            lineCount++;
                        }
                    }
                    
                    contentSb.AppendLine("() Tj");
                    contentSb.AppendLine("0 -20 Td");
                    lineCount += 2;
                }
                
                contentSb.AppendLine("ET");
                
                string content = contentSb.ToString();
                byte[] contentBytes = Encoding.ASCII.GetBytes(content);
                
                sb.AppendLine("3 0 obj<</Type/Page/Parent 2 0 R/Resources<</Font<</F1<</Type/Font/Subtype/Type1/BaseFont/Helvetica>>>>>>/MediaBox[0 0 612 792]/Contents 4 0 R>>endobj");
                sb.AppendLine($"4 0 obj<</Length {contentBytes.Length}>>stream");
                sb.Append(content);
                sb.AppendLine("endstream");
                sb.AppendLine("endobj");
                
                // Cross-reference table
                var xref = sb.Length;
                sb.AppendLine("xref");
                sb.AppendLine("0 5");
                sb.AppendLine("0000000000 65535 f");
                sb.AppendLine("0000000009 00000 n");
                sb.AppendLine("0000000058 00000 n");
                sb.AppendLine("0000000115 00000 n");
                sb.AppendLine("0000000270 00000 n");
                
                // Trailer
                sb.AppendLine("trailer<</Size 5/Root 1 0 R>>");
                sb.AppendLine("startxref");
                sb.AppendLine(xref.ToString());
                sb.AppendLine("%%EOF");

                // Write PDF file
                await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.ASCII);

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to PDF: {ex.Message}");
            }
        }

        public async Task<string> ExportToOdfAsync(List<JournalEntry> entries, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                // Create ODF (OpenDocument Format) content
                var contentXml = new StringBuilder();
                contentXml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                contentXml.AppendLine("<office:document xmlns:office=\"urn:oasis:names:tc:opendocument:xmlns:office:1.0\"");
                contentXml.AppendLine("  xmlns:text=\"urn:oasis:names:tc:opendocument:xmlns:text:1.0\"");
                contentXml.AppendLine("  xmlns:style=\"urn:oasis:names:tc:opendocument:xmlns:style:1.0\"");
                contentXml.AppendLine("  xmlns:xlink=\"http://www.w3.org/1999/xlink\"");
                contentXml.AppendLine("  office:version=\"1.2\">");
                contentXml.AppendLine("  <office:body>");
                contentXml.AppendLine("    <office:text>");

                foreach (var entry in entries.OrderByDescending(e => e.CreatedAt))
                {
                    contentXml.AppendLine("      <text:h text:outline-level=\"1\">");
                    contentXml.AppendLine($"        <text:p>{EscapeXml(entry.Title ?? "")}</text:p>");
                    contentXml.AppendLine("      </text:h>");

                    contentXml.AppendLine("      <text:p>");
                    contentXml.AppendLine($"        <text:span>Date: {entry.CreatedAt:yyyy-MM-dd HH:mm:ss}</text:span>");
                    contentXml.AppendLine("      </text:p>");

                    contentXml.AppendLine("      <text:p>");
                    contentXml.AppendLine($"        <text:span><text:s/><text:s/>Primary Mood: {EscapeXml(entry.PrimaryMood ?? "")}</text:span>");
                    contentXml.AppendLine("      </text:p>");

                    if (!string.IsNullOrEmpty(entry.SecondaryMood1))
                    {
                        contentXml.AppendLine("      <text:p>");
                        contentXml.AppendLine($"        <text:span><text:s/><text:s/>Secondary Mood 1: {EscapeXml(entry.SecondaryMood1)}</text:span>");
                        contentXml.AppendLine("      </text:p>");
                    }

                    if (!string.IsNullOrEmpty(entry.SecondaryMood2))
                    {
                        contentXml.AppendLine("      <text:p>");
                        contentXml.AppendLine($"        <text:span><text:s/><text:s/>Secondary Mood 2: {EscapeXml(entry.SecondaryMood2)}</text:span>");
                        contentXml.AppendLine("      </text:p>");
                    }

                    if (!string.IsNullOrEmpty(entry.Category))
                    {
                        contentXml.AppendLine("      <text:p>");
                        contentXml.AppendLine($"        <text:span><text:s/><text:s/>Category: {EscapeXml(entry.Category)}</text:span>");
                        contentXml.AppendLine("      </text:p>");
                    }

                    if (entry.Tags != null && entry.Tags.Count > 0)
                    {
                        contentXml.AppendLine("      <text:p>");
                        contentXml.AppendLine($"        <text:span><text:s/><text:s/>Tags: {EscapeXml(string.Join(", ", entry.Tags))}</text:span>");
                        contentXml.AppendLine("      </text:p>");
                    }

                    contentXml.AppendLine("      <text:p>");
                    contentXml.AppendLine("        <text:span>Word Count: " + entry.WordCount + "</text:span>");
                    contentXml.AppendLine("      </text:p>");

                    contentXml.AppendLine("      <text:p>");
                    contentXml.AppendLine("        <text:span>---</text:span>");
                    contentXml.AppendLine("      </text:p>");

                    // Add content
                    var contentLines = (entry.Content ?? "").Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    foreach (var line in contentLines)
                    {
                        contentXml.AppendLine("      <text:p>");
                        contentXml.AppendLine($"        <text:span>{EscapeXml(line)}</text:span>");
                        contentXml.AppendLine("      </text:p>");
                    }

                    contentXml.AppendLine("      <text:p/>");
                    contentXml.AppendLine("      <text:p/>");
                }

                contentXml.AppendLine("    </office:text>");
                contentXml.AppendLine("  </office:body>");
                contentXml.AppendLine("</office:document>");

                // For simplicity, save as ODT by just writing the XML
                // A proper ODT would be a ZIP file with multiple XML files
                await File.WriteAllTextAsync(filePath, contentXml.ToString(), Encoding.UTF8);

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to ODF: {ex.Message}");
            }
        }

        private string EscapeXml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        private string EscapePdfText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            
            // Remove special characters and limit length
            var limited = text.Length > 100 ? text.Substring(0, 100) : text;
            var escaped = limited.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)").Replace("\r", "").Replace("\n", " ");
            return escaped;
        }

        public async Task<string> ExportToCSV(List<JournalEntry> entries, string filePath)
        {
            return await ExportToCsvAsync(entries, filePath);
        }

        public async Task<string> ExportToPDF(List<JournalEntry> entries, string filePath)
        {
            return await ExportToPdfAsync(entries, filePath);
        }

        public async Task<string> ExportToText(List<JournalEntry> entries, string filePath)
        {
            return await ExportToTextAsync(entries, filePath);
        }
    }
}
