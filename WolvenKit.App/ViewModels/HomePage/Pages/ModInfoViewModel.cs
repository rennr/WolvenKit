using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.App.Models;
using WolvenKit.Core.Interfaces;

namespace WolvenKit.App.ViewModels.HomePage.Pages;

public partial class ModInfoViewModel : ObservableObject
{
    private readonly ILoggerService _logger;

    public ModInfoViewModel(ModInfo mod, string path, ILoggerService settings)
    {
        _logger = settings;
        Mod = mod;
        Path = path;

        Folder = System.IO.Path.GetFileName(Path);


    }

    public ModInfo Mod { get; init; }

    public string Path { get; init; }
    public string Folder { get; init; }

    [ObservableProperty] private int _loadOrder;

    public bool IsEnabled { get; set; }

    public string Name => Mod.Name;



}
