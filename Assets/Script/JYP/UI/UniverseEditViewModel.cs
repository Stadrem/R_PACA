using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class UniverseEditViewModel : INotifyPropertyChanged
{
    private string title;
    private string genre;
    private string content;
    private List<string> tags = new List<string>();
    private List<ObjectiveListViewController.ObjectiveListEntry> objectives = new();
    private List<CharactersEntryController.CharacterEntry> characters = new();
    private DateTime createdDate;
    // private List<string> backgrounds = new List<string>();

    public string Title
    {
        get => title;
        set => SetField(ref title, value);
    }

    public string Genre
    {
        get => genre;
        set => SetField(ref genre, value);
    }

    public string Content
    {
        get => content;
        set => SetField(ref content, value);
    }

    public List<string> Tags
    {
        get => tags;
        set => SetField(ref tags, value);
    }

    public List<ObjectiveListViewController.ObjectiveListEntry> Objectives
    {
        get => objectives;
        set => SetField(ref objectives, value);
    }

    public List<CharactersEntryController.CharacterEntry> Characters
    {
        get => characters;
        set => SetField(ref characters, value);
    }

    public DateTime CreatedDate
    {
        get => createdDate;
        set => SetField(ref createdDate, value);
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}