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

			Console.WriteLine("Select the type of operations (1: Addition/Subtraction, 2: Multiplication/Division, 3: Algebra): ");
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

			List<string> problems;
			if (operationChoice == "3")
			{
				problems = GenerateAlgebraProblems(min, max, 10 * 20); // 10 pages, 20 problems per page
			}
			else
			{
				problems = GenerateMathProblems(operationChoice, min, max, 10 * 20); // 10 pages, 20 problems per page
			}

			Document document = CreatePdfDocument(problems);

			SavePdfDocument(document, "MathProblems.pdf");

			Console.WriteLine("PDF generated and saved as MathProblems.pdf");
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

		private static List<string> GenerateMathProblems(string operationChoice, int min, int max, int count)
		{
			var problems = new List<string>();
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

			while (problems.Count < count)
			{
				int a = rand.Next(min, max + 1);
				int b = rand.Next(min, max + 1);
				string problem = null;

				if (operationChoice == "1")
				{
					if (rand.Next(2) == 0 && a + b <= max)
					{
						problem = $"{a} + {b} = ";
					}
					else if (a - b >= min)
					{
						problem = $"{a} - {b} = ";
					}
				}
				else if (operationChoice == "2")
				{
					if (rand.Next(2) == 0 && a * b <= max)
					{
						problem = $"{a} * {b} = ";
					}
					else if (b != 0 && a / b >= min && a % b == 0)
					{
						problem = $"{a} / {b} = ";
					}
				}

				if (problem != null)
				{
					problems.Add(problem);
				}
			}

			return problems;
		}

		private static List<string> GenerateAlgebraProblems(int min, int max, int count)
		{
			var problems = new List<string>();
			var rand = new Random();

			while (problems.Count < count)
			{
				int a = rand.Next(min, max + 1);
				int b = rand.Next(min, max + 1);
				int c = rand.Next(min, max + 1);
				string problem = null;

				switch (rand.Next(3))
				{
					case 0:
						// X is the result
						if (a + b <= max)
						{
							problem = $"{a} + {b} = X";
						}
						else if (a - b >= min)
						{
							problem = $"{a} - {b} = X";
						}
						break;

					case 1:
						// X is in the first position
						if (b + c <= max)
						{
							problem = $"X + {b} = {b + c}";
						}
						else if (c - b >= min)
						{
							problem = $"X - {b} = {c - b}";
						}
						break;

					case 2:
						// X is in the second position
						if (a + c <= max)
						{
							problem = $"{a} + X = {a + c}";
						}
						else if (a - c >= min)
						{
							problem = $"{a} - X = {a - c}";
						}
						break;
				}

				if (problem != null)
				{
					problems.Add(problem);
				}
			}

			return problems;
		}

		private static Document CreatePdfDocument(List<string> problems)
		{
			var document = new Document();
			var section = document.AddSection();
			section.PageSetup.PageFormat = PageFormat.Letter;

			int problemsPerPage = 40;
			int problemsPerColumn = 20;

			var table = section.AddTable();
			table.Borders.Width = 0;

			var column1 = table.AddColumn(Unit.FromCentimeter(8)); // Increase the width of the first column
			var column2 = table.AddColumn(Unit.FromCentimeter(8)); // Adjust the width of the second column

			for (int i = 0; i < problems.Count; i += problemsPerPage)
			{
				for (int j = 0; j < problemsPerPage && i + j < problems.Count; j += 2)
				{
					var row = table.AddRow();
					row.Cells[0].AddParagraph(problems[i + j]).Format.Font.Size = 18;
					row.Cells[0].AddParagraph(""); // Add an empty paragraph for spacing

					if (i + j + 1 < problems.Count)
					{
						row.Cells[1].AddParagraph(problems[i + j + 1]).Format.Font.Size = 18;
						row.Cells[1].AddParagraph(""); // Add an empty paragraph for spacing
					}
				}

				if (i + problemsPerPage < problems.Count)
				{
					section.AddPageBreak();
					table = section.AddTable();
					table.Borders.Width = 0;
					column1 = table.AddColumn(Unit.FromCentimeter(8)); // Ensure the column width is consistent
					column2 = table.AddColumn(Unit.FromCentimeter(8)); // Ensure the column width is consistent
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