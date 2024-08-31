[Japanese Readme is here](README.md)

# ListView Extensions
WPF ListView can effectively show the contents of a collection, however it's hard to implement functions such as sorting by clicking on the header, right-clicking, double-clicking, and retrieving the selected items.  
ListView Extensions is a library that provides the support to easily implement above functions of ListView for all aspects of View, ViewModel and Model.

![](https://img.shields.io/badge/Nuget-1.1.0-blue?logo=nuget&style=plastic)
![](https://img.shields.io/badge/.NET_Framework-4.5.2-orange?logo=.net&style=plastic)
![](https://img.shields.io/badge/.NET_Core-3.1-orange?logo=.net&style=plastic)
![](https://img.shields.io/badge/.NET-6-orange?logo=.net&style=plastic)

![Screenshot of ListView Extensions](https://raw.githubusercontent.com/EH500-Kintarou/ListViewExtensions/master/Images/SampleScreenshot.png)

## Key Features
- Sorting
- Data biding of multiple item selection
- Notifications to ViewModel of double-clicking (Command and Method binding)
- Thread-safe collection operations

![Class Relationship Overview](https://raw.githubusercontent.com/EH500-Kintarou/ListViewExtensions/master/Images/ClassRelationshipOverview_ja.png)

## Required Environment
- .NET Core 3.1 or more / .NET Framework 4.5.2 or more

## Getting Started
### 1. Get via Nuget
![](https://img.shields.io/badge/Nuget-1.1.0-blue?logo=nuget&style=plastic) https://www.nuget.org/packages/ListViewExtensions

### 2. Add XAML namespace
Add namespace of "http://schemas.eh500-kintarou.com/ListViewExtensions" into your XAML code.

```xaml
<Window x:Class="ListViewExtensionsTest.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
        xmlns:ei="http://schemas.microsoft.com/expression/2010/interactions"
		xmlns:lv="http://schemas.eh500-kintarou.com/ListViewExtensions"
        Title="MainWindow" Height="480" Width="640">
```

### 3. Implementing View
Create ListView in the View. Add SortableGridViewColumn instead of GridViewColumn to be able to sort by cliking a column.

```xaml
<ListView ItemsSource="{Binding People}" >
	<ListView.View>
        <GridView>
            <lv:SortableGridViewColumn Width="120" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Name}" Header="Name" />
            <lv:SortableGridViewColumn Width="150" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Pronunciation}" Header="Pronunciation" />
            <lv:SortableGridViewColumn Width="70"  SortableSource="{Binding People}" DisplayMemberBinding="{Binding Age}" Header="Age" />
            <lv:SortableGridViewColumn Width="120" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Birthday}" Header="Birthday" />
            <lv:SortableGridViewColumn Width="120" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Height}" Header="Height" />
		</GridView>
	</ListView.View>
</ListView>
```

### 4. Implementing Model
Instantiate SortableObservableCollection class in the Model. This is a thread-safe collection that corresponds the contents shown in the ListView.

```cs
public SortableObservableCollection<PersonModel> People { get; } = new SortableObservableCollection<PersonModel>();
```

### 5. Implementing ViewModel
Instantiate ListViewViewModel class in the ViewModel. It dispatches change notifications of SortableObservableCollection in the Model on the UI thread, and also has commands such as sorting and selection to receive the actions which is generated in the View.

```cs
People = new ListViewViewModel<PersonViewModel, PersonModel>(model.People, person => new PersonViewModel(person), DispatcherHelper.UIDispatcher);
```

### 6. Check sample code out
This repository contains [a sample project](https://github.com/EH500-Kintarou/ListViewExtensions/tree/master/Sample). It will make you more clear how to use it.

## Project URL
![](https://img.shields.io/badge/Github-1.1.0-green?logo=github&style=plastic) https://github.com/EH500-Kintarou/ListViewExtensions  
![](https://img.shields.io/badge/Nuget-1.1.0-blue?logo=nuget&style=plastic) https://www.nuget.org/packages/ListViewExtensions  
![](https://img.shields.io/badge/Blogger-1.1.0-orange?logo=blogger&style=plastic) https://days-of-programming.blogspot.com/search/label/ListView%20Extensions

## Version History
### ver.1.1.0 (31-August-2024)
- Target framework was changed to .NET Framework 4.5.2, .NET Core 3.1 and .NET 6
- Improved ListView header support
  - Added SortableGridViewColumnHeader and SortableGridViewColumn
  - Obsolete attribute is added to SortedHeader
- Parameters of Sort method in ISortableObservableCollection was changed
- Refactored of the source code, enabled Nullable, added Unit Test etc...

### ver.1.0.1 (24-March-2018)
- Deleted the classes which has Obsolete attribute
- Non-Generic interfaces of SyncedObservableCollection are changed to Explicit Interface Implementation

### ver.1.0.0 (26-November-2017)
- First Release
