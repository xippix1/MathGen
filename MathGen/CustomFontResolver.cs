using PdfSharp.Fonts;
using System;
using System.IO;

public class CustomFontResolver : IFontResolver
{
	public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
	{
		if (familyName.Equals("Courier", StringComparison.OrdinalIgnoreCase))
		{
			string fontFile = "cour.ttf"; // Change to the correct font file name
			if (isBold && isItalic)
				fontFile = "courbi.ttf";
			else if (isBold)
				fontFile = "courbd.ttf";
			else if (isItalic)
				fontFile = "couri.ttf";

			return new FontResolverInfo(fontFile);
		}

		// If the font family is not found, return the default font
		return new FontResolverInfo("cour.ttf");
	}

	public byte[] GetFont(string faceName)
	{
		string fontFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
		string fontFilePath = Path.Combine(fontFolder, faceName);

		if (!File.Exists(fontFilePath))
		{
			throw new InvalidOperationException($"Font '{faceName}' not found in the system fonts.");
		}

		return File.ReadAllBytes(fontFilePath);
	}
}