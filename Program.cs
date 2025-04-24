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

// Generate Dummy Data if the database is empty
await GenerateDummyDataAsync();

// Get all of the available Categories from the database
var availableCategories = await service.GetAllCategoriesAsync();

// Preload all data from the database
// var availableDrugs = await GetAllDrugsAsync([availableCategories.First()]);
// var availableDrugs = await GetAllDrugsAsync(availableCategories);
var availableDrugs = await GetAllDrugsAsync([]);

// Obtain the Drugs from User Input
var selectedDrugs = await GetUserInputSelectedDrugsAsync();
if (!selectedDrugs.Any())
{
    Console.WriteLine("No drugs selected. Exiting...");
    return;
}

// Use the Drugs selected to display the Drug Information
await ShowDrugInformationAsync(selectedDrugs);

// Save all of the Drug Information to the clipboard
await SaveToClipboardAsync(selectedDrugs);

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

async Task GenerateDummyDataAsync()
{
    if (await service.HasDrugsAsync())
    {
        Console.WriteLine("The database already has data. Skipping dummy data generation.");
        return;
    }

    Console.WriteLine("Generating dummy data...");
    var random = new Random();

    var drugCount = 100;
    var sideEffectCount = 100;
    var categoryCount = 10;
    var maxDrugSideEffectCount = 10;
    var maxDrugCategoryCount = 2;

    for (int i = 1; i <= drugCount; i++)
    {
        await service.CreateDrugAsync(new Drug(i, $"Drug{i}"));
    }

    for (int i = 1; i <= sideEffectCount; i++)
    {
        await service.CreateSideEffectAsync(new SideEffect(i, $"SideEffect{i}"));
    }

    for (int i = 1; i <= categoryCount; i++)
    {
        await service.CreateCategoryAsync(new Category(i, $"Category{i}", (ConsoleColor)i));
    }

    for (int i = 0; i < drugCount; i++)
    {
        var rand = random.Next(0, maxDrugSideEffectCount + 1);
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

    for (int i = 0; i < drugCount; i++)
    {
        var rand = random.Next(0, maxDrugCategoryCount + 1);
        var existingCategoryIds = new List<int>();
        for (int j = 0; j < rand; j++)
        {
            int categoryId;
            do
            {
                categoryId = random.Next(1, categoryCount + 1);
            } while (existingCategoryIds.Contains(categoryId));

            existingCategoryIds.Add(categoryId);

            await service.CreateDrugCategoryLinkAsync(new DrugCategoryLink(0, i, categoryId));
        }
    }

    Console.WriteLine("Dummy data generated successfully.");
}

void Print<T>(T data)
{
    Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
}

void DisplayDrugWithCategoryColor(DrugFullInformation drug, bool hovered, bool endLine)
{
    // Highlight the hovered item
    if (hovered)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
        Console.Write(">>");
        Console.ResetColor();
    }

    // Build the base string
    var str = $"{(hovered ? "" : "  ")}{drug.DrugName}";

    // Handle categories
    if (drug.Categories.Any())
    {
        Console.Write(str);
        Console.Write(" (");

        foreach (var category in drug.Categories)
        {
            if (category.ColorCode.HasValue)
            {
                Console.ForegroundColor = category.ColorCode.Value;
            }
            Console.Write($"*");
            Console.ResetColor();
        }

        Console.Write(")");
    }
    else
    {
        Console.Write(str);
    }

    // End the line if required
    if (endLine)
    {
        Console.WriteLine();
    }
}

async Task<DrugFullInformation?> GetUserInputDrugAsync(IEnumerable<DrugFullInformation> selectedDrugs)
{
    string input = string.Empty;
    int selectedIndex = 0;

    while (true)
    {
        RenderInputScreen(input, selectedDrugs);

        var drugs = FilterDrugsByInput(input);

        RenderDrugList(drugs, selectedDrugs, selectedIndex);

        var key = Console.ReadKey(true);

        if (HandleKeyPress(key, ref input, ref selectedIndex, drugs, out var selectedDrug))
        {
            return selectedDrug;
        }
    }
}

void RenderInputScreen(string input, IEnumerable<DrugFullInformation> selectedDrugs)
{
    Console.Clear();
    Console.WriteLine("Type to search for a drug (or press 'Esc' to quit):");

    if (selectedDrugs.Any())
    {
        Console.Write("Selected: ");
        foreach (var selectedDrug in selectedDrugs)
        {
            DisplayDrugWithCategoryColor(selectedDrug, false, false);
        }
        Console.WriteLine();
    }

    Console.WriteLine($"Search: {input}");
    RenderAvailableCategories();
}

void RenderAvailableCategories()
{
    foreach (var category in availableCategories)
    {
        if (category.ColorCode.HasValue)
        {
            Console.ForegroundColor = category.ColorCode.Value;
        }
        Console.Write($"{category.Name}");
        Console.ResetColor();
        Console.Write(" ");
    }
    Console.WriteLine();
}

List<DrugFullInformation> FilterDrugsByInput(string input)
{
    return string.IsNullOrWhiteSpace(input)
        ? availableDrugs.ToList()
        : availableDrugs.Where(t => t.Like(input)).ToList();
}

void RenderDrugList(List<DrugFullInformation> drugs, IEnumerable<DrugFullInformation> selectedDrugs, int selectedIndex)
{
    int consoleHeight = Console.WindowHeight - (selectedDrugs.Any() ? 7 : 6);
    int startIndex = Math.Max(0, selectedIndex - consoleHeight / 2);
    int endIndex = Math.Min(drugs.Count, startIndex + consoleHeight);

    Console.WriteLine("\nUse the arrow keys to navigate and press Enter to select:");
    for (int i = startIndex; i < endIndex; i++)
    {
        DisplayDrugWithCategoryColor(drugs[i], i == selectedIndex, true);
    }
}

bool HandleKeyPress(ConsoleKeyInfo key, ref string input, ref int selectedIndex, List<DrugFullInformation> drugs, out DrugFullInformation? selectedDrug)
{
    selectedDrug = null;

    if (key.Key == ConsoleKey.Escape)
    {
        return true;
    }
    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
    {
        input = input.Substring(0, input.Length - 1);
        selectedIndex = 0; // Reset selection
    }
    else if (key.Key == ConsoleKey.Enter && drugs.Any())
    {
        selectedDrug = drugs[selectedIndex];
        return true;
    }
    else if (key.Key == ConsoleKey.UpArrow)
    {
        selectedIndex = (selectedIndex == 0) ? drugs.Count - 1 : selectedIndex - 1;
    }
    else if (key.Key == ConsoleKey.DownArrow)
    {
        selectedIndex = (selectedIndex == drugs.Count - 1) ? 0 : selectedIndex + 1;
    }
    else if (!char.IsControl(key.KeyChar))
    {
        input += key.KeyChar;
        selectedIndex = 0; // Reset selection
    }

    return false;
}

async Task<IEnumerable<DrugFullInformation>> GetUserInputSelectedDrugsAsync()
{
    var drugs = new List<DrugFullInformation>();
    while (await GetUserInputDrugAsync(drugs) is DrugFullInformation drug)
    {
        if (!drugs.Any(t => t.DrugId == drug.DrugId))
        {
            drugs.Add(drug);
        }
    }
    return drugs;
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

async Task<IEnumerable<DrugFullInformation>> GetAllDrugsAsync(IEnumerable<Category> categories)
{
    return await service.GetAllDrugsAsync(categories);
}