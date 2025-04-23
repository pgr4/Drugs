using Drugs.Library.Models;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Drugs.Library;
using TextCopy;

var services = new ServiceCollection();
ServiceExtensions.AddDrugLibrary(services);

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

// Resolve a repository
var service = serviceProvider.GetRequiredService<Service>();

async Task GenerateDummyDataAsync()
{
    var random = new Random();

    var drugCount = 100;
    var sideEffectCount = 100;
    var maxDrugSideEffectCount = 10;

    for (int i = 1; i <= drugCount; i++)
    {
        await service.CreateDrugAsync(new Drug(i, $"Drug{i}"));
    }

    for (int i = 1; i <= sideEffectCount; i++)
    {
        await service.CreateSideEffectAsync(new SideEffect(i, $"SideEffect{i}"));
    }

    for (int i = 0; i < drugCount; i++)
    {
        var rand = random.Next(0, maxDrugSideEffectCount);
        var existingSideEffectIds = new List<int>();
        for (int j = 0; j < rand; j++)
        {
            int sideEffectId;
            do
            {
                sideEffectId = random.Next(1, sideEffectCount + 1);
            } while (existingSideEffectIds.Contains(sideEffectId));

            existingSideEffectIds.Add(sideEffectId);

            await service.CreateDrugSideEffectLinkAsync(new DrugSideEffectLink(0, i, sideEffectId));
        }
    }
}

void Print<T>(T data)
{
    Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
}

async Task PrintDrugsLikeAsync(string name)
{
    Print(await service.GetDrugsBySimilarNameAsync(name));
}

async Task<Drug?> SelectDrugAsync(IEnumerable<Drug> selectedDrugs)
{
    string input = string.Empty;
    int selectedIndex = 0;

    while (true)
    {
        // Clear the console and render the input
        Console.Clear();
        Console.WriteLine("Type to search for a drug (or press 'Esc' to quit):");
        if (selectedDrugs.Any())
        {
            Console.WriteLine($"Selected: {string.Join(", ", selectedDrugs.Select(d => d.Name))}");
        }
        Console.WriteLine($"Search: {input}");

        // Fetch results based on the current input
        var results = string.IsNullOrWhiteSpace(input) ? new List<Drug>() : (await service.GetDrugsBySimilarNameAsync(input)).ToList();

        // Determine the visible range of items
        int consoleHeight = Console.WindowHeight - (selectedDrugs.Any() ? 6 : 5);
        int startIndex = Math.Max(0, selectedIndex - consoleHeight / 2);
        int endIndex = Math.Min(results.Count, startIndex + consoleHeight);

        // Render the visible portion of the list
        Console.WriteLine("\nUse the arrow keys to navigate and press Enter to select:");
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"> {results[i].Name}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {results[i].Name}");
            }
        }

        // Read user input
        var key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.Escape)
        {
            return null;
        }
        else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
        {
            input = input.Substring(0, input.Length - 1);
            selectedIndex = 0; // Reset selection
        }
        else if (key.Key == ConsoleKey.Enter && results.Any())
        {
            return results[selectedIndex];
        }
        else if (key.Key == ConsoleKey.UpArrow)
        {
            selectedIndex = (selectedIndex == 0) ? results.Count - 1 : selectedIndex - 1;
        }
        else if (key.Key == ConsoleKey.DownArrow)
        {
            selectedIndex = (selectedIndex == results.Count - 1) ? 0 : selectedIndex + 1;
        }
        else if (!char.IsControl(key.KeyChar))
        {
            input += key.KeyChar;
            selectedIndex = 0; // Reset selection
        }
    }
}

async Task<IEnumerable<Drug>> SelectDrugsAsync()
{
    var drugs = new List<Drug>();
    while (await SelectDrugAsync(drugs) is Drug drug)
    {
        if (!drugs.Any(t => t.DrugId == drug.DrugId))
        {
            drugs.Add(drug);
        }
    } 
    return drugs;
}

async Task<IEnumerable<DrugFullInformation>> GetDrugFullInformationAsync(IEnumerable<Drug> drugs)
{
    return await Task.WhenAll(drugs.Select(async drug => await service.GetDrugInformationAsync(drug)));
}

async Task ShowDrugInformationAsync(IEnumerable<DrugFullInformation> drugFullInformationList)
{
    Console.Clear();
    foreach (var drugFullInformation in drugFullInformationList) 
    {
        Print(drugFullInformation);
    }
}

async Task SaveToClipboardAsync(IEnumerable<DrugFullInformation> drugFullInformationList)
{
    var text = JsonSerializer.Serialize(drugFullInformationList, new JsonSerializerOptions { WriteIndented = true });

    if (string.IsNullOrEmpty(text))
    {
        Console.WriteLine("The text is empty or null. Nothing to copy.");
        return;
    }

    try
    {
        await ClipboardService.SetTextAsync(text); // Use TextCopy's ClipboardService
        Console.WriteLine("Text copied to clipboard successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to copy text to clipboard: {ex.Message}");
    }
}

if (!await service.HasDrugsAsync())
{
    Console.WriteLine("Generating dummy data...");
    await GenerateDummyDataAsync();
    Console.WriteLine("Dummy data generated successfully.");
}

// Obtain the Drugs from User Input
var drugs = await SelectDrugsAsync();

if (!drugs.Any())
{
    Console.WriteLine("No drugs selected. Exiting...");
    return;
}

// Get the Drug Information for the selected Drugs
var drugFullInformationList = await GetDrugFullInformationAsync(drugs);

// Use the Drugs selected to display the Drug Information
await ShowDrugInformationAsync(drugFullInformationList);

// Save all of the Drug Information to the clipboard
await SaveToClipboardAsync(drugFullInformationList);

Console.WriteLine("Press any key to exit...");
Console.ReadKey();