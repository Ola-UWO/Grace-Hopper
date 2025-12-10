using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
//using System.Windows.Forms;

namespace ReeveUnionManager.Models;

public class ManagerLogObject(BusinessLogic businessLogic)
{
    private readonly BusinessLogic _businessLogic = businessLogic;

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

    public async Task<string> FormatDocument()
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

            Table table = new Table(
                new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.None },
                        new BottomBorder { Val = BorderValues.None },
                        new LeftBorder { Val = BorderValues.None },
                        new RightBorder { Val = BorderValues.None },
                        new InsideHorizontalBorder { Val = BorderValues.None },
                        new InsideVerticalBorder { Val = BorderValues.None }
                    )
                )
            );

            table.Append(
                new TableRow(
                    new TableCell(
                        new TableCellProperties(
                            new TableCellMargin(
                                new RightMargin() { Width = "250" }
                            )
                        ),
                        new Paragraph(new Run(new Text("Day of the week: " + ShiftDetailsDayOfWeek)))
                    ),
                    new TableCell(
                        new TableCellProperties(
                            new TableCellMargin(
                                new RightMargin() { Width = "250" }
                            )
                        ),
                        new Paragraph(new Run(new Text("Date: " + ShiftDetailsDate)))
                    )
                )
            );

            table.Append(
                new TableRow(
                    new TableCell(
                        new TableCellProperties(
                            new TableCellMargin(
                                new RightMargin() { Width = "250" }
                            )
                        ),
                        new Paragraph(new Run(new Text("BM Name: " + ShiftDetailsName)))
                    ),
                    new TableCell(
                        new TableCellProperties(
                            new TableCellMargin(
                                new RightMargin() { Width = "250" }
                            )
                        ),
                        new Paragraph(new Run(new Text("    Shift: " + ShiftStartTime + " " + ShiftEndTime)))
                    )
                )
            );

            body.Append(table);

            Paragraph dashLine = new(new Run(new Text("-----------------------------------------------------------------------------------------------------------------")));
            body.Append(dashLine);

            Paragraph eventSupport = new(new Run(new Text("Event Support / Reservations: ")));
            Paragraph eventSupportInfo = new(new Run(new Text($"• {EventSupportChangesName} {EventSupportChangesTime} {EventSupportChangesLocation}")));
            Paragraph eventSupportDetails = new(new Run(new Text($"• {EventSupportChangesDetails}")));
            
            body.Append(eventSupport);
            body.Append(eventSupportInfo);
            body.Append(eventSupportDetails);

            Paragraph setsForTomorrow = new(new Run(new Text("Sets for Tomorrow: ")));
            Paragraph setsForTomorrowDetails = new(new Run(new Text($"• {RoomSetsNotes}")));
            
            body.Append(setsForTomorrow);
            body.Append(setsForTomorrowDetails);

            Paragraph avTech = new(new Run(new Text("AV / Technology: ")));
            Paragraph avTechDetails = new(new Run(new Text($"• {AvTechnologyNotes}")));
            
            body.Append(avTech);
            body.Append(avTechDetails);

            Paragraph foodService = new(new Run(new Text("Food Service: ")));
            Paragraph foodServiceInfo = new(new Run(new Text($"• {FoodServiceCategory} {FoodServiceLocation}")));
            Paragraph foodServiceDetails = new(new Run(new Text($"• {FoodServiceDescription}")));
            
            body.Append(foodService);
            body.Append(foodServiceInfo);
            body.Append(foodServiceDetails);

            Paragraph retailServices = new(new Run(new Text("Retail Services: ")));
            Paragraph retailServicesDetails = new(new Run(new Text($"• {RetailServicesNotes}")));
            
            body.Append(retailServices);
            body.Append(retailServicesDetails);

            Paragraph custodial = new(new Run(new Text("Maintenence / Custodial: ")));
            Paragraph custidialDetails = new(new Run(new Text($"• {CustiodialNotes}")));
            
            body.Append(custodial);
            body.Append(custidialDetails);

            Paragraph misc = new(new Run(new Text("Miscellaneous: ")));
            Paragraph miscDetails = new(new Run(new Text($"• {MiscNotes}")));
            
            body.Append(misc);
            body.Append(miscDetails);

            Paragraph frontDeskTasks = new(new Run(new Text("Front Desk Tasks Done: ")));
            Paragraph frontDeskTasksDetails = new(new Run(new Text($"• {FrontDeskTasksNotes}")));
            
            body.Append(frontDeskTasks);
            body.Append(frontDeskTasksDetails);

            Paragraph hourBeforeClosing = new(new Run(new Text($"Number of guests 1 hour before close: {NumberOfGuestsHourBeforeClosing}   Number of guests asked to leave at closing: {NumberofGuestsAtClosing}")));
            body.Append(hourBeforeClosing);

            Paragraph eventLogTitle = new(new Run(new Text("Event Log:")));
            body.Append(eventLogTitle);

            Table eventTable = new();

            TableProperties eventTblProps = new(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Apples },
                    new BottomBorder { Val = BorderValues.Apples },
                    new LeftBorder { Val = BorderValues.Apples },
                    new RightBorder { Val = BorderValues.Apples },
                    new InsideHorizontalBorder { Val = BorderValues.Apples },
                    new InsideVerticalBorder { Val = BorderValues.Apples }
                )
            );

            eventTable.AppendChild(eventTblProps);

            TableRow eventHeader = new();
            eventHeader.Append(
                CreateCell("Event", 3000, true),
                CreateCell("Event Time", 1500),
                CreateCell("Check-in?", 1500),
                CreateCell("Notes", 3250)
            );
            eventTable.Append(eventHeader);

            var events = await _businessLogic.GetAllEvents();

            foreach (var ev in events)
            {
                TableRow row = new();
                row.Append(
                    CreateCell(ev.EventTitle ?? "", 3000),
                    CreateCell(ev.EventDateAndTime ?? "", 1500),
                    CreateCell(ev.EventCheckIn ? "Yes" : "No", 1500),
                    CreateCell(ev.EventNotes ?? "", 3250)
                );
                eventTable.Append(row);
            }

            body.Append(eventTable);

            Paragraph spacer = new(new Run(new Text("")));
            body.Append(spacer);

            Paragraph phoneLogTitle = new(new Run(new Text("Building Manager Phone Log")));
            body.Append(phoneLogTitle);
            
            Table callLogTable = new();

            TableProperties callTblProps = new(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Apples },
                    new BottomBorder { Val = BorderValues.Apples },
                    new LeftBorder { Val = BorderValues.Apples },
                    new RightBorder { Val = BorderValues.Apples },
                    new InsideHorizontalBorder { Val = BorderValues.Apples },
                    new InsideVerticalBorder { Val = BorderValues.Apples }
                )
            );

            callLogTable.AppendChild(callTblProps);

            TableRow callLogHeader = new();
            callLogHeader.Append(
                CreateCell("Time", 1750),
                CreateCell("Reason", 7500)
            );
            callLogTable.Append(callLogHeader);

            body.Append(callLogTable);

            mainPart.Document = new Document(body);

            mainPart.Document.Save();
        }
        string newFilePath = Path.Combine(FileSystem.AppDataDirectory, "ManagerLog.docx");
        return newFilePath;
    }

    private static TableCell CreateCell(string text, int width, bool bold = false)
    {
        RunProperties rp = new();
        if (bold)
        {
            rp.Append(new Bold());
        }

        Run run = new(rp, new Text(text));

        Paragraph paragraph = new(run);

        return new TableCell(
            new TableCellProperties(
                new TableCellWidth
                {
                    Type = TableWidthUnitValues.Dxa,
                    Width = width.ToString()
                }
            ),
            paragraph
        );
    }
}
