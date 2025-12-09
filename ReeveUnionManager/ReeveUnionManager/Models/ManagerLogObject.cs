using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace ReeveUnionManager.Models;

public class ManagerLogObject
{
    public string? ShiftDetailsName {get; set;}
    public string? ShiftDetailsDate {get; set;}
    public string? ShiftDetailsDayOfWeek {get; set;}
    public string? ShiftStartTime {get; set;}
    public string? ShiftEndTime {get; set;}
    public string? EventSupportChangesName {get; set;}
    public string? EventSupportChangesTime {get; set;}
    public string? EventSupportChangesLocation {get; set;}
    public string? EventSupportChangesDetails {get; set;}
    public object? EventSupportChangesPictures {get; set;}
    public string? RoomSetsNotes {get; set;}
    public object? RoomSetsPictures {get; set;}
    public string? AvTechnologyNotes {get; set;}
    public object? AvTechnologyPictures {get; set;}
    public string? FoodServiceCategory {get; set;}
    public string? FoodServiceLocation {get; set;}
    public string? FoodServiceDescription {get; set;}
    public object? FoodServicePictures {get; set;}
    public string? RetailServicesNotes {get; set;}
    public object? RetailServicesPictures {get; set;}
    public string? CustiodialNotes {get; set;}
    public object? CustiodialPictures {get; set;}
    public string? MiscNotes {get; set;}
    public object? MiscPictures {get; set;}
    public string? FrontDeskTasksNotes {get; set;}
    public object? FrontDeskTasksPictures {get; set;}
    public string? NumberOfGuestsDate {get; set;}
    public string? NumberOfGuestsHourBeforeClosing {get; set;}
    public string? NumberofGuestsAtClosing {get; set;}
    public string? NumberOfGuestsNotes {get; set;}

    public string formatDocument()
    {
        string filePath = Path.Combine(FileSystem.AppDataDirectory, "ManagerLog.docx");

        // Prevent OpenXML from crashing on existing file
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(
            filePath, WordprocessingDocumentType.Document))
        {
            MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();

            Body body = new Body();
            foreach (var prop in typeof(ManagerLogObject).GetProperties())
            {
                object? value = null;

                string label = prop.Name;
                value = prop.GetValue(this);
                string textToWrite;

                if (value == null) 
                {
                    textToWrite = $"{label}: (none)"; 
                }
                else if (value is string strValue)
                { 
                    textToWrite = $"{label}: {strValue}";
                } 
                else 
                {
                    // for picture fields / object fields
                    textToWrite = $"{label}: [Object data present]";
                }
                
                Paragraph para = new Paragraph(new Run(new Text(textToWrite)));
                body.Append(para);
            }
            mainPart.Document = new Document(body);

            mainPart.Document.Save();
        }
        string newFilePath = Path.Combine(FileSystem.AppDataDirectory, "ManagerLog.docx");
        return newFilePath;
    }
}
