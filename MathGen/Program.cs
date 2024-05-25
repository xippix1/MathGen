using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace MathGen
{
	internal class Program
	{
		private static void Main()
		{
			GlobalFontSettings.FontResolver = new CustomFontResolver();

			Console.WriteLine("Select the type of operations (1: Addition/Subtraction, 2: Multiplication/Division): ");
			var operationChoice = Console.ReadLine();

			Console.WriteLine("Enter the range for the outcomes (e.g., 0-20): ");
			var rangeInput = Console.ReadLine();

			if (operationChoice == null || rangeInput == null)
			{
				Console.WriteLine("Invalid input. Exiting.");
				return;
			}

			if (!TryParseRange(rangeInput, out int min, out int max))
			{
				Console.WriteLine("Invalid range. Exiting.");
				return;
			}

			List<string> sums = GenerateSums(operationChoice, min, max, 20 * 40); // 10 pages, 20 sums per page

			Document document = CreatePdfDocument(sums);

			SavePdfDocument(document, "MathSums.pdf");

			Console.WriteLine("PDF generated and saved as MathSums.pdf");
		}

		private static bool TryParseRange(string input, out int min, out int max)
		{
			min = 0;
			max = 0;
			if (input == null)
			{
				return false;
			}

			var parts = input.Split('-');
			if (parts.Length == 2 && int.TryParse(parts[0], out min) && int.TryParse(parts[1], out max) && min <= max)
			{
				return true;
			}
			return false;
		}

		private static List<string> GenerateSums(string operationChoice, int min, int max, int count)
		{
			var sums = new List<string>();
			var rand = new Random();
			var operations = new List<Func<int, int, string>>();

			if (operationChoice == "1")
			{
				operations.Add((a, b) => $"{a} + {b} = ");
				operations.Add((a, b) => $"{a} - {b} = ");
			}
			else if (operationChoice == "2")
			{
				operations.Add((a, b) => $"{a} * {b} = ");
				operations.Add((a, b) => $"{a} / {b} = ");
			}

			while (sums.Count < count)
			{
				int a = rand.Next(min, max + 1);
				int b = rand.Next(min, max + 1);
				string sum = null;

				if (operationChoice == "1")
				{
					if (rand.Next(2) == 0 && a + b <= max)
					{
						sum = $"{a} + {b} = ";
					}
					else if (a - b >= min)
					{
						sum = $"{a} - {b} = ";
					}
				}
				else if (operationChoice == "2")
				{
					if (rand.Next(2) == 0 && a * b <= max)
					{
						sum = $"{a} * {b} = ";
					}
					else if (b != 0 && a / b >= min && a % b == 0)
					{
						sum = $"{a} / {b} = ";
					}
				}

				if (sum != null)
				{
					sums.Add(sum);
				}
			}

			return sums;
		}

		private static Document CreatePdfDocument(List<string> sums)
		{
			var document = new Document();
			var section = document.AddSection();
			section.PageSetup.PageFormat = PageFormat.Letter;

			int sumsPerPage = 40;
			int sumsPerColumn = 20;

			var table = section.AddTable();
			table.Borders.Width = 0;

			var column1 = table.AddColumn(Unit.FromCentimeter(9));
			var column2 = table.AddColumn(Unit.FromCentimeter(9));

			for (int i = 0; i < sums.Count; i += sumsPerPage)
			{
				for (int j = 0; j < sumsPerPage && i + j < sums.Count; j += 2)
				{
					var row = table.AddRow();
					row.Cells[0].AddParagraph(sums[i + j]).Format.Font.Size = 18;
					row.Cells[0].AddParagraph(""); // Add an empty paragraph for spacing

					if (i + j + 1 < sums.Count)
					{
						row.Cells[1].AddParagraph(sums[i + j + 1]).Format.Font.Size = 18;
						row.Cells[1].AddParagraph(""); // Add an empty paragraph for spacing
					}
				}

				if (i + sumsPerPage < sums.Count)
				{
					section.AddPageBreak();
					table = section.AddTable();
					table.Borders.Width = 0;
					column1 = table.AddColumn(Unit.FromCentimeter(9));
					column2 = table.AddColumn(Unit.FromCentimeter(9));
				}
			}

			return document;
		}

		private static void SavePdfDocument(Document document, string filename)
		{
			var pdfRenderer = new PdfDocumentRenderer { Document = document };
			pdfRenderer.RenderDocument();
			pdfRenderer.PdfDocument.Save(filename);
		}
	}
}