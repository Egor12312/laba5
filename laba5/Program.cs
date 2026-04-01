using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program {
  static void Main()
  {
    string directoryPath;
    string errorWordsFilePath;
    bool directoryExists;
    string[] allFiles;
    int fileIndex;
    int totalFilesCount;
    string currentFilePath;
    Dictionary<string, string> errorWordsDictionary;

    directoryPath = @"C:\Users\Asus\Documents\TestFiles";
    errorWordsFilePath = Path.Combine(directoryPath, "errors.txt");
    errorWordsDictionary = LoadErrorWords(errorWordsFilePath);
    directoryExists = Directory.Exists(directoryPath);

    if (directoryExists == false)
    {
      Console.WriteLine("Directory does not exist!");
      return;
    }

    allFiles = Directory.GetFiles(directoryPath, "*.txt");
    totalFilesCount = allFiles.Length;

    for (fileIndex = 0; fileIndex < totalFilesCount; ++fileIndex)
    {
      currentFilePath = allFiles[fileIndex];

      ProcessFile(currentFilePath, errorWordsDictionary);
    }
    Console.WriteLine("Processing completed!");
  }

  static Dictionary<string, string> LoadErrorWords(string filePath)
  {
    Dictionary<string, string> errorDictionary;
    bool fileExists;
    string[] allLines;
    int lineIndex;
    int linesCount;
    string currentLine;
    string[] lineParts;
    bool hasSeparator;
    string wrongWord;
    string correctWord;
    int minimumPartsCount;
    int wrongWordIndex;
    int correctWordIndex;

    wrongWordIndex = 0;
    correctWordIndex = 1;
    minimumPartsCount = 2;
    errorDictionary = new Dictionary<string, string>();
    fileExists = File.Exists(filePath);

    if (fileExists == false)
    {
      Console.WriteLine("Error words file not found. Creating default dictionary.");

      errorDictionary.Add("привет-привет-пирвет", "привет");
      errorDictionary.Add("здраствуйте", "здравствуйте");
      errorDictionary.Add("пожалуста", "пожалуйста");
      errorDictionary.Add("спосибо", "спасибо");
      return errorDictionary;
    }

    allLines = File.ReadAllLines(filePath);
    linesCount = allLines.Length;

    for (lineIndex = 0; lineIndex < linesCount; ++lineIndex)
    {
      currentLine = allLines[lineIndex];

      if (string.IsNullOrWhiteSpace(currentLine))
      {
        continue;
      }

      lineParts = currentLine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
      hasSeparator = (lineParts.Length >= minimumPartsCount);

      if (hasSeparator)
      {
        wrongWord = lineParts[wrongWordIndex];
        correctWord = lineParts[correctWordIndex];

        if (errorDictionary.ContainsKey(wrongWord) == false)
        {
          errorDictionary.Add(wrongWord, correctWord);
        }
      }
    }

    return errorDictionary;
  }

  static void ProcessFile(string filePath, Dictionary < string, string > errorWords)
  {
    string fileContent;
    string correctedContent;
    bool fileExists;

    fileExists = File.Exists(filePath);

    if (fileExists == false)
    {
      Console.WriteLine("File not found: " + filePath);
      return;
    }
    fileContent = File.ReadAllText(filePath);
    correctedContent = FixErrorWords(fileContent, errorWords);
    correctedContent = FixPhoneNumbers(correctedContent);

    File.WriteAllText(filePath, correctedContent);
    Console.WriteLine("Processed: " + filePath);
  }

  static string FixErrorWords(string text, Dictionary<string, string > errorWords)
  {
    string result;
    string wrongPattern;
    string correctWord;

    result = text;

    foreach (KeyValuePair < string, string > errorPair in errorWords)
    {
      wrongPattern = errorPair.Key;
      correctWord = errorPair.Value;
      result = result.Replace(wrongPattern, correctWord);
    }
    return result;
  }

  static string FixPhoneNumbers(string text)
  {
    string pattern;
    string replacement;
    string result;
    Regex regex;

    pattern = @"\((\d{3})\)\s(\d{3})-(\d{2})-(\d{2})";
    replacement = "+380 $1 $2 $3 $4";
    regex = new Regex(pattern);
    result = regex.Replace(text, replacement);

    return result;
  }
}